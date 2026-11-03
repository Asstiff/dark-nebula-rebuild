#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEngine;

public static partial class WideReview
{
    static void AuditPortalRaster()
    {
        var go=new GameObject("WideReview portal raster camera");var camera=go.AddComponent<Camera>();
        camera.transform.position=new Vector3(0,1000,0);camera.transform.rotation=Quaternion.Euler(90,0,0);
        camera.orthographic=true;camera.orthographicSize=160f*Screen.height/Screen.width;
        camera.aspect=(float)Screen.width/Screen.height;camera.farClipPlane=2000;
        camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;camera.cullingMask=1<<31;
        var target=new RenderTexture(Screen.width,Screen.height,24);target.Create();camera.targetTexture=target;
        var quad=GameObject.CreatePrimitive(PrimitiveType.Quad);quad.name="WideReview region probe";quad.layer=31;
        quad.transform.rotation=Quaternion.Euler(90,0,0);quad.transform.localScale=Vector3.one*20;
        var material=new Material(Shader.Find("Icy Tower Forever/Alpha Blended Color"));material.mainTexture=Texture2D.whiteTexture;material.color=Color.red;
        quad.GetComponent<Renderer>().sharedMaterial=material;
        var block=new MaterialPropertyBlock();var pixel=new Texture2D(1,1,TextureFormat.RGB24,false);
        var report=new StringBuilder();float band=Mathf.Min(1,camera.aspect/LegacyScreenFit.ReferenceAspect);
        Action<string,float,Vector4,bool> probe=(name,z,region,visible)=>
        {
            quad.transform.position=new Vector3(0,0,z);block.SetVector("_PortalRegion",region);quad.GetComponent<Renderer>().SetPropertyBlock(block);
            camera.Render();Vector3 point=camera.WorldToScreenPoint(quad.transform.position);
            RenderTexture previous=RenderTexture.active;RenderTexture.active=target;
            pixel.ReadPixels(new Rect((int)point.x,(int)point.y,1,1),0,0);pixel.Apply();RenderTexture.active=previous;
            float red=pixel.GetPixel(0,0).r;
            if((red>.5f)!=visible)throw new Exception("Portal raster "+name+" failed red="+red);
            report.AppendLine(Screen.width+"x"+Screen.height+" "+name+" red="+red+" PASS");
        };
        probe("original-view-preserved",200,new Vector4(10000,10001,0,band),true);
        if(band<.9f)
        {
            probe("future-region-visible-control",272,new Vector4(-10000,10000,1,band),true);
            probe("future-region-hidden",272,new Vector4(-10000,240,1,band),false);
            probe("past-region-hidden",-272,new Vector4(-240,10000,1,band),false);
            probe("neutral-extension-kept",-272,new Vector4(-10000,10000,1,band),true);
            probe("forest-extension-hidden",-272,new Vector4(-10000,10000,0,band),false);
        }
        camera.targetTexture=null;target.Release();UnityEngine.Object.DestroyImmediate(target);
        UnityEngine.Object.DestroyImmediate(quad);UnityEngine.Object.DestroyImmediate(material);UnityEngine.Object.DestroyImmediate(pixel);UnityEngine.Object.DestroyImmediate(go);
        File.AppendAllText(Path.Combine(output,"PORTAL-RASTER-PASS.txt"),report.ToString());
    }
}
#endif
