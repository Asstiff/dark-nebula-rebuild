using System.Collections.Generic;
using UnityEngine;

public class LevelCircle : MonoBehaviour
{
	[SerializeField]
	private int _firstSelectableLevel;

	[SerializeField]
	private int _lastSelectableLevel;

	public int selectedLevel = -1;

	public GameObject levelMap;

	public AnimationCurve InCurve;

	private bool _inited;

	private Dictionary<int, LevelSelectGfx> selected = new Dictionary<int, LevelSelectGfx>();

	private int maxLevel;

	private Vector3 originalPosition;

	private float animationStart;

	private Vector3 animationStartPosition;

	private float animationDirection;

	private float animationDistance = 400f;

	public bool HasSelectedLevel
	{
		get
		{
			return selectedLevel != -1;
		}
	}

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		if (_inited)
		{
			return;
		}
		_inited = true;
		originalPosition = base.transform.position;
		selected.Clear();
		selectedLevel = UserSettings.Instance.GetLastSelectedLevel(UserSettings.Instance.IsExpertMode);
		Transform transform = base.transform.FindChild("Stars");
		for (int i = _firstSelectableLevel; i <= _lastSelectableLevel + 1; i++)
		{
			bool flag = ContentGatingManager.Instance.IsLevelGated(i) && !ContentGatingManager.Instance.IsGateUnlocked(i);
			LevelSelectGfx component = base.transform.FindChild("Selected" + i).GetComponent<LevelSelectGfx>();
			selected.Add(i, component);
			if (component != null)
			{
				component.IsLockedGate = flag;
				component.SetState(false);
				if (i > UserProgress.Instance.GetMaxSelectableLevel(UserSettings.Instance.IsExpertMode))
				{
					component.gameObject.SetActive(false);
				}
			}
			Transform transform2 = transform.FindChild("Star" + i);
			if (!(transform2 != null))
			{
				continue;
			}
			int levelStar = UserProgress.Instance.GetLevelStar(i, UserSettings.Instance.IsExpertMode);
			tk2dBaseSprite component2 = transform2.gameObject.GetComponent<tk2dBaseSprite>();
			if (flag)
			{
				if (component2 != null)
				{
					component2.spriteId = component2.GetSpriteIdByName("level_lock");
				}
				continue;
			}
			if (levelStar == 0)
			{
				Object.Destroy(transform2.gameObject);
				continue;
			}
			string text = "Skillstars";
			if (UserSettings.Instance.IsExpertMode)
			{
				text += "Expert";
			}
			switch (levelStar)
			{
			case 1:
				text += "/0";
				break;
			case 2:
				text += "/1";
				break;
			case 3:
				text += "/2";
				break;
			}
			if (component2 != null)
			{
				component2.spriteId = component2.GetSpriteIdByName(text);
			}
		}
	}

	public void SnapOut()
	{
		base.transform.position = originalPosition + new Vector3(animationDistance, 0f, 0f);
	}

	public void SnapIn()
	{
		base.transform.position = originalPosition;
	}

	public void AnimateIn(bool pFromLeft)
	{
		animationStart = Time.realtimeSinceStartup;
		animationDirection = ((!pFromLeft) ? (-1f) : 1f);
		animationStartPosition = originalPosition + new Vector3(animationDistance * (float)((!pFromLeft) ? 1 : (-1)), 0f, 0f);
	}

	public void AnimateOut(bool pFromLeft)
	{
		animationStart = Time.realtimeSinceStartup;
		animationDirection = ((!pFromLeft) ? (-1f) : 1f);
		animationStartPosition = originalPosition;
	}

	private void Update()
	{
		if (animationDirection != 0f)
		{
			float num = (Time.realtimeSinceStartup - animationStart) * 4f;
			float num2 = InCurve.Evaluate(num);
			base.transform.position = animationStartPosition + new Vector3(num2 * animationDistance * animationDirection, 0f, 0f);
			if (num >= 1f)
			{
				animationDirection = 0f;
			}
		}
	}

	private int Clamp(int v, int min, int max)
	{
		if (v < min)
		{
			return min;
		}
		if (v > max)
		{
			return max;
		}
		return v;
	}

	public bool HasLevel(int pLevel)
	{
		Init();
		return selected.ContainsKey(pLevel);
	}

	public void UnselectLevel()
	{
		SelectLevel(-1);
	}

	public void SelectLevel(int pLevel)
	{
		Init();
		if ((HasLevel(pLevel) && pLevel == 11) || pLevel == 23)
		{
			Transform transform = base.transform.FindChild("Stars");
			transform.gameObject.SetActive(false);
			for (int i = _firstSelectableLevel; i <= _lastSelectableLevel; i++)
			{
				selected[i].gameObject.SetActive(false);
			}
			selected[_lastSelectableLevel + 1].gameObject.SetActive(true);
			selectedLevel = pLevel;
			return;
		}
		if (HasLevel(selectedLevel))
		{
			selected[selectedLevel].SetState(false);
		}
		selectedLevel = pLevel;
		if (HasLevel(selectedLevel))
		{
			selected[selectedLevel].SetState(true);
			selected[_lastSelectableLevel + 1].gameObject.SetActive(false);
		}
	}
}
