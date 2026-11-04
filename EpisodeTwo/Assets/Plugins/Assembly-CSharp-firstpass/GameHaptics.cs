using System;
using UnityEngine;

public enum HapticEvent { Damage, Jump, Pickup, Spawn, Finish, Score, Star }

public static class GameHaptics
{
    static float lastPickup = -10f;
    static float lastScore = -10f;
#if UNITY_ANDROID && !UNITY_EDITOR
    static AndroidJavaObject vibrator;
    static int sdk;
    static bool initialized;
    static void Initialize()
    {
        if (initialized) return;
        initialized = true;
        using (var version = new AndroidJavaClass("android.os.Build$VERSION")) sdk = version.GetStatic<int>("SDK_INT");
        using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
            vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
    }
#endif
    public static void Play(HapticEvent kind)
    {
        VibrationMode mode = GamePreferences.Vibration;
        if (mode == VibrationMode.Disabled) return;
        if (mode == VibrationMode.Original)
        {
            if (kind == HapticEvent.Damage) Handheld.Vibrate();
            return;
        }
        if (kind == HapticEvent.Pickup)
        {
            if (Time.realtimeSinceStartup - lastPickup < .075f) return;
            lastPickup = Time.realtimeSinceStartup;
        }
        if (kind == HapticEvent.Score)
        {
            if (Time.realtimeSinceStartup - lastScore < .8f) return;
            lastScore = Time.realtimeSinceStartup;
        }
        long[] timing;
        int[] strength;
        switch (kind)
        {
            case HapticEvent.Pickup: timing = new long[] { 0, 12 }; strength = new int[] { 0, 55 }; break;
            case HapticEvent.Jump: timing = new long[] { 0, 28 }; strength = new int[] { 0, 145 }; break;
            case HapticEvent.Spawn: timing = new long[] { 0, 20, 40, 30 }; strength = new int[] { 0, 65, 0, 100 }; break;
            case HapticEvent.Finish: timing = new long[] { 0, 30, 55, 55 }; strength = new int[] { 0, 100, 0, 150 }; break;
            case HapticEvent.Score: timing = new long[] { 0, 9, 170, 9, 170, 9, 170, 9, 170, 9 }; strength = new int[] { 0, 40, 0, 45, 0, 50, 0, 55, 0, 60 }; break;
            case HapticEvent.Star: timing = new long[] { 0, 18, 35, 32 }; strength = new int[] { 0, 90, 0, 150 }; break;
            default: timing = new long[] { 0, 65 }; strength = new int[] { 0, 200 }; break;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            Initialize();
            if (vibrator == null || !vibrator.Call<bool>("hasVibrator")) return;
            if (sdk >= 26)
            {
                using (var effectClass = new AndroidJavaClass("android.os.VibrationEffect"))
                using (var effect = effectClass.CallStatic<AndroidJavaObject>("createWaveform", timing, strength, -1))
                    vibrator.Call("vibrate", effect);
            }
            else vibrator.Call("vibrate", timing, -1);
        }
        catch (AndroidJavaException) { }
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    public static void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try { if (vibrator != null) vibrator.Call("cancel"); } catch (AndroidJavaException) { }
#endif
    }
}
