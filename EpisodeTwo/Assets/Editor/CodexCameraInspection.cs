#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CodexCameraInspection
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    public static void Run()
    {
        string output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../safearea-qa/editor-review-" + (Application.dataPath.Contains("migrate-1") ? "ep1" : "ep2")));
        Directory.CreateDirectory(output);
        string[] paths = AssetDatabase.FindAssets("t:Scene", new[] {"Assets/Scenes"}).Select(AssetDatabase.GUIDToAssetPath).ToArray();
        int checkedScenes = 0;
        foreach (string path in paths)
        {
            Scene scene = EditorSceneManager.OpenScene(path);
            var sizes = UnityEngine.Object.FindObjectsOfType<CameraSize>(true);
            if (sizes.Length == 0) continue;
            var transforms = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
            var originalScales = transforms.Select(t => t.localScale).ToArray();
            foreach (var s in sizes) typeof(CameraSize).GetMethod("Awake", Private).Invoke(s, null);
            bool level = path.Contains("/Levels/");
            if (!level)
            foreach (Renderer r in UnityEngine.Object.FindObjectsOfType<Renderer>(true))
            {
                bool menu = r.sharedMaterial != null && r.sharedMaterial.name.StartsWith("MenuBackground");
                var sprite = r.GetComponent<tk2dBaseSprite>();
                bool background = sprite != null && sprite.collection != null && sprite.collection.spriteCollectionName == "UIBackgroundCollection";
                if (menu || background) Debug.Log("CAMERA_BACKGROUND " + path + " object=" + r.name + " bounds=" + r.bounds);
            }
            foreach (float aspect in new[] {0.45f, 0.5625f, 2f/3f})
            {
                foreach (var s in sizes)
                {
                    Camera c = s.GetComponent<Camera>();
                    c.rect = new Rect(0,0,1,1);
                    c.aspect = aspect;
                    typeof(CameraSize).GetMethod("FixAspect", Private).Invoke(s, null);
                }
                if (level)
                {
                    foreach (PauseScreen pause in UnityEngine.Object.FindObjectsOfType<PauseScreen>(true))
                    {
                        Renderer bg = pause.transform.Find("Background").GetComponent<Renderer>();
                        Camera hud = sizes.First(s => s.IsHud).GetComponent<Camera>();
                        Bounds b = bg.bounds;
                        // The fixed pause plane must cover the entire HUD camera view.
                        Vector3 lo = hud.WorldToViewportPoint(b.min), hi = hud.WorldToViewportPoint(b.max);
                        if (lo.x > 0.001f || lo.y > 0.001f || hi.x < 0.999f || hi.y < 0.999f)
                            throw new Exception("Pause coverage failed " + path + " aspect="+aspect+" lo="+lo+" hi="+hi);
                    }
                }
            }
            for (int i=0;i<transforms.Length;i++)
                if (transforms[i].localScale != originalScales[i]) throw new Exception("Background/object scale changed: " + path + "/" + transforms[i].name);
            checkedScenes++;
            Debug.Log("CAMERA_REVIEW checked " + path);
            if (!level && (scene.name == "MainMenu" || scene.name == "LevelSelect" || scene.name == "LoadLevel"))
            {
                var cameras = sizes.Select(s => s.GetComponent<Camera>()).OrderBy(c => c.depth).ToArray();
                foreach (Camera c in cameras) { c.rect = new Rect(0f, 0.1625f, 1f, 0.675f); c.aspect = 2f/3f; c.orthographicSize = 256f; }
                Save(cameras, Path.Combine(output, scene.name + "-native-frame.png"));
            }
            if (level && (scene.name == "level2-1" || scene.name == "level1-1" || scene.name == "level1-01" || scene.name == "level-1"))
                RenderLevel(output, sizes, scene.name);
        }
        Debug.Log("CAMERA_REVIEW PASS scenes=" + checkedScenes + " all scale transforms preserved; pause coverage passes three portrait ratios");
    }

    static void RenderLevel(string output, CameraSize[] sizes, string name)
    {
        foreach (PauseScreen pause in UnityEngine.Object.FindObjectsOfType<PauseScreen>(true))
        {
            foreach (Transform child in pause.transform.parent)
                child.gameObject.SetActive(child == pause.transform || child.GetComponent<Camera>() != null);
            pause.gameObject.SetActive(true);
        }
        var cameras=sizes.Select(s=>s.GetComponent<Camera>()).OrderBy(c=>c.depth).ToArray();
        foreach(var s in sizes) { var c=s.GetComponent<Camera>();c.rect=new Rect(0,0,1,1);c.aspect=.45f; }
        foreach(var s in sizes) s.GetComponent<Camera>().orthographicSize=(s.IsHud?256f:240f)*(2f/3f)/.45f;
        Save(cameras, Path.Combine(output,name+"-before.png"));
        foreach(var s in sizes) typeof(CameraSize).GetMethod("FixAspect",Private).Invoke(s,null);
        foreach(var s in sizes) Debug.Log("CAMERA_REVIEW render " + s.name + " size=" + s.GetComponent<Camera>().orthographicSize);
        Save(cameras, Path.Combine(output,name+"-after.png"));
    }
    static void Save(Camera[] cameras,string path)
    {
        var target = new RenderTexture(540,1200,24);
        target.Create();
        var initial = RenderTexture.active; RenderTexture.active = target; GL.Clear(true, true, Color.black); RenderTexture.active = initial;
        foreach(var c in cameras) { c.targetTexture=target;c.Render();c.targetTexture=null; }
        var previous=RenderTexture.active;RenderTexture.active=target;
        var image=new Texture2D(540,1200,TextureFormat.RGB24,false);
        image.ReadPixels(new Rect(0,0,540,1200),0,0);image.Apply();File.WriteAllBytes(path,image.EncodeToPNG());
        RenderTexture.active=previous;UnityEngine.Object.DestroyImmediate(image);target.Release();UnityEngine.Object.DestroyImmediate(target);
    }
}
#endif
