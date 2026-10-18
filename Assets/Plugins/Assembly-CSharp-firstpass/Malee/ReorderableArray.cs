using System;
using UnityEngine;

namespace Malee
{
	[Serializable]
	public abstract class ReorderableArray<T>
	{
		[SerializeField]
		private T[] array = new T[0];

		[SerializeField]
		private int _hashCode;

		internal int hashCode => _hashCode;

		public T this[int index]
		{
			get
			{
				return array[index];
			}
			set
			{
				array[index] = value;
			}
		}

		public int Length => array.Length;

		public T[] Array => array;

		public ReorderableArray()
			: this(0)
		{
		}

		public ReorderableArray(int length)
		{
			array = new T[length];
		}

		public void Add(T value)
		{
			System.Array.Resize(ref array, array.Length + 1);
			array[^1] = value;
		}

		public void Remove(T value)
		{
			int num = System.Array.IndexOf(array, value);
			if (num >= 0)
			{
				int num2 = array.Length;
				T[] destinationArray = new T[num2 - 1];
				System.Array.Copy(array, destinationArray, num);
				System.Array.Copy(array, num + 1, destinationArray, num, num2 - (num + 1));
				array = destinationArray;
			}
		}
	}
}
