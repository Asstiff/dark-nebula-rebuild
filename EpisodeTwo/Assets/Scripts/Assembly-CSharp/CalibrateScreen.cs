using UnityEngine;

public class CalibrateScreen : MonoBehaviour
{
	public delegate void HandlerDelegate();

	public GameObject ButtonCalibrate;

	public GameObject ButtonGoBack;

	public GameObject Title;

	private SimpleText _titleText;

	private float _triggerGoBackAtTime;

	public event HandlerDelegate EventGoBack;

	private void Start()
	{
		SimpleText[] componentsInChildren = GetComponentsInChildren<SimpleText>();
		foreach (SimpleText simpleText in componentsInChildren)
		{
			simpleText.UseCurrentTextAsStringID();
		}
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
		_triggerGoBackAtTime = 0f;
		SimpleText[] componentsInChildren = GetComponentsInChildren<SimpleText>();
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

	private void OnGoBack()
	{
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
