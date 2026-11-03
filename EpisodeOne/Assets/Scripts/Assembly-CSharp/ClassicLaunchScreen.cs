using UnityEngine;
using UnityEngine.SceneManagement;

// Original 1.3 launch artwork, shown once per cold start at its native aspect.
public class ClassicLaunchScreen : MonoBehaviour
{
    static bool shown;
    public static bool Visible {get;private set;}
    float since=-1f;
    Camera camera;
    Material material;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset(){shown=false;Visible=false;}
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Show()
    {
        if(shown || SceneManager.GetActiveScene().name!="MainMenu")return;
        shown=true;
        Texture2D art=Resources.Load<Texture2D>("ClassicLaunch");
        if(art==null)return;
        var root=new GameObject("Original Episode One launch");
        var screen=root.AddComponent<ClassicLaunchScreen>();
        screen.camera=root.AddComponent<Camera>();
        screen.camera.clearFlags=CameraClearFlags.SolidColor;screen.camera.backgroundColor=Color.black;
        screen.camera.orthographic=true;screen.camera.depth=2000;
        screen.camera.nearClipPlane=0.1f;screen.camera.farClipPlane=10f;
        screen.camera.cullingMask=1<<31;
        var quad=GameObject.CreatePrimitive(PrimitiveType.Quad);quad.name="Original launch artwork";
        Object.Destroy(quad.GetComponent<Collider>());
        quad.layer=31;quad.transform.SetParent(root.transform,false);
        quad.transform.localPosition=new Vector3(0,0,1);quad.transform.localScale=new Vector3(320,480,1);
        Material source=Resources.Load<Material>("ClassicLaunchMaterial");
        screen.material=source!=null?new Material(source):new Material(Shader.Find("Unlit/Transparent"));screen.material.mainTexture=art;
        quad.GetComponent<Renderer>().sharedMaterial=screen.material;
        Visible=true;
    }
    void Update()
    {
        camera.orthographicSize=Mathf.Max(240f,160f*Screen.height/Mathf.Max(Screen.width,1));
        if(since<0f)return;
        float elapsed=Time.realtimeSinceStartup-since;
        material.color=new Color(1,1,1,Mathf.Clamp01((1.5f-elapsed)/0.25f));
        if(elapsed>=1.5f){Visible=false;Object.Destroy(gameObject);}
    }
    void OnPostRender()
    {
        // Cold-start asset uploads can delay the first frame. Count only time
        // for which the original launch artwork has actually been presented.
        if(since<0f)since=Time.realtimeSinceStartup;
    }
    void OnDestroy(){Visible=false;if(material!=null)Object.Destroy(material);}
}
