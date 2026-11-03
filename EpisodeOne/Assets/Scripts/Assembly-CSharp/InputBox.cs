using UnityEngine;

public class InputBox : MonoBehaviour
{
	private static InputBox _focusedBox;

	[SerializeField]
	private bool _hideText;

	[SerializeField]
	private string _textMask = "*";

	[SerializeField]
	private TouchScreenKeyboardType _keyboardType;

	[SerializeField]
	private DynamicFont _inputTextMesh;

	[SerializeField]
	private Collider _inputCollider;

	private string _text;

	private TouchScreenKeyboard _keyboard;

	private char MaskCharacter
	{
		get
		{
			return (!string.IsNullOrEmpty(_textMask)) ? _textMask[0] : '*';
		}
	}

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
			if (_hideText)
			{
				_inputTextMesh.Text = new string(MaskCharacter, _text.Length);
			}
			else
			{
				_inputTextMesh.Text = _text;
			}
		}
	}

	private bool HasFocus
	{
		get
		{
			return _focusedBox == this;
		}
	}

	public void Focus()
	{
		_focusedBox = this;
	}

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.MaxValue) && hitInfo.collider == _inputCollider)
			{
				Focus();
				Click(Input.mousePosition);
			}
		}
		if (_keyboard != null && _keyboard.done)
		{
			if (!_keyboard.wasCanceled)
			{
				Text = _keyboard.text;
			}
			_keyboard = null;
		}
		if (Application.isEditor && HasFocus && !string.IsNullOrEmpty(Input.inputString))
		{
			string text = Text;
			string inputString = Input.inputString;
			foreach (char c in inputString)
			{
				text = ((c != '\b' || text.Length <= 0) ? (text + c) : text.Substring(0, text.Length - 1));
			}
			Text = text;
		}
	}

	private void Click(Vector2 pPos)
	{
		if (!Application.isEditor)
		{
			_keyboard = TouchScreenKeyboard.Open(Text, _keyboardType, false, false, _hideText);
		}
	}
}
