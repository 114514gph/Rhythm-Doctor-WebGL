using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class RDTransitionPiece : RDBase
{
	[NonSerialized]
	public tk2dSprite sprite;

	[NonSerialized]
	public int sheetStartPos;

	[NonSerialized]
	public int sheetEndPos;

	private Coroutine animateLinealCoroutine;

	public void SetSpriteSheet(string spriteSourceName)
	{
		sprite = GetComponent<tk2dSprite>();
		sheetStartPos = (sheetEndPos = sprite.spriteId);
		string value = sprite.CurrentSprite.name.Remove(sprite.CurrentSprite.name.LastIndexOf('/'));
		int num = sheetStartPos + 1;
		do
		{
			num++;
		}
		while (sprite.Collection.spriteDefinitions[num].name.Contains(value));
		sheetEndPos = num - 1;
	}

	public void AnimateLineal(float duration, float startDelay = 0f, bool reverse = false)
	{
		if (animateLinealCoroutine != null)
		{
			StopCoroutine(animateLinealCoroutine);
		}
		animateLinealCoroutine = StartCoroutine(AnimateLinealInternal(duration, startDelay, reverse));
	}

	public IEnumerator AnimateLinealInternal(float duration, float startDelay = 0f, bool reverse = false)
	{
		yield return new WaitForSecondsRealtime(startDelay);
		float timeToChangeSprite = duration / (float)(sheetEndPos - sheetStartPos);
		int num = (reverse ? sheetEndPos : sheetStartPos);
		int endIndex = (reverse ? sheetStartPos : sheetEndPos);
		sprite.SetSprite(num);
		yield return new WaitForSecondsRealtime(timeToChangeSprite);
		do
		{
			int num2 = (reverse ? (sprite.spriteId - 1) : (sprite.spriteId + 1));
			sprite.SetSprite(num2);
			yield return new WaitForSecondsRealtime(timeToChangeSprite);
		}
		while (sprite.spriteId != endIndex);
	}
}
