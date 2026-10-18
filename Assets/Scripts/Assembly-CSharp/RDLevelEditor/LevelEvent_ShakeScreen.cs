using System;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_ShakeScreen : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Slider(null, "ShakeScreen", 0f, true, false)]
		public StrengthLevel shakeLevel { get; set; } = StrengthLevel.Medium;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[] { "Normal", "Smooth", "Rotate" }, false, false)]
		public EditorShakeType shakeType { get; set; }

		public override void Run()
		{
			int amount;
			float smoothDuration;
			float smoothStrength;
			switch (shakeLevel)
			{
			case StrengthLevel.High:
				amount = 20;
				smoothDuration = 1f;
				smoothStrength = 4f;
				break;
			case StrengthLevel.Medium:
				amount = 10;
				smoothDuration = 0.6f;
				smoothStrength = 3f;
				break;
			case StrengthLevel.Low:
				amount = 5;
				smoothDuration = 0.3f;
				smoothStrength = 2f;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			int[] array = rooms;
			foreach (int room in array)
			{
				if (shakeType == EditorShakeType.Normal)
				{
					if (LevelEvent_Base.level.smoothShake)
					{
						shakeType = EditorShakeType.Smooth;
					}
					else if (LevelEvent_Base.level.rotateShake)
					{
						shakeType = EditorShakeType.Rotate;
					}
				}
				if (shakeType == EditorShakeType.Smooth)
				{
					RunOnBeat(() =>
					{
						base.vfx.ShakeCamSmooth(smoothDuration, smoothStrength, room);
					});
				}
				else if (shakeType == EditorShakeType.Rotate)
				{
					RunOnBeat(() =>
					{
						base.vfx.ShakeCamRotate(smoothDuration, smoothStrength, room);
					});
				}
				else
				{
					RunOnBeat(() =>
					{
						base.vfx.ShakeCam(15, amount, room);
					});
				}
			}
		}
	}
}
