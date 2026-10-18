namespace MP3Sharp.Decoding.Decoders.LayerIII
{
	internal class Layer3SideInfo
	{
		public ChannelData[] Channels;

		public int MainDataBegin;

		public int PrivateBits;

		public Layer3SideInfo()
		{
			Channels = new ChannelData[2];
			Channels[0] = new ChannelData();
			Channels[1] = new ChannelData();
		}
	}
}
