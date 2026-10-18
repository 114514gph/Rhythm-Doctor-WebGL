using UnityEngine.UI;

public class RDUITextFontManager : RDBase
{
	public bool inactive;

	public bool changeFontForLanguage;

	public bool isStatusText;

	public bool isLevelEditor;

	private void Start()
	{
		if (!inactive)
		{
			RDStringToUIText.Apply(GetComponent<Text>(), changeFontForLanguage, isStatusText);
		}
	}
}
