#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class WideReviewEntrySampler
{
    static string folder;
    static double start,next;
    static int shot;
    static StreamWriter csv;
    public static void StartRecording(string path)
    {
        folder=path;start=EditorApplication.timeSinceStartup;shot=0;next=0;
        csv=new StreamWriter(Path.Combine(folder,"entry-sequence.csv"));
        csv.WriteLine("elapsed,scene,cameraZ,gameRunning");
        EditorApplication.update-=Tick;EditorApplication.update+=Tick;
    }
    static void Tick()
    {
        if(!EditorApplication.isPlaying || EditorApplication.timeSinceStartup-start>30){csv.Dispose();csv=null;EditorApplication.update-=Tick;return;}
        if(Hud.Instance!=null && !Hud.ImageActionRunning)
        {
            if(Time.timeScale==0)Hud.Instance.onResumeGame();
            foreach(var pause in UnityEngine.Object.FindObjectsByType<PauseScreen>())pause.gameObject.SetActive(false);
        }
        if(EditorApplication.timeSinceStartup<next)return;
        next=EditorApplication.timeSinceStartup+.5f;
        string scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        float z=World.cameraFollow==null?0:World.cameraFollow.transform.position.z;
        csv.WriteLine((EditorApplication.timeSinceStartup-start)+","+scene+","+z+","+World.IsRunning);csv.Flush();
        ScreenCapture.CaptureScreenshot(Path.Combine(folder,"entry-step-"+(shot++).ToString("D2")+".png"));
    }
}
#endif
