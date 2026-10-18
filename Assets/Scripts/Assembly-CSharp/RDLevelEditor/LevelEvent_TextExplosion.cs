using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRooms, true, true, true, false, -10, false)]
	public class LevelEvent_TextExplosion : LevelEvent_Base
	{
		private List<RDScrollyText> textExplosions;

		private const int textCount = 20;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("", InputField.LineType.SingleLine, 40, 14, false, true)]
		public string text { get; set; } = "Example";

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ColorInfo(false)]
		public ColorOrPalette color { get; set; } = Color.black;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public TextExplosionMode mode { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public TextExplosionDirection direction { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(1f, 1000f)]
		public float speed { get; set; } = 100f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		public override void Init()
		{
			base.Init();
			string withCheck = RDString.GetWithCheck("editor.TextExplosion.exampleText", out var exists);
			if (exists)
			{
				text = withCheck;
			}
		}

		public override IEnumerator Prepare()
		{
			textExplosions = new List<RDScrollyText>();
			int[] array = rooms;
			for (int i = 0; i < array.Length; i++)
			{
				_ = array[i];
				for (int j = 0; j < 20; j++)
				{
					textExplosions.Add(RDScrollyText.Create());
				}
			}
			prepared = true;
			yield return null;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (RDString.samuraiMode)
				{
					this.text = "Samurai.";
				}
				string text = RDEditorUtils.EscapeTextForJSON(this.text);
				bool randomColors = mode == TextExplosionMode.Random;
				float num = speed / 100f;
				float num2 = base.vfx.RDHeight / 2f;
				float rDHeight = base.vfx.RDHeight;
				Color value = color.ToColor().WithAlpha(1f);
				bool reverse = direction == TextExplosionDirection.Right;
				int num3 = 0;
				foreach (RDScrollyText textExplosion in textExplosions)
				{
					float num4 = Random.value * 1.5f + 0.5f;
					textExplosion.Setup(text, num2, num4 / num, ease, rDHeight, 2f, randomColors, value, rooms[num3 / 20], reverse).Run();
					num3++;
				}
			});
		}

		public override string GetTooltipText()
		{
			return text;
		}
	}
}
