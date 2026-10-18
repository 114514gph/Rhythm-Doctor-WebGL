using System;
using System.Runtime.Serialization;

namespace MP3Sharp.Decoding
{
	[Serializable]
	public class BitstreamException : MP3SharpException
	{
		private int m_Errorcode;

		public virtual int ErrorCode => m_Errorcode;

		public BitstreamException(string message, Exception inner)
			: base(message, inner)
		{
			InitBlock();
		}

		public BitstreamException(int errorcode, Exception inner)
			: this(GetErrorString(errorcode), inner)
		{
			InitBlock();
			m_Errorcode = errorcode;
		}

		protected BitstreamException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			m_Errorcode = info.GetInt32("ErrorCode");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("ErrorCode", m_Errorcode);
			base.GetObjectData(info, context);
		}

		private void InitBlock()
		{
			m_Errorcode = BitstreamErrors.UNKNOWN_ERROR;
		}

		public static string GetErrorString(int errorcode)
		{
			return "Bitstream errorcode " + Convert.ToString(errorcode, 16);
		}
	}
}
