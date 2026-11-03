#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class LaunchAssets : IPreprocessBuildWithReport
{
    public int callbackOrder {get{return 0;}}
    public void OnPreprocessBuild(BuildReport report){Prepare();}
    public static void Prepare()
    {
        const string texturePath="Assets/Resources/ClassicLaunch.png";
        var importer=AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if(importer==null)return;
        if(importer.textureCompression!=TextureImporterCompression.Uncompressed || importer.mipmapEnabled)
        {
            importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled=false;importer.alphaIsTransparency=false;
            importer.wrapMode=TextureWrapMode.Clamp;importer.SaveAndReimport();
        }
        const string path="Assets/Resources/ClassicLaunchMaterial.mat";
        if(AssetDatabase.LoadAssetAtPath<Material>(path)!=null)return;
        var material=new Material(Shader.Find("Unlit/Transparent"));
        material.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        AssetDatabase.CreateAsset(material,path);AssetDatabase.SaveAssets();
    }
}
#endif
