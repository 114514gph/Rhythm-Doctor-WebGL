using System;
using UnityEngine;

[Serializable]
public class tk2dUILayoutItem
{
	public tk2dBaseSprite sprite;

	public tk2dUIMask UIMask;

	public tk2dUILayout layout;

	public GameObject gameObj;

	public bool snapLeft;

	public bool snapRight;

	public bool snapTop;

	public bool snapBottom;

	public bool fixedSize;

	public float fillPercentage = -1f;

	public float sizeProportion = 1f;

	public bool inLayoutList;

	public int childDepth;

	public Vector3 oldPos = Vector3.zero;

	public static tk2dUILayoutItem FixedSizeLayoutItem()
	{
		return new tk2dUILayoutItem
		{
			fixedSize = true
		};
	}
}
