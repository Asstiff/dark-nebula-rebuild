using System;
using System.Collections.Generic;
using UnityEngine;

namespace FLD
{
	public class iOSFacebookManager : FacebookManager
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
			Debug.LogError("Unsupported platform");
		}

		public override void Authorize(Action pCallback)
		{
		}

		public override void EndSession()
		{
		}

		protected override void GraphRequest(string pPath, string pMethod, Dictionary<string, object> pParameters, Action<object> pCallback)
		{
		}

		public override void ShowPostDialog(string pTitle, string pCaption, string pMessage, string pLinkURL, string pPictureURL)
		{
		}

		protected override void ShowNativeComposer(string pText)
		{
		}

		protected override void ShowNativeComposer(string pText, string pImagePath, string pLinkURL)
		{
		}
	}
}
