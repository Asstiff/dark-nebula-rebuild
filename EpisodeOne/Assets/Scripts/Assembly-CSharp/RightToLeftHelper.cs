using System;
using FLD;
using UnityEngine;

public class RightToLeftHelper : MonoBehaviour
{
	[SerializeField]
	private bool _flipSprite;

	[SerializeField]
	private bool _flipPosition;

	[SerializeField]
	private Vector3 _offset = Vector3.zero;

	private DynamicFont dynFont;

	private void Start()
	{
		if (!LocalizationManager.Instance.IsRightToLeftLanguage)
		{
			return;
		}
		dynFont = GetComponent<DynamicFont>();
		if (dynFont != null)
		{
			DynamicFont dynamicFont = dynFont;
			dynamicFont.FontRefreshed = (Action)Delegate.Combine(dynamicFont.FontRefreshed, new Action(UpdateOffset));
		}
		if (_flipSprite)
		{
			tk2dSprite component = GetComponent<tk2dSprite>();
			if (component != null)
			{
				component.scale = new Vector3(0f - component.scale.x, component.scale.y, component.scale.z);
			}
			else
			{
				tk2dSlicedSprite component2 = GetComponent<tk2dSlicedSprite>();
				if (component2 != null)
				{
					component2.scale = new Vector3(0f - component2.scale.x, component2.scale.y, component2.scale.z);
				}
			}
		}
		if (_flipPosition)
		{
			base.transform.localPosition = new Vector3(0f - base.transform.localPosition.x, base.transform.localPosition.y, base.transform.localPosition.z);
		}
		UpdateOffset();
	}

	private void OnDestroy()
	{
		if (dynFont != null)
		{
			DynamicFont dynamicFont = dynFont;
			dynamicFont.FontRefreshed = (Action)Delegate.Remove(dynamicFont.FontRefreshed, new Action(UpdateOffset));
		}
	}

	private void UpdateOffset()
	{
		base.transform.localPosition += _offset;
	}
}
