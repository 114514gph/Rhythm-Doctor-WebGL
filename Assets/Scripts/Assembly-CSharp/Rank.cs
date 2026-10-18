public struct Rank
{
	public const int NeverSelected = -3;

	public const int NotAvailable = -2;

	public const int NotFinished = -1;

	public const int Fminus = -10;

	public const int F = 0;

	public const int Fplus = 10;

	public const int Dminus = -11;

	public const int D = 1;

	public const int Dplus = 11;

	public const int Cminus = -12;

	public const int C = 2;

	public const int Cplus = 12;

	public const int Bminus = -13;

	public const int B = 3;

	public const int Bplus = 13;

	public const int Aminus = -14;

	public const int A = 4;

	public const int Aplus = 14;

	public const int Sminus = -15;

	public const int S = 5;

	public const int Splus = 15;

	public const int ToPass = 3;

	public const int ForMedal = 4;

	public const int BossClear = 4;

	public const int BossNoCheckpoints = 14;

	public const int BossPerfect = 5;

	private int internalValue;

	public bool passed => (int)ToNormal() >= 3;

	public bool perfected => (int)ToNormal() == 5;

	public bool unlocksNightShift => (int)ToNormal() >= 4;

	public bool isNormalRank
	{
		get
		{
			if ((int)ToNormal() >= 0)
			{
				return (int)ToNormal() <= 5;
			}
			return false;
		}
	}

	public bool noCheckpoints => internalValue == 14;

	public override bool Equals(object obj)
	{
		return ((Rank)obj).internalValue.Equals(internalValue);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static implicit operator Rank(int otherType)
	{
		return new Rank
		{
			internalValue = otherType
		};
	}

	public static implicit operator int(Rank otherType)
	{
		return otherType.internalValue;
	}

	public static Rank FromString(string _string, int fallback = 0)
	{
		return _string switch
		{
			"F" => 0, 
			"D" => 1, 
			"C" => 2, 
			"B" => 3, 
			"A" => 4, 
			"S" => 5, 
			"F+" => 10, 
			"D+" => 11, 
			"C+" => 12, 
			"B+" => 13, 
			"A+" => 14, 
			"S+" => 15, 
			"F-" => -10, 
			"D-" => -11, 
			"C-" => -12, 
			"B-" => -13, 
			"A-" => -14, 
			"S-" => -15, 
			"NotAvailable" => -2, 
			"NotFinished" => -1, 
			"NeverSelected" => -3, 
			_ => fallback, 
		};
	}

	public new string ToString()
	{
		return internalValue switch
		{
			5 => "S", 
			4 => "A", 
			3 => "B", 
			2 => "C", 
			1 => "D", 
			0 => "F", 
			15 => "S+", 
			14 => "A+", 
			13 => "B+", 
			12 => "C+", 
			11 => "D+", 
			10 => "F+", 
			-15 => "S-", 
			-14 => "A-", 
			-13 => "B-", 
			-12 => "C-", 
			-11 => "D-", 
			-10 => "F-", 
			-2 => "NotAvailable", 
			-1 => "NotFinished", 
			-3 => "NeverSelected", 
			_ => "F", 
		};
	}

	public int ComparativeInt()
	{
		switch (internalValue)
		{
		case -15:
		case 5:
		case 15:
			return 5;
		case -14:
		case 4:
		case 14:
			return 4;
		case -13:
		case 3:
		case 13:
			return 3;
		case -12:
		case 2:
		case 12:
			return 2;
		case -11:
		case 1:
		case 11:
			return 1;
		case -10:
		case 0:
		case 10:
			return 0;
		case -3:
		case -2:
		case -1:
			return -1;
		default:
			return 0;
		}
	}

	public Rank ToNormal()
	{
		int num;
		switch (internalValue)
		{
		case -15:
		case 15:
			num = 5;
			break;
		case -14:
		case 14:
			num = 4;
			break;
		case -13:
		case 13:
			num = 3;
			break;
		case -12:
		case 12:
			num = 2;
			break;
		case -11:
		case 11:
			num = 1;
			break;
		case -10:
		case 10:
			num = 0;
			break;
		default:
			num = internalValue;
			break;
		}
		return num;
	}

	public Rank ToPlus()
	{
		return internalValue switch
		{
			5 => 15, 
			4 => 14, 
			3 => 13, 
			2 => 12, 
			1 => 11, 
			0 => 10, 
			_ => internalValue, 
		};
	}

	public Rank ToMinus()
	{
		return internalValue switch
		{
			5 => -15, 
			4 => -14, 
			3 => -13, 
			2 => -12, 
			1 => -11, 
			0 => -10, 
			_ => internalValue, 
		};
	}
}
