using System;
using System.Collections.Generic;
using System.Text;

namespace FLD
{
	public static class EnumerableExtensions
	{
		public static T First<T>(this IEnumerable<T> pEnumerable)
		{
			using (IEnumerator<T> enumerator = pEnumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
				throw new InvalidOperationException("Enumerable is empty");
			}
		}

		public static List<T> ToList<T>(this IEnumerable<T> pEnumerable)
		{
			List<T> list = new List<T>();
			foreach (T item in pEnumerable)
			{
				list.Add(item);
			}
			return list;
		}

		public static string Join(string pSeparator, IEnumerable<string> pValues)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerator<string> enumerator = pValues.GetEnumerator();
			if (enumerator.MoveNext())
			{
				stringBuilder.Append(enumerator.Current);
				while (enumerator.MoveNext())
				{
					stringBuilder.Append(pSeparator).Append(enumerator.Current);
				}
			}
			return stringBuilder.ToString();
		}

		public static IEnumerable<T> Sorted<T>(this IEnumerable<T> pEnumerable, Comparison<T> pComparer = null)
		{
			List<T> list = new List<T>(pEnumerable);
			if (pComparer != null)
			{
				list.Sort(pComparer);
			}
			else
			{
				list.Sort();
			}
			return list;
		}

		public static T[] Concatenate<T>(this T[] pArray1, params T[][] pMore)
		{
			int num = pArray1.Length;
			foreach (T[] array in pMore)
			{
				num += array.Length;
			}
			T[] array2 = new T[num];
			pArray1.CopyTo(array2, 0);
			int num2 = pArray1.Length;
			foreach (T[] array3 in pMore)
			{
				array3.CopyTo(array2, num2);
				num2 += array3.Length;
			}
			return array2;
		}
	}
}
