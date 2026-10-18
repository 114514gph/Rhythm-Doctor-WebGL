using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RDStringToUIText : RDBase
{
	private const float UITextSizeMultiplier = 4f;

	private const float TextMeshSizeMultiplier = 5f;

	public const float OutlineFactorPixel = 8f;

	public const float OutlineFactorVector = 12f;

	public string key;

	public float cjkScale;

	public bool changeFontForLanguage = true;

	public bool isStatusText;

	public bool isLevelEditor;

	[Tooltip("Scale the text down when it renders wider than its rect. Unlike Best Fit, this works with the non-dynamic pixel fonts.")]
	public bool shrinkToFitWidth;

	private Vector3 baseScale;

	private bool baseScaleCached;

	private void Awake()
	{
		if (base.enabled)
		{
			RefreshText();
		}
	}

	public void RefreshText()
	{
		Text component = GetComponent<Text>();
		if (component != null)
		{
			if (!string.IsNullOrEmpty(key))
			{
				component.text = RDString.Get(key);
			}
			Apply(component, changeFontForLanguage, isStatusText, cjkScale);
			if (shrinkToFitWidth)
			{
				ShrinkToFitWidth(component);
			}
			return;
		}
		TextMesh component2 = GetComponent<TextMesh>();
		if (component2 != null)
		{
			if (!string.IsNullOrEmpty(key))
			{
				component2.text = RDString.Get(key);
			}
			Apply(component2, changeFontForLanguage, cjkScale);
			return;
		}
		TextMeshProUGUI component3 = GetComponent<TextMeshProUGUI>();
		if (component3 != null)
		{
			if (!string.IsNullOrEmpty(key))
			{
				component3.text = RDString.Get(key);
			}
			Apply(component3, changeFontForLanguage, cjkScale);
		}
		else
		{
			Debug.LogError("didn't found any uitext or textmesh to set RDString with key: " + key + " onGameObject: " + base.gameObject.name);
		}
	}

	private void ShrinkToFitWidth(Text text)
	{
		RectTransform rectTransform = text.rectTransform;
		if (!baseScaleCached)
		{
			baseScale = rectTransform.localScale;
			baseScaleCached = true;
		}
		rectTransform.localScale = baseScale;
		float width = rectTransform.rect.width;
		float preferredWidth = text.preferredWidth;
		if (preferredWidth > width && preferredWidth > 0f && width > 0f)
		{
			rectTransform.localScale = baseScale * (width / preferredWidth);
		}
	}

	public static void Apply(Text text, bool changeFontForLanguage = true, bool isStatusText = false, float scale = 0f)
	{
		if (!changeFontForLanguage)
		{
			return;
		}
		RDConstants data = RDConstants.data;
		if (RDString.isCJK || Persistence.GetReducePixelFontUsage())
		{
			float num = 1f;
			text.font = RDString.currentLanguageFont;
			text.lineSpacing = 1f;
			text.fontSize = Mathf.RoundToInt(((float)text.fontSize + scale * 4f) * num);
			EightSidedOutline component = text.gameObject.GetComponent<EightSidedOutline>();
			if ((bool)component)
			{
				float effectDistance = (float)text.fontSize / 12f;
				component.effectDistance = effectDistance;
			}
			if (isStatusText)
			{
				if ((bool)component)
				{
					component.enabled = true;
				}
				text.color = "cececeff".HexToColor();
			}
		}
		else
		{
			text.font = data.latinFontPack.font;
			text.resizeTextForBestFit = false;
		}
	}

	public static void Apply(TextMesh text, bool changeFontForLanguage = true, float cjkScale = 0f)
	{
		if (changeFontForLanguage)
		{
			if (RDString.isCJK || Persistence.GetReducePixelFontUsage())
			{
				text.font = RDString.currentLanguageFont;
				text.lineSpacing = 1f;
				text.GetComponent<MeshRenderer>().material = text.font.material;
				text.fontSize = 40;
				text.characterSize = 0.2f * text.characterSize;
			}
		}
		else
		{
			RDBaseDllDummy.printesw("change font for language is false!: " + text.gameObject.name);
		}
	}

	public static void Apply(TextMeshProUGUI text, bool changeFontForLanguage = true, float cjkScale = 0f)
	{
		if (changeFontForLanguage)
		{
			text.font = RDString.currentLanguageFontAsset;
		}
	}
}
