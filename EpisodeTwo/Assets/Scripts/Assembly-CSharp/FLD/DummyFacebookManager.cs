using System;
using System.Collections.Generic;

namespace FLD
{
	public class DummyFacebookManager : FacebookManager
	{
		public override bool CanUseNativeComposer
		{
			get
			{
				return false;
			}
		}

		public override List<string> GrantedPermissions
		{
			get
			{
				return new List<string>();
			}
		}

		public override void Initialize(string pAppId, string[] pReadPermissions, string[] pPublishPermissions)
		{
			base.ApplicationId = pAppId;
			base.ReadPermissions = pReadPermissions;
			base.PublishPermissions = pPublishPermissions;
		}

		public override void Authorize(Action pCallback)
		{
			if (pCallback != null)
			{
				pCallback();
			}
		}

		public override void EndSession()
		{
		}

		protected override void GraphRequest(string pPath, string pMethod, Dictionary<string, object> pParameters, Action<object> pCallback)
		{
			if (pCallback != null)
			{
				pCallback(null);
			}
		}

		protected override void ShowNativeComposer(string pText)
		{
		}

		protected override void ShowNativeComposer(string pText, string pImagePath, string pLinkURL)
		{
		}

		public override void ShowPostDialog(string pTitle, string pCaption, string pMessage, string pLinkURL, string pPictureURL)
		{
		}
	}
}
