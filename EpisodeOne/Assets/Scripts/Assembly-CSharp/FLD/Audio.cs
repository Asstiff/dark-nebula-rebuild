using System.Collections.Generic;
using UnityEngine;

namespace FLD
{
	public static class Audio
	{
		private const string AvailableUnitName = "(available)";

		private static GameObject _audioContainer = null;

		private static Dictionary<string, AudioUnit> _activeAudioUnits = new Dictionary<string, AudioUnit>();

		private static bool _hasLoaded = false;

		private static IPool<AudioUnit> _availableAudioUnits;

		private static bool _muteSound = false;

		private static bool _muteMusic = false;

		private static GameObject AudioContainer
		{
			get
			{
				if (_audioContainer == null)
				{
					_audioContainer = GameObject.Find("_Audio_");
					if (_audioContainer == null)
					{
						_audioContainer = new GameObject("_Audio_");
						Object.DontDestroyOnLoad(_audioContainer);
					}
				}
				return _audioContainer;
			}
		}

		public static bool IsSoundEffectsMuted
		{
			get
			{
				Load();
				return _muteSound;
			}
			set
			{
				_muteSound = value;
				Storage.settings.Set("_muteSound", _muteSound);
				foreach (KeyValuePair<string, AudioUnit> activeAudioUnit in _activeAudioUnits)
				{
					if (activeAudioUnit.Value.Type == AudioUnit.AudioType.SoundEffect)
					{
						activeAudioUnit.Value.IsMute = _muteSound;
					}
				}
			}
		}

		public static bool IsMusicMuted
		{
			get
			{
				Load();
				return _muteMusic;
			}
			set
			{
				_muteMusic = value;
				Storage.settings.Set("_muteMusic", _muteMusic);
				foreach (KeyValuePair<string, AudioUnit> activeAudioUnit in _activeAudioUnits)
				{
					if (activeAudioUnit.Value.Type == AudioUnit.AudioType.Music)
					{
						activeAudioUnit.Value.IsMute = _muteMusic;
					}
				}
			}
		}

		private static void Load()
		{
			if (!_hasLoaded)
			{
				_muteSound = Storage.settings.GetBool("_muteSound");
				_muteMusic = Storage.settings.GetBool("_muteMusic");
				_hasLoaded = true;
			}
		}

		public static void PlayMusicWithCrossFade(AudioClip pClip, float pVolume, float pCrossFadeTime = 1f, bool pLoop = false)
		{
			if (IsMusicMuted)
			{
				return;
			}
			if (IsSameSoundPlaying("_music_", pClip))
			{
				if (GetAudioUnit("_music_").GetComponent<AudioSource>().volume != pVolume)
				{
					ResumeMusic(pCrossFadeTime);
				}
			}
			else if (IsSoundPlaying("_music_"))
			{
				CrossFadeMusic(pCrossFadeTime, pClip, pVolume, pLoop);
			}
			else
			{
				PlayMusic(pClip, pVolume, pLoop);
				ResumeMusic(pCrossFadeTime);
			}
		}

		public static void PlayMusic(AudioClip pClip, float pVolume, bool pLoop = false)
		{
			if (!IsMusicMuted)
			{
				AudioUnit audioUnit = GetAudioUnit("_music_");
				if (audioUnit != null)
				{
					audioUnit.Remove();
				}
				Play("_music_", pClip, Vector3.zero, null, AudioUnit.AudioType.Music, pLoop);
				SetMusicVolume(pVolume);
				audioUnit = GetAudioUnit("_music_");
				if (audioUnit != null && audioUnit.AudioSource != null)
				{
					audioUnit.AudioSource.priority = 0;
				}
			}
		}

		public static void StopMusic(float pFadeTime = 0f)
		{
			Stop("_music_", pFadeTime);
		}

		public static void SetMusicVolume(float pVolume)
		{
			SetVolume("_music_", pVolume);
		}

		public static void PauseMusic(float pFadeTime = 0f)
		{
			Pause("_music_", pFadeTime);
		}

		public static bool ResumeMusic(float pFadeTime = 0f)
		{
			if (IsMusicMuted)
			{
				return false;
			}
			return Resume("_music_", pFadeTime);
		}

		public static void CrossFadeMusic(float pFadeTime, AudioClip pMusic, float pVolume, bool pLoop = false)
		{
			AudioUnit audioUnit = GetAudioUnit("_music_");
			if (audioUnit != null)
			{
				_activeAudioUnits.Remove(audioUnit.Id);
				audioUnit.Id = "_music_crossfade_" + Random.Range(0, int.MaxValue);
				audioUnit.gameObject.name = audioUnit.Id;
				_activeAudioUnits.Add(audioUnit.Id, audioUnit);
				audioUnit.Stop(pFadeTime);
			}
			PlayMusic(pMusic, pVolume, pLoop);
			ResumeMusic(pFadeTime);
		}

		public static void Pause(string pID, float pFadeTime = 0f)
		{
			if (IsSoundPlaying(pID))
			{
				GetAudioUnit(pID).Pause(pFadeTime);
			}
		}

		public static bool Resume(string pID, float pFadeTime = 0f)
		{
			AudioUnit audioUnit = GetAudioUnit(pID);
			if (audioUnit == null)
			{
				return false;
			}
			audioUnit.Resume(pFadeTime);
			return true;
		}

		public static void PlayRandom(string pID, AudioClip[] pClips)
		{
			Play(pID, GetRandomClip(pClips));
		}

		public static void PlayRandom(string pID, AudioClip[] pClips, Vector3 pPosition)
		{
			Play(pID, GetRandomClip(pClips), pPosition);
		}

		public static void PlayRandom(string pID, AudioClip[] pClips, Transform pTransform)
		{
			Play(pID, GetRandomClip(pClips), pTransform);
		}

		public static void Play(string pID, AudioClip pClip)
		{
			Play(pID, pClip, Vector3.zero, null, AudioUnit.AudioType.SoundEffect, false, false);
		}

		public static void Play(string pID, AudioClip pClip, Vector3 pPosition)
		{
			Play(pID, pClip, pPosition, null);
		}

		public static void Play(string pID, AudioClip pClip, Transform pTransform)
		{
			Play(pID, pClip, Vector3.zero, pTransform);
		}

		private static void Play(string pID, AudioClip pClip, Vector3 pPosition, Transform pTransform, AudioUnit.AudioType pType = AudioUnit.AudioType.SoundEffect, bool pLoop = false, bool pSpatial = true)
		{
			if ((!IsSoundEffectsMuted || pType != AudioUnit.AudioType.SoundEffect) && !IsSoundPlaying(pID))
			{
				if (_availableAudioUnits == null)
				{
					_availableAudioUnits = new Pool<AudioUnit>(CreateAudioUnit, 8);
				}
				AudioUnit audioUnit = _availableAudioUnits.Take();
				audioUnit.gameObject.name = pID;
				audioUnit.Id = pID;
				audioUnit.Type = pType;
				audioUnit.Volume = 1f;
				audioUnit.IsMute = ((pType != AudioUnit.AudioType.Music) ? _muteSound : _muteMusic);
				audioUnit.AudioSource.clip = pClip;
				audioUnit.AudioSource.loop = pLoop;
				audioUnit.AudioSource.playOnAwake = false;
				audioUnit.AudioSource.priority = 128;
				audioUnit.ConfigureStereo(pSpatial && pType == AudioUnit.AudioType.SoundEffect);
			audioUnit.AudioSource.Play();
				audioUnit.Play();
				if (pTransform == null)
				{
					audioUnit.transform.parent = AudioContainer.transform;
				}
				else
				{
					audioUnit.transform.parent = pTransform;
				}
				audioUnit.transform.localPosition = pPosition;
				_activeAudioUnits.Add(pID, audioUnit);
			}
		}

		private static AudioUnit CreateAudioUnit()
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = AudioContainer.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.name = "(available)";
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			AudioUnit audioUnit = gameObject.AddComponent<AudioUnit>();
			audioUnit.AudioSource = audioSource;
			audioUnit.Removed += OnAudioRemoved;
			return audioUnit;
		}

		public static void PlayLoop(string pID, AudioClip pClip)
		{
			Play(pID, pClip, Vector3.zero, null, AudioUnit.AudioType.SoundEffect, true, false);
		}

		public static void Stop(string pID)
		{
			Stop(pID, 0f);
		}

		public static void Stop(string pID, float pFadeTime)
		{
			if (IsSoundPlaying(pID))
			{
				GetAudioUnit(pID).Stop(pFadeTime);
			}
		}

		public static void SetVolume(string pID, float pVolume)
		{
			if (IsSoundPlaying(pID))
			{
				GetAudioUnit(pID).Volume = pVolume;
			}
		}

		private static void OnAudioRemoved(AudioUnit pUnit)
		{
			_activeAudioUnits.Remove(pUnit.Id);
			_availableAudioUnits.Recycle(pUnit);
			pUnit.transform.parent = AudioContainer.transform;
			pUnit.gameObject.name = "(available)";
		}

		public static bool IsSoundPlaying(string pID)
		{
			return _activeAudioUnits.ContainsKey(pID);
		}

		public static bool IsSameSoundPlaying(string pID, AudioClip sound)
		{
			AudioUnit value;
			return _activeAudioUnits.TryGetValue(pID, out value) && value.AudioSource.clip.Equals(sound);
		}

		public static AudioUnit GetAudioUnit(string pID)
		{
			AudioUnit value;
			if (_activeAudioUnits.TryGetValue(pID, out value))
			{
				return value;
			}
			return null;
		}

		private static AudioClip GetRandomClip(AudioClip[] pClips)
		{
			return pClips[Random.Range(0, pClips.Length)];
		}
	}
}
