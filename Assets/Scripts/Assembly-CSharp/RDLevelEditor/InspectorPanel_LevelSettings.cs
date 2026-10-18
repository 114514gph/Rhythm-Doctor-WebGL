using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_LevelSettings : InspectorPanel
	{
		private const string OnlyOnePlayerModeToken = "editor.LevelSettings.onlyOnePlayerMode";

		private const string OnlyTwoPlayerModeToken = "editor.LevelSettings.onlyTwoPlayerMode";

		private const string BothModesToken = "editor.LevelSettings.bothModes";

		private const string NoneSelectedToken = "editor.LevelSettings.noneSelected";

		private const string MissingSelectedToken = "editor.LevelSettings.missing";

		private const string SecondsToken = "editor.LevelSettings.seconds";

		private const string previewSongTotalDurationToken = "editor.LevelSettings.previewSongTotalDuration";

		private const string addArtistKey = "editor.LevelSettings.addArtist";

		private const string addArtistToken = "[ArtistName]";

		private const float padding_y = 8f;

		private const float height = 68f;

		private const float artistPrefabHeight = 16f;

		private const float artistPopupPaddingHeight = 4f;

		[Header("Toggle Settings")]
		public GameObject general;

		public GameObject export;

		public Toggle generalToggle;

		public Toggle exportToggle;

		public float generalHeight;

		public float exportHeight;

		public bool showingGeneralFirst;

		[Header("General Settings")]
		public List<InputField> maxMistakesList;

		public List<InputField> rankDescriptionList;

		public Toggle horizontalStripsOn;

		public Toggle horizontalStripsOff;

		public Text totalHits;

		public InputField levelVolume;

		public Transform colorPaletteHolder;

		private List<ColorPicker> colorPalettePickers;

		public RectTransform artistPopup;

		public RectTransform artistContainer;

		public GameObject artistPrefab;

		public GameObject addArtist;

		public Text addArtistText;

		public ArtistUIDisclaimer artistUIDisclaimer;

		public float artistRectMaxHeight;

		public GameObject approvalLevelPrefab;

		[HideInInspector]
		public ApprovalLevelBadge approvalLevelBadge;

		private ArtistData currentArtist;

		private string levelPath = "anyRandomTempText";

		private bool showingArtistPanel;

		private bool forceCloseArtistPanel;

		[HideInInspector]
		public Action currentArtistDisclaimerAction;

		[Header("Export Settings")]
		public PreviewSongPlayer previewSongPlayer;

		public Image previewSongPlayerImage;

		public Image previewSongLoopImage;

		public RectTransform previewSongContainer;

		public GameObject browseButton;

		public GameObject previewLoopButton;

		public InputField artist;

		public InputField song;

		public InputField artistPermission;

		public InputField artistLinks;

		public InputField author;

		public InputField previewImage;

		public InputField syringeIcon;

		public InputField previewSongStartTime;

		public InputField previewSongDuration;

		public InputField description;

		public InputField tags;

		public InputField previewSongName;

		public Text previewSongMessage;

		public Slider songLabelHue;

		public Toggle songLabelGrayscale;

		public Dropdown canBePlayedOn;

		public Dropdown separate2PLevel;

		public Dropdown difficulty;

		public Dropdown specialArtistType;

		public Toggle seizureOnToggle;

		public Toggle seizureOffToggle;

		public Text songLabelHueText;

		public Text songLabelHueLabel;

		public Image songLabelHueBackground;

		public Image songLabelHueResult;

		public Text separate2PLevelErrorText;

		public RectTransform separate2PLevelContainer;

		public Text[] requiredFieldLabels;

		public Color[] hueColors = new Color[6]
		{
			Color.red,
			new Color(1f, 1f, 0f, 1f),
			Color.green,
			Color.cyan,
			Color.blue,
			Color.magenta
		};

		public Sprite hueSprite;

		public Sprite valueSprite;

		private RectTransform separate2PLevelErrorRect;

		private float separate2PLevelRectDefaultHeight;

		private List<string> separate2PLevelOptions;

		private bool hasSelectedSeparate2PLevel;

		private bool hasAddedAsterisks;

		private bool hasSelectedExportOnce;

		private bool blockSave;

		private float previewSongLength;

		private float previewSongMaxHeight;

		private float previewSongMinHeight;

		private AudioClip loadedClip;

		private LevelPlayMode[] availablePlayModes = new LevelPlayMode[3]
		{
			LevelPlayMode.OnePlayerOnly,
			LevelPlayMode.TwoPlayerOnly,
			LevelPlayMode.BothModes
		};

		private LevelDifficulty[] availableDifficulties = new LevelDifficulty[4]
		{
			LevelDifficulty.Easy,
			LevelDifficulty.Medium,
			LevelDifficulty.Tough,
			LevelDifficulty.VeryTough
		};

		private SpecialArtistType[] availableArtistTypes = new SpecialArtistType[3]
		{
			SpecialArtistType.None,
			SpecialArtistType.AuthorIsArtist,
			SpecialArtistType.PublicLicense
		};

		public EditorWebServices editorWebServices => base.editor.webServices;

		private new void Awake()
		{
			base.Awake();
			previewSongMinHeight = previewSongContainer.sizeDelta.y;
			previewSongMaxHeight = previewSongContainer.sizeDelta.y + previewSongMessage.rectTransform.sizeDelta.y;
			separate2PLevelErrorRect = separate2PLevelErrorText.GetComponent<RectTransform>();
			separate2PLevelRectDefaultHeight = separate2PLevelContainer.sizeDelta.y;
			foreach (InputField maxMistakes in maxMistakesList)
			{
				maxMistakes.onEndEdit.AddListener(delegate
				{
					Save();
				});
			}
			foreach (InputField inputField in rankDescriptionList)
			{
				inputField.onEndEdit.AddListener(delegate
				{
					Save();
				});
				inputField.onValueChanged.AddListener((string s) =>
				{
					RDEditorUtils.UpdateUIText(inputField.textComponent, s);
				});
			}
			levelVolume.onEndEdit.AddListener(delegate
			{
				Save();
			});
			levelVolume.onValueChanged.AddListener((string s) =>
			{
				RDEditorUtils.UpdateUIText(levelVolume.textComponent, s);
			});
			colorPalettePickers = new List<ColorPicker>();
			foreach (Transform item in colorPaletteHolder)
			{
				ColorPicker component = item.GetComponent<ColorPicker>();
				colorPalettePickers.Add(component);
				component.onEndEdit = (ColorPicker.EndHandler)Delegate.Combine(component.onEndEdit, (ColorPicker.EndHandler)((string t) =>
				{
					Save();
					foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
					{
						eventControl.UpdateUI();
					}
				}));
			}
			artist.onEndEdit.AddListener(delegate
			{
				Save();
			});
			artist.onValueChanged.AddListener((string s) =>
			{
				RDEditorUtils.UpdateUIText(artist.textComponent, s);
			});
			artistLinks.onEndEdit.AddListener(delegate
			{
				Save();
			});
			artistLinks.onValueChanged.AddListener((string s) =>
			{
				RDEditorUtils.UpdateUIText(artistLinks.textComponent, s);
			});
			song.onEndEdit.AddListener(delegate
			{
				Save();
			});
			song.onValueChanged.AddListener((string s) =>
			{
				RDEditorUtils.UpdateUIText(song.textComponent, s);
			});
			author.onEndEdit.AddListener(delegate
			{
				Save();
			});
			author.onValueChanged.AddListener((string s) =>
			{
				RDEditorUtils.UpdateUIText(author.textComponent, s);
			});
			description.onEndEdit.AddListener(delegate
			{
				Save();
			});
			description.onValueChanged.AddListener((string s) =>
			{
				RDEditorUtils.UpdateUIText(description.textComponent, s);
			});
			tags.onEndEdit.AddListener(delegate
			{
				Save();
			});
			tags.onValueChanged.AddListener((string s) =>
			{
				RDEditorUtils.UpdateUIText(tags.textComponent, s);
			});
			canBePlayedOn.AddOptionsWithEnumArray(availablePlayModes);
			canBePlayedOn.onValueChanged.AddListener(delegate
			{
				Save();
			});
			difficulty.AddOptionsWithEnumArray(availableDifficulties);
			difficulty.onValueChanged.AddListener(delegate
			{
				Save();
			});
			specialArtistType.AddOptionsWithEnumArray(availableArtistTypes);
			specialArtistType.onValueChanged.AddListener(delegate
			{
				Save();
			});
			separate2PLevel.onValueChanged.AddListener(delegate
			{
				Save();
				hasSelectedSeparate2PLevel = true;
			});
			songLabelHue.onValueChanged.AddListener(delegate
			{
				Save();
			});
			songLabelGrayscale.onValueChanged.AddListener(delegate
			{
				Save();
			});
			horizontalStripsOn.onValueChanged.AddListener(delegate
			{
				Save();
			});
			seizureOnToggle.onValueChanged.AddListener(delegate
			{
				Save();
			});
			previewSongName.onValueChanged.AddListener(delegate
			{
				Save();
			});
			previewSongStartTime.onEndEdit.AddListener(delegate
			{
				Save();
			});
			previewSongStartTime.onValueChanged.AddListener((string s) =>
			{
				RDEditorUtils.UpdateUIText(previewSongStartTime.textComponent, s);
				UpdatePlayPreviewSong();
			});
			previewSongDuration.onEndEdit.AddListener(delegate
			{
				Save();
			});
			previewSongDuration.onValueChanged.AddListener((string s) =>
			{
				RDEditorUtils.UpdateUIText(previewSongDuration.textComponent, s);
				UpdatePlayPreviewSong();
			});
			LocalizeDropdown(canBePlayedOn);
			LocalizeDropdown(separate2PLevel);
			LocalizeDropdown(difficulty);
			LocalizeDropdown(specialArtistType);
			Texture2D texture2D = new Texture2D(1, 7);
			for (int num = 0; num < 7; num++)
			{
				texture2D.SetPixel(0, num, hueColors[num % 6]);
			}
			texture2D.Apply();
			hueSprite = Sprite.Create(texture2D, new Rect(0f, 0.5f, 1f, 6f), new Vector2(0.5f, 0.5f));
			Texture2D texture2D2 = new Texture2D(1, 2);
			texture2D2.SetPixel(0, 0, Color.black);
			texture2D2.SetPixel(0, 1, Color.white);
			texture2D2.Apply();
			valueSprite = Sprite.Create(texture2D2, new Rect(0f, 0.5f, 1f, 1f), new Vector2(0.5f, 0.5f));
			artist.onValueChanged.AddListener((string s) =>
			{
				ToggleArtistPopup(s, artist.transform.position.y, artist);
			});
		}

		private void OnEnable()
		{
			previewSongPlayerImage.sprite = base.editor.playSprite;
		}

		private void LateUpdate()
		{
			if (hasSelectedExportOnce && !hasAddedAsterisks)
			{
				Text[] array = requiredFieldLabels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].text += " <color=#FF0000>*</color>";
				}
				hasAddedAsterisks = true;
			}
			if (general.activeSelf)
			{
				base.content.SizeDeltaY(generalHeight);
			}
			else
			{
				RectTransform rectTransform = export.transform as RectTransform;
				RectTransform rectTransform2 = rectTransform.GetChild(rectTransform.childCount - 1) as RectTransform;
				exportHeight = 0f - rectTransform2.anchoredPosition.y + rectTransform2.sizeDelta.y - rectTransform.anchoredPosition.y + 10f;
				base.content.SizeDeltaY(exportHeight);
			}
			forceCloseArtistPanel = false;
		}

		public void UpdateUI(bool changeActivePanel = true)
		{
			blockSave = true;
			if (changeActivePanel)
			{
				if (showingGeneralFirst)
				{
					generalToggle.isOn = true;
				}
				else
				{
					exportToggle.isOn = true;
					showingGeneralFirst = true;
				}
			}
			RDLevelSettings levelSettings = base.editor.levelSettings;
			for (int i = 0; i < levelSettings.rankMaxMistakes.Length; i++)
			{
				maxMistakesList[i].text = levelSettings.rankMaxMistakes[i].ToString();
			}
			for (int j = 0; j < levelSettings.rankDescription.Length; j++)
			{
				InputField inputField = rankDescriptionList[j];
				inputField.text = levelSettings.rankDescription[j];
				RDEditorUtils.UpdateUIText(inputField.textComponent, inputField.textComponent.text);
			}
			int value = Mathf.RoundToInt(levelSettings.levelVolume * 100f);
			levelVolume.text = Mathf.Clamp(value, 0, 200).ToString();
			string[] colorPalette = base.editor.colorPalette;
			for (int k = 0; k < colorPalette.Count(); k++)
			{
				colorPalettePickers[k].color = colorPalette[k];
			}
			bool flag = levelSettings.multiplayerAppearance == MultiplayerAppearance.HorizontalStrips;
			horizontalStripsOn.isOn = flag;
			horizontalStripsOff.isOn = !flag;
			bool seizureWarning = levelSettings.seizureWarning;
			seizureOnToggle.isOn = seizureWarning;
			seizureOffToggle.isOn = !seizureWarning;
			artist.text = ((!string.IsNullOrEmpty(levelSettings.artist)) ? levelSettings.artist : string.Empty);
			song.text = ((!string.IsNullOrEmpty(levelSettings.song)) ? levelSettings.song : string.Empty);
			specialArtistType.value = Array.IndexOf(availableArtistTypes, levelSettings.specialArtistType);
			artistPermission.text = ((!string.IsNullOrEmpty(levelSettings.artistPermissionFileName)) ? levelSettings.artistPermissionFileName : string.Empty);
			artistLinks.text = ((!string.IsNullOrEmpty(levelSettings.artistLinks)) ? levelSettings.artistLinks : string.Empty);
			author.text = ((!string.IsNullOrEmpty(levelSettings.author)) ? levelSettings.author : string.Empty);
			previewImage.text = ((!string.IsNullOrEmpty(levelSettings.previewImageName)) ? levelSettings.previewImageName : string.Empty);
			syringeIcon.text = ((!string.IsNullOrEmpty(levelSettings.syringeIconName)) ? levelSettings.syringeIconName : string.Empty);
			description.text = ((!string.IsNullOrEmpty(levelSettings.description)) ? levelSettings.description : string.Empty);
			tags.text = ((!string.IsNullOrEmpty(levelSettings.tags)) ? levelSettings.tags : string.Empty);
			if (!string.IsNullOrEmpty(levelSettings.previewSongName))
			{
				StartCoroutine(ShowSelectorForSong(levelSettings.previewSongName));
			}
			else
			{
				levelSettings.previewSongName = string.Empty;
				previewSongName.text = string.Empty;
				previewSongContainer.sizeDelta = new Vector2(previewSongContainer.sizeDelta.x, previewSongMinHeight);
				previewSongMessage.gameObject.SetActive(value: false);
			}
			previewSongStartTime.text = levelSettings.previewSongStartTime.ToString();
			previewSongDuration.text = levelSettings.previewSongDuration.ToString();
			canBePlayedOn.value = Array.IndexOf(availablePlayModes, levelSettings.canBePlayedOn);
			difficulty.value = Array.IndexOf(availableDifficulties, levelSettings.difficulty);
			songLabelHue.value = levelSettings.songLabelHue * 100f;
			songLabelGrayscale.isOn = levelSettings.songLabelGrayscale;
			UpdateLabelHueControls();
			if (levelSettings.canBePlayedOn == LevelPlayMode.BothModes)
			{
				hasSelectedSeparate2PLevel = false;
				separate2PLevel.interactable = true;
				LoadSeparate2PLevelOptions();
			}
			else
			{
				separate2PLevel.interactable = false;
				separate2PLevel.value = 0;
			}
			bool active = levelSettings.specialArtistType == SpecialArtistType.None;
			artistPermission.transform.parent.gameObject.SetActive(active);
			ToggleSettings();
			blockSave = false;
		}

		private void UpdateLabelHueControls()
		{
			songLabelHueLabel.text = (songLabelGrayscale.isOn ? RDString.Get("editor.LevelSettings.songLabelValue") : RDString.Get("editor.LevelSettings.songLabelHue"));
			songLabelHueText.text = songLabelHue.value + "%";
			songLabelHueResult.color = (songLabelGrayscale.isOn ? Color.HSVToRGB(0f, 0f, songLabelHue.value / 100f) : Color.HSVToRGB(songLabelHue.value / 100f, 0.6f, 0.95f));
			songLabelHueBackground.sprite = (songLabelGrayscale.isOn ? valueSprite : hueSprite);
		}

		public void Save()
		{
			if (blockSave)
			{
				return;
			}
			using (new SaveStateScope())
			{
				RDLevelSettings levelSettings = base.editor.levelSettings;
				for (int i = 0; i < levelSettings.rankMaxMistakes.Length; i++)
				{
					int result = -1;
					if (int.TryParse(maxMistakesList[i].text, out result) && result >= 0)
					{
						levelSettings.rankMaxMistakes[i] = result;
					}
					else
					{
						maxMistakesList[i].text = levelSettings.rankMaxMistakes[i].ToString();
					}
				}
				for (int j = 0; j < levelSettings.rankDescription.Length; j++)
				{
					levelSettings.rankDescription[j] = rankDescriptionList[j].text;
				}
				levelSettings.multiplayerAppearance = ((!horizontalStripsOn.isOn) ? MultiplayerAppearance.Nothing : MultiplayerAppearance.HorizontalStrips);
				levelSettings.seizureWarning = seizureOnToggle.isOn;
				if (string.IsNullOrEmpty(levelVolume.text))
				{
					levelVolume.text = "100";
				}
				int value = (int.TryParse(levelVolume.text, out value) ? value : 100);
				value = Mathf.Clamp(value, 0, 200);
				levelVolume.text = value.ToString();
				levelSettings.levelVolume = Mathf.Clamp((float)value / 100f, 0f, 2f);
				for (int k = 0; k < colorPalettePickers.Count; k++)
				{
					base.editor.colorPalette[k] = colorPalettePickers[k].color;
				}
				levelSettings.author = author.text;
				levelSettings.artist = artist.text;
				levelSettings.song = song.text;
				levelSettings.artistPermissionFileName = artistPermission.text;
				levelSettings.artistLinks = artistLinks.text;
				levelSettings.previewImageName = previewImage.text;
				levelSettings.syringeIconName = syringeIcon.text;
				levelSettings.description = description.text;
				levelSettings.tags = tags.text;
				levelSettings.songLabelHue = songLabelHue.value / 100f;
				levelSettings.songLabelGrayscale = songLabelGrayscale.isOn;
				UpdateLabelHueControls();
				if (string.IsNullOrEmpty(previewSongName.text))
				{
					levelSettings.previewSongName = string.Empty;
					previewSongStartTime.interactable = false;
					previewSongDuration.interactable = false;
					previewSongStartTime.text = 0f.ToString();
					previewSongDuration.text = 10f.ToString();
				}
				else
				{
					levelSettings.previewSongName = previewSongName.text;
					previewSongStartTime.interactable = true;
					previewSongDuration.interactable = true;
					float result2 = 0f;
					if (string.IsNullOrEmpty(previewSongStartTime.text) || !float.TryParse(previewSongStartTime.text, out result2) || result2 < 0f || result2 > previewSongLength - 1f)
					{
						result2 = 0f;
						previewSongStartTime.text = 0f.ToString();
						levelSettings.previewSongStartTime = 0f;
					}
					else
					{
						levelSettings.previewSongStartTime = result2;
					}
					float num = ((string.IsNullOrEmpty(previewSongDuration.text) || !float.TryParse(previewSongDuration.text, out var result3) || result3 < 1f || result3 > 20f) ? 10f : result3);
					float num2 = ((num < previewSongLength - result2) ? num : (previewSongLength - result2));
					previewSongDuration.text = num2.ToString();
					levelSettings.previewSongDuration = num2;
				}
				levelSettings.difficulty = availableDifficulties[difficulty.value];
				levelSettings.specialArtistType = availableArtistTypes[specialArtistType.value];
				levelSettings.canBePlayedOn = availablePlayModes[canBePlayedOn.value];
				if (levelSettings.canBePlayedOn != LevelPlayMode.BothModes)
				{
					levelSettings.separate2PLevelFilename = string.Empty;
					separate2PLevel.value = 0;
					separate2PLevel.interactable = false;
					separate2PLevelErrorText.gameObject.SetActive(value: false);
					separate2PLevelContainer.sizeDelta = new Vector2(separate2PLevelContainer.sizeDelta.x, separate2PLevelRectDefaultHeight);
				}
				else
				{
					separate2PLevel.interactable = true;
					levelSettings.separate2PLevelFilename = string.Empty;
					if (separate2PLevel.options.Count > 0)
					{
						string directoryName = Path.GetDirectoryName(base.editor.openedFilePath);
						string text = separate2PLevel.options[separate2PLevel.value].text;
						string path = Path.Combine(directoryName, text);
						if (separate2PLevel.value == 0 || !RDFile.Exists(path))
						{
							separate2PLevelErrorText.gameObject.SetActive(value: false);
							separate2PLevelContainer.sizeDelta = new Vector2(separate2PLevelContainer.sizeDelta.x, separate2PLevelRectDefaultHeight);
						}
						else
						{
							levelSettings.separate2PLevelFilename = text;
						}
					}
				}
				bool active = levelSettings.specialArtistType == SpecialArtistType.None;
				artistPermission.transform.parent.gameObject.SetActive(active);
				base.editor.levelSettings = levelSettings;
				if (levelSettings.canBePlayedOn == LevelPlayMode.BothModes)
				{
					LoadSeparate2PLevelOptions();
				}
			}
		}

		public void ToggleSettings()
		{
			general.SetActive(generalToggle.isOn);
			export.SetActive(exportToggle.isOn);
			if (export.activeSelf)
			{
				hasSelectedExportOnce = true;
			}
			if (!generalToggle.isOn)
			{
				return;
			}
			bool[] array = new bool[21];
			List<LevelEvent_Base> list = new List<LevelEvent_Base>();
			foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
			{
				list.Add(eventControl.levelEvent);
			}
			base.game.currentLevel.CalculateCrotchetsInEachBar(list);
			foreach (LevelEvent_Base item in list)
			{
				if (item.active && item != null)
				{
					Tab tab = item.tab;
					if (tab == Tab.Actions || tab == Tab.Rooms || tab == Tab.Decorations || tab == Tab.Windows)
					{
						item.GetPaletteColorsUsed(array);
					}
				}
			}
			int num = base.game.currentLevel.CountHits(list);
			totalHits.text = RDString.Get("editor.LevelSettings.totalHits") + " " + num;
			totalHits.font = RDString.GetAppropiateFontForString(totalHits.text);
			for (int i = 0; i < 21; i++)
			{
				colorPalettePickers[i].star.gameObject.SetActive(array[i]);
			}
		}

		public void ShowSelectorForImage(int type)
		{
			int num = -1;
			InputField inputField;
			switch (type)
			{
			case 0:
				num = 2000000;
				inputField = previewImage;
				break;
			case 1:
				num = 20000;
				inputField = syringeIcon;
				break;
			default:
				inputField = artistPermission;
				break;
			}
			string text = RDEditorUtils.ShowFileSelectorForImage(RDString.Get("editor.dialog.selectImage"), num);
			if (!string.IsNullOrEmpty(text))
			{
				string directoryName = Path.GetDirectoryName(base.editor.openedFilePath);
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				printe("image size: " + new FileInfo(directoryName + directorySeparatorChar + text).Length + " bytes");
				inputField.text = text;
				Save();
			}
		}

		public void LoadSeparate2PLevelOptions()
		{
			blockSave = true;
			separate2PLevel.ClearOptions();
			separate2PLevelOptions = new List<string>();
			separate2PLevelOptions.Add(RDString.Get("editor.LevelSettings.noneSelected"));
			bool flag = false;
			int value = 0;
			if (!string.IsNullOrEmpty(base.editor.openedFilePath))
			{
				string directoryName = Path.GetDirectoryName(base.editor.openedFilePath);
				string fileName = Path.GetFileName(base.editor.openedFilePath);
				string[] filesWithExtension = directoryName.GetFilesWithExtension("rdlevel", SearchOption.TopDirectoryOnly);
				string text = "backup.rdlevel";
				string[] array = filesWithExtension;
				for (int i = 0; i < array.Length; i++)
				{
					string fileName2 = Path.GetFileName(array[i]);
					if (fileName2 != text && fileName2 != fileName)
					{
						separate2PLevelOptions.Add(fileName2);
					}
					if (fileName2 == base.editor.levelSettings.separate2PLevelFilename)
					{
						flag = true;
						value = separate2PLevelOptions.Count - 1;
					}
				}
				if (!string.IsNullOrEmpty(base.editor.levelSettings.separate2PLevelFilename) && !flag)
				{
					separate2PLevelOptions.Add(base.editor.levelSettings.separate2PLevelFilename + " " + RDString.Get("editor.LevelSettings.missing"));
				}
			}
			separate2PLevel.AddOptions(separate2PLevelOptions);
			if (!hasSelectedSeparate2PLevel && !flag && !string.IsNullOrEmpty(base.editor.levelSettings.separate2PLevelFilename))
			{
				separate2PLevel.value = separate2PLevel.options.Count - 1;
			}
			if (flag)
			{
				separate2PLevel.value = value;
				float num = separate2PLevelRectDefaultHeight;
				if (LevelValidation.GetSeparate2PLevelErrors(Path.GetDirectoryName(base.editor.openedFilePath), base.editor.levelSettings.separate2PLevelFilename) != LevelErrorName.None)
				{
					separate2PLevelErrorText.gameObject.SetActive(value: true);
					num += separate2PLevelErrorRect.sizeDelta.y;
				}
				else
				{
					separate2PLevelErrorText.gameObject.SetActive(value: false);
				}
				separate2PLevelContainer.sizeDelta = new Vector2(separate2PLevelContainer.sizeDelta.x, num);
			}
			blockSave = false;
		}

		public void BrowsePreviewSong()
		{
			previewSongPlayerImage.sprite = base.editor.playSprite;
			previewSongPlayer.Stop();
			string text = RDEditorUtils.ShowFileSelectorForAudio(RDString.Get("editor.dialog.selectSound"), -1L);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (Path.GetExtension(text).ToLowerInvariant() != ".ogg")
			{
				string text2 = Path.ChangeExtension(text, "ogg");
				if (File.Exists(Path.Combine(Path.GetDirectoryName(base.editor.openedFilePath), text2)))
				{
					StartCoroutine(ShowSelectorForSong(text2));
					return;
				}
			}
			StartCoroutine(ShowSelectorForSong(text));
		}

		private IEnumerator ShowSelectorForSong(string audioFilename)
		{
			blockSave = true;
			previewSongContainer.sizeDelta = new Vector2(previewSongContainer.sizeDelta.x, previewSongMaxHeight);
			previewSongMessage.gameObject.SetActive(value: true);
			CoroutineWithData loadAudio = new CoroutineWithData(this, RDEditorUtils.AudioClipFromFilename(audioFilename));
			yield return loadAudio.coroutine;
			RDAudioLoadResult rDAudioLoadResult = (RDAudioLoadResult)loadAudio.result;
			blockSave = true;
			if (rDAudioLoadResult.type == RDAudioLoadType.SuccessExternalClipLoaded)
			{
				previewSongLength = rDAudioLoadResult.clip.length;
				previewSongMessage.text = RDString.Get("editor.LevelSettings.previewSongTotalDuration") + ": " + previewSongLength + RDString.Get("editor.LevelSettings.seconds");
				previewSongName.text = audioFilename;
				loadedClip = rDAudioLoadResult.clip;
				browseButton.SetActive(value: true);
				previewLoopButton.SetActive(value: true);
			}
			else
			{
				previewSongMessage.text = RDString.GetEnumValue(rDAudioLoadResult.type);
				browseButton.SetActive(value: false);
				previewLoopButton.SetActive(value: false);
			}
			blockSave = false;
			Save();
		}

		public void TogglePlayPreviewSong()
		{
			if (loadedClip == null || !float.TryParse(previewSongStartTime.text, out var result) || !float.TryParse(previewSongDuration.text, out var result2))
			{
				return;
			}
			if (previewSongPlayer.Playing)
			{
				previewSongPlayerImage.sprite = base.editor.playSprite;
				previewSongPlayer.Stop();
				return;
			}
			previewSongPlayerImage.sprite = base.editor.pauseSprite;
			previewSongPlayer.Play(loadedClip, result, result2);
			if (base.editor.isPlaying)
			{
				base.editor.PlayPauseButtonClick();
			}
		}

		public void SeekPlayPreviewSongLoop()
		{
			if (!previewSongPlayer.Playing)
			{
				TogglePlayPreviewSong();
			}
			previewSongPlayer.SeekToEnd(-1.5f);
		}

		private void UpdatePlayPreviewSong()
		{
			if (!(loadedClip == null) && float.TryParse(previewSongStartTime.text, out var result) && float.TryParse(previewSongDuration.text, out var result2) && previewSongPlayer.Playing && result >= 0f && result <= loadedClip.length - 1f && result2 >= 1f && result2 <= 20f)
			{
				previewSongPlayer.Play(loadedClip, result, result2);
			}
		}

		public void ToggleArtistPopup(string search, float yPos, InputField artistField)
		{
			if (EditorWebServices.artists == null)
			{
				return;
			}
			bool flag = levelPath != base.editor.openedFilePath;
			levelPath = base.editor.openedFilePath;
			if (approvalLevelBadge == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(approvalLevelPrefab, artist.transform);
				approvalLevelBadge = gameObject.GetComponent<ApprovalLevelBadge>();
				approvalLevelBadge.rectTransform.AnchorPosX(-3f);
				approvalLevelBadge.UpdateUI(ApprovalLevel.Pending, onlyColor: true);
			}
			base.editor.levelSettings.artist = search;
			string text = search.Trim();
			if (text == "")
			{
				artistPopup.gameObject.SetActive(value: false);
				approvalLevelBadge.UpdateUI(ApprovalLevel.Pending, onlyColor: true);
				return;
			}
			search = text.ToLower();
			List<ArtistData> list = (from item in EditorWebServices.artists.AsParallel()
				where item.approvalLevel != ApprovalLevel.Pending && item.name.ToLower().Contains(search)
				select item).ToList();
			foreach (Transform item in artistContainer)
			{
				if (item.gameObject != addArtist)
				{
					UnityEngine.Object.Destroy(item.gameObject);
				}
			}
			int num = 0;
			currentArtist = null;
			addArtist.SetActive(value: false);
			currentArtistDisclaimerAction = null;
			if (list != null && list.Count > 0)
			{
				bool flag2 = false;
				foreach (ArtistData artistData in list)
				{
					if (!flag2 && artistData.name.ToLower() == search)
					{
						approvalLevelBadge.UpdateUI(artistData.approvalLevel, onlyColor: true);
						currentArtist = artistData;
						flag2 = true;
						currentArtistDisclaimerAction = () =>
						{
							artistUIDisclaimer.SetData(artistData, artistField, onlyCheckingMode: true);
						};
					}
					if (!flag)
					{
						ArtistUISettings component = UnityEngine.Object.Instantiate(artistPrefab, artistContainer).GetComponent<ArtistUISettings>();
						component.SetData(artistData.name, artistData.approvalLevel);
						component.button.onClick.AddListener(() =>
						{
							HideArtistDropdown();
							artistUIDisclaimer.SetData(artistData, artist, onlyCheckingMode: false);
						});
					}
				}
				num = list.Count;
				if (!flag2)
				{
					num++;
					approvalLevelBadge.UpdateUI(ApprovalLevel.Pending, onlyColor: true);
				}
				if (!flag)
				{
					addArtist.SetActive(!flag2);
				}
			}
			else if (!flag)
			{
				addArtist.SetActive(value: true);
				num++;
			}
			if (flag || forceCloseArtistPanel)
			{
				return;
			}
			if (addArtist.activeSelf)
			{
				addArtistText.text = RDString.Get("editor.LevelSettings.addArtist").Replace("[ArtistName]", text);
				addArtistText.font = RDString.GetAppropiateFontForString(addArtistText.text);
				addArtist.transform.SetAsLastSibling();
			}
			float num2 = (float)num * 16f + 4f;
			artistPopup.position = artistPopup.position.WithY(yPos);
			artistPopup.anchoredPosition = artistPopup.anchoredPosition.WithY(artistPopup.anchoredPosition.y - 17f);
			artistContainer.sizeDelta = new Vector2(artistContainer.sizeDelta.x, num2);
			float y = Mathf.Min(num2, artistRectMaxHeight);
			artistPopup.sizeDelta = new Vector2(artistPopup.sizeDelta.x, y);
			artistPopup.gameObject.SetActive(value: true);
			base.editor.ShowPopupBlocker(() =>
			{
				if (!artistUIDisclaimer.gameObject.activeSelf)
				{
					if (currentArtist != null)
					{
						artistUIDisclaimer.SetData(currentArtist, artist, onlyCheckingMode: false);
					}
					else
					{
						artistUIDisclaimer.gameObject.SetActive(value: false);
						HideArtistDropdown();
					}
				}
			});
		}

		public void HideArtistDropdown()
		{
			artistPopup.gameObject.SetActive(value: false);
			base.editor.popupBlocker.gameObject.SetActive(value: false);
			forceCloseArtistPanel = true;
		}

		public void NewArtist()
		{
			HideArtistDropdown();
		}
	}
}
