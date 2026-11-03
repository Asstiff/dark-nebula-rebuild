using System;
using UnityEngine;

public class LevelBoosters : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite _speed;

	[SerializeField]
	private tk2dSprite _shield;

	[SerializeField]
	private tk2dSprite _magnet;

	[SerializeField]
	private tk2dSprite _backgroundLocked;

	[SerializeField]
	private tk2dSprite _backgroundUnlocked;

	[SerializeField]
	private tk2dSprite _backgroundSelected;

	private int _level;

	private bool _isUnlocked;

	private LevelSelectScreen _levelSelectScreen;

	private void Start()
	{
		string s = base.gameObject.name.Replace("LevelBoosters", string.Empty);
		if (int.TryParse(s, out _level))
		{
			SetBoosterStates();
			int maxSelectableLevel = UserProgress.Instance.GetMaxSelectableLevel(UserSettings.Instance.IsExpertMode);
			if (_level <= maxSelectableLevel)
			{
				_isUnlocked = true;
			}
			_levelSelectScreen = base.transform.GetComponentInParent<LevelSelectScreen>();
			if (_levelSelectScreen != null)
			{
				LevelSelectScreen levelSelectScreen = _levelSelectScreen;
				levelSelectScreen.LevelSelectedEvent = (Action<int>)Delegate.Combine(levelSelectScreen.LevelSelectedEvent, new Action<int>(SetBackgroundState));
			}
			else
			{
				SetBackgroundState(LoadLevelScreen.LevelToLoad);
			}
		}
		else
		{
			Debug.LogWarning("Couldnt parse name. Disabling all boosters. LevelBoosters Prefab needs to be placed in a separate gamobject under the LevelCircle");
			_speed.gameObject.SetActive(false);
			_shield.gameObject.SetActive(false);
			_magnet.gameObject.SetActive(false);
			_backgroundSelected.gameObject.SetActive(false);
			_backgroundLocked.gameObject.SetActive(false);
			_backgroundUnlocked.gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if (_levelSelectScreen != null)
		{
			LevelSelectScreen levelSelectScreen = _levelSelectScreen;
			levelSelectScreen.LevelSelectedEvent = (Action<int>)Delegate.Remove(levelSelectScreen.LevelSelectedEvent, new Action<int>(SetBackgroundState));
		}
	}

	private void SetBoosterStates()
	{
		bool active = BoosterManager.IsEnabled(Booster.GetIdForCurrentDifficulty(Booster.BoosterType.SPEED, _level));
		bool active2 = BoosterManager.IsEnabled(Booster.GetIdForCurrentDifficulty(Booster.BoosterType.SHIELD, _level));
		bool active3 = BoosterManager.IsEnabled(Booster.GetIdForCurrentDifficulty(Booster.BoosterType.MAGNET, _level));
		_speed.gameObject.SetActive(active);
		_shield.gameObject.SetActive(active2);
		_magnet.gameObject.SetActive(active3);
	}

	private void SetBackgroundState(int pLevel)
	{
		bool flag = pLevel == _level;
		_backgroundSelected.gameObject.SetActive(flag);
		_backgroundLocked.gameObject.SetActive(!_isUnlocked);
		_backgroundUnlocked.gameObject.SetActive(_isUnlocked && !flag);
	}
}
