using System;
using System.IO;
using UnityEngine;

namespace OggVorbisEncoder.Example
{
	public class Encoder
	{
		public enum PCMSample
		{
			EightBit = 1,
			SixteenBit = 2
		}

		private static int WriteBufferSize = 512;

		private static readonly int[] SampleRates = new int[6] { 8000, 11025, 16000, 22050, 32000, 44100 };

		[STAThread]
		private static void Main()
		{
			ConvertPCMFile();
		}

		private static void ConvertPCMFile()
		{
			byte[] pCMSamples = File.ReadAllBytes("unencoded.raw");
			byte[] bytes = ConvertRawPCMFile(44100, 2, pCMSamples, PCMSample.SixteenBit, 44100, 2);
			File.WriteAllBytes("encoded.ogg", bytes);
		}

		private static void GenerateTestFiles()
		{
			int num = 1000;
			for (int i = 1; i <= 2; i++)
			{
				int[] sampleRates = SampleRates;
				foreach (int num2 in sampleRates)
				{
					try
					{
						byte[] bytes = GenerateSineWaveFile(num2, i, num, 10f);
						File.WriteAllBytes($"C:/Temp/Sine_{num2}_{num}hz_{i}.ogg", bytes);
						Console.WriteLine($"{num2} samples/s, {i} channels succeeded.");
					}
					catch (Exception ex)
					{
						Console.WriteLine($"{num2} samples/s, {i} channels failed. {ex.Message}");
					}
				}
			}
			Console.WriteLine("Done!");
			Console.ReadLine();
		}

		public static byte[] ConvertRawPCMFile(int OutputSampleRate, int OutputChannels, byte[] PCMSamples, PCMSample PCMSampleSize, int PCMSampleRate, int PCMChannels)
		{
			int num = (int)((float)(PCMSamples.Length / (int)PCMSampleSize / PCMChannels) / (float)PCMSampleRate * (float)OutputSampleRate);
			num = num / WriteBufferSize * WriteBufferSize;
			float[][] array = new float[OutputChannels][];
			for (int i = 0; i < OutputChannels; i++)
			{
				array[i] = new float[num];
			}
			for (int j = 0; j < num; j++)
			{
				float num2 = 0f;
				for (int k = 0; k < OutputChannels; k++)
				{
					int num3 = j * PCMChannels * (int)PCMSampleSize;
					if (k < PCMChannels)
					{
						num3 += k * (int)PCMSampleSize;
					}
					switch (PCMSampleSize)
					{
					case PCMSample.EightBit:
						num2 = ByteToSample(PCMSamples[num3]);
						break;
					case PCMSample.SixteenBit:
						num2 = ShortToSample((short)((PCMSamples[num3 + 1] << 8) | PCMSamples[num3]));
						break;
					}
					array[k][j] = num2;
				}
			}
			return GenerateFile(array, OutputSampleRate, OutputChannels);
		}

		private static byte[] GenerateSineWaveFile(int OutputSampleRate, int OutputChannels, int Frequency, float DurationSeconds, float Volume = 0.2f)
		{
			float[][] array = new float[OutputChannels][];
			int num = (int)(DurationSeconds * (float)OutputSampleRate);
			num = num / WriteBufferSize * WriteBufferSize;
			for (int i = 0; i < OutputChannels; i++)
			{
				array[i] = new float[num];
			}
			for (int j = 0; j < num; j++)
			{
				float num2 = Volume * SineSample(j, Frequency, OutputSampleRate);
				for (int k = 0; k < OutputChannels; k++)
				{
					array[k][j] = num2;
				}
			}
			return GenerateFile(array, OutputSampleRate, OutputChannels);
		}

		private static byte[] GenerateFile(float[][] FloatSamples, int SampleRate, int Channels)
		{
			using MemoryStream memoryStream = new MemoryStream();
			VorbisInfo info = VorbisInfo.InitVariableBitRate(Channels, SampleRate, 0.5f);
			OggStream oggStream = new OggStream(new System.Random().Next());
			HeaderPacketBuilder headerPacketBuilder = new HeaderPacketBuilder();
			Comments comments = new Comments();
			comments.AddTag("ARTIST", "TEST");
			OggPacket packet = headerPacketBuilder.BuildInfoPacket(info);
			OggPacket packet2 = headerPacketBuilder.BuildCommentsPacket(comments);
			OggPacket packet3 = headerPacketBuilder.BuildBooksPacket(info);
			oggStream.PacketIn(packet);
			oggStream.PacketIn(packet2);
			oggStream.PacketIn(packet3);
			FlushPages(oggStream, memoryStream, Force: true);
			ProcessingState processingState = ProcessingState.Create(info);
			for (int i = 0; i <= FloatSamples[0].Length; i += WriteBufferSize)
			{
				if (i == FloatSamples[0].Length)
				{
					processingState.WriteEndOfStream();
				}
				else
				{
					processingState.WriteData(FloatSamples, WriteBufferSize, i);
				}
				OggPacket packet4;
				while (!oggStream.Finished && processingState.PacketOut(out packet4))
				{
					oggStream.PacketIn(packet4);
					FlushPages(oggStream, memoryStream, Force: false);
				}
			}
			FlushPages(oggStream, memoryStream, Force: true);
			return memoryStream.ToArray();
		}

		private static void FlushPages(OggStream oggStream, Stream Output, bool Force)
		{
			OggPage page;
			while (oggStream.PageOut(out page, Force))
			{
				Output.Write(page.Header, 0, page.Header.Length);
				Output.Write(page.Body, 0, page.Body.Length);
			}
		}

		private static float SineSample(int sample, float frequency, int samplerate)
		{
			return Mathf.Sin((float)sample / (float)samplerate * MathF.PI * 2f * frequency);
		}

		private static float ByteToSample(short pcmValue)
		{
			return (float)pcmValue / 128f;
		}

		private static float ShortToSample(short pcmValue)
		{
			return (float)pcmValue / 32768f;
		}

		private static void StripWavHeader(BinaryReader stdin)
		{
			byte[] array = new byte[6];
			for (int i = 0; i < 30; i++)
			{
				if (stdin.Read(array, 0, 2) <= 0)
				{
					break;
				}
				if (array[0] == 100 && array[1] == 97)
				{
					stdin.Read(array, 0, 6);
					break;
				}
			}
		}
	}
}
