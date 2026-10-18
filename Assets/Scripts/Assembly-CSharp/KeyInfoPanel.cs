using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class KeyInfoPanel : RDBase
{
	[Serializable]
	public struct Info
	{
		public RectTransform rect;

		public HorizontalLayoutGroup layout;

		public float GeUnfoldWidth()
		{
			float num = 0f;
			num += (float)layout.padding.left;
			float spacing = layout.spacing;
			foreach (RectTransform item in rect)
			{
				if (item.gameObject.activeSelf)
				{
					num += item.sizeDelta.x + spacing;
				}
			}
			return num;
		}

		public float GetFoldWidth()
		{
			float num = 0f;
			num += (float)layout.padding.left;
			foreach (RectTransform item in rect)
			{
				if (item.gameObject.activeSelf)
				{
					num += item.sizeDelta.x;
					break;
				}
			}
			return num + layout.spacing;
		}
	}

	public static KeyInfoPanel instance;

	public static List<KeyHelper> lastShortcuts;

	private const float InfoPanelToggleAnimDuration = 0.3f;

	public bool usesWorkshopPanel;

	public Info shortcutsPanel;

	public Info workshopPanel;

	public Text stateText;

	public Text downloadingTitleText;

	public Text installationCounterText;

	public RectTransform downloadingTitleMask;

	public Image workshopPanelBackground;

	public Color workshopPanelBackgroundDefaultColor;

	public Color workshopPanelBackgroundFirstColor;

	public Color workshopPanelBackgroundSecondColor;

	[NonSerialized]
	public bool canRestart;

	[NonSerialized]
	public bool showingWorkshopPanel;

	[NonSerialized]
	public RectTransform rect;

	[NonSerialized]
	public KeyHelper[] shortcutKeyHelpers;

	private int itemsSubscribedCache;

	private int itemsInstalledCache;

	private const float MaxDownloadingTitleWidth = 150f;

	private Coroutine moveCoroutine;

	private void Awake()
	{
		instance = this;
		rect = GetComponent<RectTransform>();
		shortcutKeyHelpers = shortcutsPanel.rect.GetComponentsInChildren<KeyHelper>();
	}

	private void Start()
	{
		if (SteamIntegration.initialized && usesWorkshopPanel)
		{
			ToggleWorkshopPanel(show: false);
		}
		else
		{
			workshopPanel.rect.gameObject.SetActive(value: false);
		}
	}

	private void LateUpdate()
	{
		if (!SteamIntegration.initialized || !usesWorkshopPanel)
		{
			return;
		}
		int itemsSubscribed = SteamWorkshop.ItemsSubscribed;
		int itemsUnsubscribed = SteamWorkshop.itemsUnsubscribed;
		if (itemsSubscribed <= 0 && itemsUnsubscribed <= 0)
		{
			if (showingWorkshopPanel)
			{
				ToggleWorkshopPanel(show: false);
			}
			return;
		}
		bool flag = false;
		string getCurrentDownloadingItemTitle = SteamWorkshop.GetCurrentDownloadingItemTitle;
		if (downloadingTitleText.text != getCurrentDownloadingItemTitle)
		{
			flag = true;
			downloadingTitleText.text = SteamWorkshop.GetCurrentDownloadingItemTitle;
			float num = downloadingTitleText.preferredWidth;
			if (num > 150f)
			{
				num = 150f;
			}
			downloadingTitleMask.sizeDelta = downloadingTitleMask.sizeDelta.WithX(num);
			bool flag2 = string.IsNullOrEmpty(downloadingTitleText.text);
			downloadingTitleMask.gameObject.SetActive(!flag2);
		}
		int itemsInstalled = SteamWorkshop.ItemsInstalled;
		if (itemsInstalledCache != itemsInstalled || itemsSubscribedCache != itemsSubscribed)
		{
			flag = true;
			itemsInstalledCache = itemsInstalled;
			itemsSubscribedCache = itemsSubscribed;
			bool flag3 = itemsSubscribedCache > 0;
			stateText.gameObject.SetActive(flag3);
			if (flag3)
			{
				bool num2 = itemsSubscribed == itemsInstalled;
				string key = (num2 ? "customLevelSelect.helpPanel.allInstalled" : "customLevelSelect.helpPanel.downloading");
				stateText.text = RDString.Get(key);
				if (num2)
				{
					stateText.text = stateText.text.Replace("[levelsNumber]", itemsSubscribed.ToString());
				}
				stateText.font = RDString.GetAppropiateFontForString(stateText.text);
			}
			bool flag4 = itemsInstalled < itemsSubscribed;
			installationCounterText.gameObject.SetActive(flag4);
			if (flag4)
			{
				installationCounterText.text = $"({itemsInstalled}/{itemsSubscribed})";
			}
		}
		else if (itemsSubscribed == 0)
		{
			if (stateText.IsActive())
			{
				flag = true;
				stateText.text = string.Empty;
				stateText.gameObject.SetActive(value: false);
			}
			if (installationCounterText.IsActive())
			{
				flag = true;
				installationCounterText.text = string.Empty;
				installationCounterText.gameObject.SetActive(value: false);
			}
		}
		if (flag || !showingWorkshopPanel)
		{
			ToggleWorkshopPanel(show: true);
		}
	}

	public void UpdateShortcutsPanel(bool forcehide = false)
	{
		if (moveCoroutine != null)
		{
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = StartCoroutine(Move(forcehide));
	}

	private IEnumerator Move(bool forcehide)
	{
		bool flag;
		do
		{
			yield return null;
			flag = true;
			KeyHelper[] array = shortcutKeyHelpers;
			foreach (KeyHelper keyHelper in array)
			{
				if (keyHelper.gameObject.activeSelf)
				{
					flag = keyHelper.enabledOnce;
					if (!flag)
					{
						break;
					}
				}
			}
		}
		while (!flag);
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate(shortcutsPanel.rect);
		float endValue = ((showingWorkshopPanel | forcehide) ? 0f : (0f - shortcutsPanel.GeUnfoldWidth()));
		shortcutsPanel.rect.DOKill();
		shortcutsPanel.rect.DOAnchorPosX(endValue, 0.3f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
	}

	public void ToggleWorkshopPanel(bool show)
	{
		showingWorkshopPanel = show;
		scrConductor.PlayImmediately(show ? "sndShortcutsPanelSlideIn" : "sndShortcutsPanelSlideOut", 1f, RDUtils.GetMixerGroup("CustomLevelSelect"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate(workshopPanel.rect);
		UpdateShortcutsRow(null);
		if (show)
		{
			Narration.Say(stateText.text + " " + downloadingTitleText.text + " " + installationCounterText.text, NarrationCategory.Notification, false);
		}
		workshopPanelBackground.DOKill();
		Color endValue = (show ? workshopPanelBackgroundFirstColor : workshopPanelBackgroundDefaultColor);
		workshopPanelBackground.DOColor(endValue, 1f).SetEase(Ease.OutExpo).OnComplete(() =>
		{
			if (!show)
			{
				workshopPanelBackground.DOColor(workshopPanelBackgroundSecondColor, 1f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
			}
		});
		float endValue2 = (show ? (0f - workshopPanel.GeUnfoldWidth()) : 0f);
		workshopPanel.rect.DOKill();
		workshopPanel.rect.DOAnchorPosX(endValue2, 0.3f).SetEase(Ease.OutExpo);
	}

	public void RestorePreviousShortcuts()
	{
		if (lastShortcuts != null)
		{
			KeyHelper[] array = shortcutKeyHelpers;
			foreach (KeyHelper keyHelper in array)
			{
				keyHelper.gameObject.SetActive(lastShortcuts.Contains(keyHelper));
			}
		}
	}

	public void UpdateShortcutsRow(KeyCode[] shortcuts)
	{
		if (shortcuts == null)
		{
			RestorePreviousShortcuts();
		}
		else
		{
			if (shortcuts == null)
			{
				return;
			}
			if (shortcuts.Length == 0)
			{
				UpdateShortcutsPanel(forcehide: true);
				return;
			}
			List<KeyHelper> list = new List<KeyHelper>();
			KeyHelper[] array = shortcutKeyHelpers;
			foreach (KeyHelper keyHelper in array)
			{
				bool flag = false;
				KeyHelperButton[] buttons = keyHelper.buttons;
				foreach (KeyHelperButton keyHelperButton in buttons)
				{
					if (shortcuts.Contains(keyHelperButton.keyCode))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					if (keyHelper.currentControllerType != KeyHelper.currentInfo.type)
					{
						keyHelper.ControllerTypeWasUpdated();
					}
					flag = keyHelper.currentControllerType == ControllerType.Keyboard || keyHelper.shouldShow;
				}
				if (flag)
				{
					list.Add(keyHelper);
				}
				keyHelper.gameObject.SetActive(flag);
			}
			lastShortcuts = list;
			UpdateShortcutsPanel();
		}
	}

	public void UpdateShortcutsRowWithStrings(string[] localizationKeys)
	{
		if (localizationKeys == null)
		{
			RestorePreviousShortcuts();
			return;
		}
		if (localizationKeys.Length == 0)
		{
			UpdateShortcutsPanel(forcehide: true);
			return;
		}
		List<KeyHelper> list = new List<KeyHelper>();
		KeyHelper[] array = shortcutKeyHelpers;
		foreach (KeyHelper keyHelper in array)
		{
			bool flag = false;
			foreach (string text in localizationKeys)
			{
				if (keyHelper.localizationKey == text)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				if (keyHelper.currentControllerType != KeyHelper.currentInfo.type)
				{
					keyHelper.ControllerTypeWasUpdated();
				}
				flag = keyHelper.currentControllerType == ControllerType.Keyboard || keyHelper.shouldShow;
			}
			if (flag)
			{
				list.Add(keyHelper);
			}
			keyHelper.gameObject.SetActive(flag);
		}
		UpdateShortcutsPanel();
	}
}
