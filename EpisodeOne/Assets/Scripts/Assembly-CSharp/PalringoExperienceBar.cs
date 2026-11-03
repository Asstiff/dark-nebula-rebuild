using FLD;
using Palringo;
using UnityEngine;

public class PalringoExperienceBar : MonoBehaviour
{
	[SerializeField]
	private tk2dSlicedSprite _experienceBar;

	[SerializeField]
	private DynamicFont _explanationLabel;

	private int _targetLevel;

	private float _xpBarWidth;

	public int TargetReputationLevel
	{
		get
		{
			return _targetLevel;
		}
		set
		{
			_targetLevel = value;
			UpdateExperienceBar();
		}
	}

	public DynamicFont ExplanationLabel
	{
		get
		{
			return _explanationLabel;
		}
	}

	private void UpdateExperienceBar()
	{
		if (_xpBarWidth <= 0f)
		{
			_xpBarWidth = _experienceBar.dimensions.x;
		}
		if (_targetLevel > 0)
		{
			DarkNebulaPalringo.Connect.GetLocalUserInfo().Wait((UserInfo localUser) =>
			{
				if (localUser != null)
				{
					float num = (float)localUser.ReputationLevel + localUser.ProgressToNextLevel;
					float num2 = Mathf.Clamp01(num / (float)_targetLevel);
					_experienceBar.dimensions = new Vector2(_xpBarWidth * num2, _experienceBar.dimensions.y);
				}
				else
				{
					_experienceBar.dimensions = new Vector2(0f, _experienceBar.dimensions.y);
				}
			});
		}
		else
		{
			_experienceBar.dimensions = new Vector2(0f, _experienceBar.dimensions.y);
		}
	}

	private void Start()
	{
		if (LocalizationManager.Instance.IsRightToLeftLanguage)
		{
			_experienceBar.anchor = tk2dBaseSprite.Anchor.MiddleRight;
		}
		UpdateExperienceBar();
	}
}
