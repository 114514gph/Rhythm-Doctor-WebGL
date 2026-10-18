using System;
using System.IO;
using UnityEngine;

[Serializable]
public class ArmSkin : RDClass
{
	public const int width = 262;

	public const int height = 20;

	public const int textureWidth = 524;

	public const int textureHeight = 40;

	private const int factor = 2;

	private int slot;

	private RDPlayer player;

	public Color sleeveColor;

	public Color skinColor;

	public Color nailsColor;

	public float palmLightness;

	public Texture2D drawing;

	private bool loaded;

	private const int currentLoadedSlot = -1;

	public ArmSkin(Color sleeveColor, Color skinColor, Color nailsColor, float palmLightness)
	{
		slot = -1;
		this.sleeveColor = sleeveColor;
		this.skinColor = skinColor;
		this.nailsColor = nailsColor;
		this.palmLightness = palmLightness;
	}

	public ArmSkin(int slot, RDPlayer player)
	{
		this.slot = slot;
		this.player = player;
		if (player == RDPlayer.P1)
		{
			skinColor = base.gc.skinColors[base.gc.defaultP1SkinColor[slot]];
			sleeveColor = base.gc.sleeveColors[base.gc.defaultP1SleeveColor[slot]];
			nailsColor = base.gc.nailColors[base.gc.defaultP1NailColor[slot]];
			palmLightness = base.gc.defaultP1PalmLightness[slot];
			drawing = base.gc.emptyTexture;
		}
		else
		{
			skinColor = base.gc.skinColors[base.gc.defaultP2SkinColor[slot]];
			sleeveColor = base.gc.sleeveColors[base.gc.defaultP2SleeveColor[slot]];
			nailsColor = base.gc.nailColors[base.gc.defaultP2NailColor[slot]];
			palmLightness = base.gc.defaultP2PalmLightness[slot];
			drawing = base.gc.emptyTexture;
		}
	}

	public void Reload()
	{
		loaded = false;
		Load();
	}

	public void Load()
	{
		if (loaded)
		{
			return;
		}
		loaded = true;
		string drawingPath = GetDrawingPath(player, slot);
		if (RDFile.Exists(drawingPath))
		{
			byte[] data = RDFile.ReadAllBytes(drawingPath);
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(data);
			drawing = texture2D;
			sleeveColor = texture2D.GetPixel(texture2D.width - 1, texture2D.height - 1);
			skinColor = texture2D.GetPixel(texture2D.width - 1, texture2D.height - 2);
			palmLightness = texture2D.GetPixel(texture2D.width - 1, texture2D.height - 4).a;
			Color pixel = texture2D.GetPixel(texture2D.width - 1, texture2D.height - 3);
			if (pixel.a > 0f)
			{
				nailsColor = pixel;
			}
		}
		else
		{
			Save(null, sleeveColor, skinColor, nailsColor, palmLightness);
		}
	}

	public void Save(Texture2D texture, Color sleeveColor, Color skinColor, Color nailsColor, float palmLightness)
	{
		if (texture == null)
		{
			texture = new Texture2D(524, 40);
			Color[] array = new Color[20960];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Color.clear;
			}
			texture.SetPixels(array);
		}
		texture.SetPixel(texture.width - 1, texture.height - 1, sleeveColor);
		texture.SetPixel(texture.width - 1, texture.height - 2, skinColor);
		texture.SetPixel(texture.width - 1, texture.height - 3, nailsColor);
		texture.SetPixel(texture.width - 1, texture.height - 4, Color.white.WithAlpha(palmLightness));
		texture.Apply();
		byte[] bytes = texture.EncodeToPNG();
		RDFile.WriteAllBytes(GetDrawingPath(player, slot), bytes);
		this.sleeveColor = sleeveColor;
		this.skinColor = skinColor;
		this.nailsColor = nailsColor;
		this.palmLightness = palmLightness;
		drawing = texture;
	}

	public static string GetDrawingPath(RDPlayer player, int slot)
	{
		int num = ((slot == -1) ? Persistence.currentSlotIndex : slot);
		return string.Concat(PlayerPrefsJson.GetFileFolderPath(), Path.DirectorySeparatorChar, "scribble", player.ToString(), "_" + num, ".png");
	}

	public override string ToString()
	{
		return $"skin: {skinColor}, sleeve: {sleeveColor}, texture {drawing != null}";
	}
}
