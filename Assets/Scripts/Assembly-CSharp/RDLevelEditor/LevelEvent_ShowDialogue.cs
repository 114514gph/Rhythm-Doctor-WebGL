using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RDTools;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.OneRoomOrOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_ShowDialogue : LevelEvent_Base
	{
		private const string inkStart = "[[ink:";

		private const string inkEnd = "]]";

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ShowDialogue]
		public string text { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[DontShow]
		public float speed { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, "EnableLocalizedIf", null)]
		[DontShow]
		public bool localized { get; set; }

		public Dictionary<string, string> localizedText { get; set; } = new Dictionary<string, string>();

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(true, null, null, null, false, 14)]
		public RDInk.PanelSide panelSide { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(true, null, null, null, false, 14)]
		public RDInk.PortraitSide portraitSide { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, null, false, 14)]
		public bool playTextSounds { get; set; } = true;

		public bool EnableLocalizedIf()
		{
			return localized;
		}

		public override IEnumerator Prepare()
		{
			if (!this.text.StartsWith("[[ink:"))
			{
				string key = RDString.language.ToString();
				if (localized && localizedText.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
				{
					this.text = value;
				}
				string[] array = this.text.Split('\n');
				string[] array2 = array;
				foreach (string text in array2)
				{
					int num = text.IndexOf(':');
					if (num == -1)
					{
						break;
					}
					string text2 = text.Substring(0, num).Trim();
					if (text2.HasImageFileExtension())
					{
						RDEditorUtils.LoadLevelTexture(LevelSource.ExternalPath, text2);
						continue;
					}
					string text3 = text2;
					int num2 = text3.IndexOf('_');
					if (num2 != -1)
					{
						text3 = text3.Substring(0, num2);
					}
					if (!Enum.TryParse<Character>(text3, out var _) && !LevelEvent_Base.level.customCharacterData.ContainsKey(text3) && LevelEvent_MakeRow.UpdateCustomCharacter(text3, justReturnErrors: false))
					{
						yield return LevelEvent_MakeRow.PrepareCustomCharacter(text3);
					}
				}
			}
			prepared = true;
			yield return null;
		}

		public override void Init()
		{
			base.Init();
			text = RDString.Get("editor.ShowDialogue.sampleText");
		}

		public override void OnCreate()
		{
			base.OnCreate();
			base.room = 4;
		}

		public override string Encode()
		{
			string text = base.Encode();
			if (localized)
			{
				foreach (string key in localizedText.Keys)
				{
					string text2 = localizedText[key];
					if (!string.IsNullOrEmpty(text2))
					{
						text = text + ", " + RDEditorUtils.EncodeUnicodeString("text" + key, text2, lastValue: true);
					}
				}
			}
			return text;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			bool num = !dict.ContainsKey("rooms");
			base.Decode(dict);
			if (num)
			{
				base.room = 4;
			}
			if (localized)
			{
				Array values = Enum.GetValues(typeof(SystemLanguage));
				localizedText = new Dictionary<string, string>();
				foreach (object item in values)
				{
					string text = item.ToString();
					string key = "text" + text;
					if (dict.ContainsKey(key))
					{
						string value = dict[key] as string;
						localizedText.Add(text, value);
					}
				}
			}
			if (RDLevelData.current.settings.version < 44)
			{
				playTextSounds = false;
			}
		}

		public override void Run()
		{
			if (this.text.StartsWith("[[ink:"))
			{
				RunOnBeat(() =>
				{
					RDInk rDInk = scrGameManager.instance.ink;
					int num = this.text.IndexOf("]]") + "]]".Length;
					string text2 = ((num == -1) ? this.text : this.text.Substring(0, num - "]]".Length));
					string text3 = text2.Substring("[[ink:".Length, text2.Length - "[[ink:".Length);
					string text4 = text3;
					try
					{
						text4 = base.game.currentLevel.EvaluateCurlyBracketsInString(text3);
					}
					catch (Exception ex2)
					{
						RDClassDll.printem("Using curly brackets but is not a variable: " + ex2.Message);
					}
					text3 = text4;
					List<string> list = text3.Split(',').ToList();
					string text5 = null;
					string text6 = null;
					rDInk.SetPanelRoom(base.room);
					if (list[0].Trim() == "this")
					{
						text6 = null;
						text5 = null;
					}
					else if (list.Count >= 2)
					{
						text6 = list[0].Trim();
						text5 = list[1];
					}
					else
					{
						text6 = "diaCutscenes";
						text5 = text3;
					}
					if (text5 != null)
					{
						text5 = text5.Trim();
					}
					rDInk.SetPanelSide(panelSide);
					if (playTextSounds)
					{
						rDInk.speechSource.enabled = true;
					}
					else
					{
						rDInk.speechSource.Stop();
						rDInk.speechSource.enabled = false;
					}
					if (text5 != null && text6 != null)
					{
						rDInk.Run(text6, text5);
					}
					else
					{
						string text7 = this.text.Substring(num);
						rDInk.RunInlineInk(text7);
					}
					if (list.Count != 2 || rDInk.type != RDInkType.Passive)
					{
						PlayStyleChange changeAfterDialogue = ((rDInk.type == RDInkType.Active) ? PlayStyleChange.OnNextBar : PlayStyleChange.None);
						PlayStyleChange changeAtStart = ((rDInk.type == RDInkType.Active) ? PlayStyleChange.Prolong : PlayStyleChange.OnNextBar);
						if (text6 == null)
						{
							list.Insert(1, null);
						}
						if (list.Count == 4)
						{
							if (list[3] == "Immediate")
							{
								list[3] = "Immediately";
							}
							changeAtStart = RDUtils.ParseEnum(list[2].Trim(), (rDInk.type == RDInkType.Active) ? PlayStyleChange.Prolong : PlayStyleChange.OnNextBar);
							changeAfterDialogue = RDUtils.ParseEnum(list[3].Trim(), PlayStyleChange.OnNextBar);
						}
						else if (list.Count == 3)
						{
							if (list[2] == "Immediate")
							{
								list[2] = "Immediately";
							}
							changeAfterDialogue = RDUtils.ParseEnum(list[2].Trim(), PlayStyleChange.OnNextBar);
						}
						rDInk.Freeze(changeAtStart, changeAfterDialogue);
					}
				});
				return;
			}
			if (this.text.StartsWith("[[") && this.text.EndsWith("]]"))
			{
				string text = this.text;
				try
				{
					text = base.game.currentLevel.EvaluateCurlyBracketsInString(this.text);
				}
				catch (Exception ex)
				{
					RDClassDll.printem("Using curly brackets but is not a variable: " + ex.Message);
				}
				this.text = RDString.Get(text.Substring(2, text.Length - 4));
			}
			base.ink.SetType(RDInkType.Passive);
			if (playTextSounds)
			{
				base.ink.speechSource.enabled = true;
			}
			else
			{
				base.ink.speechSource.Stop();
				base.ink.speechSource.enabled = false;
			}
			RunOnBeat(() =>
			{
				base.ink.RunFromText(this.text, panelSide, portraitSide);
				base.ink.SetPortraitSide(portraitSide);
				if (base.game.currentLevel.activeDialogues)
				{
					base.ink.Freeze();
				}
				if (base.game.currentLevel.activeDialoguesImmediately)
				{
					base.ink.Freeze(PlayStyleChange.Prolong, PlayStyleChange.Immediately);
				}
				base.ink.SetPanelRoom(base.room);
			});
		}

		public override string GetTooltipText()
		{
			return text;
		}
	}
}
