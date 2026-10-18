using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteAnimation")]
public class tk2dSpriteAnimation : MonoBehaviour
{
	public tk2dSpriteAnimationClip[] clips;

	[NonSerialized]
	public Dictionary<string, tk2dSpriteAnimationClip> clipDicts;

	[NonSerialized]
	public Dictionary<string, tk2dSpriteAnimationClip> clipDictsLower;

	[NonSerialized]
	public bool setup;

	public tk2dSpriteAnimationClip FirstValidClip
	{
		get
		{
			for (int i = 0; i < clips.Length; i++)
			{
				if (!clips[i].Empty && clips[i].frames[0].spriteCollection != null && clips[i].frames[0].spriteId != -1)
				{
					return clips[i];
				}
			}
			return null;
		}
	}

	public void Setup()
	{
		setup = true;
		clipDicts = new Dictionary<string, tk2dSpriteAnimationClip>();
		clipDictsLower = new Dictionary<string, tk2dSpriteAnimationClip>();
		tk2dSpriteAnimationClip[] array = clips;
		foreach (tk2dSpriteAnimationClip tk2dSpriteAnimationClip2 in array)
		{
			if (!string.IsNullOrEmpty(tk2dSpriteAnimationClip2.name))
			{
				clipDicts.Add(tk2dSpriteAnimationClip2.name, tk2dSpriteAnimationClip2);
				clipDictsLower.Add(tk2dSpriteAnimationClip2.name.ToLower(), tk2dSpriteAnimationClip2);
			}
		}
	}

	public tk2dSpriteAnimationClip GetClipByName(string name, bool anyCase = false)
	{
		if (!setup)
		{
			Setup();
		}
		if (anyCase)
		{
			string key = name.ToLower();
			if (!clipDictsLower.ContainsKey(key))
			{
				return null;
			}
			return clipDictsLower[key];
		}
		if (!clipDicts.ContainsKey(name))
		{
			return null;
		}
		return clipDicts[name];
	}

	public tk2dSpriteAnimationClip GetClipById(int id)
	{
		if (id < 0 || id >= clips.Length || clips[id].Empty)
		{
			return null;
		}
		return clips[id];
	}

	public int GetClipIdByName(string name)
	{
		for (int i = 0; i < clips.Length; i++)
		{
			if (clips[i].name == name)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetClipIdByName(tk2dSpriteAnimationClip clip)
	{
		for (int i = 0; i < clips.Length; i++)
		{
			if (clips[i] == clip)
			{
				return i;
			}
		}
		return -1;
	}
}
