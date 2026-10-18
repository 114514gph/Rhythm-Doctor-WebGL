using System;

namespace OggVorbisEncoder
{
	public class OggStream
	{
		private const int LacingSize = 1024;

		private static readonly uint[] Checksum = new uint[256]
		{
			0u, 79764919u, 159529838u, 222504665u, 319059676u, 398814059u, 445009330u, 507990021u, 638119352u, 583659535u,
			797628118u, 726387553u, 890018660u, 835552979u, 1015980042u, 944750013u, 1276238704u, 1221641927u, 1167319070u, 1095957929u,
			1595256236u, 1540665371u, 1452775106u, 1381403509u, 1780037320u, 1859660671u, 1671105958u, 1733955601u, 2031960084u, 2111593891u,
			1889500026u, 1952343757u, 2552477408u, 2632100695u, 2443283854u, 2506133561u, 2334638140u, 2414271883u, 2191915858u, 2254759653u,
			3190512472u, 3135915759u, 3081330742u, 3009969537u, 2905550212u, 2850959411u, 2762807018u, 2691435357u, 3560074640u, 3505614887u,
			3719321342u, 3648080713u, 3342211916u, 3287746299u, 3467911202u, 3396681109u, 4063920168u, 4143685023u, 4223187782u, 4286162673u,
			3779000052u, 3858754371u, 3904687514u, 3967668269u, 881225847u, 809987520u, 1023691545u, 969234094u, 662832811u, 591600412u,
			771767749u, 717299826u, 311336399u, 374308984u, 453813921u, 533576470u, 25881363u, 88864420u, 134795389u, 214552010u,
			2023205639u, 2086057648u, 1897238633u, 1976864222u, 1804852699u, 1867694188u, 1645340341u, 1724971778u, 1587496639u, 1516133128u,
			1461550545u, 1406951526u, 1302016099u, 1230646740u, 1142491917u, 1087903418u, 2896545431u, 2825181984u, 2770861561u, 2716262478u,
			3215044683u, 3143675388u, 3055782693u, 3001194130u, 2326604591u, 2389456536u, 2200899649u, 2280525302u, 2578013683u, 2640855108u,
			2418763421u, 2498394922u, 3769900519u, 3832873040u, 3912640137u, 3992402750u, 4088425275u, 4151408268u, 4197601365u, 4277358050u,
			3334271071u, 3263032808u, 3476998961u, 3422541446u, 3585640067u, 3514407732u, 3694837229u, 3640369242u, 1762451694u, 1842216281u,
			1619975040u, 1682949687u, 2047383090u, 2127137669u, 1938468188u, 2001449195u, 1325665622u, 1271206113u, 1183200824u, 1111960463u,
			1543535498u, 1489069629u, 1434599652u, 1363369299u, 622672798u, 568075817u, 748617968u, 677256519u, 907627842u, 853037301u,
			1067152940u, 995781531u, 51762726u, 131386257u, 177728840u, 240578815u, 269590778u, 349224269u, 429104020u, 491947555u,
			4046411278u, 4126034873u, 4172115296u, 4234965207u, 3794477266u, 3874110821u, 3953728444u, 4016571915u, 3609705398u, 3555108353u,
			3735388376u, 3664026991u, 3290680682u, 3236090077u, 3449943556u, 3378572211u, 3174993278u, 3120533705u, 3032266256u, 2961025959u,
			2923101090u, 2868635157u, 2813903052u, 2742672763u, 2604032198u, 2683796849u, 2461293480u, 2524268063u, 2284983834u, 2364738477u,
			2175806836u, 2238787779u, 1569362073u, 1498123566u, 1409854455u, 1355396672u, 1317987909u, 1246755826u, 1192025387u, 1137557660u,
			2072149281u, 2135122070u, 1912620623u, 1992383480u, 1753615357u, 1816598090u, 1627664531u, 1707420964u, 295390185u, 358241886u,
			404320391u, 483945776u, 43990325u, 106832002u, 186451547u, 266083308u, 932423249u, 861060070u, 1041341759u, 986742920u,
			613929101u, 542559546u, 756411363u, 701822548u, 3316196985u, 3244833742u, 3425377559u, 3370778784u, 3601682597u, 3530312978u,
			3744426955u, 3689838204u, 3819031489u, 3881883254u, 3928223919u, 4007849240u, 4037393693u, 4100235434u, 4180117107u, 4259748804u,
			2310601993u, 2373574846u, 2151335527u, 2231098320u, 2596047829u, 2659030626u, 2470359227u, 2550115596u, 2947551409u, 2876312838u,
			2788305887u, 2733848168u, 3165939309u, 3094707162u, 3040238851u, 2985771188u
		};

		private readonly int _serialNumber;

		private byte[] _bodyData = new byte[16384];

		private int _bodyFill;

		private int _bodyReturned;

		private long _granulePosition;

		private long[] _granuleValues = new long[1024];

		private int _lacingFill;

		private int[] _lacingValues = new int[1024];

		private int _pageNumber;

		private bool _writesHaveStarted;

		public bool Finished { get; private set; }

		public OggStream(int serialNumber)
		{
			_serialNumber = serialNumber;
		}

		public void PacketIn(OggPacket packet)
		{
			if (packet != null)
			{
				ClearReturnedBody();
				int num = packet.PacketData.Length;
				int num2 = (int)((float)num / 255f + 1f);
				ExpandBody(num);
				ExpandLacing(num2);
				Array.Copy(packet.PacketData, 0, _bodyData, _bodyFill, num);
				_bodyFill += num;
				int i;
				for (i = 0; i < num2 - 1; i++)
				{
					_lacingValues[_lacingFill + i] = 255;
					_granuleValues[_lacingFill + i] = _granulePosition;
				}
				_lacingValues[_lacingFill + i] = (int)((float)num % 255f);
				_granulePosition = (_granuleValues[_lacingFill + i] = packet.GranulePosition);
				_lacingValues[_lacingFill] |= 256;
				_lacingFill += num2;
				if (packet.EndOfStream)
				{
					Finished = true;
				}
			}
		}

		private void ClearReturnedBody()
		{
			if (_bodyReturned != 0)
			{
				_bodyFill -= _bodyReturned;
				if (_bodyFill != 0)
				{
					Array.Copy(_bodyData, _bodyReturned, _bodyData, 0, _bodyFill);
				}
				_bodyReturned = 0;
			}
		}

		private void ExpandLacing(int needed)
		{
			if (_lacingFill + needed >= _lacingValues.Length)
			{
				if (_lacingValues.Length > int.MaxValue - needed)
				{
					throw new InvalidOperationException("Maximum buffer exceeded");
				}
				int num = _lacingValues.Length + needed;
				if (num < 2147483615)
				{
					num += 32;
				}
				int[] array = _lacingValues;
				Array.Resize(ref array, num);
				_lacingValues = array;
				long[] array2 = _granuleValues;
				Array.Resize(ref array2, num);
				_granuleValues = array2;
			}
		}

		private void ExpandBody(int needed)
		{
			if (_bodyFill + needed >= _bodyData.Length)
			{
				if (_bodyData.Length > int.MaxValue - needed)
				{
					throw new InvalidOperationException("Maximum buffer exceeded");
				}
				int num = _bodyData.Length + needed;
				if (num < 2147482623)
				{
					num += 1024;
				}
				byte[] array = _bodyData;
				Array.Resize(ref array, num);
				_bodyData = array;
			}
		}

		public bool PageOut(out OggPage page, bool force)
		{
			int num = 0;
			long num2 = -1L;
			page = null;
			int num3 = ((_lacingFill > 255) ? 255 : _lacingFill);
			if (num3 == 0)
			{
				return false;
			}
			int i;
			if (!_writesHaveStarted)
			{
				num2 = 0L;
				for (i = 0; i < num3; i++)
				{
					if ((_lacingValues[i] & 0xFF) < 255)
					{
						i++;
						break;
					}
				}
			}
			else
			{
				int num4 = 0;
				int num5 = 0;
				for (i = 0; i < num3; i++)
				{
					if (num > 4096 && num5 >= 4)
					{
						force = true;
						break;
					}
					num += _lacingValues[i] & 0xFF;
					if ((_lacingValues[i] & 0xFF) < 255)
					{
						num2 = _granuleValues[i];
						num5 = ++num4;
					}
					else
					{
						num5 = 0;
					}
				}
				if (i == 255)
				{
					force = true;
				}
			}
			if (!force)
			{
				return false;
			}
			byte[] array = new byte[i + 27];
			for (int j = 0; j < "OggS".Length; j++)
			{
				array[j] = (byte)"OggS"[j];
			}
			array[4] = 0;
			array[5] = 0;
			if ((_lacingValues[0] & 0x100) == 0)
			{
				array[5] |= 1;
			}
			if (!_writesHaveStarted)
			{
				array[5] |= 2;
			}
			if (Finished && _lacingFill == i)
			{
				array[5] |= 4;
			}
			_writesHaveStarted = true;
			for (int k = 6; k < 14; k++)
			{
				array[k] = (byte)(num2 & 0xFF);
				num2 >>= 8;
			}
			int num6 = _serialNumber;
			for (int l = 14; l < 18; l++)
			{
				array[l] = (byte)(num6 & 0xFF);
				num6 >>= 8;
			}
			if (_pageNumber == -1)
			{
				_pageNumber = 0;
			}
			int num7 = _pageNumber++;
			for (int m = 18; m < 22; m++)
			{
				array[m] = (byte)(num7 & 0xFF);
				num7 >>= 8;
			}
			array[22] = 0;
			array[23] = 0;
			array[24] = 0;
			array[25] = 0;
			int num8 = 0;
			array[26] = (byte)(i & 0xFF);
			for (int n = 0; n < i; n++)
			{
				num8 += (array[n + 27] = (byte)(_lacingValues[n] & 0xFF));
			}
			byte[] array2 = new byte[num8];
			Array.Copy(_bodyData, _bodyReturned, array2, 0, num8);
			uint num9 = 0u;
			byte[] array3 = array;
			foreach (byte b in array3)
			{
				num9 = (num9 << 8) ^ Checksum[((num9 >> 24) & 0xFF) ^ b];
			}
			array3 = array2;
			foreach (byte b2 in array3)
			{
				num9 = (num9 << 8) ^ Checksum[((num9 >> 24) & 0xFF) ^ b2];
			}
			array[22] = (byte)(num9 & 0xFF);
			array[23] = (byte)((num9 >> 8) & 0xFF);
			array[24] = (byte)((num9 >> 16) & 0xFF);
			array[25] = (byte)((num9 >> 24) & 0xFF);
			page = new OggPage(array, array2);
			_lacingFill -= i;
			Array.Copy(_lacingValues, i, _lacingValues, 0, _lacingFill);
			Array.Copy(_granuleValues, i, _granuleValues, 0, _lacingFill);
			_bodyReturned += num8;
			return true;
		}
	}
}
