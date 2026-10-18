using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HeartMonitor : RDBase
{
	public enum LevelSpeed
	{
		Normal = 0,
		Ice = 1,
		Chili = 2
	}

	private enum SelectorPosition
	{
		GuestCredits = 0,
		Start = 1,
		Speed = 2
	}

	[Serializable]
	public struct SpeedSetting
	{
		public LevelSpeed name;

		public float discXPos;

		public GameObject phoneScreen;

		public Color screenColor;
	}

	[Serializable]
	public struct RankSprite
	{
		public string rank;

		public Texture texture;
	}

	private const string CharacterSpritePath = "Portraits/";

	[NonSerialized]
	public bool visible;

	public scnDemoLevels demoLevels;

	public scnLevelSelect levelSelect;

	public float StartXPos;

	public float MaxStartXPos;

	public float showY;

	public float showYOvershoot;

	public float hideY;

	public CLSAnimation[] animations;

	public GameObject guestPrefab;

	public RequiredControllerTablet requiredControllerTablet;

	public bool isRWLevel;

	[Header("Top Section")]
	public GameObject[] levelDetailTypes;

	public Text levelName;

	public RectTransform creditsContainer;

	public Text characterName;

	public Text descriptionPixel;

	public TMP_Text descriptionVector;

	public Image characterPortraitStatic;

	public GameObject papyrus;

	public GameObject star;

	public float screenAnimationSpeed;

	[Header("Rank")]
	public GameObject rankContainer;

	public RawImage rankRawImage;

	public RankSprite[] normalRankSprites;

	private Dictionary<Rank, Texture> normalRanks;

	public RankSprite[] bossRankSprites;

	private Dictionary<Rank, Texture> bossRanks;

	[Header("Selector")]
	public RectTransform selector;

	public RectTransform scrollableContainer;

	public float scrollContainerMaxHeight;

	public float scrollContainerMinHeight;

	public float scrollContainerAnimDuration;

	public Ease scrollContainerEase;

	public BpmAnimationManager bpmManager;

	public GameObject downArrow;

	public GameObject upArrow;

	[Header("Start")]
	public StartRow startRow;

	[Header("Speed")]
	public RectTransform speed;

	public RectTransform speedContainer;

	public Image speedBackground;

	public Sprite speedSelected;

	public Sprite speedIdle;

	public SpeedSetting[] speedSettings;

	public CLSAnimation speedSettingIce;

	public CLSAnimation speedSettingChilli;

	public RectTransform speedDisc;

	public Image screenBackground;

	[Header("GuestCredits")]
	public RectTransform guestCredits;

	public List<string> creditStrings = new List<string>();

	public List<string> links = new List<string>();

	[Header("Glitch")]
	public GameObject screenCrackImage;

	public SpriteAnimation[] glitches;

	public float minGlichTimer;

	public float maxGlitchTimer;

	private RectTransform rectTransform;

	private RectTransform startRowRect;

	private float selectorLeftPadding;

	private float selectorRightPadding;

	private float selectorTopOffset;

	private float selectorHeightAdd;

	private const float StartResizeDuration = 0.25f;

	private Dictionary<TMP_FontAsset, Material> descriptionVectorNoUnderlayMaterials = new Dictionary<TMP_FontAsset, Material>();

	private string levelToGo;

	private bool canGoToLevel;

	private bool canChangePlayerMode;

	private float characterCamXPos;

	[NonSerialized]
	public bool goingToLevel;

	private SelectorPosition currentSelected;

	private int currentLevelSpeedIndex;

	private bool isSpeedOptionShown;

	private bool isGuestCreditShown;

	private int currentCredit;

	private List<GameObject> creditsElements;

	private float creditsTimer;

	private bool playGlitch;

	private float currentGlitchTimer;

	private bool preferTwoPlayerMode;

	private bool _twoPlayerSelected;

	private int currentLinkID => (currentCredit + creditsElements.Count - 1) % creditsElements.Count;

	private bool currentLinkIsValid
	{
		get
		{
			if (currentLinkID < links.Count)
			{
				return links[currentLinkID] != null;
			}
			return false;
		}
	}

	private string playerModeNarration
	{
		get
		{
			if (!TwoPlayerSelected)
			{
				return RDString.Get("narration.onePlayerMode");
			}
			return RDString.Get("narration.twoPlayerMode");
		}
	}

	private LevelSpeed CurrentLevelSpeedName => speedSettings[currentLevelSpeedIndex].name;

	private bool TwoPlayerSelected
	{
		get
		{
			return _twoPlayerSelected;
		}
		set
		{
			GC.twoPlayerMode = value;
			if (TwoPlayerSelected != value)
			{
				_twoPlayerSelected = value;
				UpdateStartLayout(animate: true);
			}
		}
	}

	private void UpdateStartLayout(bool animate)
	{
		bool flag = currentSelected == SelectorPosition.Start;
		StartRow.Frame frame = startRow.SetState(!TwoPlayerSelected, canChangePlayerMode, flag, animate);
		if (flag)
		{
			float num = selector.anchorMax.x - selector.anchorMin.x;
			float width = ((RectTransform)selector.parent).rect.width;
			Vector2 vector = new Vector2(startRowRect.anchoredPosition.x + frame.pos.x, frame.pos.y);
			Vector2 vector2 = new Vector2(frame.size.x - num * width, frame.size.y);
			selector.DOKill();
			if (animate)
			{
				selector.DOAnchorPos(vector, 0.25f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
				selector.DOSizeDelta(vector2, 0.25f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
			}
			else
			{
				selector.anchoredPosition = vector;
				selector.sizeDelta = vector2;
			}
		}
	}

	private void PositionSelector(SelectorPosition option, bool animate)
	{
		selector.SetParent((option == SelectorPosition.GuestCredits) ? guestCredits : scrollableContainer, worldPositionStays: true);
		if (option != SelectorPosition.Start)
		{
			selector.DOKill();
			RectTransform rectTransform = ((option == SelectorPosition.Speed) ? speed : guestCredits);
			float num = rectTransform.rect.width + selectorLeftPadding + selectorRightPadding;
			float num2 = selector.anchorMax.x - selector.anchorMin.x;
			float width = ((RectTransform)selector.parent).rect.width;
			Vector2 vector = new Vector2(num - num2 * width, rectTransform.sizeDelta.y + selectorHeightAdd);
			Vector2 anchoredPosition = selector.anchoredPosition;
			selector.position = new Vector3(rectTransform.position.x - selectorLeftPadding, rectTransform.position.y + selectorTopOffset, selector.position.z);
			if (animate)
			{
				Vector2 anchoredPosition2 = selector.anchoredPosition;
				selector.anchoredPosition = anchoredPosition;
				selector.DOAnchorPos(anchoredPosition2, 0.25f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
				selector.DOSizeDelta(vector, 0.25f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
			}
			else
			{
				selector.sizeDelta = vector;
			}
		}
	}

	private Material GetNoUnderlayMaterial(TMP_FontAsset fontAsset)
	{
		if (!descriptionVectorNoUnderlayMaterials.TryGetValue(fontAsset, out var value))
		{
			value = UnityEngine.Object.Instantiate(fontAsset.material);
			value.SetKeyword(new LocalKeyword(value.shader, "UNDERLAY_ON"), value: false);
			descriptionVectorNoUnderlayMaterials[fontAsset] = value;
		}
		return value;
	}

	private void FlashStartButtonPress()
	{
		StartRow.Button active = startRow.ActiveButton(!TwoPlayerSelected);
		active.background.sprite = active.bgDefault;
		DOVirtual.DelayedCall(0.1f, () =>
		{
			active.background.sprite = active.bgSelected;
		});
	}

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		AssignRanks(normalRankSprites, ref normalRanks);
		AssignRanks(bossRankSprites, ref bossRanks);
		startRowRect = (RectTransform)startRow.transform;
		Vector2 anchoredPosition = selector.anchoredPosition;
		float num = selector.sizeDelta.x + (selector.anchorMax.x - selector.anchorMin.x) * ((RectTransform)selector.parent).rect.width;
		selectorLeftPadding = startRowRect.anchoredPosition.x - anchoredPosition.x;
		selectorRightPadding = anchoredPosition.x + num - (startRowRect.anchoredPosition.x + startRowRect.rect.width);
		selectorHeightAdd = selector.sizeDelta.y - startRowRect.sizeDelta.y;
		selectorTopOffset = selectorHeightAdd / 2f;
		startRow.Init(new Vector2(anchoredPosition.x - startRowRect.anchoredPosition.x, anchoredPosition.y), new Vector2(num, selector.sizeDelta.y));
		currentSelected = SelectorPosition.Start;
		currentLevelSpeedIndex = 1;
		preferTwoPlayerMode = GC.twoPlayerMode;
		static void AssignRanks(RankSprite[] spriteRanks, ref Dictionary<Rank, Texture> ranks)
		{
			ranks = new Dictionary<Rank, Texture>();
			for (int i = 0; i < spriteRanks.Length; i++)
			{
				RankSprite rankSprite = spriteRanks[i];
				ranks.Add(Rank.FromString(rankSprite.rank), rankSprite.texture);
			}
		}
	}

	private void Update()
	{
		if (playGlitch && Time.time > currentGlitchTimer)
		{
			SpriteAnimation[] array = glitches;
			foreach (SpriteAnimation obj in array)
			{
				obj.Stop();
				obj.gameObject.SetActive(value: true);
				obj.Play();
			}
			currentGlitchTimer = Time.time + UnityEngine.Random.Range(minGlichTimer, maxGlitchTimer);
		}
		creditsTimer -= Time.deltaTime;
		if (creditsTimer <= 0f && currentSelected != SelectorPosition.GuestCredits)
		{
			ScrollCredits(forwards: true);
		}
		if (goingToLevel || !visible)
		{
			return;
		}
		if (currentSelected == SelectorPosition.Start && RDInput.anyPlayerPress && canGoToLevel && !goingToLevel)
		{
			goingToLevel = true;
			FlashStartButtonPress();
			scnGame.levelSpeed = CurrentLevelSpeedName switch
			{
				LevelSpeed.Chili => 1.5f, 
				LevelSpeed.Ice => 0.75f, 
				_ => 1f, 
			};
			if (demoLevels != null)
			{
				demoLevels.GoToLevelTrigger(levelToGo);
			}
			if (levelSelect != null)
			{
				levelSelect.GoToLevelSequence(levelToGo);
			}
		}
		if (currentSelected == SelectorPosition.Speed && RDInput.anyPlayerPress)
		{
			ChangeLevelSpeed(1, wrap: true);
		}
		if (RDInput.leftPress || RDInput.rightPress)
		{
			if (currentSelected == SelectorPosition.Speed)
			{
				int direction = ((!RDInput.leftPress) ? 1 : (-1));
				ChangeLevelSpeed(direction);
			}
			else if (currentSelected == SelectorPosition.Start && canChangePlayerMode)
			{
				TwoPlayerSelected = !TwoPlayerSelected;
				preferTwoPlayerMode = TwoPlayerSelected;
				if (GC.twoPlayerMode)
				{
					scnLevelSelect.LevelSelectPlaySound("sndTwoPlayersActivateR", 1f, 1f, GC.PanP1);
					scnLevelSelect.LevelSelectPlaySound("sndTwoPlayersActivateL", 1f, 1f, GC.PanP2);
				}
				else
				{
					scnLevelSelect.LevelSelectPlaySound("sndTwoPlayersDeactivate");
				}
				scnLevelSelect.LevelSelectPlaySound("sndPagerValue");
				if (isSpeedOptionShown && isGuestCreditShown)
				{
					Narration.Say(playerModeNarration, NarrationCategory.Navigation, false, NarrationActionName.ToStart, NarrationActionName.ToChangePlayerMode, NarrationActionName.ForLevelSpeedSlider, NarrationActionName.ForGuestCredits, NarrationActionName.ToClose);
				}
				else if (isSpeedOptionShown)
				{
					Narration.Say(playerModeNarration, NarrationCategory.Navigation, false, NarrationActionName.ToStart, NarrationActionName.ToChangePlayerMode, NarrationActionName.ForLevelSpeedSlider, NarrationActionName.ToClose);
				}
				else if (isGuestCreditShown)
				{
					Narration.Say(playerModeNarration, NarrationCategory.Navigation, false, NarrationActionName.ToStart, NarrationActionName.ToChangePlayerMode, NarrationActionName.ForGuestCredits, NarrationActionName.ToClose);
				}
				else
				{
					Narration.Say(playerModeNarration, NarrationCategory.Navigation, false, NarrationActionName.ToStart, NarrationActionName.ToChangePlayerMode, NarrationActionName.ToClose);
				}
			}
			else if (currentSelected == SelectorPosition.GuestCredits)
			{
				bool forwards = !RDInput.leftPress;
				ScrollCredits(forwards, manual: true);
			}
		}
		if (currentSelected == SelectorPosition.GuestCredits && RDInput.anyPlayerPress && currentLinkIsValid)
		{
			Application.OpenURL(links[currentLinkID]);
		}
		int num = (int)currentSelected;
		if (RDInput.upPress)
		{
			num--;
		}
		else if (RDInput.downPress)
		{
			num++;
		}
		int num2 = ((!isGuestCreditShown) ? 1 : 0);
		int num3 = ((!isSpeedOptionShown) ? 1 : 2);
		if (num < num2 || num > num3)
		{
			scnLevelSelect.LevelSelectPlaySound("sndPagerCursorBorder");
			num = Mathf.Clamp(num, num2, num3);
		}
		else
		{
			ChangeSelectedOption((SelectorPosition)num);
		}
	}

	private void LateUpdate()
	{
		if (speedBackground.sprite == speedSelected)
		{
			speedContainer.anchoredPosition = new Vector2(0f, 0.5f);
		}
		else
		{
			speedContainer.anchoredPosition = new Vector2(1f, -0.5f);
		}
	}

	private void ChangeLevelSpeed(int direction, bool wrap = false)
	{
		int num = currentLevelSpeedIndex + direction;
		if (num >= speedSettings.Length || num < 0)
		{
			if (!wrap)
			{
				scnLevelSelect.LevelSelectPlaySound("sndPagerCursorBorder");
				return;
			}
			num = (num + speedSettings.Length) % speedSettings.Length;
		}
		speedSettings[currentLevelSpeedIndex].phoneScreen.SetActive(value: false);
		if (CurrentLevelSpeedName == LevelSpeed.Ice)
		{
			speedSettingIce.Stop();
		}
		else if (CurrentLevelSpeedName == LevelSpeed.Chili)
		{
			speedSettingChilli.Stop();
		}
		currentLevelSpeedIndex = num;
		speedDisc.DOKill();
		SpeedSetting speedSetting = speedSettings[currentLevelSpeedIndex];
		speedDisc.DOAnchorPosX(speedSetting.discXPos, 0.3f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		speedSetting.phoneScreen.SetActive(value: true);
		screenBackground.color = speedSetting.screenColor;
		string text = "";
		if (CurrentLevelSpeedName == LevelSpeed.Ice)
		{
			speedSettingIce.Play();
			text = "sndLevelDetailsSpeedIce";
		}
		else if (CurrentLevelSpeedName == LevelSpeed.Chili)
		{
			speedSettingChilli.Play();
			text = "sndLevelDetailsSpeedChili";
		}
		if (!text.IsNullOrEmpty())
		{
			scnLevelSelect.LevelSelectPlaySound(text);
		}
		scnLevelSelect.LevelSelectPlaySound("sndPagerValue");
		Narration.Say(RDString.Get($"narration.{CurrentLevelSpeedName}"), NarrationCategory.Navigation, false, NarrationActionName.ToChangeValue, NarrationActionName.ForStartButtonUp, NarrationActionName.ToClose);
	}

	public void Show(SelectableCharacter character, LevelSpeed type, Difficulty difficulty, LevelPlayMode levelPlayMode = LevelPlayMode.BothModes)
	{
		LevelMetadata levelMetadata = GC.levelMetadata[character.levels[difficulty]];
		ChangeSelectedOption(SelectorPosition.Start, playSound: false);
		PositionSelector(SelectorPosition.Start, animate: false);
		bool leftPager = character.leftPager;
		float num = (leftPager ? (StartXPos * -1f) : StartXPos);
		if (!character.skipCamAnimOnSelect)
		{
			if (levelSelect.camMoveXTween != null)
			{
				characterCamXPos = ((TweenerCore<Vector3, Vector3, VectorOptions>)levelSelect.camMoveXTween).endValue.x;
			}
			else
			{
				characterCamXPos = levelSelect.cameraObject.transform.localPosition.x;
			}
			num = (leftPager ? (0f - (160f + StartXPos)) : StartXPos);
		}
		else
		{
			characterCamXPos = float.MinValue;
			num = (leftPager ? (0f - (160f + MaxStartXPos)) : MaxStartXPos);
		}
		rectTransform.anchoredPosition = rectTransform.anchoredPosition.WithX(num);
		canChangePlayerMode = levelPlayMode == LevelPlayMode.BothModes;
		TwoPlayerSelected = (canChangePlayerMode ? preferTwoPlayerMode : (levelPlayMode == LevelPlayMode.TwoPlayerOnly));
		UpdateStartLayout(animate: false);
		Character[] array = ((difficulty == Difficulty.Hard) ? character.portraitsHard : character.portraitsNormal);
		string text = "";
		for (int i = 0; i < array.Length; i++)
		{
			if (i > 0)
			{
				text += ", ";
			}
			text += RDString.Get("enum.Character." + array[i]);
		}
		characterName.text = text;
		Level level = character.levels[difficulty];
		levelToGo = level.ToString();
		string text2 = $"levelSelect.{level}";
		levelName.text = RDString.Get(text2).Replace('\n', ' ');
		if (RDString.isCJK || Persistence.GetReducePixelFontUsage())
		{
			levelName.fontSize = 8;
			levelName.resizeTextForBestFit = true;
			levelName.resizeTextMinSize = 4;
			levelName.resizeTextMaxSize = 8;
			characterName.fontSize = 8;
			characterName.resizeTextForBestFit = true;
			characterName.resizeTextMinSize = 4;
			characterName.resizeTextMaxSize = 8;
		}
		string text3 = array[0].ToString();
		if (level == Level.AthleteTherapy)
		{
			text3 = "Miner_trainer";
		}
		characterPortraitStatic.sprite = Resources.Load<Sprite>("Portraits/" + text3);
		Level level2 = character.levels[difficulty];
		Rank levelRank = Persistence.GetLevelRank(level2);
		Dictionary<Rank, Texture> dictionary = (character.isBossLevel ? bossRanks : normalRanks);
		bool flag = dictionary.ContainsKey(levelRank);
		rankContainer.SetActive(flag);
		if (flag)
		{
			rankRawImage.texture = dictionary[levelRank];
			bool active = (int)levelRank.ToNormal() >= 4;
			star.SetActive(active);
			bool active2 = (int)levelRank.ToNormal() == 5;
			papyrus.SetActive(active2);
		}
		else
		{
			star.SetActive(value: false);
			papyrus.SetActive(value: false);
		}
		creditsContainer.LocalMoveY(0f);
		currentCredit = 0;
		if (creditsElements != null)
		{
			foreach (GameObject creditsElement in creditsElements)
			{
				UnityEngine.Object.Destroy(creditsElement);
			}
		}
		creditsElements = new List<GameObject>();
		creditStrings.Clear();
		links.Clear();
		isGuestCreditShown = InstantiateGuest(levelMetadata.guest1);
		isGuestCreditShown = InstantiateGuest(levelMetadata.guest2) || isGuestCreditShown;
		isGuestCreditShown = InstantiateGuest(levelMetadata.guest3) || isGuestCreditShown;
		isGuestCreditShown = InstantiateGuest(levelMetadata.guest4) || isGuestCreditShown;
		upArrow.gameObject.SetActive(currentSelected == SelectorPosition.Speed);
		if (creditsElements.Count > 0)
		{
			GameObject item = UnityEngine.Object.Instantiate(levelName.gameObject, creditsContainer);
			creditStrings.Add(RDString.Get(text2).Replace('\n', ' '));
			creditsElements.Add(item);
			creditsTimer = screenAnimationSpeed * 2f;
		}
		speedSettings[currentLevelSpeedIndex].phoneScreen.SetActive(value: false);
		if (CurrentLevelSpeedName == LevelSpeed.Ice)
		{
			speedSettingIce.Stop();
		}
		else if (CurrentLevelSpeedName == LevelSpeed.Chili)
		{
			speedSettingChilli.Stop();
		}
		switch (type)
		{
		case LevelSpeed.Chili:
			currentLevelSpeedIndex = 0;
			break;
		case LevelSpeed.Normal:
			currentLevelSpeedIndex = 1;
			break;
		default:
			currentLevelSpeedIndex = 2;
			break;
		}
		SpeedSetting speedSetting = speedSettings[currentLevelSpeedIndex];
		screenBackground.color = speedSetting.screenColor;
		speedDisc.DOKill();
		speedDisc.DOAnchorPosX(speedSettings[currentLevelSpeedIndex].discXPos, 0.3f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		speedSetting.phoneScreen.SetActive(value: true);
		if (CurrentLevelSpeedName == LevelSpeed.Ice)
		{
			speedSettingIce.Play();
		}
		else if (CurrentLevelSpeedName == LevelSpeed.Chili)
		{
			speedSettingChilli.Play();
		}
		string text4 = ((Persistence.GetStateFromLevel(level2) == LevelState.Passed) ? "passed" : "notPassed");
		string key = text2 + "." + text4;
		descriptionPixel.gameObject.SetActive(value: false);
		descriptionVector.gameObject.SetActive(value: false);
		if (!RDString.isCJK && !Persistence.GetReducePixelFontUsage())
		{
			descriptionPixel.gameObject.SetActive(value: true);
			descriptionPixel.text = RDString.Get(key);
		}
		else
		{
			descriptionVector.gameObject.SetActive(value: true);
			TMP_FontAsset vectorFontAsset = RDString.currentFontPack.vectorFontAsset;
			descriptionVector.font = vectorFontAsset;
			descriptionVector.fontSharedMaterial = GetNoUnderlayMaterial(vectorFontAsset);
			descriptionVector.text = RDString.Get(key);
			descriptionVector.fontSize = ((RDString.isCJK && RDString.language != SystemLanguage.Japanese) ? 8 : 7);
			descriptionVector.lineSpacing = ((!RDString.isCJK) ? (-15) : 0);
		}
		if (level2 == Level.BeansHopper || level2 == Level.RhythmWeightlifter)
		{
			int levelScore = Persistence.GetLevelScore(level2.ToString());
			descriptionPixel.text = descriptionPixel.text.Replace("[bestScore]", levelScore.ToString());
			descriptionVector.text = descriptionVector.text.Replace("[bestScore]", levelScore.ToString());
		}
		int num2 = ((difficulty == Difficulty.Normal) ? character.bpmNormal : character.bpmHard);
		bpmManager.SetBpm(num2);
		if (levelSelect != null)
		{
			levelSelect.controlsAreLocked = true;
		}
		bool show = (int)levelRank != -1 && (int)levelRank != -2 && level2 != Level.AthleteTherapy && level2 != Level.RhythmWeightlifter && level2 != Level.Montage && level2 != Level.Montage2;
		ToggleSpeed(show);
		bool active3 = GC.levelMetadata[level2].type == LevelType.Boss;
		SpriteAnimation[] array2 = glitches;
		foreach (SpriteAnimation obj in array2)
		{
			obj.Stop();
			obj.gameObject.SetActive(value: false);
		}
		playGlitch = active3;
		currentGlitchTimer = Time.time + 99999f;
		screenCrackImage.gameObject.SetActive(active3);
		CLSAnimation[] array3 = animations;
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j].Play();
		}
		visible = true;
		rectTransform.DOKill();
		showY = 0f - GetComponent<Canvas>().worldCamera.orthographicSize + 205f + 5f;
		rectTransform.DOLocalMoveY(showY, screenAnimationSpeed).SetEase(Ease.OutBack, showYOvershoot).SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				if (playGlitch)
				{
					SpriteAnimation[] array4 = glitches;
					foreach (SpriteAnimation obj2 in array4)
					{
						obj2.gameObject.SetActive(value: true);
						obj2.Play();
					}
					currentGlitchTimer = Time.time + UnityEngine.Random.Range(minGlichTimer, maxGlitchTimer);
				}
				goingToLevel = false;
				canGoToLevel = true;
			});
		int num3 = 1;
		if (isSpeedOptionShown)
		{
			num3++;
		}
		if (isGuestCreditShown)
		{
			num3++;
		}
		Narration.Set(RDString.Get("narration.patientDetailsScreen"), ElementType.VerticalMenu, num3);
		string newValue = RDString.Get("enum.Character." + array[0]);
		string arg = (Persistence.GetLevelRank(level2).passed ? "passed" : "notPassed");
		string text5 = RDString.Get($"levelSelect.{level2}.patient.{arg}");
		string newValue2 = (descriptionPixel.gameObject.activeSelf ? descriptionPixel.text : descriptionVector.text);
		string speech = RDString.Get("narration.characterSelectedTitle").Replace("[playerMode]", playerModeNarration);
		string text6 = RDString.Get("narration.characterSelectedDescription");
		text6 = text6.Replace("[characterName]", characterName.text);
		text6 = text6.Replace("[description]", newValue2);
		string text7 = "";
		if (levelSelect.patientQuote.enabled && !text5.Trim().IsNullOrEmpty())
		{
			text7 = RDString.Get("narration.characterSelectedQuote");
			text7 = text7.Replace("[firstCharacterName]", newValue);
			text7 = text7.Replace("[patientQuote]", text5);
		}
		if (isSpeedOptionShown && isGuestCreditShown)
		{
			Narration.Say(speech, NarrationCategory.Navigation, -1, -1, ElementType.None, null, text6 + text7, false, NarrationActionName.ToStart, NarrationActionName.ToChangePlayerMode, NarrationActionName.ForLevelSpeedSlider, NarrationActionName.ForGuestCredits, NarrationActionName.ToClose);
		}
		else if (isSpeedOptionShown)
		{
			Narration.Say(speech, NarrationCategory.Navigation, -1, -1, ElementType.None, null, text6 + text7, false, NarrationActionName.ToStart, NarrationActionName.ToChangePlayerMode, NarrationActionName.ForLevelSpeedSlider, NarrationActionName.ToClose);
		}
		else if (isGuestCreditShown)
		{
			Narration.Say(speech, NarrationCategory.Navigation, -1, -1, ElementType.None, null, text6 + text7, false, NarrationActionName.ToStart, NarrationActionName.ToChangePlayerMode, NarrationActionName.ForGuestCredits, NarrationActionName.ToClose);
		}
		else
		{
			Narration.Say(speech, NarrationCategory.Navigation, -1, -1, ElementType.None, null, text6 + text7, false, NarrationActionName.ToStart, NarrationActionName.ToChangePlayerMode, NarrationActionName.ToClose);
		}
		bool InstantiateGuest(GuestData gd)
		{
			if (gd == null)
			{
				return false;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(guestPrefab, creditsContainer);
			gameObject.GetComponent<HeartMonitorGuest>().Setup(gd);
			creditStrings.Add(RDString.Get("enum.GuestType." + gd.type) + " " + gd.name);
			links.Add(gd.link);
			creditsElements.Add(gameObject);
			return true;
		}
	}

	private void ScrollCredits(bool forwards, bool manual = false)
	{
		if (creditsElements == null || creditsElements.Count == 0)
		{
			return;
		}
		if (forwards)
		{
			if (currentCredit >= creditsElements.Count)
			{
				creditsContainer.LocalMoveY(0f);
				currentCredit = 0;
			}
			currentCredit++;
		}
		else
		{
			if (currentCredit <= 0)
			{
				creditsContainer.LocalMoveY(creditsElements.Count * 18);
				currentCredit = creditsElements.Count;
			}
			currentCredit--;
		}
		float duration = (manual ? 0.1f : 0.5f);
		creditsContainer.DOLocalMoveY(currentCredit * 18, duration).SetEase(Ease.Linear);
		if (manual)
		{
			NarrateGuestCredit();
		}
		creditsTimer = 3f;
	}

	private void ChangeSelectedOption(SelectorPosition option, bool playSound = true)
	{
		if (currentSelected == option)
		{
			return;
		}
		PositionSelector(option, animate: true);
		currentSelected = option;
		bool flag = currentSelected == SelectorPosition.Speed;
		downArrow.SetActive((currentSelected == SelectorPosition.Start && isSpeedOptionShown) | flag);
		downArrow.transform.localEulerAngles = new Vector3(0f, 0f, flag ? 90f : (-90f));
		UpdateStartLayout(animate: true);
		if (currentSelected == SelectorPosition.Start)
		{
			speedBackground.sprite = speedIdle;
			string speech = RDString.Get("narration.characterSelectedTitle").Replace("[playerMode]", playerModeNarration);
			if (isSpeedOptionShown && isGuestCreditShown)
			{
				Narration.Say(speech, NarrationCategory.Navigation, false, NarrationActionName.ToChangePlayerMode, NarrationActionName.ForLevelSpeedSlider, NarrationActionName.ForGuestCredits, NarrationActionName.ToClose);
			}
			else if (isSpeedOptionShown)
			{
				Narration.Say(speech, NarrationCategory.Navigation, false, NarrationActionName.ToChangePlayerMode, NarrationActionName.ForLevelSpeedSlider, NarrationActionName.ToClose);
			}
			else if (isGuestCreditShown)
			{
				Narration.Say(speech, NarrationCategory.Navigation, false, NarrationActionName.ToChangePlayerMode, NarrationActionName.ForGuestCredits, NarrationActionName.ToClose);
			}
		}
		else if (currentSelected == SelectorPosition.Speed)
		{
			speedBackground.sprite = speedSelected;
			Narration.Say(RDString.Get("narration.levelSpeedSlider").Replace("[speed]", RDString.Get($"narration.{CurrentLevelSpeedName}")), NarrationCategory.Navigation, false, NarrationActionName.ToChangeValue, NarrationActionName.ForStartButtonUp, NarrationActionName.ToClose);
		}
		else if (currentSelected == SelectorPosition.GuestCredits)
		{
			Narration.Set(RDString.Get("narration.levelGuestCredits"), ElementType.HorizontalList, creditsElements.Count);
			NarrateGuestCredit();
		}
		if (playSound)
		{
			scnLevelSelect.LevelSelectPlaySound("sndPagerCursorMove");
		}
		float endValue = ((currentSelected == SelectorPosition.Speed) ? scrollContainerMaxHeight : scrollContainerMinHeight);
		scrollableContainer.DOKill();
		scrollableContainer.DOAnchorPosY(endValue, scrollContainerAnimDuration).SetEase(scrollContainerEase);
	}

	public void Hide(Action onComplete = null)
	{
		if (!visible)
		{
			return;
		}
		if (characterCamXPos != float.MinValue)
		{
			levelSelect.cameraObject.transform.DOKill();
			levelSelect.camMoveXTween = levelSelect.cameraObject.transform.DOLocalMoveX(characterCamXPos, 0.5f);
		}
		visible = false;
		canChangePlayerMode = false;
		canGoToLevel = false;
		rectTransform.DOKill();
		hideY = 0f - GetComponent<Canvas>().worldCamera.orthographicSize;
		HideRequiredController();
		rectTransform.DOLocalMoveY(hideY, screenAnimationSpeed * 0.2f).SetEase(Ease.InQuad).SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				SpriteAnimation[] array = glitches;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Stop();
				}
				CLSAnimation[] array2 = animations;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].Stop();
				}
				if (onComplete != null)
				{
					onComplete();
				}
			});
	}

	public void ShowRequiredController()
	{
		requiredControllerTablet?.Show(screenAnimationSpeed, 0f, resetPos: true);
	}

	public void HideRequiredController()
	{
		requiredControllerTablet?.Hide(screenAnimationSpeed * 0.2f);
	}

	public void HighlightRequiredController()
	{
		requiredControllerTablet?.HighlightRequiredController();
	}

	public void ToggleSpeed(bool show)
	{
		isSpeedOptionShown = show;
		downArrow.SetActive(show);
		speed.gameObject.SetActive(show);
	}

	private void NarrateGuestCredit()
	{
		int index = (currentCredit + creditsElements.Count - 1) % creditsElements.Count;
		if (creditStrings[index] != null)
		{
			List<NarrationActionName> list = new List<NarrationActionName>
			{
				NarrationActionName.ToChangeValue,
				NarrationActionName.ForStartButtonDown,
				NarrationActionName.ToClose
			};
			string speech = string.Empty;
			if (currentLinkIsValid)
			{
				Narration.Set(creditStrings[index], -1, -1, ElementType.Link);
				list.Insert(0, NarrationActionName.ToConfirm);
			}
			else
			{
				speech = creditStrings[index];
			}
			Narration.Say(speech, NarrationCategory.Navigation, flipCategoryQueueBehaviour: false, list.ToArray());
		}
	}

	public void GoToLevel()
	{
		levelSelect.GoToLevelSequence(levelToGo);
	}
}
