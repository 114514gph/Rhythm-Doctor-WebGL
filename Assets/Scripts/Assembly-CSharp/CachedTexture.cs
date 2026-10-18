using System;
using UnityEngine;

public struct CachedTexture
{
	public DateTime fileLastAccessed;

	public Texture2D texture;

	public bool isAlpha8;
}
