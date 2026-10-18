using UnityEngine;
using UnityEngine.UI;

public class SimpleCredits : RDBase
{
	public int index;

	public bool finished;

	public Sprite[] sprites;

	public Image image;

	public void Rewind()
	{
		index = 0;
		image.sprite = sprites[0];
		finished = false;
	}

	private void Update()
	{
		if (RDInput.anyPlayerPress)
		{
			if (index < sprites.Length - 1)
			{
				index++;
				image.sprite = sprites[index];
				scrConductor.PlayImmediately("sndHammer", 1f, RDUtils.GetMixerGroup("MainMenu"));
			}
			else
			{
				finished = true;
			}
		}
	}
}
