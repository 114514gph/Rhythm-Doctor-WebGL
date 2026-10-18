using DG.Tweening;
using UnityEngine;

public class RDScrollyText : RDBase
{
	private TextMesh textMesh;

	private float xOffset;

	private float scaleValue;

	private float beatsToTravel;

	private bool reverse;

	private Ease ease = Ease.Linear;

	private const string BackgroundSortingLayer = "Background";

	public RDScrollyText Setup(string text, float y, float beatsToTravel, Ease easeType, float randomOffset, float randomScale, bool randomColors, Color? color, int roomIndex = 0, bool reverse = false)
	{
		base.gameObject.name = "ScrollyText: " + text;
		base.gameObject.SetActive(value: false);
		textMesh = GetComponent<TextMesh>();
		textMesh.text = text;
		if (reverse)
		{
			textMesh.anchor = TextAnchor.MiddleRight;
		}
		FontPack appropiateFontPackForString = RDString.GetAppropiateFontPackForString(text);
		textMesh.font = appropiateFontPackForString.font;
		textMesh.GetComponent<Renderer>().material = textMesh.font.material;
		scaleValue = 1f + Random.value * randomScale - randomScale / 2f;
		float num = 1f;
		if (!appropiateFontPackForString.usePixelFont)
		{
			float num2 = appropiateFontPackForString.font.fontSize;
			float num3 = GC.RoomRenderTextureScale;
			textMesh.fontSize = Mathf.RoundToInt(num2 * num3);
			num = 0.1f;
		}
		xOffset = 3000f * (float)roomIndex;
		Vector2 vector = new Vector2(RDBase.Vfx.RDWidth, RDBase.Vfx.RDHeight);
		textMesh.transform.localScale = new Vector3(scaleValue * num, scaleValue * num, 1f);
		textMesh.transform.position = new Vector3(xOffset + (reverse ? 0f : vector.x), vector.y - (y + Random.Range(0f, randomOffset) - randomOffset / 2f), 0f);
		Renderer component = textMesh.GetComponent<Renderer>();
		component.sortingLayerName = "Background";
		component.sortingOrder = -100;
		textMesh.color = (randomColors ? RDBase.Vfx.GetRandomColor().WithAlpha(0.8f) : (color.HasValue ? color.Value : Color.gray));
		ease = easeType;
		this.beatsToTravel = beatsToTravel;
		this.reverse = reverse;
		return this;
	}

	public void Run()
	{
		base.gameObject.SetActive(value: true);
		float num = RDBase.Vfx.RDWidth * scaleValue * 1.5f;
		float endValue = ((!reverse) ? (xOffset - num) : (xOffset + RDBase.Vfx.RDWidth + num));
		float duration = beatsToTravel * base.conductor.crotchet;
		textMesh.transform.DOMoveX(endValue, duration).SetEase(ease).OnComplete(() =>
		{
			base.gameObject.SetActive(value: false);
		});
	}

	public static RDScrollyText Create()
	{
		GameObject obj = Object.Instantiate(RDBase.Vfx.scrollyTextPrefab, scrGameManager.instance.parentContainer);
		obj.SetActive(value: false);
		return obj.GetComponent<RDScrollyText>();
	}
}
