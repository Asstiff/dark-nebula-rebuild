using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace FLD
{
	public static class HashUtility
	{
		private static MD5CryptoServiceProvider _md5Provider = new MD5CryptoServiceProvider();

		public static string MD5Sum(this string pStringToHash)
		{
			return pStringToHash.MD5SumBytes().ToHexString();
		}

		public static byte[] MD5SumBytes(this string pStringToHash)
		{
			return _md5Provider.ComputeHash(Encoding.UTF8.GetBytes(pStringToHash));
		}

		public static string HMACSHA256(this string pStringToHash, string pSecret)
		{
			using (HMACSHA256 hMACSHA = new HMACSHA256(Encoding.ASCII.GetBytes(pSecret)))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(pStringToHash);
				if (bytes != null)
				{
					return hMACSHA.ComputeHash(bytes).ToHexString();
				}
				Debug.LogError("UTF8.GetBytes returned null");
				return string.Empty;
			}
		}

		public static string HMACSHA1(this string pStringToHash, string pSecret)
		{
			using (HMACSHA1 hMACSHA = new HMACSHA1(Encoding.ASCII.GetBytes(pSecret)))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(pStringToHash);
				if (bytes != null)
				{
					return hMACSHA.ComputeHash(bytes).ToHexString();
				}
				Debug.LogError("UTF8.GetBytes returned null");
				return string.Empty;
			}
		}
	}
}
