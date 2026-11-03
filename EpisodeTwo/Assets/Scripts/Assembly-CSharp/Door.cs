using FLD;
using UnityEngine;

public class Door : Entity
{
	private static int audioIndex;

	private int flag;

	private int threshold;

	public AudioClip Sfx;

	private bool _isOpen;

	private tk2dAnimatedSprite _sprite;

	private void Start()
	{
		flag = GetParameterInt("flag");
		threshold = GetParameterInt("threshold");
		World.GetTrigger(flag).Changed += HandleWorldSignalFlagChanged;
		_sprite = GetComponentInChildren<tk2dAnimatedSprite>();
		if (flag == 0)
		{
			Open();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		World.GetTrigger(flag).Changed -= HandleWorldSignalFlagChanged;
	}

	private void HandleWorldSignalFlagChanged(int newValue)
	{
		if (!_isOpen && newValue >= threshold)
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
