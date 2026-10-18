using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class RXListExtensions
{
	public static T RDAdd<T>(this List<T> list, T item)
	{
		if (item != null)
		{
			list.Add(item);
		}
		return item;
	}

	public static T RDAdd<T, TValue>(this Dictionary<T, TValue> dict, T item, TValue value)
	{
		if (item != null)
		{
			dict.Add(item, value);
		}
		return item;
	}

	public static void Fill<T>(this ICollection<T> lst, T val, int num)
	{
		lst.Clear();
		for (int i = 0; i < num; i++)
		{
			lst.Add(val);
		}
	}

	public static void Fill<T>(this List<T> lst, T val)
	{
		lst.Fill(val, lst.Capacity);
	}

	public static void Log<T>(this List<T> list)
	{
		list.Log("");
	}

	public static void Log<T>(this List<T> list, string name)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (name != "")
		{
			stringBuilder.Append(name);
			stringBuilder.Append(": ");
		}
		stringBuilder.Append('[');
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			stringBuilder.Append(list[i].ToString());
			if (i < count - 1)
			{
				stringBuilder.Append(',');
			}
		}
		stringBuilder.Append(']');
		Debug.Log(stringBuilder.ToString());
	}

	public static T Unshift<T>(this List<T> list)
	{
		T result = list[0];
		list.RemoveAt(0);
		return result;
	}

	public static T Pop<T>(this List<T> list)
	{
		T result = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
		return result;
	}

	public static T GetLastObject<T>(this List<T> list)
	{
		return list[list.Count - 1];
	}

	public static void InsertionSort<T>(this List<T> list, Comparison<T> comparison)
	{
		int count = list.Count;
		for (int i = 1; i < count; i++)
		{
			T val = list[i];
			int num = i - 1;
			while (num >= 0 && comparison(list[num], val) > 0)
			{
				list[num + 1] = list[num];
				num--;
			}
			list[num + 1] = val;
		}
	}

	public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
	{
		if (indexB > -1 && indexB < list.Count)
		{
			T value = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = value;
		}
		return list;
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		System.Random random = new System.Random();
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int num2 = random.Next(num + 1);
			int index = num;
			int index2 = num2;
			T val = list[num2];
			T val2 = list[num];
			T val3 = (list[index] = val);
			val3 = (list[index2] = val2);
		}
	}
}
