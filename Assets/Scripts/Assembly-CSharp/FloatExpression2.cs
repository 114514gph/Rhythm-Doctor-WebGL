public readonly struct FloatExpression2
{
	public readonly FloatExpression x;

	public readonly FloatExpression y;

	public FloatExpression2(FloatExpression x, FloatExpression y)
	{
		this.x = x;
		this.y = y;
	}

	public FloatExpression2(float x, float y)
	{
		this.x = new FloatExpression(x);
		this.y = new FloatExpression(y);
	}

	public static implicit operator bool(FloatExpression2 e)
	{
		if (!e.x)
		{
			return e.y;
		}
		return true;
	}

	public (float? x, float? y) Unbox(LevelBase levelBase)
	{
		return (x: x.Unbox(levelBase), y: y.Unbox(levelBase));
	}

	public Float2 UnboxToFloat2(LevelBase levelBase)
	{
		float? num = x.Unbox(levelBase);
		float? num2 = y.Unbox(levelBase);
		Float2 result = default;
		result.xUsed = num.HasValue;
		result.yUsed = num2.HasValue;
		result.x = (result.xUsed ? num.Value : 0f);
		result.y = (result.yUsed ? num2.Value : 0f);
		return result;
	}

	public override string ToString()
	{
		if ((bool)x && (bool)y)
		{
			return $"X: {x}, Y: {y}";
		}
		if ((bool)x)
		{
			return $"X: {x}";
		}
		if ((bool)y)
		{
			return $"Y: {y}";
		}
		return string.Empty;
	}

	public string ToStringDebug()
	{
		return "x: " + x.ToStringDebug() + ", y: " + y.ToStringDebug();
	}

	public static FloatExpression2 operator *(FloatExpression2 self, float that)
	{
		return new FloatExpression2(self.x * that, self.y * that);
	}

	public static FloatExpression2 operator /(FloatExpression2 self, float that)
	{
		return new FloatExpression2(self.x / that, self.y / that);
	}
}
