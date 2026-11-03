namespace FLD
{
	[Service("Native Dialogs")]
	public interface INativeDialogService : IService
	{
		Async ShowMessage(string pMessage, string pCaption);

		Async<bool> ShowYesNoQuestion(string pMessage, string pCaption);
	}
}
