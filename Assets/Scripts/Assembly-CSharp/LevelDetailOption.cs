using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelDetailOption : RDBase
{
	public LevelDetail.DetailOptionName optionName;

	public CLSAnimation[] animations;

	[Header("Press Actions")]
	public Image backgroundImage;

	public Sprite defaultBackgroundSprite;

	public Sprite selectedBackgroundSprite;

	public Sprite pressedBackgroundSprite;

	private const float pressedSpriteDuration = 0.2f;

	private bool canBePressed = true;

	[Header("Selector")]
	public bool usesSelector = true;

	public RectTransform optionSelector;

	public Vector2 selectorPosition;

	public Vector2 selectorSize;

	[Header("Content")]
	public RectTransform contentContainer;

	public Vector2 contentSelectedPos;

	public Vector2 contentUnselectedPos;

	[Header("Confirm Options")]
	public bool usesOptions;

	public CanvasGroup contentContainerCanvas;

	public CanvasGroup optionsContainerCanvas;

	public LevelDetailOption confirmOption;

	public LevelDetailOption cancelOption;

	public bool confirmSelectedByDefault;

	public RectTransform selector;

	public float selectorConfirmXPos;

	public float selectorCancelXPos;

	[Header("Main Label")]
	public Text mainLabel;

	public Image labelIcon;

	public string mainLabelDefaultKey;

	public string mainLabelPressedKey;

	public float labelCJKHeight;

	public const float mainLabelTextChangeDuration = 0.5f;

	private bool _confirmOptionIsSelected;

	private bool _showingOptions;

	public bool ConfirmOptionIsSelected
	{
		get
		{
			return _confirmOptionIsSelected;
		}
		set
		{
			if (!usesOptions)
			{
				return;
			}
			_confirmOptionIsSelected = value;
			if (_confirmOptionIsSelected)
			{
				if (selector != null)
				{
					selector.anchoredPosition = selector.anchoredPosition.WithX(selectorConfirmXPos);
				}
				confirmOption.Select();
				cancelOption.Deselect();
			}
			else
			{
				if (selector != null)
				{
					selector.anchoredPosition = selector.anchoredPosition.WithX(selectorCancelXPos);
				}
				cancelOption.Select();
				confirmOption.Deselect();
			}
		}
	}

	public bool ShowingOptions
	{
		get
		{
			return _showingOptions;
		}
		set
		{
			if (usesOptions)
			{
				_showingOptions = value;
				if (selector != null)
				{
					selector.gameObject.SetActive(_showingOptions);
				}
				if (_showingOptions)
				{
					contentContainerCanvas.DOFade(0f, 0.2f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
					optionsContainerCanvas.DOFade(1f, 0.2f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
				}
				else
				{
					contentContainerCanvas.DOFade(1f, 0.2f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
					optionsContainerCanvas.DOFade(0f, 0.2f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
				}
			}
		}
	}

	private void Awake()
	{
		if (mainLabel != null && labelCJKHeight != 0f && RDString.isCJK)
		{
			RectTransform component = mainLabel.GetComponent<RectTransform>();
			component.anchoredPosition = new Vector2(component.anchoredPosition.x, component.anchoredPosition.y + labelCJKHeight);
		}
	}

	public void Select()
	{
		ShowingOptions = false;
		backgroundImage.sprite = selectedBackgroundSprite;
		contentContainer.anchoredPosition = contentSelectedPos;
		if (usesSelector)
		{
			optionSelector.DOComplete(withCallbacks: true);
			optionSelector.SetParent(base.transform);
			optionSelector.anchoredPosition = selectorPosition;
			optionSelector.SetParent(base.transform.parent);
			optionSelector.sizeDelta = selectorSize;
		}
		CLSAnimation[] array = animations;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	public void Press(bool showOptions = false)
	{
		if (canBePressed && base.gameObject.activeInHierarchy)
		{
			StartCoroutine(Pressed(showOptions));
		}
	}

	private IEnumerator Pressed(bool showOptions)
	{
		canBePressed = false;
		backgroundImage.sprite = pressedBackgroundSprite;
		if (usesOptions & showOptions)
		{
			if (!ShowingOptions)
			{
				ConfirmOptionIsSelected = confirmSelectedByDefault;
				ShowingOptions = true;
				canBePressed = true;
				backgroundImage.sprite = selectedBackgroundSprite;
				yield break;
			}
			if (ConfirmOptionIsSelected)
			{
				confirmOption.Press();
			}
			else
			{
				cancelOption.Press();
			}
			yield return new WaitForSecondsRealtime(0.2f);
			Select();
			canBePressed = true;
		}
		else
		{
			canBePressed = true;
			contentContainer.anchoredPosition = contentUnselectedPos;
			yield return new WaitForSecondsRealtime(0.2f);
			contentContainer.anchoredPosition = contentSelectedPos;
			backgroundImage.sprite = selectedBackgroundSprite;
		}
	}

	public void Deselect()
	{
		backgroundImage.sprite = defaultBackgroundSprite;
		contentContainer.anchoredPosition = contentUnselectedPos;
		CLSAnimation[] array = animations;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
		if (usesOptions)
		{
			ShowingOptions = false;
			ConfirmOptionIsSelected = confirmSelectedByDefault;
		}
	}
}
