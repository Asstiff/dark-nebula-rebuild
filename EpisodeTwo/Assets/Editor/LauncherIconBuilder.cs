#if UNITY_EDITOR && UNITY_ANDROID
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEngine;

// Build Android icon layers from the unmodified iOS 512px artwork.
internal static class LauncherIconBuilder
{
    private const int Size = 512;
    private const string Source = "Assets/Icons/IOSIcon.png";
    private const string Foreground = "Assets/Icons/AndroidAdaptiveForeground.png";
    private const string Background = "Assets/Icons/AndroidAdaptiveBackground.png";
    private const string Round = "Assets/Icons/AndroidRoundIcon.png";

    public static void SetIcons()
    {
        if (!File.Exists(Source)) throw new Exception("The iOS icon artwork is missing: " + Source);
        var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!source.LoadImage(File.ReadAllBytes(Source))) throw new Exception("Cannot read iOS icon artwork");
        source.wrapMode = TextureWrapMode.Clamp;
        try
        {
            // Fill the visible adaptive icon with the original artwork; extend its edges
            // into Android's extra overscan area instead of adding a black frame.
            Write(Foreground, MakeLayer(null, 0f, new Color32(0, 0, 0, 0)));
            Write(Background, MakeExtendedLayer(source, 0.62f));
            Write(Round, MakeLayer(source, 1f, new Color32(18, 18, 20, 255)));
        }
        finally { UnityEngine.Object.DestroyImmediate(source); }

        foreach (var path in new[] { Foreground, Background, Round, Source })
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = Size;
            importer.SaveAndReimport();
        }

        var fg = AssetDatabase.LoadAssetAtPath<Texture2D>(Foreground);
        var bg = AssetDatabase.LoadAssetAtPath<Texture2D>(Background);
        var round = AssetDatabase.LoadAssetAtPath<Texture2D>(Round);
        var legacy = AssetDatabase.LoadAssetAtPath<Texture2D>(Source);
        if (fg == null || bg == null || round == null || legacy == null) throw new Exception("Unable to import Android launcher icons");

        var adaptive = PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android, AndroidPlatformIconKind.Adaptive);
        foreach (var icon in adaptive)
        {
            icon.SetTexture(bg, 0);
            icon.SetTexture(fg, 1);
        }
        PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android, AndroidPlatformIconKind.Adaptive, adaptive);
        SetSingle(AndroidPlatformIconKind.Round, round);
        SetSingle(AndroidPlatformIconKind.Legacy, legacy);
    }

    private static void SetSingle(PlatformIconKind kind, Texture2D texture)
    {
        var icons = PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android, kind);
        foreach (var icon in icons) icon.SetTexture(texture, 0);
        PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android, kind, icons);
    }

    private static Texture2D MakeExtendedLayer(Texture2D source, float fraction)
    {
        var image = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
        var pixels = new Color32[Size * Size];
        float start = (1f - fraction) * 0.5f;
        for (int y = 0; y < Size; y++)
        for (int x = 0; x < Size; x++)
        {
            float u = Mathf.Clamp01(((x + 0.5f) / Size - start) / fraction);
            float v = Mathf.Clamp01(((y + 0.5f) / Size - start) / fraction);
            pixels[y * Size + x] = source.GetPixelBilinear(u, v);
        }
        image.SetPixels32(pixels);
        image.Apply();
        return image;
    }

    private static Texture2D MakeLayer(Texture2D source, float fraction, Color32 fill)
    {
        var image = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
        var pixels = new Color32[Size * Size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = fill;
        if (source != null)
        {
            int side = Mathf.RoundToInt(Size * fraction);
            int start = (Size - side) / 2;
            for (int y = 0; y < side; y++)
            for (int x = 0; x < side; x++)
            {
                Color color = source.GetPixelBilinear((x + 0.5f) / side, (y + 0.5f) / side);
                if (fill.a != 0) color = Color.Lerp((Color)fill, color, color.a);
                pixels[(start + y) * Size + start + x] = color;
            }
        }
        image.SetPixels32(pixels);
        image.Apply();
        return image;
    }

    private static void Write(string path, Texture2D texture)
    {
        try { File.WriteAllBytes(path, texture.EncodeToPNG()); }
        finally { UnityEngine.Object.DestroyImmediate(texture); }
    }
}
#endif
