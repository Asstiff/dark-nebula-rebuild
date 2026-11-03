using FLD;
using UnityEngine;

public class SimpleText : Entity
{
	[SerializeField]
	private bool _translateCurrentText;

	protected tk2dTextMesh _textMesh;

	protected tk2dTextMesh textMesh
	{
		get
		{
			if (_textMesh == null)
			{
				_textMesh = GetComponent<tk2dTextMesh>();
			}
			return _textMesh;
		}
	}

	public float Spacing
	{
		get
		{
			return textMesh.Spacing;
		}
		set
		{
			textMesh.Spacing = value;
			_textMesh.text = _textMesh.text;
			_textMesh.Commit();
		}
	}

	private void Awake()
	{
		if (_translateCurrentText)
		{
			UseCurrentTextAsStringID();
		}
	}

	public virtual void SetText(string text)
	{
		textMesh.text = text;
		_textMesh.Commit();
	}

	public virtual void SetStringID(string stringID)
	{
		textMesh.text = stringID.Localize();
		_textMesh.Commit();
	}

	public void UseCurrentTextAsStringID()
	{
		SetStringID(textMesh.text);
	}
}
