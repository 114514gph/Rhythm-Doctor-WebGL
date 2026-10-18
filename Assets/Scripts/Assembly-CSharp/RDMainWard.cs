using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RDMainWard : RDEnvironment
{
	public const float seamlessWidth = 510f;

	private const float seamlessLeftCutoff = 1505f;

	private const float seamlessRightCutoff = 1347f;

	private const float seamlessRightEdge = 995f;

	public GameObject leftEnd;

	public GameObject rightEnd;

	public GameObject seamless;

	public GameObject seamlessExtra;

	public GameObject seamlessForeground;

	public GameObject seamlessExtraForeground;

	public GameObject[] halloweenObjects;

	public GameObject[] fireplaceObjects;

	public tk2dSpriteAnimator basementDoor;

	public tk2dSpriteAnimator officeDoor;

	public tk2dSpriteAnimator elevator;

	public tk2dSpriteAnimator halloweenShadow;

	public GameObject dogWardLeft;

	public GameObject dogWardRight;

	public GameObject dogWardSeamless;

	public GameObject dogWardSeamlessExtra;

	public GameObject rightWallFloor;

	public GameObject seamlessWallFloor;

	public GameObject seamlessExtraWallFloor;

	public GameObject svtBlockage;

	public GameObject rooftopBlockade;

	public GameObject sleepingPaige;

	public tk2dSprite[] tk2dSprites;

	public WallClock[] clocks;

	public tk2dSprite[] lights;

	public tk2dSprite[] tvs;

	public RDAmbienceAudioSource[] tvAmbience;

	public Color baseLightColor;

	public Color halloweenLightColor;

	public tk2dSprite blueLights;

	public tk2dSprite vendingMachineLights;

	public AnimationCurve nightTimeCurve;

	public AnimationCurve blueLightsCurve;

	public AnimationCurve vendingMachineLightsCurve;

	public float blueLightsDuration;

	public float vendingMachineLightsDuration;

	private float scrollCurrentX;

	public float scrollOffsetX;

	public float scrollSpeed;

	private float seamlessOffsetX;

	[NonSerialized]
	public int scrollRoom;

	public bool legacyScroll;

	private float legacyScrollStartX;

	private float legacyScrollDistanceTraveled;

	public tk2dSprite[] vendingMachineSprites;

	public Transform objectsContainer;

	private bool nightMode;

	private bool _halloweenMode;

	private Color _foregroundTintColor;

	private List<tk2dSprite> foregroundList;

	public GameObject walkingBgContainer;

	private RDBGWalkingChar[] walkingChars;

	public RDBGWalkingChar walkingSamurai;

	private List<RDBGWalkingChar> possibleWalkingChars = new List<RDBGWalkingChar>();

	private List<RDBGWalkingChar> currentlyMovingChars = new List<RDBGWalkingChar>();

	private RDBGWalkingChar lastCharSelected;

	private bool startedScrolling;

	private bool isSeamless;

	public bool dogMode
	{
		set
		{
			dogWardLeft.SetActive(value);
			dogWardRight.SetActive(value);
			dogWardSeamless.SetActive(value);
			dogWardSeamlessExtra.SetActive(value);
			seamlessWallFloor.SetActive(!value);
			seamlessExtraWallFloor.SetActive(!value);
			rightWallFloor.SetActive(!value);
			if (value)
			{
				ToggleFireplace(enabled: false);
			}
		}
	}

	public bool halloweenMode
	{
		get
		{
			return _halloweenMode;
		}
		set
		{
			bool flag = scnLevelSelect.instance != null;
			GameObject[] array = halloweenObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value & flag);
			}
			Color color = ((nightMode & value) ? halloweenLightColor : baseLightColor);
			tk2dSprite[] array2 = lights;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].color = color;
			}
			_halloweenMode = value;
		}
	}

	public Color foregroundTintColor
	{
		get
		{
			return _foregroundTintColor;
		}
		set
		{
			DoThis(value);
		}
	}

	private void Awake()
	{
		List<tk2dSprite> list = new List<tk2dSprite>();
		GameObject[] array = new GameObject[4] { leftEnd, rightEnd, seamless, seamlessExtra };
		for (int i = 0; i < array.Length; i++)
		{
			foreach (Transform item in array[i].transform)
			{
				if (item.gameObject.name.Contains("Foreground"))
				{
					list.Add(item.GetComponent<tk2dSprite>());
				}
			}
		}
		foregroundList = list;
		walkingChars = walkingBgContainer.GetComponentsInChildren<RDBGWalkingChar>(includeInactive: true);
		dogMode = false;
	}

	protected override void Start()
	{
		base.Start();
		AnimateSpriteAlpha(blueLights, blueLightsDuration, blueLightsCurve);
		AnimateSpriteAlpha(vendingMachineLights, vendingMachineLightsDuration, vendingMachineLightsCurve);
	}

	public void AnimateSpriteAlpha(tk2dSprite sprite, float duration, AnimationCurve curve)
	{
		Color endValue = new Color(1f, 1f, 1f, 0f);
		sprite.DOColor(endValue, duration).SetEase(curve).SetLoops(-1, LoopType.Restart);
	}

	public void ToggleLamps(bool on, bool lightFxToo = true)
	{
		if (lightFxToo)
		{
			tk2dSprite[] array = lights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(on);
			}
		}
		else
		{
			tk2dSprite[] array = lights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].transform.GetChild(0).gameObject.SetActive(on);
			}
		}
	}

	private IEnumerator WalkingCharactersCo()
	{
		yield return new WaitForSeconds(5f);
		while (true)
		{
			RDBGWalkingChar charToMove = walkingChars[0];
			bool foundACharToMove = false;
			while (!foundACharToMove)
			{
				yield return null;
				if (UnityEngine.Random.Range(0f, 100f) < 5f)
				{
					charToMove = walkingSamurai;
				}
				else
				{
					int num = UnityEngine.Random.Range(0, possibleWalkingChars.Count);
					if (possibleWalkingChars.IndexOf(walkingSamurai) == num || (possibleWalkingChars.Count > 2 && possibleWalkingChars[num] == lastCharSelected))
					{
						continue;
					}
					charToMove = possibleWalkingChars[num];
					lastCharSelected = charToMove;
				}
				foundACharToMove = !currentlyMovingChars.Contains(charToMove);
			}
			charToMove.gameObject.SetActive(value: true);
			charToMove.StartRunning(() =>
			{
				charToMove.gameObject.SetActive(value: false);
				currentlyMovingChars.Remove(charToMove);
			});
			float walkingDuration = charToMove.walkingDuration;
			yield return new WaitForSeconds(UnityEngine.Random.Range(walkingDuration, walkingDuration + 5f));
		}
	}

	public void SetNightMode(bool night)
	{
		SetNightMode(night ? 1 : 0);
		nightMode = night;
	}

	public void SetNightMode(float nightPercent)
	{
		Color white = Color.white;
		Color b = new Color(0.392f, 0.392f, 0.51f, 1f);
		Color color = Color.Lerp(white, b, nightPercent);
		Color color2 = Color.Lerp(baseLightColor, halloweenMode ? halloweenLightColor : baseLightColor, nightPercent);
		tk2dSprite[] array = tk2dSprites;
		foreach (tk2dSprite tk2dSprite2 in array)
		{
			if (tk2dSprite2 != null)
			{
				tk2dSprite2.color = color;
			}
		}
		WallClock[] array2 = clocks;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Tint(color);
		}
		array = lights;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = color2;
		}
	}

	private void DoThis(Color value)
	{
		for (int i = 0; i < foregroundList.Count; i++)
		{
			foregroundList[i].color = value;
		}
		_foregroundTintColor = value;
	}

	public void EnableBackgroundCharacters()
	{
		walkingBgContainer.SetActive(value: true);
		for (int i = 0; i < walkingChars.Length; i++)
		{
			RDBGWalkingChar rDBGWalkingChar = walkingChars[i];
			rDBGWalkingChar.GetComponent<MeshRenderer>().material.SetColor("_GlowColor", Color.black.WithAlpha(0f));
			if (rDBGWalkingChar.eligibleToWalk)
			{
				possibleWalkingChars.Add(rDBGWalkingChar);
			}
		}
		StartCoroutine(WalkingCharactersCo());
	}

	public void ShowVendingMachine(bool show)
	{
		tk2dSprite[] array = vendingMachineSprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(show);
		}
	}

	private void Update()
	{
		if (scrollSpeed != 0f || scrollOffsetX != 0f)
		{
			if (legacyScroll)
			{
				UpdateLegacyScroll();
			}
			else
			{
				UpdateScroll();
			}
		}
	}

	private void UpdateScroll()
	{
		if (!startedScrolling)
		{
			seamlessOffsetX = (float)scrollRoom * 3000f + 995f;
			scrollCurrentX = base.transform.localPosition.x - seamlessOffsetX;
			ToggleLamps(on: false);
			startedScrolling = true;
		}
		float num = Time.deltaTime * scrollSpeed;
		scrollCurrentX += num;
		float num2 = scrollCurrentX + scrollOffsetX;
		float num3 = num2;
		bool flag = num2 > 1347f && num2 < 1505f;
		if (!isSeamless & flag)
		{
			ToggleSeamless(on: true);
		}
		if (isSeamless)
		{
			num3 = Mathf.Repeat(num2 + 510f, 510f);
		}
		base.transform.LocalMoveX(num3 + seamlessOffsetX);
		objectsContainer.position = objectsContainer.position.WithX(seamlessOffsetX + num2 - 995f);
	}

	private void UpdateLegacyScroll()
	{
		if (!startedScrolling)
		{
			legacyScrollStartX = base.transform.localPosition.x;
			ToggleLamps(on: false);
			startedScrolling = true;
		}
		legacyScrollDistanceTraveled += Time.deltaTime * scrollSpeed;
		bool flag = legacyScrollDistanceTraveled >= 510f;
		if (!isSeamless & flag)
		{
			ToggleSeamless(on: true);
		}
		legacyScrollDistanceTraveled %= 510f;
		base.transform.LocalMoveX(legacyScrollStartX + legacyScrollDistanceTraveled);
	}

	public void ToggleSeamless(bool on)
	{
		if (!(scnGame.instance.currentLevel is Level_Lesmis))
		{
			leftEnd.SetActive(!on);
		}
		rightEnd.SetActive(!on);
		seamlessExtra.SetActive(on);
		ShowVendingMachine(!on);
		ToggleLamps(on: false);
		if (on)
		{
			svtBlockage.SetActive(value: false);
		}
		isSeamless = on;
	}

	public void StopScrolling()
	{
		scrollSpeed = 0f;
		startedScrolling = false;
	}

	public void SetOfficeDoorOpened(bool open, float speed = 1f)
	{
		officeDoor.Play("MainWard_OfficeDoor_" + (open ? "Open" : "Close"), speed, shouldreset: false);
	}

	public void SetBasementDoorOpened(bool open, float speed = 1f)
	{
		basementDoor.Play("MainWard_BasementDoor_" + (open ? "Open" : "Close"), speed, shouldreset: false);
	}

	public void SetElevatorOpened(bool open, float speed = 1f)
	{
		elevator.Play("MainWard_Elevator_" + (open ? "Open" : "Close"), speed, shouldreset: false);
	}

	public void ToggleTVs(bool enabled, bool hideEntirely = false)
	{
		tk2dSprite[] array = tvs;
		foreach (tk2dSprite obj in array)
		{
			obj.gameObject.GetComponent<MeshRenderer>().enabled = enabled;
			obj.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = !enabled && !hideEntirely;
		}
		RDAmbienceAudioSource[] array2 = tvAmbience;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].gameObject.SetActive(enabled);
		}
	}

	public void ToggleFireplace(bool enabled)
	{
		GameObject[] array = fireplaceObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(enabled);
		}
	}
}
