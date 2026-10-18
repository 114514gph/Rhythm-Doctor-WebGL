using System.Collections;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, 0, false)]
	public class LevelEvent_ChangeCharacter : LevelEvent_Base
	{
		[JsonProperty("", "", "editor.MakeRow.character", "", false, true, null, null)]
		[CharacterPicker(null, true, false, false, true, false, true, false, true, true)]
		public Character character { get; set; }

		[JsonProperty("", "", "editor.MakeRow.character", "", false, true, "EnableCCIf", null)]
		[DontShow]
		public string customCharacter { get; set; } = "";

		[JsonProperty("", "", "editor.HideRow.transition", "", false, true, null, null)]
		[ToggleGroup(false, null, null, new string[] { "Smooth", "Instant" }, true, 14)]
		public TransitionType transition { get; set; } = TransitionType.Instant;

		public bool EnableCCIf()
		{
			return character == Character.Custom;
		}

		public override IEnumerator Prepare()
		{
			if (!customCharacter.IsNullOrEmpty())
			{
				LevelEvent_MakeRow.UpdateCustomCharacter(customCharacter, justReturnErrors: false);
			}
			prepared = true;
			yield return null;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (customCharacter.IsNullOrEmpty())
				{
					if (character != Character.Custom)
					{
						base.game.rows[base.row].ent.ChangeCharacter(character, transition != TransitionType.Instant);
					}
				}
				else
				{
					base.game.rows[base.row].ent.ChangeCharacterCustom(customCharacter, transition != TransitionType.Instant);
				}
			});
		}

		public override string GetTooltipText()
		{
			if (base.row >= base.editor.rowsData.Count || base.row < 0)
			{
				return string.Empty;
			}
			string rowString = base.editor.rowsData[base.row].GetRowString();
			string text = ((character == Character.Custom) ? customCharacter : RDString.GetEnumValue(character));
			return rowString + "\n" + text;
		}
	}
}
