using System.Collections;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_SVT : LevelBase
{
	private scrExecuteOnEveryHB shakeybeat;

	private scrExecuteOnEveryBeat randombeat;

	public Tweener currentTween;

	private scrExecuteOnHit slowdown;

	private scrExecuteOnUpdate kaleidoRot;

	private float kaleidoAngle;

	private float kaleidoRotSpeed = 25f;

	private bool _kre;

	private float wMult = 1f;

	private int wMode;

	public RowEntity oneshotRow => base.game.rows[0].ent;

	public RowEntity oneshotRowP2 => base.game.rows[1].ent;

	private bool kaleidoRotEnabled
	{
		get
		{
			return _kre;
		}
		set
		{
			_kre = value;
			if (!value)
			{
				base.room0.kaleidoscope.kaleidoCam.transform.rotation = Quaternion.identity;
			}
		}
	}

	public Level_SVT(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		base.isOneRowButNotBoss = true;
		base.room0.kaleidoscopeMode = true;
		yield return null;
	}

	public override void preactions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			base.conductor.visualBeatLengthMultiplier = 4f;
			if (base.game.reducedFlash)
			{
				break;
			}
			scrVolumeTracker.Add((float x) =>
			{
				if (!(oneshotRow == null))
				{
					Color a = (x * base.room0.kaleidoscope.colorA * 10f).WithAlpha(1f);
					Color b = (x * base.room0.kaleidoscope.colorB * 10f).WithAlpha(1f);
					Color a2 = Color.Lerp(a, b, 0.5f);
					a2 = Color.Lerp(a2, Color.white, 0.4f);
					a2.a = 6f * x;
					oneshotRow.SetOverrideAdditiveColor(a2);
					Color color = new Color(1f, x * 20f, 1f, 10f * x);
					oneshotRow.SetBorderColor(color);
					if (oneshotRowP2 != null)
					{
						oneshotRowP2.SetOverrideAdditiveColor(a2);
						oneshotRowP2.SetBorderColor(color);
					}
					SetAllHandsAdditiveColor(a2);
					SetAllHandsBorderColor(color.r, color.g, color.b, color.a, 0);
				}
			}, 0.03f, 0.3f);
			break;
		case 9:
			slowdown.enabled = false;
			break;
		case 12:
			slowdown.enabled = true;
			break;
		case 16:
			slowdown.enabled = false;
			break;
		case 19:
			slowdown.enabled = true;
			break;
		}
	}

	public override void actions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			base.room0.kaleidoscopeMode = true;
			RDClass.Vfx.spriteGlowColor = Color.white;
			RDClass.Vfx.spriteGlowPower = 1f;
			slowdown = scrExecuteOnHit.Add(() =>
			{
				if (RDTime.speed == 1f)
				{
					if (currentTween != null)
					{
						currentTween.Kill(complete: true);
					}
					currentTween = DOTween.To(() => base.game.oneshotSlowdownMultiplier, (float x) =>
					{
						base.game.SetSlowdown(x);
					}, base.gc.svt_slowDownTo, base.gc.svt_slowDownDuration).SetEase(base.gc.svt_ease).OnComplete(() =>
					{
						base.game.SetSlowdown(1f);
					});
				}
			}, HitType.Hit, persistent: true);
			kaleidoRot = scrExecuteOnUpdate.Add(() =>
			{
				if (kaleidoRotEnabled)
				{
					kaleidoAngle += Time.deltaTime * kaleidoRotSpeed;
					base.room0.kaleidoscope.kaleidoCam.transform.rotation = Quaternion.Euler(0f, 0f, kaleidoAngle);
				}
			});
			kaleidoRotEnabled = false;
			break;
		case 5:
			base.conductor.visualBeatLengthMultiplier = 2f;
			break;
		case 9:
			base.conductor.visualBeatLengthMultiplier = 1f;
			base.tkCam.Shake(20, 10);
			base.room0.kaleidoscopeSpeed *= 2f;
			break;
		case 11:
			base.tkCam.Shake(20, 10);
			break;
		case 13:
			base.room0.kaleidoscopeSpeed /= 2f;
			base.conductor.visualBeatLengthMultiplier = 2f;
			printe("playing Interlude");
			base.ink.Run("diaSVT", "Interlude");
			break;
		case 15:
			base.conductor.visualBeatLengthMultiplier = 2f;
			base.room0.kaleidoscopeSpeed *= 2f;
			break;
		case 17:
			base.conductor.visualBeatLengthMultiplier = 1f;
			base.room0.kaleidoscopeSpeed *= 2f;
			break;
		case 18:
			base.conductor.visualBeatLengthMultiplier = 0.5f;
			break;
		case 19:
			base.room0.kaleidoscopeSpeed /= 4f;
			break;
		case 2:
		case 3:
		case 4:
		case 6:
		case 7:
		case 8:
		case 10:
		case 12:
		case 14:
		case 16:
			break;
		}
	}

	public override void BeepGet(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		base.BeepGet(beepPeriod);
		RDClass.Vfx.oneshotSpotlights.Show(0);
	}

	public override void BeepSet(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		base.BeepSet(beepPeriod);
		RDClass.Vfx.oneshotSpotlights.Show(1);
	}

	public override void BeepGo(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		RDClass.Vfx.oneshotSpotlights.Hide();
	}

	public void randombeatfn()
	{
		zoomout();
		int num = Mathf.CeilToInt(Random.value * 4f);
		RDClass.Vfx.SetTileScreen(new Vector2(num, num));
		RDClass.Vfx.SetFlipScreenX((double)Random.value < 0.5);
		RDClass.Vfx.SetFlipScreenY((double)Random.value < 0.5);
	}

	public void randombeattame()
	{
		int num = Mathf.CeilToInt(Random.value * 3f);
		RDClass.Vfx.SetTileScreen(new Vector2(num, num));
	}

	public void zoomin()
	{
		RDClass.Vfx.SetTileScreen(Vector2.one);
		RDClass.Vfx.SetFlipScreenX(x: false);
		RDClass.Vfx.SetFlipScreenY(y: false);
		RDClass.Vfx.ZoomScreen(2f, 0.47f, 0.23f);
	}

	public void zoomout()
	{
		RDClass.Vfx.ZoomScreen(1f);
	}

	public override void OnMistake(RowEntity ent = null)
	{
	}

	private void SetKaleidoscopeColor0(float r, float g, float b)
	{
		base.room0.kaleidoscope.colorA = new Color(r, g, b, 1f);
	}

	private void SetKaleidoscopeColor1(float r, float g, float b)
	{
		base.room0.kaleidoscope.colorB = new Color(r, g, b, 1f);
	}

	public void WhiteKaleidoscope()
	{
		SetKaleidoscopeColor0(1f, 1f, 1f);
		SetKaleidoscopeColor1(1f, 1f, 1f);
	}

	public void NormalKaleidoscope()
	{
		SetKaleidoscopeColor0(1f, 0f, 0.4f);
		SetKaleidoscopeColor1(0f, 0.4f, 1f);
	}

	public void ColorKaleidoscope(float r, float g, float b, int mode)
	{
		if (mode == 0 || mode == 1)
		{
			SetKaleidoscopeColor0(r, g, b);
		}
		if (mode == 0 || mode == 2)
		{
			SetKaleidoscopeColor1(r, g, b);
		}
	}

	public void EnableKaleidoRot()
	{
		kaleidoRotEnabled = true;
	}

	public void EaseKaleidoRot()
	{
		kaleidoRotEnabled = true;
		kaleidoRotSpeed = 0f;
		DOTween.To(() => kaleidoRotSpeed, (float s) =>
		{
			kaleidoRotSpeed = s;
		}, 10f, 2f).OnComplete(() =>
		{
			kaleidoRotSpeed = 10f;
		});
	}

	public void DisableKaleidoRot()
	{
		kaleidoAngle = 0f;
		kaleidoRotEnabled = false;
	}

	public void SetWaveMult(float mult)
	{
		wMult = mult;
		SetWaveMode(wMode);
	}

	public void SetWaveMode(int waveMode)
	{
		switch (waveMode)
		{
		case 0:
			SetOneshotType(0, "BoomAndRush");
			oneshotRow.oneshotRowController.waveHeightMultiplier = 1f * wMult;
			oneshotRow.oneshotRowController.waveWidthMultiplier = 1f * wMult;
			if (oneshotRowP2 != null)
			{
				SetOneshotType(1, "BoomAndRush");
				oneshotRowP2.oneshotRowController.waveHeightMultiplier = 1f * wMult;
				oneshotRowP2.oneshotRowController.waveWidthMultiplier = 1f * wMult;
			}
			wMode = 0;
			break;
		case 1:
			SetOneshotType(0, "Spring");
			oneshotRow.oneshotRowController.waveHeightMultiplier = 1f * wMult;
			oneshotRow.oneshotRowController.waveWidthMultiplier = 1f * wMult;
			if (oneshotRowP2 != null)
			{
				SetOneshotType(1, "Spring");
				oneshotRowP2.oneshotRowController.waveHeightMultiplier = 1f * wMult;
				oneshotRowP2.oneshotRowController.waveWidthMultiplier = 1f * wMult;
			}
			wMode = 1;
			break;
		case 2:
			SetOneshotType(0, "Spring");
			oneshotRow.oneshotRowController.waveHeightMultiplier = 1.5f * wMult;
			oneshotRow.oneshotRowController.waveWidthMultiplier = 1.5f * wMult;
			if (oneshotRowP2 != null)
			{
				SetOneshotType(1, "Spring");
				oneshotRowP2.oneshotRowController.waveHeightMultiplier = 1.5f * wMult;
				oneshotRowP2.oneshotRowController.waveWidthMultiplier = 1.5f * wMult;
			}
			wMode = 2;
			break;
		case 3:
			SetOneshotType(0, "Spike");
			oneshotRow.oneshotRowController.waveHeightMultiplier = 1f * wMult;
			oneshotRow.oneshotRowController.waveWidthMultiplier = 1f * wMult;
			if (oneshotRowP2 != null)
			{
				SetOneshotType(1, "Spike");
				oneshotRowP2.oneshotRowController.waveHeightMultiplier = 1f * wMult;
				oneshotRowP2.oneshotRowController.waveWidthMultiplier = 1f * wMult;
			}
			wMode = 3;
			break;
		}
	}
}
