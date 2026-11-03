#if UNITY_EDITOR
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Convert the legacy UI atlas once at authoring time. Runtime rendering uses
// standard RGBA and Unity's normal transparent-edge import processing.
public class UiTransparencyUpgrade : IPreprocessBuildWithReport
{
    const string MaterialGuid="6b9a1b882b25d8040888def6cb51114c";
    const string Source="Assets/Texture2D/atlas0_20.png";
    const string Destination="Assets/Texture2D/StandardUI/atlas0_20.png";
    public int callbackOrder {get{return -10;}}
    public void OnPreprocessBuild(BuildReport report){Prepare();}
    public static void Prepare()
    {
        byte[] bytes=File.ReadAllBytes(Source);
        string hash;
        using(var sha=SHA256.Create())hash=BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","");
        var importer=AssetImporter.GetAtPath(Destination) as TextureImporter;
        if(importer==null || importer.userData!="straight-alpha-source:"+hash)
        {
            var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,true);
            if(!ImageConversion.LoadImage(texture,bytes))throw new Exception("Cannot decode original UI PNG");
            Color32[] pixels=texture.GetPixels32();
            for(int i=0;i<pixels.Length;i++)
            {
                Color32 p=pixels[i];int a=p.a;
                pixels[i]=a==0?new Color32(0,0,0,0):new Color32(
                    (byte)Math.Min(255,(p.r*255+a/2)/a),
                    (byte)Math.Min(255,(p.g*255+a/2)/a),
                    (byte)Math.Min(255,(p.b*255+a/2)/a),p.a);
            }
            texture.SetPixels32(pixels);texture.Apply(false);
            Directory.CreateDirectory(Path.GetDirectoryName(Destination));
            File.WriteAllBytes(Destination,texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(Destination,ImportAssetOptions.ForceSynchronousImport);
            importer=(TextureImporter)AssetImporter.GetAtPath(Destination);
            importer.textureType=TextureImporterType.Default;
            importer.alphaSource=TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency=true;
            importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;
            importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;
            importer.userData="straight-alpha-source:"+hash;
            importer.SaveAndReimport();
        }
        string materialPath=AssetDatabase.GUIDToAssetPath(MaterialGuid);
        Material material=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        Shader shader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/Shader/StandardUiAlpha.shader");
        if(material.shader!=shader || material.mainTexture!=AssetDatabase.LoadAssetAtPath<Texture2D>(Destination))
        {
            material.shader=shader;material.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(Destination);
            EditorUtility.SetDirty(material);AssetDatabase.SaveAssets();
        }
        int changed=0;
        foreach(string extension in new[]{"*.prefab","*.unity"})
        foreach(string path in Directory.GetFiles("Assets",extension,SearchOption.AllDirectories))
        {
            string text=File.ReadAllText(path);
            if(!text.Contains(MaterialGuid) || !text.Contains("premultipliedAlpha: 1"))continue;
            string updated=Regex.Replace(text,@"(?ms)^--- !u!114 .*?(?=^--- !u!|\z)",match=>
                match.Value.Contains(MaterialGuid)?match.Value.Replace("premultipliedAlpha: 1","premultipliedAlpha: 0"):match.Value);
            if(updated==text)continue;
            File.WriteAllText(path,updated);changed++;
        }
        if(changed>0)AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Debug.Log("UI_ALPHA_UPGRADE straight RGBA; Unity alphaIsTransparency; updated collections="+changed);
    }
}
#endif
