using System.Collections.Generic;

namespace FLD
{
	public static class IListExtensions
	{
		public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
		{
			if (firstIndex != secondIndex)
			{
				T value = list[firstIndex];
				list[firstIndex] = list[secondIndex];
				list[secondIndex] = value;
			}
		}
	}
}
