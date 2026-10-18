using System;
using System.Diagnostics.CodeAnalysis;
using RDLevelEditor;
using UnityEngine;

public readonly struct FloatExpression
{
	private readonly string exp;

	private readonly float num;

	public readonly bool isExpression;

	public bool isLiteral => !isExpression;

	public FloatExpression(float num)
	{
		this.num = num;
		exp = "";
		isExpression = false;
	}

	private FloatExpression(string exp, float num, bool isExpression)
	{
		this.exp = exp;
		this.num = num;
		this.isExpression = isExpression;
	}

	private FloatExpression(string exp)
	{
		this.exp = exp;
		num = 0f;
		isExpression = true;
	}

	public static implicit operator bool(FloatExpression e)
	{
		if (!e.isLiteral)
		{
			return !e.exp.IsNullOrEmpty();
		}
		return true;
	}

	public static FloatExpression FromString(string input)
	{
		if (input.IsNullOrEmpty())
		{
			return EmptyInput();
		}
		if (!float.TryParse(input, out var result))
		{
			return new FloatExpression(input);
		}
		return new FloatExpression(result);
	}

	public static FloatExpression FromJsonObject([AllowNull] object input)
	{
		if (input == null)
		{
			return EmptyInput();
		}
		try
		{
			return new FloatExpression(Convert.ToSingle(input));
		}
		catch (FormatException)
		{
			return new FloatExpression(input.ToString());
		}
	}

	public static FloatExpression EmptyInput()
	{
		return new FloatExpression("");
	}

	public override string ToString()
	{
		if (!isLiteral)
		{
			return exp;
		}
		float num = this.num;
		return num.ToString();
	}

	public string ToJsonValue()
	{
		if (isLiteral)
		{
			float num = this.num;
			return num.ToString();
		}
		if (exp.IsNullOrEmpty())
		{
			return "null";
		}
		string text = RDEditorUtils.EscapeTextForJSON(exp);
		return "\"" + text + "\"";
	}

	public string ToStringDebug()
	{
		return $"exp={exp}, num={num}, isString={isExpression}";
	}

	public FloatExpression Clamp(float min, float max)
	{
		if (!isLiteral)
		{
			return this;
		}
		return new FloatExpression(Mathf.Clamp(num, min, max));
	}

	public float? Unbox(LevelBase levelBase)
	{
		if (isLiteral)
		{
			return num;
		}
		if (exp == string.Empty)
		{
			return null;
		}
		string s;
		try
		{
			s = levelBase.EvaluateCurlyBracketsInString(exp);
		}
		catch
		{
			return null;
		}
		if (!float.TryParse(s, out var result))
		{
			return null;
		}
		return result;
	}

	public static FloatExpression operator *(FloatExpression self, float that)
	{
		return new FloatExpression(self.exp, self.num * that, self.isExpression);
	}

	public static FloatExpression operator /(FloatExpression self, float that)
	{
		return new FloatExpression(self.exp, self.num / that, self.isExpression);
	}
}
