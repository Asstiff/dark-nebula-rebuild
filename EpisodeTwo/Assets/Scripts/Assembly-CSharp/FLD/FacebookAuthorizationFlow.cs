using System;

namespace FLD
{
	public static class FacebookAuthorizationFlow
	{
		public enum AuthorizationResponse
		{
			NotAuthorized = 0,
			PartiallyAuthorized = 1,
			FullyAuthorized = 2
		}

		public static void Authorize(Action<AuthorizationResponse> pCallback)
		{
		}
	}
}
