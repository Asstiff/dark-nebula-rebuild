using System;

[Serializable]
public class tk2dCameraResolutionOverride
{
	public int width;

	public int height;

	public float scale;

	public bool Match(int pixelWidth, int pixelHeight)
	{
		return pixelWidth == width && pixelHeight == height;
	}
}
