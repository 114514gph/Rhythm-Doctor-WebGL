using UnityEngine;
using UnityEngine.UI;

public class RWCredits : RDBase
{
	public Image[] borders;

	private int selectedButton;

	public Image character;

	public SpriteAnimation characterAnimation;

	public void Update()
	{
		for (int i = 0; i < borders.Length; i++)
		{
			borders[i].enabled = i == selectedButton;
		}
		RectTransform rectTransform = borders[selectedButton].rectTransform;
		int num = ((character.sprite == characterAnimation.currentAnimationData.sprites[1]) ? 5 : 4);
		rectTransform.offsetMin = new Vector2(-num, -num);
		rectTransform.offsetMax = new Vector2(num, num);
		if (RDInput.rightPress || RDInput.leftPress)
		{
			selectedButton = 1 - selectedButton;
		}
		if (RDInput.anyPlayerPress)
		{
			if (selectedButton == 0)
			{
				GoTo7thBeatWebsite();
			}
			else
			{
				GoToRhythmDoctorWebsite();
			}
		}
	}

	public void GoToRhythmDoctorWebsite()
	{
		Application.OpenURL("https://rhythmdr.com");
	}

	public void GoTo7thBeatWebsite()
	{
		Application.OpenURL("https://7thbe.at");
	}
}
