using UnityEngine;
using UnityEngine.UI;

public static class tk2dRDExtras
{
	public static void SetTk2dSprite(this RawImage rawImage, tk2dSpriteCollectionData collection, int spriteId)
	{
		if (spriteId != -1)
		{
			tk2dSpriteDefinition tk2dSpriteDefinition2 = collection.spriteDefinitions[spriteId];
			rawImage.texture = tk2dSpriteDefinition2.material.mainTexture;
			Vector2 vector = tk2dSpriteDefinition2.uvs[0];
			Vector2 vector2 = tk2dSpriteDefinition2.uvs[3];
			rawImage.uvRect = new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
			rawImage.rectTransform.sizeDelta = new Vector2(tk2dSpriteDefinition2.regionW, tk2dSpriteDefinition2.regionH);
		}
	}
}
