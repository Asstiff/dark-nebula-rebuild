namespace FLD
{
	public static class Match
	{
		public static bool Glob(string pPattern, string pMatch)
		{
			if (pPattern == "*" || pPattern == pMatch)
			{
				return true;
			}
			return Glob(pPattern, 0, pMatch, 0);
		}

		private static bool Glob(string pPattern, int pPatternIdx, string pMatch, int pMatchIdx)
		{
			if (pMatchIdx >= pMatch.Length)
			{
				return pPatternIdx >= pPattern.Length || (pPattern[pPatternIdx] == '*' && Glob(pPattern, pPatternIdx + 1, pMatch, pMatchIdx));
			}
			if (pPatternIdx >= pPattern.Length)
			{
				return pMatchIdx >= pMatch.Length;
			}
			char c = pPattern[pPatternIdx];
			if (c == '*')
			{
				return Glob(pPattern, pPatternIdx, pMatch, pMatchIdx + 1) || Glob(pPattern, pPatternIdx + 1, pMatch, pMatchIdx);
			}
			return (c == '?' || c == pMatch[pMatchIdx]) && Glob(pPattern, pPatternIdx + 1, pMatch, pMatchIdx + 1);
		}
	}
}
