namespace FLD
{
	public class iOSPopupManager : PopupManager
	{
		public override void ShowMessageWithButton(string pTitle, string pMsg, string pBtn, MessageCallback pOnResponse)
		{
		}

		public override void ShowMessageWithButtons(string pTitle, string pMsg, string pBtn1, string pBtn2, MessageCallback pOnResponse)
		{
		}

		public override void ShowPrompt(string pTitle, string pMsg, string pPlaceholder, SingleFieldPromptCallback pCallback)
		{
		}

		public override void ShowPromptWithTwoFields(string pTitle, string pMsg, string pPlaceholder1, string pPlaceholder2, DoubleFieldPromptCallback pCallback)
		{
		}

		public override void ShowPasswordPrompt(string pTitle, string pMsg, string pPlaceholder, DoubleFieldPromptCallback pCallback)
		{
		}

		public override void ShowLoader(string pText)
		{
		}

		public override void HideLoader()
		{
		}
	}
}
