#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class WideReview
{
    [Serializable] sealed class MotionCommand
    {
        public string action,label;
        public float z;
        public int height;
        public float endZ,duration;
    }
    static double motionSince;
    static int motionStage;
    static MotionCommand motionPending;
    static double motionPendingSince;
    static MotionSampler sampler;
    public static void RunMotion()
    {
        string[] args=Environment.GetCommandLineArgs();int index=Array.IndexOf(args,"-reviewOutput");
        output=args[index+1];Directory.CreateDirectory(output);
        SessionState.SetString("WideReviewMotionOutput",output);
        SessionState.SetBool("WideReviewMotion",true);
        SessionState.SetBool("WideReviewDetails",Array.IndexOf(args,"-reviewDetails")>=0);
        SetSize(540,1200);EditorSceneManager.OpenScene("Assets/Scenes/Screens/MainMenu.unity");
        BindMotion();EditorApplication.isPlaying=true;
    }
    static void BindMotion()
    {
        EditorApplication.update-=MotionTick;EditorApplication.update+=MotionTick;
        EditorApplication.playModeStateChanged-=MotionMode;EditorApplication.playModeStateChanged+=MotionMode;
    }
    static void MotionMode(PlayModeStateChange mode)
    {
        if(mode!=PlayModeStateChange.EnteredPlayMode)return;
        output=SessionState.GetString("WideReviewMotionOutput","");
        motionStage=0;motionSince=EditorApplication.timeSinceStartup;
    }
    static void MotionTick()
    {
        if(!EditorApplication.isPlaying || !SessionState.GetBool("WideReviewMotion",false))return;
        try
        {
            Application.runInBackground=true;
            if(motionStage==0 && EditorApplication.timeSinceStartup-motionSince>4)
            {
                if(SessionState.GetBool("WideReviewDetails",false))
                {
                    File.WriteAllText(Path.Combine(output,"READY.txt"),"UI ready");motionStage=2;return;
                }
                WideReviewEntrySampler.StartRecording(output);
                LoadLevelScreen.LevelToLoad=1;SceneManager.LoadScene("LoadLevel");
                motionSince=EditorApplication.timeSinceStartup;motionStage=1;
            }
            if(motionStage==1)
            {
                var field=typeof(Hud).GetField("ImageActionRunning",BindingFlags.Public|BindingFlags.Static);
                bool intro=field!=null && (bool)field.GetValue(null);
                if(World.vessel!=null && Hud.Instance!=null && !intro && Time.timeScale==0)Hud.Instance.onResumeGame();
                if(World.cameraFollow!=null && World.vessel!=null && !intro && EditorApplication.timeSinceStartup-motionSince>32)
                {
                    World.cameraFollow.enabled=false;Time.timeScale=0;
                    foreach(var pause in UnityEngine.Object.FindObjectsByType<PauseScreen>(FindObjectsInactive.Include))pause.gameObject.SetActive(false);
                    File.WriteAllText(Path.Combine(output,"READY.txt"),SceneManager.GetActiveScene().name);
                    Debug.Log("WIDE_MOTION READY");motionStage=2;
                }
            }
            if(motionStage!=2)return;
            if(sampler!=null && sampler.running)
            {
                float fraction=(float)((EditorApplication.timeSinceStartup-sampler.since)/sampler.duration);
                SetMotionCamera(Mathf.Lerp(sampler.from,sampler.to,Mathf.Clamp01(fraction)),false);
                if(fraction>=1){sampler.running=false;Time.timeScale=0;sampler.Complete();Save("sweep-end");}
            }
            string path=Path.Combine(output,"request.json");
            if(motionPending==null && File.Exists(path))
            {
                try {motionPending=JsonUtility.FromJson<MotionCommand>(File.ReadAllText(path));File.Delete(path);}
                catch(IOException){return;}
                motionPendingSince=EditorApplication.timeSinceStartup;
                if(motionPending.height>0)SetSize(540,motionPending.height);
            }
            if(motionPending==null || EditorApplication.timeSinceStartup-motionPendingSince<.35)return;
            var cmd=motionPending;motionPending=null;
            if(cmd.action=="exit")
            {
                SessionState.SetBool("WideReviewMotion",false);EditorApplication.isPlaying=false;EditorApplication.Exit(0);return;
            }
            if(cmd.action=="camera")
            {
                SetMotionCamera(cmd.z,true);
            }
            if(cmd.action=="hudCornerAudit")AuditHudCorners();
            if(cmd.action=="selectionAudit")ReviewSelection();
            if(cmd.action=="selection")SceneManager.LoadScene("LevelSelect");
            if(cmd.action=="audit")AuditWakeBounds();
            if(cmd.action=="portalRasterAudit")AuditPortalRaster();
            if(cmd.action=="portalInspect")InspectPortal();
            if(cmd.action=="scanReview")ReviewScan((int)cmd.z);
            if(cmd.action=="textureAudit")AuditPortalTextures();
            if(cmd.action=="isolatePortal")IsolatePortal();
            if(cmd.action=="jumpAudit")AuditJump();
            if(cmd.action=="reloadLevel")
            {
                Time.timeScale=1;
                typeof(UserSettings).GetField("_expert",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(UserSettings.I,cmd.endZ>0);
                LoadLevelScreen.LevelToLoad=(int)cmd.z;
                World.IsRestart=cmd.label.StartsWith("retry");
                SceneManager.LoadScene("LoadLevel");
            }
            if(cmd.action=="checkLayers")
            {
                var layers=UnityEngine.Object.FindObjectsByType<ParallaxLayer>();
                if(World.vessel==null || World.levelNum!=(int)cmd.z || layers.Length==0 || UserSettings.I.expert!=(cmd.endZ>0))
                    throw new Exception("Review did not enter expected level/mode");
                foreach(var layer in layers)
                    if(World.GetParallaxLayer(layer.layerIndex)!=layer)throw new Exception("Stale parallax layer: "+layer.name);
                File.AppendAllText(Path.Combine(output,"layer-lifecycle.txt"),cmd.label+" level="+World.levelNum+" expert="+UserSettings.I.expert+" currentLayers="+layers.Length+" PASS"+Environment.NewLine);
            }
            if(cmd.action=="portalFlow")StartPortalFlow(cmd.z,cmd.duration);
            if(cmd.action=="postAudit")AuditPostComplete();
            if(cmd.action=="mergeAudit")AuditBranchMerge();
            if(cmd.action=="tutorial")
            {
                var screen=UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include).First(c=>c.GetType().Name=="ImageActionBehaviour");
                screen.GetType().GetMethod("Show").Invoke(screen,new object[]{"instructions"});
                screen.GetType().GetField("duration",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(screen,300f);
                Camera hud=(Camera)screen.GetType().GetField("HudCamera").GetValue(screen);
                GameObject art=(GameObject)screen.GetType().GetField("Image",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(screen);
                foreach(var r in art.GetComponentsInChildren<Renderer>())Debug.Log("WIDE_TUTORIAL renderer="+r.name+" bounds="+r.bounds+" scale="+r.transform.localScale+" texture="+r.sharedMaterial.mainTexture);
                Debug.Log("WIDE_TUTORIAL camera="+hud.orthographicSize+" rect="+hud.pixelRect);
            }
            if(cmd.action=="historyFixture")SetHistoryFixture();
            if(cmd.action=="tutorialAudit")AuditTutorial();
            if(cmd.action=="framingAudit")
            {
                var report=new FramingSnapshot();report.projections=Camera.allCameras.Select(ViewportDiagnostics.Describe).ToArray();
                File.WriteAllText(Path.Combine(output,cmd.label+"-projection.json"),JsonUtility.ToJson(report,true));
                ScreenCapture.CaptureScreenshot(Path.Combine(output,cmd.label+"-gameview.png"));
                foreach(Renderer renderer in UnityEngine.Object.FindObjectsByType<Renderer>())
                    if(renderer.name=="Logo" || renderer.name=="Background" || renderer.name=="Title")Debug.Log("WIDE_FRAMING_ART "+renderer.name+" "+renderer.bounds);
            }
            if(cmd.action=="leaderboards")SceneManager.LoadScene("Leaderboard");
            if(cmd.action=="newGame")UnityEngine.Object.FindObjectsByType<SimpleButton>().First(button=>button.name=="New Game").OnButtonUp();
            if(cmd.action=="sweep")
            {
                SetMotionCamera(cmd.z,true);Time.timeScale=1;
                sampler=new GameObject("WideReview motion sampler").AddComponent<MotionSampler>();
                sampler.Begin(output,cmd.z,cmd.endZ,cmd.duration>0?cmd.duration:12);
            }
            if(cmd.action=="postComplete")
            {
                Type type=typeof(World).Assembly.GetType("PostCompleteScreen");
                object result=Activator.CreateInstance(typeof(LevelCompleteResult));
                typeof(LevelCompleteResult).GetField("LevelNum").SetValue(result,1);
                typeof(LevelCompleteResult).GetField("CompletionTime").SetValue(result,118.066f);
                type.GetField("Result").SetValue(null,result);
                typeof(LevelCompleteScreen).GetField("Result").SetValue(null,result);
                SceneManager.LoadScene("PostComplete");
            }
            if(cmd.action=="fixture")UnityEngine.Object.FindAnyObjectByType<LeaderboardView>().SetList(new System.Collections.Generic.List<LeaderboardEntryVO>{new LeaderboardEntryVO{name="LOCAL BEST",time=118.066f,star=0}},false);
            if(cmd.action=="menu")SceneManager.LoadScene("MainMenu");
            if(cmd.action=="pick")
            {
                var shop=UnityEngine.Object.FindObjectsByType<MonoBehaviour>().First(c=>c.GetType().Name=="AvatarShopScreen");
                var swiper=shop.GetType().GetField("_swiper",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(shop);
                swiper.GetType().GetProperty("SelectedIndex").SetValue(swiper,(int)cmd.z);
            }
            if(cmd.action=="equip")
            {
                var shop=UnityEngine.Object.FindObjectsByType<MonoBehaviour>().First(c=>c.GetType().Name=="AvatarShopScreen");
                shop.GetType().GetMethod("HandleEquipEvent",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(shop,new object[]{null});
            }
            if(cmd.action=="avatar")
            {
                Type type=typeof(World).Assembly.GetType("AvatarShopScreen");type.GetField("PreviousScene").SetValue(null,"MainMenu");SceneManager.LoadScene("AvatarShop");
            }
            if(cmd.action=="options")typeof(MainMenuScreen).GetMethod("onShowOption",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(UnityEngine.Object.FindAnyObjectByType<MainMenuScreen>(),null);
            if(cmd.action=="uiDump")
            {
                foreach(Camera c in Camera.allCameras)Debug.Log("WIDE_UI_CAMERA "+c.name+" "+c.transform.position+" "+c.orthographicSize+" "+c.pixelRect);
                foreach(var panel in UnityEngine.Object.FindObjectsByType<MonoBehaviour>())
                    if(panel.GetType().Name=="OptionScreen")
                    {
                        Debug.Log("WIDE_UI_PANEL "+panel.transform.position);
                        foreach(Renderer r in panel.GetComponentsInChildren<Renderer>())Debug.Log("WIDE_UI_RENDERER "+r.name+" "+r.bounds+" enabled="+r.enabled);
                    }
            }
            if(cmd.action=="trim" || cmd.action=="extensionsOff")
            {
                foreach(var layer in UnityEngine.Object.FindObjectsByType<ParallaxLayer>())
                foreach(var renderer in layer.GetComponentsInChildren<Renderer>())
                    if(renderer.name.Contains("extension"))renderer.enabled=false;
                if(cmd.action=="trim")
                {
                    var forest=UnityEngine.Object.FindObjectsByType<ParallaxLayer>().First(l=>l.name=="aztek_floor");
                    var renderers=forest.GetComponentsInChildren<Renderer>().Where(r=>!r.name.Contains("extension")).ToArray();
                    Bounds bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
                    foreach(var r in UnityEngine.Object.FindObjectsByType<Renderer>())
                    foreach(var mat in r.sharedMaterials)
                        if(mat!=null && mat.HasProperty("_BackdropRange"))mat.SetVector("_BackdropRange",new Vector4(bounds.min.z,bounds.max.z,0,0));
                    Debug.Log("WIDE_MOTION TRIM "+bounds);
                }
            }
            if(cmd.action=="dump")
            {
                foreach(var layer in UnityEngine.Object.FindObjectsByType<ParallaxLayer>())
                {
                    var renderers=layer.GetComponentsInChildren<Renderer>();if(renderers.Length==0)continue;
                    Bounds bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
                    Debug.Log("WIDE_MOTION LAYER "+layer.name+" scroll="+layer.scrollValue+" position="+layer.transform.position+" bounds="+bounds);
                }
                foreach(var entity in UnityEngine.Object.FindObjectsByType<Entity>())
                    if(Mathf.Abs(entity.transform.position.z-World.cameraFollow.transform.position.z)<1500)
                        Debug.Log("WIDE_MOTION ENTITY "+entity.GetType().Name+" "+entity.name+" position="+entity.transform.position+" enabled="+entity.enabled);
            }
            if(World.cameraFollow!=null)
            {
                var planes=GeometryUtility.CalculateFrustumPlanes(World.cameraFollow.GetComponent<Camera>());
                foreach(var entity in UnityEngine.Object.FindObjectsByType<Entity>())
                {
                    var field=typeof(Entity).GetField("IsFrozen");if(field==null || !(bool)field.GetValue(entity))continue;
                    var renderers=entity.GetComponentsInChildren<Renderer>();
                    if(renderers.Any(r=>r.enabled && GeometryUtility.TestPlanesAABB(planes,r.bounds)))
                        Debug.Log("WIDE_MOTION FROZEN_VISIBLE "+entity.GetType().Name+" "+entity.name+" world="+entity.transform.position+" parent="+entity.transform.parent.name);
                }
            }
            Save(string.IsNullOrEmpty(cmd.label)?cmd.action:cmd.label);
            File.WriteAllText(Path.Combine(output,"response.json"),JsonUtility.ToJson(cmd));
        }
        catch(Exception e){Debug.LogException(e);File.WriteAllText(Path.Combine(output,"FAIL.txt"),e.ToString());SessionState.SetBool("WideReviewMotion",false);EditorApplication.Exit(1);}
    }
    static void SetMotionCamera(float z,bool sampleLayers)
    {
        var follow=World.cameraFollow;Camera camera=follow.GetComponent<Camera>();
        camera.orthographicSize=Mathf.Max(240,160/camera.aspect);
        Vector3 pos=camera.transform.position;pos.x=0;pos.z=z;camera.transform.position=pos;
        follow.windowPosZ=z-(EpisodeTwo?0:camera.orthographicSize);
        typeof(CameraFollow).GetField("windowPosX")?.SetValue(follow,0f);
        if(!sampleLayers)return;
        float previous=Time.timeScale;Time.timeScale=1;
        foreach(var layer in UnityEngine.Object.FindObjectsByType<ParallaxLayer>())
            typeof(ParallaxLayer).GetMethod("LateUpdate",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(layer,null);
        Time.timeScale=previous;
    }
}
#endif
