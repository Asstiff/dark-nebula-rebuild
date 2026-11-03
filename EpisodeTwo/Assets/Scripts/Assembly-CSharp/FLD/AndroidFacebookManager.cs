using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

namespace FLD
{
	public class AndroidFacebookManager : FacebookManager
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
				List<string> list = new List<string>();
				if (FacebookAndroid.isSessionValid())
				{
					foreach (object sessionPermission in FacebookAndroid.getSessionPermissions())
					{
						list.Add(sessionPermission as string);
					}
				}
				return list;
			}
		}

		public override void Initialize(string pAppId, string[] pReadPermissions, string[] pPublishPermissions)
		{
			base.ApplicationId = pAppId;
			base.ReadPermissions = pReadPermissions;
			base.PublishPermissions = pPublishPermissions;
			global::FacebookManager.dialogFailedEvent += OnDialogFailed;
			global::FacebookManager.loginFailedEvent += OnLoginFailed;
			global::FacebookManager.reauthorizationFailedEvent += OnReauthorizationFailed;
			global::FacebookManager.reauthorizationSucceededEvent += OnReauthorizationSucceeded;
			global::FacebookManager.sessionOpenedEvent += OnSessionOpened;
			FacebookAndroid.init();
			base.IsAvailable = true;
		}

		public override void Authorize(Action pCallback)
		{
			if (HasPermissions(base.ReadPermissions.Concatenate(base.PublishPermissions)))
			{
				Debug.Log("Facebook: already have all permissions");
				if (pCallback != null)
				{
					pCallback();
				}
				return;
			}
			Action<bool> callbackWrapper = null;
			if (pCallback != null)
			{
				callbackWrapper = (bool pWasCancelled) =>
				{
					base.Authorized -= callbackWrapper;
					pCallback();
				};
			}
			Action<bool> stepTwoWrapper = null;
		stepTwoWrapper = (bool pWasCancelled) =>
			{
				base.Authorized -= stepTwoWrapper;
				if (!pWasCancelled)
				{
					if (callbackWrapper != null)
					{
						base.Authorized += callbackWrapper;
					}
					Debug.Log("Facebook: reauthorizing with publish permissions");
					FacebookAndroid.reauthorizeWithPublishPermissions(base.PublishPermissions, FacebookSessionDefaultAudience.FRIENDS);
				}
				else
				{
					Debug.Log("Facebook: canceled login");
					if (pCallback != null)
					{
						pCallback();
					}
				}
			};
			if (FacebookAndroid.isSessionValid())
			{
				if (HasPermissions(base.ReadPermissions))
				{
					Debug.Log("Facebook: session already started, already have read permissions, reauthorizing with publish permissions");
					if (callbackWrapper != null)
					{
						base.Authorized += callbackWrapper;
					}
					FacebookAndroid.reauthorizeWithReadPermissions(base.ReadPermissions);
				}
				else
				{
					Debug.Log("Facebook: session already started, reauthorizing with read permissions");
					base.Authorized += stepTwoWrapper;
					FacebookAndroid.reauthorizeWithReadPermissions(base.ReadPermissions);
				}
			}
			else
			{
				Debug.Log("Facebook: logging in with read permissions");
				base.Authorized += stepTwoWrapper;
				FacebookAndroid.loginWithReadPermissions(base.ReadPermissions);
			}
		}

		public override void EndSession()
		{
			FacebookAndroid.logout();
		}

		protected override void GraphRequest(string pPath, string pMethod, Dictionary<string, object> pParameters, Action<object> pCallback)
		{
			HTTPVerb verb;
			if (pMethod == "POST")
			{
				verb = HTTPVerb.POST;
			}
			else
			{
				if (!(pMethod == "GET"))
				{
					Debug.LogError("Unrecognizable graph request method \"" + pMethod + "\"");
					return;
				}
				verb = HTTPVerb.GET;
			}
			Facebook.instance.graphRequest(pPath, verb, pParameters, (string s, object obj) =>
			{
				if (pCallback != null)
				{
					pCallback(obj);
				}
			});
		}

		protected override void ShowNativeComposer(string pText)
		{
			Debug.LogWarning("Native composer not supported on Android");
		}

		protected override void ShowNativeComposer(string pText, string pImagePath, string pLinkURL)
		{
			Debug.LogWarning("Native composer not supported on Android");
		}

		public override void ShowPostDialog(string pTitle, string pCaption, string pMessage, string pLinkURL, string pPictureURL)
		{
			if (FacebookAndroid.isSessionValid())
			{
				FacebookAndroid.showDialog("feed", new Dictionary<string, string>
				{
					{ "app_id", base.ApplicationId },
					{ "link", pLinkURL },
					{ "picture", pPictureURL },
					{ "name", pTitle },
					{ "caption", pCaption },
					{ "description", pMessage }
				});
				return;
			}
			Authorize(() =>
			{
				if (FacebookAndroid.isSessionValid())
				{
					ShowPostDialog(pTitle, pCaption, pMessage, pLinkURL, pPictureURL);
				}
			});
		}

		private void UpdateUserID()
		{
			if (!string.IsNullOrEmpty(base.UserId))
			{
				return;
			}
			GraphGet("me", new Dictionary<string, object>(), (object obj) =>
			{
				Hashtable hashtable = obj as Hashtable;
				if (hashtable != null)
				{
					base.UserId = (string)hashtable["id"];
					Debug.Log("Got user's Facebook ID: " + base.UserId);
				}
				else
				{
					Debug.LogWarning("Failed to fetch user's Facebook ID");
				}
			});
		}

		private void OnSessionOpened()
		{
			UpdateUserID();
			Debug.Log(string.Format("Facebook: Authorized. Access token: {0}  Permissions: {1}", FacebookAndroid.getAccessToken(), string.Join(", ", GrantedPermissions.ToArray())));
			OnAuthorized(false);
		}

		private void OnLoginFailed(P31Error pReason)
		{
			Debug.LogError("Facebook login failed: " + pReason.message);
			FacebookAndroid.logout();
			OnAuthorized(true);
		}

		private void OnReauthorizationSucceeded()
		{
			Debug.Log(string.Format("Facebook: Reauthorized. Permissions: {0}", string.Join(", ", GrantedPermissions.ToArray())));
			OnAuthorized(false);
		}

		private void OnReauthorizationFailed(P31Error pReason)
		{
			Debug.LogError("Facebook reauthorization failed: " + pReason.message);
			OnAuthorized(true);
			FacebookAndroid.logout();
		}

		private void OnDialogFailed(P31Error pReason)
		{
			Debug.LogError("Facebook dialog failed: " + pReason.message);
		}
	}
}
