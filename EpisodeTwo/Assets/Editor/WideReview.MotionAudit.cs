#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static partial class WideReview
{
    static void AuditPostComplete()
    {
        var completed=UnityEngine.Object.FindObjectsByType<MonoBehaviour>().First(c=>c.GetType().Name=="PostCompleteScreen");
        var view=UnityEngine.Object.FindAnyObjectByType<LeaderboardView>();
        var camera=(Camera)typeof(LeaderboardView).GetField("_viewCamera",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(view);
        Camera hud=GameObject.Find("Hud Camera").GetComponent<Camera>();
        float bottom=camera.pixelRect.yMin;
        foreach(string name in new[]{"ButtonNextLevel","ButtonTryAgain","ButtonMainMenu"})
        {
            var button=(Component)completed.GetType().GetField(name).GetValue(completed);
            Rect bounds=UnifiedHudLayout.ScreenBounds(hud,button.gameObject,null,true);
            if(bottom-bounds.yMax<5)throw new Exception("Result leaderboard overlaps "+name+" gap="+(bottom-bounds.yMax));
            Debug.Log("WIDE_POST_GAP "+Screen.width+"x"+Screen.height+" "+name+" "+(bottom-bounds.yMax));
        }
        if(camera.pixelRect.xMin<0 || camera.pixelRect.xMax>Screen.width)throw new Exception("Result leaderboard outside screen");
        foreach(var entry in view.GetComponentsInChildren<LeaderboardEntry>())
        {
            var local=entry.transform.Find("Local score name");
            if(local==null || !local.gameObject.activeSelf)continue;
            foreach(Renderer r in entry.GetComponentsInChildren<Renderer>())Debug.Log("WIDE_ROW "+r.name+" "+r.bounds);
        }
        File.AppendAllText(Path.Combine(output,"POST-PASS.txt"),Screen.width+"x"+Screen.height+" no overlap\n");
    }
    static void AuditWakeBounds()
    {
        Type managerType=typeof(World).Assembly.GetType("FreezeManager");
        object manager=typeof(World).GetField("FreezeManager").GetValue(null);
        Camera camera=World.cameraFollow.GetComponent<Camera>();float originalSize=camera.orthographicSize;
        Action<Vector3,bool,bool> test=(offset,scrolling,wideBounds)=>
        {
            GameObject parent=new GameObject("WideReview awake bounds fixture");
            if(scrolling){var layer=parent.AddComponent<ParallaxLayer>();layer.scrollValue=.5f;}
            parent.transform.position=World.cameraFollow.transform.position+offset;
            var entity=new GameObject("Frozen fixture").AddComponent<Entity>();entity.transform.SetParent(parent.transform,false);
            typeof(Entity).GetField("IsFrozen").SetValue(entity,true);entity.enabled=false;
            if(wideBounds)
            {
                GameObject mesh=GameObject.CreatePrimitive(PrimitiveType.Cube);mesh.transform.SetParent(entity.transform,false);
                mesh.transform.localPosition=new Vector3(0,0,-800);mesh.transform.localScale=new Vector3(64,1,160);
            }
            Physics.SyncTransforms();managerType.GetMethod("AddToFrozenObjects").Invoke(manager,new object[]{entity});
            if(scrolling)parent.transform.position=World.cameraFollow.transform.position;
            managerType.GetMethod("LateUpdate",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(manager,null);
            bool frozen=(bool)typeof(Entity).GetField("IsFrozen").GetValue(entity);
            bool shouldWake=scrolling || wideBounds || offset.z<1000;
            managerType.GetMethod("RemoveFromFrozenObjects").Invoke(manager,new object[]{entity});
            UnityEngine.Object.DestroyImmediate(parent);
            if(frozen==shouldWake)throw new Exception("Wake audit failed: "+offset+" scrolling="+scrolling+" bounds="+wideBounds);
        };
        test(new Vector3(0,0,3000),true,false);
        test(new Vector3(0,0,1000),false,true);
        camera.orthographicSize=800;test(new Vector3(0,0,850),false,false);
        test(new Vector3(0,0,5000),false,false);camera.orthographicSize=originalSize;
        File.WriteAllText(Path.Combine(output,"WAKE-AUDIT-PASS.txt"),"Scrolling-parent movement, visible child bounds, taller frustum and offscreen sleep verified.");
        Debug.Log("WIDE_WAKE_AUDIT PASS scrolling parent / bounds / tall frustum / offscreen sleep");
    }
    [DefaultExecutionOrder(1000)]
    public sealed class MotionSampler : MonoBehaviour
    {
        public bool running;
        public double since,duration;
        public float from,to;
        StreamWriter writer;
        Entity[] entities;
        Renderer[][] renderers;
        FieldInfo frozen;
        string directory;
        int frames,visible,frozenVisible;
        public void Begin(string path,float first,float last,double seconds)
        {
            directory=path;from=first;to=last;duration=seconds;since=EditorApplication.timeSinceStartup;
            entities=UnityEngine.Object.FindObjectsByType<Entity>();renderers=entities.Select(e=>e.GetComponentsInChildren<Renderer>()).ToArray();
            frozen=typeof(Entity).GetField("IsFrozen");
            writer=new StreamWriter(Path.Combine(path,"motion.csv"));writer.WriteLine("frame,time,entity,type,x,z,cameraZ,frozen,inCore");running=true;
        }
        void LateUpdate()
        {
            if(!running || World.cameraFollow==null)return;
            frames++;var planes=GeometryUtility.CalculateFrustumPlanes(World.cameraFollow.GetComponent<Camera>());
            for(int i=0;i<entities.Length;i++)
            {
                var entity=entities[i];if(entity==null || !renderers[i].Any(r=>r!=null && r.enabled && GeometryUtility.TestPlanesAABB(planes,r.bounds)))continue;
                bool sleeping=frozen!=null && (bool)frozen.GetValue(entity);visible++;if(sleeping)frozenVisible++;
                Vector3 pos=entity.transform.position;
                writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture,"{0},{1:F4},{2},{3},{4:F3},{5:F3},{6:F3},{7},{8}",frames,Time.time,entity.GetEntityId(),entity.GetType().Name,pos.x,pos.z,World.cameraFollow.windowPosZ,sleeping,Mathf.Abs(pos.z-World.cameraFollow.windowPosZ)<240));
            }
        }
        public void Complete()
        {
            writer.Dispose();writer=null;
            string result="frames="+frames+" visibleSamples="+visible+" frozenVisible="+frozenVisible;
            File.WriteAllText(Path.Combine(directory,"MOTION-RESULT.txt"),result);Debug.Log("WIDE_MOTION_RESULT "+result);
        }
        void OnDestroy(){if(writer!=null)writer.Dispose();}
    }
}
#endif
