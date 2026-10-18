using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_SetForeground : LevelEvent_Base, IDurationHaver
	{
		private Texture2D[] textures;

		[JsonProperty("image", "", null, "", false, true, null, null)]
		[Image(true)]
		public string[] images { get; set; } = Array.Empty<string>();

		[JsonProperty("", "", null, "", false, true, null, null)]
		public ColorOrPalette color { get; set; } = Color.white;

		[JsonProperty("", "", null, "", false, true, "EnableFpsIf", null)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float fps { get; set; } = 30f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[] { "ScaleToFill", "AspectFit", "AspectFill", "Center", "Tiled" }, false, false)]
		public ContentMode contentMode { get; set; } = ContentMode.ScaleToFill;

		[JsonProperty("", "", null, "", false, true, "EnableIfTiled", null)]
		[ToggleGroup(false, null, null, null, false, 14)]
		public TilingType tilingType { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfTiled", null)]
		[PositionPicker(false, true, "px", false)]
		public Float2 speed { get; set; } = new Float2(0f, 0f);

		[JsonProperty("", "", null, "", false, true, "EnableIfTiled", null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float duration { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIntervalIf", null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0.01f, float.PositiveInfinity)]
		public float interval { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, "EnableIfTiled", null)]
		public Ease ease { get; set; } = Ease.Linear;

		public bool EnableFpsIf()
		{
			return images.Length > 1;
		}

		public bool EnableIfTiled()
		{
			return contentMode == ContentMode.Tiled;
		}

		public bool EnableIntervalIf()
		{
			return tilingType == TilingType.Pulse;
		}

		public override IEnumerator Prepare()
		{
			if (images.Length != 0)
			{
				textures = new Texture2D[images.Length];
				int num = 0;
				string[] array = images;
				foreach (string image in array)
				{
					textures[num] = RDEditorUtils.LoadLevelTexture(scnGame.levelToLoadSource, image);
					int[] array2 = rooms;
					foreach (int num2 in array2)
					{
						if (num2 != -1)
						{
							base.game.rooms[num2].GetTextureLayer(RDSortingLayer.Foreground);
						}
					}
					num++;
				}
			}
			prepared = true;
			yield return null;
		}

		public override string Encode()
		{
			string text = base.Encode();
			if (EnableIfTiled())
			{
				text = text.Replace($", \"speed\": [{speed.x}, {speed.y}]", $", \"scrollX\": {speed.x}, \"scrollY\": {speed.y}");
			}
			return text;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			if (dict.ContainsKey("image") && dict["image"] is string text)
			{
				dict["image"] = ((text == string.Empty) ? new List<object>() : new List<object> { text });
			}
			base.Decode(dict);
			if (dict.ContainsKey("scrollX") && dict.ContainsKey("scrollY") && EnableIfTiled())
			{
				speed = new Float2(float.Parse(dict["scrollX"].ToString()), float.Parse(dict["scrollY"].ToString()));
			}
		}

		public override void Run()
		{
			int[] array = rooms;
			foreach (int room in array)
			{
				RunOnBeat(() =>
				{
					if (textures != null && textures.Length != 0)
					{
						base.vfx.GetForegroundTextureLayer(room).Show(textures, contentMode, FilterMode.Point, color.ToColor(), RDSortingLayer.Foreground, 0, speed.ToVector2(), fps, base.conductor.DurationBeatsToTime(base.beat - 1f, duration), ease, tilingType == TilingType.Scroll, dontSetSortingOrder: false, interval, base.beat - 1f);
					}
					else
					{
						base.vfx.GetForegroundTextureLayer(room).gameObject.SetActive(value: false);
					}
				});
			}
		}

		public override string GetTooltipText()
		{
			if (images == null || images.Length == 0)
			{
				return null;
			}
			if (images.Length > 4)
			{
				return string.Format("{0},\n+{1}...", string.Join(",\n", images.Take(4)), images.Length - 4);
			}
			return string.Join(",\n", images);
		}
	}
}
