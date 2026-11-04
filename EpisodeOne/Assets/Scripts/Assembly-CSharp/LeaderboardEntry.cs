using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FLD.JSON;
using Palringo;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
	public GameObject GfxStar;

	public LeaderboardEntryVO Entry;

	[SerializeField]
	private tk2dSprite _background;

	[SerializeField]
	private DynamicFont _timeText;

	[SerializeField]
	private SysFontText _nameText;

	[SerializeField]
	private DynamicFont _positionText;

	[SerializeField]
	private DynamicFont _userIDText;

	[SerializeField]
	private Transform _profileImage;

	[SerializeField]
	private Material _defaultMaterial;

	[SerializeField]
	private Color _currentPlayerColor;

	private bool _haveProfileImage;
	private DynamicFont _localNameText;

	public void SetEntry(LeaderboardEntryVO pEntry, int pListposition)
	{
		// The original SysFont renderer depends on a removed Android Java plugin.
		// Reuse the game's Unity font renderer for the local player's label.
		_nameText.gameObject.SetActive(false);
		if (_localNameText == null)
		{
			_localNameText = UnityEngine.Object.Instantiate(_timeText, _timeText.transform.parent);
			_localNameText.name = "Local score name";
			Vector3 labelPosition = _timeText.transform.localPosition;
			labelPosition.x = -85f;
			_localNameText.SetOriginalPos(labelPosition);
			_localNameText.Anchor = TextAnchor.MiddleLeft;
			_localNameText.Alignment = TextAlignment.Left;
			_localNameText.Localize = false;
			_localNameText.Wrapping = DynamicFont.WrapType.SHRINK;
			_localNameText.WrapWidth = 150f;
		}
		Entry = pEntry;
		base.transform.localPosition = new Vector3(0f, -18f + (float)pListposition * -36f, 0f);
		if (pListposition % 2 == 0)
		{
			_background.spriteId = _background.GetSpriteIdByName("LeaderboardEven");
		}
		else
		{
			_background.spriteId = _background.GetSpriteIdByName("LeaderboardOdd");
		}
		bool flag = true;
		if (pEntry.time <= 0f && string.IsNullOrEmpty(pEntry.name) && string.IsNullOrEmpty(pEntry.userID))
		{
			flag = false;
		}
		_userIDText.gameObject.SetActive(flag);
		_localNameText.gameObject.SetActive(flag);
		_profileImage.gameObject.SetActive(flag);
		_positionText.gameObject.SetActive(flag);
		_timeText.gameObject.SetActive(flag);
		GfxStar.GetComponent<Renderer>().enabled = flag;
		if (flag)
		{
			if (string.IsNullOrEmpty(pEntry.name))
			{
				pEntry.name = "DUMMY NAME";
			}
			if (DarkNebulaPalringo.IsAuthorized)
			{
				_userIDText.gameObject.SetActive(true);
				_positionText.Text = pEntry.position + ".";
			}
			else
			{
				_positionText.Text = string.Empty;
				_userIDText.gameObject.SetActive(false);
			}
			_localNameText.SetLiteralText(DarkNebulaPalringo.IsAuthorized ? pEntry.name :
				(pListposition + 1) + ". " + (pEntry.date == default(DateTime) ? "LOCAL BEST" :
				pEntry.date.ToString("MM/dd HH:mm", System.Globalization.CultureInfo.InvariantCulture)));
			_timeText.Text = LevelCompleteSection.ToTimeString(pEntry.time);
			if (pEntry.star >= 0 && pEntry.expert)
			{
				GfxStar.GetComponentInChildren<tk2dSprite>().spriteId = GfxStar.GetComponentInChildren<tk2dSprite>().GetSpriteIdByName("SkillstarsExpert/" + pEntry.star);
			}
			else if (pEntry.star >= 0)
			{
				GfxStar.GetComponentInChildren<tk2dSprite>().spriteId = GfxStar.GetComponentInChildren<tk2dSprite>().GetSpriteIdByName("Skillstars/" + pEntry.star);
			}
			if (pEntry.star < 0)
			{
				GfxStar.GetComponent<Renderer>().enabled = false;
			}
			if (pEntry.userID != null)
			{
				_userIDText.Text = pEntry.userID;
			}
			if (pEntry.profileImage != null)
			{
				SetProfileImage(pEntry.profileImage);
			}
			else
			{
				_haveProfileImage = false;
			}
			AlignLocalRow();
		}
	}

	private void AlignLocalRow()
	{
		Renderer profile=_profileImage.GetComponent<Renderer>();
		if(profile==null)return;
		float center=profile.bounds.center.y;
		foreach(DynamicFont label in new[]{_localNameText,_timeText})
		{
			Renderer renderer=label.GetComponent<Renderer>();
			if(renderer==null)continue;
			Vector3 position=label.transform.position;position.y+=center-renderer.bounds.center.y;
			label.SetOriginalPos(label.transform.parent.InverseTransformPoint(position));
		}
		Renderer timeRenderer=_timeText.GetComponent<Renderer>();
		Renderer star=GfxStar.GetComponent<Renderer>();
		if(timeRenderer!=null && star!=null)
		{
			Vector3 position=GfxStar.transform.position;
			position.x+=timeRenderer.bounds.min.x-star.bounds.extents.x-4f-star.bounds.center.x;
			position.y+=center-star.bounds.center.y;GfxStar.transform.position=position;
		}
	}

	public void SetCurrentUserIndicator()
	{
		_background.color = _currentPlayerColor;
	}

	private void Update()
	{
		if (!_haveProfileImage && Entry.palringoEntry != null && !(Entry.palringoEntry.User.Avatar == null))
		{
			SetProfileImage(Entry.palringoEntry.User.Avatar);
		}
	}

	private void SetProfileImage(Texture2D pProfileImage)
	{
		if (pProfileImage != null)
		{
			_haveProfileImage = true;
			_profileImage.GetComponent<Renderer>().material = new Material(_profileImage.GetComponent<Renderer>().material);
			_profileImage.GetComponent<Renderer>().material.mainTexture = pProfileImage;
		}
		else
		{
			_profileImage.GetComponent<Renderer>().material = _defaultMaterial;
		}
	}

	public void Reset()
	{
		_haveProfileImage = false;
		_profileImage.GetComponent<Renderer>().material = _defaultMaterial;
		if (_background != null)
		{
			_background.color = Color.white;
		}
	}

	public static LeaderboardEntryVO Deserialize(Dictionary<string, object> data)
	{
		LeaderboardEntryVO entry;
		LocalLeaderboard.TryDeserialize(data, out entry);
		return entry;
	}

	public static LeaderboardEntryVO GetEntryFromJValue(JValue pJValueEntry)
	{
		return new LeaderboardEntryVO
		{
			date = DateTime.Parse(pJValueEntry["date"].StringValue),
			name = pJValueEntry["name"].StringValue,
			time = float.Parse(pJValueEntry["time"].StringValue),
			star = int.Parse(pJValueEntry["star"].StringValue),
			expert = bool.Parse(pJValueEntry["expert"].StringValue)
		};
	}

	public static JValue GetJValueFromEntry(LeaderboardEntryVO pEntry)
	{
		JValue jValue = new JValue();
		jValue.Add("date", pEntry.date.ToString());
		jValue.Add("name", pEntry.name);
		jValue.Add("time", pEntry.time.ToString());
		jValue.Add("star", pEntry.star.ToString());
		jValue.Add("expert", pEntry.expert.ToString());
		return jValue;
	}

	public static Dictionary<string, object> Serialize(LeaderboardEntryVO entry)
	{
		return LocalLeaderboard.Serialize(entry);
	}

	public static int GetLevelFromFromLeaderboardEntry(Palringo.LeaderboardEntry pEntry)
	{
		if (pEntry.LeaderboardId.Contains("total"))
		{
			return 0;
		}
		string[] array = pEntry.LeaderboardId.Split('/');
		string value = Regex.Match(array[0], "\\d+").Value;
		return int.Parse(value);
	}

	private static bool IsExpertLeaderboardEntry(Palringo.LeaderboardEntry pEntry)
	{
		return pEntry.LeaderboardId.Contains("hard");
	}

	public static LeaderboardEntryVO GetTotalTimeEntryForLocalUser(bool pExpert)
	{
		return new LeaderboardEntryVO
		{
			name = UserSettings.Instance.GetUserName(),
			time = UserProgress.Instance.GetTotalTime(pExpert),
			star = -1,
			expert = pExpert
		};
	}

	public static LeaderboardEntryVO GetEntryFromGlobalEntry(Palringo.LeaderboardEntry pEntry)
	{
		bool flag = IsExpertLeaderboardEntry(pEntry);
		LeaderboardEntryVO result = new LeaderboardEntryVO
		{
			level = GetLevelFromFromLeaderboardEntry(pEntry),
			name = pEntry.User.Nickname,
			time = (float)pEntry.Score / 100000f,
			expert = flag,
			profileImage = pEntry.User.Avatar,
			position = pEntry.Rank,
			userID = pEntry.User.UserId,
			palringoEntry = pEntry
		};
		if (result.level == 0)
		{
			result.star = -1;
		}
		else
		{
			result.star = (int)LevelCompleteScreen.GetStarValue(result.time, result.level, flag);
		}
		return result;
	}

	private static void PrintEntry(LeaderboardEntryVO entry)
	{
		Debug.Log("Entry: \n Name: " + entry.name + "\ntime" + entry.time + "\nstar" + entry.star + "\nexpert" + entry.expert + "\nuserid" + entry.userID);
	}
}
