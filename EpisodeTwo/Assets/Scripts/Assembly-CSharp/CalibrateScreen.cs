using UnityEngine;

public class CalibrateScreen : MonoBehaviour
{
	public delegate void HandlerDelegate();

	public GameObject ButtonCalibrate;

	public GameObject ButtonGoBack;

	public GameObject Title;

	private SimpleText _titleText;

	private float _triggerGoBackAtTime;
	private readonly Transform[] _instructionLines = new Transform[8];
	private readonly Vector3[] _instructionPositions = new Vector3[8];

	public event HandlerDelegate EventGoBack;

	private void Start()
	{
		SimpleText[] componentsInChildren = GetComponentsInChildren<SimpleText>();
		foreach (SimpleText simpleText in componentsInChildren)
		{
			simpleText.UseCurrentTextAsStringID();
			for (int i = 0; i < _instructionLines.Length; i++)
			{
				if (simpleText.name != "Text" + i) continue;
				_instructionLines[i] = simpleText.transform;
				_instructionPositions[i] = simpleText.transform.localPosition;
			}
		}
		RefreshInstructionLayout();
		ButtonCalibrate.GetComponent<SimpleButton>().ButtonUpEvent += delegate
		{
			UserSettings.I.Calibrate();
			SimpleText[] componentsInChildren2 = GetComponentsInChildren<SimpleText>();
			foreach (SimpleText simpleText2 in componentsInChildren2)
			{
				simpleText2.GetComponent<Renderer>().enabled = false;
			}
			Title.GetComponent<SimpleText>().SetStringID("CALIBRATED!");
			Title.GetComponent<Renderer>().enabled = true;
			ButtonGoBack.SetActive(false);
			ButtonCalibrate.SetActive(false);
			_triggerGoBackAtTime = Time.realtimeSinceStartup + 2f;
		};
		ButtonGoBack.GetComponent<SimpleButton>().ButtonUpEvent += delegate
		{
			OnGoBack();
		};
		_titleText = Title.GetComponent<SimpleText>();
	}

	private void OnEnable()
	{
		GamePreferences.Changed += RefreshInstructionLayout;
		RefreshInstructionLayout();
		_triggerGoBackAtTime = 0f;
		// The previous calibration leaves both button hierarchies inactive.
		// Restore their text renderers as well as the currently active labels.
		SimpleText[] componentsInChildren = GetComponentsInChildren<SimpleText>(true);
		foreach (SimpleText simpleText in componentsInChildren)
		{
			simpleText.GetComponent<Renderer>().enabled = true;
		}
		if (_titleText != null)
		{
			_titleText.SetStringID("CALIBRATE");
		}
		ButtonGoBack.SetActive(true);
		ButtonCalibrate.SetActive(true);
	}

	private void OnDisable()
	{
		GamePreferences.Changed -= RefreshInstructionLayout;
	}

	private void RefreshInstructionLayout()
	{
		if (_instructionLines[0] == null) return;
		for (int i = 0; i < _instructionLines.Length; i++)
		{
			if (_instructionLines[i] == null) continue;
			Vector3 position = _instructionPositions[i];
			if (GamePreferences.Chinese)
				position.y = _instructionPositions[0].y - i * 22f - (i >= 4 ? 12f : 0f);
			_instructionLines[i].localPosition = position;
		}
	}

	private void OnGoBack()
	{
		_triggerGoBackAtTime = 0f;
		if (EventGoBack != null)
		{
			EventGoBack();
		}
	}

	public virtual void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			OnGoBack();
		}
		if (_triggerGoBackAtTime != 0f && !(_triggerGoBackAtTime > Time.realtimeSinceStartup))
		{
			OnGoBack();
		}
	}
}
