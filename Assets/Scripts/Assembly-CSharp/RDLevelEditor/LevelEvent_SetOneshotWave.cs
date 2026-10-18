using System.Collections.Generic;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, -1, RoomsUsage.NotUsed, true, true, true, false, 0, false)]
	public class LevelEvent_SetOneshotWave : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[] { "BoomAndRush", "Ball", "Spring", "Spike", "SpikeHuge", "Single" }, false, true)]
		public WaveType? waveType { get; set; } = WaveType.BoomAndRush;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		public int? height { get; set; } = 100;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		public int? width { get; set; } = 100;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public bool? wobble { get; set; } = false;

		public override void Run()
		{
			RunOnBeat(() =>
			{
				RowEntity ent = LevelEvent_Base.level.rows[base.row].ent;
				if (!(ent == null))
				{
					RowEntity rowEntity = ent;
					if (waveType.HasValue)
					{
						rowEntity.oneshotRowController.waveManager.waveType = waveType.Value;
					}
					if (height.HasValue)
					{
						rowEntity.oneshotRowController.waveHeightMultiplier = (float)height.Value / 100f;
					}
					if (width.HasValue)
					{
						rowEntity.oneshotRowController.waveWidthMultiplier = (float)width.Value / 100f;
					}
					if (wobble.HasValue)
					{
						ent.playerBox.wobblyLines = wobble.Value;
					}
				}
			});
		}

		public override string CustomControlName()
		{
			return "RowAction";
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string>();
			if (waveType.HasValue)
			{
				list.Add(RDString.GetEnumValue(waveType.Value));
			}
			if (height.HasValue)
			{
				list.Add(string.Format("{0} {1}%", RDString.Get("editor.SetOneshotWave.height"), height));
			}
			if (width.HasValue)
			{
				list.Add(string.Format("{0} {1}%", RDString.Get("editor.SetOneshotWave.width"), width));
			}
			return string.Join('\n', list);
		}
	}
}
