using System.Collections;
using RDLevelEditor;
using RDTools;

public class Level_Tutorial_3 : LevelBase
{
	private string dialogueName = "diaTutorial3";

	public Level_Tutorial_3(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		songsUsed = new string[1] { "sndTutorial" };
		levelType = LevelType.Tutorial;
		levelToSkipTo = Level.Classy;
	}

	public override IEnumerator LoadBigAssets()
	{
		RDUtils.ShowTutorialTheme("3CC349".HexToColor(), "2DAD18".HexToColor());
		base.ink.Load(dialogueName);
		yield return null;
	}

	public override void preactions()
	{
		int num = base.conductor.barNumber;
		if (num != 1)
		{
			_ = 2;
			return;
		}
		RDClassDll.printem("here");
		base.conductor.SetBPM(88f);
		PlayMusic(songsUsed[0]);
	}

	public override void actions()
	{
		if (base.conductor.barNumber != 1)
		{
			return;
		}
		base.ink.Run("Tutorial0", () =>
		{
			Timer.Add(() =>
			{
				base.ink.Run("Tutorial1", () =>
				{
					FadeOutMusic();
					Timer.Add(() =>
					{
						scnBase.GoToLevelWithEnum(levelToSkipTo);
					}, 0.5f, ignoreTimeScale: false);
				});
			}, 2f, ignoreTimeScale: false);
		});
	}
}
