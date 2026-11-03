using UnityEngine;

public class SpriteAnimation
{
	private tk2dSprite _sprite;

	private string spriteName;

	private float frameRate;

	private bool hasFetchedSpriteInformation;

	private bool shouldRandomizeStartFrame;

	private bool isPlaying;

	private bool shouldLoop;

	private int numberOfFrames;

	private float frameFloat;

	private int frame;

	private int[] spriteAnimationIds;

	public SpriteAnimation(tk2dSprite sprite, string name, float pFrameRate)
	{
		spriteName = name + "/";
		_sprite = sprite;
		frameRate = pFrameRate;
		numberOfFrames = GetFrameCount();
		hasFetchedSpriteInformation = false;
	}

	private int GetFrameCount()
	{
		int num = 0;
		for (int num2 = 1; num2 > 0; num2 = _sprite.GetSpriteIdByName(spriteName + num))
		{
			num++;
		}
		return num;
	}

	public void RandomizeStartFrame()
	{
		shouldRandomizeStartFrame = true;
	}

	public void PlaySpriteAnimation()
	{
		isPlaying = true;
		shouldLoop = false;
	}

	public void LoopSpriteAnimation()
	{
		isPlaying = true;
		shouldLoop = true;
	}

	private void FetchSpriteInformation()
	{
		hasFetchedSpriteInformation = true;
		if (shouldRandomizeStartFrame)
		{
			frameFloat = Random.Range(0, numberOfFrames);
		}
		spriteAnimationIds = new int[numberOfFrames];
		for (int i = 0; i < numberOfFrames; i++)
		{
			spriteAnimationIds[i] = _sprite.GetSpriteIdByName(spriteName + i);
		}
	}

	public void Update()
	{
		if (!isPlaying)
		{
			return;
		}
		if (!hasFetchedSpriteInformation)
		{
			FetchSpriteInformation();
		}
		int num = frame;
		if (shouldLoop)
		{
			frame = (int)frameFloat % numberOfFrames;
		}
		else
		{
			frame = (int)frameFloat;
			if (frame >= numberOfFrames)
			{
				isPlaying = false;
				return;
			}
		}
		if (frame != num)
		{
			_sprite.spriteId = spriteAnimationIds[frame];
		}
		frameFloat += frameRate * Time.deltaTime * 30f;
	}

	private void SetAdaptiveBlending(bool b)
	{
		Debug.Log("TODO SetAdaptiveBlending " + b);
	}
}
