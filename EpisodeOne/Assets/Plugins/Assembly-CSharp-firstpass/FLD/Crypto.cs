using System.Security.Cryptography;
using System.Text;

namespace FLD
{
	public static class Crypto
	{
		private static MD5CryptoServiceProvider _md5Provider = new MD5CryptoServiceProvider();

		public static string MD5(this string pStringToHash)
		{
			return pStringToHash.MD5Bytes().ToHexString();
		}

		public static string MD5(this byte[] pBytesToHash)
		{
			return pBytesToHash.MD5Bytes().ToHexString();
		}

		public static byte[] MD5Bytes(this string pStringToHash)
		{
			return _md5Provider.ComputeHash(Encoding.UTF8.GetBytes(pStringToHash));
		}

		public static byte[] MD5Bytes(this byte[] pBytesToHash)
		{
			return _md5Provider.ComputeHash(pBytesToHash);
		}

		public static byte[] MD5Bytes(this byte[] pBytesToHash, int pOffset, int pLength)
		{
			return _md5Provider.ComputeHash(pBytesToHash, pOffset, pLength);
		}

		public static string HMACSHA256(this string pStringToHash, string pSecret)
		{
			using (HMACSHA256 hMACSHA = new HMACSHA256(Encoding.ASCII.GetBytes(pSecret)))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(pStringToHash);
				return hMACSHA.ComputeHash(bytes).ToHexString();
			}
		}

		public static string HMACSHA1(this string pStringToHash, string pSecret)
		{
			using (HMACSHA1 hMACSHA = new HMACSHA1(Encoding.ASCII.GetBytes(pSecret)))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(pStringToHash);
				return hMACSHA.ComputeHash(bytes).ToHexString();
			}
		}
	}
}
