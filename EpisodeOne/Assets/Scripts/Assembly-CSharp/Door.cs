using FLD;
using UnityEngine;

public class Door : Entity
{
	private static int audioIndex;

	public AudioClip Sfx;

	public int flag;

	public int threshold = 1;

	private bool _isOpen;

	private tk2dAnimatedSprite _sprite;

	private void Awake()
	{
		_sprite = GetComponentInChildren<tk2dAnimatedSprite>();
	}

	private void OnEnable()
	{
		World.SignalFlagChanged += HandleWorldSignalFlagChanged;
	}

	private void OnDisable()
	{
		World.SignalFlagChanged -= HandleWorldSignalFlagChanged;
	}

	private void Start()
	{
		if (flag == 0)
		{
			Open();
		}
	}

	private void HandleWorldSignalFlagChanged(int pFlag, int newValue)
	{
		if (flag == pFlag && !_isOpen && newValue >= threshold)
		{
			Open();
		}
	}

	private void Open()
	{
		_isOpen = true;
		_sprite.Play("DoorOpen");
		_sprite.animationEventDelegate = OnAnimationEvent;
		Invoke("DisableCollider", 0.667f);
	}

	private void DisableCollider()
	{
		base.GetComponent<Collider>().enabled = false;
	}

	public void OnAnimationEvent(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum)
	{
		if (clip.frames[frameNum].eventInfo == "sound")
		{
			Audio.Play("door" + audioIndex, Sfx, base.transform.position);
			audioIndex = (audioIndex + 1) % 5;
		}
	}
}
