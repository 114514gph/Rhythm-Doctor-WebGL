using System.Collections.Generic;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, 0, false)]
	public class LevelEvent_PlayExpression : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[CharacterPicker(new Character[] { }, false, false, true, false, true, false, true, false, false)]
		public string expression { get; set; } = "neutral";

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, "editor.PlayExpression.replace", null, false, 14)]
		public bool replace { get; set; }

		[JsonProperty("target", "", null, "", false, true, "EnableTargetIf", null)]
		[Dropdown(new string[] { "Neutral", "Happy", "Barely", "Missed", "Prehit", "Beep" }, false, false)]
		public OverrideExpression targetExpression { get; set; }

		public bool EnableTargetIf()
		{
			return replace;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				int count = RDLevelData.current.rows.Count;
				if (count != 0)
				{
					base.row = Mathf.Clamp(base.row, 0, count - 1);
					scrChar character = base.game.rows[base.row].ent.character;
					bool flag = false;
					if (replace)
					{
						switch (targetExpression)
						{
						case OverrideExpression.Neutral:
							flag = CompareWithCurrentExpression(character, character.neutralAnimName);
							character.neutralAnimName = expression;
							break;
						case OverrideExpression.Happy:
							flag = CompareWithCurrentExpression(character, character.happyAnimName);
							character.happyAnimName = expression;
							break;
						case OverrideExpression.Barely:
							flag = CompareWithCurrentExpression(character, character.barelyAnimName);
							character.barelyAnimName = expression;
							break;
						case OverrideExpression.Missed:
							flag = CompareWithCurrentExpression(character, character.missedAnimName);
							character.missedAnimName = expression;
							break;
						case OverrideExpression.Prehit:
							flag = CompareWithCurrentExpression(character, character.prehitAnimName);
							character.prehitAnimName = expression;
							break;
						case OverrideExpression.Beep:
							flag = CompareWithCurrentExpression(character, character.beepAnimName);
							character.beepAnimName = expression;
							break;
						}
					}
					if (!replace | flag)
					{
						if (character.character == Character.Boy && expression == "whaat")
						{
							expression = "surprised";
						}
						character.PlayExpression(expression, disableOverride: true, ignoreKeep: false, 1f, alreadyPrefixed: true);
						if (base.game.currentLevel.shadowRowsCopyExpressions)
						{
							if (base.game.shadowRowStyle == ShadowRowStyle.HostToShadowIf2P)
							{
								Debug.LogError("Can't use this shadow row style here!");
							}
							for (int i = 0; i < base.game.rows.Length; i++)
							{
								if (i != base.row)
								{
									Row row = base.game.rows[i];
									if (row.shadowOrHostID == base.row)
									{
										row.ent.character.PlayExpression(expression, disableOverride: true, ignoreKeep: false, 1f, alreadyPrefixed: true);
									}
								}
							}
						}
					}
				}
			});
		}

		private bool CompareWithCurrentExpression(scrChar character, string expression)
		{
			if (character.character == Character.Custom)
			{
				if (character.customAnimation == null || character.customAnimation.currentClip == null)
				{
					return false;
				}
				return character.customAnimation.currentClip.name == expression;
			}
			return character.IsCurrentExpression(expression);
		}

		public override string GetTooltipText()
		{
			if (base.editor.rowsData == null || base.editor.rowsData.Count == 0 || base.row >= base.editor.rowsData.Count)
			{
				return string.Empty;
			}
			string rowString = base.editor.rowsData[base.row].GetRowString();
			List<string> list = new List<string> { rowString };
			string withCheck = RDString.GetWithCheck("editor.expression." + expression, out var exists);
			string text = (exists ? withCheck : expression);
			if (replace)
			{
				string enumValue = RDString.GetEnumValue(targetExpression);
				list.Add(enumValue + " -> " + text);
			}
			else
			{
				list.Add(text);
			}
			return string.Join('\n', list);
		}
	}
}
