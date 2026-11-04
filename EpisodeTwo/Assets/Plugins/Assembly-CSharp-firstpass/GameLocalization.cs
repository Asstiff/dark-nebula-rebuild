using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameLocalization
{
    [Serializable] public class Entry { public string key, en, zh; }
    [Serializable] public class Catalog { public Entry[] entries; }
    static Dictionary<string, Entry> lookup;
    static Font chineseFont;
    public static Font ChineseFont { get { return chineseFont ?? (chineseFont = Resources.Load<Font>("Fonts/DarkNebulaCJKFullWidth-Regular")); } }
    static void Load()
    {
        if (lookup != null) return;
        lookup = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
        TextAsset asset = Resources.Load<TextAsset>("Localization");
        if (asset == null) return;
        foreach (Entry entry in JsonUtility.FromJson<Catalog>(asset.text).entries)
        {
            lookup[entry.key] = entry;
            lookup[entry.en] = entry;
            if (!lookup.ContainsKey(entry.zh)) lookup[entry.zh] = entry;
        }
    }
    public static bool TryGet(string value, out string result)
    {
        Load();
        Entry entry;
        if (value != null && lookup.TryGetValue(value, out entry))
        {
            // Episode Two's recovered bitmap alphabet contains uppercase Latin only.
            result = GamePreferences.Chinese ? entry.zh : entry.en.ToUpperInvariant();
            return true;
        }
        result = value;
        return false;
    }
    public static string Text(string value)
    {
        if (string.IsNullOrEmpty(value)) return value ?? string.Empty;
        string result;
        if (TryGet(value, out result)) return result;
        if (value.Contains("\n"))
        {
            string[] lines = value.Split('\n');
            for (int i = 0; i < lines.Length; i++) lines[i] = Text(lines[i]);
            return string.Join("\n", lines);
        }
        if (value.Length > 3 && char.IsDigit(value[0]) && char.IsDigit(value[1]) && value[2] == ' ')
            return value.Substring(0, 3) + Text(value.Substring(3));
        return value;
    }
    public static string Format(string key, params object[] values) { return string.Format(Text(key), values); }
    public static bool ContainsChinese(string value)
    {
        if (value != null) foreach (char c in value) if (c >= 0x2e80 && c <= 0x9fff) return true;
        return false;
    }
}
