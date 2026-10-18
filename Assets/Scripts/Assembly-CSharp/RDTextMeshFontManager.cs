using UnityEngine;

public class RDTextMeshFontManager : RDBase
{
	public bool enlargeForMobile;

	public bool changeFontForLanguage;

	public float sizeIfCJK;

	private TextMesh text;

	private void Awake()
	{
		if (changeFontForLanguage)
		{
			text = GetComponent<TextMesh>();
			MeshRenderer component = GetComponent<MeshRenderer>();
			float characterSize = ((sizeIfCJK != 0f) ? sizeIfCJK : text.characterSize);
			if (RDString.isCJK)
			{
				text.font = RDString.currentLanguageFont;
				component.material = text.font.material;
				text.characterSize = characterSize;
			}
		}
	}
}
