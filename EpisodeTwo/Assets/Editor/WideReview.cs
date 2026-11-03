#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static partial class WideReview
{
    sealed class Step { public double delay; public Action action; public bool gameplay; }
    static readonly Queue<Step> steps=new Queue<Step>();
    static string output;
    static double since,started;
    static bool running,waitingGameplay;
    static bool EpisodeTwo => File.Exists(Path.Combine(Application.dataPath,"Editor/BuildEpisodeTwo.cs"));
    static WideReview() { if(SessionState.GetBool("WideReview",false))Bind(); if(SessionState.GetBool("WideReviewMotion",false))BindMotion(); }
    static void Bind()
    {
        EditorApplication.update-=Tick;EditorApplication.update+=Tick;
        EditorApplication.playModeStateChanged-=Mode;EditorApplication.playModeStateChanged+=Mode;
    }
    public static void Run()
    {
        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
        foreach(string name in new[]{"LaunchAssets","UiTransparencyUpgrade","ModernizationAudit"})
        {
            Type type=assembly.GetType(name);
            if(type!=null)type.GetMethod(name=="ModernizationAudit"?"Run":"Prepare").Invoke(null,null);
        }
        string[] args=Environment.GetCommandLineArgs();int index=Array.IndexOf(args,"-reviewOutput");
        output=index<0?Path.GetFullPath("../wide-review"):args[index+1];Directory.CreateDirectory(output);
        SessionState.SetString("WideReviewOutput",output);SessionState.SetBool("WideReview",true);
        SessionState.SetBool("WideReviewUiOnly",Array.IndexOf(args,"-reviewUiOnly")>=0);
        SetSize(540,1200);EditorSceneManager.OpenScene("Assets/Scenes/Screens/MainMenu.unity");
        Bind();EditorApplication.isPlaying=true;
    }
    static void SetSize(int width,int height)
    {
        var assembly=typeof(Editor).Assembly;var sizesType=assembly.GetType("UnityEditor.GameViewSizes");
        var singleton=typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instance=singleton.GetProperty("instance",BindingFlags.Public|BindingFlags.Static).GetValue(null);
        var group=sizesType.GetMethod("GetGroup").Invoke(instance,new[]{Enum.Parse(assembly.GetType("UnityEditor.GameViewSizeGroupType"),"Android")});
        string label="WideReview "+width+"x"+height;
        var labels=(string[])group.GetType().GetMethod("GetDisplayTexts").Invoke(group,null);
        int sizeIndex=Array.FindIndex(labels,l=>l.Contains(label));
        if(sizeIndex<0)
        {
            var size=Activator.CreateInstance(assembly.GetType("UnityEditor.GameViewSize"),BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,new object[]{Enum.Parse(assembly.GetType("UnityEditor.GameViewSizeType"),"FixedResolution"),width,height,label},null);
            group.GetType().GetMethod("AddCustomSize").Invoke(group,new[]{size});
            sizeIndex=(int)group.GetType().GetMethod("GetTotalCount").Invoke(group,null)-1;
        }
        var view=EditorWindow.GetWindow(assembly.GetType("UnityEditor.GameView"));
        view.GetType().GetProperty("selectedSizeIndex",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic).SetValue(view,sizeIndex);
    }
    static void Add(double delay,Action action,bool gameplay=false) { steps.Enqueue(new Step{delay=delay,action=action,gameplay=gameplay}); }
    static void Mode(PlayModeStateChange mode)
    {
        if(mode!=PlayModeStateChange.EnteredPlayMode)return;
        output=SessionState.GetString("WideReviewOutput","");running=true;steps.Clear();
        since=started=EditorApplication.timeSinceStartup;
        if(SessionState.GetBool("WideReviewUiOnly",false))
        {
            Add(3,()=>SceneManager.LoadScene("Leaderboard"));
            Add(3,()=>{
                Save("leaderboard-empty");
                foreach(var camera in Camera.allCameras)Debug.Log("WIDE_UI_CAMERA "+camera.name+" rect="+camera.rect+" aspect="+camera.aspect+" size="+camera.orthographicSize);
                foreach(var entry in UnityEngine.Object.FindObjectsByType<LeaderboardEntry>())Debug.Log("WIDE_ENTRY "+entry.transform.localScale);
                UnityEngine.Object.FindAnyObjectByType<LeaderboardView>().SetList(new List<LeaderboardEntryVO>{new LeaderboardEntryVO{name="LOCAL TEST",time=42.125f,star=2,date=DateTime.Now}},false);
            });
            Add(1,()=>{Save("leaderboard-fixture");SceneManager.LoadScene("LevelSelect");});
            Add(3,()=>{ReviewSelection();Save("level-select-9x20");SetSize(540,810);});
            Add(1,()=>{ReviewSelection();Save("level-select-2x3");SetSize(540,960);});
            Add(1,()=>{ReviewSelection();Save("level-select-9x16");SetSize(540,1440);});
            Add(1,()=>{ReviewSelection();Save("level-select-9x24");Debug.Log("WIDE_UI_REVIEW PASS");Finish(0);});
            return;
        }
        Add(.4,()=>Save("launch"));
        Add(4.6,()=>{AssertMenu();Save("menu");Load(1);});
        Add(.35,()=>{if(UnityEngine.Object.FindAnyObjectByType<LoadLevelScreen>()!=null)Save("loading");});
        Add(25,()=>{AssertGameplay();Save("gameplay-9x20");waitingGameplay=false;Hud.Instance.Pause();},true);
        Add(.6,()=>{Save("pause-9x20");Freeze();SetSize(540,810);});
        Add(1,()=>{AssertGameplay();Save("gameplay-2x3");SetSize(540,960);});
        Add(1,()=>{AssertGameplay();Save("gameplay-9x16");SetSize(540,1200);});
        Add(1,()=>{AssertGameplay();SetSize(540,1440);});
        Add(1,()=>{AssertGameplay();Save("gameplay-9x24");Hud.Instance.Pause();});
        Add(.6,()=>{Save("pause-9x24");Time.timeScale=1;SetSize(540,1200);SceneManager.LoadScene("Leaderboard");});
        Add(3,()=>{Save("leaderboard");SceneManager.LoadScene("MainMenu");});
        Add(3,()=>{AssertMenu();InvokeMenu("onShowOption");});
        Add(1,()=>{Save("options");SceneManager.LoadScene("MainMenu");});
        if(EpisodeTwo)
        {
            Add(3,()=>Load(2));
            Add(25,()=>{AssertGameplay();Save("level-2");Load(17);},true);
            Add(25,()=>{AssertGameplay();Save("level-17");},true);
        }
        else
        {
            Add(3,()=>InvokeMenu("onEpisodeTwo",true));
            Add(1,()=>Save("episode-two-alert"));
        }
        Add(.1,()=>{Debug.Log("WIDE_REVIEW PASS: menus/loading/pause/leaderboards; 2:3, 9:16, 9:20, 9:24; 320-unit gameplay width; corner and cutout bounds");File.WriteAllText(Path.Combine(output,"PASS.txt"),"Verified native 320-unit horizontal gameplay and HUD bounds at 540x810, 540x960, 540x1200 and 540x1440.\n");Finish(0);});
    }
    static void Tick()
    {
        if(!EditorApplication.isPlaying || !running)return;
        try
        {
            if(EditorApplication.timeSinceStartup-started>240)throw new Exception("WideReview timeout");
            Application.runInBackground=true;
            LegacyScreenFit.PreviewSafeArea=new Rect(40,40,Screen.width-80,Screen.height-92);
            LegacyScreenFit.PreviewCutouts=new[]{new Rect(Screen.width*.5f-55,Screen.height-50,110,50)};
            if(waitingGameplay && Hud.Instance!=null && Time.timeScale==0 && !ImageRunning())Hud.Instance.onResumeGame();
            if(steps.Count==0)return;
            Step step=steps.Peek();
            if(EditorApplication.timeSinceStartup-since<step.delay)return;
            if(step.gameplay && (World.vessel==null || World.cameraFollow==null || ImageRunning()))return;
            steps.Dequeue();step.action();since=EditorApplication.timeSinceStartup;
        }
        catch(Exception e){Save("failure");Debug.LogException(e);File.WriteAllText(Path.Combine(output,"FAIL.txt"),e.ToString());Finish(1);}
    }
    static bool ImageRunning()
    {
        var field=typeof(Hud).GetField("ImageActionRunning",BindingFlags.Public|BindingFlags.Static);
        return field!=null && (bool)field.GetValue(null);
    }
    static void Load(int level)
    {
        Time.timeScale=1;waitingGameplay=true;LoadLevelScreen.LevelToLoad=level;SceneManager.LoadScene("LoadLevel");
    }
    static void Freeze()
    {
        waitingGameplay=false;
        foreach(var pause in UnityEngine.Object.FindObjectsByType<PauseScreen>(FindObjectsInactive.Include))pause.gameObject.SetActive(false);
        Time.timeScale=0;
    }
    static void InvokeMenu(string method,bool argument=false)
    {
        var menu=UnityEngine.Object.FindAnyObjectByType<MainMenuScreen>();
        typeof(MainMenuScreen).GetMethod(method,BindingFlags.NonPublic|BindingFlags.Instance).Invoke(menu,argument?new object[]{null}:null);
    }
    static void Check(bool condition,string message) { if(!condition)throw new Exception(message); }
    static void ReviewSelection()
    {
        Camera camera=GameObject.Find("Hud Camera").GetComponent<Camera>();
        Check(camera.rect==new Rect(0,0,1,1),"Old framed viewport still active");
        foreach(var renderer in UnityEngine.Object.FindObjectsByType<Renderer>())
            if(renderer.name=="Background" && renderer.GetComponent<ScaleToScreen>()!=null)
            {
                Rect background=UnifiedHudLayout.ScreenBounds(camera,renderer.gameObject,null);
                Check(background.xMin<1 && background.xMax>Screen.width-1,"Background fitted against old camera size: "+background);
                Debug.Log("WIDE_BACKGROUND "+background);
            }
        if(!EpisodeTwo)return;
        var screen=UnityEngine.Object.FindAnyObjectByType<LevelSelectScreen>();
        var circle=(Component)typeof(LevelSelectScreen).GetField("levelCircle").GetValue(screen);
        var label=(GameObject)circle.GetType().GetField("levelName").GetValue(circle);
        var pack=(Component)typeof(LevelSelectScreen).GetField("LevelPackFlipper").GetValue(screen);
        var start=(GameObject)typeof(LevelSelectScreen).GetField("btnStart").GetValue(screen);
        float smallest=float.PositiveInfinity;
        for(int level=1;level<=19;level++)
        {
            circle.GetType().GetMethod("SelectLevel").Invoke(circle,new object[]{level});
            float bottom=float.PositiveInfinity;
            foreach(Renderer renderer in circle.GetComponentsInChildren<Renderer>())
                if(renderer.enabled && !renderer.transform.IsChildOf(label.transform))bottom=Mathf.Min(bottom,UnifiedHudLayout.ScreenBounds(camera,renderer.gameObject,null).yMin);
            Rect packBounds=UnifiedHudLayout.ScreenBounds(camera,pack.gameObject,null,true);
            float gap=bottom-packBounds.yMax;smallest=Mathf.Min(smallest,gap);
            Check(gap>=8,"Map overlaps bottom UI at level "+level+": gap="+gap);
        }
        Rect startBounds=UnifiedHudLayout.ScreenBounds(camera,start,null);
        Check(startBounds.yMin>=LegacyScreenFit.ContentRect().yMin-1,"Start button clipped by bottom corners");
        Debug.Log("WIDE_SELECTION PASS "+Screen.width+"x"+Screen.height+" levels 1-19 minGap="+smallest);
        circle.GetType().GetMethod("SelectLevel").Invoke(circle,new object[]{1});
    }
    static void AssertMenu()
    {
        var layout=UnityEngine.Object.FindAnyObjectByType<MenuRowLayout>();
        Check(layout!=null && layout.Rows.Length==5,"Five menu rows missing");
        Rect safe=LegacyScreenFit.ContentRect();
        foreach(var row in layout.Rows)
        {
            var button=row.GetComponent<SimpleButton>();Rect b=UnifiedHudLayout.ScreenBounds(button.viewCamera,row,null);
            Check(b.xMin>=safe.xMin-2 && b.xMax<=safe.xMax+2 && b.yMin>=safe.yMin-2 && b.yMax<=safe.yMax+2,"Menu row outside safe bounds: "+row.name+" "+b);
            Debug.Log("WIDE_MENU "+row.name+" "+b);
        }
    }
    static void AssertGameplay()
    {
        Camera camera=World.cameraFollow.GetComponent<Camera>();
        float width=2*camera.orthographicSize*camera.aspect;
        Check(Mathf.Abs(width-320)<.1f,"Original gameplay width changed: "+width);
        Hud hud=Hud.Instance;Camera ui=hud.GetComponentInChildren<Camera>();Rect safe=LegacyScreenFit.ContentRect();
        Rect left=UnifiedHudLayout.ScreenBounds(ui,hud.GfxIconLifeOrb,hud.GfxTextOrbs),right=UnifiedHudLayout.ScreenBounds(ui,hud.GfxIconLives,hud.GfxTextLives);
        Debug.Log("WIDE_HUD_BOUNDS left="+left+" right="+right+" cutout="+LegacyScreenFit.HudTop(left.xMin,left.xMax));
        Check(left.xMin>=safe.xMin-2 && right.xMax<=safe.xMax+2,"Side HUD clipped by rounded corners");
        Check(Mathf.Abs(left.yMax-right.yMax)<2,"Episode HUD tops misaligned");
        Check(left.yMax<=LegacyScreenFit.HudTop(left.xMin,left.xMax)+1 && right.yMax<=LegacyScreenFit.HudTop(right.xMin,right.xMax)+1,"Side HUD intersects cutout");
        var field=typeof(Hud).GetField("GfxTextTime");
        if(field!=null)
        {
            Rect timer=UnifiedHudLayout.ScreenBounds(ui,(GameObject)field.GetValue(hud),null);
            Check(timer.yMax<=safe.yMax+1 && timer.yMax<=LegacyScreenFit.HudTop(timer.xMin,timer.xMax)+1,"Timer intersects cutout");
            Debug.Log("WIDE_TIMER "+timer);
        }
        Debug.Log("WIDE_ASSERT PASS "+SceneManager.GetActiveScene().name+" "+Screen.width+"x"+Screen.height+" width="+width+" left="+left+" right="+right);
    }
    static void Save(string label)
    {
        var cameras=Camera.allCameras.Where(c=>c.targetTexture==null).OrderBy(c=>c.depth).ToArray();
        var target=new RenderTexture(Screen.width,Screen.height,24);target.Create();var previous=RenderTexture.active;
        RenderTexture.active=target;GL.Clear(true,true,Color.black);
        foreach(var c in cameras){
            if(label.StartsWith("framing"))Debug.Log("WIDE_RT_BEFORE "+JsonUtility.ToJson(ViewportDiagnostics.Describe(c)));
            c.targetTexture=target;
            if(label.StartsWith("framing"))Debug.Log("WIDE_RT_DURING "+JsonUtility.ToJson(ViewportDiagnostics.Describe(c)));
            c.Render();c.targetTexture=null;
            if(label.StartsWith("framing"))Debug.Log("WIDE_RT_AFTER "+JsonUtility.ToJson(ViewportDiagnostics.Describe(c)));
        }
        RenderTexture.active=target;var im=new Texture2D(Screen.width,Screen.height,TextureFormat.RGB24,false);
        im.ReadPixels(new Rect(0,0,Screen.width,Screen.height),0,0);im.Apply();File.WriteAllBytes(Path.Combine(output,label+".png"),im.EncodeToPNG());
        RenderTexture.active=previous;UnityEngine.Object.DestroyImmediate(im);target.Release();UnityEngine.Object.DestroyImmediate(target);
    }
    static void Finish(int code)
    {
        SessionState.SetBool("WideReview",false);running=false;EditorApplication.isPlaying=false;EditorApplication.Exit(code);
    }
}
#endif
