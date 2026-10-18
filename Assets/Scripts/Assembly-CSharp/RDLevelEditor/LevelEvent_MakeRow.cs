using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, -10, RoomsUsage.OneRoom, false, false, false, false, 0, false)]
	public class LevelEvent_MakeRow : LevelEvent_Base
	{
		public SoundData pulseSound;

		public bool mimicsRow;

		public string customCharacterName;

		public bool failedLoadingCustomCharacter;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public RowType rowType { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		public RDPlayer player { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Character character { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableCpuMarkerIf", null)]
		public Character cpuMarker { get; set; } = Character.Otto;

		[JsonProperty("", "", null, "", false, true, "EnableHideAtStartIf", null)]
		public bool hideAtStart { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableMimicsRowIf", null)]
		public int rowToMimic { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableMuteBeatsIf", null)]
		public bool muteBeats { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableMutedIn1PIf", null)]
		public bool muteIn1P { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableLengthIf", null)]
		[IntInfo(1, 7)]
		[Off]
		public int? length { get; set; } = 7;

		public bool muted
		{
			get
			{
				if (!muteBeats)
				{
					if (muteIn1P)
					{
						return !GC.twoPlayerMode;
					}
					return false;
				}
				return true;
			}
		}

		public bool EnableCpuMarkerIf()
		{
			return cpuMarker != Character.Otto;
		}

		public bool EnableHideAtStartIf()
		{
			return hideAtStart;
		}

		public bool EnableMimicsRowIf()
		{
			return mimicsRow;
		}

		public bool EnableMuteBeatsIf()
		{
			return muteBeats;
		}

		public bool EnableMutedIn1PIf()
		{
			return muteIn1P;
		}

		public bool EnableLengthIf()
		{
			if (length < 7)
			{
				return length > 0;
			}
			return false;
		}

		public override IEnumerator Prepare()
		{
			if (character == Character.Custom)
			{
				UpdateCustomCharacter(justReturnErrors: false);
				UpdateCustomCharacter(justReturnErrors: false, editorRowHeader: true);
				if (!failedLoadingCustomCharacter)
				{
					yield return PrepareCustomCharacter(customCharacterName);
				}
			}
			if (pulseSound == null)
			{
				Debug.Log("error: missing pulse data");
				pulseSound = new SoundData("Shaker", "pulseSound", "pulseSoundVolume", "pulseSoundPitch", "pulseSoundPan", "pulseSoundOffset");
			}
			yield return pulseSound.Prepare();
			prepared = true;
			CreateRow();
		}

		private void CreateRow()
		{
			if (base.row >= 8 || !RDLevelData.current.settings.createRowsManually)
			{
				RDPlayer singleplayer = ((player == RDPlayer.CPU) ? RDPlayer.CPU : RDPlayer.P1);
				bool animate = base.game.barToScrubTo <= base.conductor.barNumber;
				RDPlayer multiplayer = player;
				if (player == RDPlayer.CPU && LevelEvent_Base.level.cpuIsP2On2P && GC.twoPlayerMode)
				{
					multiplayer = RDPlayer.P2;
				}
				base.game.rows[base.row].cpuCharacter = cpuMarker;
				base.game.MakeRow(rowType, base.row, 1f, character, singleplayer, multiplayer, mimicsRow ? rowToMimic : (-1), mimicsRow, base.room, animate, null, customCharacterName, present: false, hideAtStart);
				if (base.game.currentLevel.GetType() == typeof(LevelBase) && character == Character.Boy)
				{
					base.game.rows[base.row].ent.character.happyAnimName = "happySimple";
				}
				string filename = "";
				if (pulseSound.externalClip || pulseSound.filename != "None")
				{
					filename = pulseSound.conductorFilename;
				}
				pulseSound.minPitch = (pulseSound.maxPitch = pulseSound.pitch);
				pulseSound.filename = filename;
				base.game.rows[base.row].SetPulseSounds(pulseSound);
				base.game.rows[base.row].ent.SetRowLength(length ?? 7, 0f);
			}
		}

		public override void Init()
		{
			base.Init();
			pulseSound = new SoundData("Shaker", "pulseSound", "pulseSoundVolume", "pulseSoundPitch", "pulseSoundPan", "pulseSoundOffset");
			usesY = false;
		}

		public override string Encode()
		{
			if (RDLevelData.encodingLevelEvents)
			{
				return null;
			}
			RDEditorUtils.IndentationLevel = 0;
			string text = base.Encode();
			if (character == Character.Custom)
			{
				text = text.Replace(character.ToString(), "custom:" + customCharacterName);
			}
			return text + ", " + pulseSound.Encode(isLastValue: true);
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			string text = dict["character"] as string;
			if (text.StartsWith("custom:"))
			{
				dict["character"] = Character.Custom.ToString();
				customCharacterName = text.Replace("custom:", "");
				if (base.editorMode)
				{
					UpdateCustomCharacter(justReturnErrors: true);
					UpdateCustomCharacter(justReturnErrors: false, editorRowHeader: true);
				}
			}
			base.Decode(dict);
			pulseSound.Decode(dict);
			mimicsRow = dict.ContainsKey("rowToMimic");
		}

		public static IEnumerator PrepareCustomCharacter(string customCharacterName)
		{
			string currentLevelFolderPath = RDEditorUtils.GetCurrentLevelFolderPath();
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			string path = currentLevelFolderPath + directorySeparatorChar + customCharacterName;
			string charFolderName = (RDDirectory.Exists(path) ? customCharacterName : "");
			List<string> list = RDEditorUtils.AnimationClipNamesFromCustomCharacterName(customCharacterName, charFolderName);
			List<string> list2 = new List<string> { customCharacterName };
			foreach (string item in list)
			{
				list2.Add(customCharacterName + "_" + item);
			}
			foreach (string item2 in list2)
			{
				string filename = item2 + ".wav";
				string filename2 = item2 + ".ogg";
				if (RDEditorUtils.FindClip(filename, suppressWarning: true, charFolderName) == RDAudioLoadType.SuccessExternalClipLoaded)
				{
					yield return RDEditorUtils.AudioClipFromFilename(filename, suppressWarning: true, charFolderName);
				}
				else if (RDEditorUtils.FindClip(filename2, suppressWarning: true, charFolderName) == RDAudioLoadType.SuccessExternalClipLoaded)
				{
					yield return RDEditorUtils.AudioClipFromFilename(filename2, suppressWarning: true, charFolderName);
				}
			}
		}

		public void UpdateCustomCharacter(bool justReturnErrors, bool editorRowHeader = false)
		{
			if ((!editorRowHeader || !(base.editor == null)) && !UpdateCustomCharacter(customCharacterName, justReturnErrors, editorRowHeader))
			{
				character = Character.Samurai;
				failedLoadingCustomCharacter = true;
			}
		}

		public static bool UpdateCustomCharacter(string customCharacterName, bool justReturnErrors, bool useEditorData = false, bool fromCustomCharactersFolder = false)
		{
			string currentLevelFolderPath = RDEditorUtils.GetCurrentLevelFolderPath();
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			string path = currentLevelFolderPath + directorySeparatorChar + customCharacterName;
			string subFolder = ((fromCustomCharactersFolder || RDDirectory.Exists(path)) ? customCharacterName : "");
			Texture2D texture2D = RDEditorUtils.LoadLevelTexture(scnGame.levelToLoadSource, customCharacterName + ".png", subFolder, fromCustomCharactersFolder);
			string text = RDEditorUtils.LoadLevelText(scnGame.levelToLoadSource, customCharacterName + ".json", subFolder, fromCustomCharactersFolder);
			if (texture2D == null || string.IsNullOrEmpty(text))
			{
				return false;
			}
			Texture2D outlineTexture = RDEditorUtils.LoadLevelTexture(scnGame.levelToLoadSource, customCharacterName + "_outline.png", subFolder, fromCustomCharactersFolder, isAlpha8: true);
			Texture2D glowTexture = RDEditorUtils.LoadLevelTexture(scnGame.levelToLoadSource, customCharacterName + "_glow.png", subFolder, fromCustomCharactersFolder, isAlpha8: true);
			Texture2D freezeTexture = RDEditorUtils.LoadLevelTexture(scnGame.levelToLoadSource, customCharacterName + "_freeze.png", subFolder, fromCustomCharactersFolder, isAlpha8: true);
			texture2D.filterMode = FilterMode.Point;
			texture2D.wrapMode = TextureWrapMode.Clamp;
			List<CustomAnimationData.JsonError> list;
			if (justReturnErrors)
			{
				CustomAnimationData customAnimationData = default;
				customAnimationData.LoadFromJson(text);
				list = customAnimationData.jsonErrors;
			}
			else
			{
				list = LevelBase.LoadCustomCharacter(useEditorData ? scnEditor.instance.customCharacterData : scnGame.instance.currentLevel.customCharacterData, customCharacterName, text, texture2D, outlineTexture, glowTexture, freezeTexture);
			}
			if (list.Count > 0)
			{
				RDBaseDllDummy.printes("Failed loading custom character");
				return false;
			}
			return true;
		}

		public override void Run()
		{
			bool animate = base.game.barToScrubTo <= base.conductor.barNumber;
			RowEntity ent = base.game.rows[base.row].ent;
			ent.Present(animate);
			if (hideAtStart)
			{
				ent.Hide(keepCharacterVisible: false, effects: false);
			}
		}

		public string GetRowString(bool shortName = false)
		{
			if (base.editor.rowsData == null || base.editor.rowsData.Count == 0)
			{
				return string.Empty;
			}
			string arg = ((character == Character.Custom) ? customCharacterName : (shortName ? RDString.Get($"enum.Character.{character}.short") : RDString.GetEnumValue(character)));
			return $"{base.row + 1}. {arg}";
		}
	}
}
