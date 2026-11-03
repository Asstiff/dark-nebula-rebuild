using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshCombineUtility
{
    public struct MeshInstance
    {
        public Mesh mesh;
        public int subMeshIndex;
        public Matrix4x4 transform;
    }
    public static Mesh Combine(MeshInstance[] sources,bool generateStrips)
    {
        var instances=new List<CombineInstance>(sources.Length);
        int vertexCount=0;
        foreach(MeshInstance source in sources)
        {
            if(source.mesh==null)continue;
            vertexCount+=source.mesh.vertexCount;
            instances.Add(new CombineInstance {mesh=source.mesh,subMeshIndex=source.subMeshIndex,transform=source.transform});
        }
        var result=new Mesh();result.name="Combined scenery";
        result.indexFormat=vertexCount>65535?IndexFormat.UInt32:IndexFormat.UInt16;
        if(instances.Count>0)result.CombineMeshes(instances.ToArray(),true,true,false);
        return result;
    }
}
