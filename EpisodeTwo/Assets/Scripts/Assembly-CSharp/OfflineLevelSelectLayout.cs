using UnityEngine;

// Reuse the native purchase-button slot after removing the offline build's ads.
// The map and its pack-switch animation keep their authored transforms.
[DefaultExecutionOrder(200)]
public class OfflineLevelSelectLayout : MonoBehaviour
{
    public Camera Camera;
    public GameObject StartButton, PackSelector;
    public Vector3 RemovedAdPosition;
    Vector3 startPosition,packPosition;
    Rect lastSafe;
    float lastSize=-1,lastAspect=-1;
    bool initialized;
    int settleUntil;
    void LateUpdate()
    {
        if(Camera==null || StartButton==null || PackSelector==null)return;
        Rect safe=LegacyScreenFit.ContentRect();
        if(!initialized)
        {
            Vector3 row=RemovedAdPosition-StartButton.transform.position;
            startPosition=RemovedAdPosition;
            packPosition=PackSelector.transform.position+row;
            initialized=true;settleUntil=Time.frameCount+15;
        }
        if(safe==lastSafe && Mathf.Approximately(lastSize,Camera.orthographicSize) && Mathf.Approximately(lastAspect,Camera.aspect) && Time.frameCount>settleUntil)return;
        StartButton.transform.position=startPosition;
        PackSelector.transform.position=packPosition;
        Rect start=UnifiedHudLayout.ScreenBounds(Camera,StartButton,null,true);
        Rect pack=UnifiedHudLayout.ScreenBounds(Camera,PackSelector,null,true);
        // The original rows were only 41 units apart (about five units clear).
        // Reserve a visible gap before anchoring the pair inside the safe area.
        float gap=pack.yMin-start.yMax;
        float minimumGap=Mathf.Max(Screen.width*.06f,Mathf.Max(start.height,pack.height)*.5f);
        if(gap<minimumGap)
        {
            StartButton.transform.position-=Camera.transform.up*((minimumGap-gap)*2f*Camera.orthographicSize/Camera.pixelHeight);
            start=UnifiedHudLayout.ScreenBounds(Camera,StartButton,null,true);
        }
        float bottom=Mathf.Min(start.yMin,pack.yMin);
        float offset=Mathf.Max(0,safe.yMin+Screen.width*.02f-bottom);
        Vector3 delta=Camera.transform.up*(offset*2f*Camera.orthographicSize/Camera.pixelHeight);
        StartButton.transform.position+=delta;PackSelector.transform.position+=delta;
        lastSafe=safe;lastSize=Camera.orthographicSize;lastAspect=Camera.aspect;
    }
}
