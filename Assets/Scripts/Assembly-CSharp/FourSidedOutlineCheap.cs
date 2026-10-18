using UnityEngine;
using UnityEngine.UI;

public class FourSidedOutlineCheap : RDBase
{
	private const int CopiesCount = 4;

	public RectTransform[] outlines;

	private Text[] textComponents = new Text[4];

	private Text text;

	public Color effectColor = new Color(0f, 0f, 0f, 0.5f);

	public Vector2 effectDistance = new Vector2(1f, 1f);

	public bool useGraphicAlpha;

	private void Awake()
	{
		text = GetComponent<Text>();
		for (int i = 0; i < 4; i++)
		{
			textComponents[i] = outlines[i].GetComponent<Text>();
		}
	}

	private void LateUpdate()
	{
		Vector3[] array = new Vector3[4]
		{
			new Vector2(1f, 0f) * effectDistance.x,
			new Vector2(0f, 1f) * effectDistance.y,
			new Vector2(-1f, 0f) * effectDistance.x,
			new Vector2(0f, -1f) * effectDistance.y
		};
		for (int i = 0; i < 4; i++)
		{
			outlines[i].anchoredPosition3D = (base.transform as RectTransform).anchoredPosition3D + array[i];
			Text obj = textComponents[i];
			obj.color = effectColor * text.color.a;
			obj.alignment = text.alignment;
			obj.fontSize = text.fontSize;
		}
	}

	public void SetText(string text)
	{
		for (int i = 0; i < 4; i++)
		{
			textComponents[i].text = text;
		}
	}
}
