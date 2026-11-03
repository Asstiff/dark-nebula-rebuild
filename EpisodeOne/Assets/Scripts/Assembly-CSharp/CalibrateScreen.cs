using FLD;
using UnityEngine;

public class CalibrateScreen : MonoBehaviour
{
	public delegate void HandlerDelegate();

	public SimpleButton ButtonCalibrate;

	public SimpleButton ButtonGoBack;

	public DynamicFont TitleText;

	public DynamicFont Text;

	private float _triggerGoBackAtTime;

	public event HandlerDelegate EventGoBack;

	private void Start()
	{
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		Text.Text = "CALIBRATE TEXT".Localize();
		ButtonCalibrate.Text = "CALIBRATE CONTROLS".Localize();
		ButtonCalibrate.ButtonUpEvent += delegate
		{
			UserSettings.Instance.Calibrate();
			Text.GetComponent<Renderer>().enabled = false;
			TitleText.Text = "CALIBRATED!".Localize();
			ButtonGoBack.gameObject.SetActive(false);
			ButtonCalibrate.gameObject.SetActive(false);
			_triggerGoBackAtTime = Time.realtimeSinceStartup + 2f;
		};
		ButtonGoBack.Text = "GO BACK".Localize();
		ButtonGoBack.ButtonUpEvent += delegate
		{
			OnGoBack();
		};
	}

	public void Show()
	{
		_triggerGoBackAtTime = 0f;
		Text.GetComponent<Renderer>().enabled = true;
		TitleText.Text = "CALIBRATE".Localize();
		ButtonGoBack.gameObject.SetActive(true);
		ButtonCalibrate.gameObject.SetActive(true);
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
