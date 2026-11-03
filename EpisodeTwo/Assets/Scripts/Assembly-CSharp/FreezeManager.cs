using System.Collections.Generic;
using UnityEngine;

// Wake objects before their visible bounds enter the camera. Scrolling parents
// keep moving while a child sleeps, so those entries cannot use a fixed sector.
[DefaultExecutionOrder(200)]
public class FreezeManager : MonoBehaviour
{
    sealed class FrozenEntry
    {
        public MonoBehaviour behaviour;
        public Vector3 position;
        public Bounds bounds;
        public bool scrolling;
        public int sector;
        public Bounds CurrentBounds
        {
            get
            {
                Bounds result=bounds;
                if(scrolling)result.center+=behaviour.transform.position-position;
                return result;
            }
        }
    }
    readonly Dictionary<int,List<FrozenEntry>> sectors=new Dictionary<int,List<FrozenEntry>>();
    readonly Dictionary<MonoBehaviour,FrozenEntry> entries=new Dictionary<MonoBehaviour,FrozenEntry>();
    readonly List<FrozenEntry> scrolling=new List<FrozenEntry>();
    CameraFollow cameraFollow;
    float maximumExtent;

    void Start(){cameraFollow=World.cameraFollow;}
    void LateUpdate()
    {
        if(cameraFollow==null)cameraFollow=World.cameraFollow;
        if(cameraFollow==null)return;
        Vector2 visible=World.VisibleHalfSize;
        float height=Mathf.Max(1080f,visible.y+600f);
        float width=Mathf.Max(480f,visible.x+96f);
        for(int i=scrolling.Count-1;i>=0;i--)
        {
            FrozenEntry entry=scrolling[i];
            if(entry.behaviour==null || Within(entry.CurrentBounds,width,height))Wake(entry);
        }
        int first=Sector(cameraFollow.windowPosZ-height-maximumExtent);
        int last=Sector(cameraFollow.windowPosZ+height+maximumExtent);
        for(int sector=first;sector<=last;sector++)
        {
            List<FrozenEntry> list;
            if(!sectors.TryGetValue(sector,out list))continue;
            for(int i=list.Count-1;i>=0;i--)
            {
                FrozenEntry entry=list[i];
                if(entry.behaviour==null || Within(entry.CurrentBounds,width,height))Wake(entry);
            }
        }
    }
    static int Sector(float z){return Mathf.FloorToInt(z/1000f);}
    bool Within(Bounds bounds,float width,float height)
    {
        return bounds.min.z<=cameraFollow.windowPosZ+height && bounds.max.z>=cameraFollow.windowPosZ-height
            && bounds.min.x<=cameraFollow.windowPosX+width && bounds.max.x>=cameraFollow.windowPosX-width;
    }
    void Wake(FrozenEntry entry)
    {
        Remove(entry);
        if(entry.behaviour!=null)((IFreezable)entry.behaviour).Defrost();
    }
    public void AddToFrozenObjects(MonoBehaviour behaviour)
    {
        RemoveFromFrozenObjects(behaviour);
        Vector3 position=behaviour.transform.position;
        Bounds bounds=new Bounds(position,Vector3.one*128f);
        foreach(Renderer renderer in behaviour.GetComponentsInChildren<Renderer>())bounds.Encapsulate(renderer.bounds);
        foreach(Collider collider in behaviour.GetComponentsInChildren<Collider>())
            if(collider.enabled)bounds.Encapsulate(collider.bounds);
        ParallaxLayer layer=behaviour.GetComponentInParent<ParallaxLayer>();
        var entry=new FrozenEntry{behaviour=behaviour,position=position,bounds=bounds,
            scrolling=layer!=null && !Mathf.Approximately(layer.scrollValue,1f),sector=Sector(position.z)};
        entries.Add(behaviour,entry);
        if(entry.scrolling)scrolling.Add(entry);
        else
        {
            List<FrozenEntry> list;
            if(!sectors.TryGetValue(entry.sector,out list))sectors.Add(entry.sector,list=new List<FrozenEntry>());
            list.Add(entry);maximumExtent=Mathf.Max(maximumExtent,bounds.extents.z+Mathf.Abs(bounds.center.z-position.z));
        }
    }
    public void RemoveFromFrozenObjects(MonoBehaviour behaviour)
    {
        FrozenEntry entry;if(entries.TryGetValue(behaviour,out entry))Remove(entry);
    }
    void Remove(FrozenEntry entry)
    {
        entries.Remove(entry.behaviour);
        if(entry.scrolling)scrolling.Remove(entry);
        else
        {
            List<FrozenEntry> list;if(sectors.TryGetValue(entry.sector,out list))list.Remove(entry);
        }
    }
}
