using MP3Sharp.Support;

namespace MP3Sharp.Decoding
{
	internal sealed class Crc16
	{
		private static readonly short Polynomial;

		private short m_Crc;

		static Crc16()
		{
			Polynomial = (short)SupportClass.Identity(32773L);
		}

		public Crc16()
		{
			m_Crc = (short)SupportClass.Identity(65535L);
		}

		public void add_bits(int bitstring, int length)
		{
			int num = 1 << length - 1;
			do
			{
				if (((m_Crc & 0x8000) == 0) ^ ((bitstring & num) == 0))
				{
					m_Crc <<= 1;
					m_Crc ^= Polynomial;
				}
				else
				{
					m_Crc <<= 1;
				}
			}
			while ((num = SupportClass.URShift(num, 1)) != 0);
		}

		public short Checksum()
		{
			short crc = m_Crc;
			m_Crc = (short)SupportClass.Identity(65535L);
			return crc;
		}
	}
}
