using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Continue the edge of a flat authored backdrop outside its original bounds.
// The original mesh, transform, UVs and aspect remain unchanged.
public class BackdropMargins : MonoBehaviour
{
    public Camera Camera;
    readonly List<Mesh> meshes=new List<Mesh>();
    readonly List<GameObject> objects=new List<GameObject>();
    IEnumerator Start()
    {
        yield return null;
        if(Camera==null)yield break;
        Renderer source=null;float area=0;
        foreach(Renderer r in Object.FindObjectsByType<Renderer>())
        {
            if(r.gameObject.scene!=gameObject.scene || r.name!="Background" || r.GetComponent<MeshFilter>()==null)continue;
            float value=r.bounds.size.x*r.bounds.size.y;
            if(value>area){area=value;source=r;}
        }
        if(source==null)yield break;
        Create(source,true);Create(source,false);
    }
    void Create(Renderer source,bool top)
    {
        Mesh input=source.GetComponent<MeshFilter>().sharedMesh;
        if(input==null)return;
        Vector3 up=Camera.transform.up;
        Vector3 center=source.bounds.center;
        float edge=Vector3.Dot(center,up)+(top?1f:-1f)*source.bounds.extents.y;
        Vector3[] vertices=input.vertices;
        for(int i=0;i<vertices.Length;i++)
        {
            Vector3 p=source.transform.TransformPoint(vertices[i]);
            vertices[i]=p+2f*(edge-Vector3.Dot(p,up))*up;
        }
        int[] triangles=input.triangles;
        for(int i=0;i<triangles.Length;i+=3){int swap=triangles[i+1];triangles[i+1]=triangles[i+2];triangles[i+2]=swap;}
        var mesh=new Mesh();mesh.name="Backdrop edge continuation";
        mesh.vertices=vertices;mesh.uv=input.uv;
        if(input.colors32.Length==input.vertexCount)mesh.colors32=input.colors32;
        mesh.triangles=triangles;mesh.RecalculateBounds();meshes.Add(mesh);
        var go=new GameObject(top?"Upper backdrop continuation":"Lower backdrop continuation");
        go.layer=source.gameObject.layer;go.transform.SetParent(transform.root,true);
        go.AddComponent<MeshFilter>().sharedMesh=mesh;
        go.AddComponent<MeshRenderer>().sharedMaterial=source.sharedMaterial;objects.Add(go);
    }
    void OnDestroy()
    {
        foreach(GameObject go in objects)if(go!=null)Destroy(go);
        foreach(Mesh mesh in meshes)if(mesh!=null)Destroy(mesh);
    }
}
