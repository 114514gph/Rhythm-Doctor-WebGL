using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class RDAthleteWard : RDEnvironment
{
	private enum BaseballTrajectoryType
	{
		Field = 0,
		Screen = 1,
		Foul = 2
	}

	private delegate void LightMethod(int index, bool on);

	private delegate void AllLightMethod(bool on);

	public enum Winner
	{
		Home = 0,
		Guest = 1,
		Tie = 2
	}

	[Header("Baseball Components")]
	public Perspective2DRoom perspective2DRoom;

	public GameObject baseballPrefab;

	public GameObject specialBaseballPrefab;

	public Transform baseballStartMarker;

	public Transform baseballEndMarker;

	public Transform baseballScreenMarker;

	public Transform baseballHolder;

	private Dictionary<Beat, Transform> baseballDict = new Dictionary<Beat, Transform>();

	private Dictionary<Beat, Sequence> sequenceDict = new Dictionary<Beat, Sequence>();

	private float baseballAngleMin;

	private float baseballAngleMax;

	private float prevBaseballAngle;

	private BaseballTrajectoryType baseballTrajectory;

	[Header("Scoreboard Components")]
	public SevSegLight[] homeLights;

	public SevSegLight[] guestLights;

	public GameObject[] otherLights;

	public GameObject[] lightContainers;

	private bool _trackingScore;

	private bool _usingLights;

	private bool _glitchyLights;

	private const float blinkDur = 0.2f;

	private const int blinkLoopCount = 3;

	private const float glitchDur = 0.3846f;

	[NonSerialized]
	public const int LAST_INNING_LIGHT_INDEX = 8;

	[NonSerialized]
	public const int RUN_LIGHTS_INDEX = 9;

	[NonSerialized]
	public const int HIT_LIGHTS_INDEX = 11;

	[NonSerialized]
	public const int ERR_LIGHT_INDEX = 13;

	private int currentHomeScore;

	private int currentHomeRuns;

	private int currentGuestScore;

	private int currentGuestRuns;

	private int currentInningIndex;

	private int ballCount;

	private int strikeCount;

	private int outCount;

	private Tween[] lightBlinkingTweens = new Tween[3];

	private Tween glitchTween;

	[Header("Components")]
	public GameObject physioWardContainer;

	public GameObject stadiumContainer;

	public GameObject stadiumDayContainer;

	public GameObject stadiumStormContainer;

	public RDEnvironment stadiumEnvironment;

	public tk2dSpriteAnimator grass;

	public tk2dSpriteAnimator bird;

	public tk2dSpriteAnimator scoreboard;

	public tk2dSpriteAnimator stormClouds;

	public tk2dSpriteAnimator lightning;

	public tk2dSpriteAnimator team;

	public BaseballRainSpawner rain;

	public Color stormTint;

	public tk2dSprite physioBg;

	public tk2dSprite physioFg;

	public tk2dSprite physioEdega;

	public tk2dSprite[] physioLightingSprites;

	private float[] physioLightingAlphas;

	public GameObject physioTheathreFg;

	public GameObject physioTheatreFgDark;

	public bool trackingScore
	{
		get
		{
			return _trackingScore;
		}
		set
		{
			_trackingScore = value;
			if (value && !usingLights)
			{
				usingLights = true;
				ClearAllLights();
			}
			UpdateScoreboardToCurrentScore();
		}
	}

	public bool usingLights
	{
		get
		{
			return _usingLights;
		}
		set
		{
			_usingLights = value;
			GameObject[] array = lightContainers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(_usingLights);
			}
		}
	}

	public bool glitchyLights
	{
		get
		{
			return _glitchyLights;
		}
		set
		{
			_glitchyLights = value;
			glitchTween?.Kill();
			if (value)
			{
				if (!usingLights)
				{
					usingLights = true;
				}
				glitchTween = DOTween.Sequence().AppendCallback(() =>
				{
					RandomizeLights();
				}).AppendInterval(0.3846f)
					.SetLoops(-1);
			}
		}
	}

	public void HitBaseball(HitType hittype, Beat beat)
	{
		if (hittype != HitType.Hit)
		{
			return;
		}
		AnimateGrass();
		AnimateBird();
		if (!baseballDict.TryGetValue(beat, out var value))
		{
			return;
		}
		BeatOneshot beatOneshot = beat as BeatOneshot;
		value.GetComponent<SpeedTrail>().enabled = true;
		Perspective2DSprite component = value.GetComponent<Perspective2DSprite>();
		Vector2 screenPosition = (Vector2)(beat.row.ent.character.transform.position - perspective2DRoom.transform.position) + new Vector2(13f, -4f);
		Vector3 projectedPosition = perspective2DRoom.GetProjectedPosition(component, screenPosition, Vector3.one);
		float num = 0f;
		float duration = 0.5f / RDTime.speed * base.game.visualSpeed;
		TweenCallback tweenCallback = () =>
		{
			DestroyBaseball(beat);
		};
		Vector3 endValue;
		if (baseballTrajectory == BaseballTrajectoryType.Screen)
		{
			endValue = perspective2DRoom.GetProjectedPosition(component, baseballScreenMarker.localPosition, baseballScreenMarker.localScale);
			num = -300f;
			tweenCallback = (TweenCallback)Delegate.Combine(tweenCallback, (TweenCallback)(() =>
			{
				base.game.currentLevel.RunEventsWithTag("hitScreen");
			}));
			duration = base.conductor.crotchet;
		}
		else if (baseballTrajectory == BaseballTrajectoryType.Foul)
		{
			Vector3 vector = new Vector3(0f, UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-10f, 10f));
			endValue = perspective2DRoom.GetProjectedPosition(component, baseballEndMarker.localPosition, baseballEndMarker.localScale) + vector;
			num = -2880f;
		}
		else
		{
			float num2 = 0f;
			if (beatOneshot.subdivCount >= 3)
			{
				float t = (float)(beatOneshot.subdivIndex + 1) / (float)beatOneshot.subdivCount;
				num2 = Mathf.Lerp(baseballAngleMin, baseballAngleMax, t);
			}
			else
			{
				float num3 = baseballAngleMax - baseballAngleMin;
				float num4 = 0.33f * num3;
				float num5;
				do
				{
					num5 = UnityEngine.Random.Range(baseballAngleMin, baseballAngleMax);
				}
				while (Mathf.Abs(num5 - prevBaseballAngle) < num4);
				num2 = num5;
			}
			float f = (num2 + 140f) * (MathF.PI / 180f);
			endValue = new Vector3(Mathf.Sin(f) * 352f, 100f, Mathf.Cos(f) * 352f) + projectedPosition;
			num = -2880f;
			prevBaseballAngle = num2;
		}
		component.position = projectedPosition;
		sequenceDict[beat].Kill();
		Sequence value2 = DOTween.Sequence().Append(component.DOMove(endValue, duration).SetEase(Ease.Linear)).Join(value.DORotate(Vector3.forward * num, duration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear))
			.OnComplete(tweenCallback);
		sequenceDict[beat] = value2;
	}

	public void PrepareBaseballs(int bar)
	{
		foreach (Beat beat in base.game.beats)
		{
			if (!(beat is BeatOneshot))
			{
				continue;
			}
			BeatOneshot oneshot = beat as BeatOneshot;
			if (oneshot.bar != bar || oneshot.unhittable)
			{
				continue;
			}
			Transform ball = UnityEngine.Object.Instantiate(baseballPrefab, baseballHolder).transform;
			Perspective2DSprite ball3D = ball.GetComponent<Perspective2DSprite>();
			ball.gameObject.name = "Baseball";
			ball.gameObject.SetActive(value: false);
			float num = 0.39f;
			float num2 = 0.065f;
			float dur = num + num2;
			double actionTime = oneshot.chakTimeAbs - (double)(num / RDTime.speed);
			scrExecuteOnCertainTime.Add(() =>
			{
				ball.gameObject.SetActive(value: true);
				Sequence value = ThrowBaseball(ball3D, beat.row.ent, dur).OnComplete(() =>
				{
					DestroyBaseball(oneshot);
				});
				sequenceDict.Add(beat, value);
			}, actionTime);
			baseballDict.Add(beat, ball);
		}
	}

	public Sequence ThrowBaseball(Perspective2DSprite ball3D, RowEntity targetCharacter, float duration)
	{
		duration = duration / RDTime.speed * base.game.visualSpeed;
		Transform transform = ball3D.transform;
		Vector3 projectedPosition = perspective2DRoom.GetProjectedPosition(ball3D, baseballStartMarker.localPosition, baseballStartMarker.localScale);
		Vector3 eulerAngles = Vector3.forward * UnityEngine.Random.Range(0f, 360f);
		Vector3 vector = new Vector3(UnityEngine.Random.Range(-2f, 2f), 0f, UnityEngine.Random.Range(-2f, 2f));
		Vector3 endValue = Vector3.forward * 360f * 5f;
		float jumpPower = 40f;
		ball3D.position = projectedPosition;
		transform.eulerAngles = eulerAngles;
		Character character = targetCharacter.character.character;
		Vector2 vector2 = ((character == Character.Athlete || character == Character.AthleteAlt) ? new Vector2(17f, -16f) : new Vector2(17f, -13f));
		Vector2 screenPosition = (Vector2)(targetCharacter.character.transform.position - perspective2DRoom.transform.position) + vector2;
		Vector3 vector3 = perspective2DRoom.GetProjectedPosition(ball3D, screenPosition, Vector2.one) + vector;
		return DOTween.Sequence().Append(ball3D.DOJump(vector3, jumpPower, 1, duration).SetEase(Ease.Linear).Join(transform.DORotate(endValue, duration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear))).Append(ball3D.DOJump(vector3 + new Vector3(-8f, 0f, 20f), 4f, 1, 0.25f).SetEase(Ease.OutBounce))
			.Append(transform.GetComponent<tk2dSprite>().DOFade(0f, 0.5f).SetEase(Ease.Linear));
	}

	public void SetBaseballAngleRange(float minAngle, float maxAngle)
	{
		baseballAngleMin = minAngle;
		baseballAngleMax = maxAngle;
	}

	public void SetBaseballTrajectory(string trajectoryString)
	{
		Enum.TryParse<BaseballTrajectoryType>(trajectoryString, ignoreCase: false, out baseballTrajectory);
	}

	private void DestroyBaseball(Beat beat)
	{
		UnityEngine.Object.Destroy(baseballDict[beat].gameObject);
		baseballDict.Remove(beat);
	}

	public void CustomBaseball(Vector3 startPos, Vector3 endPos, float height, float rotation, float baseballAlpha, float duration, string easeString)
	{
		Transform ball = UnityEngine.Object.Instantiate(baseballPrefab, baseballHolder).transform;
		Perspective2DSprite component = ball.GetComponent<Perspective2DSprite>();
		ball.gameObject.name = "Custom Baseball";
		ball.gameObject.SetActive(value: true);
		Enum.TryParse<Ease>(easeString, out var result);
		component.sprite.color = component.sprite.color.WithAlpha(baseballAlpha);
		component.position = startPos;
		component.DOJump(endPos, height, 1, duration).SetEase(result).OnComplete(() =>
		{
			UnityEngine.Object.Destroy(ball.gameObject);
		});
		ball.eulerAngles = Vector3.forward * UnityEngine.Random.Range(0f, 360f);
		ball.transform.DORotate(Vector3.forward * rotation, duration, RotateMode.LocalAxisAdd);
	}

	public void UpdateScoreboard(HitType hitType, Beat beat, bool home)
	{
		switch (hitType)
		{
		case HitType.Hit:
			AddScore(home);
			break;
		case HitType.JustMiss:
			AddStrike();
			break;
		case HitType.BigMiss:
			AddBall();
			break;
		}
	}

	public void AddScore(bool home)
	{
		if (home)
		{
			currentHomeScore++;
		}
		else
		{
			currentGuestScore++;
		}
		if (!glitchyLights)
		{
			UpdateScoreboardToCurrentScore();
		}
	}

	public void AddRun(bool home)
	{
		if (home)
		{
			currentHomeRuns++;
		}
		else
		{
			currentGuestRuns++;
		}
		if (!glitchyLights)
		{
			UpdateScoreboardToCurrentRuns();
		}
	}

	public void AddBall()
	{
		ballCount++;
		if (!glitchyLights)
		{
			UpdateScoreboardToCurrentValue(0);
		}
	}

	public void AddStrike()
	{
		strikeCount++;
		if (!glitchyLights)
		{
			UpdateScoreboardToCurrentValue(1);
		}
	}

	public void AddOut()
	{
		outCount++;
		if (!glitchyLights)
		{
			UpdateScoreboardToCurrentValue(2);
		}
	}

	private void UpdateScoreboardToCurrentScore()
	{
		string text = (currentHomeScore % 100).ToString("D2");
		SetScoreboardLightsWithString(home: true, text.ToString(), 11);
		if (GC.twoPlayerMode)
		{
			string text2 = (currentGuestScore % 100).ToString("D2");
			SetScoreboardLightsWithString(home: false, text2.ToString(), 11);
		}
	}

	private void UpdateScoreboardToCurrentRuns()
	{
		if (GC.twoPlayerMode)
		{
			int num = currentHomeRuns % 100;
			int num2 = currentGuestRuns % 100;
			SetScoreboardLightsWithString(home: true, num.ToString("D2"), 9);
			SetScoreboardLightsWithString(home: false, num2.ToString("D2"), 9);
		}
	}

	private void UpdateScoreboardToCurrentValue(int valMode)
	{
		Tween tween = lightBlinkingTweens[valMode];
		LightMethod lightMethod = valMode switch
		{
			1 => SetStrikeLight, 
			0 => SetBallLight, 
			_ => SetOutLight, 
		};
		AllLightMethod targetAllLightMethod = valMode switch
		{
			1 => SetAllStrikeLights, 
			0 => SetAllBallLights, 
			_ => SetAllOutLights, 
		};
		int num = valMode switch
		{
			1 => strikeCount, 
			0 => ballCount, 
			_ => outCount, 
		};
		int num2 = ((valMode == 0) ? 4 : 3);
		int num3 = num % num2;
		if (num3 == 0 && num > 0)
		{
			tween = DOTween.Sequence().AppendCallback(() =>
			{
				targetAllLightMethod(on: true);
			}).AppendInterval(0.2f)
				.AppendCallback(() =>
				{
					targetAllLightMethod(on: false);
				})
				.AppendInterval(0.2f)
				.SetLoops(3);
			if (valMode == 1)
			{
				AddOut();
			}
		}
		else
		{
			tween?.Kill();
			for (int num4 = 0; num4 < num2 - 1; num4++)
			{
				lightMethod(num4, num4 < num3);
			}
		}
	}

	public void SetScoreboardLightWithChar(bool home, int index, char value)
	{
		if (usingLights && index >= 0 && index <= 13)
		{
			(home ? homeLights : guestLights)[index].SetLightWithChar(value);
		}
	}

	public void SetScoreboardLightWithInt(bool home, int index, int value)
	{
		if (usingLights && value >= 0 && value < 8)
		{
			SetScoreboardLightWithChar(home, index, value.ToString()[0]);
		}
	}

	public void SetScoreboardLightsWithString(bool home, string value, int startIndex = 0, bool eraseLightsToRight = true)
	{
		if (!usingLights)
		{
			return;
		}
		SevSegLight[] array = (home ? homeLights : guestLights);
		int num = (eraseLightsToRight ? array.Length : Math.Min(value.Length, array.Length));
		for (int i = startIndex; i < num; i++)
		{
			int num2 = i - startIndex;
			if (num2 < value.Length)
			{
				array[i].SetLightWithChar(value[num2]);
			}
			else
			{
				array[i].ClearLight();
			}
		}
	}

	public void SetBallLight(int index, bool on)
	{
		if (usingLights && index >= 0 && index <= 2)
		{
			otherLights[index].SetActive(on);
		}
	}

	public void SetAllBallLights(bool on)
	{
		if (usingLights)
		{
			for (int i = 0; i < 3; i++)
			{
				otherLights[i].SetActive(on);
			}
		}
	}

	public void SetStrikeLight(int index, bool on)
	{
		if (usingLights && index >= 0 && index <= 1)
		{
			otherLights[3 + index].SetActive(on);
		}
	}

	public void SetAllStrikeLights(bool on)
	{
		if (usingLights)
		{
			for (int i = 0; i < 2; i++)
			{
				otherLights[3 + i].SetActive(on);
			}
		}
	}

	public void SetOutLight(int index, bool on)
	{
		if (usingLights && index >= 0 && index <= 1)
		{
			otherLights[5 + index].SetActive(on);
		}
	}

	public void SetAllOutLights(bool on)
	{
		if (usingLights)
		{
			for (int i = 0; i < 2; i++)
			{
				otherLights[5 + i].SetActive(on);
			}
		}
	}

	public void AdvanceScoreboardInning()
	{
		if (GC.twoPlayerMode)
		{
			Winner inningLights;
			if (currentHomeScore > currentGuestScore)
			{
				inningLights = Winner.Home;
				AddRun(home: true);
			}
			else if (currentHomeScore < currentGuestScore)
			{
				inningLights = Winner.Guest;
				AddRun(home: false);
			}
			else
			{
				inningLights = Winner.Tie;
				AddRun(home: true);
				AddRun(home: false);
			}
			SetInningLights(inningLights);
			currentHomeScore = 0;
			currentGuestScore = 0;
			UpdateScoreboardToCurrentScore();
		}
	}

	public void SetInningLights(Winner winner)
	{
		SetScoreboardLightWithInt(home: true, currentInningIndex, (winner != Winner.Guest) ? 1 : 0);
		SetScoreboardLightWithInt(home: false, currentInningIndex, (winner != Winner.Home) ? 1 : 0);
		currentInningIndex++;
		if (currentInningIndex > 8)
		{
			currentInningIndex -= 9;
		}
	}

	public void RandomizeLights()
	{
		if (!usingLights)
		{
			return;
		}
		foreach (SevSegLight item in homeLights.Concat(guestLights))
		{
			item.SetLight(UnityEngine.Random.Range(0, 128));
		}
		int num = UnityEngine.Random.Range(0, 128);
		for (int i = 0; i < otherLights.Length; i++)
		{
			otherLights[i].SetActive((num & (1 << i)) != 0);
		}
	}

	public void ClearScoreboardLights(bool home)
	{
		if (usingLights)
		{
			SevSegLight[] array = (home ? homeLights : guestLights);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ClearLight();
			}
		}
	}

	public void ClearAllLights()
	{
		ClearScoreboardLights(home: true);
		ClearScoreboardLights(home: false);
		GameObject[] array = otherLights;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
	}

	private void Awake()
	{
		if (base.enabled)
		{
			int num = physioLightingSprites.Length;
			physioLightingAlphas = new float[num];
			for (int i = 0; i < num; i++)
			{
				physioLightingAlphas[i] = physioLightingSprites[i].color.a;
			}
		}
	}

	public void TweenPhysioLightingAlpha(float alpha, float dur)
	{
		for (int i = 0; i < physioLightingSprites.Length; i++)
		{
			tk2dSprite tk2dSprite2 = physioLightingSprites[i];
			float num = physioLightingAlphas[i] * alpha;
			tk2dSprite2.DOKill();
			if (dur != 0f)
			{
				tk2dSprite2.DOFade(num, dur);
			}
			else
			{
				tk2dSprite2.color = tk2dSprite2.color.WithAlpha(num);
			}
		}
	}

	public void SetStadium(bool stadium)
	{
		physioWardContainer.SetActive(!stadium);
		stadiumContainer.SetActive(stadium);
		if (stadium)
		{
			stadiumEnvironment.StartAmbiences();
			StopAmbiences();
		}
		else
		{
			stadiumEnvironment.StopAmbiences();
			StartAmbiences();
		}
	}

	public void SetScoreboard(int stage)
	{
		scoreboard.SetFrame(stage);
	}

	public void AnimateGrass()
	{
		grass.PlayFrom(0f);
	}

	public void AnimateBird()
	{
		bird.PlayFrom(0f);
	}

	public void AnimateLightning(bool showTeam)
	{
		lightning.PlayFrom(0f);
		if (showTeam)
		{
			team.PlayFrom(0f);
		}
	}

	public void SetTeamStare(bool stare)
	{
		team.SetFrame(stare ? 10 : 0);
		if (stare)
		{
			team.Pause();
		}
		stormClouds.gameObject.SetActive(!stare);
		lightning.gameObject.SetActive(!stare);
		scoreboard.gameObject.SetActive(!stare);
	}

	public void AnimateClouds()
	{
		stormClouds.PlayFrom(0f);
	}

	public void SetRain(float baseballsPerSecond)
	{
		rain.ballsPerSecond = baseballsPerSecond;
	}

	public void KillRain()
	{
		rain.Kill();
	}

	public void SetNightMode(bool night)
	{
		Color white = Color.white;
		Color color = new Color(0.392f, 0.392f, 0.51f, 1f);
		Color color2 = (night ? color : white);
		physioFg.color = color2;
		physioBg.color = color2;
		physioTheathreFg.SetActive(!night);
		physioTheatreFgDark.SetActive(night);
	}

	public void SetStorm(bool storm, bool animate, bool preStorm = false)
	{
		stadiumDayContainer.SetActive(!storm);
		stadiumStormContainer.SetActive(storm);
		if (!preStorm & storm)
		{
			if (animate)
			{
				AnimateClouds();
			}
			else
			{
				stormClouds.PlayFromFrame(stormClouds.DefaultClip.frames.Length - 1);
			}
		}
		if (preStorm)
		{
			stormClouds.StopAndResetFrame();
		}
		Color tint = (storm ? stormTint : Color.white);
		TintBaseball(baseballPrefab, shadowOnly: true);
		TintBaseball(rain.baseballPrefab);
		foreach (Transform value in baseballDict.Values)
		{
			TintBaseball(value.gameObject, shadowOnly: true);
		}
		foreach (GameObject obj in rain.objList)
		{
			TintBaseball(obj.gameObject);
		}
		void TintBaseball(GameObject obj, bool shadowOnly = false)
		{
			tk2dSprite component = obj.GetComponent<tk2dSprite>();
			if (!shadowOnly)
			{
				component.color = tint.WithAlpha(component.color.a);
			}
			component.GetComponent<Perspective2DSprite>().renderShadow = !storm;
		}
	}

	public void ToggleEdega(bool visible, float fadeDuration = 0f)
	{
		physioEdega.gameObject.SetActive(visible);
		int num = (visible ? 1 : 0);
		physioEdega.DOFade(num, fadeDuration).Done();
	}
}
