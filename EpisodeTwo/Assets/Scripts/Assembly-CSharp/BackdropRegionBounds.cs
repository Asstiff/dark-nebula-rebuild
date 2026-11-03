using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Old mixed-biome maps contain forest scenery padding beyond the forest floor.
// Clip tile scenery to its authored region while keeping portal/glow entities,
// all original geometry, and all collisions intact.
public class BackdropRegionBounds : MonoBehaviour
{
    readonly List<Material> owned=new List<Material>();
    IEnumerator Start()
    {
        yield return null;
        ParallaxLayer forest=null,matrix=null;
        foreach(ParallaxLayer layer in Object.FindObjectsByType<ParallaxLayer>())
        {
            if(layer.gameObject.scene!=gameObject.scene)continue;
            if(layer.name=="aztek_floor")forest=layer;
            if(layer.name=="matrix_floor")matrix=layer;
        }
        if(forest==null || matrix==null)yield break;
        Renderer[] floor=forest.GetComponentsInChildren<Renderer>();
        if(floor.Length==0)yield break;
        Bounds bounds=floor[0].bounds;foreach(Renderer renderer in floor)bounds.Encapsulate(renderer.bounds);
        // Geometry belongs to the authored forest, not to tall-screen padding.
        // The opening fly-in deliberately starts before the playable floor.
        // Keep its authored water/foliage backdrop; only bound the portal end.
        Vector4 region=new Vector4(-100000000f,bounds.max.z,0,0);
        var copies=new Dictionary<Material,Material>();
        foreach(ParallaxLayer layer in Object.FindObjectsByType<ParallaxLayer>())
        {
        if(layer.gameObject.scene!=gameObject.scene || !layer.name.StartsWith("aztek_"))continue;
        foreach(Renderer renderer in layer.GetComponentsInChildren<Renderer>())
        {
            if(renderer.GetComponentInParent<Entity>()!=null || renderer.GetComponentInParent<CombineChildren>()==null)continue;
            Material[] materials=renderer.sharedMaterials;bool changed=false;
            for(int i=0;i<materials.Length;i++)
            {
                Material original=materials[i];
                if(original==null)continue;
                bool opaque=original.shader.name=="Mobile/Unlit (Supports Lightmap)";
                if(!opaque && !original.HasProperty("_BackdropRange"))continue;
                Material copy;
                if(!copies.TryGetValue(original,out copy))
                {
                    copy=new Material(original);copy.name=original.name+" (forest bounds)";
                    if(opaque)copy.shader=Resources.Load<Shader>("ForestOpaqueBounds");
                    copy.SetVector("_BackdropRange",region);copies.Add(original,copy);owned.Add(copy);
                }
                materials[i]=copy;changed=true;
            }
            if(changed)renderer.sharedMaterials=materials;
        }
        }
        Debug.Log("BACKDROP_REGION forest="+region+" boundedMaterials="+owned.Count);
    }
    void OnDestroy(){foreach(Material material in owned)if(material!=null)Destroy(material);}
}
