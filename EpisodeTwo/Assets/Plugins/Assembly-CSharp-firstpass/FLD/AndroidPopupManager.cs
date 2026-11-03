using System;
using UnityEngine;

namespace FLD
{
	public class AndroidPopupManager : PopupManager
	{
		private MessageCallback _messageCallback;

		private SingleFieldPromptCallback _singlePromptCallback;

		private DoubleFieldPromptCallback _doublePromptCallback;

		private string _buttonLabel1;

		private string _buttonLabel2;

		public AndroidPopupManager()
		{
			GameObject gameObject = new GameObject("EtceteraAndroidManager");
			gameObject.AddComponent<EtceteraAndroidManager>();
			EtceteraAndroidManager.alertButtonClickedEvent += OnAlertButtonClicked;
			EtceteraAndroidManager.promptFinishedWithTextEvent += OnSinglePromptEntered;
			EtceteraAndroidManager.twoFieldPromptFinishedWithTextEvent += OnDoublePromptEntered;
		}

		private void OnAlertButtonClicked(string pButton)
		{
			if (_messageCallback != null)
			{
				int pButton2 = -1;
				if (pButton == _buttonLabel1)
				{
					pButton2 = 0;
				}
				if (pButton == _buttonLabel2)
				{
					pButton2 = 1;
				}
				_messageCallback(pButton2);
				_messageCallback = null;
			}
		}

		private void OnSinglePromptEntered(string pText)
		{
			if (_singlePromptCallback != null)
			{
				_singlePromptCallback(true, pText);
				_singlePromptCallback = null;
			}
		}

		private void OnDoublePromptEntered(string pText1, string pText2)
		{
			if (_doublePromptCallback != null)
			{
				_doublePromptCallback(true, pText1, pText2);
				_doublePromptCallback = null;
			}
		}

		private void OnPromptCancelled()
		{
			if (_singlePromptCallback != null)
			{
				_singlePromptCallback(false, null);
			}
			if (_doublePromptCallback != null)
			{
				_doublePromptCallback(false, null, null);
			}
			_singlePromptCallback = null;
			_doublePromptCallback = null;
		}

		public override void ShowMessageWithButton(string pTitle, string pMsg, string pBtn, MessageCallback pOnResponse)
		{
			_buttonLabel1 = pBtn;
			_messageCallback = pOnResponse;
			EtceteraAndroid.showAlert(pTitle, pMsg, pBtn);
		}

		public override void ShowMessageWithButtons(string pTitle, string pMsg, string pBtn1, string pBtn2, MessageCallback pOnResponse)
		{
			_buttonLabel1 = pBtn1;
			_buttonLabel2 = pBtn2;
			_messageCallback = pOnResponse;
			EtceteraAndroid.showAlert(pTitle, pMsg, pBtn1, pBtn2);
		}

		public override void ShowPrompt(string pTitle, string pMsg, string pPlaceholder, SingleFieldPromptCallback pCallback)
		{
			_singlePromptCallback = pCallback;
			EtceteraAndroid.showAlertPrompt(pTitle, pMsg, pPlaceholder, string.Empty, LanguageManager.Instance.GetText("ufGenericBtnOK"), LanguageManager.Instance.GetText("ufGenericBtnCancel"));
		}

		public override void ShowPromptWithTwoFields(string pTitle, string pMsg, string pPlaceholder1, string pPlaceholder2, DoubleFieldPromptCallback pCallback)
		{
			_doublePromptCallback = pCallback;
			EtceteraAndroid.showAlertPromptWithTwoFields(pTitle, pMsg, pPlaceholder1, string.Empty, pPlaceholder2, string.Empty, LanguageManager.Instance.GetText("ufGenericBtnOK"), LanguageManager.Instance.GetText("ufGenericBtnCancel"));
		}

		public override void ShowPasswordPrompt(string pTitle, string pMsg, string pPlaceholder, DoubleFieldPromptCallback pCallback)
		{
			_doublePromptCallback = pCallback;
			EtceteraAndroid.showAlertPromptWithTwoFields(pTitle, pMsg, pPlaceholder, string.Empty, LanguageManager.Instance.GetText("ufGenericHintPassword"), string.Empty, LanguageManager.Instance.GetText("ufGenericBtnOK"), LanguageManager.Instance.GetText("ufGenericBtnCancel"));
		}

		public override void ShowLoader(string pText)
		{
			throw new NotImplementedException();
		}

		public override void HideLoader()
		{
			throw new NotImplementedException();
		}
	}
}
