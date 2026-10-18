using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, 0, false)]
	public class LevelEvent_SpinningRows : LevelEvent_Base, IDurationHaver
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		public SpinningRowsAction action { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableOtherRowIf", null)]
		[Row(false, false)]
		public int toRow { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableAngleIf", null)]
		[InputField("°", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float angle { get; set; }

		[JsonProperty("", "", "editor.NewWindowDance.amplitude", "", false, true, "EnableFrequencyAndAmplitudeIf", null)]
		[InputField("°", InputField.LineType.SingleLine, 40, 14, false, false)]
		[Off]
		public float? amplitude { get; set; } = 0f;

		[JsonProperty("", "", "editor.NewWindowDance.frequency", "", false, true, "EnableFrequencyAndAmplitudeIf", null)]
		[InputField("/beat", InputField.LineType.SingleLine, 40, 14, false, false)]
		[Off]
		public float? frequency { get; set; } = 0f;

		[JsonProperty("", "", null, "", false, true, "EnableDoEffectsIf", null)]
		public bool doEffects { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableDurationIf", null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float duration { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableDurationIf", null)]
		public Ease ease { get; set; } = Ease.Linear;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Description("editor.SpinningRows.warning", 42, 16, TextAnchor.MiddleCenter)]
		public bool description => false;

		public bool EnableAngleIf()
		{
			SpinningRowsAction spinningRowsAction = action;
			return spinningRowsAction == SpinningRowsAction.Rotate || spinningRowsAction == SpinningRowsAction.ConstantRotation || spinningRowsAction == SpinningRowsAction.Split || spinningRowsAction == SpinningRowsAction.Merge;
		}

		public bool EnableOtherRowIf()
		{
			return action == SpinningRowsAction.Connect;
		}

		public bool EnableFrequencyAndAmplitudeIf()
		{
			return action == SpinningRowsAction.WavyRotation;
		}

		public bool EnableDoEffectsIf()
		{
			return action == SpinningRowsAction.Merge;
		}

		public bool EnableDurationIf()
		{
			if (!EnableAngleIf())
			{
				return EnableFrequencyAndAmplitudeIf();
			}
			return true;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				RowEntity rowEnt = base.game.rows[base.row].ent;
				RowEntity ent = base.game.rows[toRow].ent;
				SpinningRows spinningRowsController = rowEnt.targetCharacterRowEnt.spinningRowsController;
				SpinningRows.SpinningRowData spinningRowData = spinningRowsController?.spinningRows[rowEnt];
				float delay = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
				if (action == SpinningRowsAction.Connect)
				{
					if (ent.spinningRowsController == null)
					{
						GameObject gameObject = new GameObject("Spinning Rows Controller");
						gameObject.transform.SetParent(ent.transform, worldPositionStays: false);
						ent.spinningRowsController = gameObject.AddComponent<SpinningRows>().Setup(ent);
					}
					ent.spinningRowsController.Connect(rowEnt);
				}
				else if (action == SpinningRowsAction.Disconnect)
				{
					spinningRowsController.Disconnect(rowEnt);
				}
				else
				{
					SpinningRowsAction spinningRowsAction = action;
					if (spinningRowsAction == SpinningRowsAction.Rotate || spinningRowsAction == SpinningRowsAction.Merge || spinningRowsAction == SpinningRowsAction.Split)
					{
						if (spinningRowData != null)
						{
							spinningRowData.Rotate(angle, delay, ease);
							if (action == SpinningRowsAction.Merge)
							{
								SpinningRows.SpinningRowData spinningRowData2 = spinningRowsController.spinningRows[spinningRowsController.mainRow];
								spinningRowData2.Rotate(angle, delay, ease);
								spinningRowData2.SetWavyAmpltiude(0f, delay, ease);
								spinningRowData.SetWavyAmpltiude(0f, delay, ease);
								DOVirtual.DelayedCall(delay, () =>
								{
									spinningRowsController.Merge(rowEnt, doEffects);
								});
							}
							else if (action == SpinningRowsAction.Split)
							{
								spinningRowsController.Split(rowEnt);
							}
						}
					}
					else if (action == SpinningRowsAction.ConstantRotation)
					{
						spinningRowData?.SetConstantRotation(angle / base.conductor.crotchet, delay, ease);
					}
					else if (action == SpinningRowsAction.WavyRotation && spinningRowData != null)
					{
						float? num = amplitude;
						if (num.HasValue)
						{
							float valueOrDefault = num.GetValueOrDefault();
							spinningRowData.SetWavyAmpltiude(valueOrDefault, delay, ease);
						}
						num = frequency;
						if (num.HasValue)
						{
							float valueOrDefault2 = num.GetValueOrDefault();
							spinningRowData.SetWavyFrequency(1f / base.conductor.DurationBeatsToTime(1f / valueOrDefault2), delay, ease);
						}
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			string rowString = base.editor.rowsData[base.row].GetRowString();
			List<string> list = new List<string> { RDString.GetEnumValue(action) };
			if (action == SpinningRowsAction.Connect)
			{
				string rowString2 = base.editor.rowsData[toRow].GetRowString();
				list.Add(rowString + " -> " + rowString2);
			}
			else
			{
				list.Add(rowString);
				if (EnableAngleIf())
				{
					list.Add(string.Format("{0} {1}", RDString.Get("editor.SpinningRows.angle"), angle));
				}
				if (EnableFrequencyAndAmplitudeIf())
				{
					if (amplitude.HasValue)
					{
						list.Add(string.Format("{0} {1}", RDString.Get("editor.NewWindowDance.amplitude"), amplitude));
					}
					if (frequency.HasValue)
					{
						list.Add(string.Format("{0} {1}", RDString.Get("editor.NewWindowDance.frequency"), frequency));
					}
				}
			}
			return string.Join('\n', list);
		}
	}
}
