using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FLD
{
	public static class EnumerableExtensions
	{
		public static bool Any<T>(this IEnumerable<T> pEnumerable, Func<T, bool> pPredicate)
		{
			foreach (T item in pEnumerable)
			{
				if (pPredicate(item))
				{
					return true;
				}
			}
			return false;
		}

		public static T FirstOrDefault<T>(this IEnumerable<T> pEnumerable)
		{
			using (IEnumerator<T> enumerator = pEnumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
				return default(T);
			}
		}

		public static List<T> ToList<T>(this IEnumerable<T> pEnumerable)
		{
			return new List<T>(pEnumerable);
		}

		public static T[] ToArray<T>(this IEnumerable<T> pEnumerable)
		{
			return new List<T>(pEnumerable).ToArray();
		}

		public static bool Contains<T>(this T[] pArray, T pValue)
		{
			for (int i = 0; i < pArray.Length; i++)
			{
				if (pArray[i].Equals(pValue))
				{
					return true;
				}
			}
			return false;
		}

		public static int IndexOf<T>(this T[] pArray, T pValue)
		{
			for (int i = 0; i < pArray.Length; i++)
			{
				if (pArray[i].Equals(pValue))
				{
					return i;
				}
			}
			return -1;
		}

		public static T[] GetResized<T>(this T[] pArray, int pNewSize)
		{
			T[] array = new T[pNewSize];
			for (int i = 0; i < Mathf.Min(array.Length, pArray.Length); i++)
			{
				array[i] = pArray[i];
			}
			return array;
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

		public static List<T> Sorted<T>(this IEnumerable<T> pEnumerable)
		{
			return pEnumerable.Sorted(null);
		}

		public static List<T> Sorted<T>(this IEnumerable<T> pEnumerable, Comparison<T> pComparer)
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

		public static void Shuffle<T>(this List<T> pList)
		{
			for (int num = pList.Count - 1; num >= 1; num--)
			{
				pList.Swap(num, Random.Range(0, num + 1));
			}
		}

		public static void Shuffle<T>(this T[] pArray)
		{
			for (int num = pArray.Length - 1; num >= 1; num--)
			{
				pArray.Swap(num, Random.Range(0, num + 1));
			}
		}

		public static void Swap<T>(this List<T> pList, int pIndexOne, int pIndexTwo)
		{
			T value = pList[pIndexOne];
			pList[pIndexOne] = pList[pIndexTwo];
			pList[pIndexTwo] = value;
		}

		public static void Swap<T>(this T[] pArray, int pIndexOne, int pIndexTwo)
		{
			T val = pArray[pIndexOne];
			pArray[pIndexOne] = pArray[pIndexTwo];
			pArray[pIndexTwo] = val;
		}

		public static T GetRandomWeighted<T>(this T[] pArray, Func<T, float> pWeightFunction)
		{
			float num = Random.value * pArray.Map(pWeightFunction).Sum();
			for (int i = 0; i < pArray.Length; i++)
			{
				num -= pWeightFunction(pArray[i]);
				if (num < 0f)
				{
					return pArray[i];
				}
			}
			return default(T);
		}

		public static T GetRandomWeighted<T>(this IList<T> pList, Func<T, float> pWeightFunction)
		{
			float num = Random.value * pList.Map(pWeightFunction).Sum();
			for (int i = 0; i < pList.Count; i++)
			{
				num -= pWeightFunction(pList[i]);
				if (num < 0f)
				{
					return pList[i];
				}
			}
			return default(T);
		}

		public static T GetRandom<T>(this T[] pArray)
		{
			int num = pArray.Length;
			if (num == 0)
			{
				return default(T);
			}
			return pArray[Random.Range(0, num)];
		}

		public static T GetRandom<T>(this IList<T> pList)
		{
			int count = pList.Count;
			if (count == 0)
			{
				return default(T);
			}
			return pList[Random.Range(0, count)];
		}

		public static T GetRandom<T>(this IEnumerable<T> enumerable)
		{
			return new List<T>(enumerable).GetRandom();
		}

		public static T Find<T>(this IEnumerable<T> pEnumerable, Func<T, bool> pPredicate)
		{
			foreach (T item in pEnumerable)
			{
				if (pPredicate(item))
				{
					return item;
				}
			}
			return default(T);
		}

		public static IEnumerable<T> Filter<T>(this IEnumerable<T> pEnumerable, Func<T, bool> pPredicate)
		{
			foreach (T x in pEnumerable)
			{
				if (pPredicate(x))
				{
					yield return x;
				}
			}
		}

		public static IEnumerable<T> FilterRemove<T>(this IEnumerable<T> pEnumerable, Func<T, bool> pPredicate)
		{
			foreach (T x in pEnumerable)
			{
				if (!pPredicate(x))
				{
					yield return x;
				}
			}
		}

		public static int RemoveIf<T>(this IList<T> pList, Func<T, bool> pPredicate)
		{
			int num = 0;
			for (int num2 = pList.Count - 1; num2 >= 0; num2--)
			{
				if (pPredicate(pList[num2]))
				{
					pList.RemoveAt(num2);
					num++;
				}
			}
			return num;
		}

		public static IEnumerable<S> Map<T, S>(this IEnumerable<T> pEnumerable, Func<T, S> pMapper)
		{
			foreach (T x in pEnumerable)
			{
				yield return pMapper(x);
			}
		}

		public static void Apply<T>(this IEnumerable<T> pEnumerable, Action<T> pFunction)
		{
			foreach (T item in pEnumerable)
			{
				pFunction(item);
			}
		}

		public static T Reduce<T>(this IEnumerable<T> pEnumerable, Func<T, T, T> pReducer)
		{
			using (IEnumerator<T> enumerator = pEnumerable.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					return default(T);
				}
				T val = enumerator.Current;
				while (enumerator.MoveNext())
				{
					val = pReducer(val, enumerator.Current);
				}
				return val;
			}
		}

		public static int Sum(this IEnumerable<int> pEnumerable)
		{
			return pEnumerable.Reduce((int a, int b) => a + b);
		}

		public static float Sum(this IEnumerable<float> pEnumerable)
		{
			return pEnumerable.Reduce((float a, float b) => a + b);
		}

		public static string Join(this IEnumerable<string> pEnumerable, string pJoinString)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (IEnumerator<string> enumerator = pEnumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					stringBuilder.Append(enumerator.Current);
					while (enumerator.MoveNext())
					{
						stringBuilder.Append(pJoinString);
						stringBuilder.Append(enumerator.Current);
					}
				}
			}
			return stringBuilder.ToString();
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

		public static IEnumerable<string> AllToString<T>(this IEnumerable<T> pEnumerable)
		{
			foreach (T item in pEnumerable)
			{
				yield return item.ToString();
			}
		}

		public static IEnumerable<T> Slice<T>(this IList<T> pList, int pOffset)
		{
			for (int i = pOffset; i < pList.Count; i++)
			{
				yield return pList[i];
			}
		}

		public static IEnumerable<T> Slice<T>(this IList<T> pList, int pOffset, int pCount)
		{
			for (int i = pOffset; i < pOffset + pCount && i < pList.Count; i++)
			{
				yield return pList[i];
			}
		}

		public static bool ArrayEquals(this byte[] pLeft, byte[] pRight)
		{
			if (pLeft.Length != pRight.Length)
			{
				return false;
			}
			for (int i = 0; i < pLeft.Length; i++)
			{
				if (pLeft[i] != pRight[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
