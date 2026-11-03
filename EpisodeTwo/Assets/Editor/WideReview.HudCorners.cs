#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public static partial class WideReview
{
    static void AuditHudCorners()
    {
        Hud hud=Hud.Instance;Camera camera=hud.GetComponentInChildren<Camera>();
        var timeField=typeof(Hud).GetField("GfxTextTime");
        GameObject timer=timeField==null?null:(GameObject)timeField.GetValue(hud);
        Vector3 timerBefore=timer==null?Vector3.zero:timer.transform.position;
        Vector2 originalRadii=LegacyScreenFit.PreviewTopCornerRadii;
        Rect[] originalCutouts=LegacyScreenFit.PreviewCutouts;
        var report=new StringBuilder();
        foreach(Vector2 radii in new[]{Vector2.zero,new Vector2(.072222f,.072222f),new Vector2(.14f,.14f),new Vector2(.22f,.22f),new Vector2(.28f,.28f),new Vector2(.12f,.24f)})
        {
            LegacyScreenFit.PreviewTopCornerRadii=radii;
            foreach(bool sideHole in new[]{false,true})
            {
                LegacyScreenFit.PreviewCutouts=new[]{new Rect(Screen.width*(sideHole?.07f:.46f),Screen.height-Screen.width*.10f,Screen.width*.08f,Screen.width*.1f)};
                UnifiedHudLayout.Apply(camera,hud.GfxIconLifeOrb,hud.GfxTextOrbs,hud.GfxIconLives,hud.GfxTextLives);
                Rect left=UnifiedHudLayout.ScreenBounds(camera,hud.GfxIconLifeOrb,hud.GfxTextOrbs);
                Rect right=UnifiedHudLayout.ScreenBounds(camera,hud.GfxIconLives,hud.GfxTextLives);
                if(Mathf.Abs(left.yMax-right.yMax)>.1f)throw new Exception("Side counters not on one row");
                if(Mathf.Abs(left.xMin-Screen.width*.083f)>.2f || Mathf.Abs(right.xMax-Screen.width*.917f)>.2f)throw new Exception("Side margin changed with radius");
                if(Mathf.Abs(left.yMax-(Screen.height-Screen.width*.083f))>.2f)throw new Exception("HUD height changed with device metadata");
                CheckArc(left.xMin,left.yMax,radii.x*Screen.width);
                CheckArc(Screen.width-right.xMax,right.yMax,radii.y*Screen.width);

                UnifiedHudLayout.Apply(camera,hud.GfxIconLifeOrb,hud.GfxTextOrbs,hud.GfxIconLives,hud.GfxTextLives);
                Rect repeated=UnifiedHudLayout.ScreenBounds(camera,hud.GfxIconLifeOrb,hud.GfxTextOrbs);
                if(Vector2.Distance(left.position,repeated.position)>.1f)throw new Exception("HUD placement drifts");
                if(timer!=null && Vector3.Distance(timerBefore,timer.transform.position)>.001f)throw new Exception("Timer moved");
                report.AppendLine(Screen.width+"x"+Screen.height+" radii="+radii+" sideHole="+sideHole+" left="+left+" right="+right+" PASS");
            }
        }
        LegacyScreenFit.PreviewTopCornerRadii=originalRadii;LegacyScreenFit.PreviewCutouts=originalCutouts;
        UnifiedHudLayout.Apply(camera,hud.GfxIconLifeOrb,hud.GfxTextOrbs,hud.GfxIconLives,hud.GfxTextLives);
        File.AppendAllText(Path.Combine(output,"HUD-CORNERS-PASS.txt"),report.ToString());
    }
    static void CheckArc(float x,float y,float radius)
    {
        if(radius<=0 || x>=radius || y<=Screen.height-radius)return;
        float distance=Vector2.Distance(new Vector2(x,y),new Vector2(radius,Screen.height-radius));
        if(distance>radius+.1f)throw new Exception("HUD outside rounded display: "+distance+" > "+radius);
    }
}
#endif
