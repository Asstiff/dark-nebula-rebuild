#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public static partial class WideReview
{
    static void InspectPortal()
    {
        var report=new StringBuilder();
        report.AppendLine("scene="+UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        report.AppendLine("vessel="+World.vessel.transform.position+" camera="+World.cameraFollow.transform.position);
        foreach(var layer in UnityEngine.Object.FindObjectsByType<ParallaxLayer>())
        {
            object alpha=typeof(ParallaxLayer).GetField("currentAlpha",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(layer);
            var cached=(Material)typeof(ParallaxLayer).GetField("sharedMaterial",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(layer);
            report.AppendLine("LAYER "+layer.name+" index="+layer.layerIndex+" scroll="+layer.scrollValue+" alpha="+alpha+" cached="+(cached==null?"null":cached.name));
            foreach(var material in layer.GetComponentsInChildren<Renderer>().Where(r=>r.GetComponentInParent<CombineChildren>()!=null).SelectMany(r=>r.sharedMaterials).Distinct())
                report.AppendLine("  MATERIAL "+material.name+" shader="+material.shader.name+" color="+(material.HasProperty("_Color")?material.color.ToString():"none")+" cachedMatch="+(cached==material));
        }
        foreach(Entity entity in UnityEngine.Object.FindObjectsByType<Entity>().OrderBy(e=>e.transform.position.z))
        {
            string type=entity.GetType().Name;
            if(!(type.Contains("Vessel") || type.Contains("Magnet") || type.Contains("Fader") || type.Contains("EndOf")))continue;
            report.AppendLine(type+" position="+entity.transform.position+" parameters="+
                string.Join(";",entity.ParameterNames.Select((key,index)=>key+"="+entity.ParameterNormal[index])));
        }
        File.WriteAllText(Path.Combine(output,"portal-entities.txt"),report.ToString());
    }

    static void StartPortalFlow(float z,float duration)
    {
        var portal=UnityEngine.Object.FindObjectsByType<MagnetVesselSlotEntity>().OrderBy(p=>Mathf.Abs(p.transform.position.z-z)).First();
        var sampler=new GameObject("WideReview actual portal sequence").AddComponent<PortalFlowSampler>();
        sampler.Begin(portal,output,duration);
    }
}

public sealed class PortalFlowSampler : MonoBehaviour
{
    MagnetVesselSlotEntity portal;
    Rigidbody body;
    string folder;
    bool approach, recording;
    float nextShot, until;
    int shot;
    StreamWriter csv;
    public void Begin(MagnetVesselSlotEntity target,string output,float duration)
    {
        portal=target;folder=output;
        body=World.vessel.GetComponent<Rigidbody>();
        World.cameraFollow.enabled=true;
        World.cameraFollow.UseCameraManager=true;
        Time.timeScale=1;
        StartCoroutine(Run(duration));
    }
    System.Collections.IEnumerator Run(float duration)
    {
        if(duration<0)
        {
            var collider=World.vessel.GetComponent<Collider>();
            foreach(int flag in new[]{20,10,37,11})
            {
                var trigger=UnityEngine.Object.FindObjectsByType<VesselTriggerEntity>()
                    .First(t=>t.GetParameterInt("passTriggerFlag")==flag);
                trigger.SendMessage("OnTriggerEnter",collider);
                yield return new WaitForSeconds(4.25f);
            }
        }
        var position=portal.transform.position-Vector3.forward*620f;
        body.position=position;World.vessel.transform.position=position;
        body.linearVelocity=Vector3.zero;
        ((StandardVessel)World.vessel).canMove=false;
        approach=true;recording=true;
        until=Time.realtimeSinceStartup+Mathf.Abs(duration);
        nextShot=Time.realtimeSinceStartup;
        csv=new StreamWriter(Path.Combine(folder,"portal-flow.csv"));
        csv.WriteLine("time,vesselX,vesselZ,cameraX,cameraY,cameraZ,halfHeight,orthographic");
    }
    void FixedUpdate()
    {
        if(!approach || portal==null || body==null)return;
        var trigger=portal.GetComponentInChildren<Collider>();
        if((trigger!=null && trigger.bounds.Contains(body.position)) || body.position.z>=portal.transform.position.z-12f)
        {approach=false;return;}
        body.MovePosition(body.position+Vector3.forward*160f*Time.fixedDeltaTime);
    }
    void LateUpdate()
    {
        if(!recording || World.vessel==null)return;
        var camera=World.cameraFollow.GetComponent<Camera>();
        var p=camera.transform.position;
        csv.WriteLine(Time.time+","+World.vessel.transform.position.x+","+World.vessel.transform.position.z+","+
            p.x+","+p.y+","+p.z+","+camera.orthographicSize+","+camera.orthographic);
        if(Time.realtimeSinceStartup>=nextShot)
        {
            ScreenCapture.CaptureScreenshot(Path.Combine(folder,"portal-step-"+(shot++).ToString("D2")+".png"));
            nextShot=Time.realtimeSinceStartup+.5f;
        }
        if(Time.realtimeSinceStartup<until)return;
        var observed=(System.Collections.Hashtable)typeof(World).GetField("Flags",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
        var flags=string.Join(",",new[]{20,10,37,11,60,104,106,103,105,99}.Select(f=>f+"="+(observed.ContainsKey(f)?observed[f]:0)));
        File.WriteAllText(Path.Combine(folder,"flow-flags.txt"),flags);
        csv.Dispose();csv=null;recording=false;Time.timeScale=0;
        WideReview.IsolatePortal();
        File.WriteAllText(Path.Combine(folder,"PORTAL-FLOW-DONE.txt"),"Native portal sequence; preceding flags initialized through original trigger handlers.");
        Destroy(this);
    }
    void OnDestroy(){if(csv!=null)csv.Dispose();}
}
#endif
