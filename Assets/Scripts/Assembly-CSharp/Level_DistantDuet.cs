using System.Collections;
using System.Linq;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_DistantDuet : LevelBase
{
	private Background sky;

	private Background cityFront;

	private Background cityBack;

	private Background cityFront2;

	private Background cityBack2;

	private Background[] skyBackgrounds;

	private Background[] skyBackgrounds2;

	private Background[] allSkyBackgrounds;

	private float skySpeedMult = 1f;

	private Tween skySpeedTween;

	public Level_DistantDuet(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		string text = ((data.settings.description == "nightshift") ? "_Blue/0" : "");
		sky = Background.Create("Intimate_SkySS" + text, base.gc.intimate_skyScrollX);
		cityBack = Background.Create("Intimate_CityBackSS" + text, base.gc.intimate_cityBackScrollX);
		cityBack.SetTileType(TileType.Horizontal);
		cityBack2 = Background.Create("Intimate_CityBackSS", base.gc.intimate_cityBackScrollX);
		cityBack2.SetTileType(TileType.Horizontal);
		cityFront = Background.Create("Intimate_CityFrontSS" + text, base.gc.intimate_cityFrontScrollX);
		cityFront.SetTileType(TileType.Horizontal);
		cityFront2 = Background.Create("Intimate_CityFrontSS", base.gc.intimate_cityFrontScrollX);
		cityFront2.SetTileType(TileType.Horizontal);
		skyBackgrounds = new Background[3] { sky, cityBack, cityFront };
		skyBackgrounds2 = new Background[2] { cityBack2, cityFront2 };
		allSkyBackgrounds = skyBackgrounds.Concat(skyBackgrounds2).ToArray();
		Background[] array = skyBackgrounds;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].visible = true;
		}
		array = skyBackgrounds2;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].visible = false;
		}
		runFirstActionBarInPreactions = true;
		yield return null;
		array = skyBackgrounds2;
		foreach (Background background in array)
		{
			background.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
			background.transform.localPosition = background.transform.localPosition.WithY(60f);
			background.SetGreyscale(greyscale: true);
		}
		yield return null;
	}

	public void SetNightMode()
	{
		RDMainWard[] array = Object.FindObjectsOfType<RDMainWard>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetNightMode(night: true);
		}
	}

	public void DDHandSetup(bool isNightShift)
	{
		if (!GC.twoPlayerMode)
		{
			if (!isNightShift)
			{
				SetHandToPaige(0, rightHand: false);
			}
			SetHandToPaige(2, rightHand: false);
		}
		if (GC.twoPlayerMode)
		{
			base.game.handController2.UpdateHandsPositions(base.game.GetP2HandUpdateType());
		}
		else
		{
			base.game.handController2.UpdateHandsPositions(HandUpdateType.LeftHand);
		}
		base.game.handController2.transform.localPosition = base.game.handController2.transform.localPosition.WithY(-25f);
		base.game.handController1.UpdateHandsPositions(base.game.GetP1HandUpdateType());
		base.game.handController1.transform.localPosition = base.game.handController1.transform.localPosition.WithY(-25f);
	}

	public void GlowHandsOn()
	{
		SetAllHandsBorderColor(1f, 1f, 1f, 1f, 0);
	}

	public void GlowHandsOff()
	{
		SetAllHandsBorderColor(0f, 0f, 0f, 0f, 0);
	}

	public void Room1HandSetup(bool isNightShift)
	{
		base.game.handController0.Show(show: true, instant: true);
		base.game.handController0.UpdateHandsPositions((!isNightShift || GC.twoPlayerMode) ? HandUpdateType.BothHands : HandUpdateType.RightHand, 0f);
	}

	public void HidePaigeHand()
	{
		if (!GC.twoPlayerMode)
		{
			base.game.handController2.leftArm.Toggle(HandAction.Hide, 0.75f);
		}
	}

	[ListedMethod(false)]
	public void SetSkySpeed(float speed, float durBeats, string easeStr)
	{
		skySpeedTween?.Kill(complete: true);
		if (durBeats == 0f)
		{
			skySpeedMult = speed;
			UpdateSkyScroll();
			return;
		}
		float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats, calledCloseToNextBarMaybe: true);
		Ease ease = RDUtils.ParseEnum(easeStr, Ease.Linear);
		skySpeedTween = DOTween.To(() => skySpeedMult, (float s) =>
		{
			skySpeedMult = s;
		}, speed, duration).SetEase(ease).OnUpdate(() =>
		{
			UpdateSkyScroll();
		})
			.OnComplete(() =>
			{
				skySpeedMult = speed;
				UpdateSkyScroll();
			});
	}

	private void UpdateSkyScroll()
	{
		sky.velocity = sky.velocity.WithX(base.gc.intimate_skyScrollX * skySpeedMult);
		cityBack.velocity = cityBack.velocity.WithX(base.gc.intimate_cityBackScrollX * skySpeedMult);
		cityBack2.velocity = cityBack2.velocity.WithX(base.gc.intimate_cityBackScrollX * skySpeedMult);
		cityFront.velocity = cityFront.velocity.WithX(base.gc.intimate_cityFrontScrollX * skySpeedMult);
		cityFront2.velocity = cityFront2.velocity.WithX(base.gc.intimate_cityFrontScrollX * skySpeedMult);
	}

	[ListedMethod(false)]
	public void ToggleSecondSky(bool on)
	{
		Background[] array = skyBackgrounds2;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].visible = on;
		}
	}

	[ListedMethod(false)]
	public void ToggleGreySky(bool on)
	{
		Background[] array = skyBackgrounds;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetGreyscale(on);
		}
	}
}
