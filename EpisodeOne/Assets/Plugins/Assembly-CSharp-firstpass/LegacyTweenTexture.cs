using UnityEngine;

// Unity removed GUITexture. This bridge implements the full-screen overlay used
// by iTween camera fades; scene-specific legacy GUI layouts require verification.
public sealed class LegacyTweenTexture : MonoBehaviour
{
    public Texture texture;
    public Color color = Color.white;

    private void OnGUI()
    {
        if (texture == null || Event.current.type != EventType.Repaint) return;
        var previousColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture, ScaleMode.StretchToFill, true);
        GUI.color = previousColor;
    }
}
