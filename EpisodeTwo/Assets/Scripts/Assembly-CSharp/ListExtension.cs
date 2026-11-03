using System;
using System.Collections.Generic;

public static class ListExtension
{
	public static void Exclude<T>(this List<T> list, List<T> exclude)
	{
		foreach (T item in exclude)
		{
			list.Remove(item);
		}
	}

	public static void Once(this Action Signal, Action Callback)
	{
		Action handler = null;
		handler = () =>
		{
			Signal = (Action)Delegate.Remove(Signal, handler);
			Callback();
		};
		Signal = (Action)Delegate.Combine(Signal, handler);
	}
}
