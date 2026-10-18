using System.Text;
using UnityEngine;

public static class RXArrayExtensions
{
	public static void RemoveItem<T>(this T[] items, T itemToRemove, ref int count) where T : class
	{
		bool flag = false;
		for (int i = 0; i < count; i++)
		{
			if (flag)
			{
				T val = items[i];
				items[i - 1] = val;
			}
			else if (items[i] == itemToRemove)
			{
				flag = true;
			}
		}
		if (flag)
		{
			count--;
		}
	}

	public static void Log<T>(this T[] items)
	{
		items.Log("");
	}

	public static void Log<T>(this T[] items, string name)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (name != "")
		{
			stringBuilder.Append(name);
			stringBuilder.Append(": ");
		}
		stringBuilder.Append('[');
		int num = items.Length;
		for (int i = 0; i < num; i++)
		{
			stringBuilder.Append(items[i].ToString());
			if (i < num - 1)
			{
				stringBuilder.Append(',');
			}
		}
		stringBuilder.Append(']');
		Debug.Log(stringBuilder.ToString());
	}
}
