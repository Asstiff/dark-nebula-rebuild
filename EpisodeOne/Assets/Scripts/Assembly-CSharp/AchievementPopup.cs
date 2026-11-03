using System.Collections;
using UnityEngine;

public class AchievementPopup : MonoBehaviour
{
	private const string PopupResourcePath = "Prefabs/UI/AchievementPopup";

	[SerializeField]
	private tk2dSlicedSprite _background;

	[SerializeField]
	private DynamicFont _achievementText;

	[SerializeField]
	private tk2dSprite _achievementIcon;

	private static AchievementPopup _popup;

	private float BoxHeight
	{
		get
		{
			if (_background != null)
			{
				return _background.dimensions.y * _background.scale.y;
			}
			return 0f;
		}
	}

	private Vector3 InPosition
	{
		get
		{
			Camera component = base.transform.parent.GetComponent<Camera>();
			if (component != null)
			{
				return new Vector3(0f, component.orthographicSize - BoxHeight / 2f, component.nearClipPlane + 1f);
			}
			return Vector3.zero;
		}
	}

	private Vector3 OutPosition
	{
		get
		{
			return InPosition + new Vector3(0f, BoxHeight, 0f);
		}
	}

	private void Start()
	{
	}

	private void SetIcon(string pIconId)
	{
		_achievementIcon.SetSprite(pIconId);
	}

	private void SetText(string pText)
	{
		_achievementText.Text = pText;
	}

	private void Show()
	{
		base.gameObject.SetActive(true);
		StartCoroutine(ShowCoroutine());
	}

	private IEnumerator ShowCoroutine()
	{
		base.transform.localPosition = OutPosition;
		Go.to(base.transform, 0.15f, new GoTweenConfig().localPosition(InPosition).setEaseType(GoEaseType.QuadOut));
		yield return new WaitForSeconds(3f);
		Go.to(base.transform, 0.15f, new GoTweenConfig().localPosition(OutPosition).setEaseType(GoEaseType.QuadIn));
	}

	private static AchievementPopup GetPopup()
	{
		if (_popup == null)
		{
			_popup = (AchievementPopup)Object.Instantiate(Resources.Load<AchievementPopup>("Prefabs/UI/AchievementPopup"));
			_popup.transform.parent = Camera.main.transform;
			_popup.transform.localPosition = _popup.OutPosition;
		}
		return _popup;
	}

	public static void Open(string pIconId, string pAchievementText)
	{
		AchievementPopup popup = GetPopup();
		popup.SetIcon(pIconId);
		popup.SetText(pAchievementText);
		popup.Show();
	}
}
