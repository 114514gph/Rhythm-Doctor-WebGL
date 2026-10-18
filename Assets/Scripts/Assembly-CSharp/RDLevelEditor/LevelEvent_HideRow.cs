using System.Collections.Generic;
using RDTools;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, 0, false)]
	public class LevelEvent_HideRow : LevelEvent_Base
	{
		[JsonProperty("", "", "editor.visibility", "", false, true, null, null)]
		public RowVisibilityMode show { get; set; } = RowVisibilityMode.Hidden;

		[JsonProperty("transition", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public TransitionType transitionType { get; set; }

		[JsonProperty("", "", "editor.SpinningRows.doEffects", "", false, true, "EnableShakeIf", null)]
		[Checkbox]
		public bool shake { get; set; } = true;

		public bool EnableShakeIf()
		{
			return transitionType != TransitionType.Instant;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			if (RDLevelData.current.settings.version < 17)
			{
				dict["show"] = (RDEditorUtils.DecodeBool(dict["show"]) ? RowVisibilityMode.Visible.ToString() : RowVisibilityMode.Hidden.ToString());
			}
			base.Decode(dict);
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (base.row >= LevelEvent_Base.level.rows.Length || RDLevelData.current.rows.Count == 0)
				{
					RDClassDll.printem($"{base.row} >= {LevelEvent_Base.level.rows.Length}, {RDLevelData.current.rows.Count} == 0");
				}
				else
				{
					base.row = Mathf.Clamp(base.row, 0, RDLevelData.current.rows.Count - 1);
					RowEntity ent = LevelEvent_Base.level.rows[base.row].ent;
					bool num = ent.appearance != RowVisibilityMode.Hidden;
					bool flag = transitionType != TransitionType.Instant;
					bool flag2 = !EnableShakeIf() || shake;
					if (show == RowVisibilityMode.Visible)
					{
						ShowAnimationType animationType = ShowAnimationType.InstantWithEffects;
						if (transitionType == TransitionType.Full)
						{
							animationType = ShowAnimationType.FullAnimation;
						}
						else if (transitionType == TransitionType.Instant)
						{
							animationType = ShowAnimationType.InstantNoEffects;
						}
						ent.Show(animationType, setup: false, keepCharacterVisible: true, flag2);
					}
					else if (show == RowVisibilityMode.OnlyRow)
					{
						ent.Show(flag ? ShowAnimationType.InstantWithEffects : ShowAnimationType.InstantNoEffects, setup: false, keepCharacterVisible: false, flag2);
					}
					else if (show == RowVisibilityMode.OnlyCharacter)
					{
						ent.Hide(keepCharacterVisible: true, flag, flag2);
						ent.character.visible = true;
					}
					else
					{
						ent.Hide(keepCharacterVisible: false, flag, flag2);
					}
					bool flag3 = ent.appearance != RowVisibilityMode.Hidden;
					if (num != flag3)
					{
						base.game.RepositionRowsAndStrips();
						if (flag3 && scnGame.instance.currentLevel.rowReflectionsJumping && ent.row.reflection != null)
						{
							ent.row.reflection.updateOriginY = true;
						}
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			if (base.editor.rowsData == null || base.editor.rowsData.Count == 0 || base.row >= base.editor.rowsData.Count)
			{
				return string.Empty;
			}
			string rowString = base.editor.rowsData[base.row].GetRowString();
			string enumValue = RDString.GetEnumValue(show);
			string enumValue2 = RDString.GetEnumValue(transitionType);
			List<string> values = new List<string> { rowString, enumValue, enumValue2 };
			return string.Join('\n', values);
		}
	}
}
