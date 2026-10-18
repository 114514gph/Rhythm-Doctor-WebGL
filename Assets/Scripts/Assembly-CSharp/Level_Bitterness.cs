using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_Bitterness : LevelBase
{
	private RDHoodieBoyWard _coleWard;

	private RDCoffeeShop _coffeeShop;

	private RDMainWard _frontHospital;

	private RDMainWard _backHospital;

	private RDBossStageText bossText;

	private float brokenLightTimer;

	private bool[] brokenLightsInUse;

	private scrExecuteOnUpdate brokenLightUpdate;

	private int nicoleSortOrder = -1;

	private scrBitternessParticles iconParticles;

	private List<tk2dSprite> bgSprites = new List<tk2dSprite>();

	private float _bgAlpha = 1f;

	private Tween bgAlphaTween;

	private RDHoodieBoyWard coleWard
	{
		get
		{
			if (_coleWard == null)
			{
				_coleWard = base.room0.coleWard.GetComponent<RDHoodieBoyWard>();
			}
			return _coleWard;
		}
	}

	private RDCoffeeShop coffeeShop
	{
		get
		{
			if (_coffeeShop == null)
			{
				_coffeeShop = base.room1.coffeeShop.GetComponent<RDCoffeeShop>();
			}
			return _coffeeShop;
		}
	}

	private RDMainWard frontHospital
	{
		get
		{
			if (_frontHospital == null)
			{
				_frontHospital = base.room2.hospitalWard.GetComponent<RDMainWard>();
			}
			return _frontHospital;
		}
	}

	private RDMainWard backHospital
	{
		get
		{
			if (_backHospital == null)
			{
				_backHospital = base.room3.hospitalWard.GetComponent<RDMainWard>();
			}
			return _backHospital;
		}
	}

	private int lightsNumber => coleWard.lights.Length;

	private float bgAlpha
	{
		get
		{
			return _bgAlpha;
		}
		set
		{
			_bgAlpha = value;
			foreach (tk2dSprite bgSprite in bgSprites)
			{
				if (bgSprite.gameObject.activeSelf)
				{
					bgSprite.color = bgSprite.color.WithAlpha(value);
				}
			}
		}
	}

	public Level_Bitterness(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		checkpointBarsDesc = new int[1] { 97 };
		base.game.barToSkipToOnSlash = 90;
		base.game.hpController.gameObject.SetActive(value: false);
	}

	public override bool FailLevel(RowEntity ent)
	{
		Persistence.SetLevelRank(base.game.levelIdentifier, -1);
		scrChar mainCharacter = (ent.rowID switch
		{
			4 => base.game.rows[0].ent, 
			5 => base.game.rows[2].ent, 
			_ => ent, 
		}).character;
		failedLevel = true;
		RDClass.Vfx.KillFlashTextUIAnimations();
		scrExecuteOnPeriodicBeat.Clear();
		DOTween.KillAll();
		mainCharacter.animator.deltaTimeStyle = tk2dSpriteAnimator.DeltaTimeStyle.AlwaysOneAlways;
		DOVirtual.DelayedCall(2f, () =>
		{
			ent.heart.DoFinalCrack();
			RDUtils.Rotate2D(ent.heart.transform, 0f);
			mainCharacter.PlayExpression("gameover");
		});
		DOVirtual.DelayedCall(3f, () =>
		{
			RDClass.Vfx.SpotlightFocusFromCenter(mainCharacter.transform.position.x, 0.5f, playSwitchSound: true, RDUtils.GetMixerGroup("FeedbackSoundsParent"));
		});
		DOVirtual.DelayedCall(5f, () =>
		{
			base.game.statusText.Show("status.bossFailRestart", narrate: true);
			GameOverBarInfo genericGameOverInfo = new GameOverBarInfo(-1, new int[7] { 1, 1, 1, 1, 1, 1, 1 });
			List<GameOverBarInfo> gameOverBars = new List<GameOverBarInfo>
			{
				new GameOverBarInfo(97, new int[6] { 1, 1, 1, 1, 1, 1 })
			};
			PlayGameOverDialogue(gameOverBars, genericGameOverInfo);
		});
		RunEventsWithTag("FailLevel");
		return true;
	}

	public override IEnumerator LoadBigAssets()
	{
		iconParticles = LoadLevelAsset("RDHoodieBoyWard/Bitterness/BitternessParticles").GetComponent<scrBitternessParticles>();
		iconParticles.rainSystem.transform.LocalMoveXY(3000f + RDClass.Vfx.RDWidth * 0.5f, RDClass.Vfx.RDHeight + 8f);
		iconParticles.fastRainSystem.transform.LocalMoveXY(3000f + RDClass.Vfx.RDWidth * 0.5f, RDClass.Vfx.RDHeight + 8f);
		iconParticles.ToggleAllSystemsActive(toggleActive: false);
		bossText = LoadLevelAsset<RDBossStageText>();
		bossText.gameObject.SetActive(value: false);
		levelType = LevelType.Boss;
		dontKillWindowDanceOnGameOverUntilExit = true;
		glitchObstructionOnGameOverExit = true;
		base.ink.Load("diaBitterness");
		yield return null;
	}

	public override void OnMistake(RowEntity ent = null)
	{
		if (base.numMistakes > missesToCrackHeart)
		{
			base.game.FailLevel((ent == null) ? base.game.rows[0].ent : ent);
		}
	}

	public override void preactions()
	{
		if (base.conductor.barNumber != 1)
		{
			return;
		}
		MeshRenderer[] componentsInChildren = frontHospital.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			if (meshRenderer.sortingLayerName != "Foreground")
			{
				meshRenderer.gameObject.SetActive(value: false);
			}
		}
		SpriteRenderer[] componentsInChildren2 = frontHospital.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren2)
		{
			if (spriteRenderer.sortingLayerName != "Foreground")
			{
				spriteRenderer.gameObject.SetActive(value: false);
			}
		}
		componentsInChildren = backHospital.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer2 in componentsInChildren)
		{
			if (meshRenderer2.sortingLayerName == "Foreground")
			{
				meshRenderer2.gameObject.SetActive(value: false);
			}
		}
		componentsInChildren2 = backHospital.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer2 in componentsInChildren2)
		{
			if (spriteRenderer2.sortingLayerName == "Foreground")
			{
				spriteRenderer2.gameObject.SetActive(value: false);
			}
		}
		brokenLightsInUse = new bool[lightsNumber];
		missesToCrackHeart = rankLowerBounds[1];
		if (base.game.windowChoreographer != null)
		{
			WindowChoreographer windowChoreographer = base.game.windowChoreographer;
			int num = Mathf.RoundToInt(352 * windowChoreographer.GetWindowScaleForDancing());
			float num2 = windowChoreographer.WindowDanceResolution.x;
			float num3 = (float)Mathf.RoundToInt(num2 * 0.3f) + (float)num / 2f;
			float num4 = (float)Mathf.RoundToInt(num2 * 0.7f) - (float)num / 2f;
			b6 = num3 > num4;
		}
	}

	public void ShowBossText(int room, float xPerc, float yPerc)
	{
		if (!base.conductor.isScrubbing)
		{
			bossText.gameObject.SetActive(value: true);
			float x = 3000f * (float)room + RDClass.Vfx.RDWidth * (xPerc / 100f);
			float y = RDClass.Vfx.RDHeight * (yPerc / 100f);
			bossText.transform.localPosition = new Vector3(x, y, 20f);
			bossText.Play(1f, 0.05f);
		}
	}

	public void HideBossText()
	{
		bossText.gameObject.SetActive(value: false);
	}

	public void InfectAll()
	{
		Row[] array = base.rows;
		foreach (Row row in array)
		{
			if (row.ent != null)
			{
				row.ent.heart.QuickInfect();
			}
		}
	}

	public void PlayGlitchNoise()
	{
		scrConductor.PlayImmediately("sndNoiseMed", 0.5f);
	}

	public void ToggleColeSpectrum(bool on)
	{
		coleWard.showSpectrum = on;
	}

	public void ToggleColeLights(bool on)
	{
		coleWard.ToggleAllLights(on);
	}

	public void TurnOnColeLight(int index)
	{
		coleWard.TurnOnLight(index);
	}

	public void IncrementColeLightCounter(bool lightOn)
	{
		coleWard.IncrementLight(lightOn);
	}

	public void SetColeLightCounter(int index, bool lightOn)
	{
		coleWard.SetIncrementalLightCounter(index, lightOn);
	}

	public void StartBrokenLights()
	{
		brokenLightUpdate = scrExecuteOnUpdate.Add(() =>
		{
			brokenLightTimer -= Time.deltaTime;
			if (brokenLightTimer <= 0f)
			{
				int num = Random.Range(0, lightsNumber);
				int num2 = 0;
				while (brokenLightsInUse[num])
				{
					num = Random.Range(0, lightsNumber);
					num2++;
					if (num2 > 1000)
					{
						Debug.Log("Loop broke >:(");
						break;
					}
				}
				AnimateBrokenLight(num);
				brokenLightTimer = Random.Range(0.5f, 4f);
			}
		});
	}

	private void AnimateBrokenLight(int index)
	{
		if (index < lightsNumber)
		{
			brokenLightsInUse[index] = true;
			tk2dSprite lightSpr = coleWard.lights[index].GetComponent<tk2dSprite>();
			lightSpr.gameObject.SetActive(value: true);
			lightSpr.color = lightSpr.color.WithAlpha(0f);
			float duration = Random.Range(0.33f, 0.66f);
			int num = Random.Range(3, 6);
			DOTween.Sequence().Append(lightSpr.DOFade(1f, duration).SetEase(Ease.Flash, 2 * num + 1, 0f)).AppendInterval(Random.Range(1f, 2f))
				.Append(lightSpr.DOFade(0f, duration).SetEase(Ease.Flash, 2 * num + 1, 0f))
				.OnComplete(() =>
				{
					lightSpr.gameObject.SetActive(value: false);
					brokenLightsInUse[index] = false;
				});
		}
	}

	public void StopBrokenLights()
	{
		brokenLightUpdate?.Remove();
		ToggleColeLights(on: false);
	}

	private void ShowOrHideCustomers(int from, int to, bool hide)
	{
		coffeeShop.ShowOrHideCustomers(from, to, hide, 0f);
	}

	public void HideCustomers(int from, int count)
	{
		int to = from + count - 1;
		ShowOrHideCustomers(from, to, hide: true);
	}

	public void ShowCustomers(int from, int count)
	{
		int to = from + count - 1;
		ShowOrHideCustomers(from, to, hide: false);
	}

	public void RandomizeCustomers(int from, int count)
	{
		int to = from + count - 1;
		coffeeShop.RandomizeCustomers(from, to, 0f);
	}

	public void MoveNicoleLayer(bool bg)
	{
		tk2dSprite sprite = base.rows[2].ent.character.sprite;
		if (nicoleSortOrder == -1)
		{
			nicoleSortOrder = sprite.SortingOrder;
		}
		sprite.SortingOrder = (bg ? 6 : nicoleSortOrder);
		sprite.GetComponent<MeshRenderer>().sortingLayerName = (bg ? "Background" : "Default");
	}

	public void ToggleCoffeeNightLights(bool on)
	{
		coffeeShop.lights_night.gameObject.SetActive(on);
	}

	public void ToggleIconRain(bool toggle)
	{
		ParticleSystem rainSystem = iconParticles.rainSystem;
		rainSystem.gameObject.SetActive(toggle);
		if (toggle)
		{
			rainSystem.Play();
		}
		else
		{
			rainSystem.Stop();
		}
	}

	public void ToggleFastIconRain(bool toggle)
	{
		ParticleSystem fastRainSystem = iconParticles.fastRainSystem;
		fastRainSystem.gameObject.SetActive(toggle);
		if (toggle)
		{
			fastRainSystem.Play();
		}
		else
		{
			fastRainSystem.Stop();
		}
	}

	public void DeactivateFastIconRain()
	{
		iconParticles.fastRainSystem.Stop();
	}

	public void SetIconBurstPos(float xPerc, float yPerc)
	{
		iconParticles.burstSystem.transform.LocalMoveXY(RDClass.Vfx.RDWidth * (xPerc / 100f), RDClass.Vfx.RDHeight * (yPerc / 100f));
	}

	public void ToggleIconBurst(bool toggle)
	{
		iconParticles.burstSystem.gameObject.SetActive(toggle);
	}

	public void EmitIconBurst(int amount)
	{
		iconParticles.burstSystem.Emit(amount);
	}

	public void KillAllIconParticles()
	{
		iconParticles.ToggleAllSystemsActive(toggleActive: false);
	}

	public void TweenBGAlpha(float alphaPercent, float durBeats)
	{
		bgAlphaTween?.Kill();
		bgSprites.Clear();
		tk2dSprite[] componentsInChildren = coleWard.GetComponentsInChildren<tk2dSprite>(includeInactive: true);
		foreach (tk2dSprite item in componentsInChildren)
		{
			bgSprites.Add(item);
		}
		componentsInChildren = coffeeShop.GetComponentsInChildren<tk2dSprite>(includeInactive: true);
		foreach (tk2dSprite tk2dSprite2 in componentsInChildren)
		{
			if (!coffeeShop.customers.Contains(tk2dSprite2))
			{
				bgSprites.Add(tk2dSprite2);
			}
		}
		float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
		bgAlphaTween = DOTween.To(() => bgAlpha, (float a) =>
		{
			bgAlpha = a;
		}, alphaPercent / 100f, duration).Done();
	}

	public void SetHospitalX(float x)
	{
		for (int i = 0; i < 4; i++)
		{
			base.game.rooms[i].hospitalWard.transform.localPosition = new Vector3(x + 3000f * (float)i, 99f, -50f);
		}
	}

	public void SetColeHospitalX(float x)
	{
		base.game.rooms[0].hospitalWard.transform.localPosition = new Vector3(x, 99f, -50f);
	}

	public void SetNicoleHospitalX(float x)
	{
		base.game.rooms[1].hospitalWard.transform.localPosition = new Vector3(x + 3000f, 99f, -50f);
	}

	public void SneakyKaleidoscope(int room)
	{
		base.game.rooms[room].ShowThemeBackgrounds(RDTheme.Kaleidoscope, disableEffects: false, hideOtherBackgrounds: false, affectRowContainers: false, firstRowOnFloorHack: false, paintRows: false);
	}
}
