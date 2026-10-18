using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RDLevelEditor
{
	public class RDLevelData
	{
		public static RDLevelData current;

		public static bool decodingFailed;

		public static StringBuilder encode = new StringBuilder(100000);

		public static StringBuilder eventEncode = new StringBuilder(1000);

		public static bool encodingLevelEvents;

		public List<LevelEvent_Base> levelEvents;

		public List<LevelEvent_MakeRow> rows;

		public List<LevelEvent_MakeDecorationBase> decorations;

		public List<Conditional> conditionals = new List<Conditional>();

		public List<BookmarkData> bookmarks = new List<BookmarkData>();

		public RDLevelSettings settings;

		public string[] colorPalette = new string[21];

		public int errorStatus;

		public LevelErrorName errorName;

		public bool usesWindowDance;

		public bool resizesWindow;

		public int windowCount = 1;

		public RDLevelData()
		{
			settings = new RDLevelSettings(68);
			rows = new List<LevelEvent_MakeRow>();
			decorations = new List<LevelEvent_MakeDecorationBase>();
			levelEvents = new List<LevelEvent_Base>();
			conditionals = new List<Conditional>();
			bookmarks = new List<BookmarkData>();
			colorPalette = new string[21];
		}

		public RDLevelData(RDLevelSettings settings, List<LevelEvent_MakeRow> rows, List<LevelEvent_Base> levelEvents, List<Conditional> conditionals, List<LevelEvent_MakeDecorationBase> decorations, List<BookmarkData> bookmarks, string[] colorPalette)
		{
			this.settings = settings;
			this.rows = rows;
			this.decorations = decorations;
			this.levelEvents = levelEvents;
			this.conditionals = conditionals;
			this.bookmarks = bookmarks;
			this.colorPalette = colorPalette;
		}

		public RDLevelData(Dictionary<string, object> rootDict, bool onlyActiveEvents, bool onlySettings = false, bool forceLatestVersion = false)
			: this()
		{
			errorName = LevelErrorName.None;
			current = this;
			if (rootDict.ContainsKey("settings"))
			{
				try
				{
					Dictionary<string, object> dict = rootDict["settings"] as Dictionary<string, object>;
					settings.Decode(dict);
				}
				catch
				{
					Debug.LogWarning("Error loading RDLevelSettings");
					decodingFailed = true;
					errorName = LevelErrorName.LevelSettingsNull;
					return;
				}
			}
			else
			{
				Debug.Log("THIS IS r1??? Calling setup.");
				settings = new RDLevelSettings(1);
			}
			if (forceLatestVersion)
			{
				settings.version = 68;
			}
			foreach (Dictionary<string, object> item2 in rootDict["rows"] as List<object>)
			{
				LevelEvent_MakeRow levelEvent_MakeRow = new LevelEvent_MakeRow();
				levelEvent_MakeRow.Decode(item2);
				levelEvents.Add(levelEvent_MakeRow);
				rows.Add(levelEvent_MakeRow);
			}
			if (rootDict.ContainsKey("decorations"))
			{
				foreach (Dictionary<string, object> item3 in rootDict["decorations"] as List<object>)
				{
					string text = ((!item3.ContainsKey("type")) ? LevelEventType.Sprite.ToString() : (item3["type"] as string));
					LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase = (LevelEvent_MakeDecorationBase)Activator.CreateInstance(Type.GetType("RDLevelEditor.LevelEvent_" + text));
					levelEvent_MakeDecorationBase.Decode(item3);
					levelEvents.Add(levelEvent_MakeDecorationBase);
					decorations.Add(levelEvent_MakeDecorationBase);
				}
			}
			InspectorPanel_SetVFXPreset.legacyPresetsInLevel.Clear();
			bool flag = false;
			bool flag2 = false;
			foreach (Dictionary<string, object> item4 in rootDict["events"] as List<object>)
			{
				string text2 = "";
				foreach (string key in item4.Keys)
				{
					text2 += $"{key}: {item4[key]} | ";
				}
				LevelEvent_Base levelEvent_Base = null;
				string text4 = item4["type"] as string;
				if (text4 == "ReorderSprite" && settings.version < 68)
				{
					text4 = LevelEventType.ReorderDecoration.ToString();
				}
				string text5 = "RDLevelEditor.LevelEvent_" + text4;
				Type type = Type.GetType(text5);
				if (type == null || !type.IsSubclassOf(typeof(LevelEvent_Base)))
				{
					Debug.LogWarning("Error instancing LevelEvent class: " + text5);
					continue;
				}
				levelEvent_Base = (LevelEvent_Base)Activator.CreateInstance(type);
				levelEvent_Base.Decode(item4);
				if (levelEvent_Base.usesWindowDance && levelEvent_Base.active)
				{
					int num = 0;
					num = ((levelEvent_Base is LevelEvent_NewWindowDance levelEvent_NewWindowDance) ? levelEvent_NewWindowDance.window : ((levelEvent_Base is LevelEvent_WindowResize levelEvent_WindowResize) ? levelEvent_WindowResize.window : ((levelEvent_Base is LevelEvent_SetWindowContent levelEvent_SetWindowContent) ? levelEvent_SetWindowContent.window : ((levelEvent_Base is LevelEvent_RenameWindow levelEvent_RenameWindow) ? levelEvent_RenameWindow.window : ((levelEvent_Base is LevelEvent_HideWindow levelEvent_HideWindow) ? levelEvent_HideWindow.window : 0)))));
					windowCount = Mathf.Max(windowCount, num + 1);
					usesWindowDance = true;
				}
				if (levelEvent_Base.active || !onlyActiveEvents)
				{
					levelEvents.Add(levelEvent_Base);
					if ((levelEvent_Base is LevelEvent_PlaySong) & onlySettings)
					{
						decodingFailed = false;
						return;
					}
				}
				if (levelEvent_Base.active)
				{
					if (settings.version < 62 && !flag && !string.IsNullOrWhiteSpace(levelEvent_Base?.tag))
					{
						flag = true;
					}
					if (settings.version < 67 && !flag2 && levelEvent_Base is LevelEvent_SetVFXPreset && (levelEvent_Base as LevelEvent_SetVFXPreset).duration > 0f)
					{
						flag2 = true;
					}
				}
			}
			if (rootDict.TryGetValue("conditionals", out var value))
			{
				List<object> obj2 = value as List<object>;
				_ = GC.conditionalsInfo;
				foreach (Dictionary<string, object> item5 in obj2)
				{
					try
					{
						conditionals.Add(Conditional.Decode(item5));
					}
					catch (Exception exception)
					{
						Debug.LogWarning("Error adding conditional, see following exception:");
						Debug.LogException(exception);
						decodingFailed = true;
						errorName = LevelErrorName.LevelDataNull;
						return;
					}
				}
			}
			if (rootDict.ContainsKey("bookmarks"))
			{
				foreach (Dictionary<string, object> item6 in rootDict["bookmarks"] as List<object>)
				{
					try
					{
						BarAndBeat newBarAndBeat = new BarAndBeat((int)item6["bar"], Convert.ToSingle(item6["beat"]));
						BookmarkData item = new BookmarkData(newBarAndBeat, (int)item6["color"]);
						bookmarks.Add(item);
					}
					catch (Exception ex)
					{
						Debug.LogError("Error adding bookmarks " + ex.ToString());
						decodingFailed = true;
						errorName = LevelErrorName.LevelEventsNull;
						return;
					}
				}
			}
			if (rootDict.ContainsKey("colorPalette"))
			{
				List<object> obj3 = rootDict["colorPalette"] as List<object>;
				int num2 = 0;
				foreach (string item7 in obj3)
				{
					try
					{
						colorPalette[num2] = RDEditorUtils.DecodeColor(item7, hasAlpha: true);
						num2++;
						if (num2 >= 21)
						{
							break;
						}
					}
					catch (Exception ex2)
					{
						Debug.LogError("Error adding color palette colors " + ex2.ToString());
						decodingFailed = true;
						errorName = LevelErrorName.LevelEventsNull;
						return;
					}
				}
			}
			else
			{
				colorPalette = RDEditorConstants.defaultColorPalette;
			}
			string[] mods = settings.mods;
			List<string> list = ((mods == null) ? new List<string>() : mods.ToList());
			if (settings.version < 47)
			{
				list.Add("booleansDefaultToTrue");
			}
			if (settings.version < 49)
			{
				list.Add("classicHitParticles");
			}
			if (flag2)
			{
				list.Add("oldVFXEasing");
			}
			if (flag)
			{
				if (settings.version < 53)
				{
					list.Add("legacyTaggedEvents");
				}
				if (settings.version < 62)
				{
					list.Add("runTaggedEventsWhileScrubbing");
				}
			}
			settings.mods = list.ToArray();
			levelEvents = levelEvents.OrderBy((LevelEvent_Base o) => o.sortOrder).ToList();
			decodingFailed = false;
		}

		private void FixFloatingAndAdvanceLevelEvents()
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<LevelEvent_FloatingText> list3 = new List<LevelEvent_FloatingText>();
			List<LevelEvent_AdvanceText> list4 = new List<LevelEvent_AdvanceText>();
			foreach (LevelEvent_Base levelEvent in levelEvents)
			{
				if (levelEvent is LevelEvent_FloatingText levelEvent_FloatingText)
				{
					list3.Add(levelEvent_FloatingText);
					list.Add(levelEvent_FloatingText.id);
				}
				if (levelEvent is LevelEvent_AdvanceText levelEvent_AdvanceText)
				{
					list4.Add(levelEvent_AdvanceText);
					list2.Add(levelEvent_AdvanceText.id);
				}
			}
			List<int> list5 = list.Distinct().ToList();
			List<int> list6 = list2.Distinct().ToList();
			List<int> list7 = new List<int>();
			foreach (int item in list6)
			{
				if (!list5.Contains(item))
				{
					list7.Add(item);
				}
			}
			foreach (LevelEvent_AdvanceText item2 in list4)
			{
				if (list7.Contains(item2.id))
				{
					RDBaseDllDummy.printes("advanceTextToDelete: " + item2.id);
					levelEvents.Remove(item2);
				}
			}
			if (list5.Count == list.Count)
			{
				return;
			}
			RDBaseDllDummy.printes("All FloatingTexts id's are not unique");
			List<int> list8 = new List<int>();
			foreach (LevelEvent_FloatingText item3 in list3)
			{
				RDBaseDllDummy.printes("fe.id = " + item3.id);
				if (list8.Contains(item3.id))
				{
					int num = -1;
					do
					{
						num++;
					}
					while (list5.Contains(num));
					RDBaseDllDummy.printes("reassigning to: " + num);
					item3.id = num;
				}
				list8.Add(item3.id);
			}
		}

		public string Encode()
		{
			DateTime utcNow = DateTime.UtcNow;
			_ = Environment.NewLine;
			RDEditorUtils.IndentationLevel = 0;
			encodingLevelEvents = false;
			encode.Length = 0;
			encode.Append(RDEditorUtils.OpenDictionary());
			RDEditorUtils.IndentationLevel = 1;
			encode.Append(RDEditorUtils.OpenDictionary("settings"));
			encode.Append(settings.Encode());
			encode.Append(RDEditorUtils.CloseDictionary());
			encode.Append(RDEditorUtils.OpenArray("rows"));
			int num = 0;
			foreach (LevelEvent_MakeRow row in rows)
			{
				RDEditorUtils.IndentationLevel = 2;
				row.row = num;
				encode.Append(RDEditorUtils.Indentation + "{ " + row.Encode() + " }," + Environment.NewLine);
				num++;
			}
			if (num > 0)
			{
				encode.Length -= Environment.NewLine.Length + 1;
				encode.Append(Environment.NewLine);
			}
			RDEditorUtils.IndentationLevel = 1;
			encode.Append(RDEditorUtils.CloseArray());
			encode.Append(RDEditorUtils.OpenArray("decorations"));
			int num2 = 0;
			foreach (LevelEvent_MakeDecorationBase decoration in decorations)
			{
				RDEditorUtils.IndentationLevel = 2;
				decoration.row = num2;
				encode.Append(RDEditorUtils.Indentation + "{ " + decoration.Encode() + " }," + Environment.NewLine);
				num2++;
			}
			if (num2 > 0)
			{
				encode.Length -= Environment.NewLine.Length + 1;
				encode.Append(Environment.NewLine);
			}
			RDEditorUtils.IndentationLevel = 1;
			encode.Append(RDEditorUtils.CloseArray());
			encodingLevelEvents = true;
			encode.Append(RDEditorUtils.OpenArray("events"));
			int num3 = 0;
			foreach (LevelEvent_Base levelEvent in levelEvents)
			{
				if (!levelEvent.isBaseEvent && !(levelEvent is LevelEvent_MakeRow) && !(levelEvent is LevelEvent_MakeDecorationBase))
				{
					RDEditorUtils.IndentationLevel = 2;
					encode.AppendMany(RDEditorUtils.Indentation, "{ ");
					RDEditorUtils.IndentationLevel = 0;
					string value = levelEvent.Encode();
					if (levelEvent == null || string.IsNullOrEmpty(value))
					{
						Debug.Log("Error encoding event.");
						continue;
					}
					encode.Append(value);
					RDEditorUtils.IndentationLevel = 2;
					encode.AppendMany(" },", Environment.NewLine);
					num3++;
				}
			}
			if (num3 > 0)
			{
				encode.Length -= Environment.NewLine.Length + 1;
				encode.Append(Environment.NewLine);
			}
			RDEditorUtils.IndentationLevel = 1;
			encode.Append(RDEditorUtils.CloseArray());
			encodingLevelEvents = false;
			encode.Append(RDEditorUtils.OpenArray("conditionals"));
			bool flag = false;
			foreach (Conditional conditional in conditionals)
			{
				RDEditorUtils.IndentationLevel = 2;
				encode.AppendMany(RDEditorUtils.Indentation, "{ ");
				RDEditorUtils.IndentationLevel = 0;
				string value2 = conditional.Encode();
				if (!string.IsNullOrEmpty(value2))
				{
					encode.Append(value2);
				}
				RDEditorUtils.IndentationLevel = 2;
				encode.AppendMany("},", Environment.NewLine);
				flag = true;
			}
			if (flag)
			{
				encode.Length -= Environment.NewLine.Length + 1;
				encode.Append(Environment.NewLine);
			}
			RDEditorUtils.IndentationLevel = 1;
			encode.Append(RDEditorUtils.CloseArray());
			encode.Append(RDEditorUtils.OpenArray("bookmarks"));
			bool flag2 = false;
			foreach (BookmarkData bookmark in bookmarks)
			{
				RDEditorUtils.IndentationLevel = 2;
				encode.AppendMany(RDEditorUtils.Indentation, "{ ");
				RDEditorUtils.IndentationLevel = 0;
				BarAndBeat barAndBeat = bookmark.barAndBeat;
				string text = RDEditorUtils.EncodeInt("bar", barAndBeat.bar);
				barAndBeat = bookmark.barAndBeat;
				string value3 = text + RDEditorUtils.EncodeFloat("beat", barAndBeat.beat) + RDEditorUtils.EncodeInt("color", bookmark.colorIndex);
				if (!string.IsNullOrEmpty(value3))
				{
					encode.Append(value3);
				}
				RDEditorUtils.IndentationLevel = 2;
				encode.AppendMany("},", Environment.NewLine);
				flag2 = true;
			}
			if (flag2)
			{
				encode.Length -= Environment.NewLine.Length + 1;
				encode.Append(Environment.NewLine);
			}
			RDEditorUtils.IndentationLevel = 1;
			encode.Append(RDEditorUtils.CloseArray());
			string newLine = Environment.NewLine;
			encode.Append(RDEditorUtils.OpenArray("colorPalette"));
			RDEditorUtils.IndentationLevel = 2;
			for (int i = 0; i < 21; i++)
			{
				string text2 = ((i != 20) ? "," : "");
				encode.Append(RDEditorUtils.Indentation + "\"" + colorPalette[i] + "\"" + text2 + newLine);
			}
			RDEditorUtils.IndentationLevel = 1;
			encode.Append(RDEditorUtils.CloseArray(isLastMember: true));
			RDEditorUtils.IndentationLevel = 0;
			encode.Append(RDEditorUtils.CloseDictionary(isLastMember: true));
			_ = (DateTime.UtcNow - utcNow).TotalSeconds;
			return encode.ToString();
		}

		public float GetFirstSongOffset()
		{
			float result = 0f;
			foreach (LevelEvent_Base levelEvent in levelEvents)
			{
				if (levelEvent is LevelEvent_PlaySong levelEvent_PlaySong)
				{
					result = levelEvent_PlaySong.song.offset;
				}
			}
			return result;
		}

		public string GetFirstSongFileName()
		{
			foreach (LevelEvent_Base levelEvent in levelEvents)
			{
				if (levelEvent is LevelEvent_PlaySong levelEvent_PlaySong)
				{
					return levelEvent_PlaySong.song.filename;
				}
			}
			return null;
		}

		public float GetSongBPM(string songName)
		{
			if (levelEvents == null)
			{
				return -1f;
			}
			foreach (LevelEvent_Base levelEvent in levelEvents)
			{
				if (levelEvent is LevelEvent_PlaySong levelEvent_PlaySong && levelEvent_PlaySong.song.filename == songName)
				{
					return levelEvent_PlaySong.beatsPerMinute;
				}
			}
			foreach (LevelEvent_Base levelEvent2 in levelEvents)
			{
				if (levelEvent2 is LevelEvent_PlaySong { beatsPerMinute: var beatsPerMinute })
				{
					return beatsPerMinute;
				}
			}
			foreach (LevelEvent_Base levelEvent3 in levelEvents)
			{
				if (levelEvent3 is LevelEvent_SetBeatsPerMinute { beatsPerMinute: var beatsPerMinute2 })
				{
					return beatsPerMinute2;
				}
			}
			return 100f;
		}
	}
}
