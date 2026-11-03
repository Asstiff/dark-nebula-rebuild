using System.IO;

namespace FLD
{
	public class PathPattern
	{
		private string[] _patternPieces;

		public PathPattern(string pPattern)
		{
			_patternPieces = SplitPath(pPattern, true);
		}

		private string[] SplitPath(string pPath, bool pIsPattern)
		{
			if (Path.DirectorySeparatorChar != '/')
			{
				pPath = pPath.Replace(Path.DirectorySeparatorChar, '/');
			}
			if (pIsPattern && pPath[pPath.Length - 1] == '/')
			{
				pPath += "**";
			}
			return pPath.Split('/').FilterRemove(string.IsNullOrEmpty).ToArray();
		}

		public bool Matches(string pPath)
		{
			return RecursiveCheckMatch(0, SplitPath(pPath, false), 0);
		}

		public static bool Matches(string pPattern, string pPath)
		{
			return new PathPattern(pPattern).Matches(pPath);
		}

		private bool RecursiveCheckMatch(int pPatternIdx, string[] pMatch, int pMatchIdx)
		{
			if (pPatternIdx >= _patternPieces.Length)
			{
				return pMatchIdx >= pMatch.Length;
			}
			if (pMatchIdx >= pMatch.Length)
			{
				return pPatternIdx == _patternPieces.Length - 1 && _patternPieces[pPatternIdx] == "**";
			}
			if (_patternPieces[pPatternIdx] == "**")
			{
				return RecursiveCheckMatch(pPatternIdx, pMatch, pMatchIdx + 1) || RecursiveCheckMatch(pPatternIdx + 1, pMatch, pMatchIdx);
			}
			return Match.Glob(_patternPieces[pPatternIdx], pMatch[pMatchIdx]) && RecursiveCheckMatch(pPatternIdx + 1, pMatch, pMatchIdx + 1);
		}
	}
}
