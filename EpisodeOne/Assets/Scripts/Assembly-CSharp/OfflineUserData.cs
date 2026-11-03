using UnityEngine;

// Local test-build progress; no account or network dependency.
public sealed class OfflineUserData
{
    public static readonly OfflineUserData Instance = new OfflineUserData();
    private const string Prefix = "OfflineProgress.";
    public void StoreCustomData(string key, string value) { PlayerPrefs.SetString(Prefix + key, value); PlayerPrefs.Save(); }
    public void StoreCustomData(string key, int value) { StoreCustomData(key, value.ToString()); }
    public void StoreCustomData(string key, float value) { StoreCustomData(key, value.ToString(System.Globalization.CultureInfo.InvariantCulture)); }
    public string GetCustomData(string key) { return PlayerPrefs.GetString(Prefix + key, ""); }
    public bool HasCustomData(string key) { return PlayerPrefs.HasKey(Prefix + key); }
    public int? GetCustomInteger(string key) { int n; return int.TryParse(GetCustomData(key), out n) ? n : (int?)null; }
    public float? GetCustomFloat(string key) { float n; return float.TryParse(GetCustomData(key), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out n) ? n : (float?)null; }
    public void DeleteCustomData(string key) { PlayerPrefs.DeleteKey(Prefix + key); PlayerPrefs.Save(); }
}
