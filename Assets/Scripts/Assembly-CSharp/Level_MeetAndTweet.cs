using System.Collections;
using System.Collections.Generic;
using RDLevelEditor;
using UnityEngine;

public class Level_MeetAndTweet : LevelBase
{
	public enum StoryMode
	{
		Left = 0,
		Right = 1,
		Large = 2
	}

	private string currentStory;

	private StoryMode currentMode;

	private int currentIndex;

	private int individualWrenIndex;

	private int individualCockatielIndex;

	private List<LevelEvent_MakeDecorationBase> decorationData;

	public Level_MeetAndTweet(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		decorationData = RDLevelData.current.decorations;
		yield return null;
	}

	public void SetCurrentStory(string story, bool leftSide, int index)
	{
		currentStory = story;
		currentMode = ((!leftSide) ? StoryMode.Right : StoryMode.Left);
		currentIndex = index;
	}

	public void SetCenterStory(string story, int index)
	{
		currentStory = story;
		currentMode = StoryMode.Large;
		currentIndex = index;
	}

	public void AdvanceCurrentStory()
	{
		currentIndex++;
	}

	public void SwapCurrentStory(bool reset)
	{
		if (currentMode == StoryMode.Left)
		{
			currentMode = StoryMode.Right;
		}
		else
		{
			currentMode = StoryMode.Left;
		}
		if (reset)
		{
			currentIndex = 1;
		}
	}

	public void ShowCurrentStory()
	{
		ShowStory(currentStory, currentMode, currentIndex);
	}

	public void MistakeCurrentStory(float minAngle, float maxAngle, float minShift, float maxShift)
	{
		ShowMistake(currentStory, currentMode, currentIndex, minAngle, maxAngle, minShift, maxShift);
	}

	public void ClearCurrentStory()
	{
		ClearStory(currentStory);
	}

	public void SetIndividualStory(string story)
	{
		currentStory = story;
		individualWrenIndex = 1;
		individualCockatielIndex = 1;
	}

	public void ShowIndividualStory(bool leftSide)
	{
		ShowStory(currentStory, (!leftSide) ? StoryMode.Right : StoryMode.Left, leftSide ? individualWrenIndex : individualCockatielIndex);
	}

	public void MistakeIndividualStory(bool leftSide, float minAngle, float maxAngle, float minShift, float maxShift)
	{
		ShowMistake(currentStory, (!leftSide) ? StoryMode.Right : StoryMode.Left, leftSide ? individualWrenIndex : individualCockatielIndex, minAngle, maxAngle, minShift, maxShift);
	}

	public void AdvanceIndividualStory(bool leftSide)
	{
		if (leftSide)
		{
			individualWrenIndex++;
		}
		else
		{
			individualCockatielIndex++;
		}
	}

	public void ShowStory(string story, StoryMode storyMode, int index)
	{
		GrabAndActivateStoryDec(story, storyMode, index);
	}

	private CustomSprite GrabAndActivateStoryDec(string story, StoryMode storyMode, int index)
	{
		CustomSprite customSprite = FindDec(story, storyMode, index);
		if (customSprite == null)
		{
			RDBaseDllDummy.printem("Decoration is null???");
			return null;
		}
		RDBaseDllDummy.printem($"showing story: {story}, {storyMode}, {index}, gameObject: {customSprite.gameObject.name}");
		if (storyMode != StoryMode.Large)
		{
			customSprite.transform.localScale = Vector3.one * 0.5f;
		}
		else
		{
			customSprite.transform.localScale = Vector3.one * 0.25f;
		}
		customSprite.gameObject.SetActive(value: true);
		return customSprite;
	}

	public void ClearStory(string story)
	{
		foreach (LevelEvent_MakeDecorationBase decorationDatum in decorationData)
		{
			if (decorationDatum is LevelEvent_Sprite levelEvent_Sprite && levelEvent_Sprite.filename.StartsWith(story))
			{
				sprites[decorationDatum.decorationId].gameObject.SetActive(value: false);
			}
		}
	}

	public void ShowMistake(string story, StoryMode storyMode, int index, float minAngle, float maxAngle, float minShift, float maxShift)
	{
		CustomSprite customSprite = GrabAndActivateStoryDec(story, storyMode, index);
		if (customSprite != null)
		{
			customSprite.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle) * Mathf.Sign(Random.Range(-1f, 1f)));
			float x = Random.Range(minShift, maxShift) * Mathf.Sign(Random.Range(-1f, 1f));
			float y = Random.Range(minShift, maxShift) * Mathf.Sign(Random.Range(-1f, 1f));
			customSprite.transform.localPosition += new Vector3(x, y, customSprite.transform.localPosition.z);
		}
	}

	private CustomSprite FindDec(string story, StoryMode storyMode, int index)
	{
		string text = "";
		switch (storyMode)
		{
		case StoryMode.Left:
			text = "A";
			break;
		case StoryMode.Right:
			text = "B";
			break;
		case StoryMode.Large:
			text = "C";
			break;
		}
		string text2 = story + "_" + text + index + ".png";
		foreach (LevelEvent_MakeDecorationBase decorationDatum in decorationData)
		{
			if (decorationDatum is LevelEvent_Sprite levelEvent_Sprite && levelEvent_Sprite.filename == text2)
			{
				return sprites[decorationDatum.decorationId];
			}
		}
		return null;
	}

	public void SetWobble(int wobblePerc)
	{
		float num = (float)wobblePerc / 100f;
		num /= 100f;
		base.room1.quadMaterial.SetFloat("_Wobble", num);
		base.room2.quadMaterial.SetFloat("_Wobble", num);
		base.room3.quadMaterial.SetFloat("_Wobble", num);
	}
}
