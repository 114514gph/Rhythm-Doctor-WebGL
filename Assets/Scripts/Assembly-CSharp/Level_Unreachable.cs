using System.Collections.Generic;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_Unreachable : LevelBase
{
	private bool kaleidoscopeRowGlow;

	private Color manualGlow = Color.white;

	private List<Row> glowRows = new List<Row>();

	private bool glowHand;

	private float overrideAlpha = 1f;

	private Tween overrideAlphaTween;

	public Level_Unreachable(RDLevelData data)
		: base(data)
	{
	}

	public override void preactions()
	{
		if (base.conductor.barNumber != 1 || base.game.reducedFlash)
		{
			return;
		}
		scrVolumeTracker.Add((float x) =>
		{
			Color a = (x * base.room0.kaleidoscope.colorA * 10f).WithAlpha(1f);
			Color b = (x * base.room0.kaleidoscope.colorB * 10f).WithAlpha(1f);
			Color a2 = (kaleidoscopeRowGlow ? Color.Lerp(a, b, 0.5f) : manualGlow);
			a2 = Color.Lerp(a2, Color.white, 0.4f);
			a2.a = 6f * x * overrideAlpha;
			Color color = new Color(1f, x * 20f, 1f, 10f * x * overrideAlpha);
			foreach (Row glowRow in glowRows)
			{
				if (glowRow != null)
				{
					RowEntity ent = glowRow.ent;
					ent.SetOverrideAdditiveColor(a2);
					ent.SetBorderColor(color);
				}
			}
			if (glowHand)
			{
				SetAllHandsAdditiveColor(a2);
				SetAllHandsBorderColor(color.r, color.g, color.b, color.a, 0);
			}
		}, 0.03f, 0.3f);
	}

	public void ToggleHandGlow(bool on)
	{
		glowHand = on;
	}

	public void ToggleKaleidoscopeGlow(bool on)
	{
		kaleidoscopeRowGlow = on;
	}

	public void SetManualGlow(float r, float g, float b)
	{
		manualGlow = new Color(r, g, b);
	}

	public void AddGlowRow(int row)
	{
		Row item = base.game.rows[row];
		if (!glowRows.Contains(item))
		{
			glowRows.Add(item);
		}
	}

	public void RemoveGlowRow(int row)
	{
		Row item = base.game.rows[row];
		if (glowRows.Contains(item))
		{
			glowRows.Remove(item);
		}
	}

	public void TweenOverrideAlpha(float alpha, float durationBeats)
	{
		overrideAlphaTween?.Kill();
		float duration = scrConductor.instance.DurationBeatsToTime(durationBeats);
		overrideAlphaTween = DOTween.To(() => overrideAlpha, (float x) =>
		{
			overrideAlpha = x;
		}, alpha, duration);
	}

	public void SpaceSetup()
	{
		base.room0.spaceBackground.SwitchToCloseupMode();
	}
}
