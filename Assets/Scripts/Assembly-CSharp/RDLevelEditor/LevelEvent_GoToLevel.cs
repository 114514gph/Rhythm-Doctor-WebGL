using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_GoToLevel : LevelEvent_Base
	{
		public string mainLevelName;

		public bool isSkipping;

		[JsonProperty("", "", "editor.RenameWindow.action", "", false, true, null, null)]
		[ToggleGroup(false, null, null, new string[] { "SetNext", "LoadNext", "LoadImmediately" }, true, 14)]
		public GoToLevelAction action { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableSettingsIf", null)]
		[Dropdown(null, true, true)]
		public string rdlevel { get; set; } = "";

		[JsonProperty("", "", null, "", false, true, "EnableSkipIf", null)]
		[Checkbox]
		public bool skippable { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableCheckboxesIf", null)]
		[Checkbox]
		public bool fadeOut { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, "EnableCheckboxesIf", null)]
		[Checkbox]
		public bool startImmediately { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableCheckboxesIf", null)]
		[Checkbox]
		public bool keepMistakes { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableCheckboxesIf", null)]
		[Checkbox]
		public bool dontUpdateRestart { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableLoadDescIf", null)]
		[Description("editor.GoToLevel.loadDescription", 42, 8, TextAnchor.MiddleCenter)]
		public bool description => false;

		public bool EnableSettingsIf()
		{
			return action != GoToLevelAction.LoadNext;
		}

		public bool EnableCheckboxesIf()
		{
			if (EnableSettingsIf())
			{
				return !rdlevel.IsNullOrEmpty();
			}
			return false;
		}

		public bool EnableSkipIf()
		{
			if (EnableCheckboxesIf())
			{
				return action == GoToLevelAction.SetNext;
			}
			return false;
		}

		public bool EnableLoadDescIf()
		{
			return !EnableSettingsIf();
		}

		public override string Encode()
		{
			if (RDUtils.GetRDLevelFilename(rdlevel) == mainLevelName && !mainLevelName.IsNullOrEmpty())
			{
				string text = rdlevel;
				rdlevel = "main.rdlevel";
				string result = base.Encode();
				rdlevel = text;
				mainLevelName = null;
				return result;
			}
			return base.Encode();
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			rdlevel = RDUtils.GetRDLevelFilename(rdlevel);
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (action == GoToLevelAction.SetNext)
				{
					bool flag = !rdlevel.IsNullOrEmpty();
					LevelEvent_Base.level.queuedNextLevel = (flag ? this : null);
					LevelEvent_Base.level.skippable = flag && skippable;
				}
				else if (action == GoToLevelAction.LoadNext)
				{
					LevelEvent_Base.level.queuedNextLevel?.PrepNextLevel();
				}
				else
				{
					PrepNextLevel();
				}
			});
		}

		public void PrepNextLevel()
		{
			if (rdlevel.IsNullOrEmpty())
			{
				return;
			}
			if (base.conductor.isScrubbing)
			{
				if (LevelEvent_Base.level.scrubbedGoToLevel == null)
				{
					LevelEvent_Base.level.scrubbedGoToLevel = this;
				}
				return;
			}
			string levelDirectory = scnGame.levelPersistence.GetLevelDirectory();
			string levelPath = Path.Combine(levelDirectory, RDUtils.GetRDLevelFilename(rdlevel));
			printe("Prepping next level...");
			if (levelDirectory.IsNullOrEmpty() || !File.Exists(levelPath))
			{
				ShowLoadFailed(levelPath, "editor.PublishLevel.missingFile");
				return;
			}
			if (base.editorMode)
			{
				string fileName = Path.GetFileName(levelPath);
				if (!base.editor.FileIsSaved())
				{
					base.editor.CheckIfNeedsToSaveFile(() =>
					{
						FadeToCustomLevel(levelPath);
					}, RDString.Get("editor.dialog.goToLevelCalledUnsaved").Replace("[filename]", fileName));
				}
				else
				{
					base.editor.dialog.Show(RDString.Get("editor.dialog.goToLevelCalled").Replace("[filename]", fileName), new string[2]
					{
						RDString.Get("editor.PublishLevel.dialogOK"),
						RDString.Get("editor.dialog.cancel")
					}, (int option) =>
					{
						base.editor.dialog.Hide();
						if (option == 0)
						{
							FadeToCustomLevel(levelPath);
						}
					});
				}
			}
			else
			{
				FadeToCustomLevel(levelPath);
			}
			scnEditor.PauseIfUnpaused();
			if (base.conductor.isScrubbing)
			{
				base.editor.playAsSoonItReloads = false;
			}
		}

		private void FadeToCustomLevel(string levelPath)
		{
			if (isSkipping)
			{
				base.game.SkipLevelWithCallback(() =>
				{
					GoToCustomLevel(levelPath);
				});
			}
			else if (fadeOut)
			{
				scnEditor.UnpauseIfPaused();
				base.vfx.FadeOut();
				LevelEvent_Base.level.FadeOutMusic();
				Timer.Add(() =>
				{
					GoToCustomLevel(levelPath);
				}, 1f, ignoreTimeScale: false);
			}
			else if (base.editorMode)
			{
				Timer.Add(() =>
				{
					GoToCustomLevel(levelPath);
				}, 0.05f);
			}
			else
			{
				GoToCustomLevel(levelPath);
			}
		}

		private void GoToCustomLevel(string levelPath)
		{
			LevelPersistence levelPersistence = scnGame.levelPersistence.Setup(levelPath);
			if (startImmediately)
			{
				levelPersistence.startImmediately = true;
			}
			if (keepMistakes)
			{
				levelPersistence.mistakes = base.game.mistakesManager;
				levelPersistence.restartLevelMistakes = (dontUpdateRestart ? (levelPersistence.restartLevelMistakes ?? base.game.mistakesAtStart) : base.game.mistakesManager.Clone());
			}
			levelPersistence.restartLevelPath = (dontUpdateRestart ? (levelPersistence.restartLevelPath ?? levelPersistence.GetLevelPath()) : null);
			if (base.editorMode)
			{
				base.editor.OpenFileWithPath(levelPath, updateLastFile: false);
			}
			else
			{
				scnBase.GoToLevel(levelPath);
			}
		}

		private void ShowLoadFailed(string levelPath, string errorStr)
		{
			if (base.editorMode)
			{
				scnEditor.PauseIfUnpaused();
				base.editor.dialog.Show(RDString.Get(errorStr).Replace("[filename]", Path.GetFileName(levelPath)), new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
				{
					base.editor.dialog.Hide();
				});
			}
			else
			{
				scnBase.GoToLevelSelect();
			}
		}

		public override string GetTooltipText()
		{
			if (EnableCheckboxesIf())
			{
				return RDString.GetEnumValue(action) + "\n" + rdlevel;
			}
			return RDString.GetEnumValue(action);
		}
	}
}
