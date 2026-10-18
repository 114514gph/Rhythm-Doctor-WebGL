using System;

namespace MP3Sharp.Decoding
{
	[Serializable]
	internal class CircularByteBuffer
	{
		private byte[] m_DataArray;

		private int m_Index;

		private int m_Length;

		private int m_NumValid;

		public int BufferSize
		{
			get
			{
				return m_Length;
			}
			set
			{
				byte[] array = new byte[value];
				int num = ((m_Length > value) ? value : m_Length);
				for (int i = 0; i < num; i++)
				{
					array[i] = InternalGet(i - m_Length + 1);
				}
				m_DataArray = array;
				m_Index = num - 1;
				m_Length = value;
			}
		}

		public byte this[int index]
		{
			get
			{
				return InternalGet(-1 - index);
			}
			set
			{
				InternalSet(-1 - index, value);
			}
		}

		public int NumValid
		{
			get
			{
				return m_NumValid;
			}
			set
			{
				if (value > m_NumValid)
				{
					throw new Exception("Can't set NumValid to " + value + " which is greater than the current numValid value of " + m_NumValid);
				}
				m_NumValid = value;
			}
		}

		public CircularByteBuffer(int size)
		{
			m_DataArray = new byte[size];
			m_Length = size;
		}

		public CircularByteBuffer(CircularByteBuffer cdb)
		{
			lock (cdb)
			{
				m_Length = cdb.m_Length;
				m_NumValid = cdb.m_NumValid;
				m_Index = cdb.m_Index;
				m_DataArray = new byte[m_Length];
				for (int i = 0; i < m_Length; i++)
				{
					m_DataArray[i] = cdb.m_DataArray[i];
				}
			}
		}

		public CircularByteBuffer Copy()
		{
			return new CircularByteBuffer(this);
		}

		public void Reset()
		{
			m_Index = 0;
			m_NumValid = 0;
		}

		public byte Push(byte newValue)
		{
			lock (this)
			{
				byte result = InternalGet(m_Length);
				m_DataArray[m_Index] = newValue;
				m_NumValid++;
				if (m_NumValid > m_Length)
				{
					m_NumValid = m_Length;
				}
				m_Index++;
				m_Index %= m_Length;
				return result;
			}
		}

		public byte Pop()
		{
			lock (this)
			{
				if (m_NumValid == 0)
				{
					throw new Exception("Can't pop off an empty CircularByteBuffer");
				}
				m_NumValid--;
				return this[m_NumValid];
			}
		}

		public byte Peek()
		{
			lock (this)
			{
				return InternalGet(m_Length);
			}
		}

		private byte InternalGet(int offset)
		{
			int i;
			for (i = m_Index + offset; i >= m_Length; i -= m_Length)
			{
			}
			for (; i < 0; i += m_Length)
			{
			}
			return m_DataArray[i];
		}

		private void InternalSet(int offset, byte valueToSet)
		{
			int i;
			for (i = m_Index + offset; i > m_Length; i -= m_Length)
			{
			}
			for (; i < 0; i += m_Length)
			{
			}
			m_DataArray[i] = valueToSet;
		}

		public byte[] GetRange(int str, int stp)
		{
			byte[] array = new byte[str - stp + 1];
			int num = str;
			int num2 = 0;
			while (num >= stp)
			{
				array[num2] = this[num];
				num--;
				num2++;
			}
			return array;
		}

		public override string ToString()
		{
			string text = "";
			for (int i = 0; i < m_DataArray.Length; i++)
			{
				text = text + m_DataArray[i] + " ";
			}
			return text + "\n index = " + m_Index + " numValid = " + NumValid;
		}
	}
}
