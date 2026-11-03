using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Records actual projection geometry, independent of screenshot size or device DPI.
// No gameplay state, input, profile or score data is read or changed.
public sealed class ViewportDiagnostics : MonoBehaviour
{
    [Serializable] public sealed class Projection
    {
        public string scene, camera, platform;
        public int screenWidth, screenHeight;
        public Rect viewport, safeArea;
        public float aspect, halfHeight, horizontalWorldSpan, verticalWorldSpan;
        public float projectionX, projectionY;
        public bool renderTexture;
    }

    public static Projection Describe(Camera camera)
    {
        Vector3 left = camera.ViewportToWorldPoint(new Vector3(0f, .5f, camera.nearClipPlane));
        Vector3 right = camera.ViewportToWorldPoint(new Vector3(1f, .5f, camera.nearClipPlane));
        Vector3 bottom = camera.ViewportToWorldPoint(new Vector3(.5f, 0f, camera.nearClipPlane));
        Vector3 top = camera.ViewportToWorldPoint(new Vector3(.5f, 1f, camera.nearClipPlane));
        return new Projection {
            scene = camera.gameObject.scene.name, camera = camera.name, platform = Application.platform.ToString(),
            screenWidth = Screen.width, screenHeight = Screen.height, viewport = camera.rect, safeArea = Screen.safeArea,
            aspect = camera.aspect, halfHeight = camera.orthographicSize,
            horizontalWorldSpan = Vector3.Distance(left, right), verticalWorldSpan = Vector3.Distance(bottom, top),
            projectionX = camera.projectionMatrix.m00, projectionY = camera.projectionMatrix.m11,
            renderTexture = camera.targetTexture != null
        };
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Begin()
    {
        var root = new GameObject("Viewport diagnostics");
        DontDestroyOnLoad(root);
        root.AddComponent<ViewportDiagnostics>();
    }

    void OnEnable() { SceneManager.sceneLoaded += Changed; StartCoroutine(Record()); }
    void OnDisable() { SceneManager.sceneLoaded -= Changed; }
    void Changed(Scene scene, LoadSceneMode mode) { StopAllCoroutines(); StartCoroutine(Record()); }

    IEnumerator Record()
    {
        yield return new WaitForSecondsRealtime(2f);
        yield return new WaitForEndOfFrame();
        foreach (Camera camera in Camera.allCameras)
            Debug.Log("VIEWPORT_AUDIT " + JsonUtility.ToJson(Describe(camera)));
    }
}
