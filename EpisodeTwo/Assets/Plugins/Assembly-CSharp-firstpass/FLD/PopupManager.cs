namespace FLD
{
	public abstract class PopupManager
	{
		public delegate void MessageCallback(int pButton);

		public delegate void SingleFieldPromptCallback(bool pConfirmed, string pText);

		public delegate void DoubleFieldPromptCallback(bool pConfirmed, string pText, string pText2);

		public abstract void ShowMessageWithButton(string pTitle, string pMsg, string pBtn, MessageCallback pOnResponse);

		public abstract void ShowMessageWithButtons(string pTitle, string pMsg, string pBtn1, string pBtn2, MessageCallback pOnResponse);

		public abstract void ShowPrompt(string pTitle, string pMsg, string pPlaceholder, SingleFieldPromptCallback pCallback);

		public abstract void ShowPromptWithTwoFields(string pTitle, string pMsg, string pPlaceholder1, string pPlaceholder2, DoubleFieldPromptCallback pCallback);

		public abstract void ShowPasswordPrompt(string pTitle, string pMsg, string pPlaceholder, DoubleFieldPromptCallback pCallback);

		public abstract void ShowLoader(string pText);

		public abstract void HideLoader();
	}
}
