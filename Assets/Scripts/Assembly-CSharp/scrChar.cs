using System;
using System.Collections.Generic;
using UnityEngine;

public partial class scrChar : Entity
{
	public const float DefaultFramesPerBeat = 4f;

	public static Dictionary<Character, CustomAnimationData> baseCharacterAnimations;

	public static Dictionary<string, scrChar> namedCharacters = new Dictionary<string, scrChar>();

	public string charName;

	public Character character;

	[NonSerialized]
	public int possessed;

	[NonSerialized]
	public bool keepAnimation;

	[NonSerialized]
	public bool noHappy;

	[NonSerialized]
	public bool noMiss;

	[NonSerialized]
	public bool noBeat;

	[NonSerialized]
	public bool noBeep;

	private int beatsOnThisExpression;

	public string neutralAnimName = "neutral";

	public string barelyAnimName = "barely";

	public string missedAnimName = "missed";

	public string happyAnimName = "happy";

	public string beepAnimName = "beep";

	public string prehitAnimName = "prehit";

	public string expressionPrefix = "";

	private bool setup;

	public CustomAnimation customAnimation;

	private void Start()
	{
		if (!setup)
		{
			Setup(character);
		}
	}

	public void Setup(Character character, string expression = "neutral", string customCharacterName = null)
	{
		Setup();
		LevelBase levelBase = scnGame.instance?.currentLevel;
		if (character == Character.Custom && customCharacterName != null && !levelBase.customCharacterData.ContainsKey(customCharacterName))
		{
			character = Character.Samurai;
		}
		this.character = character;
		customAnimation.data = ((character == Character.Custom) ? levelBase.customCharacterData[customCharacterName] : baseCharacterAnimations[character]);
		if (!setup)
		{
			CustomAnimation obj = customAnimation;
			obj.onAnimationCompleted = (Action<CustomAnimation, CustomAnimationClip>)Delegate.Combine(obj.onAnimationCompleted, new Action<CustomAnimation, CustomAnimationClip>(OnCustomAnimEnd));
			CustomAnimation obj2 = customAnimation;
			obj2.onAnimationCompleted = (Action<CustomAnimation, CustomAnimationClip>)Delegate.Combine(obj2.onAnimationCompleted, new Action<CustomAnimation, CustomAnimationClip>(DoAnimationCompletedEvent));
			CustomAnimation obj3 = customAnimation;
			obj3.onClipFrameUpdate = (Action<CustomAnimation, CustomAnimationClip, int>)Delegate.Combine(obj3.onClipFrameUpdate, new Action<CustomAnimation, CustomAnimationClip, int>(DoAnimationFrameEvent));
		}
		customAnimation.enabled = true;
		PlayExpression(expression);
		setup = true;
	}

	private void DoAnimationFrameEvent(CustomAnimation animation, CustomAnimationClip clip, int frame)
	{
		if (clip.frameEvents.TryGetValue(frame, out var value))
		{
			DoAllAnimationEvents(value);
		}
	}

	private void DoAnimationCompletedEvent(CustomAnimation animation, CustomAnimationClip clip)
	{
		DoAnimationFrameEvent(animation, clip, -1);
	}

	private void DoAllAnimationEvents(Dictionary<AnimationEvent, string> events)
	{
		foreach (KeyValuePair<AnimationEvent, string> @event in events)
		{
			DoAnimationEvent(@event.Key, @event.Value);
		}
	}

	private void DoAnimationEvent(AnimationEvent eventType, string eventValue)
	{
		switch (eventType)
		{
		case AnimationEvent.changeExpressionPrefix:
			expressionPrefix = eventValue;
			break;
		case AnimationEvent.changeCharacter:
		{
			if (Enum.TryParse<Character>(eventValue, out var result))
			{
				ChangeCharacter(result);
			}
			break;
		}
		case AnimationEvent.playClip:
			PlayExpression(eventValue, disableOverride: true, ignoreKeep: false, 1f, alreadyPrefixed: true);
			break;
		case AnimationEvent.playExpression:
			PlayExpression(eventValue);
			break;
		}
	}

	public void ResetExpressions()
	{
		neutralAnimName = "neutral";
		barelyAnimName = "barely";
		missedAnimName = "missed";
		happyAnimName = "happy";
		beepAnimName = "beep";
		prehitAnimName = "prehit";
		expressionPrefix = "";
		PlayExpression("neutral");
	}

	public void ChangeCharacter(Character newChar, string customName = null)
	{
		ResetExpressions();
		if ((character == Character.Custom || newChar != character) && (character != Character.Custom || !(customName == customAnimation.data.name)))
		{
			Setup(newChar, "neutral", customName);
			charName = newChar.ToString();
		}
	}

	public override void OnBeat()
	{
		beatsOnThisExpression++;
		if (noBeat)
		{
			return;
		}
		CustomAnimationClip currentClip = customAnimation.currentClip;
		if (currentClip == null)
		{
			return;
		}
		if (customAnimation.isLoopOnBeat)
		{
			float num = ((currentClip.fps <= 0f) ? 4f : currentClip.fps) / base.conductor.visualCrotchet;
			float timeStart = (float)currentClip.loopStart * 1f / num;
			if (customAnimation.clipFrame >= currentClip.frames.Length - 1)
			{
				customAnimation.Play(currentClip.name, timeStart, num);
			}
		}
		else if (currentClip.fps == 0f && beatsOnThisExpression == 3)
		{
			PlayExpression("neutral");
		}
	}

	public void PlayExpression(string expression, bool disableOverride = false, bool ignoreKeep = false, float speedMultiplier = 1f, bool alreadyPrefixed = false, float time = 0f)
	{
		if (!disableOverride)
		{
			expression = ExpressionOverride(expression);
		}
		if (!alreadyPrefixed && !expressionPrefix.IsNullOrEmpty())
		{
			expression = string.Join('_', expressionPrefix, expression);
		}
		if (expression == "" || (keepAnimation && !ignoreKeep))
		{
			return;
		}
		if (noBeat && expression != neutralAnimName && (IsCurrentExpression("happy") || IsCurrentExpression("barely") || IsCurrentExpression("missed")) && !customAnimation.isLoopOnBeat)
		{
			CustomAnimationClip currentClip = customAnimation.currentClip;
			if ((currentClip != null && currentClip.fps != 0f) || beatsOnThisExpression < 3)
			{
				return;
			}
		}
		Dictionary<string, CustomAnimationClip> clips = customAnimation.data.clips;
		if (ExpressionExists(expression, disableOverride))
		{
			CustomAnimationClip clip = clips[expression];
			customAnimation.PlayFromClip(clip, 1f, 0f, time);
		}
		else
		{
			printe("animation for custom character " + expression + " was not found");
		}
		beatsOnThisExpression = 0;
	}

	public bool ExpressionExists(string expression, bool disableOverride = false)
	{
		if (!disableOverride)
		{
			expression = ExpressionOverride(expression);
		}
		return customAnimation.data.clips.ContainsKey(expression);
	}

	public CustomAnimationClip GetCustomExpression(string expression, bool disableOverride = false)
	{
		if (!disableOverride)
		{
			expression = ExpressionOverride(expression);
		}
		Dictionary<string, CustomAnimationClip> clips = customAnimation.data.clips;
		CustomAnimationClip result = null;
		if (clips.ContainsKey(expression))
		{
			result = clips[expression];
		}
		return result;
	}

	public bool IsCurrentExpression(string expression, bool disableOverride = false)
	{
		string text = expression;
		if (!disableOverride)
		{
			text = ExpressionOverride(expression);
		}
		return customAnimation.currentClip.name == text;
	}

	public string ExpressionOverride(string expression)
	{
		return expression switch
		{
			"neutral" => neutralAnimName, 
			"barely" => barelyAnimName, 
			"missed" => missedAnimName, 
			"happy" => happyAnimName, 
			"beep" => beepAnimName, 
			"prehit" => prehitAnimName, 
			_ => expression, 
		};
	}

	public void OnCustomAnimEnd(CustomAnimation animation, CustomAnimationClip clip)
	{
		if (customAnimation.currentClip.name != "neutral" && !customAnimation.isLoopOnBeat && customAnimation.data.clips.TryGetValue("neutral", out var value))
		{
			int num = value.frames.Length;
			float num2 = ((value.fps > 0f) ? value.fps : 4f);
			int num3 = Mathf.CeilToInt((float)num / num2);
			float time = base.conductor.currentBeat % (float)num3 * base.conductor.crotchet;
			PlayExpression("neutral", disableOverride: false, ignoreKeep: false, 1f, alreadyPrefixed: false, time);
		}
	}

	public static scrChar Named(string name)
	{
		return namedCharacters[name];
	}

	public static void FlushCharacters()
	{
		namedCharacters.Clear();
	}

	public static scrChar Create(Character character, string expression = "neutral")
	{
		GameObject gameObject = new GameObject();
		gameObject.name = character.ToString();
		gameObject.AddComponent<MeshRenderer>().material = UnityEngine.Object.Instantiate(RDConstants.data.RDSpriteMaterial);
		gameObject.AddComponent<MeshFilter>();
		scrChar obj = gameObject.AddComponent<scrChar>();
		obj.customAnimation = gameObject.AddComponent<CustomAnimation>();
		obj.shaderDataToUse = RDBase.Vfx.globalShaderData;
		obj.Setup(character, expression);
		return obj;
	}
}
