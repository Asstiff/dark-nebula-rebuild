using System.Collections.Generic;
using UnityEngine;

public class LowLatencyAudio : MonoBehaviour
{
	private AudioSource _audioSource;

	private string _name;

	private static Dictionary<string, LowLatencyAudio> _audioList = new Dictionary<string, LowLatencyAudio>();

	public void Init(string pName, AudioClip pClip)
	{
		_name = pName;
		_audioSource = base.gameObject.AddComponent<AudioSource>();
		_audioSource.clip = pClip;
		_audioSource.loop = true;
		_audioSource.playOnAwake = true;
		_audioSource.priority = 64;
		_audioSource.bypassEffects = true;
		_audioSource.Pause();
		_audioList.Add(_name, this);
	}

	public void Play()
	{
		_audioSource.time = 0f;
		_audioSource.Play();
		CancelInvoke("Off");
		Invoke("Off", _audioSource.clip.length - 0.1f);
	}

	private void Off()
	{
		_audioSource.time = 0f;
		_audioSource.Pause();
	}

	private void OnDisable()
	{
		_audioList.Remove(_name);
	}

	public static void Register(string pName, AudioClip pClip)
	{
		if (!_audioList.ContainsKey(pName))
		{
			GameObject gameObject = new GameObject("_AudioSourceGO_" + pName);
			LowLatencyAudio lowLatencyAudio = gameObject.AddComponent<LowLatencyAudio>();
			lowLatencyAudio.Init(pName, pClip);
		}
	}

	public static void Play(string pName)
	{
		if (!_audioList.ContainsKey(pName))
		{
			Debug.Log("Sound not found " + pName);
		}
		else
		{
			_audioList[pName].Play();
		}
	}
}
