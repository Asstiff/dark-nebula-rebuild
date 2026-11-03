#if UNITY_EDITOR
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

public static class ModernizationAudit
{
    public static void Run()
    {
        var source=new Mesh();source.name="Mesh combine audit source";
        source.vertices=new[]{new Vector3(-1,-1,0),new Vector3(1,-1,0),new Vector3(-1,1,0),new Vector3(1,1,0)};
        source.normals=new[]{Vector3.forward,Vector3.forward,Vector3.forward,Vector3.forward};
        source.tangents=new[]{new Vector4(1,0,0,1),new Vector4(1,0,0,1),new Vector4(1,0,0,1),new Vector4(1,0,0,1)};
        source.uv=new[]{Vector2.zero,Vector2.right,Vector2.up,Vector2.one};source.uv2=source.uv;
        source.colors=new[]{Color.white,Color.red,new Color(0,1,0,0.5f),Color.blue};
        source.triangles=new[]{0,2,1,2,3,1};
        var modern=new MeshCombineUtility.MeshInstance[64];
        var legacy=new LegacyCombineReference.MeshInstance[modern.Length];
        for(int i=0;i<modern.Length;i++)
        {
            Matrix4x4 matrix=Matrix4x4.TRS(new Vector3(i*3,2,-i),Quaternion.Euler(17,i%4*30,8),new Vector3(1.5f,2,0.7f));
            modern[i]=new MeshCombineUtility.MeshInstance{mesh=source,subMeshIndex=0,transform=matrix};
            legacy[i]=new LegacyCombineReference.MeshInstance{mesh=source,subMeshIndex=0,transform=matrix};
        }
        Mesh a=null,b=null,large=null;
        try
        {
            a=LegacyCombineReference.Combine(legacy);b=MeshCombineUtility.Combine(modern,false);
            if(a.vertexCount!=b.vertexCount || a.triangles.Length!=b.triangles.Length)throw new Exception("Combined topology changed");
            Vector3[] av=a.vertices,bv=b.vertices;Vector2[] au=a.uv,bu=b.uv;Color32[] ac=a.colors32,bc=b.colors32;
            for(int i=0;i<av.Length;i++)
                if(Vector3.Distance(av[i],bv[i])>0.001f || Vector2.Distance(au[i],bu[i])>0.00001f || !ac[i].Equals(bc[i]))throw new Exception("Combined geometry/UV/color changed at "+i);
            int[] ai=a.triangles,bi=b.triangles;for(int i=0;i<ai.Length;i++)if(ai[i]!=bi[i])throw new Exception("Combined indices changed");
            var many=new MeshCombineUtility.MeshInstance[17000];
            for(int i=0;i<many.Length;i++)many[i]=new MeshCombineUtility.MeshInstance{mesh=source,transform=Matrix4x4.Translate(Vector3.right*i)};
            large=MeshCombineUtility.Combine(many,false);
            if(large.vertexCount!=68000 || large.indexFormat!=IndexFormat.UInt32 || large.triangles[large.triangles.Length-1]>67999)throw new Exception("32-bit mesh indices failed");
            var clock=Stopwatch.StartNew();
            for(int i=0;i<8;i++)UnityEngine.Object.DestroyImmediate(LegacyCombineReference.Combine(legacy));
            double oldMs=clock.Elapsed.TotalMilliseconds;clock.Restart();
            for(int i=0;i<8;i++)UnityEngine.Object.DestroyImmediate(MeshCombineUtility.Combine(modern,false));
            double newMs=clock.Elapsed.TotalMilliseconds;
            UnityEngine.Debug.Log("MODERNIZATION_AUDIT PASS geometry/UV/color/topology; 68000 vertices UInt32; legacyMs="+oldMs.ToString("F3")+" nativeMs="+newMs.ToString("F3"));
        }
        finally
        {
            if(a!=null)UnityEngine.Object.DestroyImmediate(a);if(b!=null)UnityEngine.Object.DestroyImmediate(b);
            if(large!=null)UnityEngine.Object.DestroyImmediate(large);UnityEngine.Object.DestroyImmediate(source);
        }
    }
}
#endif
