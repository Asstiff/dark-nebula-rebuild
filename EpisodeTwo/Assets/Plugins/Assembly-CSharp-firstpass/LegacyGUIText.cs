using UnityEngine;

// Placeholder for removed GUIText components; scene references and visual
// appearance require reconstruction and validation before shipping.
public sealed class LegacyGUIText : MonoBehaviour
{
    public string text;
    public Material material;
    public Rect pixelInset;
    private void OnGUI()
    {
        GUI.Label(new Rect(pixelInset.x, Screen.height - pixelInset.y - pixelInset.height, pixelInset.width, pixelInset.height), text);
    }
}
