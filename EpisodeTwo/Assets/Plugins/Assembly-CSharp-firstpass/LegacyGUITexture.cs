using UnityEngine;

// Compatibility component for the optional Sphero controls. Original Unity
// GUITexture components must be rebuilt and their fields reconnected in scenes.
public sealed class LegacyGUITexture : MonoBehaviour
{
    public Texture texture;
    public Rect pixelInset;
    public Color color = Color.white;

    public bool HitTest(Vector3 position)
    {
        return ScreenRect().Contains(new Vector2(position.x, position.y));
    }

    private Rect ScreenRect()
    {
        return new Rect(pixelInset.x + transform.position.x * Screen.width,
            pixelInset.y + transform.position.y * Screen.height, pixelInset.width, pixelInset.height);
    }

    private void OnGUI()
    {
        if (texture == null) return;
        Rect r = ScreenRect();
        r.y = Screen.height - r.y - r.height;
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(r, texture);
        GUI.color = previous;
    }
}
