using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalCredits : MonoBehaviour
{
	private class PreordersRow
	{
		public StringBuilder sb = new StringBuilder();

		public float width;

		public int names;
	}

	public string asset;

	public Text bottomText;

	public Text topText;

	public Color[] colors;

	public void Populate()
	{
		string[,] grid = new CSVReader(Resources.Load<TextAsset>(asset)).grid;
		string[] array = new string[colors.Length];
		for (int i = 0; i < colors.Length; i++)
		{
			array[i] = "<color=#" + ColorUtility.ToHtmlStringRGB(colors[i]) + ">";
		}
		string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_:().,!'\" ";
		Dictionary<char, float> charWidthLUT = new Dictionary<char, float>();
		TextGenerator cachedTextGenerator = topText.cachedTextGenerator;
		cachedTextGenerator.Populate(text, topText.GetGenerationSettings(topText.rectTransform.rect.size));
		for (int j = 0; j < text.Length; j++)
		{
			float charWidth = cachedTextGenerator.characters[j].charWidth;
			charWidthLUT.Add(text[j], charWidth);
		}
		float aproxWidth = charWidthLUT['a'];
		PreordersRow[] array2 = new PreordersRow[10];
		for (int k = 0; k < 10; k++)
		{
			array2[k] = new PreordersRow();
		}
		int upperBound = grid.GetUpperBound(1);
		for (int l = 1; l < upperBound - 1; l++)
		{
			string value = grid[0, l];
			float smallestWidth = array2.Min((PreordersRow x) => x.width);
			int num = Array.FindIndex(array2, (PreordersRow x) => x.width == smallestWidth);
			PreordersRow preordersRow = array2[num];
			preordersRow.width += GetNameWidth(value) + 15f;
			StringBuilder sb = preordersRow.sb;
			int num2 = num + preordersRow.names;
			sb.Append(array[(int)Mathf.Repeat(num2, array.Length)]);
			sb.Append(value);
			sb.Append("</color>      ");
			preordersRow.names++;
		}
		topText.text = string.Join("\n\n", array2[0].sb, array2[1].sb, array2[2].sb, array2[3].sb, array2[4].sb, "");
		bottomText.text = string.Join("\n\n", "", array2[5].sb, array2[6].sb, array2[7].sb, array2[8].sb, array2[9].sb);
		float GetNameWidth(string name)
		{
			float num3 = 0f;
			foreach (char key in name)
			{
				num3 = ((!charWidthLUT.TryGetValue(key, out var value2)) ? (num3 + aproxWidth) : (num3 + value2));
			}
			return num3;
		}
	}
}
