#if UNITY_EDITOR
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

public static partial class WideReview
{
    static void AuditJump()
    {
        new GameObject("WideReview jump audit").AddComponent<WideReviewJump>().Begin(output);
    }
}

public class WideReviewJump : MonoBehaviour
{
    string folder;
    StandardVessel ball;
    Transform platform;
    Rigidbody body;
    bool hold;
    Vector3 origin;
    public void Begin(string path) { folder=path; StartCoroutine(Run()); }
    void FixedUpdate()
    {
        if(!hold)return;
        platform.position=origin+Vector3.right*Mathf.Sin(Time.time*2f)*30f;
        var p=platform.position; p.y=81f;
        body.position=p; body.linearVelocity=Vector3.zero;
    }
    void Log(string s) { File.AppendAllText(Path.Combine(folder,"jump-audit.txt"),s+"\n"); }
    IEnumerator Run()
    {
        ball=(StandardVessel)World.vessel; body=ball.GetComponent<Rigidbody>();
        origin=new Vector3(2000,81,body.position.z);
        var go=new GameObject("Review platform"); platform=go.transform;platform.position=origin;
        var box=go.AddComponent<BoxCollider>();box.size=new Vector3(100,40,100);box.isTrigger=true;
        var rb=go.AddComponent<Rigidbody>();rb.isKinematic=true;
        var entity=go.AddComponent<AnimatedEntity>();entity.canBeBorded=true;
        go.tag="CanBeBorded";
        var abyss=new GameObject("Review abyss");abyss.tag="Abyss";abyss.transform.position=origin;
        var ac=abyss.AddComponent<BoxCollider>();ac.size=new Vector3(600,40,600);ac.isTrigger=true;
        abyss.AddComponent<AbyssCollision>();
        hold=true;ball.canMove=false;Time.timeScale=1;
        yield return new WaitForSeconds(.2f);
        Log("initial count="+ball.IsOnMovingPlatform+" falling="+ball.IsFalling);
        for(int n=0;n<8;n++)
        {
            ball.TriggerJump();
            yield return new WaitForSeconds(1.8f);
            Log("jump="+n+" count="+ball.IsOnMovingPlatform+" falling="+ball.IsFalling+" air="+ball.IsInAir);
            if(ball.IsFalling)break;
        }
        // Duplicate or late exit must not erase another platform or underflow.
        ball.SetPlatform(platform);ball.RemovePlatform(platform);ball.RemovePlatform(platform);
        Log("duplicate exit count="+ball.IsOnMovingPlatform);
        hold=false;
        body.position=origin+Vector3.right*200f;body.linearVelocity=Vector3.zero;
        yield return new WaitForSeconds(.25f);
        Log("outside platform falling="+ball.IsFalling);
        Time.timeScale=0;
        Log("DONE");
    }
}
#endif
