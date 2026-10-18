using System.Collections.Generic;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, -1, RoomsUsage.NotUsed, true, true, true, false, 0, true)]
	public class LevelEvent_LinkRows : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Row(false, false)]
		public int sourceRow { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfSourceIsTarget", null)]
		[Description("editor.LinkRows.sourceIsTargetDescription", 28, 16, TextAnchor.MiddleCenter)]
		public bool descriptionA => false;

		[JsonProperty("", "", null, "", false, true, "EnableActionIf", null)]
		public LinkRowsAction action { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfLinking", null)]
		public LinkRowsBeatBehavior beatBehavior { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfCopyingBeats", null)]
		[Checkbox]
		public bool muteBeatsounds { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, "EnableIfCopyingBeats", null)]
		[Checkbox]
		public bool normalizeMistakeWeight { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, "EnableIfLinking", null)]
		[Checkbox]
		public bool copyPaintEffects { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Description("editor.LinkRows.warning", 42, 16, TextAnchor.MiddleCenter)]
		public bool descriptionB => false;

		public bool EnableActionIf()
		{
			return base.row != sourceRow;
		}

		public bool EnableIfLinking()
		{
			if (EnableActionIf())
			{
				return action == LinkRowsAction.Link;
			}
			return false;
		}

		public bool EnableIfCopyingBeats()
		{
			if (EnableIfLinking())
			{
				return beatBehavior == LinkRowsBeatBehavior.CopyBeats;
			}
			return false;
		}

		public bool EnableIfSourceIsTarget()
		{
			return base.row == sourceRow;
		}

		public override void RunPrebar()
		{
			if (base.row != sourceRow)
			{
				RunOnBeat(() =>
				{
					SetShadowPrebar(base.game.rows[base.row]);
				}, forcePrebar: true);
			}
		}

		public override void Run()
		{
			if (base.row == sourceRow)
			{
				return;
			}
			RunOnBeat(() =>
			{
				Row row = base.game.rows[base.row];
				SetShadowRealtime(row);
				if (tag != null)
				{
					SetShadowPrebar(row);
				}
				if (base.editor != null)
				{
					base.editor.tabSection_rows.UpdateUI();
				}
			});
		}

		private void SetShadowPrebar(Row targetRow)
		{
			if (action == LinkRowsAction.Link)
			{
				targetRow.isShadow = true;
				targetRow.shadowOrHostID = sourceRow;
				targetRow.muteRowSounds = beatBehavior == LinkRowsBeatBehavior.CopyBeats && muteBeatsounds;
				targetRow.copyBeatsWhenShadow = beatBehavior == LinkRowsBeatBehavior.CopyBeats;
			}
			else
			{
				targetRow.isShadow = false;
				targetRow.shadowOrHostID = -1;
				targetRow.muteRowSounds = false;
			}
		}

		private void SetShadowRealtime(Row targetRow)
		{
			if (action == LinkRowsAction.Link)
			{
				targetRow.isShadowRealtime = true;
				targetRow.shadowOrHostIDRealtime = sourceRow;
				targetRow.normalizeMistakeWeightWhenShadow = beatBehavior == LinkRowsBeatBehavior.CopyBeats && normalizeMistakeWeight;
				targetRow.copyPaintEffectsWhenShadowOrHost = copyPaintEffects;
				targetRow.reactToHostsBeatsWhenShadow = beatBehavior == LinkRowsBeatBehavior.ReactToBeats;
			}
			else
			{
				targetRow.isShadowRealtime = false;
				targetRow.shadowOrHostIDRealtime = -1;
			}
		}

		public override string GetTooltipText()
		{
			if (base.row == sourceRow)
			{
				return null;
			}
			List<string> list = new List<string> { RDString.GetEnumValue(action) };
			string rowString = base.editor.rowsData[base.row].GetRowString();
			string rowString2 = base.editor.rowsData[sourceRow].GetRowString();
			list.Add(rowString + " -> " + rowString2);
			return string.Join('\n', list);
		}
	}
}
