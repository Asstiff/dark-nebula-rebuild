using UnityEngine;

[DefaultExecutionOrder(200)]
public class MenuRowLayout : MonoBehaviour
{
    public GameObject[] Rows;
    void LateUpdate()
    {
        if(Rows==null || Rows.Length==0 || Rows[0]==null)return;
        var button=Rows[0].GetComponent<SimpleButton>();
        if(button==null || !button.enabled || button.viewCamera==null)return;
        Camera camera=button.viewCamera;
        Rect safe=LegacyScreenFit.ContentRect();
        float step=Mathf.Min(Screen.width*0.14f,safe.height*0.095f);
        float top=Mathf.Clamp(Screen.height*0.55f,safe.yMin+step*(Rows.Length-1)+Screen.width*0.06f,safe.yMax-Screen.width*0.1f);
        for(int i=0;i<Rows.Length;i++)
        {
            if(Rows[i]==null || !Rows[i].activeSelf)continue;
            Vector3 p=camera.WorldToScreenPoint(Rows[i].transform.position);
            Rect bounds=UnifiedHudLayout.ScreenBounds(camera,Rows[i],null);
            p.y+=top-i*step-bounds.center.y;
            Rows[i].transform.position=camera.ScreenToWorldPoint(p);
        }
    }
}
