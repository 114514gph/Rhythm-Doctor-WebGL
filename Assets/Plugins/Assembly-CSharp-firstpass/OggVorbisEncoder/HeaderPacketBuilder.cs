using System;
using OggVorbisEncoder.Setup;

namespace OggVorbisEncoder
{
	public class HeaderPacketBuilder
	{
		private const string VorbisString = "vorbis";

		private const string VendorString = "OggVorbisEncoder";

		public OggPacket BuildInfoPacket(VorbisInfo info)
		{
			EncodeBuffer encodeBuffer = new EncodeBuffer();
			PackInfo(encodeBuffer, info);
			return new OggPacket(encodeBuffer.GetBytes(), endOfStream: false, 0, 0);
		}

		public OggPacket BuildCommentsPacket(Comments comments)
		{
			EncodeBuffer encodeBuffer = new EncodeBuffer();
			PackComment(encodeBuffer, comments);
			return new OggPacket(encodeBuffer.GetBytes(), endOfStream: false, 0, 1);
		}

		public OggPacket BuildBooksPacket(VorbisInfo info)
		{
			EncodeBuffer encodeBuffer = new EncodeBuffer(4096);
			PackBooks(encodeBuffer, info);
			return new OggPacket(encodeBuffer.GetBytes(), endOfStream: false, 0, 2);
		}

		private void PackBooks(EncodeBuffer buffer, VorbisInfo info)
		{
			CodecSetup codecSetup = info.CodecSetup;
			buffer.Write(5u, 8);
			buffer.WriteString("vorbis");
			buffer.Write((uint)(codecSetup.BookParams.Count - 1), 8);
			foreach (IStaticCodeBook bookParam in codecSetup.BookParams)
			{
				PackStaticBook(buffer, bookParam);
			}
			buffer.Write(0u, 6);
			buffer.Write(0u, 16);
			buffer.Write((uint)(codecSetup.FloorParams.Count - 1), 6);
			foreach (Floor floorParam in codecSetup.FloorParams)
			{
				buffer.Write(1u, 16);
				PackFloor(buffer, floorParam);
			}
			buffer.Write((uint)(codecSetup.ResidueParams.Count - 1), 6);
			foreach (ResidueEntry residueParam in codecSetup.ResidueParams)
			{
				buffer.Write((uint)residueParam.ResidueType, 16);
				PackResidue(buffer, residueParam);
			}
			buffer.Write((uint)(codecSetup.MapParams.Count - 1), 6);
			foreach (Mapping mapParam in codecSetup.MapParams)
			{
				buffer.Write(0u, 16);
				PackMapping(buffer, info, mapParam);
			}
			buffer.Write((uint)(codecSetup.ModeParams.Count - 1), 6);
			for (int i = 0; i < codecSetup.ModeParams.Count; i++)
			{
				PackModes(buffer, codecSetup, i);
			}
			buffer.Write(1u, 1);
		}

		private static void PackModes(EncodeBuffer buffer, CodecSetup codecSetup, int i)
		{
			buffer.Write((uint)codecSetup.ModeParams[i].BlockFlag, 1);
			buffer.Write((uint)codecSetup.ModeParams[i].WindowType, 16);
			buffer.Write((uint)codecSetup.ModeParams[i].TransformType, 16);
			buffer.Write((uint)codecSetup.ModeParams[i].Mapping, 8);
		}

		private static void PackResidue(EncodeBuffer buffer, ResidueEntry residue)
		{
			buffer.Write((uint)residue.Begin, 24);
			buffer.Write((uint)residue.End, 24);
			buffer.Write((uint)(residue.Grouping - 1), 24);
			buffer.Write((uint)(residue.Partitions - 1), 6);
			buffer.Write((uint)residue.GroupBook, 8);
			int num = 0;
			for (int i = 0; i < residue.Partitions; i++)
			{
				if (Encoding.Log(residue.SecondStages[i]) > 3)
				{
					buffer.Write((uint)residue.SecondStages[i], 3);
					buffer.Write(1u, 1);
					buffer.Write((uint)residue.SecondStages[i] >> 3, 5);
				}
				else
				{
					buffer.Write((uint)residue.SecondStages[i], 4);
				}
				num += Count(residue.SecondStages[i]);
			}
			for (int j = 0; j < num; j++)
			{
				buffer.Write((uint)residue.BookList[j], 8);
			}
		}

		private static int Count(int value)
		{
			int num = 0;
			while (value != 0)
			{
				num += value & 1;
				value >>= 1;
			}
			return num;
		}

		private static void PackFloor(EncodeBuffer buffer, Floor floor)
		{
			int num = 0;
			int num2 = floor.PostList[1];
			int num3 = -1;
			buffer.Write((uint)floor.PartitionClass.Length, 5);
			int[] partitionClass = floor.PartitionClass;
			foreach (int num4 in partitionClass)
			{
				buffer.Write((uint)num4, 4);
				if (num3 < num4)
				{
					num3 = num4;
				}
			}
			for (int j = 0; j < num3 + 1; j++)
			{
				buffer.Write((uint)(floor.ClassDimensions[j] - 1), 3);
				buffer.Write((uint)floor.ClassSubs[j], 2);
				if (floor.ClassSubs[j] != 0)
				{
					buffer.Write((uint)floor.ClassBook[j], 8);
				}
				for (int k = 0; k < 1 << floor.ClassSubs[j]; k++)
				{
					buffer.Write((uint)(floor.ClassSubBook[j][k] + 1), 8);
				}
			}
			buffer.Write((uint)(floor.Mult - 1), 2);
			buffer.Write((uint)Encoding.Log(num2 - 1), 4);
			int bits = Encoding.Log(num2 - 1);
			int l = 0;
			int m = 0;
			for (; l < floor.PartitionClass.Length; l++)
			{
				for (num += floor.ClassDimensions[floor.PartitionClass[l]]; m < num; m++)
				{
					buffer.Write((uint)floor.PostList[m + 2], bits);
				}
			}
		}

		private static void PackMapping(EncodeBuffer buffer, VorbisInfo info, Mapping mapping)
		{
			if (mapping.SubMaps > 1)
			{
				buffer.Write(1u, 1);
				buffer.Write((uint)(mapping.SubMaps - 1), 4);
			}
			else
			{
				buffer.Write(0u, 1);
			}
			if (mapping.CouplingSteps > 0)
			{
				buffer.Write(1u, 1);
				buffer.Write((uint)(mapping.CouplingSteps - 1), 8);
				int bits = Encoding.Log(info.Channels - 1);
				for (int i = 0; i < mapping.CouplingSteps; i++)
				{
					buffer.Write((uint)mapping.CouplingMag[i], bits);
					buffer.Write((uint)mapping.CouplingAng[i], bits);
				}
			}
			else
			{
				buffer.Write(0u, 1);
			}
			buffer.Write(0u, 2);
			if (mapping.SubMaps > 1)
			{
				for (int j = 0; j < info.Channels; j++)
				{
					buffer.Write((uint)mapping.ChannelMuxList[j], 4);
				}
			}
			for (int k = 0; k < mapping.SubMaps; k++)
			{
				buffer.Write(0u, 8);
				buffer.Write((uint)mapping.FloorSubMap[k], 8);
				buffer.Write((uint)mapping.ResidueSubMap[k], 8);
			}
		}

		private void PackStaticBook(EncodeBuffer buffer, IStaticCodeBook book)
		{
			bool flag = false;
			buffer.Write(5653314u, 24);
			buffer.Write((uint)book.Dimensions, 16);
			buffer.Write((uint)book.LengthList.Length, 24);
			int i;
			for (i = 1; i < book.LengthList.Length && book.LengthList[i - 1] != 0 && book.LengthList[i] >= book.LengthList[i - 1]; i++)
			{
			}
			if (i == book.LengthList.Length)
			{
				flag = true;
			}
			if (flag)
			{
				buffer.Write(1u, 1);
				buffer.Write((uint)(book.LengthList[0] - 1), 5);
				int num = 0;
				for (i = 1; i < book.LengthList.Length; i++)
				{
					byte b = book.LengthList[i];
					byte b2 = book.LengthList[i - 1];
					if (b > b2)
					{
						for (byte b3 = b2; b3 < b; b3++)
						{
							buffer.Write((uint)(i - num), Encoding.Log(book.LengthList.Length - num));
							num = i;
						}
					}
				}
				buffer.Write((uint)(i - num), Encoding.Log(book.LengthList.Length - num));
			}
			else
			{
				buffer.Write(0u, 1);
				for (i = 0; i < book.LengthList.Length && book.LengthList[i] != 0; i++)
				{
				}
				if (i == book.LengthList.Length)
				{
					buffer.Write(0u, 1);
					for (i = 0; i < book.LengthList.Length; i++)
					{
						buffer.Write((uint)(book.LengthList[i] - 1), 5);
					}
				}
				else
				{
					buffer.Write(1u, 1);
					for (i = 0; i < book.LengthList.Length; i++)
					{
						if (book.LengthList[i] == 0)
						{
							buffer.Write(0u, 1);
							continue;
						}
						buffer.Write(1u, 1);
						buffer.Write((uint)(book.LengthList[i] - 1), 5);
					}
				}
			}
			buffer.Write((uint)book.MapType, 4);
			if (book.MapType != CodeBookMapType.None)
			{
				if (book.MapType != CodeBookMapType.Implicit && book.MapType != CodeBookMapType.Listed)
				{
					throw new InvalidOperationException(string.Format("Unknown {0}: {1}", "CodeBookMapType", book.MapType));
				}
				if (book.QuantList == null)
				{
					throw new InvalidOperationException("QuantList cannot be null");
				}
				buffer.Write((uint)book.QuantMin, 32);
				buffer.Write((uint)book.QuantDelta, 32);
				buffer.Write((uint)(book.Quant - 1), 4);
				buffer.Write((uint)book.QuantSequenceP, 1);
				int num2 = 0;
				switch (book.MapType)
				{
				case CodeBookMapType.Implicit:
					num2 = book.GetQuantVals();
					break;
				case CodeBookMapType.Listed:
					num2 = book.LengthList.Length * book.Dimensions;
					break;
				}
				for (i = 0; i < num2; i++)
				{
					buffer.Write((uint)Math.Abs(book.QuantList[i]), book.Quant);
				}
			}
		}

		private static void PackComment(EncodeBuffer buffer, Comments vorbisComment)
		{
			buffer.Write(3u, 8);
			buffer.WriteString("vorbis");
			buffer.Write((uint)"OggVorbisEncoder".Length, 32);
			buffer.WriteString("OggVorbisEncoder");
			buffer.Write((uint)vorbisComment.UserComments.Count, 32);
			foreach (string userComment in vorbisComment.UserComments)
			{
				if (!string.IsNullOrEmpty(userComment))
				{
					buffer.Write((uint)userComment.Length, 32);
					buffer.WriteString(userComment);
				}
				else
				{
					buffer.Write(0u, 32);
				}
			}
			buffer.Write(1u, 1);
		}

		private static void PackInfo(EncodeBuffer buffer, VorbisInfo info)
		{
			CodecSetup codecSetup = info.CodecSetup;
			buffer.Write(1u, 8);
			buffer.WriteString("vorbis");
			buffer.Write(0u, 32);
			buffer.Write((uint)info.Channels, 8);
			buffer.Write((uint)info.SampleRate, 32);
			buffer.Write(0u, 32);
			buffer.Write((uint)info.BitRateNominal, 32);
			buffer.Write(0u, 32);
			buffer.Write((uint)Encoding.Log(codecSetup.BlockSizes[0] - 1), 4);
			buffer.Write((uint)Encoding.Log(codecSetup.BlockSizes[1] - 1), 4);
			buffer.Write(1u, 1);
		}
	}
}
