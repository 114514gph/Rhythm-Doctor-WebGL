using System;
using MP3Sharp.Decoding.Decoders;

namespace MP3Sharp.Decoding
{
	internal class Decoder
	{
		internal class Params : ICloneable
		{
			private OutputChannels m_OutputChannels;

			public virtual OutputChannels OutputChannels
			{
				get
				{
					return m_OutputChannels;
				}
				set
				{
					if (value == null)
					{
						throw new NullReferenceException("out");
					}
					m_OutputChannels = value;
				}
			}

			public virtual Equalizer InitialEqualizerSettings => null;

			public object Clone()
			{
				try
				{
					return MemberwiseClone();
				}
				catch (Exception ex)
				{
					throw new ApplicationException(this?.ToString() + ": " + ex);
				}
			}
		}

		private static readonly Params DEFAULT_PARAMS = new Params();

		private readonly Params params_Renamed;

		private Equalizer m_Equalizer;

		private SynthesisFilter m_LeftChannelFilter;

		private SynthesisFilter m_RightChannelFilter;

		private bool m_IsInitialized;

		private LayerIDecoder m_L1Decoder;

		private LayerIIDecoder m_L2Decoder;

		private LayerIIIDecoder m_L3Decoder;

		private ABuffer m_Output;

		private int m_OutputChannels;

		private int m_OutputFrequency;

		public static Params DefaultParams => (Params)DEFAULT_PARAMS.Clone();

		public virtual Equalizer Equalizer
		{
			set
			{
				if (value == null)
				{
					value = Equalizer.PASS_THRU_EQ;
				}
				m_Equalizer.FromEqualizer = value;
				float[] bandFactors = m_Equalizer.BandFactors;
				if (m_LeftChannelFilter != null)
				{
					m_LeftChannelFilter.EQ = bandFactors;
				}
				if (m_RightChannelFilter != null)
				{
					m_RightChannelFilter.EQ = bandFactors;
				}
			}
		}

		public virtual ABuffer OutputBuffer
		{
			set
			{
				m_Output = value;
			}
		}

		public virtual int OutputFrequency => m_OutputFrequency;

		public virtual int OutputChannels => m_OutputChannels;

		public virtual int OutputBlockSize => 2304;

		public Decoder()
			: this(null)
		{
			InitBlock();
		}

		public Decoder(Params params0)
		{
			InitBlock();
			if (params0 == null)
			{
				params0 = DEFAULT_PARAMS;
			}
			params_Renamed = params0;
			Equalizer initialEqualizerSettings = params_Renamed.InitialEqualizerSettings;
			if (initialEqualizerSettings != null)
			{
				m_Equalizer.FromEqualizer = initialEqualizerSettings;
			}
		}

		private void InitBlock()
		{
			m_Equalizer = new Equalizer();
		}

		public virtual ABuffer DecodeFrame(Header header, Bitstream stream)
		{
			if (!m_IsInitialized)
			{
				Initialize(header);
			}
			int layer = header.layer();
			m_Output.ClearBuffer();
			RetrieveDecoder(header, stream, layer).DecodeFrame();
			m_Output.WriteBuffer(1);
			return m_Output;
		}

		protected internal virtual DecoderException NewDecoderException(int errorcode)
		{
			return new DecoderException(errorcode, null);
		}

		protected internal virtual DecoderException NewDecoderException(int errorcode, Exception throwable)
		{
			return new DecoderException(errorcode, throwable);
		}

		protected internal virtual IFrameDecoder RetrieveDecoder(Header header, Bitstream stream, int layer)
		{
			IFrameDecoder frameDecoder = null;
			switch (layer)
			{
			case 3:
				if (m_L3Decoder == null)
				{
					m_L3Decoder = new LayerIIIDecoder(stream, header, m_LeftChannelFilter, m_RightChannelFilter, m_Output, 0);
				}
				frameDecoder = m_L3Decoder;
				break;
			case 2:
				if (m_L2Decoder == null)
				{
					m_L2Decoder = new LayerIIDecoder();
					m_L2Decoder.Create(stream, header, m_LeftChannelFilter, m_RightChannelFilter, m_Output, 0);
				}
				frameDecoder = m_L2Decoder;
				break;
			case 1:
				if (m_L1Decoder == null)
				{
					m_L1Decoder = new LayerIDecoder();
					m_L1Decoder.Create(stream, header, m_LeftChannelFilter, m_RightChannelFilter, m_Output, 0);
				}
				frameDecoder = m_L1Decoder;
				break;
			}
			if (frameDecoder == null)
			{
				throw NewDecoderException(DecoderErrors.UNSUPPORTED_LAYER, null);
			}
			return frameDecoder;
		}

		private void Initialize(Header header)
		{
			float factor = 32700f;
			int num = ((header.mode() == 3) ? 1 : 2);
			if (m_Output == null)
			{
				m_Output = new SampleBuffer(header.frequency(), num);
			}
			float[] bandFactors = m_Equalizer.BandFactors;
			m_LeftChannelFilter = new SynthesisFilter(0, factor, bandFactors);
			if (num == 2)
			{
				m_RightChannelFilter = new SynthesisFilter(1, factor, bandFactors);
			}
			m_OutputChannels = num;
			m_OutputFrequency = header.frequency();
			m_IsInitialized = true;
		}
	}
}
