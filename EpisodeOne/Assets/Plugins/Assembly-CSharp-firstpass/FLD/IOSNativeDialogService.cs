namespace FLD
{
	[Service("iOS Native Dialogs", AvailablePlatforms = "iPhone")]
	public class IOSNativeDialogService : IService, INativeDialogService
	{
		public Async<bool> Initialize()
		{
			return Async.MakePrecompleted(true);
		}

		public Async ShowMessage(string pMessage, string pCaption)
		{
			return Async.MakePrecompleted();
		}

		public Async<bool> ShowYesNoQuestion(string pMessage, string pCaption)
		{
			return Async.MakePrecompleted(false);
		}
	}
}
