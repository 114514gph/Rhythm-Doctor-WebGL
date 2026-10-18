using System.Collections;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_OrientalDubstep : LevelBase
{
	public RDSamuraiWard samuraiWard;

	private scrExecuteOnEveryHB cycler;

	private scrExecuteOnHit bassDropTimeSlow;

	private bool halloweenWeek;

	private int currentCycleColorID;

	private uint[] ColorPalette = new uint[11]
	{
		4291189578u, 4291198794u, 4283090599u, 4283081414u, 4283070918u, 4283058886u, 4285745862u, 4288039622u, 4290136774u, 4291185316u,
		4291185279u
	};

	public Level_OrientalDubstep(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		halloweenWeek = RDBase.IsHalloweenWeek();
	}

	public override IEnumerator LoadBigAssets()
	{
		samuraiWard = LoadLevelAsset<RDSamuraiWard>();
		samuraiWard.transform.localPosition = new Vector3(3176f, 99f, 0f);
		samuraiWard.SetToEmptyVersion(empty: true);
		return base.LoadBigAssets();
	}

	public void SetBgStyle(int style)
	{
		switch (style)
		{
		case 0:
			base.room0.kaleidoscopeModeCross = true;
			base.room0.camera.clearFlags = (RDClass.Vfx.infiniteZoom ? CameraClearFlags.Color : CameraClearFlags.Depth);
			base.room0.mKaleidoscopeCross.kaleidoCam.depth = base.room0.camera.depth - 1f;
			base.room0.mKaleidoscopeCross.kaleidoCam.targetTexture = base.room0.renderTexture;
			base.room0.mKaleidoscopeCross.kaleidoCam.GetComponent<VideoBloom>().enabled = true;
			base.room0.mKaleidoscopeCross.particleSystems[3].gameObject.SetActive(value: true);
			break;
		case 1:
			base.room0.kaleidoscopeModeCross = false;
			base.room0.camera.clearFlags = CameraClearFlags.Color;
			base.room0.mKaleidoscopeCross.kaleidoCam.depth = base.room0.camera.depth - 1f;
			base.room0.mKaleidoscopeCross.kaleidoCam.targetTexture = base.room0.renderTexture;
			base.room0.mKaleidoscopeCross.particleSystems[3].gameObject.SetActive(value: false);
			break;
		}
	}

	public override void preactions()
	{
		if (base.conductor.barNumber != 1)
		{
			return;
		}
		base.room2.rotatingVoxelMode = true;
		base.game.MakeRow(RowType.Classic, 2, 1f, Character.Samurai, RDPlayer.P1, RDPlayer.P1, -1, isShadow: false, 1);
		RowEntity ent = base.rows[2].ent;
		ent.SetAdditiveColor(base.gc.techno_additive0);
		ent.SetOverlayColor(base.gc.techno_overlay0);
		if (GC.twoPlayerMode)
		{
			ChangeCharacter("SamuraiBlue", 2);
			base.game.MakeRow(RowType.Classic, 3, 1f, Character.Samurai, RDPlayer.P1, RDPlayer.P2, -1, isShadow: false, 1);
			if (halloweenWeek)
			{
				ChangeCharacter("SamuraiPirate", 3);
			}
			RowEntity ent2 = base.rows[3].ent;
			ent2.SetAdditiveColor(base.gc.techno_additive0);
			ent2.SetOverlayColor(base.gc.techno_overlay0);
			ent2.Reposition((int)(ent.y - 19f), instant: true);
			ent2.customPosition = true;
			base.room1.RepositionRowsAndStrips(instant: true);
		}
		else if (halloweenWeek)
		{
			ChangeCharacter("SamuraiPirate", 2);
		}
		base.room0.SetRowContainerY(-30f);
		base.room1.SetRowContainerY(-30f);
		samuraiWard.ShowLanterns();
		bassDropTimeSlow = scrExecuteOnHit.Add(() =>
		{
			RDClass.Vfx.BassDropNew(0);
			DOTween.Sequence().AppendInterval(0.05f).AppendCallback(() =>
			{
				base.game.SetVisualSpeed(0.1f);
			});
		}, HitType.Hit, persistent: true, autoDelete: false, -1, overwrite: false);
		bassDropTimeSlow.isActive = false;
	}

	public override void actions()
	{
		switch (base.conductor.barNumber)
		{
		case 5:
			if (halloweenWeek)
			{
				int rowID = (GC.twoPlayerMode ? 1 : 0);
				ChangeCharacter("SamuraiPirate", rowID);
			}
			break;
		case 6:
			samuraiWard.gameObject.SetActive(value: false);
			base.room0.rowContainer.DOMoveY(0f, 1f).SetEase(Ease.OutBack);
			base.room0.rotatingVoxelMode = true;
			base.room2.rotatingVoxelMode = false;
			if (!base.game.reducedFlash)
			{
				RDClass.Vfx.Flash();
			}
			base.tkCam.Shake(20, 10);
			break;
		case 8:
		case 12:
			base.tkCam.Shake(20, 10);
			break;
		case 31:
			base.game.DeleteRow(0);
			if (GC.twoPlayerMode)
			{
				base.game.DeleteRow(1);
			}
			break;
		}
	}

	private void EnterWhitePart(bool enter)
	{
		RDHueShift component = RDClass.Vfx.hudCamera.GetComponent<RDHueShift>();
		if (enter)
		{
			component.enabled = true;
			RDClass.Vfx.BgColor(Color.white);
			Color overlayColor = new Color(0.83f, 0.4f, 0.4f, 1f);
			base.rowEnt1.SetOverlayColor(overlayColor);
			cycler = scrExecuteOnEveryHB.Add(HueShiftCycler);
		}
		else
		{
			component.enabled = false;
			RDClass.Vfx.BgColor(1341733375u.RGBAToColor());
			cycler.Remove();
		}
	}

	private void HueShiftCycler()
	{
		int num = currentCycleColorID;
		while (currentCycleColorID == num)
		{
			currentCycleColorID = Mathf.FloorToInt(Random.value * (float)ColorPalette.Length);
		}
		Color rgbColor = ColorPalette[currentCycleColorID].ARGBToColor();
		float H = 0f;
		float S = 0f;
		float V = 0f;
		Color.RGBToHSV(rgbColor, out H, out S, out V);
		base.rowEnt1.SetOverlayColor(new Color(0.83f, 0.4f, 0.4f, 1f));
		RDHueShift component = RDClass.Vfx.hudCamera.GetComponent<RDHueShift>();
		component.enabled = true;
		component.Hue = H;
	}

	public void EnableBassDropTimeSlow()
	{
		bassDropTimeSlow.isActive = true;
	}

	public void DisableBassDropTimeSlow()
	{
		bassDropTimeSlow.isActive = false;
	}

	public void SetToZenGarden()
	{
		base.room0.voxelObj.SetVoxelMode(RotatingVoxelMode.ZenGarden);
	}

	public void SetToHeart()
	{
		base.room0.voxelObj.SetVoxelMode(RotatingVoxelMode.Heart);
	}

	public void SetToBrokenHeart()
	{
		base.room0.voxelObj.SetVoxelMode(RotatingVoxelMode.BrokenHeart);
		base.room0.voxelObj.SetCubeMode(enableCubes: true);
	}

	public void OpenDoors()
	{
		base.game.StartCoroutine(samuraiWard.OpenDoors(opened: true, instant: false, lightLanterns: false));
	}

	public void SpeedUpRoom3Heart(float duration)
	{
		int num = 6;
		float duration2 = base.conductor.DurationBeatsToTime(num, duration);
		DOTween.To(() => base.room2.rotatingVoxelSpeed, (float s) =>
		{
			base.room2.rotatingVoxelSpeed = s;
		}, 1280f, duration2).SetEase(Ease.InSine);
	}
}
