using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct FontPack
{
	public SystemLanguage language;

	public bool usePixelFont;

	public FontData vectorFont;

	public FontData pixelFont;

	public FontData flashFont;

	public TMP_FontAsset vectorFontAsset;

	public TMP_FontAsset pixelFontAsset;

	public TMP_FontAsset flashFontAsset;

	public Font font
	{
		get
		{
			if (!usePixelFont)
			{
				return vectorFont.font;
			}
			return pixelFont.font;
		}
	}

	public TMP_FontAsset fontAsset
	{
		get
		{
			if (!usePixelFont)
			{
				return vectorFontAsset;
			}
			return pixelFontAsset;
		}
	}

	public bool isCJK
	{
		get
		{
			SystemLanguage systemLanguage = language;
			return systemLanguage == SystemLanguage.ChineseSimplified || systemLanguage == SystemLanguage.ChineseTraditional || systemLanguage == SystemLanguage.Chinese || systemLanguage == SystemLanguage.Korean || systemLanguage == SystemLanguage.Japanese;
		}
	}
}
