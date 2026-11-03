#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEditor;
using UnityEngine;
public static partial class WideReview {
 static void AuditPortalTextures() {
  var report = new System.Text.StringBuilder();
  foreach(string name in new[]{"atlas0_49.png","atlas0_alpha_9.png"}) {
   string path="Assets/Texture2D/"+name;
   var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
   var importer=(TextureImporter)AssetImporter.GetAtPath(path);
   var android=importer.GetPlatformTextureSettings("Android");
   report.AppendLine(name+" imported="+texture.width+"x"+texture.height+" format="+texture.format+" max="+importer.maxTextureSize+" compression="+importer.textureCompression+" androidOverride="+android.overridden+" androidMax="+android.maxTextureSize+" androidFormat="+android.format);
  }
  File.WriteAllText(Path.Combine(output,"texture-audit.txt"),report.ToString());
 }
 public static void IsolatePortal() {
  var camera=World.cameraFollow.GetComponent<Camera>();
  var target=new RenderTexture(540,1200,24);camera.targetTexture=target;
  var pixels=new Texture2D(540,1200,TextureFormat.RGB24,false);
  Action<string> shot=name=>{camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,540,1200),0,0);pixels.Apply();File.WriteAllBytes(Path.Combine(output,"isolate-"+name+".png"),pixels.EncodeToPNG());};
  shot("normal");
  foreach(var layer in UnityEngine.Object.FindObjectsByType<ParallaxLayer>()) {
   var renderers=layer.GetComponentsInChildren<Renderer>();
   var states=renderers.Select(r=>r.forceRenderingOff).ToArray();
   foreach(var r in renderers)r.forceRenderingOff=true;
   shot(layer.name);
   for(int i=0;i<renderers.Length;i++)renderers[i].forceRenderingOff=states[i];
  }
  camera.targetTexture=null;RenderTexture.active=null;target.Release();UnityEngine.Object.DestroyImmediate(target);UnityEngine.Object.DestroyImmediate(pixels);
 }
 static void ReviewScan(int flag) {
  new GameObject("Scan review").AddComponent<ScanReviewSampler>().Begin(flag,output);
 }
}
public class ScanReviewSampler:MonoBehaviour {
 int flag;string folder;
 public void Begin(int value,string path){flag=value;folder=path;StartCoroutine(Run());}
 IEnumerator Run(){
  var vessel=(StandardVessel)World.vessel;
  var body=vessel.GetComponent<Rigidbody>();
  var trigger=UnityEngine.Object.FindObjectsByType<VesselTriggerEntity>().First(t=>t.GetParameterInt("passTriggerFlag")==flag);
  Vector3 p=trigger.transform.position+Vector3.forward*100;p.y=body.position.y;p.x=0;
  body.position=p;vessel.transform.position=p;body.linearVelocity=Vector3.zero;vessel.canMove=false;
  World.cameraFollow.enabled=true;World.cameraFollow.UseCameraManager=true;Time.timeScale=1;
  yield return new WaitForSeconds(.5f);
  trigger.SendMessage("OnTriggerEnter",vessel.GetComponent<Collider>());
  var scan=UnityEngine.Object.FindObjectsByType<AnimatedEntity>().First(e=>e.activateEventFlag==flag && e.GetParameterString("image")=="entity_shockwave");
  var camera=World.cameraFollow.GetComponent<Camera>();
  var renderer=scan.GetComponentInChildren<Renderer>();
  var report=new System.Text.StringBuilder();
  Action record=()=>report.AppendLine(Time.time+","+camera.WorldToViewportPoint(renderer.bounds.center).y+","+camera.WorldToViewportPoint(renderer.bounds.center+Vector3.forward*renderer.bounds.extents.z).y+","+camera.WorldToViewportPoint(renderer.bounds.center-Vector3.forward*renderer.bounds.extents.z).y);
  report.AppendLine("time,center,top,bottom");record();
  for(int i=0;i<8;i++){
   yield return new WaitForSeconds(.4f);record();
   ScreenCapture.CaptureScreenshot(Path.Combine(folder,"scan-"+flag+"-"+i+".png"));
  }
  File.WriteAllText(Path.Combine(folder,"scan-"+flag+"-motion.csv"),report.ToString());
  Time.timeScale=0;Destroy(gameObject);
 }
}
#endif
