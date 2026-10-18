#if !UNITY_WEBGL
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DG.Tweening;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityFileDialog;

namespace RDLevelEditor
{
	public class RDPublishPopup : RDEditorBase
	{
		private const string FileToken = "[filename]";

		private const string ArtistToken = "[artist]";

		public GameObject levelDataContainer;

		public GameObject steamUpdateContainer;

		public Text levelName;

		public Text previewSongName;

		public Text description;

		public Text tags;

		public Text levelMode;

		public Button rdzipButton;

		public Button steamPublishButton;

		public Button steamUpdateExistingButton;

		public Button steamUpdateSelectedButton;

		public Button editButton;

		public Button convertButton;

		public Button backButton;

		public Button forcePublishButton;

		public Button changePublishedItemPageNumberLeft;

		public Button changePublishedItemPageNumberRight;

		public InspectorPanel_LevelSettings levelSettings;

		public Text incompleteSettingsText;

		public RawImage previewImage;

		public GameObject loadingScreen;

		public Image onDialogScreenBlocker;

		public Text progressText;

		public LevelErrorPresentation levelErrorPresentation;

		public GameObject publishedItemPrefab;

		public RectTransform publishedItemsContainer;

		public Text selectedPublishedLevelText;

		public RawImage selectedPublishedLevelImage;

		public GameObject selectedPublishedLevelImageContainer;

		public RectTransform publishedLevelsScrollViewContainer;

		public Text currentPageText;

		private int levelToUpdateIndex;

		private int currentPublishedItemsPageNumber;

		private int maxPublishedItemsPageNumber;

		private List<Text> publishedLevels;

		private RectTransform levelModeRect;

		private RectTransform rectTransform;

		private RDLevelSettings LevelSettings => base.editor.levelSettings;

		private bool Separate2PLevelFileIsMissing
		{
			get
			{
				if (LevelSettings.canBePlayedOn == LevelPlayMode.BothModes && !string.IsNullOrEmpty(LevelSettings.separate2PLevelFilename))
				{
					string text = Path.Combine(Path.GetDirectoryName(base.editor.openedFilePath), LevelSettings.separate2PLevelFilename);
					if (!string.IsNullOrEmpty(text) && !RDFile.Exists(text))
					{
						return true;
					}
				}
				return false;
			}
		}

		private bool ArtistHasDeclined => base.editor.ApprovalLevelForArtist(LevelSettings.artist) == ApprovalLevel.Declined;

		private bool ArtistProofIsMissing
		{
			get
			{
				ApprovalLevel approvalLevel = base.editor.ApprovalLevelForArtist(LevelSettings.artist);
				if (LevelSettings.artistPermissionFileName == "" && approvalLevel == ApprovalLevel.Pending)
				{
					return LevelSettings.specialArtistType == SpecialArtistType.None;
				}
				return false;
			}
		}

		private void Awake()
		{
			levelModeRect = levelMode.GetComponent<RectTransform>();
			rectTransform = GetComponent<RectTransform>();
			publishedLevels = new List<Text>();
		}

		private void Update()
		{
			if (SteamIntegration.initialized)
			{
				SteamIntegration.CheckCallbacks();
				SteamWorkshop.CheckUploadInfo();
			}
		}

		public void ShowLevelSettings()
		{
			levelSettings.showingGeneralFirst = false;
			Hide();
			base.editor.LevelSettingsClick();
		}

		public void FindNonOGGSong()
		{
			Hide();
			foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
			{
				if (eventControl.levelEvent is LevelEvent_PlaySong levelEvent_PlaySong && levelEvent_PlaySong.RequiresConversion())
				{
					levelEvent_PlaySong.forceOGGConvert = true;
					base.editor.DeselectAllEventControls();
					base.editor.SelectEventControl(eventControl);
					base.gameObject.SetActive(value: false);
					break;
				}
			}
		}

		public void ToggleLoadingScreen(bool show)
		{
			loadingScreen.SetActive(show);
			if (show)
			{
				progressText.text = RDString.Get("editor.loading");
				progressText.font = RDString.GetAppropiateFontForString(progressText.text);
			}
		}

		public void ToggleSteamUpdate(bool show)
		{
			levelToUpdateIndex = -1;
			if (!show)
			{
				ToggleSteamUpdateScreen(show: false);
				return;
			}
			currentPublishedItemsPageNumber = 1;
			ToggleLoadingScreen(show: true);
			StartCoroutine(LoadPublishedItemsPageNumber(currentPublishedItemsPageNumber));
		}

		private void ToggleSteamUpdateScreen(bool show)
		{
			steamUpdateContainer.SetActive(show);
			backButton.gameObject.SetActive(show);
			steamUpdateSelectedButton.gameObject.SetActive(show);
			steamUpdateExistingButton.gameObject.SetActive(!show);
			levelDataContainer.SetActive(!show);
			rdzipButton.gameObject.SetActive(!show);
			steamPublishButton.gameObject.SetActive(!show);
		}

		public void ChangePublishedItemsPageNumber(int direction)
		{
			currentPublishedItemsPageNumber += direction;
			ToggleLoadingScreen(show: true);
			StartCoroutine(LoadPublishedItemsPageNumber(currentPublishedItemsPageNumber));
		}

		private IEnumerator LoadPublishedItemsPageNumber(int pageNumber)
		{
			yield return SteamWorkshop.GetPublishedItems(pageNumber, (int levelToLoadIndex) =>
			{
				int num4 = ((SteamWorkshop.totalPublishedItems > 50) ? 50 : SteamWorkshop.totalPublishedItems);
				progressText.text = string.Format("{0} ({1}/{2})", RDString.Get("editor.PublishLevel.loadingPublishedLevels"), levelToLoadIndex + 1, num4);
			});
			if (!SteamWorkshop.OperationSucceeded)
			{
				ToggleLoadingScreen(show: false);
				string message = RDString.Get("editor.PublishLevel." + SteamWorkshop.errors[0]);
				base.editor.dialog.Show(message, new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
				{
					base.editor.dialog.Hide();
				});
				yield break;
			}
			if (SteamWorkshop.totalPublishedItems == 0)
			{
				ToggleLoadingScreen(show: false);
				string message2 = RDString.Get("editor.PublishLevel.noPublishedLevels");
				string[] options = new string[1] { RDString.Get("editor.PublishLevel.dialogOK") };
				base.editor.dialog.Show(message2, options, (int option) =>
				{
					base.editor.dialog.Hide();
				});
				yield break;
			}
			try
			{
				changePublishedItemPageNumberLeft.gameObject.SetActive(SteamWorkshop.totalPublishedItems > 50 && currentPublishedItemsPageNumber != 1);
				changePublishedItemPageNumberRight.gameObject.SetActive(SteamWorkshop.totalPublishedItems > 50 && SteamWorkshop.totalPublishedItems > 50 * pageNumber);
				int num = (((float)(SteamWorkshop.totalPublishedItems % 50) > 0f) ? 1 : 0);
				currentPageText.text = $"{pageNumber}/{SteamWorkshop.totalPublishedItems / 50 + num}";
				ToggleSteamUpdateScreen(show: true);
				int num2 = 0;
				foreach (SteamWorkshop.ResultItem resultItem in SteamWorkshop.resultItems)
				{
					_ = resultItem;
					if (num2 >= publishedLevels.Count)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(publishedItemPrefab, publishedItemsContainer);
						publishedLevels.Add(gameObject.GetComponentInChildren<Text>());
					}
					else
					{
						publishedLevels[num2].transform.parent.gameObject.SetActive(value: true);
					}
					Text text = publishedLevels[num2];
					text.text = SteamWorkshop.resultItems[num2].title;
					text.font = RDString.GetAppropiateFontForString(text.text);
					Button componentInParent = text.GetComponentInParent<Button>();
					int levelIndex = num2;
					componentInParent.onClick.AddListener(() =>
					{
						ShowPublicSteamLevel(levelIndex);
					});
					num2++;
				}
				float y = publishedItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
				float spacing = publishedLevelsScrollViewContainer.GetComponent<VerticalLayoutGroup>().spacing;
				publishedLevelsScrollViewContainer.sizeDelta = new Vector2(publishedLevelsScrollViewContainer.sizeDelta.x, (float)num2 * (y + spacing));
				for (int num3 = num2; num3 < publishedLevels.Count; num3++)
				{
					publishedLevels[num3].transform.parent.gameObject.SetActive(value: false);
				}
				ShowPublicSteamLevel(0);
				EventSystem.current.SetSelectedGameObject(publishedLevels[0].transform.parent.gameObject);
			}
			catch (Exception ex)
			{
				Debug.Log("Error loading levels to update " + ex.Message);
				ToggleLoadingScreen(show: false);
				string message3 = RDString.Get("editor.PublishLevel." + SteamWorkshop.errors[0]);
				base.editor.dialog.Show(message3, new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
				{
					base.editor.dialog.Hide();
				});
			}
		}

		public void ShowPublicSteamLevel(int index)
		{
			levelToUpdateIndex = index;
			selectedPublishedLevelText.text = SteamWorkshop.resultItems[index].title;
			selectedPublishedLevelText.font = RDString.GetAppropiateFontForString(selectedPublishedLevelText.text);
			byte[] data = RDFile.ReadAllBytes(SteamWorkshop.resultItems[index].previewImagePath);
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(data);
			selectedPublishedLevelImage.texture = texture2D;
			Vector2 imageFixedSize = LevelValidation.GetImageFixedSize(previewImage.texture, LevelValidation.PreviewImageRecommendedDimensions);
			imageFixedSize *= 2f;
			selectedPublishedLevelImage.GetComponent<RectTransform>().sizeDelta = imageFixedSize;
			selectedPublishedLevelImageContainer.SetActive(value: true);
			ToggleLoadingScreen(show: false);
		}

		public void Show()
		{
			levelToUpdateIndex = -1;
			TogglePublishPopup(show: true);
			base.editor.popupBackground.SetActive(value: true);
			LevelEditorPlaySound("sndEditorPopupLarge", "LevelEditorInspectorPanel");
			SteamIntegration.OpenConnection();
			if (SteamIntegration.initialized)
			{
				SteamWorkshop.Initialize();
			}
			List<LevelErrorName> levelSettingsErrors = LevelValidation.GetLevelSettingsErrors(Path.GetDirectoryName(base.editor.openedFilePath), LevelSettings);
			List<LevelEvent_Base> list = new List<LevelEvent_Base>();
			foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
			{
				list.Add(eventControl.levelEvent);
			}
			levelSettingsErrors.AddRange(LevelValidation.GetLevelEventsErrors(list));
			List<LevelErrorName> errorsToCheck = levelErrorPresentation.errorContainer.GetErrorsToCheck();
			List<LevelErrorName> list2 = levelSettingsErrors.Intersect(errorsToCheck).ToList();
			bool flag = list2.Count > 0;
			bool active = list2.Contains(LevelErrorName.PlaySongNotOGG);
			bool active2 = list2.Any((LevelErrorName x) => x != LevelErrorName.PlaySongNotOGG);
			levelErrorPresentation.gameObject.SetActive(flag);
			editButton.gameObject.SetActive(active2);
			convertButton.gameObject.SetActive(active);
			forcePublishButton.gameObject.SetActive(flag && !string.IsNullOrEmpty(base.editor.openedFilePath));
			incompleteSettingsText.gameObject.SetActive(flag);
			levelDataContainer.SetActive(!flag);
			rdzipButton.gameObject.SetActive(!flag);
			steamPublishButton.gameObject.SetActive(SteamIntegration.initialized && !flag);
			steamUpdateExistingButton.gameObject.SetActive(SteamIntegration.initialized && !flag);
			bool artistHasDeclined = ArtistHasDeclined;
			steamPublishButton.interactable = !artistHasDeclined;
			steamUpdateExistingButton.interactable = !artistHasDeclined;
			steamUpdateSelectedButton.gameObject.SetActive(value: false);
			backButton.gameObject.SetActive(value: false);
			steamUpdateContainer.SetActive(value: false);
			if (flag)
			{
				incompleteSettingsText.text = RDString.Get("editor.PublishLevel.incompleteSettings") + "\n";
				incompleteSettingsText.font = RDString.GetAppropiateFontForString(incompleteSettingsText.text);
				levelErrorPresentation.ShowErrors(list2, LevelSettings);
				rdzipButton.gameObject.SetActive(value: false);
				return;
			}
			levelName.text = LevelSettings.artist + " - " + LevelSettings.song + "\n<color=#888>" + RDString.Get("editor.PublishLevel.byLabel") + "</color> " + LevelSettings.author;
			levelName.font = RDString.GetAppropiateFontForString(levelName.text);
			TimeSpan timeSpan = TimeSpan.FromSeconds(LevelSettings.previewSongStartTime);
			string text = $"{timeSpan.Hours * 60 + timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
			timeSpan = TimeSpan.FromSeconds(LevelSettings.previewSongStartTime + LevelSettings.previewSongDuration);
			string text2 = $"{timeSpan.Hours * 60 + timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
			string text3 = "<color=#888>" + RDString.Get("editor.LevelSettings.previewSong") + "</color>";
			string text4 = LevelSettings.previewSongName + " (" + text + " - " + text2 + ")";
			previewSongName.text = text3 + "\n" + text4;
			previewSongName.font = RDString.GetAppropiateFontForString(previewSongName.text);
			previewImage.texture = RDEditorUtils.GetCustomTextureWithName(LevelSettings.previewImageName);
			Vector2 imageFixedSize = LevelValidation.GetImageFixedSize(previewImage.texture, LevelValidation.PreviewImageRecommendedDimensions);
			previewImage.GetComponent<RectTransform>().sizeDelta = imageFixedSize;
			string text5 = "<color=#888>" + RDString.Get("editor.LevelSettings.description") + "</color>";
			string text6 = (string.IsNullOrEmpty(LevelSettings.description) ? RDString.Get("editor.PublishLevel.none") : LevelSettings.description);
			description.text = text5 + "\n" + text6;
			description.font = RDString.GetAppropiateFontForString(description.text);
			string text7 = "<color=#888>" + RDString.Get("editor.LevelSettings.tags") + ":</color> ";
			string text8 = (string.IsNullOrEmpty(LevelSettings.tags) ? RDString.Get("editor.PublishLevel.none") : LevelSettings.tags);
			tags.text = text7 + text8;
			tags.font = RDString.GetAppropiateFontForString(tags.text);
			levelModeRect.sizeDelta = new Vector2(122f, levelModeRect.sizeDelta.y);
			if (LevelSettings.canBePlayedOn == LevelPlayMode.BothModes)
			{
				if (!string.IsNullOrEmpty(LevelSettings.separate2PLevelFilename))
				{
					levelMode.text = RDString.Get("editor.PublishLevel.1and2pSeparateLevel") + " " + LevelSettings.separate2PLevelFilename;
					levelModeRect.sizeDelta = new Vector2(280f, levelModeRect.sizeDelta.y);
					if (Separate2PLevelFileIsMissing)
					{
						Text text9 = levelMode;
						text9.text = text9.text + "<color=#FFC60B>" + RDString.Get("editor.LevelSettings.missing") + "</color>";
					}
				}
				else
				{
					levelMode.text = RDString.Get("editor.PublishLevel.BothLevels");
				}
			}
			else if (LevelSettings.canBePlayedOn == LevelPlayMode.TwoPlayerOnly)
			{
				levelMode.text = RDString.Get("editor.PublishLevel.2pOnlyLevel");
			}
			else
			{
				levelMode.text = RDString.Get("editor.PublishLevel.1pOnlyLevel");
			}
			levelMode.font = RDString.GetAppropiateFontForString(levelMode.text);
		}

		public void Hide()
		{
			base.editor.popupBackground.SetActive(value: false);
			if (base.gameObject.activeSelf)
			{
				TogglePublishPopup(show: false);
				LevelEditorPlaySound("sndEditorPopdownLarge", "LevelEditorInspectorPanel");
			}
		}

		private void TogglePublishPopup(bool show)
		{
			if (show)
			{
				base.gameObject.SetActive(value: true);
			}
			Vector3 localScale = (show ? Vector3.zero : Vector3.one);
			Vector3 endValue = (show ? Vector3.one : Vector3.zero);
			float duration = (show ? base.gc.roomsSelectionPopup_showAnimDuration : base.gc.roomsSelectionPopup_hideAnimDuration);
			Ease ease = (show ? base.gc.roomsSelectionPopup_showAnimEase : base.gc.roomsSelectionPopup_hideAnimEase);
			rectTransform.DOKill(complete: true);
			rectTransform.localScale = localScale;
			rectTransform.DOScale(endValue, duration).SetEase(ease).SetUpdate(UpdateType.Normal, isIndependentUpdate: true)
				.OnComplete(() =>
				{
					if (!show)
					{
						base.gameObject.SetActive(value: false);
					}
				});
		}

		public void ForcePublish()
		{
			base.editor.dialog.Show(RDString.Get("editor.ConfirmForceExport"), new string[2]
			{
				RDString.Get("editor.PublishLevel.dialogCancel"),
				RDString.Get("editor.PublishLevel.dialogOK")
			}, (int option) =>
			{
				base.editor.dialog.Hide(() =>
				{
					if (option == 1)
					{
						PrepareToPublish(publishingToSteam: false);
					}
				});
			});
		}

		public void PrepareToPublish(bool publishingToSteam)
		{
			if (Separate2PLevelFileIsMissing)
			{
				onDialogScreenBlocker.raycastTarget = true;
				string text = RDString.Get("editor.PublishLevel.ZipWithMissingFile");
				text = text.Replace("[filename]", LevelSettings.separate2PLevelFilename);
				string[] options = new string[2]
				{
					RDString.Get("editor.PublishLevel.dialogCancel"),
					RDString.Get("editor.PublishLevel.PublishAs1POnly")
				};
				base.editor.dialog.Show(text, options, (int option) =>
				{
					base.editor.dialog.Hide(() =>
					{
						if (option == 1)
						{
							if (publishingToSteam)
							{
								ConfirmPublish(missing2PSeparateLevel: true);
							}
							else
							{
								BrowsePublishedRDZipPath(missing2PSeparateLevel: true);
							}
						}
						onDialogScreenBlocker.raycastTarget = false;
					});
				});
			}
			else if (publishingToSteam)
			{
				ConfirmPublish(missing2PSeparateLevel: false);
			}
			else
			{
				BrowsePublishedRDZipPath(missing2PSeparateLevel: false);
			}
			void ConfirmPublish(bool missing2PSeparateLevel)
			{
				if (ArtistProofIsMissing)
				{
					onDialogScreenBlocker.raycastTarget = true;
					string text2 = RDString.Get("editor.ArtistDisclaimer.artistNewRequirement");
					text2 = text2.Replace("[artist]", LevelSettings.artist);
					string[] options2 = new string[1] { RDString.Get("editor.PublishLevel.dialogOK") };
					base.editor.dialog.Show(text2, options2, (int option) =>
					{
						base.editor.dialog.Hide(() =>
						{
							onDialogScreenBlocker.raycastTarget = false;
						});
					});
				}
				else
				{
					ConfirmPublishToSteam(missing2PSeparateLevel);
				}
			}
		}

		private void ConfirmPublishToSteam(bool missing2PSeparateLevel)
		{
			base.editor.dialog.Show(RDString.Get("editor.PublishLevel.steamConfirmationDialog"), new string[2]
			{
				RDString.Get("editor.PublishLevel.dialogCancel"),
				RDString.Get("editor.PublishLevel.dialogUpload")
			}, (int option) =>
			{
				base.editor.dialog.Hide(() =>
				{
					if (option == 1 && SteamIntegration.initialized)
					{
						StartCoroutine(Publish(null, missing2PSeparateLevel));
					}
				});
			});
		}

		private void BrowsePublishedRDZipPath(bool missing2PSeparateLevel)
		{
			string text = RDUtils.SanitizeFileName(LevelSettings.levelName, ' ');
			if (text == " - ")
			{
				text = Path.GetFileNameWithoutExtension(base.editor.openedFilePath);
				if (text.IsNullOrEmpty())
				{
					text = "level";
				}
			}
			string text2 = ((!forcePublishButton.gameObject.activeSelf) ? "rdzip" : "zip");
			text = text.Replace("  ", string.Empty);
			string empty = string.Empty;
			try
			{
				empty = FileBrowser.SaveFile(scnEditor.GetLastUsedFolder(), text, RDString.Get("customLevelSelect.import.rdzipFiles"), new string[1] { text2 }, RDString.Get("editor.PublishLevel.saveRDZip"));
			}
			catch (Exception ex)
			{
				Debug.Log(LevelSettings.levelName + " was sanitized to " + text + " with errors: " + ex.Message);
				empty = FileBrowser.SaveFile(scnEditor.GetLastUsedFolder(), "", RDString.Get("customLevelSelect.import.rdzipFiles"), new string[1] { text2 }, RDString.Get("editor.PublishLevel.saveRDZip"));
			}
			if (!string.IsNullOrEmpty(empty))
			{
				StartCoroutine(Publish(empty, missing2PSeparateLevel));
			}
		}

		private IEnumerator Publish(string rdZipPath, bool missingSeparate2PLevelFile)
		{
			ToggleLoadingScreen(show: true);
			bool publishingToSteam = string.IsNullOrEmpty(rdZipPath);
			bool updating = levelToUpdateIndex != -1;
			_ = string.Empty;
			string key = ((!updating) ? (publishingToSteam ? "editor.PublishLevel.publishing" : "editor.PublishLevel.exporting") : "editor.PublishLevel.updating");
			progressText.text = RDString.Get(key);
			progressText.font = RDString.GetAppropiateFontForString(progressText.text);
			levelDataContainer.SetActive(value: false);
			Canvas.ForceUpdateCanvases();
			yield return null;
			string openedFilePath = base.editor.openedFilePath;
			string levelDirectoryName = Path.GetDirectoryName(openedFilePath);
			string text = ((missingSeparate2PLevelFile || string.IsNullOrEmpty(LevelSettings.separate2PLevelFilename)) ? string.Empty : Path.Combine(levelDirectoryName, LevelSettings.separate2PLevelFilename));
			string path = Path.Combine(levelDirectoryName, LevelSettings.previewSongName);
			string fileName = Path.GetFileName(openedFilePath);
			bool flag = !RDFile.Exists(openedFilePath);
			bool flag2 = !LevelSettings.previewSongName.IsNullOrEmpty() && !RDFile.Exists(path);
			bool flag3 = !text.IsNullOrEmpty() && !RDFile.Exists(text);
			if (flag | flag2 | flag3)
			{
				ToggleLoadingScreen(show: false);
				levelDataContainer.SetActive(value: true);
				_ = string.Empty;
				string text2 = (flag ? fileName : ((!flag2) ? LevelSettings.separate2PLevelFilename : LevelSettings.previewSongName));
				printe("File missing: " + text2);
				ShowFileNotFoundDialog(text2, isSeparate2PLevelFile: false);
				yield break;
			}
			if (base.editor.levelSettingsPanel.previewSongPlayer.Playing)
			{
				base.editor.levelSettingsPanel.TogglePlayPreviewSong();
			}
			if (base.editor.isPlaying)
			{
				base.editor.PlayPauseButtonClick();
			}
			List<string> rdlevelFiles = new List<string>();
			List<string> list = new List<string>();
			string publishTempDirectory = RDUtils.GetAvailableDirectoryName(Path.Combine(LevelValidation.TempLevelsFolder, new DirectoryInfo(levelDirectoryName).Name));
			Directory.CreateDirectory(publishTempDirectory);
			List<string> optionalFilesName = new List<string>();
			string[] filesForLevel = GetFilesForLevel(openedFilePath, ref optionalFilesName, ref rdlevelFiles, null, fileName);
			RDLevelData separate2PLevelData = new RDLevelData();
			List<string> list2 = CloneFiles(filesForLevel, optionalFilesName, levelDirectoryName, publishTempDirectory, isForSeparate2PLevel: false, ref separate2PLevelData);
			List<string> filesToPublish = new List<string>();
			if (list2 == null)
			{
				yield break;
			}
			filesToPublish.AddRange(list2);
			CloneIfExists(LevelSettings.syringeIconName);
			CloneIfExists(LevelSettings.previewSongName);
			if (missingSeparate2PLevelFile)
			{
				base.editor.levelSettings.canBePlayedOn = LevelPlayMode.OnePlayerOnly;
			}
			else if (!string.IsNullOrEmpty(text))
			{
				rdlevelFiles.Add(text);
			}
			while (rdlevelFiles.Count() > 0)
			{
				string text3 = rdlevelFiles.Pop();
				printe("Checking extra level file: " + text3);
				string text4 = Path.Combine(levelDirectoryName, "main.rdlevel");
				bool flag4 = text3.ToLowerInvariant() == text4.ToLowerInvariant();
				if (flag4)
				{
					ShowExtraLevelFileErrorsDialog(text3, "", "editor.PublishLevel.secondaryFileNameInvalid");
					yield break;
				}
				if (!((text3 == openedFilePath || list.Contains(text3)) | flag4))
				{
					list.Add(text3);
					if (!RDFile.Exists(text3))
					{
						ShowExtraLevelFileErrorsDialog(text3, "", "editor.PublishLevel.missingFile");
						yield break;
					}
					string text5 = RDFile.ReadAllText(text3);
					text5 = LevelBase.FlipScreenFix(text5);
					new Dictionary<string, object>();
					Dictionary<string, object> rootDict;
					try
					{
						rootDict = Json.Deserialize(text5) as Dictionary<string, object>;
					}
					catch
					{
						ShowExtraLevelFileErrorsDialog(text3, RDString.Get("editor.PublishLevel.FileParseError"));
						yield break;
					}
					RDLevelData separate2PLevelData2;
					try
					{
						separate2PLevelData2 = new RDLevelData(rootDict, onlyActiveEvents: true);
					}
					catch
					{
						ShowExtraLevelFileErrorsDialog(text3, RDString.Get("editor.PublishLevel.FileParseError"));
						yield break;
					}
					string fileName2 = Path.GetFileName(text3);
					LevelErrorName separate2PLevelErrors = LevelValidation.GetSeparate2PLevelErrors(levelDirectoryName, fileName2);
					if (separate2PLevelErrors != LevelErrorName.None)
					{
						List<LevelErrorName> errors = new List<LevelErrorName> { separate2PLevelErrors };
						ShowExtraLevelFileErrorsDialog(text3, errors, separate2PLevelData2);
						yield break;
					}
					List<LevelErrorName> levelErrors = LevelValidation.GetLevelErrors(text3, separate2PLevelData2, ignoreLevelSettings: true);
					if (levelErrors.Count > 0)
					{
						ShowExtraLevelFileErrorsDialog(text3, levelErrors, separate2PLevelData2);
						yield break;
					}
					string[] filesForLevel2 = GetFilesForLevel(text3, ref optionalFilesName, ref rdlevelFiles, separate2PLevelData2, fileName);
					list2 = CloneFiles(filesForLevel2, optionalFilesName, levelDirectoryName, publishTempDirectory, isForSeparate2PLevel: true, ref separate2PLevelData2);
					if (list2 == null)
					{
						yield break;
					}
					filesToPublish.AddRange(list2);
					string text6 = Path.Combine(publishTempDirectory, fileName2);
					base.editor.ForceSave(text6, separate2PLevelData2);
					filesToPublish.Add(text6);
				}
			}
			string text7 = Path.Combine(publishTempDirectory, "main.rdlevel");
			base.editor.ForceSave(text7);
			filesToPublish.Add(text7);
			filesToPublish = filesToPublish.Distinct().ToList();
			bool errorsPublishing = false;
			bool warningPopupShowing = false;
			if (publishingToSteam)
			{
				CustomLevelData customLevelData = new CustomLevelData();
				customLevelData.settings = LevelSettings;
				customLevelData.path = publishTempDirectory;
				customLevelData.tags = new List<string>(LevelSettings.tags.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries));
				string item = LevelSettings.difficulty switch
				{
					LevelDifficulty.Medium => "Medium", 
					LevelDifficulty.Tough => "Tough", 
					LevelDifficulty.VeryTough => "Very Tough", 
					_ => "Easy", 
				};
				customLevelData.tags.Add(item);
				if (LevelSettings.seizureWarning)
				{
					customLevelData.tags.Add("Seizure Warning");
				}
				ApprovalLevel approvalLevel = base.editor.ApprovalLevelForArtist(LevelSettings.artist);
				SpecialArtistType specialArtistType = LevelSettings.specialArtistType;
				if (approvalLevel != ApprovalLevel.Allowed && approvalLevel != ApprovalLevel.PartiallyAllowed && approvalLevel != ApprovalLevel.PartiallyDeclined)
				{
					switch (specialArtistType)
					{
					case SpecialArtistType.AuthorIsArtist:
						customLevelData.tags.Add("Composed by me");
						break;
					case SpecialArtistType.PublicLicense:
						customLevelData.tags.Add("Public License");
						break;
					}
				}
				if (approvalLevel == ApprovalLevel.Pending && specialArtistType == SpecialArtistType.None)
				{
					customLevelData.tags.Add("New Artist");
				}
				LevelPlayMode canBePlayedOn = LevelSettings.canBePlayedOn;
				bool num = canBePlayedOn == LevelPlayMode.BothModes;
				if (num || canBePlayedOn == LevelPlayMode.TwoPlayerOnly)
				{
					customLevelData.tags.Add("Multiplayer");
				}
				if (num || canBePlayedOn == LevelPlayMode.OnePlayerOnly)
				{
					customLevelData.tags.Add("Singleplayer");
				}
				string previewPath = Path.Combine(customLevelData.path, LevelSettings.previewImageName);
				PublishedFileId_t updateId = (updating ? SteamWorkshop.resultItems[levelToUpdateIndex].id : default(PublishedFileId_t));
				yield return SteamWorkshop.UploadToWorkshop(customLevelData.settings.levelName, customLevelData.path, previewPath, customLevelData.settings.description, customLevelData.tags.ToArray(), updateId);
				if (!SteamWorkshop.OperationSucceeded)
				{
					errorsPublishing = true;
					string message = RDString.Get("editor.PublishLevel." + SteamWorkshop.errors[0]);
					base.editor.dialog.Show(message, new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int _) =>
					{
						base.editor.dialog.Hide();
					});
				}
				else if (SteamWorkshop.warnings.Count > 0)
				{
					string message2 = "Warning: " + string.Join("\n", SteamWorkshop.warnings.Select((SteamWorkshop.Warnings s) => RDString.Get("editor.PublishLevel." + s)));
					base.editor.dialog.Show(message2, new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int _) =>
					{
						base.editor.dialog.Hide();
					});
					warningPopupShowing = true;
					base.editor.dialog.onHideComplete = () =>
					{
						warningPopupShowing = false;
					};
				}
			}
			else
			{
				try
				{
					ZipUtils.Zip(rdZipPath, filesToPublish.ToArray());
				}
				catch (Exception ex)
				{
					Debug.Log("error zipping level when exporting: " + ex.Message);
					errorsPublishing = true;
					string message3 = RDString.Get("editor.PublishLevel.errorExporting") + " " + ex.Message;
					base.editor.dialog.Show(message3, new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
					{
						base.editor.dialog.Hide();
					});
				}
			}
			if (missingSeparate2PLevelFile)
			{
				base.editor.levelSettings.canBePlayedOn = LevelPlayMode.BothModes;
			}
			ToggleLoadingScreen(show: false);
			if (!steamUpdateContainer.activeInHierarchy && !forcePublishButton.gameObject.activeSelf)
			{
				levelDataContainer.SetActive(value: true);
			}
			if (errorsPublishing)
			{
				yield break;
			}
			yield return new WaitUntil(() => !warningPopupShowing);
			string message4 = RDString.Get(publishingToSteam ? "editor.PublishLevel.finishPublish" : "editor.PublishLevel.finishExport");
			_ = new string[0];
			string[] options;
			if (publishingToSteam)
			{
				options = new string[3]
				{
					RDString.Get("editor.PublishLevel.done"),
					RDString.Get("editor.PublishLevel.viewInWorkshop"),
					RDString.Get("editor.PublishLevel.agreement")
				};
				base.editor.dialog.Show(message4, options, (int option) =>
				{
					base.editor.dialog.Hide();
					if (option == 1)
					{
						SteamWorkshop.ShowLastUploadedItemOnWorkshop();
					}
					if (option == 2)
					{
						SteamWorkshop.OpenAgreement();
					}
				});
				yield break;
			}
			options = new string[2]
			{
				RDString.Get("editor.PublishLevel.dialogOK"),
				RDString.Get("customLevelSelect.showInFolder")
			};
			base.editor.dialog.Show(message4, options, (int option) =>
			{
				base.editor.dialog.Hide();
				if (option == 1)
				{
					RDUtils.RevealInExplorer(rdZipPath);
				}
			});
			void CloneIfExists(string filename)
			{
				string text8 = Path.Combine(levelDirectoryName, filename);
				if (!string.IsNullOrEmpty(filename) && RDFile.Exists(text8))
				{
					string text9 = Path.Combine(publishTempDirectory, filename);
					if (!RDFile.Exists(text9))
					{
						File.Copy(text8, text9);
					}
					filesToPublish.Add(text9);
				}
			}
		}

		private List<string> CloneFiles(string[] fileToPublish, List<string> optionalFiles, string levelDirectory, string targetDirectory, bool isForSeparate2PLevel, ref RDLevelData separate2PLevelData)
		{
			List<string> list = new List<string>();
			fileToPublish = Array.FindAll(fileToPublish, (string file) => Path.GetExtension(file).Replace(".", string.Empty).ToLowerInvariant() != "rdlevel");
			string[] array = fileToPublish;
			foreach (string text in array)
			{
				string text2 = Path.Combine(levelDirectory, text);
				RDBaseDllDummy.printem(text2);
				if (!RDFile.Exists(text2))
				{
					if (!optionalFiles.Contains(text))
					{
						ToggleLoadingScreen(show: false);
						levelDataContainer.SetActive(value: true);
						ShowFileNotFoundDialog(text, isForSeparate2PLevel);
						RDBaseDllDummy.printem("File missing: " + text2);
						return null;
					}
					RDBaseDllDummy.printem("Didn't find optional file: " + text2);
					continue;
				}
				string text3 = text;
				if (text.Contains(Path.DirectorySeparatorChar))
				{
					int num2 = text2.LastIndexOf(Path.DirectorySeparatorChar);
					if (num2 != -1 && RDDirectory.Exists(text2.Substring(0, num2)))
					{
						text3 = text2.Substring(num2 + 1);
					}
				}
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				bool flag = true;
				if (text3.Length > 250 || text3.StartsWith(".") || string.IsNullOrEmpty(text3.Trim(' ', '.')) || bytes.Any((byte a) =>
				{
					bool result = a <= 31;
					if (a <= 31)
					{
						string arg = Encoding.ASCII.GetString(new byte[1] { a });
						Debug.Log($"Invalid char: {arg}, {a}");
					}
					return result;
				}))
				{
					Debug.Log(string.Format("{0}, {1}, {2}", text3.Length, text3.StartsWith("."), string.IsNullOrEmpty(text3.Trim(' ', '.'))));
					flag = false;
				}
				if (flag)
				{
					string[] invalidFileNameChars = LevelValidation.InvalidFileNameChars;
					foreach (string text4 in invalidFileNameChars)
					{
						if (text3.Contains(text4))
						{
							Debug.Log("Invalid filename: " + text3 + " contains \"" + text4 + "\"");
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					string[] invalidFileNameChars = LevelValidation.InvalidFileNames;
					foreach (string text5 in invalidFileNameChars)
					{
						if (text3 == text5)
						{
							Debug.Log("Invalid filename: " + text3 + " == " + text5);
							flag = false;
							break;
						}
					}
				}
				if (!flag)
				{
					ToggleLoadingScreen(show: false);
					levelDataContainer.SetActive(value: true);
					string message = RDString.Get("editor.PublishLevel.fileNameInvalid").Replace("[filename]", text);
					string[] options = new string[1] { RDString.Get("editor.PublishLevel.dialogOK") };
					base.editor.dialog.Show(message, options, (int option) =>
					{
						base.editor.dialog.Hide();
					});
					return null;
				}
				string text6 = Path.Combine(targetDirectory, text3);
				if (!File.Exists(text6))
				{
					File.Copy(text2, text6);
					list.Add(text6);
				}
			}
			return list;
		}

		private void ShowFileNotFoundDialog(string filename, bool isSeparate2PLevelFile)
		{
			string message = RDString.Get(isSeparate2PLevelFile ? "editor.PublishLevel.separate2PMissingFile" : "editor.PublishLevel.missingFile").Replace("[filename]", filename);
			string[] options = new string[1] { RDString.Get("editor.PublishLevel.dialogOK") };
			base.editor.dialog.Show(message, options, (int option) =>
			{
				base.editor.dialog.Hide();
			});
		}

		private void ShowExtraLevelFileErrorsDialog(string levelPath, string error, string headerKey = "editor.PublishLevel.ExtraLevelErrors")
		{
			string text = RDString.Get(headerKey).Replace("[filename]", Path.GetFileName(levelPath));
			if (!error.IsNullOrEmpty())
			{
				text = text + "\n\n" + error;
			}
			base.editor.dialog.Show(text, new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
			{
				base.editor.dialog.Hide();
			});
			ToggleLoadingScreen(show: false);
			levelDataContainer.SetActive(!incompleteSettingsText.gameObject.activeSelf);
		}

		private void ShowExtraLevelFileErrorsDialog(string levelPath, List<LevelErrorName> errors, RDLevelData levelData)
		{
			ShowExtraLevelFileErrorsDialog(levelPath, levelErrorPresentation.GetErrors(errors, levelData.settings, levelPath, showTitle: false));
		}

		public static string[] GetFilesForLevel(string levelPath, ref List<string> optionalFilesName, ref List<string> rdlevelFiles, RDLevelData levelData = null, string mainLevelName = null)
		{
			scnEditor instance = scnEditor.instance;
			List<string> filesForLevel = new List<string>();
			string fileName = Path.GetFileName(levelPath);
			CheckFile(filesForLevel, fileName);
			List<LevelEvent_Base> list;
			List<LevelEvent_MakeRow> list2;
			List<LevelEvent_MakeDecorationBase> list3;
			string previewImageName;
			string artistPermissionFileName;
			bool flag;
			if (levelData == null)
			{
				list = new List<LevelEvent_Base>();
				foreach (LevelEventControl_Base eventControl in instance.eventControls)
				{
					list.Add(eventControl.levelEvent);
				}
				list2 = instance.rowsData;
				list3 = instance.decorationsData;
				previewImageName = instance.levelSettings.previewImageName;
				artistPermissionFileName = instance.levelSettings.artistPermissionFileName;
				flag = instance.levelSettings.specialArtistType == SpecialArtistType.None;
			}
			else
			{
				list = levelData.levelEvents;
				list2 = levelData.rows;
				list3 = levelData.decorations;
				previewImageName = levelData.settings.previewImageName;
				artistPermissionFileName = levelData.settings.artistPermissionFileName;
				flag = instance.levelSettings.specialArtistType == SpecialArtistType.None;
			}
			foreach (LevelEvent_Base item in list)
			{
				string text = null;
				if (item is LevelEvent_SetBackgroundColor)
				{
					string[] images = ((LevelEvent_SetBackgroundColor)item).images;
					foreach (string text2 in images)
					{
						CheckFile(filesForLevel, text2);
					}
				}
				else if (item is LevelEvent_SetForeground)
				{
					string[] images = ((LevelEvent_SetForeground)item).images;
					foreach (string text3 in images)
					{
						CheckFile(filesForLevel, text3);
					}
				}
				else if (item is LevelEvent_MaskRoom)
				{
					string[] images = ((LevelEvent_MaskRoom)item).images;
					foreach (string text4 in images)
					{
						CheckFile(filesForLevel, text4);
					}
				}
				else if (item is LevelEvent_PlaySong)
				{
					text = ((LevelEvent_PlaySong)item).song.filename;
					CheckFile(filesForLevel, text);
				}
				else if (item is LevelEvent_PlaySound)
				{
					text = ((LevelEvent_PlaySound)item).sound.filename;
					CheckFile(filesForLevel, text);
				}
				else if (item is LevelEvent_SetBeatSound)
				{
					text = ((LevelEvent_SetBeatSound)item).sound.filename;
					CheckFile(filesForLevel, text);
				}
				else if (item is LevelEvent_SetClapSounds)
				{
					text = ((LevelEvent_SetClapSounds)item).p1Sound?.filename;
					CheckFile(filesForLevel, text);
					text = ((LevelEvent_SetClapSounds)item).p2Sound?.filename;
					CheckFile(filesForLevel, text);
				}
				else if (item is LevelEvent_SetGameSound)
				{
					SoundDataStruct[] sounds = ((LevelEvent_SetGameSound)item).sounds;
					for (int i = 0; i < sounds.Length; i++)
					{
						SoundDataStruct soundDataStruct = sounds[i];
						CheckFile(filesForLevel, soundDataStruct.filename);
					}
				}
				else if (item is LevelEvent_SetCountingSound)
				{
					SoundDataStruct[] sounds = ((LevelEvent_SetCountingSound)item).sounds;
					for (int i = 0; i < sounds.Length; i++)
					{
						SoundDataStruct soundDataStruct2 = sounds[i];
						CheckFile(filesForLevel, soundDataStruct2.filename);
					}
				}
				else if (item is LevelEvent_ChangeCharacter)
				{
					string customCharacter = ((LevelEvent_ChangeCharacter)item).customCharacter;
					if (!customCharacter.IsNullOrEmpty())
					{
						CheckForCharacter(customCharacter, optionalFilesName);
					}
				}
				else if (item is LevelEvent_ShowDialogue)
				{
					string[] images = (item as LevelEvent_ShowDialogue).text.Split('\n');
					foreach (string text5 in images)
					{
						int num = text5.IndexOf(':');
						if (num == -1)
						{
							break;
						}
						if (text5.Substring(0, num).Trim().HasImageFileExtension())
						{
							CheckFile(filesForLevel, text);
						}
					}
				}
				else if (item is LevelEvent_GoToLevel)
				{
					LevelEvent_GoToLevel levelEvent_GoToLevel = (LevelEvent_GoToLevel)item;
					levelEvent_GoToLevel.mainLevelName = mainLevelName;
					if (levelEvent_GoToLevel.action != GoToLevelAction.LoadNext && !levelEvent_GoToLevel.rdlevel.IsNullOrEmpty())
					{
						string rDLevelFilename = RDUtils.GetRDLevelFilename(levelEvent_GoToLevel.rdlevel);
						if (!filesForLevel.Contains(rDLevelFilename))
						{
							filesForLevel.Add(rDLevelFilename);
							rdlevelFiles.Add(Path.Combine(Path.GetDirectoryName(levelPath), rDLevelFilename));
						}
					}
				}
				else if (item is LevelEvent_FloatingText)
				{
					text = ((LevelEvent_FloatingText)item).font;
					CheckFile(filesForLevel, text);
				}
				else if (item is LevelEvent_SetFont)
				{
					text = ((LevelEvent_SetFont)item).font;
					CheckFile(filesForLevel, text);
				}
			}
			foreach (LevelEvent_MakeRow item2 in list2)
			{
				string customCharacterName = item2.customCharacterName;
				CheckFile(filesForLevel, item2.pulseSound.filename);
				if (item2.character == Character.Custom && !string.IsNullOrEmpty(customCharacterName))
				{
					CheckForCharacter(customCharacterName, optionalFilesName);
				}
			}
			foreach (LevelEvent_MakeDecorationBase item3 in list3)
			{
				if (item3 is LevelEvent_Sprite levelEvent_Sprite)
				{
					bool flag2 = !levelEvent_Sprite.filename.HasImageFileExtension();
					if (levelEvent_Sprite.filename.IsNullOrEmpty())
					{
						continue;
					}
					if (flag2)
					{
						CheckForCharacter(Path.GetFileNameWithoutExtension(levelEvent_Sprite.filename), optionalFilesName);
					}
					else
					{
						CheckFile(filesForLevel, levelEvent_Sprite.filename);
					}
				}
				if (item3 is LevelEvent_Text levelEvent_Text)
				{
					CheckFile(filesForLevel, levelEvent_Text.font);
				}
			}
			CheckFile(filesForLevel, previewImageName);
			if (flag)
			{
				CheckFile(filesForLevel, artistPermissionFileName);
			}
			return filesForLevel.ToArray();
			void CheckForCharacter(string name, List<string> list4)
			{
				string currentLevelFolderPath = RDEditorUtils.GetCurrentLevelFolderPath();
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				string text6 = (RDDirectory.Exists(currentLevelFolderPath + directorySeparatorChar + name) ? name : "");
				object obj;
				if (string.IsNullOrEmpty(text6))
				{
					obj = "";
				}
				else
				{
					directorySeparatorChar = Path.DirectorySeparatorChar;
					obj = text6 + directorySeparatorChar;
				}
				string text7 = (string)obj;
				CheckFile(filesForLevel, text7 + name + ".json");
				CheckFile(filesForLevel, text7 + name + ".png");
				CheckFile(filesForLevel, text7 + name + "_outline.png");
				CheckFile(filesForLevel, text7 + name + "_glow.png");
				CheckFile(filesForLevel, text7 + name + "_freeze.png");
				list4.Add(text7 + name + "_outline.png");
				list4.Add(text7 + name + "_glow.png");
				list4.Add(text7 + name + "_freeze.png");
				List<string> list5 = RDEditorUtils.AnimationClipNamesFromCustomCharacterName(name, text6);
				List<string> list6 = new List<string> { name };
				foreach (string item4 in list5)
				{
					list6.Add(name + "_" + item4);
				}
				foreach (string item5 in list6)
				{
					CheckFile(filesForLevel, text7 + item5 + ".wav");
					list4.Add(text7 + item5 + ".wav");
					CheckFile(filesForLevel, text7 + item5 + ".ogg");
					list4.Add(text7 + item5 + ".ogg");
				}
			}
		}

		private static void CheckFile(List<string> list, string name)
		{
			if (!string.IsNullOrEmpty(name) && (name.HasAudioFileExtension() || name.HasImageFileExtension() || name.HasFontFileExtension() || name.EndsWith("rdlevel", StringComparison.InvariantCultureIgnoreCase) || name.EndsWith("json", StringComparison.InvariantCultureIgnoreCase)) && !list.Contains(name))
			{
				list.Add(name);
			}
		}
	}
}

#endif
