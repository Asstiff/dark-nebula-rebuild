using System.Collections.Generic;
using UnityEngine;

public class LevelCircle : MonoBehaviour
{
	public int selectedLevel = 1;

	public GameObject levelMap;

	public GameObject levelName;

	public LevelPack Pack;

	public AnimationCurve InCurve;

	private bool _inited;

	private Dictionary<int, LevelSelectGfx> selected = new Dictionary<int, LevelSelectGfx>();

	private int maxLevel;

	private Vector3 originalPosition;

	private float animationStart;

	private Vector3 animationStartPosition;

	private float animationDirection;

	private float animationDistance = 400f;

	private void OnDisable()
	{
		selected = null;
	}

	private void Start()
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
		selectedLevel = UserSettings.I.GetPackMinLevel(Pack);
		Transform transform = base.transform.Find("Stars");
		int packMaxLevel = UserSettings.I.GetPackMaxLevel(Pack);
		for (int i = UserSettings.I.GetPackMinLevel(Pack); i <= packMaxLevel; i++)
		{
			LevelSelectGfx component = base.transform.Find("Selected" + i).GetComponent<LevelSelectGfx>();
			selected.Add(i, component);
			if (component != null)
			{
				component.SetState(false);
				if (i == 32)
				{
					component.gameObject.SetActive(false);
				}
			}
			Transform transform2 = transform.Find("Star" + i);
			if (!(transform2 != null))
			{
				continue;
			}
			int levelStar = UserSettings.I.GetLevelStar(i, UserSettings.I.expert);
			if (levelStar == 0)
			{
				Object.Destroy(transform2.gameObject);
				continue;
			}
			string text = "Skillstars";
			text = ((!UserSettings.I.expert) ? (text + string.Empty) : (text + "Expert"));
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
			tk2dBaseSprite component2 = transform2.gameObject.GetComponent<tk2dBaseSprite>();
			if (!(component2 == null))
			{
				component2.spriteId = component2.GetSpriteIdByName(text);
			}
		}
		Debug.Log("SelectCount " + selected.Count + " " + Pack);
		if (Pack == LevelPack.Original)
		{
			base.transform.Find("Background20").gameObject.SetActive(UserSettings.I.GetMaxSelectableLevel(Pack) >= UserSettings.I.GetPackMaxLevel(Pack));
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

	private void SetMaxLevel(int level, LevelPack pack)
	{
		UserSettings i = UserSettings.I;
		int packMinLevel = i.GetPackMinLevel(pack);
		int num = i.GetPackMaxLevel(pack);
		if (pack == LevelPack.Extended)
		{
			num--;
		}
		int maxSelectableLevel = i.GetMaxSelectableLevel(pack);
		for (int j = packMinLevel; j <= num; j++)
		{
			selected[j].gameObject.SetActive(j <= maxSelectableLevel);
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

	public void SelectLevel(int pLevel)
	{
		Debug.Log(string.Concat(Pack, " Select: ", pLevel));
		Init();
		int packMaxLevel = UserSettings.I.GetPackMaxLevel(Pack);
		int packMinLevel = UserSettings.I.GetPackMinLevel(Pack);
		SetMaxLevel(UserSettings.I.GetMaxSelectableLevel(Pack), Pack);
		selected[selectedLevel].SetState(false);
		selectedLevel = Clamp(pLevel, packMinLevel, packMaxLevel);
		selected[selectedLevel].SetState(true);
		levelName.GetComponent<SimpleText>().SetText(LoadLevelScreen.GetLevelName(pLevel));
		if (selectedLevel == 32)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).gameObject.SetActive(false);
			}
			selected[selectedLevel].gameObject.SetActive(true);
		}
	}
}
