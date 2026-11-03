using System;
using System.Collections.Generic;

namespace FLD
{
	public abstract class FacebookManager
	{
		private static FacebookManager _instance;

		public string ApplicationId { get; protected set; }

		public string UserId { get; protected set; }

		public string[] ReadPermissions { get; protected set; }

		public string[] PublishPermissions { get; protected set; }

		public static FacebookManager Instance
		{
			get
			{
				if (_instance == null)
				{
					// The legacy Android Facebook bridge is unavailable in the offline build.
					_instance = new DummyFacebookManager();
				}
				return _instance;
			}
		}

		public bool IsAvailable { get; protected set; }

		public bool HasBasicInformation
		{
			get
			{
				List<string> grantedPermissions = GrantedPermissions;
				foreach (string item in grantedPermissions)
				{
					switch (item)
					{
					case "user_birthday":
					case "email":
					case "user_location":
						return true;
					}
				}
				return false;
			}
		}

		public bool HasAllPermissions
		{
			get
			{
				return HasPermissions(ReadPermissions.Concatenate(PublishPermissions));
			}
		}

		public abstract List<string> GrantedPermissions { get; }

		public abstract bool CanUseNativeComposer { get; }

		public event Action<bool> Authorized;

		protected void OnAuthorized(bool pWasCancelled)
		{
			if (Authorized != null)
			{
				Authorized(pWasCancelled);
			}
		}

		public bool HasPermission(string pPermission)
		{
			return HasPermissions(new string[1] { pPermission });
		}

		public bool HasPermissions(string[] pPermissions)
		{
			List<string> grantedPermissions = GrantedPermissions;
			foreach (string item in pPermissions)
			{
				if (!grantedPermissions.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		public abstract void Initialize(string pAppId, string[] pReadPermissions, string[] pPublishPermissions);

		public abstract void Authorize(Action pCallback);

		public abstract void EndSession();

		public void GraphGet(string pPath, Action<object> pCallback)
		{
			GraphGet(pPath, null, pCallback);
		}

		public void GraphGet(string pPath, Dictionary<string, object> pParameters, Action<object> pCallback)
		{
			GraphRequest(pPath, "GET", pParameters, pCallback);
		}

		public void GraphPost(string pPath, Dictionary<string, object> pParameters)
		{
			GraphPost(pPath, pParameters, null);
		}

		public void GraphPost(string pPath, Dictionary<string, object> pParameters, Action<object> pCallback)
		{
			GraphRequest(pPath, "POST", pParameters, pCallback);
		}

		public abstract void ShowPostDialog(string pTitle, string pCaption, string pMessage, string pLinkURL, string pPictureURL);

		protected abstract void GraphRequest(string pPath, string pMethod, Dictionary<string, object> pParameters, Action<object> pCallback);

		protected abstract void ShowNativeComposer(string pText);

		protected abstract void ShowNativeComposer(string pText, string pImagePath, string pLinkURL);
	}
}
