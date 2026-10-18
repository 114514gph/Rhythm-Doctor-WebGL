using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
	public string asset = "credits";

	public Color[] colors;

	public Transform patexumLogo;

	public float extraY;

	public bool isHelpingHands;

	public RectTransform logoTransform;

	private RectTransform rect;

	private Text text;

	private StringBuilder sb;

	private string[,] grid;

	private SystemLanguage lastLanguage = SystemLanguage.English;

	private bool firstTime;

	private Vector2 initialPosition;

	private float initialLineSpacing;

	private void Awake()
	{
		rect = GetComponent<RectTransform>();
		sb = new StringBuilder(10000);
		text = GetComponent<Text>();
		CSVReader cSVReader = new CSVReader(Resources.Load<TextAsset>(asset));
		grid = cSVReader.grid;
		initialPosition = rect.localPosition;
		initialLineSpacing = text.lineSpacing;
	}

	private void OnEnable()
	{
		SystemLanguage language = Persistence.GetLanguage();
		if (!firstTime || lastLanguage != language)
		{
			firstTime = true;
			lastLanguage = language;
			UpdateCredits();
		}
	}

	private void UpdateCredits()
	{
		sb.Clear();
		int num = 0;
		string text = "";
		for (int i = 1; i < grid.GetUpperBound(1); i++)
		{
			string text2 = grid[0, i];
			string text3 = grid[1, i];
			string text4 = grid[2, i];
			bool num2 = !string.IsNullOrEmpty(text2);
			bool flag = !string.IsNullOrEmpty(text3);
			bool flag2 = !string.IsNullOrEmpty(text4);
			bool flag3 = num2 && text2.StartsWith("::");
			if (num2 && !flag)
			{
				text = text2;
				if (!flag3)
				{
					string withCheck = RDString.GetWithCheck(text2, out var exists);
					withCheck = (exists ? withCheck : text2).ToUpper();
					sb.Append("\n\n\n<color=#FB3C68>" + withCheck + "</color>");
					if (text == "credits.tools" || text == "credits.localization")
					{
						sb.Append("\n");
					}
				}
			}
			if (!((num2 & flag) | flag3))
			{
				continue;
			}
			string text5 = ColorUtility.ToHtmlStringRGB(colors[num++ % colors.Length]);
			if (text == "credits.tools" || text == "credits.localization" || (text.StartsWith("::") && !flag3))
			{
				string withCheck2 = RDString.GetWithCheck(text2, out var exists2);
				sb.Append("\n" + (exists2 ? withCheck2 : text2) + " <color=#aaa>" + text3 + "</color>");
				continue;
			}
			if (grid[0, i - 1] == text2)
			{
				sb.Append("\n" + text3);
				if (flag2)
				{
					sb.Append(" <color=#aaa>" + text4 + "</color>");
				}
				continue;
			}
			text2 = text2.Replace("::", "");
			string withCheck3 = RDString.GetWithCheck(text2, out var exists3);
			sb.Append("\n\n<color=#" + text5 + ">" + (exists3 ? withCheck3 : text2) + "</color>");
			if (flag)
			{
				sb.Append("\n" + text3);
			}
		}
		this.text.text = sb.ToString();
		UpdateFont();
	}

	public void UpdateFont()
	{
		RDStringToUIText.Apply(text);
		rect.localScale = Vector2.one;
		rect.localPosition = initialPosition;
		text.lineSpacing = initialLineSpacing;
		patexumLogo.localPosition = new Vector2(patexumLogo.localPosition.x, 0f);
		bool flag = text.font.name.Contains("NotoSans");
		if (flag)
		{
			extraY = (isHelpingHands ? 1.5f : 1.8f);
			float num = (isHelpingHands ? 1f : 0.83f);
			rect.localScale = new Vector2(num, num);
			text.lineSpacing = (isHelpingHands ? 1.04f : 1.27f);
			rect.localPosition = new Vector2(rect.localPosition.x, rect.localPosition.y + extraY);
		}
		int num2 = 1;
		string[] array = text.text.Split("\n");
		for (int i = 0; i < array.Length && !array[i].Contains("Patexum"); i++)
		{
			num2++;
		}
		float num3 = (flag ? 393.375f : (-3f));
		float num4 = text.lineSpacing + (flag ? (-0.23f) : 0f);
		float y = (0f - ((float)((num2 - 1) * text.fontSize) * num4 + num3)) / rect.localScale.y;
		patexumLogo.localPosition = patexumLogo.localPosition.WithY(y);
	}
}
