using System;
using UnityEngine;

public class AudioUnit : MonoBehaviour
{
	public enum AudioType
	{
		SoundEffect = 0,
		Music = 1
	}

	public string Id = string.Empty;

	public AudioSource AudioSource;

	public AudioType Type;

	private bool _isRemoved = true;

	private bool _isMute;

	private float _volume = 1f;

	private bool _pause;

	private bool _stop;

	private float _fadeOutCompleteTime;

	private float _fadeOutDuration;

	private float _fadeOutFromVolume;

	private bool _fadeIn;

	private float _fadeInCompleteTime;

	private float _fadeInDuration;

	private float _pausedAt;

	public float Volume
	{
		set
		{
			_volume = value;
			if (!_isMute)
			{
				AudioSource.volume = _volume;
			}
		}
	}

	public float FadeVolume
	{
		set
		{
			if (!_isMute)
			{
				AudioSource.volume = value;
			}
		}
	}

	public bool IsMute
	{
		get
		{
			return _isMute;
		}
		set
		{
			if (value)
			{
				_isMute = true;
				if (AudioSource != null)
				{
					AudioSource.volume = 0f;
				}
			}
			else
			{
				_isMute = false;
				if (AudioSource != null)
				{
					AudioSource.volume = _volume;
				}
			}
		}
	}

	public event Action<AudioUnit> Removed;
    private bool positionalStereo;
    public void ConfigureStereo(bool positional)
    {
        positionalStereo = positional;
        AudioSource.spatialBlend = 0f;
        AudioSource.panStereo = 0f;
        AudioSource.dopplerLevel = 0f;
    }
    private void LateUpdate()
    {
        if (!positionalStereo || AudioSource == null || !AudioSource.isPlaying) return;
        Camera camera = World.cameraFollow != null ? World.cameraFollow.GetComponent<Camera>() : Camera.main;
        if (camera == null) return;
        float horizontal = camera.WorldToViewportPoint(transform.position).x;
        AudioSource.panStereo = Mathf.Clamp((horizontal - .5f) * 1.7f, -.85f, .85f);
    }


	public void Play()
	{
		_isRemoved = false;
	}

	public void Pause(float pFadeTime = 0f)
	{
		_pause = true;
		_pausedAt = AudioSource.time;
		Stop(pFadeTime);
	}

	public void Resume(float pFadeTime = 0f)
	{
		_stop = false;
		if (pFadeTime <= 0f)
		{
			_fadeIn = false;
			FadeVolume = _volume;
		}
		else
		{
			_fadeInDuration = pFadeTime;
			_fadeInCompleteTime = Time.realtimeSinceStartup + _fadeInDuration;
			_fadeIn = true;
			FadeVolume = 0f;
		}
		AudioSource.Play();
		AudioSource.time = _pausedAt;
	}

	private void Update()
	{
		if (_stop)
		{
			float num = Mathf.Max(0f, _fadeOutCompleteTime - Time.realtimeSinceStartup);
			FadeVolume = _fadeOutFromVolume * (num / _fadeOutDuration);
			_pausedAt = AudioSource.time;
			if (num <= 0f)
			{
				if (_pause)
				{
					_pause = false;
					AudioSource.Pause();
				}
				else
				{
					Remove();
				}
				_stop = false;
			}
		}
		else if (_fadeIn)
		{
			float num2 = Mathf.Max(0f, _fadeInCompleteTime - Time.realtimeSinceStartup);
			FadeVolume = _volume * (1f - num2 / _fadeInDuration);
			if (num2 <= 0f)
			{
				_fadeIn = false;
			}
		}
		if (AudioSource == null || AudioSource.clip == null)
		{
			Remove();
		}
		else if (AudioSource.time >= AudioSource.clip.length || !AudioSource.isPlaying)
		{
			if (AudioSource.loop)
			{
				AudioSource.time = 0f;
			}
			else
			{
				Remove();
			}
		}
	}

	public void Stop(float pFadeTime)
	{
		_stop = true;
		_fadeOutDuration = pFadeTime;
		_fadeOutCompleteTime = Time.realtimeSinceStartup + _fadeOutDuration;
		_fadeOutFromVolume = AudioSource.volume;
		if (_fadeOutFromVolume == 0f)
		{
			Remove();
		}
	}

	internal void Remove()
	{
		if (!_isRemoved)
		{
			if (Removed != null)
			{
				Removed(this);
			}
			_isRemoved = true;
		}
	}
}
