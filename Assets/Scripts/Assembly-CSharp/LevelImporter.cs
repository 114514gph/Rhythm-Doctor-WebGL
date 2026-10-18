using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityFileDialog;

public class LevelImporter : RDBase
{
	public enum ContentName
	{
		InstallLevelsDraggable = 0,
		InstallLevelsList = 1,
		Installing = 2,
		LevelsInstalled = 3,
		Issues = 4
	}

	public enum ToggleButton
	{

	}

	private const float ContentHeight = 18f;

	private readonly List<string> ValidExtensions = new List<string>(new string[2] { "rdzip", "zip" });

	private const string InvalidUrlToken = "customLevelSelect.import.addFromURLError";

	public RectTransform screenContainer;

	public RectTransform closeArrow;

	public Image fadeBackground;

	public GameObject dragAndDrop;

	public Text contentTitleText;

	public GameObject issuesContainer;

	public Text issuesLevelText;

	public Text issuesErrorText;

	public GameObject importLevelPrefab;

	public LevelErrorContainer levelErrorContainer;

	[Header("Draggable Content")]
	public GameObject draggableContainer;

	public GameObject instructions_Draggable;

	public GameObject instructions_NonDraggable;

	public Button draggableBrowseButton;

	public Button addFromUrlButton;

	public Button addButton;

	[Header("Top Panel Buttons")]
	public GameObject options;

	public Text addLabel;

	public Button closeButton;

	public Button insertUrl;

	public Button browseButton;

	public Button stopButton;

	public Text stopButtonLabel;

	public Button installButton;

	public Button startOverButton;

	public Button backToLevelsInstalledButton;

	[Header("Content Info Buttons")]
	public Transform toggleButtonsDefaultParent;

	public Button removeButton;

	public Button checkIssuesButton;

	public Button clearButton;

	public EventSystem eventSystem;

	[Header("Info Scroll View Content")]
	public ScrollRect infoScrollView;

	public LevelImporterInfoSection errorsIS;

	public LevelImporterInfoSection installedIS;

	public LevelImporterInfoSection toInstallIS;

	public Scrollbar infoContentVerticalScrollBar;

	[Header("Insert URL")]
	public GameObject insertURlContainer;

	public GameObject infoScrollViewContainer;

	public InputField urlInput;

	private bool _showing;

	private ContentName _currentContentName;

	private int _currentTopButtonIndex;

	private bool _canToggleClearButton;

	private RectTransform removeButtonRect;

	private RectTransform checkIssuesButtonRect;

	private RectTransform infoScrollViewContent;

	private bool stoppedInstallCoroutine;

	private List<ImportLevel> levelsToInstall;

	private List<KeyCode> shortcuts = new List<KeyCode>
	{
		KeyCode.Tab,
		KeyCode.Escape,
		KeyCode.F
	};

	private bool errorsImportingLevel;

	private int lastUrlInputTextLength;

	private Button[] topButtons;

	private int frameToSelectAddButton;

	private PointerEventData pointer;

	private List<RaycastResult> raycastResult = new List<RaycastResult>();

	private GameObject lastToggleableButtonContent;

	private bool didCurrentTopButtonIndexChangeByUserInput;

	private ContentName lastContentName;

	private GameObject lastSelected;

	private const string LevelPackageNotFoundError = "customLevelSelect.import.levelPackageNotFound";

	private const string UnzippingError = "customLevelSelect.import.unzippingError";

	private const string MainSettingsError = "customLevelSelect.import.checkErrors";

	private const string MainEmptyError = "customLevelSelect.import.mainFileEmptyError";

	private const string MainCorruptError = "customLevelSelect.import.mainFileCorruptError";

	private const string MainNotFoundError = "customLevelSelect.import.mainFileNotFoundError";

	private const string MovingToCLSPathError = "customLevelSelect.import.movingToCLSPathError";

	public bool Showing
	{
		get
		{
			return _showing;
		}
		set
		{
			if (_showing == value)
			{
				return;
			}
			_showing = value;
			base.cls.CanReceiveInput = false;
			toInstallIS.Clear();
			errorsIS.Clear();
			installedIS.Clear();
			ToggleInsertUrlContainer(show: false, select: false);
			CurrentContentName = ContentName.InstallLevelsDraggable;
			if (_showing)
			{
				base.gameObject.SetActive(value: true);
				Select(draggableBrowseButton);
			}
			DOVirtual.DelayedCall(_showing ? 0.8f : 0.24000001f, () =>
			{
				if (_showing)
				{
					base.cls.infoPanel.UpdateShortcutsRow(shortcuts.ToArray());
				}
				else if (base.cls.ShowingWard)
				{
					base.cls.infoPanel.UpdateShortcutsRow(scnCLS.WardShortcuts);
				}
				base.cls.CanReceiveInput = true;
			});
			float endValue = (_showing ? 0.5f : 0f);
			fadeBackground.DOKill();
			if (_showing)
			{
				fadeBackground.DOFade(endValue, 0.16000001f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutCubic)
					.SetDelay(0.64000005f);
			}
			else
			{
				fadeBackground.DOFade(endValue, 0.8f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutCubic);
			}
			float endValue2 = (_showing ? 200f : (base.cls.ShowingWard ? LevelDetail.IdleWardPos.y : LevelDetail.IdleLevelsPos.y));
			RectTransform rect = base.cls.levelDetail.rect;
			rect.DOKill();
			if (_showing)
			{
				rect.DOAnchorPosY(endValue2, 0.56f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InBack, 1.2f);
			}
			else
			{
				rect.DOAnchorPosY(endValue2, 0.8f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack, 1.2f);
			}
			float endValue3 = (_showing ? 4f : (-20f));
			closeArrow.DOKill();
			if (_showing)
			{
				closeArrow.DOAnchorPosX(endValue3, 0.4f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack, 1.1f)
					.SetDelay(0.56f);
			}
			else
			{
				closeArrow.DOAnchorPosX(endValue3, 0.8f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack);
			}
			float endValue4 = (_showing ? 0f : (-200f));
			screenContainer.DOKill();
			if (_showing)
			{
				screenContainer.DOAnchorPosY(endValue4, 0.4f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack, 1.1f)
					.SetDelay(0.56f);
			}
			else
			{
				screenContainer.DOAnchorPosY(endValue4, 0.8f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack, 1.05f)
					.OnComplete(() =>
					{
						base.gameObject.SetActive(value: false);
						if (!base.cls.ShowDetailStaticNoise)
						{
							base.cls.levelDetail.AnimateDetailIdle(animate: true);
						}
					});
			}
			string sound = (_showing ? "sndImportPanelOpen" : "sndImportPanelClose");
			base.cls.CLSPlaySound(sound);
			if (_showing)
			{
				NarrateContext();
			}
			else if (base.cls.ShowingWard)
			{
				base.cls.NarrateWardName(fullContext: true);
			}
			else
			{
				base.cls.levelDetail.NarrateIntro();
			}
		}
	}

	public bool CanDragAndDrop
	{
		get
		{
			return dragAndDrop.activeSelf;
		}
		set
		{
			dragAndDrop.SetActive(value);
		}
	}

	public ContentName CurrentContentName
	{
		get
		{
			return _currentContentName;
		}
		set
		{
			_currentContentName = value;
			bool active = false;
			bool flag = true;
			bool active2 = false;
			bool active3 = false;
			bool active4 = false;
			bool active5 = false;
			bool active6 = false;
			bool active7 = false;
			CanDragAndDrop = false;
			infoScrollView.gameObject.SetActive(value: true);
			draggableContainer.SetActive(value: false);
			issuesContainer.SetActive(value: false);
			options.SetActive(value: true);
			switch (_currentContentName)
			{
			case ContentName.InstallLevelsDraggable:
				contentTitleText.text = RDString.Get("customLevelSelect.import.installLevels");
				active6 = (active2 = true);
				infoScrollView.gameObject.SetActive(value: false);
				draggableContainer.SetActive(value: true);
				CanDragAndDrop = true;
				instructions_Draggable.gameObject.SetActive(CanDragAndDrop);
				instructions_NonDraggable.gameObject.SetActive(!CanDragAndDrop);
				options.SetActive(value: false);
				break;
			case ContentName.InstallLevelsList:
				contentTitleText.text = RDString.Get("customLevelSelect.import.installLevels");
				active6 = (active2 = (active7 = true));
				CanDragAndDrop = true;
				if (toInstallIS.levels.Count > 0)
				{
					toInstallIS.gameObject.SetActive(value: true);
					active3 = true;
				}
				if (insertURlContainer.activeSelf)
				{
					ToggleInsertUrlContainer(show: false);
				}
				break;
			case ContentName.Installing:
				contentTitleText.text = RDString.Get("customLevelSelect.import.installing");
				active = true;
				flag = false;
				break;
			case ContentName.LevelsInstalled:
				contentTitleText.text = RDString.Get("customLevelSelect.import.levelsInstalled");
				active4 = true;
				if (toInstallIS.levels.Count > 0)
				{
					active3 = true;
				}
				break;
			case ContentName.Issues:
				contentTitleText.text = RDString.Get("customLevelSelect.import.issues");
				active5 = true;
				infoScrollView.gameObject.SetActive(value: false);
				issuesContainer.SetActive(value: true);
				break;
			}
			contentTitleText.font = RDString.GetAppropiateFontForString(contentTitleText.text);
			insertUrl.gameObject.SetActive(active6);
			browseButton.gameObject.SetActive(active2);
			stopButton.gameObject.SetActive(active);
			closeButton.gameObject.SetActive(flag);
			installButton.gameObject.SetActive(active3);
			startOverButton.gameObject.SetActive(active4);
			backToLevelsInstalledButton.gameObject.SetActive(active5);
			addLabel.gameObject.SetActive(active7);
			if (flag)
			{
				if (shortcuts.IndexOf(KeyCode.Escape) == -1)
				{
					shortcuts.Add(KeyCode.Escape);
				}
			}
			else if (shortcuts.IndexOf(KeyCode.Escape) != -1)
			{
				shortcuts.Remove(KeyCode.Escape);
			}
			base.cls.infoPanel.UpdateShortcutsRow(shortcuts.ToArray());
			CurrentTopButtonIndex = 0;
		}
	}

	private EventSystem es => EventSystem.current;

	private int CurrentTopButtonIndex
	{
		get
		{
			return _currentTopButtonIndex;
		}
		set
		{
			int currentTopButtonIndex = _currentTopButtonIndex;
			_currentTopButtonIndex = value;
			bool flag = false;
			if (CurrentTopButton == null)
			{
				_currentTopButtonIndex = currentTopButtonIndex;
				base.cls.CLSPlaySound("sndPagerCursorBorder");
				flag = true;
			}
			Select(CurrentTopButton);
			if (didCurrentTopButtonIndexChangeByUserInput && !flag)
			{
				if (CurrentTopButton == browseButton)
				{
					base.cls.CLSPlaySound("sndImportMainButtonHighlight", 1f, "CustomLevelSelect", RDUtils.PitchSemitonesToPercent(2f));
					NarrateItem(1);
				}
				else if (CurrentTopButton == insertUrl)
				{
					base.cls.CLSPlaySound("sndImportMainButtonHighlight", 1f, "CustomLevelSelect", RDUtils.PitchSemitonesToPercent(-2f));
					NarrateItem(2);
				}
				else if (CurrentTopButton == installButton)
				{
					base.cls.CLSPlaySound("sndImportInstallButtonHighlight");
					NarrateItem(3);
				}
			}
			didCurrentTopButtonIndexChangeByUserInput = false;
		}
	}

	private Button CurrentTopButton
	{
		get
		{
			int num = 0;
			for (int i = 0; i < topButtons.Length; i++)
			{
				if (topButtons[i].gameObject.activeSelf)
				{
					if (num == CurrentTopButtonIndex)
					{
						return topButtons[i];
					}
					num++;
				}
			}
			return null;
		}
	}

	private bool CanToggleClearButton
	{
		get
		{
			return _canToggleClearButton;
		}
		set
		{
			_canToggleClearButton = value;
			if (!_canToggleClearButton)
			{
				HideToggleableButtons();
			}
		}
	}

	public void Initialize()
	{
		dragAndDrop.GetComponent<FileDragAndDrop>().OnFilesDropped = (string[] filesPath) =>
		{
			RDBaseDllDummy.printem(filesPath.Length);
			foreach (string zipPath in filesPath)
			{
				StartCoroutine(AddContent(zipPath));
			}
		};
		CanToggleClearButton = true;
		removeButtonRect = removeButton.GetComponent<RectTransform>();
		checkIssuesButtonRect = checkIssuesButton.GetComponent<RectTransform>();
		infoScrollViewContent = infoScrollView.content;
		errorsIS.levels = new List<ImportLevel>();
		installedIS.levels = new List<ImportLevel>();
		toInstallIS.levels = new List<ImportLevel>();
		Text component = urlInput.placeholder.GetComponent<Text>();
		Font appropiateFontForString = RDString.GetAppropiateFontForString(component.text);
		if (component.font != appropiateFontForString)
		{
			component.font = appropiateFontForString;
			component.fontSize = 8;
		}
		urlInput.onValueChanged.AddListener(delegate
		{
			urlInput.textComponent.font = RDString.GetAppropiateFontForString(urlInput.text);
			urlInput.textComponent.fontSize = ((urlInput.textComponent.font.name == "RDLatinFontPoint") ? 8 : 6);
		});
		base.gameObject.SetActive(value: false);
		topButtons = options.GetComponentsInChildren<Button>(includeInactive: true);
		pointer = new PointerEventData(eventSystem);
	}

	private void Update()
	{
		if (frameToSelectAddButton == Time.frameCount)
		{
			Select(addButton);
			Narration.Stop();
			Narration.Say(RDString.Get("narration.pasteSuccess"), NarrationCategory.Notification, false);
			Narration.Say(RDString.Get("narration.pressEnterToConfirm"), NarrationCategory.Instruction, false);
		}
		pointer.position = Input.mousePosition;
		eventSystem.RaycastAll(pointer, raycastResult);
		bool flag = false;
		foreach (RaycastResult item in raycastResult)
		{
			ImportLevel component = item.gameObject.GetComponent<ImportLevel>();
			LevelImporterInfoSection component2 = item.gameObject.GetComponent<LevelImporterInfoSection>();
			if (component != null || component2 != null)
			{
				flag = true;
				if (lastToggleableButtonContent == item.gameObject)
				{
					break;
				}
				lastToggleableButtonContent = item.gameObject;
				HideToggleableButtons();
				if (component != null)
				{
					ShowLevelToggleableButtons(component);
				}
				else if (component2 != null)
				{
					ShowSectionClearButton(component2);
				}
			}
		}
		if (!flag && lastToggleableButtonContent != null)
		{
			lastToggleableButtonContent = null;
			HideToggleableButtons();
		}
		if (!base.cls.CanReceiveInput)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.F) && !insertURlContainer.activeSelf)
		{
			scrVfxControl.SetFullscreen(!Screen.fullScreen);
		}
		else if (RDInput.cancelPress && closeButton.gameObject.activeSelf && !insertURlContainer.activeSelf)
		{
			Showing = false;
		}
		else if ((CurrentContentName == ContentName.InstallLevelsDraggable || CurrentContentName == ContentName.InstallLevelsList) && insertURlContainer.activeSelf)
		{
			if (RDInput.cancelPress)
			{
				ToggleInsertUrlContainer(show: false, select: true, playSound: true);
			}
			else if ((Input.GetKeyDown(KeyCode.Return) && es.currentSelectedGameObject == addButton.gameObject) || (RDInput.controlIsPressed && Input.GetKeyDown(KeyCode.Return)))
			{
				ValidateUrl();
				base.cls.CLSPlaySound("sndImportMainButtonClick");
			}
		}
		else if (CurrentContentName == ContentName.InstallLevelsDraggable)
		{
			if (RDInput.upPress || RDInput.leftPress)
			{
				if (es.currentSelectedGameObject == draggableBrowseButton.gameObject)
				{
					base.cls.CLSPlaySound("sndPagerCursorBorder");
					return;
				}
				Select(draggableBrowseButton);
				base.cls.CLSPlaySound("sndImportMainButtonHighlight", 1f, "CustomLevelSelect", RDUtils.PitchSemitonesToPercent(2f));
				NarrateItem(1);
			}
			else if (RDInput.downPress || RDInput.rightPress)
			{
				if (es.currentSelectedGameObject == addFromUrlButton.gameObject)
				{
					base.cls.CLSPlaySound("sndPagerCursorBorder");
					return;
				}
				Select(addFromUrlButton);
				base.cls.CLSPlaySound("sndImportMainButtonHighlight", 1f, "CustomLevelSelect", RDUtils.PitchSemitonesToPercent(-2f));
				NarrateItem(2);
			}
			else if (Input.GetKeyDown(KeyCode.Return))
			{
				if (es.currentSelectedGameObject == draggableBrowseButton.gameObject)
				{
					Browse();
				}
				else
				{
					ToggleInsertUrlContainer(show: true, select: true, playSound: true);
				}
			}
		}
		else if (RDInput.upPress || RDInput.leftPress)
		{
			didCurrentTopButtonIndexChangeByUserInput = true;
			CurrentTopButtonIndex--;
		}
		else if (RDInput.downPress || RDInput.rightPress)
		{
			didCurrentTopButtonIndexChangeByUserInput = true;
			CurrentTopButtonIndex++;
		}
		else if (Input.GetKeyDown(KeyCode.Return))
		{
			if (CurrentTopButton == browseButton)
			{
				Browse();
			}
			else if (CurrentTopButton == insertUrl)
			{
				ToggleInsertUrlContainer(show: true, select: true, playSound: true);
			}
			else if (CurrentTopButton == installButton)
			{
				StartCoroutine(Install());
			}
			else if (CurrentTopButton == stopButton)
			{
				Stop();
			}
			else if (CurrentTopButton == startOverButton)
			{
				StartOver();
			}
			else if (CurrentTopButton == backToLevelsInstalledButton)
			{
				BackToLevelsInstalled();
			}
		}
	}

	public void LateUpdate()
	{
		if (!urlInput.text.IsNullOrEmpty() && urlInput.text.Length - lastUrlInputTextLength > 6)
		{
			frameToSelectAddButton = Time.frameCount + 1;
		}
		lastUrlInputTextLength = urlInput.text.Length;
		if (CurrentContentName != lastContentName)
		{
			RDBaseDllDummy.printem("current content name: " + CurrentContentName);
			lastContentName = CurrentContentName;
		}
		if (es.currentSelectedGameObject != lastSelected)
		{
			RDBaseDllDummy.printem("selected gameobject: " + es.currentSelectedGameObject);
			lastSelected = es.currentSelectedGameObject;
		}
	}

	private void Select(Selectable selectable)
	{
		selectable.Select();
		selectable.OnSelect(null);
	}

	public void Close()
	{
		Showing = false;
	}

	private void ShowSectionClearButton(LevelImporterInfoSection infoSection)
	{
		if (!CanToggleClearButton)
		{
			return;
		}
		base.cls.CLSPlaySound("sndImportClearButtonHighlight");
		clearButton.gameObject.SetActive(value: true);
		clearButton.transform.SetParent(infoSection.transform);
		clearButton.onClick.RemoveAllListeners();
		clearButton.onClick.AddListener(() =>
		{
			infoSection.Clear();
			base.cls.CLSPlaySound("sndImportClearButtonClick");
			if (toInstallIS.levels.Count == 0 && installedIS.levels.Count == 0 && errorsIS.levels.Count == 0)
			{
				CurrentContentName = ContentName.InstallLevelsDraggable;
			}
		});
	}

	public void HideToggleableButtons()
	{
		clearButton.gameObject.SetActive(value: false);
		clearButton.transform.SetParent(toggleButtonsDefaultParent);
		clearButton.onClick.RemoveAllListeners();
		removeButton.gameObject.SetActive(value: false);
		removeButton.transform.SetParent(toggleButtonsDefaultParent);
		removeButton.onClick.RemoveAllListeners();
		checkIssuesButton.gameObject.SetActive(value: false);
		checkIssuesButton.transform.SetParent(toggleButtonsDefaultParent);
		checkIssuesButton.onClick.RemoveAllListeners();
	}

	public void BackToLevelsInstalled()
	{
		CurrentContentName = ContentName.LevelsInstalled;
		base.cls.PlayHammerSound();
	}

	public void Stop()
	{
		stoppedInstallCoroutine = true;
		RDPackageInstaller.cancelDownload = true;
		stopButton.interactable = false;
		stopButtonLabel.text = RDString.Get("customLevelSelect.import.stopping");
		stopButtonLabel.font = RDString.GetAppropiateFontForString(stopButtonLabel.text);
		base.cls.PlayHammerSound();
	}

	public void Browse()
	{
		string text = RDString.Get("customLevelSelect.import.rdzipFiles");
		ToggleInsertUrlContainer(show: false, select: false);
		string[] obj = FileBrowser.PickFiles(scnEditor.GetLastUsedFolder(), text, new string[2] { "rdzip", "zip" }, text) ?? Array.Empty<string>();
		List<string> list = new List<string>();
		bool flag = true;
		string[] array = obj;
		foreach (string text2 in array)
		{
			string text3 = Path.GetExtension(Path.GetFileName(text2)).Replace(".", "");
			if (flag)
			{
				scnEditor.SaveLastUsedFile(text2);
			}
			if (ValidExtensions.Contains(text3.ToLowerInvariant()))
			{
				string item = Uri.UnescapeDataString(text2.Replace("file:", ""));
				list.Add(item);
			}
			flag = false;
		}
		foreach (string item2 in list)
		{
			StartCoroutine(AddContent(item2));
		}
		NarrateContext();
		NarrateInfoSection(toInstallIS);
	}

	public void ShowLevelToggleableButtons(ImportLevel level)
	{
		if (level.canToggleRemoveButton)
		{
			base.cls.CLSPlaySound("sndImportClearButtonHighlight");
			removeButton.gameObject.SetActive(value: true);
			removeButton.transform.SetParent(level.transform);
			removeButtonRect.anchoredPosition = new Vector2(removeButtonRect.anchoredPosition.x, 0f);
			removeButton.onClick.RemoveAllListeners();
			removeButton.onClick.AddListener(() =>
			{
				RemoveImportedLevel(level);
				base.cls.CLSPlaySound("sndImportClearButtonClick");
			});
		}
		else if (level.canToggleCheckIssuesButton)
		{
			base.cls.CLSPlaySound("sndImportIssuesButtonHighlight");
			checkIssuesButton.gameObject.SetActive(value: true);
			checkIssuesButton.transform.SetParent(level.transform);
			checkIssuesButtonRect.anchoredPosition = new Vector2(removeButtonRect.anchoredPosition.x, 0f);
			checkIssuesButton.onClick.RemoveAllListeners();
			checkIssuesButton.onClick.AddListener(() =>
			{
				CheckLevelsIssues(level);
				base.cls.CLSPlaySound("sndImportIssuesButtonClick");
			});
		}
	}

	public void RemoveImportedLevel(ImportLevel level)
	{
		toInstallIS.Remove(level);
		removeButton.transform.SetParent(toggleButtonsDefaultParent);
		removeButton.gameObject.SetActive(value: false);
		UnityEngine.Object.Destroy(level.gameObject);
		UpdateInfoContentHeight();
		if (toInstallIS.levels.Count == 0 && installedIS.levels.Count == 0 && errorsIS.levels.Count == 0)
		{
			CurrentContentName = ContentName.InstallLevelsDraggable;
		}
	}

	public void Install_Public()
	{
		StartCoroutine(Install());
	}

	private IEnumerator Install()
	{
		base.cls.CLSPlaySound("sndImportInstallButtonClick");
		ToggleInsertUrlContainer(show: false);
		CurrentContentName = ContentName.Installing;
		levelsToInstall = new List<ImportLevel>(toInstallIS.levels);
		stoppedInstallCoroutine = false;
		Narration.Say(RDString.Get("customLevelSelect.import.installingLevels").Replace("[number]", levelsToInstall.Count.ToString()), NarrationCategory.Notification, false);
		CanToggleClearButton = false;
		foreach (ImportLevel item in levelsToInstall)
		{
			item.canToggleRemoveButton = false;
		}
		List<char> list = new List<char>(Path.GetInvalidPathChars());
		list.AddRange(Path.GetInvalidFileNameChars());
		list.Distinct().ToArray();
		yield return null;
		foreach (ImportLevel level in levelsToInstall)
		{
			new List<string>();
			bool shouldDeleteFile = false;
			errorsImportingLevel = false;
			level.progressImage.SetActive(value: true);
			if (level.isUrl)
			{
				string filename = string.Empty;
				bool alreadyDownloadedFile = false;
				level.progressText.SetActive(value: true);
				yield return RDPackageInstaller.GetFileNameFromUrl(level.path, (string _filename, string _defaultFilePath) =>
				{
					filename = _filename;
					if (!string.IsNullOrEmpty(_defaultFilePath))
					{
						alreadyDownloadedFile = true;
						level.path = _defaultFilePath;
					}
				});
				if (!alreadyDownloadedFile)
				{
					string resultFilePath = Path.Combine(LevelValidation.TempLevelsFolder, filename);
					resultFilePath = RDUtils.GetAvailableFilename(resultFilePath, Path.GetExtension(resultFilePath));
					yield return RDPackageInstaller.DownloadPackage(level.path, resultFilePath, level.progress);
					if (stoppedInstallCoroutine)
					{
						level.isUrl = true;
						level.progressText.SetActive(value: false);
						level.progress.text = "0%";
						level.progressImage.SetActive(value: false);
						stopButton.interactable = true;
						stopButtonLabel.text = RDString.Get("customLevelSelect.import.stop");
						stopButtonLabel.font = RDString.GetAppropiateFontForString(stopButtonLabel.text);
						break;
					}
					if (RDPackageInstaller.anyErrors)
					{
						AddLevelToErrorSection(level, RDPackageInstaller.errorMessage);
					}
					else
					{
						level.path = resultFilePath;
					}
				}
				level.progressText.SetActive(value: false);
			}
			if (!RDFile.Exists(level.path) && !level.isUrl)
			{
				RDBaseDllDummy.printesw("The file no longer exists at path: " + level.path);
				AddLevelToErrorSection(level, RDString.Get("customLevelSelect.import.levelPackageNotFound"));
			}
			if (!errorsImportingLevel)
			{
				RDPackageInstaller.CheckFileIsZip(level.path);
				if (RDPackageInstaller.anyErrors)
				{
					AddLevelToErrorSection(level, RDPackageInstaller.errorMessage);
				}
			}
			bool flag = false;
			string text = LevelValidation.TempLevelsFolder;
			if (!errorsImportingLevel)
			{
				try
				{
					text = Path.Combine(text, Path.GetFileNameWithoutExtension(level.path));
					text = RDUtils.GetAvailableDirectoryName(text);
					ZipUtils.Unzip(level.path, text);
					if (level.isUrl)
					{
						File.SetAttributes(level.path, FileAttributes.Normal);
						File.Delete(level.path);
					}
					flag = Path.GetExtension(level.path).Replace(".", string.Empty).ToLowerInvariant() == "rdzip";
					if (flag)
					{
						level.path = text;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("1: " + ex.Message);
					AddLevelToErrorSection(level, RDString.Get("customLevelSelect.import.unzippingError").Replace("[error]", ex.Message));
				}
			}
			string path = string.Empty;
			if (!errorsImportingLevel)
			{
				path = RDPackageInstaller.FindRDLevel(text);
				if (RDPackageInstaller.anyErrors)
				{
					AddLevelToErrorSection(level, RDPackageInstaller.errorMessage);
				}
			}
			if (!errorsImportingLevel && !flag)
			{
				string directoryName = Path.GetDirectoryName(path);
				level.path = RDPackageInstaller.MoveRDLevelFolder(directoryName, LevelValidation.CustomLevelsPath);
				if (RDPackageInstaller.anyErrors)
				{
					AddLevelToErrorSection(level, RDPackageInstaller.errorMessage);
				}
				else
				{
					string text2 = new DirectoryInfo(directoryName).Name;
					string infoText = text2 + " - " + Path.GetFileNameWithoutExtension(path) + "\n(" + level.infoText.text + ")";
					int siblingIndex = installedIS.transform.GetSiblingIndex() + 1;
					installedIS.Add(level, infoText, siblingIndex, showCheckIssues: false);
					base.cls.CLSPlaySound("sndImportInstallSuccess");
				}
			}
			CustomLevelData customLevelData = new CustomLevelData();
			RDLevelData rDLevelData = null;
			if (!errorsImportingLevel & flag)
			{
				string text3 = Path.Combine(level.path, "main.rdlevel");
				new Dictionary<string, object>();
				string text4 = string.Empty;
				if (RDFile.Exists(text3))
				{
					try
					{
						if (Json.Deserialize(RDFile.ReadAllText(text3)) is Dictionary<string, object> { Count: >1 } dictionary)
						{
							customLevelData = new CustomLevelData(isFromSteam: false);
							rDLevelData = new RDLevelData(dictionary, onlyActiveEvents: true);
							customLevelData.settings = rDLevelData.settings;
						}
						else
						{
							Debug.LogError("2: rootDict null or empty:  " + text3);
							text4 = RDString.Get("customLevelSelect.import.mainFileEmptyError");
						}
					}
					catch (Exception ex2)
					{
						Debug.LogError("3: Error desearializing json: " + text3 + "\nException: " + ex2.Message);
						text4 = RDString.Get("customLevelSelect.import.mainFileCorruptError") + " " + ex2.Message;
					}
				}
				else
				{
					Debug.LogError("4: The main.rdlevel file was not found at path:  " + text3);
					text4 = RDString.Get("customLevelSelect.import.mainFileNotFoundError");
				}
				if (!string.IsNullOrEmpty(text4))
				{
					AddLevelToErrorSection(level, text4);
					shouldDeleteFile = true;
				}
			}
			if (!errorsImportingLevel & flag)
			{
				level.customLevel = customLevelData;
				level.settings = customLevelData.settings;
				if (base.cls.levelsData.Find((CustomLevelData levelData) => levelData.Hash == level.customLevel.Hash) != null)
				{
					RDBaseDllDummy.printesw("Found same hash :  " + level.path);
					AddLevelToErrorSection(level, RDString.Get("customLevelSelect.import.sameHashError"));
					shouldDeleteFile = true;
				}
			}
			if (!errorsImportingLevel & flag)
			{
				level.errorsName = LevelValidation.GetLevelSettingsErrors(level.path, level.settings);
				if (!LevelValidation.CheckLevelForErrors(levelErrorContainer.GetErrorsToCheck(), level.errorsName))
				{
					level.errorsName = LevelValidation.GetSharedErrors(levelErrorContainer.GetErrorsToCheck(), level.errorsName);
					RDBaseDllDummy.printesw("Has settings errors - " + level.path);
					AddLevelToErrorSection(level, RDString.Get("customLevelSelect.import.checkErrors"));
					shouldDeleteFile = true;
				}
				toInstallIS.Remove(level);
			}
			if (!errorsImportingLevel & flag)
			{
				try
				{
					level.path = RDPackageInstaller.MoveRDLevelFolder(level.path, LevelValidation.CustomLevelsPath);
					string infoText2 = level.settings.levelName + "\n(" + level.infoText.text + ")";
					int siblingIndex2 = installedIS.transform.GetSiblingIndex() + 1;
					installedIS.Add(level, infoText2, siblingIndex2, showCheckIssues: false);
					base.cls.CLSPlaySound("sndImportInstallSuccess");
					if (Persistence.GetCustomLevelRank(level.customLevel.Hash) == -2)
					{
						Persistence.SetCustomLevelRank(level.customLevel.Hash, -3);
					}
				}
				catch (Exception ex3)
				{
					Debug.LogError("5: from " + level.path + " to " + LevelValidation.CustomLevelsPath + ". " + ex3.Message);
					AddLevelToErrorSection(level, RDString.Get("customLevelSelect.import.movingToCLSPathError") + " " + ex3.Message);
					shouldDeleteFile = true;
				}
			}
			if (!errorsImportingLevel)
			{
				base.cls.levelsData.Add(level.customLevel);
				string fileName = Path.GetFileName(path);
				string text5 = Path.Combine(level.path, fileName);
				string text6 = string.Empty;
				if (rDLevelData == null)
				{
					try
					{
						new Dictionary<string, object>();
						if (Json.Deserialize(RDFile.ReadAllText(text5)) is Dictionary<string, object> { Count: >1 } dictionary2)
						{
							rDLevelData = new RDLevelData(dictionary2, onlyActiveEvents: true);
						}
						else
						{
							Debug.LogError("7: rootDict null or empty:  " + text5);
							text6 = RDString.Get("customLevelSelect.import.mainFileEmptyError");
						}
					}
					catch (Exception ex4)
					{
						Debug.LogError("8: Error desearializing json: " + text5 + "\nException: " + ex4.Message);
						text6 = RDString.Get("customLevelSelect.import.mainFileCorruptError") + " " + ex4.Message;
					}
				}
				if (!string.IsNullOrEmpty(text6))
				{
					AddLevelToErrorSection(level, text6);
					shouldDeleteFile = true;
				}
				else if (rDLevelData != null)
				{
					RDLevelSettings settings = rDLevelData.settings;
					settings.firstSongFileName = rDLevelData.GetFirstSongFileName();
					string previewSongName = level.settings.previewSongName;
					settings.bpm = rDLevelData.GetSongBPM(previewSongName);
					settings.lastModifiedTime = new FileInfo(text5).LastWriteTimeUtc.ToString();
					settings.mainRDLevelRelativePath = Path.GetFileName(path);
					string text7 = new DirectoryInfo(level.path).Name;
					DesktopLevelLoader.AddOrUpdateLevelCache("Local/" + text7, settings);
				}
			}
			if (shouldDeleteFile)
			{
				try
				{
					Debug.Log("Deleting level path " + level.path);
					Directory.Delete(level.path, recursive: true);
					Debug.Log("Deleted level path successfully");
				}
				catch (Exception ex5)
				{
					Debug.LogError("9: IOException: " + ex5.Message);
				}
			}
			level.progressImage.SetActive(value: false);
			toInstallIS.Remove(level);
			yield return null;
			if (stoppedInstallCoroutine)
			{
				stopButton.interactable = true;
				stopButtonLabel.text = RDString.Get("customLevelSelect.import.stop");
				stopButtonLabel.font = RDString.GetAppropiateFontForString(stopButtonLabel.text);
				break;
			}
			UpdateInfoContentHeight();
		}
		DesktopLevelLoader.SaveCacheLevelsDict();
		try
		{
			Directory.Delete(LevelValidation.TempLevelsFolder, recursive: true);
		}
		catch (Exception ex6)
		{
			Debug.LogError("10: IOException: " + ex6.Message);
		}
		CanToggleClearButton = true;
		foreach (ImportLevel level2 in toInstallIS.levels)
		{
			level2.canToggleRemoveButton = true;
		}
		CurrentContentName = ContentName.LevelsInstalled;
		yield return null;
		infoContentVerticalScrollBar.value = 1f;
		base.cls.CLSPlaySound("sndImportInstallFinish");
		NarrateInfoSection(errorsIS);
		NarrateInfoSection(installedIS);
		Narration.Say(RDString.Get("narration.pressEnterToConfirm"), NarrationCategory.Instruction, false);
	}

	private void AddLevelToErrorSection(ImportLevel level, string subtitleToken)
	{
		level.canToggleCheckIssuesButton = true;
		errorsImportingLevel = true;
		string text = level.infoText.text;
		level.errorToken = subtitleToken;
		if (!string.IsNullOrEmpty(subtitleToken))
		{
			text = text + "\n<color=#FF0404>" + subtitleToken + "</color>";
		}
		int siblingIndex = errorsIS.transform.GetSiblingIndex() + 1;
		errorsIS.Add(level, text, siblingIndex, showCheckIssues: true);
		base.cls.CLSPlaySound("sndImportInstallError");
		Narration.Say(level.infoText.text.RemoveRichTextColorTags(), NarrationCategory.Notification, false);
	}

	public void StartOver()
	{
		errorsIS.Clear();
		installedIS.Clear();
		toInstallIS.Clear();
		CurrentContentName = ContentName.InstallLevelsDraggable;
		base.cls.PlayHammerSound();
		NarrateContext();
	}

	public void CheckLevelsIssues(ImportLevel level)
	{
		CurrentContentName = ContentName.Issues;
		issuesLevelText.text = level.infoText.text;
		issuesLevelText.font = RDString.GetAppropiateFontForString(issuesLevelText.text);
		issuesErrorText.text = RDString.Get("customLevelSelect.import.issuesWithLevel");
		if (level.errorsName == null || level.errorsName.Count <= 0)
		{
			Text text = issuesErrorText;
			text.text = text.text + "\n - " + level.errorToken;
			issuesErrorText.font = RDString.GetAppropiateFontForString(issuesErrorText.text);
			return;
		}
		LevelValidation.CheckLevelForErrors(levelErrorContainer.GetErrorsToCheck(), level.errorsName);
		SettingErrorLocalization[] showableErrors = levelErrorContainer.showableErrors;
		foreach (SettingErrorLocalization settingErrorLocalization in showableErrors)
		{
			foreach (SettingCondition error in settingErrorLocalization.errors)
			{
				if (!level.errorsName.Contains(error.name))
				{
					continue;
				}
				string value = string.Empty;
				if (error.type == LevelErrorType.FileNotFound)
				{
					if (error.name == LevelErrorName.PreviewImageFileNotFound)
					{
						value = level.settings.previewImageName;
					}
					else if (error.name == LevelErrorName.SyringeIconFileNotFound)
					{
						value = level.settings.syringeIconName;
					}
					else if (error.name == LevelErrorName.PreviewSongFileNotFound)
					{
						value = level.settings.previewSongName;
					}
				}
				Text text2 = issuesErrorText;
				text2.text = text2.text + "\n - " + RDString.Get(settingErrorLocalization.settingNameToken) + ": " + error.GetLocalized(value);
			}
		}
		issuesErrorText.font = RDString.GetAppropiateFontForString(issuesErrorText.text);
	}

	public void ToggleInsertUrlContainer_Public(bool show)
	{
		ToggleInsertUrlContainer(show, select: true, playSound: true);
	}

	public void ToggleInsertUrlContainer(bool show, bool select = true, bool playSound = false)
	{
		base.cls.infoPanel.UpdateShortcutsRow(shortcuts.ToArray());
		insertURlContainer.SetActive(show);
		if (playSound)
		{
			if (show)
			{
				base.cls.CLSPlaySound("sndImportMainButtonClick");
				NarrateURLInput();
			}
			else
			{
				base.cls.PlayHammerSound();
				NarrateContext();
			}
		}
		if (show)
		{
			if (select)
			{
				Select(urlInput);
			}
			urlInput.text = string.Empty;
			if (shortcuts.IndexOf(KeyCode.F) != -1)
			{
				shortcuts.Remove(KeyCode.F);
			}
		}
		else if (shortcuts.IndexOf(KeyCode.F) == -1)
		{
			shortcuts.Add(KeyCode.F);
		}
		if (CurrentContentName == ContentName.InstallLevelsDraggable)
		{
			draggableContainer.SetActive(!show);
			if (select)
			{
				Select(addFromUrlButton);
			}
			return;
		}
		infoScrollViewContainer.SetActive(!show);
		insertUrl.gameObject.SetActive(!show);
		browseButton.gameObject.SetActive(!show);
		installButton.gameObject.SetActive(!show);
		if (!select)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < topButtons.Length; i++)
		{
			if (topButtons[i].gameObject.activeSelf)
			{
				if (topButtons[num] == insertUrl)
				{
					CurrentTopButtonIndex = num;
					break;
				}
				num++;
			}
		}
	}

	private static void NarrateURLInput()
	{
		Narration.Say(RDString.Get("editor.PublishLevel.addURLInstructions"), NarrationCategory.Navigation, -1, -1, ElementType.Inputfield, null, null, false, NarrationActionName.ToConfirmMultiline, NarrationActionName.ToCancelEscape);
	}

	public void ValidateUrl()
	{
		string[] array = urlInput.text.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text in array)
		{
			if (!Uri.TryCreate(text, UriKind.Absolute, out var result))
			{
				RDBaseDllDummy.printesw("Couldn't create a valid absolute url from " + text);
				base.cls.CanReceiveInput = false;
				string message = RDString.Get("customLevelSelect.import.addFromURLError");
				string[] array2 = new string[1] { RDString.Get("editor.PublishLevel.dialogOK") };
				base.cls.dialog.onHideComplete = () =>
				{
					base.cls.CanReceiveInput = true;
					Select(urlInput);
					NarrateURLInput();
				};
				base.cls.dialog.Show(message, array2, (int option) =>
				{
					base.cls.dialog.Hide();
				});
				continue;
			}
			string text2 = Path.GetFileName(result.LocalPath);
			string text3 = Path.GetExtension(text2).Replace(".", string.Empty).ToLowerInvariant();
			if (string.IsNullOrEmpty(text3))
			{
				RDBaseDllDummy.printesw("The file name " + text2 + " has no extension");
				text2 = text;
			}
			else if (!ValidExtensions.Contains(text3))
			{
				RDBaseDllDummy.printesw("The extension " + text3 + " is not a valid one");
			}
			ToggleInsertUrlContainer(show: false);
			StartCoroutine(AddContent(text, text2));
			NarrateContext();
			NarrateInfoSection(toInstallIS);
		}
		urlInput.text = string.Empty;
	}

	private IEnumerator AddContent(string zipPath, string filename = null)
	{
		if (toInstallIS.levels.FindAll((ImportLevel level) => level.path == zipPath).Count == 0)
		{
			ImportLevel component = UnityEngine.Object.Instantiate(importLevelPrefab, infoScrollViewContent).GetComponent<ImportLevel>();
			string path = (component.path = Uri.UnescapeDataString(zipPath.Replace("file:", "")));
			if (string.IsNullOrEmpty(filename))
			{
				filename = Path.GetFileName(path);
			}
			else
			{
				component.isUrl = true;
			}
			toInstallIS.Add(component, filename, toInstallIS.transform.GetSiblingIndex() + 1, showCheckIssues: false);
			component.transform.SetAsLastSibling();
			CurrentContentName = ContentName.InstallLevelsList;
		}
		UpdateInfoContentHeight();
		yield return null;
		infoContentVerticalScrollBar.value = 1f;
	}

	private string MakeValidFileName(string name)
	{
		string arg = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
		string pattern = string.Format("([{0}]*\\.+$)|([{0}]+)", arg);
		return Regex.Replace(name, pattern, string.Empty);
	}

	private void UpdateInfoContentHeight()
	{
		int num = infoScrollViewContent.childCount;
		foreach (RectTransform item in infoScrollViewContent)
		{
			if (!item.gameObject.activeSelf)
			{
				num--;
			}
		}
		infoScrollViewContent.sizeDelta = new Vector2(infoScrollViewContent.sizeDelta.x, (float)num * 18f);
	}

	private void NarrateContext()
	{
		ElementType elementType = ElementType.None;
		int itemsLength = -1;
		switch (CurrentContentName)
		{
		case ContentName.InstallLevelsDraggable:
			elementType = ElementType.VerticalList;
			itemsLength = 2;
			break;
		case ContentName.InstallLevelsList:
			elementType = ElementType.HorizontalList;
			itemsLength = 3;
			break;
		case ContentName.LevelsInstalled:
			elementType = ElementType.HorizontalList;
			itemsLength = 1;
			break;
		}
		Narration.Set(contentTitleText.text, elementType, itemsLength);
		NarrateItem();
	}

	private void NarrateInfoSection(LevelImporterInfoSection liis)
	{
		if (liis.levels.Count > 0)
		{
			Narration.Say(liis.titleText.text, NarrationCategory.Notification, false);
		}
	}

	private void NarrateItem(int currentIndex = -1)
	{
		if (es.currentSelectedGameObject == null)
		{
			return;
		}
		string speech = string.Empty;
		if (es.currentSelectedGameObject.GetComponentInChildren<Text>() != null)
		{
			speech = es.currentSelectedGameObject.GetComponentInChildren<Text>().text;
		}
		else if (topButtons.Length != 0)
		{
			if (CurrentTopButton == browseButton)
			{
				speech = draggableBrowseButton.GetComponentInChildren<Text>().text;
			}
			if (CurrentTopButton == insertUrl)
			{
				speech = addFromUrlButton.GetComponentInChildren<Text>().text;
			}
		}
		Narration.Say(speech, NarrationCategory.Navigation, currentIndex, -10, ElementType.Button, null, null, false, NarrationActionName.ToSelect, NarrationActionName.ToNavigateVertical);
	}
}
