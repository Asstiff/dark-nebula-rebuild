using System;
using UnityEngine;

// Keep interactive content inside the display's safe rectangle while scenery
// continues to render edge to edge.
public static class LegacyScreenFit
{
    public const float ReferenceAspect = 2f / 3f;

#if UNITY_EDITOR
    public static Rect PreviewSafeArea;
    public static Rect[] PreviewCutouts;
    public static Vector2 PreviewTopCornerRadii; // normalized by screen width
#endif

    public static Rect ContentRect()
    {
        float sw = Screen.width, sh = Screen.height;
#if UNITY_EDITOR
        if (PreviewSafeArea.width > 0f && PreviewSafeArea.height > 0f) return PreviewSafeArea;
#endif
        if (sw <= 0f || sh <= 0f) return new Rect(0f, 0f, 1f, 1f);
        Rect safe = Screen.safeArea;
        if (safe.width <= 0f || safe.height <= 0f) safe = new Rect(0f, 0f, sw, sh);

        float padding = Application.isMobilePlatform ? Mathf.Min(sw, sh) * 0.02f : 0f;
        Vector4 cornerInsets = Vector4.zero; // left, bottom, right, top
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidCorners.Request();
        cornerInsets = AndroidCorners.Insets;
#endif
        float left = Mathf.Max(safe.xMin, Mathf.Max(padding, cornerInsets.x * sw));
        float bottom = Mathf.Max(safe.yMin, Mathf.Max(padding, cornerInsets.y * sh));
        float right = Mathf.Min(safe.xMax, Mathf.Min(sw - padding, sw * (1f - cornerInsets.z)));
        float top = Mathf.Min(safe.yMax, Mathf.Min(sh - padding, sh * (1f - cornerInsets.w)));
        if (right <= left || top <= bottom) return safe;
        return Rect.MinMaxRect(left, bottom, right, top);
    }

    public static float HudTop(float xMin,float xMax)
    {
        float padding=Screen.width*0.015f;
        float top=Screen.height-padding;
        Rect[] cutouts=Screen.cutouts;
#if UNITY_EDITOR
        if(PreviewCutouts!=null)cutouts=PreviewCutouts;
#endif
        foreach(Rect cutout in cutouts)
            if(cutout.yMax>Screen.height*0.5f && xMax>cutout.xMin-padding && xMin<cutout.xMax+padding)
                top=Mathf.Min(top,cutout.yMin-padding);
        return top;
    }

    // The safe rectangle positions controls, never a reduced camera viewport.
    // All scenery and page backgrounds render to the complete display.
    public static Rect FrameViewport()
    {
        return new Rect(0f,0f,1f,1f);
    }

    public static void CoverBackdrop(Camera camera, Renderer backdrop, Vector3 nativeScale, Vector3 nativePosition)
    {
        Transform image = backdrop.transform;
        image.localScale = nativeScale;
        image.localPosition = nativePosition;
        Bounds bounds=backdrop.bounds;
        if (bounds.size.x <= 0f || bounds.size.y <= 0f) return;

        // One scale preserves the artwork's proportions. On a tall display the
        // image fills the height and its left/right edges fall outside the view.
        float height = camera.orthographicSize * 2f;
        float scale = Mathf.Max(height * camera.aspect / bounds.size.x, height / bounds.size.y);
        // Some recovered backdrops are XZ planes rotated into the view. Scale
        // all local axes equally so their artwork keeps the same proportions.
        image.localScale = nativeScale * scale;
        Vector3 offset = camera.transform.position - backdrop.bounds.center;
        image.position += camera.transform.right * Vector3.Dot(offset, camera.transform.right)
            + camera.transform.up * Vector3.Dot(offset, camera.transform.up);
    }

    public static float MainMenuCameraSize(Camera camera)
    {
        // The menu title is baked into its artwork. Keep its original 320-unit
        // horizontal field instead of cropping more of it on a taller display.
        return 160f / Mathf.Max(camera.aspect,0.01f);
    }

    // Read the fixed pause artwork as the limit for the level camera framing.
    // The original HUD at the 2:3 reference aspect has a 256-unit half-height.
    // Apply the same zoom factor to gameplay/light cameras to keep their proportions.
    public static float LevelCameraSize(Camera camera, float referenceSize)
    {
        float aspect = Mathf.Max(camera.aspect, 0.01f);
        float scale = Mathf.Max(1f, ReferenceAspect / aspect);
        foreach (PauseScreen pause in UnityEngine.Object.FindObjectsByType<PauseScreen>(FindObjectsInactive.Include))
        {
            if (pause.gameObject.scene != camera.gameObject.scene) continue;
            Transform background = pause.transform.Find("Background");
            Renderer renderer = background != null ? background.GetComponent<Renderer>() : null;
            if (renderer == null) continue;
            Bounds bounds = renderer.bounds;
            float limit = Mathf.Min(bounds.extents.y, bounds.extents.x / aspect);
            scale = Mathf.Min(scale, limit / 256f);
            break;
        }
        return referenceSize * scale;
    }

    // Insets move the interactive anchors, rather than changing the field of view.
    public static Vector3 HudAnchor(Camera camera, Vector3 anchor)
    {
        Rect safe = ContentRect();
        Rect full = new Rect(0f, 0f, Screen.width, Screen.height);
        if (full.width <= 0f || full.height <= 0f) return camera.ViewportToWorldPoint(anchor);
        anchor.x = (safe.x + safe.width * anchor.x) / full.width;
        anchor.y = (safe.y + safe.height * anchor.y) / full.height;
        return camera.ViewportToWorldPoint(anchor);
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    // Android 12+ exposes rounded corners separately from display-cutout insets.
    // Query View/WindowInsets on the Android UI thread and cache normalized values.
    private static class AndroidCorners
    {
        private static readonly object Sync = new object();
        private static Vector4 _insets;
        private static Vector2 _topRadii;
        private static bool _pending;
        private static float _nextRequest;
        private static int _width, _height;

        public static Vector4 Insets { get { lock (Sync) return _insets; } }
        public static Vector2 TopRadii { get { lock (Sync) return _topRadii; } }

        public static void Request()
        {
            int width = Screen.width, height = Screen.height;
            float now = Time.realtimeSinceStartup;
            lock (Sync)
            {
                if (_pending || (width == _width && height == _height && now < _nextRequest)) return;
                _pending = true;
                _width = width;
                _height = height;
                _nextRequest = now + 10f;
            }
            try
            {
                using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(ReadOnUiThread));
            }
            catch (Exception)
            {
                lock (Sync) _pending = false;
            }
        }

        private static void ReadOnUiThread()
        {
            try
            {
                using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                {
                    if (version.GetStatic<int>("SDK_INT") < 31) return;
                }
                using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var window = activity.Call<AndroidJavaObject>("getWindow"))
                using (var view = window.Call<AndroidJavaObject>("getDecorView"))
                using (var insets = view.Call<AndroidJavaObject>("getRootWindowInsets"))
                {
                    if (insets == null) return;
                    int width = view.Call<int>("getWidth"), height = view.Call<int>("getHeight");
                    if (width <= 0 || height <= 0) return;
                    float left = 0f, bottom = 0f, right = 0f, top = 0f;
                    Vector2 topRadii=Vector2.zero;
                    for (int position = 0; position < 4; position++)
                    {
                        using (var corner = insets.Call<AndroidJavaObject>("getRoundedCorner", position))
                        {
                            if (corner == null) continue;
                            if(position==0)topRadii.x=(float)corner.Call<int>("getRadius")/width;
                            if(position==1)topRadii.y=(float)corner.Call<int>("getRadius")/width;
                            using (var center = corner.Call<AndroidJavaObject>("getCenter"))
                            {
                                int x = center.Get<int>("x"), y = center.Get<int>("y");
                                if (position == 0 || position == 3) left = Math.Max(left, (float)x / width);
                                if (position == 1 || position == 2) right = Math.Max(right, (float)(width - x) / width);
                                if (position == 0 || position == 1) top = Math.Max(top, (float)y / height);
                                if (position == 2 || position == 3) bottom = Math.Max(bottom, (float)(height - y) / height);
                            }
                        }
                    }
                    lock (Sync) { _insets = new Vector4(left, bottom, right, top); _topRadii=topRadii; }
                }
            }
            catch (Exception) { /* Keep the safe-area and minimum-padding fallback. */ }
            finally { lock (Sync) _pending = false; }
        }
    }
#endif
}
