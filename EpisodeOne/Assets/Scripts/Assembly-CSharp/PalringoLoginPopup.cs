using System;
using FLD;
using UnityEngine;

public class PalringoLoginPopup : MonoBehaviour
{
	private const string PopupResourcePath = "Prefabs/UI/PalringoLoginPopup";

	public Action PopupClosed;

	[SerializeField]
	private SimpleButton _backButton;

	[SerializeField]
	private SimpleButton _loginButton;

	[SerializeField]
	private SimpleButton _getPalringoButton;

	[SerializeField]
	private SimpleButton _successContinueButton;

	[SerializeField]
	private InputBox _usernameBox;

	[SerializeField]
	private InputBox _passwordBox;

	[SerializeField]
	private DynamicFont _errorText;

	[SerializeField]
	private GameObject _loginContainer;

	[SerializeField]
	private GameObject _waitContainer;

	[SerializeField]
	private GameObject _successContainer;

	public event Action<string, string> Accepted;

	public event Action Canceled;

	private void Awake()
	{
		_backButton.ButtonUpEvent += HandleBackButton;
		_loginButton.ButtonUpEvent += HandleLoginButton;
		_successContinueButton.ButtonUpEvent += HandleSuccessContinueButton;
		_getPalringoButton.ButtonUpEvent += HandlePalringoButton;
	}

	private void HandlePalringoButton(SimpleButton source)
	{
		DarkNebulaPalringo.Connect.OpenAppStore();
	}

	private void HandleSuccessContinueButton(SimpleButton source)
	{
		Close();
	}

	private void Start()
	{
		_successContinueButton.Text = "CONTINUE".Localize().ToUpper();
		_loginButton.Text = "LOG IN".Localize();
		_getPalringoButton.Text = "GET PALRINGO".Localize();
		_errorText.Text = string.Empty;
		Show(_loginContainer);
	}

	private void Show(GameObject pContainer)
	{
		DarkNebulaMetrics.PalringoConnectTry();
		_loginContainer.SetActive(_loginContainer == pContainer);
		_waitContainer.SetActive(_waitContainer == pContainer);
		_successContainer.SetActive(_successContainer == pContainer);
	}

	private void HandleBackButton(SimpleButton pButton)
	{
		if (Canceled != null)
		{
			Canceled();
		}
		DarkNebulaMetrics.PalringoConnectCancel();
		Close();
	}

	private void HandleLoginButton(SimpleButton source)
	{
		Show(_waitContainer);
		if (Accepted != null)
		{
			Accepted(_usernameBox.Text, _passwordBox.Text);
		}
	}

	public void Close()
	{
		if (PopupClosed != null)
		{
			PopupClosed();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void TryAgain(string pMessage)
	{
		_errorText.Text = pMessage;
		_passwordBox.Text = string.Empty;
		Show(_loginContainer);
	}

	public void LoginSucceeded()
	{
		DarkNebulaMetrics.PalringoConnectSuccess();
		Show(_successContainer);
	}

	public static PalringoLoginPopup Open()
	{
		PalringoLoginPopup palringoLoginPopup = (PalringoLoginPopup)UnityEngine.Object.Instantiate(Resources.Load<PalringoLoginPopup>("Prefabs/UI/PalringoLoginPopup"));
		if (palringoLoginPopup != null)
		{
			palringoLoginPopup.transform.parent = Camera.main.transform;
			palringoLoginPopup.transform.localPosition = new Vector3(0f, 0f, Camera.main.nearClipPlane + 10f);
		}
		return palringoLoginPopup;
	}
}
