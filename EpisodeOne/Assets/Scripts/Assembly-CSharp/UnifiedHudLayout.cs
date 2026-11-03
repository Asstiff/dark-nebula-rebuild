using UnityEngine;

// The side counters share one placement rule in both episodes. Only a control
// whose horizontal bounds overlap the cutout needs to move below that cutout.
public static class UnifiedHudLayout
{
    public static void Apply(Camera camera, GameObject leftIcon, GameObject leftText,
        GameObject rightIcon, GameObject rightText, GameObject timer = null)
    {
        if(camera == null || Screen.width <= 0 || Screen.height <= 0)return;
        Rect safe=LegacyScreenFit.ContentRect();
        float gap=Screen.width*0.02f;
        // Fixed anchors in units of screen width, identical on both episodes
        // and independent of device corner/cutout metadata. The common 8.3% inset
        // fits rounded corners up to 28% of the width without per-device shifts.
        float edge=Screen.width*.083f;
        Rect left=ScreenBounds(camera,leftIcon,leftText);
        Rect right=ScreenBounds(camera,rightIcon,rightText);
        Move(camera,leftIcon,leftText,edge-left.xMin,0);
        Move(camera,rightIcon,rightText,Screen.width-edge-right.xMax,0);
        left=ScreenBounds(camera,leftIcon,leftText);right=ScreenBounds(camera,rightIcon,rightText);
        float sideTop=Screen.height-Screen.width*.083f;
        Move(camera,leftIcon,leftText,0,sideTop-left.yMax);
        Move(camera,rightIcon,rightText,0,sideTop-right.yMax);
        if(timer!=null)
        {
            Rect bounds=ScreenBounds(camera,timer,null);
            float top=Mathf.Min(safe.yMax,LegacyScreenFit.HudTop(bounds.xMin,bounds.xMax))-gap;
            Move(camera,timer,null,Screen.width*0.5f-bounds.center.x,top-bounds.yMax);
        }
    }

    static void Place(Camera camera,GameObject icon,GameObject text,float edge,float unused,bool right,float gap)
    {
        Rect bounds=ScreenBounds(camera,icon,text);
        Move(camera,icon,text,edge-(right?bounds.xMax:bounds.xMin),0f);
        bounds=ScreenBounds(camera,icon,text);
        float top=LegacyScreenFit.HudTop(bounds.xMin,bounds.xMax)-gap;
        Move(camera,icon,text,0f,top-bounds.yMax);
    }

    public static Rect ScreenBounds(Camera camera,GameObject a,GameObject b,bool visibleOnly=false)
    {
        Vector2 min=new Vector2(float.PositiveInfinity,float.PositiveInfinity);
        Vector2 max=new Vector2(float.NegativeInfinity,float.NegativeInfinity);
        foreach(GameObject go in new[]{a,b})
        {
            if(go==null)continue;
            foreach(Renderer renderer in go.GetComponentsInChildren<Renderer>(true))
            {
                if(visibleOnly && (!renderer.enabled || !renderer.gameObject.activeInHierarchy))continue;
                Bounds bounds=renderer.bounds;
                for(int x=-1;x<=1;x+=2)for(int y=-1;y<=1;y+=2)for(int z=-1;z<=1;z+=2)
                {
                    Vector3 p=camera.WorldToScreenPoint(bounds.center+Vector3.Scale(bounds.extents,new Vector3(x,y,z)));
                    min=Vector2.Min(min,p);max=Vector2.Max(max,p);
                }
            }
        }
        if(float.IsInfinity(min.x))return new Rect();
        return Rect.MinMaxRect(min.x,min.y,max.x,max.y);
    }

    static void Move(Camera camera,GameObject a,GameObject b,float x,float y)
    {
        Vector3 delta=(camera.transform.right*x+camera.transform.up*y)*(2f*camera.orthographicSize/camera.pixelHeight);
        if(a!=null)a.transform.position+=delta;
        if(b!=null)b.transform.position+=delta;
    }
}
