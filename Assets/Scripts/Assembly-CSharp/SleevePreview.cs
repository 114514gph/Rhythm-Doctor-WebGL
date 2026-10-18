using UnityEngine;
using UnityEngine.UI;

public class SleevePreview : RDBase
{
	public Image sleeve;

	public RawImage hand;

	public RawImage nails;

	public RawImage palm;

	private void Awake()
	{
		sleeve.material = new Material(sleeve.material);
	}

	public void SetArmSkin(ArmSkin armSkin)
	{
		SetTexture(armSkin.drawing);
		SetHandColor(armSkin.skinColor);
		SetSleeveColor(armSkin.sleeveColor);
		SetNailsColor(armSkin.nailsColor);
		SetPalmColor(armSkin.skinColor.WithLightnessBoost(armSkin.palmLightness));
	}

	public void SetTexture(Texture texture)
	{
		texture.filterMode = FilterMode.Point;
		sleeve.material.SetTexture("_Drawing", texture);
		sleeve.material.SetColor("_TintColor", Color.white);
	}

	public void SetHandColor(Color color)
	{
		hand.color = color;
	}

	public void SetSleeveColor(Color color)
	{
		sleeve.color = color;
	}

	public void SetNailsColor(Color color)
	{
		nails.color = color;
	}

	public void SetPalmColor(Color color)
	{
		palm.color = color;
	}
}
