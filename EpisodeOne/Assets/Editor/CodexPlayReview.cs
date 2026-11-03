#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class CodexPlayReview
{
    static double since;
    static int stage;
    static string output;
    static bool running;
    static CodexPlayReview()
    {
        if (SessionState.GetBool("CodexPlayReview", false))
        {
            EditorApplication.update += Update;
            EditorApplication.playModeStateChanged += Mode;
        }
    }
    public static void Run()
    {
        LauncherIconBuilder.SetIcons();
        string project = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        output=Path.GetFullPath(Path.Combine(project,"../safearea-qa/play-review-ep1"));
        Directory.CreateDirectory(output);
        SessionState.SetString("CodexPlayReviewOutput",output);
        SessionState.SetBool("CodexPlayReview",true);
        // Set the Editor Game view to the phone's portrait ratio.
        var assembly=typeof(Editor).Assembly;
        var sizesType=assembly.GetType("UnityEditor.GameViewSizes");
        var singleton=typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instance=singleton.GetProperty("instance",BindingFlags.Public|BindingFlags.Static).GetValue(null);
        var groupType=assembly.GetType("UnityEditor.GameViewSizeGroupType");
        var group=sizesType.GetMethod("GetGroup").Invoke(instance,new object[]{Enum.Parse(groupType,"Android")});
        var sizeType=assembly.GetType("UnityEditor.GameViewSize");
        var sizeKind=assembly.GetType("UnityEditor.GameViewSizeType");
        var size=Activator.CreateInstance(sizeType,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,new object[]{Enum.Parse(sizeKind,"FixedResolution"),540,1200,"Codex 9x20 QA"},null);
        var labels=(string[])group.GetType().GetMethod("GetDisplayTexts").Invoke(group,null);
        int index=Array.FindIndex(labels, label=>label.Contains("Codex 9x20 QA"));
        if(index < 0) { group.GetType().GetMethod("AddCustomSize").Invoke(group,new[]{size}); index=(int)group.GetType().GetMethod("GetTotalCount").Invoke(group,null)-1; }
        var view=EditorWindow.GetWindow(assembly.GetType("UnityEditor.GameView"));
        view.GetType().GetProperty("selectedSizeIndex",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic).SetValue(view,index);
        EditorSceneManager.OpenScene("Assets/Scenes/Screens/MainMenu.unity");
        EditorApplication.update -= Update;EditorApplication.update += Update;
        EditorApplication.playModeStateChanged -= Mode;EditorApplication.playModeStateChanged += Mode;
        EditorApplication.isPlaying=true;
    }
    static void Mode(PlayModeStateChange mode)
    {
        if(mode==PlayModeStateChange.EnteredPlayMode) { since=EditorApplication.timeSinceStartup;stage=0;running=true;output=SessionState.GetString("CodexPlayReviewOutput",""); }
    }
    static void Update()
    {
        if(!EditorApplication.isPlaying || !running)return;
        try
        {
            Application.runInBackground=true;
            LegacyScreenFit.PreviewSafeArea=new Rect(Screen.width*.0723f,Screen.height*.0325f,Screen.width*.8554f,Screen.height*.9246f);
            Time.captureDeltaTime=1f/60f;
            if(stage==1 && Hud.Instance!=null && Time.timeScale==0f) Hud.Instance.onResumeGame();
            if(EditorApplication.timeSinceStartup-since < (stage==0?5:stage==1?15:1))return;
            if(stage==0)
            {
                if(SceneManager.GetActiveScene().name != "MainMenu") { SceneManager.LoadScene("MainMenu"); since=EditorApplication.timeSinceStartup; return; }
                Save("MainMenu");
                LoadLevelScreen.LevelToLoad=1;
                SceneManager.LoadScene("LoadLevel");
                stage=1;since=EditorApplication.timeSinceStartup;
            }
            else if(stage==1)
            {
                if(!SceneManager.GetActiveScene().path.Contains("/Levels/"))throw new Exception("Level did not load: "+SceneManager.GetActiveScene().path);
                if(World.vessel != null)
                {
                    var camera = World.cameraFollow.GetComponent<Camera>();
                    Vector3 viewport=camera.WorldToViewportPoint(World.vessel.transform.position);
                    if(viewport.x < 0f || viewport.x > 1f || viewport.y < 0f || viewport.y > 1f) throw new Exception("Vessel outside camera frame: " + viewport);
                    Debug.Log("PLAY_REVIEW VESSEL world=" + World.vessel.transform.position + " camera=" + camera.transform.position + " viewport=" + camera.WorldToViewportPoint(World.vessel.transform.position));
                }
                Save("gameplay");
                foreach(CameraSize sizing in UnityEngine.Object.FindObjectsOfType<CameraSize>())Debug.Log("PLAY_REVIEW CAMERA "+sizing.name+" ortho="+sizing.GetComponent<Camera>().orthographicSize+" screen="+Screen.width+"x"+Screen.height);
                Hud.Instance.Pause();stage=2;since=EditorApplication.timeSinceStartup;
            }
            else
            {
                Save("pause");
                var pause=UnityEngine.Object.FindObjectOfType<PauseScreen>();
                if(pause.transform.Find("Full Screen Pause Backdrop")!=null)throw new Exception("Unexpected replacement backdrop");
                Debug.Log("PLAY_REVIEW PASS original pause background; camera framing; menu runtime screenshots");
                SessionState.SetBool("CodexPlayReview",false);running=false;EditorApplication.isPlaying=false;EditorApplication.Exit(0);
            }
        }
        catch(Exception e) {Debug.LogException(e);SessionState.SetBool("CodexPlayReview",false);running=false;EditorApplication.Exit(1);}
    }
    static void Save(string label)
    {
        var cameras=Camera.allCameras.Where(c=>c.targetTexture==null).OrderBy(c=>c.depth).ToArray();
        var target=new RenderTexture(540,1200,24);target.Create();var previous=RenderTexture.active;RenderTexture.active=target;GL.Clear(true,true,Color.black);
        foreach(var c in cameras){c.targetTexture=target;c.Render();c.targetTexture=null;}
        RenderTexture.active=target;var image=new Texture2D(540,1200,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,540,1200),0,0);image.Apply();File.WriteAllBytes(Path.Combine(output,label+".png"),image.EncodeToPNG());
        RenderTexture.active=previous;UnityEngine.Object.DestroyImmediate(image);target.Release();UnityEngine.Object.DestroyImmediate(target);
        Debug.Log("PLAY_REVIEW saved "+label+" scene="+SceneManager.GetActiveScene().name);
    }
}
#endif
