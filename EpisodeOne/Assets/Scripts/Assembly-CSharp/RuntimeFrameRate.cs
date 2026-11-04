using System;
using UnityEngine;
using UnityEngine.Rendering;

// Mobile targets must follow the current display mode: a fixed 60 target can
// become 45 fps on a 90 Hz display under Unity's integer-divisor frame pacing.
public sealed class RuntimeFrameRate : MonoBehaviour
{
    const int FallbackRate = 60;
    const float RefreshCheckSeconds = 1f;
    static RuntimeFrameRate instance;
    bool applicationPaused;
    bool focused;
    float nextRefreshCheck;
    public static int CurrentTarget { get; private set; }
    public static double CurrentDisplayRate { get; private set; }

    public static int TargetForRefreshRate(double rate)
    {
        if (double.IsNaN(rate) || double.IsInfinity(rate) || rate <= 0 || rate > int.MaxValue)
            return FallbackRate;
        return Math.Max(1, (int)Math.Round(rate, MidpointRounding.AwayFromZero));
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetRuntime()
    {
        instance = null;
        CurrentTarget = 0;
        CurrentDisplayRate = 0;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (instance != null) return;
        var owner = new GameObject("Runtime frame pacing");
        DontDestroyOnLoad(owner);
        instance = owner.AddComponent<RuntimeFrameRate>();
        instance.focused = Application.isFocused;
        ApplyDisplayRate();
    }

    static void ApplyDisplayRate()
    {
        double rate = Screen.currentResolution.refreshRateRatio.value;
        int target = TargetForRefreshRate(rate);
        bool changed = CurrentTarget != target || Application.targetFrameRate != target;
        CurrentDisplayRate = rate;
        CurrentTarget = target;
        if (Application.targetFrameRate != target) Application.targetFrameRate = target;
        // Every foreground update should be presented. Paused/background frames
        // remain governed by Unity's existing runInBackground and game pause logic.
        if (OnDemandRendering.renderFrameInterval != 1) OnDemandRendering.renderFrameInterval = 1;
        if (changed)
            Debug.Log("[Frame pacing] display=" + rate.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)
                + "Hz target=" + target + " vSync=" + QualitySettings.vSyncCount
                + " renderInterval=" + OnDemandRendering.renderFrameInterval);
    }

    void Update()
    {
        if (applicationPaused || !focused || Time.timeScale <= 0f || Time.unscaledTime < nextRefreshCheck) return;
        nextRefreshCheck = Time.unscaledTime + RefreshCheckSeconds;
        ApplyDisplayRate();
    }

    void OnApplicationPause(bool paused)
    {
        applicationPaused = paused;
        if (!paused) RefreshAfterResume();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        focused = hasFocus;
        if (hasFocus) RefreshAfterResume();
    }

    void RefreshAfterResume()
    {
        nextRefreshCheck = 0f;
        if (!applicationPaused && focused && Time.timeScale > 0f) ApplyDisplayRate();
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
