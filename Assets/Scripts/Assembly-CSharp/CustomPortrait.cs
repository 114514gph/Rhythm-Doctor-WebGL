using System.Collections.Generic;

public class CustomPortrait : InvisibleEntity
{
	public string customCharacter;

	public Character character;

	public bool keepAnimation;

	private int beatsOnThisExpression;

	public float customCrotchet;

	public bool forceLoop;

	public CustomAnimation customAnimation;

	private new void Awake()
	{
		base.Awake();
		customAnimation.onAnimationCompleted = OnCustomAnimEnd;
	}

	public bool Setup(Character character, string expression)
	{
		customAnimation.data = scrChar.baseCharacterAnimations[character];
		customAnimation.isNewData = true;
		customAnimation.enabled = true;
		this.character = character;
		return PlayExpression(expression);
	}

	public bool Setup(string customCharacter, string expression, bool checkEditorCharData = false)
	{
		if (checkEditorCharData)
		{
			if (base.editor == null)
			{
				return false;
			}
			if (!base.editor.customCharacterData.ContainsKey(customCharacter))
			{
				return false;
			}
			customAnimation.data = base.editor.customCharacterData[customCharacter];
		}
		else
		{
			LevelBase currentLevel = scnGame.instance.currentLevel;
			if (currentLevel == null)
			{
				return false;
			}
			if (!currentLevel.customCharacterData.ContainsKey(customCharacter))
			{
				return false;
			}
			customAnimation.data = currentLevel.customCharacterData[customCharacter];
		}
		customAnimation.isNewData = true;
		customAnimation.enabled = true;
		character = Character.Custom;
		this.customCharacter = customCharacter;
		return PlayExpression(expression);
	}

	public override void OnBeat()
	{
		if (customAnimation.currentClip != null)
		{
			beatsOnThisExpression++;
			if (customAnimation.isLoopOnBeat)
			{
				customAnimation.AttemptLoopOnBeat(customCrotchet);
			}
			else if (customAnimation.currentClip.fps == 0f && beatsOnThisExpression == 3 && !forceLoop)
			{
				PlayExpression("neutral");
			}
		}
	}

	public bool PlayExpression(string expression, bool ignoreKeep = false, float speedMultiplier = 1f)
	{
		if (string.IsNullOrEmpty(expression))
		{
			return false;
		}
		if (keepAnimation && !ignoreKeep)
		{
			return false;
		}
		Dictionary<string, CustomAnimationClip> clips = customAnimation.data.clips;
		if (clips == null)
		{
			return false;
		}
		if (!clips.TryGetValue(expression, out var value))
		{
			printe("animation " + expression + " for character " + customCharacter + " was not found");
			return false;
		}
		customAnimation.PlayFromClip(value, speedMultiplier, customCrotchet);
		beatsOnThisExpression = 0;
		return true;
	}

	public void OnCustomAnimEnd(CustomAnimation animation, CustomAnimationClip clip)
	{
		string expression = (forceLoop ? customAnimation.currentClip.name : "neutral");
		if (customAnimation.currentClip.name != "neutral" && !customAnimation.isLoopOnBeat)
		{
			PlayExpression(expression);
		}
	}
}
