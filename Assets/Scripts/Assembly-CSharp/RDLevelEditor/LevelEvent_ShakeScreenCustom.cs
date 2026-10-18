using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_ShakeScreenCustom : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[] { "Normal", "Smooth", "Rotate", "BassDrop" }, false, false)]
		public EditorShakeType shakeType { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float duration { get; set; } = 0.5f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[FloatInfo(0f, 50f)]
		public float amplitude { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, "EnableFrequencyIf", null)]
		[FloatInfo(0f, 50f)]
		public float frequency { get; set; } = 10f;

		[JsonProperty("", "", null, "", false, true, "EnableFadeOutIf", null)]
		[Checkbox]
		public bool fadeOut { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Checkbox]
		public bool useBeats { get; set; }

		public bool EnableFrequencyIf()
		{
			return shakeType != EditorShakeType.Normal;
		}

		public bool EnableFadeOutIf()
		{
			EditorShakeType editorShakeType = shakeType;
			return editorShakeType == EditorShakeType.Smooth || editorShakeType == EditorShakeType.Rotate;
		}

		public override void Run()
		{
			if (duration <= 0f)
			{
				return;
			}
			float secs = ((!useBeats) ? duration : base.conductor.DurationBeatsToTime(base.beat - 1f, duration));
			int freq = Mathf.RoundToInt(frequency * 10f);
			int[] array = rooms;
			foreach (int room in array)
			{
				RDCamera cam = base.vfx.GetCamera(room);
				switch (shakeType)
				{
				case EditorShakeType.Smooth:
					RunOnBeat(() =>
					{
						cam.ClearSmoothShake();
						cam.smoothShake = cam.transform.DOShakePosition(secs, new Vector3(amplitude, amplitude, 0f), freq, 90f, snapping: false, fadeOut);
					});
					break;
				case EditorShakeType.Rotate:
					RunOnBeat(() =>
					{
						cam.ClearRotateShake();
						cam.rotateShake = cam.transform.DOShakeRotation(secs, amplitude, freq, 90f, fadeOut);
					});
					break;
				case EditorShakeType.BassDrop:
					RunOnBeat(() =>
					{
						base.vfx.BassDropNew(room, amplitude, secs, secs * 1.5f, Mathf.RoundToInt(frequency));
					});
					break;
				default:
					RunOnBeat(() =>
					{
						cam.ShakeWithDuration(secs, Mathf.RoundToInt(amplitude));
					});
					break;
				}
			}
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string>
			{
				RDString.GetEnumValue(shakeType),
				string.Format("{0} {1}", RDString.Get("editor.ShakeScreenCustom.amplitude"), amplitude)
			};
			if (EnableFrequencyIf())
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.ShakeScreenCustom.frequency"), frequency));
			}
			if (EnableFadeOutIf() && fadeOut)
			{
				list.Add(RDString.Get("editor.ShakeScreenCustom.fadeOut"));
			}
			return string.Join('\n', list);
		}
	}
}
