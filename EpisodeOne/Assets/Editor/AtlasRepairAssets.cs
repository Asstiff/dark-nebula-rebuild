#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Install only the generated repairs of the original atlas's unfinished bands.
// Every pixel outside the three masks is copied from the original PNG exactly.
public class AtlasRepairAssets : IPreprocessBuildWithReport
{
    const string Original="Assets/Texture2D/atlas0_10.png";
    const string Candidate="Assets/Editor/ArtSources/atlas0_10-candidate.png";
    const string Repaired="Assets/Texture2D/Repairs/atlas0_10_v2.png";
    public int callbackOrder=>-9;
    public void OnPreprocessBuild(BuildReport report){Prepare();}
    public static void Prepare()
    {
        if(!File.Exists(Repaired))
        {
            var original=new Texture2D(2,2,TextureFormat.RGBA32,false,true);
            var generated=new Texture2D(2,2,TextureFormat.RGBA32,false,true);
            ImageConversion.LoadImage(original,File.ReadAllBytes(Original));
            ImageConversion.LoadImage(generated,File.ReadAllBytes(Candidate));
            var pixels=original.GetPixels32();int width=original.width,height=original.height;
            var masks=new[]{new RectInt(0,114,130,48),new RectInt(0,244,130,48),new RectInt(130,894,390,48)};
            int changed=0;
            foreach(RectInt mask in masks)
            for(int y=mask.yMin;y<mask.yMax;y++)for(int x=mask.xMin;x<mask.xMax;x++)
            {
                int index=(height-1-y)*width+x;
                Color next=generated.GetPixelBilinear((x+.5f)/width,1f-(y+.5f)/height);
                float weight=Mathf.Clamp01((mask.yMax-1-y)/7f);
                Color color=Color.Lerp(pixels[index],next,weight);color.a=1;
                pixels[index]=(Color32)color;changed++;
            }
            original.SetPixels32(pixels);original.Apply(false);
            Directory.CreateDirectory(Path.GetDirectoryName(Repaired));File.WriteAllBytes(Repaired,original.EncodeToPNG());
            Object.DestroyImmediate(original);Object.DestroyImmediate(generated);
            AssetDatabase.ImportAsset(Repaired,ImportAssetOptions.ForceSynchronousImport);
            var importer=(TextureImporter)AssetImporter.GetAtPath(Repaired);
            importer.mipmapEnabled=false;importer.alphaIsTransparency=false;importer.npotScale=TextureImporterNPOTScale.None;
            importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.wrapMode=TextureWrapMode.Clamp;importer.filterMode=FilterMode.Bilinear;
            importer.SaveAndReimport();Debug.Log("ATLAS_REPAIR masks="+changed+" pixels; original atlas preserved");
        }
        var source=AssetDatabase.LoadAssetAtPath<Texture2D>(Original);
        var target=AssetDatabase.LoadAssetAtPath<Texture2D>(Repaired);
        foreach(string guid in AssetDatabase.FindAssets("t:Material",new[]{"Assets/Material"}))
        {
            var material=AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
            if(!material.HasProperty("_MainTex"))continue;
            if(material.mainTexture!=source)continue;
            material.mainTexture=target;EditorUtility.SetDirty(material);
        }
        AssetDatabase.SaveAssets();
    }
}
#endif
