public static class tk2dExtensions
{
	public static int[] GetFrameIds(this tk2dBaseSprite sprite, string spriteName, int numberOfFrames)
	{
		int[] array = new int[numberOfFrames];
		for (int i = 0; i < numberOfFrames; i++)
		{
			array[i] = sprite.GetSpriteIdByName(spriteName + i);
		}
		return array;
	}
}
