using UnityEngine;

[AddComponentMenu("2D Toolkit/Sprite/tk2dAnimatedSprite")]
public class tk2dAnimatedSprite : tk2dSprite
{
	public delegate void AnimationCompleteDelegate(tk2dAnimatedSprite sprite, int clipId);

	public delegate void AnimationEventDelegate(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum);

	public tk2dSpriteAnimation anim;

	public int clipId;

	public bool playAutomatically;

	public static bool g_paused;

	public bool paused;

	public bool createCollider;

	private tk2dSpriteAnimationClip currentClip;

	private float clipTime;

	private int previousFrame = -1;

	public AnimationCompleteDelegate animationCompleteDelegate;

	public AnimationEventDelegate animationEventDelegate;

	public tk2dSpriteAnimationClip CurrentClip
	{
		get
		{
			return currentClip;
		}
	}

	public float ClipTimeSeconds
	{
		get
		{
			return clipTime / currentClip.fps;
		}
	}

	private new void Start()
	{
		base.Start();
		if (playAutomatically)
		{
			Play(clipId);
		}
	}

	public static tk2dAnimatedSprite AddComponent(GameObject go, tk2dSpriteAnimation anim, int clipId)
	{
		tk2dSpriteAnimationClip tk2dSpriteAnimationClip2 = anim.clips[clipId];
		tk2dAnimatedSprite tk2dAnimatedSprite2 = go.AddComponent<tk2dAnimatedSprite>();
		tk2dAnimatedSprite2.collection = tk2dSpriteAnimationClip2.frames[0].spriteCollection;
		tk2dAnimatedSprite2.spriteId = tk2dSpriteAnimationClip2.frames[0].spriteId;
		tk2dAnimatedSprite2.anim = anim;
		return tk2dAnimatedSprite2;
	}

	public void Play()
	{
		Play(clipId);
	}

	public void Play(float clipStartTime)
	{
		Play(clipId, clipStartTime);
	}

	public void PlayFromFrame(int frame)
	{
		PlayFromFrame(clipId, frame);
	}

	public void Play(string name)
	{
		int id = ((!anim) ? (-1) : anim.GetClipIdByName(name));
		Play(id);
	}

	public void PlayFromFrame(string name, int frame)
	{
		int id = ((!anim) ? (-1) : anim.GetClipIdByName(name));
		PlayFromFrame(id, frame);
	}

	public void Play(string name, float clipStartTime)
	{
		int id = ((!anim) ? (-1) : anim.GetClipIdByName(name));
		Play(id, clipStartTime);
	}

	public void Stop()
	{
		currentClip = null;
	}

	public void StopAndResetFrame()
	{
		if (currentClip != null)
		{
			SwitchCollectionAndSprite(currentClip.frames[0].spriteCollection, currentClip.frames[0].spriteId);
		}
		Stop();
	}

	public bool isPlaying()
	{
		return currentClip != null;
	}

	protected override bool NeedBoxCollider()
	{
		return createCollider;
	}

	public int GetClipIdByName(string name)
	{
		return (!anim) ? (-1) : anim.GetClipIdByName(name);
	}

	public void Play(int id)
	{
		Play(id, 0f);
	}

	public void PlayFromFrame(int id, int frame)
	{
		tk2dSpriteAnimationClip tk2dSpriteAnimationClip2 = anim.clips[id];
		Play(id, (float)frame / tk2dSpriteAnimationClip2.fps);
	}

	public void Play(int id, float clipStartTime)
	{
		clipId = id;
		if (id >= 0 && (bool)anim && id < anim.clips.Length)
		{
			currentClip = anim.clips[id];
			if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Single || currentClip.frames == null)
			{
				SwitchCollectionAndSprite(currentClip.frames[0].spriteCollection, currentClip.frames[0].spriteId);
				if (currentClip.frames[0].triggerEvent && animationEventDelegate != null)
				{
					animationEventDelegate(this, currentClip, currentClip.frames[0], 0);
				}
				currentClip = null;
			}
			else if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomFrame || currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomLoop)
			{
				int num = Random.Range(0, currentClip.frames.Length - 1);
				tk2dSpriteAnimationFrame tk2dSpriteAnimationFrame2 = currentClip.frames[num];
				clipTime = (float)num * currentClip.fps;
				SwitchCollectionAndSprite(tk2dSpriteAnimationFrame2.spriteCollection, tk2dSpriteAnimationFrame2.spriteId);
				if (tk2dSpriteAnimationFrame2.triggerEvent && animationEventDelegate != null)
				{
					animationEventDelegate(this, currentClip, tk2dSpriteAnimationFrame2, num);
				}
				if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomFrame)
				{
					currentClip = null;
					previousFrame = -1;
				}
			}
			else
			{
				SwitchCollectionAndSprite(currentClip.frames[0].spriteCollection, currentClip.frames[0].spriteId);
				clipTime = clipStartTime * currentClip.fps;
				previousFrame = -1;
				if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Once && clipTime >= currentClip.fps * (float)currentClip.frames.Length)
				{
					clipTime = currentClip.fps * ((float)currentClip.frames.Length - 0.1f);
				}
			}
		}
		else
		{
			OnCompleteAnimation();
			currentClip = null;
		}
	}

	public void Pause()
	{
		paused = true;
	}

	public void Resume()
	{
		paused = false;
	}

	private void OnCompleteAnimation()
	{
		previousFrame = -1;
		if (animationCompleteDelegate != null)
		{
			animationCompleteDelegate(this, clipId);
		}
	}

	public void SetFrame(int currFrame)
	{
		if (currentClip != null && currentClip.frames.Length > 0 && currFrame >= 0)
		{
			int num = currFrame % currentClip.frames.Length;
			SetFrameInternal(num);
			ProcessEvents(num - 1, num, 1);
		}
	}

	private void SetFrameInternal(int currFrame)
	{
		if (previousFrame != currFrame)
		{
			SwitchCollectionAndSprite(currentClip.frames[currFrame].spriteCollection, currentClip.frames[currFrame].spriteId);
			previousFrame = currFrame;
		}
	}

	private void ProcessEvents(int start, int last, int direction)
	{
		if (animationEventDelegate == null || start == last)
		{
			return;
		}
		int num = last + direction;
		tk2dSpriteAnimationFrame[] frames = currentClip.frames;
		for (int i = start + direction; i != num; i += direction)
		{
			if (frames[i].triggerEvent)
			{
				animationEventDelegate(this, currentClip, frames[i], i);
			}
		}
	}

	private void Update()
	{
		if (g_paused || paused || currentClip == null || currentClip.frames == null)
		{
			return;
		}
		clipTime += Time.deltaTime * currentClip.fps;
		int num = previousFrame;
		switch (currentClip.wrapMode)
		{
		case tk2dSpriteAnimationClip.WrapMode.Loop:
		case tk2dSpriteAnimationClip.WrapMode.RandomLoop:
		{
			int num5 = (int)clipTime % currentClip.frames.Length;
			SetFrameInternal(num5);
			if (num5 < num)
			{
				ProcessEvents(num, currentClip.frames.Length - 1, 1);
				ProcessEvents(-1, num5, 1);
			}
			else
			{
				ProcessEvents(num, num5, 1);
			}
			break;
		}
		case tk2dSpriteAnimationClip.WrapMode.LoopSection:
		{
			int num3 = (int)clipTime;
			int num4 = currentClip.loopStart + (num3 - currentClip.loopStart) % (currentClip.frames.Length - currentClip.loopStart);
			SetFrameInternal(num3);
			if (num3 >= currentClip.loopStart)
			{
				num3 = num4;
				if (num < currentClip.loopStart)
				{
					ProcessEvents(num, currentClip.loopStart - 1, 1);
					ProcessEvents(currentClip.loopStart - 1, num3, 1);
				}
				else if (num3 < num)
				{
					ProcessEvents(num, currentClip.frames.Length - 1, 1);
					ProcessEvents(currentClip.loopStart - 1, num3, 1);
				}
				else
				{
					ProcessEvents(num, num3, 1);
				}
			}
			else
			{
				ProcessEvents(num, num3, 1);
			}
			break;
		}
		case tk2dSpriteAnimationClip.WrapMode.PingPong:
		{
			int num6 = (int)clipTime % (currentClip.frames.Length + currentClip.frames.Length - 2);
			int direction = 1;
			if (num6 >= currentClip.frames.Length)
			{
				num6 = 2 * currentClip.frames.Length - 2 - num6;
				direction = -1;
			}
			if (num6 < num)
			{
				direction = -1;
			}
			SetFrameInternal(num6);
			ProcessEvents(num, num6, direction);
			break;
		}
		case tk2dSpriteAnimationClip.WrapMode.Once:
		{
			int num2 = (int)clipTime;
			if (num2 >= currentClip.frames.Length)
			{
				SetFrameInternal(currentClip.frames.Length - 1);
				ProcessEvents(num, currentClip.frames.Length - 1, 1);
				currentClip = null;
				OnCompleteAnimation();
			}
			else
			{
				SetFrameInternal(num2);
				ProcessEvents(num, num2, 1);
			}
			break;
		}
		case tk2dSpriteAnimationClip.WrapMode.RandomFrame:
			break;
		}
	}
}
