using System.Collections.Generic;
using System.IO;

namespace FLD
{
	public static class PathUtilities
	{
		public static string[] CollectFiles(string pDirectory)
		{
			return CollectFiles(pDirectory, (PathPattern[])null, (PathPattern[])null);
		}

		public static string[] CollectFiles(string pDirectory, PathPattern pIncludePattern, PathPattern pExcludePattern)
		{
			PathPattern[] pIncludePattern2 = ((pIncludePattern == null) ? null : new PathPattern[1] { pIncludePattern });
			PathPattern[] pExcludePattern2 = ((pExcludePattern == null) ? null : new PathPattern[1] { pExcludePattern });
			return CollectFiles(pDirectory, pIncludePattern2, pExcludePattern2);
		}

		public static string[] CollectFiles(string pDirectory, PathPattern[] pIncludePattern, PathPattern[] pExcludePattern)
		{
			List<string> list = new List<string>();
			string fullPath = Path.GetFullPath(pDirectory);
			RecursiveCollectFiles(fullPath, fullPath, pIncludePattern, pExcludePattern, list);
			return list.ToArray();
		}

		public static string[] CollectFiles(string pDirectory, string pIncludePattern, string pExcludePattern)
		{
			return CollectFiles(pDirectory, new PathPattern(pIncludePattern), new PathPattern(pExcludePattern));
		}

		public static string[] CollectFiles(string pDirectory, PathPattern pIncludePattern)
		{
			return CollectFiles(pDirectory, pIncludePattern, null);
		}

		public static string[] CollectFiles(string pDirectory, string pIncludePattern)
		{
			return CollectFiles(pDirectory, new PathPattern(pIncludePattern));
		}

		private static void RecursiveCollectFiles(string pBaseDirectory, string pScanDirectory, PathPattern[] pIncludePatterns, PathPattern[] pExcludePatterns, List<string> pOut)
		{
			string[] fileSystemEntries = Directory.GetFileSystemEntries(pScanDirectory);
			foreach (string text in fileSystemEntries)
			{
				if (Directory.Exists(text))
				{
					RecursiveCollectFiles(pBaseDirectory, text, pIncludePatterns, pExcludePatterns, pOut);
				}
				string relEntry = GetRelativePath(text, pBaseDirectory);
				if ((pIncludePatterns == null || pIncludePatterns.Any((PathPattern pattern) => pattern.Matches(relEntry))) && (pExcludePatterns == null || !pExcludePatterns.Any((PathPattern pattern) => pattern.Matches(relEntry))))
				{
					pOut.Add(relEntry);
				}
			}
		}

		public static string GetRelativePath(string pPath, string pBasePath)
		{
			string fullPath = Path.GetFullPath(pPath);
			string fullPath2 = Path.GetFullPath(pBasePath);
			if (!fullPath.StartsWith(fullPath2))
			{
				return null;
			}
			if (fullPath2[fullPath2.Length - 1] == Path.DirectorySeparatorChar)
			{
				return fullPath.Substring(fullPath2.Length);
			}
			return fullPath.Substring(fullPath2.Length + 1);
		}

		public static bool IsSubpathOf(string pPath, string pBasePath)
		{
			string fullPath = Path.GetFullPath(pPath);
			string fullPath2 = Path.GetFullPath(pBasePath);
			return fullPath.StartsWith(fullPath2);
		}
	}
}
