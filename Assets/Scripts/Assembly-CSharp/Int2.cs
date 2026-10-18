public struct Int2
{
	public int x;

	public int y;

	public int[] asArray
	{
		get
		{
			return new int[2] { x, y };
		}
		set
		{
			x = value[0];
			y = value[1];
		}
	}

	public Int2(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public override string ToString()
	{
		return $"x: {x} y: {y}";
	}
}
