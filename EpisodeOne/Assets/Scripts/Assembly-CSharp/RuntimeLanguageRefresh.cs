using UnityEngine;
using UnityEngine.SceneManagement;

public static class RuntimeLanguageRefresh
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        GamePreferences.Changed -= Refresh;
        GamePreferences.Changed += Refresh;
        SceneManager.sceneLoaded -= SceneLoaded;
        SceneManager.sceneLoaded += SceneLoaded;
    }
    static void SceneLoaded(Scene scene, LoadSceneMode mode) { Refresh(); }
    public static void Refresh()
    {
        foreach (tk2dTextMesh label in Object.FindObjectsByType<tk2dTextMesh>(FindObjectsSortMode.None))
        {
            if(label.IsLiteralText) continue;
            label.text = GameLocalization.Text(label.text);
            label.Commit();
        }
        foreach (DynamicFont label in Object.FindObjectsByType<DynamicFont>(FindObjectsSortMode.None))
            if(!label.IsLiteralText) label.Text = GameLocalization.Text(label.Text);
    }
}
