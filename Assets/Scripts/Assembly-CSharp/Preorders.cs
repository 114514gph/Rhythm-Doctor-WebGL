using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Preorders : MonoBehaviour
{
	public string assetName;

	public Text[] texts;

	public Color[] colors;

	private string[] hexColors;

	private void Awake()
	{
		string[,] grid = new CSVReader(Resources.Load<TextAsset>(assetName)).grid;
		int num = texts.Length;
		int num2 = 0;
		hexColors = new string[colors.Length];
		Color[] array = colors;
		foreach (Color color in array)
		{
			hexColors[num2] = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">";
			num2++;
		}
		StringBuilder[] array2 = new StringBuilder[num];
		for (num2 = 0; num2 < num; num2++)
		{
			array2[num2] = new StringBuilder();
		}
		for (int j = 1; j < grid.GetUpperBound(1); j++)
		{
			int num3 = (j - 1) % num;
			StringBuilder obj = array2[num3];
			obj.Append(hexColors[j % hexColors.Length]);
			obj.Append(grid[0, j]);
			obj.Append("</color>");
			obj.Append("\n\n");
		}
		for (num2 = 0; num2 < num; num2++)
		{
			texts[num2].text = array2[num2].ToString();
		}
	}
}
