using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, true, -10, false)]
	public class LevelEvent_Tile : LevelEvent_Base, IDurationHaver, ICompatibleDecorationTypeChecker
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[PositionPicker(false, false, null, false)]
		public Float2? tiling { get; set; } = new Float2(2f, 2f);

		[JsonProperty("", "", null, "", false, true, null, null)]
		[PositionPicker(false, false, null, false)]
		[Off]
		public Float2? position { get; set; } = new Float2(0f, 0f);

		[JsonProperty("", "", null, "", false, true, null, null)]
		[PositionPicker(false, false, null, false)]
		[Off]
		public Float2? speed { get; set; } = new Float2(0f, 0f);

		[JsonProperty("", "", null, "", false, true, "EnableSpeedDescriptionIf", null)]
		[Description("editor.SetBackgroundColor.speedDescription", 14, 0, TextAnchor.UpperCenter)]
		public bool speedDescription => false;

		[JsonProperty("", "", null, "", false, true, "EnablePulseDescriptionIf", null)]
		[Description("editor.SetBackgroundColor.pulseDescription", 14, 0, TextAnchor.UpperCenter)]
		public bool pulseDescription => false;

		[JsonProperty("", "", null, "", false, true, "EnableTilingIf", null)]
		[ToggleGroup(false, null, null, null, false, 14)]
		public TilingType tilingType { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIntervalIf", null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0.01f, float.PositiveInfinity)]
		public float interval { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float duration { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		public bool isCompatibleDecorationType => base.ownerDecoration is LevelEvent_Sprite;

		public bool EnableTilingIf()
		{
			return speed.HasValue;
		}

		public bool EnableIntervalIf()
		{
			if (EnableTilingIf())
			{
				return tilingType == TilingType.Pulse;
			}
			return false;
		}

		public bool EnableSpeedDescriptionIf()
		{
			if (EnableTilingIf())
			{
				return tilingType == TilingType.Scroll;
			}
			return false;
		}

		public bool EnablePulseDescriptionIf()
		{
			if (EnableTilingIf())
			{
				return tilingType == TilingType.Pulse;
			}
			return false;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (RDLevelData.current.settings.version < 60)
			{
				position *= -1f;
			}
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (isCompatibleDecorationType)
				{
					if (!LevelEvent_Base.level.sprites.ContainsKey(target))
					{
						Debug.Log("Tile R - didn't found matching id " + target);
					}
					else
					{
						CustomSprite sprite = LevelEvent_Base.level.sprites[target];
						Material material = sprite.renderer.material;
						float num = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
						if (material.mainTexture == null)
						{
							material.mainTexture = sprite.customAnimation.data.mainTexture;
						}
						if (tiling.HasValue || position.HasValue || speed.HasValue)
						{
							material.mainTexture.wrapMode = TextureWrapMode.Repeat;
						}
						Float2? @float = position;
						if (@float.HasValue)
						{
							Float2 valueOrDefault = @float.GetValueOrDefault();
							if (valueOrDefault.xUsed)
							{
								sprite.positionTweenX?.Kill();
								sprite.positionTweenX = DOTween.To(() => sprite.tileOffset.x, (float s) =>
								{
									sprite.tileOffset.x = s;
								}, valueOrDefault.x, num).SetEase(ease).Done();
							}
							if (valueOrDefault.yUsed)
							{
								sprite.positionTweenY?.Kill();
								sprite.positionTweenY = DOTween.To(() => sprite.tileOffset.y, (float s) =>
								{
									sprite.tileOffset.y = s;
								}, valueOrDefault.y, num).SetEase(ease).Done();
							}
						}
						@float = tiling;
						if (@float.HasValue)
						{
							Float2 valueOrDefault2 = @float.GetValueOrDefault();
							if (valueOrDefault2.xUsed)
							{
								sprite.tilingTweenX?.Kill();
								sprite.tilingTweenX = DOTween.To(() => sprite.tileAmount.x, (float s) =>
								{
									sprite.tileAmount.x = s;
								}, valueOrDefault2.x, num).SetEase(ease).Done();
							}
							if (valueOrDefault2.yUsed)
							{
								sprite.tilingTweenY?.Kill();
								sprite.tilingTweenY = DOTween.To(() => sprite.tileAmount.y, (float s) =>
								{
									sprite.tileAmount.y = s;
								}, valueOrDefault2.y, num).SetEase(ease).Done();
							}
						}
						@float = speed;
						if (@float.HasValue)
						{
							Float2 valueOrDefault3 = @float.GetValueOrDefault();
							if (valueOrDefault3.xUsed)
							{
								sprite.SetTileSpeedX(valueOrDefault3.x, tilingType, num, ease, interval, base.beat - 1f);
							}
							if (valueOrDefault3.yUsed)
							{
								sprite.SetTileSpeedY(valueOrDefault3.y, tilingType, num, ease, interval, base.beat - 1f);
							}
						}
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string>();
			if (((bool?)tiling) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.Tile.tiling"), tiling));
			}
			if (((bool?)position) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.Tile.position"), position));
			}
			if (((bool?)speed) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.Tile.speed"), speed));
			}
			return string.Join('\n', list);
		}
	}
}
