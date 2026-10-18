using UnityEngine;

public class TextSwitch : RDBase
{
	public delegate void OptionSelected(int option);

	public int currentOption;

	public string[] options;

	public OptionSelected optionSelected;

	private TextMesh textMesh;

	private void Start()
	{
		textMesh = GetComponent<TextMesh>();
		RDStringToUIText.Apply(textMesh, changeFontForLanguage: true, -1f);
		UpdateText();
	}

	private void Update()
	{
		if (RDInput.leftPress)
		{
			LeftAction();
		}
		else if (RDInput.rightPress)
		{
			RightAction();
		}
		else if (RDInput.anyPlayerPress && optionSelected != null)
		{
			optionSelected(currentOption);
		}
	}

	public void LeftAction()
	{
		UpdateOption(-1);
	}

	public void RightAction()
	{
		UpdateOption(1);
	}

	private void UpdateOption(int direction)
	{
		currentOption += direction;
		if (currentOption == options.Length)
		{
			currentOption = 0;
		}
		else if (currentOption == -1)
		{
			currentOption = options.Length - 1;
		}
		if (scnBase.instance is scnExpoMenu)
		{
			scnExpoMenu instance = scnExpoMenu.instance;
			instance.startAfreshSelected = false;
			if (RDInput.leftPress || RDInput.rightPress)
			{
				scrConductor.PlayImmediatelyIgnoringTimeScale("sndPagerCursorMove", 1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"));
				instance.introMenu.tip.text = instance.defaultInstructions;
			}
		}
		UpdateText();
	}

	public void UpdateText()
	{
		string text = options[currentOption];
		string text2 = (text.Contains(".") ? RDString.Get(text) : text);
		string text3 = " ";
		textMesh.font = RDString.GetAppropiateFontForString(text2);
		RDConstants data = RDConstants.data;
		if (textMesh.font != data.latinFontPack.font)
		{
			textMesh.font = data.chineseTradFontPack.font;
			printe("this is happening! " + textMesh.font.name);
			text3 = "\u3000";
			textMesh.fontSize = 40;
			int num = 10;
			textMesh.characterSize = 0.2f * (float)num;
		}
		else
		{
			textMesh.characterSize = 15f;
		}
		textMesh.GetComponent<Renderer>().material = textMesh.font.material;
		textMesh.text = "<color=#22B14CFF><" + text3 + "</color>" + text2 + "<color=#22B14CFF>" + text3 + "></color>";
	}
}
