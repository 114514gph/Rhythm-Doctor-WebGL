using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimationClip
{
	public string name;

	public int[] frames;

	public float fps;

	public AnimationType type;

	public int loopStart;

	public Vector2 pivotOffset;

	public float reflectionOffset;

	public string endEvent;

	public Dictionary<int, Dictionary<AnimationEvent, string>> frameEvents;

	public Vector2 portraitOffset;

	public Vector2Int portraitSize;

	public float portraitScale;

	public string portraitName;

	public override string ToString()
	{
		string text = "";
		int[] array = frames;
		foreach (int num in array)
		{
			text = text + num + " ";
		}
		return $"name: {name} / frames {text} / fps {fps} / type {type} / loopStart {loopStart}";
	}

	public Vector2 GetPortraitTransformSize()
	{
		return new Vector2(Mathf.RoundToInt((float)portraitSize.x * portraitScale), Mathf.RoundToInt((float)portraitSize.y * portraitScale));
	}

	public void AddFrameEvent(int frame, string key, string value)
	{
		if (Enum.TryParse<AnimationEvent>(key, out var result))
		{
			if (!frameEvents.ContainsKey(frame))
			{
				frameEvents[frame] = new Dictionary<AnimationEvent, string>();
			}
			if (result == AnimationEvent.changeExpressionPrefix && value.EndsWith("_"))
			{
				string text = value;
				value = text.Substring(0, text.Length - 1);
			}
			frameEvents[frame][result] = value;
		}
	}

	public void AddLegacyFrameEvent(int frame, string eventData)
	{
		string[] array = eventData.TrimAllSpaces().Split(":");
		string key = array[0];
		string value = ((array.Length != 0) ? array[1] : "");
		AddFrameEvent(frame, key, value);
	}
}
