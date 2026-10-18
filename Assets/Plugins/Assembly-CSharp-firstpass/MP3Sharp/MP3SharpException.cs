using System;
using System.IO;
using System.Runtime.Serialization;
using MP3Sharp.Support;

namespace MP3Sharp
{
	[Serializable]
	public class MP3SharpException : Exception
	{
		public MP3SharpException()
		{
		}

		public MP3SharpException(string message)
			: base(message)
		{
		}

		public MP3SharpException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected MP3SharpException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public void PrintStackTrace()
		{
			SupportClass.WriteStackTrace(this, Console.Error);
		}

		public void PrintStackTrace(StreamWriter ps)
		{
			if (base.InnerException == null)
			{
				SupportClass.WriteStackTrace(this, ps);
			}
			else
			{
				SupportClass.WriteStackTrace(base.InnerException, Console.Error);
			}
		}
	}
}
