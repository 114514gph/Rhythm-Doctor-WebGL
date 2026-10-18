using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public partial class CustomLevelSyringe : RDBase
{
	private static Vector2[] CirclePositions;

	[Header("Info to change")]
	public GameObject syringeIconContainer;

	public Text songText;

	public Text artistText;

	public RectTransform songRect;

	public RectTransform artistRect;

	public RawImage syringeIcon;

	public RectTransform syringeIconRect;

	public Image syringeBodyImage;

	public Sprite syringeBodyDefault;

	public Sprite syringeBodyBox;

	[NonSerialized]
	public string path;

	[NonSerialized]
	public float offset;

	[Header("Movement")]
	public AnimationCurve movementCurve;

	public IEnumerator moveCoroutine;

	public int currentPositionIndex;

	public int currentDataIndex;

	private const float CircleRadius = 254f;

	[Header("Overlay Fade")]
	public Image overlayImage;

	public Image overlayBoxImage;

	[Header("Hue")]
	public Image songBackgroundImage;

	[Header("BPM Based")]
	public RectTransform plungerRect;

	public BpmMovementAnimation plungerAnimation;

	public BpmSpriteAnimation liquidAnimation;

	[NonSerialized]
	public float bpm;

	[Header("USB Toggle")]
	public RectTransform syringeUsb;

	[Header("Level Selection Liquid Pressing")]
	public RectTransform liquidRect;

	[Header("Level Box")]
	public SpriteAnimation bodyAnimation;

	public SpriteAnimation boxBrokenAnimation;

	public RectTransform box;

	public RectTransform boxBroken;

	public Image boxDetailsImage;

	public Image boxSongBackground;

	public Text boxSongText;

	public Text boxArtistText;

	public RectTransform boxSongRect;

	public RectTransform boxArtistRect;

	[NonSerialized]
	public Rank CurrentRank;

	private const float PlungerReleaseXPos = -140f;

	private const float PlungerPressXPos = 7f;

	private const float LiquidStartXPos = 6.5f;

	private const float LiquidWidth = 156f;

	private const Ease PlungerPressEase = Ease.OutQuint;

	public const float MaxOverlayAlpha = 0.4f;

	public const float MinOverlayAlpha = 0f;

	public const float TimeToFadeOverlay = 0.4f;

	public RectTransform rect { get; set; }

	private void Awake()
	{
		rect = GetComponent<RectTransform>();
		moveCoroutine = MoveInternal(-1);
		movementCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
		movementCurve.preWrapMode = WrapMode.Once;
		movementCurve.postWrapMode = WrapMode.Once;
	}

	public void SetTextAnchoredPosition(Text text, RectTransform textRect)
	{
		float preferredWidth = text.preferredWidth;
		float num = ((preferredWidth <= 109f) ? preferredWidth : 109f);
		textRect.DOKill();
		textRect.anchoredPosition = textRect.anchoredPosition.WithX(num * -1f);
	}

	public void UpdateInfo(CustomLevelData data)
	{
		path = data.path;
		CurrentRank = Persistence.GetCustomLevelRank(data.Hash);
		if (data.isLegacyLevel)
		{
			box.gameObject.SetActive(value: false);
			boxBroken.gameObject.SetActive(value: false);
			syringeBodyImage.sprite = syringeBodyDefault;
		}
		else
		{
			if ((int)CurrentRank == -2)
			{
				CurrentRank = -3;
				Persistence.SetCustomLevelRank(data.Hash, CurrentRank);
			}
			bool flag = (int)CurrentRank == -3;
			box.gameObject.SetActive(flag);
			boxBroken.gameObject.SetActive(flag);
			Sprite sprite = (flag ? syringeBodyBox : syringeBodyDefault);
			syringeBodyImage.sprite = sprite;
			if (flag)
			{
				boxSongText.text = data.settings.song;
				boxSongText.font = RDString.GetAppropiateFontForString(boxSongText.text);
				SetTextAnchoredPosition(boxSongText, boxSongRect);
				boxArtistText.text = data.settings.artist;
				boxArtistText.font = RDString.GetAppropiateFontForString(boxArtistText.text);
				SetTextAnchoredPosition(boxArtistText, boxArtistRect);
			}
		}
		artistText.text = data.settings.artist.RemoveNewLines();
		artistText.font = RDString.GetAppropiateFontForString(artistText.text);
		SetTextAnchoredPosition(artistText, artistRect);
		songText.text = data.settings.song.RemoveNewLines();
		songText.font = RDString.GetAppropiateFontForString(songText.text);
		SetTextAnchoredPosition(songText, songRect);
		bpm = data.settings.bpm;
		offset = data.settings.firstSongoffset;
		LoadSyringeIcon(data);
		if (!base.cls.scrollingFaster && !base.cls.Searching)
		{
			FadeOverlay(currentDataIndex == base.cls.CurrentLevelIndex, animate: false);
		}
		if (data.isLegacyLevel)
		{
			songBackgroundImage.color = GetLabelColor();
			return;
		}
		float songLabelHue = data.settings.songLabelHue;
		bool songLabelGrayscale = data.settings.songLabelGrayscale;
		Color labelColor = GetLabelColor(songLabelHue, 95f, songLabelGrayscale);
		songBackgroundImage.color = labelColor;
		boxDetailsImage.color = labelColor;
		boxSongBackground.color = labelColor;
		songText.color = ((songLabelGrayscale && (double)songLabelHue <= 0.5) ? Color.white : Color.black);
		boxArtistText.color = GetLabelColor(songLabelHue, 75f, songLabelGrayscale);
	}

	private void LoadSyringeIcon(CustomLevelData levelData)
	{
		if (levelData.syringeIcon != null)
		{
			ShowSyringeIcon(levelData.syringeIcon);
			return;
		}
		syringeIconContainer.SetActive(value: false);
		if (LevelValidation.CheckLevelForErrors(new List<LevelErrorName>
		{
			LevelErrorName.SyringeIconNull,
			LevelErrorName.SyringeIconFileNotFound,
			LevelErrorName.SyringeIconFileSize,
			LevelErrorName.SyringeIconWrongExtension
		}, new List<LevelErrorName> { LevelValidation.GetSignatureImageError(levelData.path, levelData.settings) }))
		{
			DesktopLevelLoader.LoadImageAsyncFromPath(DesktopLevelLoader.GetValidImageInPath(levelData.path, levelData.settings.syringeIconName), levelData, (Texture syringeIconTexture) =>
			{
				int index = base.cls.levelsData.IndexOf(levelData);
				base.cls.levelsData[index].syringeIcon = syringeIconTexture;
				base.cls.levelsData[index].syringeIcon.filterMode = FilterMode.Point;
				ShowSyringeIcon(syringeIconTexture);
			});
		}
		void ShowSyringeIcon(Texture syringeIconTexture)
		{
			syringeIcon.texture = syringeIconTexture;
			syringeIconRect.sizeDelta = LevelValidation.GetImageFixedSize(syringeIconTexture, LevelValidation.SyringeIconRecommendedDimensions);
			syringeIconContainer.SetActive(value: true);
		}
	}

	private static Color GetLabelColor(float hue = -1f, float brightness = 95f, bool isGrayscale = false)
	{
		float num = 60f;
		if (hue < 0f)
		{
			return Color.HSVToRGB(0f, 0f, 75f);
		}
		if (isGrayscale)
		{
			return Color.HSVToRGB(0f, 0f, hue);
		}
		return Color.HSVToRGB(hue, num / 100f, brightness / 100f);
	}

	public void Move(int direction, float timeToMove = -1f)
	{
		StopMove();
		moveCoroutine = MoveInternal(direction, timeToMove);
		StartCoroutine(moveCoroutine);
	}

	public void StopMove()
	{
		rect.DOKill();
		StopCoroutine(moveCoroutine);
	}

	private IEnumerator MoveInternal(int direction, float timeToMove = -1f)
	{
		Vector2 position = GetPosition();
		if (timeToMove <= 0f)
		{
			rect.anchoredPosition = position;
			yield break;
		}
		float beginYPos = rect.anchoredPosition.y;
		float num = (((beginYPos < 0f && position.y < 0f) || (beginYPos > 0f && position.y > 0f)) ? (-1f) : 1f);
		float currentDistanceBetweenPoints = Mathf.Abs(Mathf.Abs(beginYPos) + Mathf.Abs(position.y) * num);
		float t = 0f;
		do
		{
			t += RDTime.unscaledDeltaTime;
			float num2 = beginYPos + currentDistanceBetweenPoints * (float)(direction * -1) * movementCurve.Evaluate(t / timeToMove);
			float circleXPos = GetCircleXPos(254f, num2);
			rect.anchoredPosition = new Vector2(circleXPos, num2);
			yield return null;
		}
		while (t < timeToMove);
		rect.anchoredPosition = GetPosition();
	}

	public Vector2 GetPosition()
	{
		if (CirclePositions == null)
		{
			CirclePositions = new Vector2[11];
			int num = Mathf.FloorToInt((float)CirclePositions.Length / 2f);
			for (int i = 0; i < CirclePositions.Length; i++)
			{
				float f = Mathf.Round(29f * (float)num);
				if (Mathf.Abs(f) > 254f)
				{
					RDBaseDllDummy.printesw("DistanceBetweenPoints = " + 29f + " is too high for radius = " + 254f + ". Minimum radius accepted for " + 29f + " is " + 29f * (float)(scnCLS.instance.MaxHalfLength - 1));
				}
				else
				{
					float circleXPos = GetCircleXPos(254f, f);
					CirclePositions[i] = new Vector2(circleXPos, f);
					num--;
				}
			}
		}
		return CirclePositions[Mathf.Clamp(currentPositionIndex, 0, CirclePositions.Length - 1)];
	}

	private static float GetCircleXPos(float radius, float y)
	{
		return Mathf.Round(Mathf.Sqrt(radius * radius - y * y)) - radius;
	}

	public void ToggleSyringeUsb(bool showing, bool moveInstantly = false)
	{
		float endValue = (showing ? 179f : 170f);
		if (moveInstantly)
		{
			syringeUsb.anchoredPosition = new Vector2(endValue, syringeUsb.anchoredPosition.y);
			return;
		}
		syringeUsb.DOKill();
		syringeUsb.DOAnchorPosX(endValue, 0.2f).SetEase(Ease.OutFlash).SetUpdate(isIndependentUpdate: true);
	}

	public void ToggleLiquidPressing(bool pressing, bool deletedLevel = false)
	{
		float endValue = (pressing ? 162.5f : 6.5f);
		float endValue2 = (pressing ? 7f : (-140f));
		liquidRect.DOKill();
		liquidRect.DOAnchorPosX(endValue, 0.4f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutQuint);
		plungerRect.DOKill();
		if (pressing)
		{
			PlayPlungerIdle(play: false);
		}
		plungerRect.DOKill();
		plungerRect.DOAnchorPosX(endValue2, 0.4f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutQuint)
			.OnComplete(() =>
			{
				if (!pressing && !deletedLevel && !base.cls.Searching)
				{
					PlayPlungerIdle(play: true);
				}
			});
	}

	public void PlayPlungerIdle(bool play)
	{
		if (play)
		{
			if ((int)CurrentRank != -3)
			{
				plungerAnimation.Play();
				liquidAnimation.Play();
			}
		}
		else
		{
			plungerAnimation.Stop();
			liquidAnimation.Stop();
		}
	}

	public void FadeOverlay(bool fadeOut, bool animate = true)
	{
		overlayImage.DOKill();
		overlayBoxImage.DOKill();
		Image image;
		Image image2;
		if ((int)CurrentRank == -3)
		{
			image = overlayBoxImage;
			image2 = overlayImage;
		}
		else
		{
			image = overlayImage;
			image2 = overlayBoxImage;
		}
		image2.color = Color.black.WithAlpha(0f);
		float num = (fadeOut ? 0f : 0.4f);
		if (animate)
		{
			image.DOKill();
			image.DOFade(num, 0.4f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		}
		else
		{
			image.color = Color.black.WithAlpha(num);
		}
	}

	public void PlayUnwrapAnimation()
	{
		_ = boxBrokenAnimation.currentAnimationData.sprites.Length;
		_ = boxBrokenAnimation.singleSpriteDuration;
		boxBrokenAnimation.Play();
		scrConductor.PlayImmediately("sndLibrarySyringeUnwrap", 1f, RDUtils.GetMixerGroup("CustomLevelSelect"));
		float delay = 4f * boxBrokenAnimation.singleSpriteDuration;
		Vector2 boxBrokenStartPos = boxBroken.anchoredPosition;
		boxBroken.DOAnchorPosY(-130f, 0.5f).SetEase(Ease.InFlash).SetUpdate(isIndependentUpdate: true)
			.SetDelay(delay)
			.OnComplete(() =>
			{
				boxBroken.anchoredPosition = boxBrokenStartPos;
				boxBrokenAnimation.Stop();
				boxBroken.gameObject.SetActive(value: false);
			});
		float endValue = box.anchoredPosition.x - 200f;
		float positionToPlaySyringeBody = box.sizeDelta.x - 200f;
		Vector2 boxStartPos = box.anchoredPosition;
		box.DOAnchorPosX(endValue, 0.5f).SetEase(Ease.InQuart).SetUpdate(isIndependentUpdate: true)
			.SetDelay(delay)
			.OnUpdate(() =>
			{
				if (box.anchoredPosition.x <= positionToPlaySyringeBody)
				{
					bodyAnimation.Play();
				}
			})
			.OnComplete(() =>
			{
				StartCoroutine(base.cls.SendLevelDataToLevelDetail(loadingLevelsData: false, -1f));
				PlayPlungerIdle(play: true);
				box.anchoredPosition = boxStartPos;
				box.gameObject.SetActive(value: false);
				base.cls.CanReceiveInput = true;
			});
	}
}
