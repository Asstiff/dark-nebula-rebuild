using System;
using UnityEngine;

public enum VibrationMode { Disabled, Original, Enhanced }

public static class GamePreferences
{
    public static event Action Changed;
    public static bool ShowTimer
    {
        get { return PlayerPrefs.GetInt("Gameplay.ShowTimer", 1) != 0; }
        set { PlayerPrefs.SetInt("Gameplay.ShowTimer", value ? 1 : 0); Save(); }
    }
    public static VibrationMode Vibration
    {
        get { return (VibrationMode)Mathf.Clamp(PlayerPrefs.GetInt("Gameplay.VibrationMode", 2), 0, 2); }
        set { PlayerPrefs.SetInt("Gameplay.VibrationMode", (int)value); Save(); }
    }
    public static bool Chinese
    {
        get
        {
            SystemLanguage language = Application.systemLanguage;
            bool systemChinese = language == SystemLanguage.Chinese || language == SystemLanguage.ChineseSimplified || language == SystemLanguage.ChineseTraditional;
            return PlayerPrefs.GetInt("Gameplay.Chinese", systemChinese ? 1 : 0) != 0;
        }
        set { PlayerPrefs.SetInt("Gameplay.Chinese", value ? 1 : 0); Save(); }
    }
    static void Save() { PlayerPrefs.Save(); if (Changed != null) Changed(); }
}
