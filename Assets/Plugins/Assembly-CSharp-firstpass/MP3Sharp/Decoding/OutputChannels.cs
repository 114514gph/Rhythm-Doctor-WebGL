using System;

namespace MP3Sharp.Decoding
{
	internal class OutputChannels
	{
		public static int BOTH_CHANNELS = 0;

		public static int LEFT_CHANNEL = 1;

		public static int RIGHT_CHANNEL = 2;

		public static int DOWNMIX_CHANNELS = 3;

		public static readonly OutputChannels LEFT = new OutputChannels(LEFT_CHANNEL);

		public static readonly OutputChannels RIGHT = new OutputChannels(RIGHT_CHANNEL);

		public static readonly OutputChannels BOTH = new OutputChannels(BOTH_CHANNELS);

		public static readonly OutputChannels DOWNMIX = new OutputChannels(DOWNMIX_CHANNELS);

		private readonly int outputChannels;

		public virtual int ChannelsOutputCode => outputChannels;

		public virtual int ChannelCount
		{
			get
			{
				if (outputChannels != BOTH_CHANNELS)
				{
					return 1;
				}
				return 2;
			}
		}

		private OutputChannels(int channels)
		{
			outputChannels = channels;
			if (channels < 0 || channels > 3)
			{
				throw new ArgumentException("channels");
			}
		}

		public static OutputChannels fromInt(int code)
		{
			return code switch
			{
				1 => LEFT, 
				2 => RIGHT, 
				0 => BOTH, 
				3 => DOWNMIX, 
				_ => throw new ArgumentException("Invalid channel code: " + code), 
			};
		}

		public override bool Equals(object o)
		{
			bool result = false;
			if (o is OutputChannels)
			{
				result = ((OutputChannels)o).outputChannels == outputChannels;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return outputChannels;
		}
	}
}
