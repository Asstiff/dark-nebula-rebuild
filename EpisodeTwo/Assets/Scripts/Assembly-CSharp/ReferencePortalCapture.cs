using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Optional Android intent diagnostics. Normal launch is manual gameplay.
public sealed class ReferencePortalCapture : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void StartIfRequested()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        bool scan = false;
        using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))
        using(var intent=activity.Call<AndroidJavaObject>("getIntent"))
        {
            scan = intent.Call<bool>("getBooleanExtra","scanReview",false);
            if(!scan && !intent.Call<bool>("getBooleanExtra","portalReview",false)) return;
        }
        var go=new GameObject("Reference capture"); DontDestroyOnLoad(go);
        go.AddComponent<ReferencePortalCapture>().scanReview = scan;
#endif
    }

    bool scanReview;
    Rigidbody playerBody;
    MagnetVesselSlotEntity portal;
    bool approach;
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(3);
        LoadLevelScreen.LevelToLoad=1;
        SceneManager.LoadScene("LoadLevel");
        while(World.vessel==null || World.cameraFollow==null || Hud.Instance==null) yield return null;
        yield return new WaitForSecondsRealtime(24);
        if(Time.timeScale==0) Hud.Instance.onResumeGame();
        Time.timeScale=1;
        var vessel=(StandardVessel)World.vessel;
        var collider=vessel.GetComponentInChildren<Collider>();
        if (scanReview)
        {
            playerBody=vessel.GetComponent<Rigidbody>();
            vessel.canMove=false;
            foreach (int scanFlag in new[] {10,11})
            {
                VesselTriggerEntity scanTrigger=null;
                foreach(var trigger in Object.FindObjectsByType<VesselTriggerEntity>())
                    if(trigger.GetParameterInt("passTriggerFlag")==scanFlag) {scanTrigger=trigger;break;}
                Vector3 scanPosition=scanTrigger.transform.position+Vector3.forward*100;
                scanPosition.x=0;scanPosition.y=playerBody.position.y;
                playerBody.position=scanPosition;vessel.transform.position=scanPosition;playerBody.linearVelocity=Vector3.zero;
                yield return new WaitForSecondsRealtime(1);
                scanTrigger.SendMessage("OnTriggerEnter",collider);
                Debug.Log("SCAN_REVIEW flag="+scanFlag+" screen="+Screen.width+"x"+Screen.height);
                yield return new WaitForSecondsRealtime(5);
            }
            Debug.Log("SCAN_REVIEW_END");Destroy(gameObject);yield break;
        }
        foreach(int flag in new[]{20,10,37,11})
        {
            foreach(var trigger in Object.FindObjectsByType<VesselTriggerEntity>())
            {
                if(trigger.GetParameterInt("passTriggerFlag")!=flag)continue;
                trigger.SendMessage("OnTriggerEnter",collider);
                break;
            }
            yield return new WaitForSecondsRealtime(4.25f);
        }
        foreach(var slot in Object.FindObjectsByType<MagnetVesselSlotEntity>())
            if(slot.GetParameterInt("flag")==104) portal=slot;
        playerBody=vessel.GetComponent<Rigidbody>();
        var position=portal.transform.position-Vector3.forward*620;
        playerBody.position=position;vessel.transform.position=position;
        playerBody.linearVelocity=Vector3.zero;vessel.canMove=false;
        approach=true;
        Debug.Log("REFERENCE_APPROACH rect="+World.cameraFollow.GetComponent<Camera>().pixelRect);
        for (int sample = 0; sample < 36; sample++)
        {
            yield return new WaitForSecondsRealtime(.5f);
            var camera = World.cameraFollow.GetComponent<Camera>();
            var sprite = vessel.GfxPuck.GetComponentInChildren<tk2dAnimatedSprite>();
            if (sprite == null) { Debug.Log("PORTAL_PLAYER no active sprite"); continue; }
            var renderer = sprite.GetComponent<Renderer>();
            Debug.Log("PORTAL_PLAYER body=" + camera.WorldToViewportPoint(playerBody.position)
                + " sprite=" + camera.WorldToViewportPoint(renderer.bounds.center)
                + " local=" + sprite.transform.localPosition + " visible=" + renderer.enabled
                + " off=" + renderer.forceRenderingOff + " clip=" + (sprite.CurrentClip == null ? "none" : sprite.CurrentClip.name)
                + " falling=" + vessel.IsFalling);
        }
        Debug.Log("REFERENCE_SEQUENCE_END ball="+World.vessel.transform.position);
        Destroy(gameObject);
    }
    void FixedUpdate()
    {
        if(!approach || portal==null || playerBody==null)return;
        // Hand back control at the native trigger, before the magnet catches the ball.
        var trigger=portal.GetComponentInChildren<Collider>();
        if(trigger!=null && trigger.bounds.Contains(playerBody.position)){approach=false;return;}
        if(playerBody.position.z>=portal.transform.position.z-12){approach=false;return;}
        playerBody.MovePosition(playerBody.position+Vector3.forward*160*Time.fixedDeltaTime);
    }
}
