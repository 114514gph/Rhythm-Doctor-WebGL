using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class Timeline : RDEditorBase
	{
		public const int DefaultCellWidth = 14;

		public const int DefaultCellHeight = 14;

		private const int DefaultCrotchetsPerBar = 8;

		public const float ContentLerpSpeed = 0.8f;

		[Header("Debug")]
		public bool debugData;

		public RectTransform playhead;

		public RectTransform playheadLine;

		public ScrollRect scrollRect;

		public RectTransform scrollview;

		public RectTransform scrollviewContent;

		public RectTransform scrollViewVert;

		public RectTransform scrollViewVertContent;

		public tlVerticalScrollRect verticalScrollRect;

		public GameObject wavePrefab;

		public GameObject bookmarkPrefab;

		public GameObject bookmarkLinePrefab;

		public Material materialPrefab;

		public RectTransform scrollbarLinesContainer;

		public RectTransform wavesContainer;

		public Transform numbersContainer;

		public Transform grid;

		public Transform beatLines;

		public GameObject beatNumberPrefab;

		public GameObject barNumberPrefab;

		public RectTransform disabledRowsQuad;

		public List<RectTransform> cachedBarNumbers = new List<RectTransform>();

		public Image followPlayheadIcon;

		public Image bookmarkPrevIcon;

		public Image bookmarkNextIcon;

		public Text copiedLabel;

		public RectTransform scrollBarSlidingArea;

		public RectTransform rectTransform;

		public Image expandButtonImage;

		public Sprite expandButtonSprite;

		public Sprite contractButtonSprite;

		public Image zoomVertButtonImage;

		public Sprite zoomInVertButtonSprite;

		public Sprite zoomOutVertButtonSprite;

		[NonSerialized]
		public Dictionary<string, Texture> cachedWaveTextures = new Dictionary<string, Texture>();

		[NonSerialized]
		public tlBookmark currentBookmark;

		[NonSerialized]
		public List<tlBookmark> bookmarks = new List<tlBookmark>();

		public bool followPlayhead;

		[NonSerialized]
		public float whereToGo;

		[NonSerialized]
		public List<int> barColumns;

		private bool shouldUpdateUI;

		private bool shouldUpdateUIControls;

		private bool isUpdatingUI;

		private int zoomIndex = 1;

		private int previousZoomIndex = 1;

		private int zoomVertIndex = 1;

		private int previousZoomVertIndex = 1;

		private bool hasZoomed;

		private bool hasZoomedVert;

		private readonly float[] zoomFactors = new float[7] { 0.5f, 1f, 1.5f, 2f, 3f, 4f, 8f };

		private readonly float[] zoomVertFactors = new float[2] { 0.5f, 1f };

		private Texture[][] gridTextures;

		private bool clickedInsideTimeline;

		private int maxUsedY;

		private LevelEvent_SetCrotchetsPerBar defaultCPB;

		public int rowCellCount = 4;

		public float soundTriggerTimer;

		public float soundTriggerCooldown = 0.075f;

		private bool soundTriggeredThisFrame;

		private Tween positionXTween;

		private Tween positionYTween;

		private const int VisibleRowsDefault = 4;

		private const int VisibleRowsExpanded1 = 8;

		private const int VisibleRowsExpanded2 = 12;

		private const float DefaultScrollAreaWidthPercentage = 0.1f;

		private const float MinScrollAreaWidthPercentage = 0.02f;

		public RectTransform selectionBox;

		public float selectionScrollSpeed;

		public bool disableBoxSelection;

		private Vector2 selectionBoxStartPos;

		private float leftScrollAreaWidthPercentage;

		private float rightScrollAreaWidthPercentage;

		private bool useMaxSpeedForLeftScrolling;

		private bool useMaxSpeedForRightScrolling;

		private bool startedSelection;

		private const int BarNumberHalfWidth = 10;

		public List<LevelEvent_SetCrotchetsPerBar> setCrotchetsPerBarEventsTemp;

		private List<LevelEvent_SetRowXs> _setBeatModifiersEvents;

		public int scaledRowCellCount => Mathf.RoundToInt((float)rowCellCount / zoomVertFactor);

		public int usedRowCount
		{
			get
			{
				Tab currentTab = base.editor.currentTab;
				if (currentTab == Tab.Rows || currentTab == Tab.Rooms || currentTab == Tab.Windows)
				{
					return scaledRowCellCount;
				}
				return Mathf.Max(scaledRowCellCount, maxUsedY + 2);
			}
		}

		public float center => 0f - scrollviewContent.anchoredPosition.x + scrollview.rect.width / 2f;

		public float centerVert => scrollViewVertContent.anchoredPosition.y + scrollViewVert.rect.height / 2f;

		public float height => scaledRowCellCount * cellHeight;

		public float width => rectTransform.rect.width;

		public float leftPosition => rectTransform.offsetMin.x;

		public Int2 CellSize => new Int2(cellWidth, cellHeight);

		public new int cellWidth => (int)(14f * zoomFactor);

		public new int cellHeight => (int)(14f * zoomVertFactor);

		public float zoomFactor => zoomFactors[zoomIndex];

		public float zoomVertFactor => zoomVertFactors[zoomVertIndex];

		public Vector2 MousePositionOnTimeline
		{
			get
			{
				float y = rectTransform.offsetMin.y;
				float y2 = scrollViewVertContent.offsetMax.y;
				return new Vector2(base.editor.mousePosition.x - leftPosition - scrollviewContent.localPosition.x - base.editor.bottomPanel.offsetMin.x, (float)(14 * rowCellCount) - base.editor.mousePosition.y + y + y2);
			}
		}

		public bool mouseIsOnNumbers
		{
			get
			{
				float y = rectTransform.offsetMin.y;
				return Mathf.Max(Mathf.FloorToInt(((float)(14 * rowCellCount) - base.editor.mousePosition.y + y) / (float)cellHeight), -1f) == -1f;
			}
		}

		public Int2 cellPointedByMouse
		{
			get
			{
				Vector2 mousePositionOnTimeline = MousePositionOnTimeline;
				int a = Mathf.FloorToInt(mousePositionOnTimeline.x / (float)cellWidth);
				int value = Mathf.FloorToInt(mousePositionOnTimeline.y / (float)cellHeight);
				return new Int2(Mathf.Max(a, 0), Mathf.Clamp(value, -1, usedRowCount - 1));
			}
		}

		public bool IsMousePointerCloserToCellOnRight
		{
			get
			{
				bool result = false;
				Vector2 mousePositionOnTimeline = MousePositionOnTimeline;
				int num = Mathf.FloorToInt(mousePositionOnTimeline.x / (float)cellWidth);
				int num2 = num * cellWidth;
				int num3 = (num + 1) * cellWidth;
				if (Math.Abs(mousePositionOnTimeline.x - (float)num2) > Math.Abs(mousePositionOnTimeline.x - (float)num3))
				{
					result = true;
				}
				return result;
			}
		}

		public List<LevelEvent_SetCrotchetsPerBar> setCrotchetsPerBarEvents
		{
			get
			{
				if (setCrotchetsPerBarEventsTemp != null)
				{
					return setCrotchetsPerBarEventsTemp;
				}
				List<LevelEvent_SetCrotchetsPerBar> list = new List<LevelEvent_SetCrotchetsPerBar>();
				list.Add(defaultCPB);
				foreach (LevelEventControl_Base eventControls_sound in base.editor.eventControls_sounds)
				{
					LevelEvent_Base levelEvent = eventControls_sound.GetComponent<LevelEventControl_Base>().levelEvent;
					if (levelEvent is LevelEvent_SetCrotchetsPerBar)
					{
						list.Add(levelEvent as LevelEvent_SetCrotchetsPerBar);
					}
				}
				return list.OrderBy((LevelEvent_SetCrotchetsPerBar i) => i.sortOrder).ToList();
			}
		}

		public List<LevelEvent_SetBeatsPerMinute> setBeatsPerMinuteEvents
		{
			get
			{
				List<LevelEvent_SetBeatsPerMinute> list = new List<LevelEvent_SetBeatsPerMinute>();
				foreach (LevelEventControl_Base eventControls_sound in base.editor.eventControls_sounds)
				{
					LevelEvent_Base levelEvent = eventControls_sound.GetComponent<LevelEventControl_Base>().levelEvent;
					if (levelEvent is LevelEvent_SetBeatsPerMinute)
					{
						list.Add((LevelEvent_SetBeatsPerMinute)levelEvent);
					}
					else if (levelEvent is LevelEvent_PlaySong)
					{
						LevelEvent_SetBeatsPerMinute item = new LevelEvent_SetBeatsPerMinute
						{
							bar = levelEvent.bar,
							beat = levelEvent.beat,
							beatsPerMinute = (levelEvent as LevelEvent_PlaySong).beatsPerMinute
						};
						list.Add(item);
					}
				}
				return list.OrderBy((LevelEvent_SetBeatsPerMinute i) => i.sortOrder).ToList();
			}
		}

		public List<LevelEvent_SetRowXs> setBeatModifiersEvents
		{
			get
			{
				if (_setBeatModifiersEvents == null)
				{
					UpdateSetBeatModifiersCache();
				}
				return _setBeatModifiersEvents;
			}
		}

		private void Awake()
		{
			defaultCPB = new LevelEvent_SetCrotchetsPerBar
			{
				bar = 1,
				beat = 1f,
				crotchetsPerBar = 8
			};
			verticalScrollRect.horizontalScrollRect = scrollRect;
			verticalScrollRect.timelineEventTrigger = GetComponentInChildren<TimelineEventTrigger>();
			scrollRect.GetComponent<tlHorizontalScrollRect>().verticalScrollRect = verticalScrollRect;
		}

		public void Setup()
		{
			gridTextures = new Texture[zoomVertFactors.Length][];
			int num = 0;
			float[] array = zoomVertFactors;
			foreach (float num2 in array)
			{
				Texture[] array2 = new Texture[zoomFactors.Length];
				int num3 = 0;
				float[] array3 = zoomFactors;
				foreach (float num4 in array3)
				{
					array2[num3] = GetGridTexture(Mathf.RoundToInt((float)cellWidth * num4), Mathf.RoundToInt((float)cellHeight * num2));
					num3++;
				}
				gridTextures[num] = array2;
				num++;
			}
			RawImage component = grid.GetComponent<RawImage>();
			component.color = Color.white.WithAlpha(0.07f);
			component.texture = gridTextures[zoomVertIndex][zoomIndex];
			UpdateUI();
			beatLines.GetComponent<RawImage>().texture = component.texture;
		}

		public void UpdateWhereToGo()
		{
			float f = Mathf.Min(0f, scrollview.rect.width / 2f - playhead.anchoredPosition.x);
			whereToGo = Mathf.RoundToInt(f);
		}

		public void UpdateContentX(float lerpSpeed = 0.8f)
		{
			float num = Mathf.Lerp(scrollviewContent.anchoredPosition.x, whereToGo, lerpSpeed);
			if (Mathf.Abs(num - whereToGo) < 4f)
			{
				num = whereToGo;
			}
			ScrollTo(0f - num);
		}

		public void CenterOnPosition(float x, float duration = 0f)
		{
			ScrollTo(x - scrollview.rect.width / 2f, duration);
		}

		public void CenterOnVertPosition(float y, float duration = 0f)
		{
			ScrollToVert(y - scrollViewVert.rect.height / 2f, duration);
		}

		public void CenterOnPlayhead()
		{
			CenterOnPosition(playhead.anchoredPosition.x, 0.1f);
		}

		public void ScrollTo(float x, float duration = 0f)
		{
			float num = scrollviewContent.sizeDelta.x - scrollview.rect.width;
			x = Mathf.Clamp(x, 0f, num);
			float num2 = x / num;
			if (scrollRect.horizontalNormalizedPosition != num2)
			{
				if (positionXTween != null)
				{
					positionXTween.Kill();
				}
				positionXTween = scrollRect.DOHorizontalNormalizedPos(num2, duration).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true)
					.Done();
			}
		}

		public void ScrollToVert(float y, float duration = 0f)
		{
			float y2 = scrollViewVertContent.sizeDelta.y;
			y = Mathf.Clamp(y, 0f, y2);
			float endValue = (y2 - y) / y2;
			positionYTween?.Kill();
			positionYTween = verticalScrollRect.DOVerticalNormalizedPos(endValue, duration).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true)
				.Done();
		}

		public void PreviousPage()
		{
			followPlayhead = false;
			ScrollTo(0f - scrollviewContent.anchoredPosition.x - scrollview.rect.width);
		}

		public void NextPage()
		{
			followPlayhead = false;
			ScrollTo(0f - scrollviewContent.anchoredPosition.x + scrollview.rect.width);
		}

		public void PreviousBookmark()
		{
			if (bookmarks.Count == 0)
			{
				scrConductor.PlayImmediately("sndEditorErrorSmall", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
				return;
			}
			float num = 0f;
			if (currentBookmark != null)
			{
				num = currentBookmark.rt.anchoredPosition.x;
			}
			float num2 = float.NegativeInfinity;
			tlBookmark tlBookmark2 = null;
			foreach (tlBookmark bookmark in bookmarks)
			{
				float num3 = bookmark.rt.anchoredPosition.x - num;
				if (num3 < 0f && num3 > num2)
				{
					tlBookmark2 = bookmark;
					num2 = num3;
				}
			}
			if (tlBookmark2 != null)
			{
				SelectBookmark(tlBookmark2);
				scrConductor.PlayImmediately("sndEditorBookmarkJump", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 0.9f, 0f, isBeatSound: false, ignoreListenerPause: true);
			}
			else
			{
				scrConductor.PlayImmediately("sndEditorErrorSmall", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
				printe("not found");
			}
		}

		public void NextBookmark()
		{
			if (bookmarks.Count == 0)
			{
				scrConductor.PlayImmediately("sndEditorErrorSmall", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
				return;
			}
			float num = 0f;
			if (currentBookmark != null)
			{
				num = currentBookmark.rt.anchoredPosition.x;
			}
			float num2 = float.PositiveInfinity;
			tlBookmark tlBookmark2 = null;
			foreach (tlBookmark bookmark in bookmarks)
			{
				float num3 = bookmark.rt.anchoredPosition.x - num;
				if (num3 > 0f && num3 < num2)
				{
					tlBookmark2 = bookmark;
					num2 = num3;
				}
			}
			if (tlBookmark2 != null)
			{
				scrConductor.PlayImmediately("sndEditorBookmarkJump", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1.1f, 0f, isBeatSound: false, ignoreListenerPause: true);
				SelectBookmark(tlBookmark2);
			}
			else
			{
				scrConductor.PlayImmediately("sndEditorErrorSmall", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
				printe("not found");
			}
		}

		public void FirstPage()
		{
			scrollviewContent.LocalMoveX(0f);
		}

		public void SelectBookmark(tlBookmark bookmark)
		{
			foreach (tlBookmark bookmark2 in bookmarks)
			{
				bookmark2.SetHighlighted(on: false);
			}
			if (bookmark != null)
			{
				CenterOnPosition(bookmark.rt.anchoredPosition.x);
				currentBookmark = bookmark;
				currentBookmark.SetHighlighted(on: true);
			}
		}

		public void ClearBookmarks()
		{
			foreach (tlBookmark bookmark in bookmarks)
			{
				GameObject obj = bookmark.gameObject;
				UnityEngine.Object.Destroy(bookmark.lineRect.gameObject);
				UnityEngine.Object.Destroy(obj);
			}
			bookmarks.Clear();
		}

		private void UpdateBookmarks(int previousCellWidth)
		{
			foreach (tlBookmark bookmark in bookmarks)
			{
				float num = bookmark.rt.anchoredPosition.x / (float)previousCellWidth;
				bookmark.rt.anchoredPosition = new Vector2((float)cellWidth * num, 8f);
			}
		}

		public void AddBookmark(float xPos)
		{
			tlBookmark component = UnityEngine.Object.Instantiate(bookmarkPrefab, scrollviewContent.transform, worldPositionStays: false).GetComponent<tlBookmark>();
			component.rt.anchoredPosition = new Vector2(xPos, 8f);
			bookmarks.Add(component);
		}

		public void ZoomIn()
		{
			int previousCellWidth = cellWidth;
			int num = zoomIndex;
			zoomIndex = Math.Min(zoomIndex + 1, zoomFactors.Length - 1);
			if (num != zoomIndex)
			{
				scrollRect.StopMovement();
				hasZoomed = true;
				UpdateUI(updateControls: false);
				UpdateBookmarks(previousCellWidth);
				float num2 = RDUtils.PitchSemitonesToPercent((zoomIndex - 3) * 2);
				float pitch = num2;
				scrConductor.PlayImmediately("sndEditorZoomIn", 1f, RDUtils.GetMixerGroup("LevelEditorTimeline"), pitch, 0f, isBeatSound: false, ignoreListenerPause: true);
			}
			else
			{
				scrConductor.PlayImmediately("sndEditorErrorSmall", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
			}
		}

		public void ZoomOut()
		{
			int previousCellWidth = cellWidth;
			int num = zoomIndex;
			zoomIndex = Math.Max(zoomIndex - 1, 0);
			if (num != zoomIndex)
			{
				scrollRect.StopMovement();
				hasZoomed = true;
				UpdateUI(updateControls: false);
				UpdateBookmarks(previousCellWidth);
				float num2 = RDUtils.PitchSemitonesToPercent((zoomIndex - 3) * 2);
				float pitch = num2;
				scrConductor.PlayImmediately("sndEditorZoomOut", 1f, RDUtils.GetMixerGroup("LevelEditorTimeline"), pitch, 0f, isBeatSound: false, ignoreListenerPause: true);
			}
			else
			{
				scrConductor.PlayImmediately("sndEditorErrorSmall", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
			}
		}

		public void ZoomVert()
		{
			int num = zoomVertIndex;
			zoomVertIndex = Math.Abs(zoomVertIndex - 1);
			verticalScrollRect.StopMovement();
			hasZoomedVert = true;
			UpdateUI(updateControls: false);
			bool flag = zoomVertIndex > num;
			zoomVertButtonImage.sprite = (flag ? zoomOutVertButtonSprite : zoomInVertButtonSprite);
			string sound = (flag ? "sndEditorZoomIn" : "sndEditorZoomOut");
			float num2 = RDUtils.PitchSemitonesToPercent((zoomVertIndex - 3) * 2);
			float pitch = num2;
			scrConductor.PlayImmediately(sound, 1f, RDUtils.GetMixerGroup("LevelEditorTimeline"), pitch, 0f, isBeatSound: false, ignoreListenerPause: true);
		}

		public void CycleExpanded(bool playSound = true)
		{
			int num = rowCellCount;
			if (rowCellCount < 4)
			{
				rowCellCount = 4;
			}
			else if (rowCellCount < 8)
			{
				rowCellCount = 8;
			}
			else if (rowCellCount < 12)
			{
				rowCellCount = 12;
			}
			else
			{
				rowCellCount = 4;
			}
			UpdateUI(updateControls: false);
			if (rowCellCount == num)
			{
				rowCellCount = 0;
				CycleExpanded(playSound);
			}
			else if (playSound)
			{
				float pitch = RDUtils.PitchSemitonesToPercent((rowCellCount > 8) ? 3f : ((rowCellCount == 8) ? 0f : (-2f)));
				LevelEditorPlaySound("sndEditorTogglePanelExpand", "LevelEditorActive", 1f, pitch);
			}
		}

		private void UpdateTimelineHeight()
		{
			int a = Mathf.FloorToInt((Screen.height / base.editor.scaleFactor - 36) / 14);
			rowCellCount = Mathf.Max(1, Mathf.Min(a, rowCellCount));
			expandButtonImage.sprite = ((rowCellCount >= 12) ? contractButtonSprite : expandButtonSprite);
			RectTransform bottomPanel = base.editor.bottomPanel;
			float y = 16f + (float)(rowCellCount * 14);
			bottomPanel.SizeDeltaY(y);
		}

		private void Update()
		{
			if (base.editor.fullscreen)
			{
				return;
			}
			if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.UpArrow))
			{
				rowCellCount++;
				UpdateUI(updateControls: false);
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.DownArrow))
			{
				rowCellCount--;
				UpdateUI(updateControls: false);
			}
			if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
			{
				disableBoxSelection = false;
			}
			if (disableBoxSelection)
			{
				return;
			}
			if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.Mouse0))
			{
				Vector3[] array = new Vector3[4];
				scrollview.GetWorldCorners(array);
				Vector2 vector = array[0];
				Vector2 vector2 = array[2];
				Vector3[] array2 = new Vector3[4];
				scrollviewContent.GetWorldCorners(array2);
				vector2.y = array2[1].y;
				Vector3 mousePosition = Input.mousePosition;
				Vector2 mousePositionOnTimeline = MousePositionOnTimeline;
				float num = vector2.x - vector.x;
				bool num2 = mousePosition.x > vector.x && mousePosition.x < vector2.x && mousePosition.y > vector.y && mousePosition.y < vector2.y;
				LevelEventControl_Base[] componentsInChildren = base.editor.currentTabSection.containerTransform.GetComponentsInChildren<LevelEventControl_Base>(includeInactive: true);
				if (num2 && (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.Mouse0)))
				{
					if (!Input.GetKey(KeyCode.LeftShift))
					{
						base.editor.DeselectAllEventControls(updateInspectorUI: false);
					}
					startedSelection = true;
					selectionBoxStartPos = mousePositionOnTimeline;
					selectionBox.anchoredPosition = new Vector2(mousePositionOnTimeline.x, mousePositionOnTimeline.y * -1f);
					float num3 = num * 0.1f;
					rightScrollAreaWidthPercentage = ((mousePosition.x > vector2.x - num3) ? ((vector2.x - mousePosition.x) / num) : 0.1f);
					leftScrollAreaWidthPercentage = ((mousePosition.x < vector.x + num3) ? ((mousePosition.x - vector.x) / num) : 0.1f);
				}
				if (startedSelection)
				{
					float num4 = num * rightScrollAreaWidthPercentage;
					float num5 = vector2.x - num4;
					float num6 = num * leftScrollAreaWidthPercentage;
					float num7 = vector.x + num6;
					float num8 = num * 0.02f;
					int num9 = 0;
					float value = 0f;
					if (Input.mousePosition.x > num5)
					{
						num9 = 1;
						value = ((mousePosition.x > vector2.x - num8) ? 1f : ((mousePosition.x - num5) / num4));
					}
					else if (Input.mousePosition.x < num7)
					{
						num9 = -1;
						value = ((mousePosition.x < vector.x + num8) ? 1f : ((num7 - mousePosition.x) / num6));
					}
					if (num9 != 0)
					{
						value = Mathf.Clamp(value, 0f, 1f) * (float)num9;
						float num10 = selectionScrollSpeed * value * RDTime.unscaledDeltaTime;
						scrollRect.horizontalNormalizedPosition += num10 / scrollviewContent.sizeDelta.x;
						scrollRect.horizontalNormalizedPosition = Mathf.Clamp(scrollRect.horizontalNormalizedPosition, 0f, 1f);
					}
					float num11 = Mathf.Clamp(mousePositionOnTimeline.x, 0f, scrollviewContent.sizeDelta.x);
					float num12 = Mathf.Clamp(mousePositionOnTimeline.y, 0f, usedRowCount * cellHeight);
					float x = Mathf.Min(num11, selectionBoxStartPos.x);
					float y = 0f - Mathf.Min(num12, selectionBoxStartPos.y);
					float x2 = Mathf.Abs(num11 - selectionBoxStartPos.x);
					float y2 = Mathf.Abs(num12 - selectionBoxStartPos.y);
					selectionBox.anchoredPosition = new Vector2(x, y);
					selectionBox.sizeDelta = new Vector2(x2, y2);
					Rect rect = new Rect(selectionBox.anchoredPosition.x, selectionBox.anchoredPosition.y * -1f, selectionBox.rect.width, selectionBox.rect.height);
					LevelEventControl_Base[] array3 = componentsInChildren;
					foreach (LevelEventControl_Base levelEventControl_Base in array3)
					{
						RectTransform component = levelEventControl_Base.GetComponent<RectTransform>();
						Rect other = new Rect(component.anchoredPosition.x, component.anchoredPosition.y * -1f, component.sizeDelta.x, component.sizeDelta.y);
						if (rect.Overlaps(other, allowInverse: true))
						{
							if (!levelEventControl_Base.isShownAsSelected && base.editor.EventIsVisible(levelEventControl_Base.levelEvent))
							{
								bool flag = true;
								if (levelEventControl_Base.levelEvent is LevelEvent_AdvanceText)
								{
									flag = (levelEventControl_Base.levelEvent as LevelEvent_AdvanceText).floatingText.showChildren;
								}
								if (flag)
								{
									levelEventControl_Base.ShowAsSelected(notBeingActuallySelected: true);
									PlayPassiveSoundIfCooldown("sndEditorEventSelectAdditional");
								}
							}
						}
						else if (levelEventControl_Base.isShownAsSelected && !Input.GetKey(KeyCode.LeftShift))
						{
							levelEventControl_Base.ShowAsDeselected();
							PlayPassiveSoundIfCooldown("sndEditorEventDeselect");
							base.editor.selectedControls.Remove(levelEventControl_Base);
						}
					}
				}
			}
			else if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.Mouse0))
			{
				startedSelection = false;
				LevelEventControl_Base[] array3 = base.editor.currentTabSection.containerTransform.GetComponentsInChildren<LevelEventControl_Base>(includeInactive: true);
				foreach (LevelEventControl_Base levelEventControl_Base2 in array3)
				{
					if (levelEventControl_Base2.isShownAsSelected && !base.editor.selectedControls.Contains(levelEventControl_Base2))
					{
						base.editor.AddEventControlToSelection(levelEventControl_Base2);
					}
				}
				selectionBox.sizeDelta = Vector2.zero;
			}
			if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.A))
			{
				float num13 = scrollview.rect.width;
				float posX = 0f - scrollviewContent.anchoredPosition.x + num13 / 2f;
				int bar = GetBarAndBeatWithPosX(posX).bar;
				foreach (TabSection tabSection in base.editor.tabSections)
				{
					LevelEventControl_Base[] array3 = tabSection.containerTransform.GetComponentsInChildren<LevelEventControl_Base>(includeInactive: true);
					foreach (LevelEventControl_Base levelEventControl_Base3 in array3)
					{
						if (levelEventControl_Base3.bar == bar && !base.editor.selectedControls.Contains(levelEventControl_Base3))
						{
							levelEventControl_Base3.ShowAsSelected(notBeingActuallySelected: true);
							base.editor.AddEventControlToSelection(levelEventControl_Base3);
						}
					}
				}
				scrConductor.PlayImmediately("sndEditorEventSelectMultiple", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
			}
			float num14 = scrollBarSlidingArea.transform.parent.GetComponent<RectTransform>().rect.width;
			scrollBarSlidingArea.gameObject.SetActive(num14 > 10f);
			soundTriggerTimer -= Time.unscaledDeltaTime;
			soundTriggeredThisFrame = false;
		}

		private void CullMaskedObjects()
		{
			Vector2 anchoredPosition = scrollviewContent.anchoredPosition;
			Vector2 anchoredPosition2 = scrollViewVertContent.anchoredPosition;
			float num = 0f - anchoredPosition.x;
			float num2 = num + scrollview.rect.width;
			float y = anchoredPosition2.y;
			float num3 = y + scrollViewVert.rect.height;
			foreach (RectTransform cachedBarNumber in cachedBarNumbers)
			{
				float x = cachedBarNumber.localPosition.x;
				bool active = x >= num - 10f && x <= num2 + 10f;
				cachedBarNumber.gameObject.SetActive(active);
			}
			List<LevelEventControl_Base> list = null;
			switch (base.editor.currentTab)
			{
			case Tab.Song:
				list = base.editor.eventControls_sounds;
				break;
			case Tab.Rows:
				if (base.editor.eventControls_rows.Any())
				{
					int selectedRowsTabPageIndex = base.editor.selectedRowsTabPageIndex;
					int lastRowOfRoom = base.editor.LastRowOfRoom(selectedRowsTabPageIndex);
					int firstRowOfRoom = ((selectedRowsTabPageIndex != 0) ? (base.editor.LastRowOfRoom(selectedRowsTabPageIndex - 1) + 1) : 0);
					list = base.editor.eventControls_rows.Where((List<LevelEventControl_Base> _, int index) => index <= lastRowOfRoom && index >= firstRowOfRoom).SelectMany((List<LevelEventControl_Base> result) => result).ToList();
				}
				break;
			case Tab.Actions:
				list = base.editor.eventControls_actions;
				break;
			case Tab.Decorations:
				if (base.editor.eventControls_decorations.Any())
				{
					int selectedDecorationsTabPageIndex = base.editor.selectedDecorationsTabPageIndex;
					int lastDecorationOfRoom = base.editor.LastDecorationOfRoom(selectedDecorationsTabPageIndex);
					int firstDecorationOfRoom = ((selectedDecorationsTabPageIndex != 0) ? (base.editor.LastDecorationOfRoom(selectedDecorationsTabPageIndex - 1) + 1) : 0);
					list = base.editor.eventControls_decorations.Where((List<LevelEventControl_Base> _, int index) => index <= lastDecorationOfRoom && index >= firstDecorationOfRoom).SelectMany((List<LevelEventControl_Base> result) => result).ToList();
				}
				break;
			case Tab.Rooms:
				list = base.editor.eventControls_rooms;
				break;
			case Tab.Windows:
				list = base.editor.eventControls_windows;
				break;
			}
			if (list == null)
			{
				return;
			}
			foreach (LevelEventControl_Base item in list)
			{
				LevelEvent_Base levelEvent = item.levelEvent;
				RectTransform rt = item.rt;
				Vector3 localPosition = rt.localPosition;
				Vector2 sizeDelta = rt.sizeDelta;
				float x2 = localPosition.x;
				float num4 = x2 + sizeDelta.x;
				float num5 = 0f - localPosition.y;
				float num6 = num5 + sizeDelta.y;
				if (levelEvent is LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat)
				{
					float x3 = ((LevelEventControl_AddClassicBeat)item).skipshotBorder.rectTransform.sizeDelta.x;
					num4 += (levelEvent_AddOneshotBeat.skipshot ? x3 : 0f);
					float x4 = ((LevelEventControl_AddClassicBeat)item).subdivBorder.rectTransform.sizeDelta.x;
					num4 += ((levelEvent_AddOneshotBeat.subdivisions > 1) ? x4 : 0f);
				}
				else if (levelEvent is LevelEvent_AddClassicBeat || levelEvent is LevelEvent_AddFreeTimeBeat || levelEvent is LevelEvent_PulseFreeTimeBeat)
				{
					float x5 = ((LevelEventControl_AddClassicBeat)item).heldbeatBorder.rectTransform.sizeDelta.x;
					num4 += Mathf.Max(x5, 0f);
				}
				else if (item is LevelEventControl_Action levelEventControl_Action && (bool)levelEventControl_Action.durationBorder)
				{
					num4 += Mathf.Max(levelEventControl_Action.durationBorder.rectTransform.sizeDelta.x - 1f, 0f);
				}
				bool flag = num4 + 5f >= num && x2 - 5f <= num2 && num5 - 5f <= num3 && num6 + 5f >= y && item.visible && base.editor.EventIsVisible(levelEvent);
				GameObject gameObject = item.gameObject;
				if (!flag && gameObject.activeInHierarchy)
				{
					gameObject.SetActive(value: false);
				}
				else if (flag && !gameObject.activeInHierarchy)
				{
					gameObject.SetActive(value: true);
				}
			}
		}

		private void LateUpdate()
		{
			if (base.editor.fullscreen)
			{
				return;
			}
			CullMaskedObjects();
			if (base.game != null && base.game.levelFinishedLoading && base.conductor != null && base.conductor.barNumber >= base.editor.startBar && !base.editor.scrubbing)
			{
				if (base.conductor.barNumberUpdateOnBar > 0)
				{
					int barNumberUpdateOnBar = base.conductor.barNumberUpdateOnBar;
					LevelEvent_SetCrotchetsPerBar closestSetCPBEventWithBar = GetClosestSetCPBEventWithBar(barNumberUpdateOnBar, setCrotchetsPerBarEvents);
					if (closestSetCPBEventWithBar != null)
					{
						float num = (float)barNumberUpdateOnBar + Mathf.Clamp01(base.conductor.progressOnCurrentBar);
						int num2 = (int)num;
						float beatNumber = (float)closestSetCPBEventWithBar.crotchetsPerBar * (num % 1f) + 1f;
						if (num2 < 1)
						{
							num2 = 1;
							beatNumber = 1f;
						}
						BarAndBeat position = new BarAndBeat(num2, beatNumber);
						float posXFromBarAndBeat = GetPosXFromBarAndBeat(position);
						posXFromBarAndBeat = Mathf.Round(posXFromBarAndBeat);
						MovePlayHead(posXFromBarAndBeat);
						if (followPlayhead)
						{
							UpdateWhereToGo();
						}
					}
				}
				else
				{
					MovePlayHead(0f);
				}
			}
			else
			{
				int barNumber = Math.Max(1, base.editor.startBar);
				float posXFromBarAndBeat2 = GetPosXFromBarAndBeat(new BarAndBeat(barNumber, 1f));
				MovePlayHead(posXFromBarAndBeat2);
			}
			if (followPlayhead)
			{
				UpdateContentX();
			}
			followPlayheadIcon.color = (followPlayhead ? base.gc.followPlayhead_onColor : base.gc.followPlayhead_offColor);
			bool num3 = bookmarks.Count > 0;
			bool flag = num3 && currentBookmark != null && !bookmarks.First().isSelected;
			bool flag2 = num3 && !bookmarks.Last().isSelected;
			bookmarkPrevIcon.color = (flag ? base.gc.selectBookmark_onColor : base.gc.selectBookmark_offColor);
			bookmarkNextIcon.color = (flag2 ? base.gc.selectBookmark_onColor : base.gc.selectBookmark_offColor);
			int num4 = base.editor.currentTab switch
			{
				Tab.Rows => base.editor.currentPageRowsData.Count, 
				Tab.Rooms => 4, 
				Tab.Decorations => Mathf.Min(base.editor.currentPageDecorationsData.Count, scaledRowCellCount - 1), 
				Tab.Windows => 4, 
				_ => scaledRowCellCount, 
			};
			int num5 = (scaledRowCellCount - num4) * cellHeight;
			disabledRowsQuad.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, num5);
			if (!isUpdatingUI && shouldUpdateUI)
			{
				shouldUpdateUI = false;
				isUpdatingUI = true;
				StartCoroutine(UpdateUIInternalCo());
			}
		}

		public void MovePlayHead(float pos)
		{
			playhead.LocalMoveX(pos);
			float num = scrollbarLinesContainer.rect.width;
			float num2 = num / scrollviewContent.sizeDelta.x;
			playheadLine.AnchorPosX(Mathf.Clamp(playhead.anchoredPosition.x * num2, 0f, num));
		}

		public void ToggleFollowPlayhead()
		{
			followPlayhead = !followPlayhead;
			scrConductor.PlayImmediately(followPlayhead ? "sndEditorToggleFollowOn" : "sndEditorToggleFollowOff", 1f, RDUtils.GetMixerGroup("LevelEditorTimeline"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
		}

		public void UnfollowPlayhead()
		{
			followPlayhead = false;
		}

		private bool PointInsideRect(RectTransform rect, Vector2 point)
		{
			if (point.x > rect.anchoredPosition.x && point.x < rect.anchoredPosition.x + rect.sizeDelta.x && point.y > rect.anchoredPosition.y - rect.sizeDelta.y && point.y < rect.anchoredPosition.y)
			{
				return true;
			}
			return false;
		}

		public static Texture2D GetGridTexture(int cellWidth, int cellHeight)
		{
			Texture2D texture2D = new Texture2D(cellWidth, cellHeight, TextureFormat.Alpha8, mipChain: false);
			for (int i = 0; i < texture2D.height; i++)
			{
				for (int j = 0; j < texture2D.width; j++)
				{
					texture2D.SetPixel(j, i, Color.clear);
				}
			}
			for (int k = 0; k < texture2D.height; k++)
			{
				for (int l = 0; l < 1; l++)
				{
					texture2D.SetPixel(l, k, Color.white);
				}
			}
			for (int m = 0; m < texture2D.width; m++)
			{
				for (int n = 0; n < 1; n++)
				{
					texture2D.SetPixel(m, n, Color.white);
				}
			}
			texture2D.wrapMode = TextureWrapMode.Repeat;
			texture2D.filterMode = FilterMode.Point;
			texture2D.Apply();
			return texture2D;
		}

		private void ResizeGrid(int beatCount)
		{
			int num = usedRowCount;
			grid.GetComponent<RectTransform>().sizeDelta = new Vector2(beatCount * cellWidth, num * cellHeight);
			RawImage component = grid.GetComponent<RawImage>();
			component.uvRect = new Rect(0f, 0f, beatCount, num);
			component.texture = gridTextures[zoomVertIndex][zoomIndex];
			RectTransform component2 = beatLines.GetComponent<RectTransform>();
			component2.sizeDelta = new Vector2(beatCount * cellWidth, component2.sizeDelta.y);
			RawImage component3 = beatLines.GetComponent<RawImage>();
			component3.uvRect = new Rect(0f, 0f, beatCount, 0.2f);
			component3.texture = gridTextures[1][zoomIndex];
		}

		public void UpdateMaxUsedY()
		{
			maxUsedY = 0;
			Tab currentTab = base.editor.currentTab;
			switch (currentTab)
			{
			case Tab.Decorations:
				maxUsedY = base.tabSection_decorations.decorationHeaders.Count - 2;
				break;
			case Tab.Song:
			case Tab.Actions:
				foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
				{
					if (eventControl.tabSection.tab == currentTab && eventControl.levelEvent.GetType() != typeof(LevelEvent_Base))
					{
						maxUsedY = Mathf.Max(eventControl.levelEvent.y, maxUsedY);
					}
				}
				break;
			}
			ApplyNewMaxUsedY();
		}

		private void ApplyNewMaxUsedY()
		{
			int num = usedRowCount;
			grid.GetComponent<RectTransform>().SizeDeltaY(num * cellHeight);
			RawImage component = grid.GetComponent<RawImage>();
			component.uvRect = new Rect(0f, 0f, component.uvRect.width, num);
			component.texture = gridTextures[zoomVertIndex][zoomIndex];
			RectTransform component2 = beatLines.GetComponent<RectTransform>();
			component2.SizeDeltaY(component2.sizeDelta.y);
			component = beatLines.GetComponent<RawImage>();
			component.uvRect = new Rect(0f, 0f, component.uvRect.width, 0.2f);
			component.texture = gridTextures[1][zoomIndex];
			scrollViewVertContent.SizeDeltaY((num - scaledRowCellCount) * cellHeight);
		}

		public void UpdateUI(bool updateControls = true)
		{
			UpdateTimelineHeight();
			UpdateSetBeatModifiersCache();
			shouldUpdateUIControls = updateControls;
			shouldUpdateUI = true;
		}

		public void UpdateClassicControlsForRow(int row)
		{
			foreach (LevelEventControl_Base item in base.editor.eventControls_rows[row])
			{
				if (item is LevelEventControl_AddClassicBeat)
				{
					item.UpdateUI();
				}
			}
		}

		public static LevelEvent_SetCrotchetsPerBar GetClosestSetCPBEventWithBar(int bar, List<LevelEvent_SetCrotchetsPerBar> cpbEvents)
		{
			for (int num = cpbEvents.Count - 1; num >= 0; num--)
			{
				if (bar - cpbEvents[num].bar >= 0)
				{
					return cpbEvents[num];
				}
			}
			Debug.LogWarning("this should never happen");
			return null;
		}

		public LevelEvent_SetRowXs GetClosestSetBeatModifiersEventWithRow(int row, BarAndBeat barAndBeat)
		{
			for (int num = setBeatModifiersEvents.Count - 1; num >= 0; num--)
			{
				LevelEvent_SetRowXs levelEvent_SetRowXs = setBeatModifiersEvents[num];
				if (row == levelEvent_SetRowXs.row && GetPosXFromBarAndBeat(barAndBeat) >= GetPosXFromBarAndBeat(levelEvent_SetRowXs.barAndBeat) && levelEvent_SetRowXs.active)
				{
					return levelEvent_SetRowXs;
				}
			}
			return null;
		}

		public BarAndBeat FixBarAndBeat(BarAndBeat barAndBeat)
		{
			return GetBarAndBeatWithPosX(GetPosXFromBarAndBeat(barAndBeat));
		}

		public float GetPosXFromBarAndBeat(int bar, float beat, bool debug = false)
		{
			return GetColumnFromBarAndBeat(bar, beat, setCrotchetsPerBarEvents, debug) * (float)cellWidth;
		}

		public float GetPosXFromBarAndBeat(BarAndBeat position, bool debug = false)
		{
			return GetColumnFromBarAndBeat(position, setCrotchetsPerBarEvents, debug) * (float)cellWidth;
		}

		public float GetPosXFromBarAndBeat(BarAndBeat position, List<LevelEvent_SetCrotchetsPerBar> cpbEvents, bool debug = false)
		{
			return GetColumnFromBarAndBeat(position, cpbEvents, debug) * (float)cellWidth;
		}

		public static float GetColumnFromBarAndBeat(BarAndBeat position, List<LevelEvent_SetCrotchetsPerBar> cpbEvents, bool debug)
		{
			return GetColumnFromBarAndBeat(position.bar, position.beat, cpbEvents, debug);
		}

		public static float GetColumnFromBarAndBeat(int bar, float beat, List<LevelEvent_SetCrotchetsPerBar> cpbEvents, bool debug)
		{
			if (debug)
			{
				MonoBehaviour.print("bar: " + bar + ",  beat:" + beat);
			}
			LevelEvent_SetCrotchetsPerBar closestSetCPBEventWithBar = GetClosestSetCPBEventWithBar(bar, cpbEvents);
			int num = 0;
			int num2 = cpbEvents.IndexOf(closestSetCPBEventWithBar);
			num = (bar - closestSetCPBEventWithBar.bar) * closestSetCPBEventWithBar.crotchetsPerBar;
			for (int num3 = num2; num3 > 0; num3--)
			{
				num += (cpbEvents[num3].barAndBeat.bar - cpbEvents[num3 - 1].barAndBeat.bar) * cpbEvents[num3 - 1].crotchetsPerBar;
			}
			return (float)num + (beat - 1f);
		}

		public int GetRowIndexWithPosY(int y)
		{
			y = Math.Min(0, y);
			int val = usedRowCount - 1;
			return Math.Max(Math.Min(Math.Abs(y / cellHeight), val), 0);
		}

		public float GetPosYFromRowIndex(int rowIndex)
		{
			return -cellHeight * rowIndex;
		}

		public BarAndBeat GetBarAndBeatWithPosX(float posX, List<LevelEvent_SetCrotchetsPerBar> cpbEvents = null, float minX = 0f)
		{
			posX = Mathf.Max(minX, posX);
			if (cpbEvents == null)
			{
				cpbEvents = setCrotchetsPerBarEvents;
			}
			for (int i = 0; i < cpbEvents.Count; i++)
			{
				int index = cpbEvents.Count - 1 - i;
				LevelEvent_SetCrotchetsPerBar levelEvent_SetCrotchetsPerBar = cpbEvents[index];
				float posXFromBarAndBeat = GetPosXFromBarAndBeat(new BarAndBeat(levelEvent_SetCrotchetsPerBar.bar, 1f), cpbEvents);
				if (posX >= posXFromBarAndBeat)
				{
					int barNumber = levelEvent_SetCrotchetsPerBar.bar + Mathf.FloorToInt((posX - posXFromBarAndBeat) / (float)(cellWidth * levelEvent_SetCrotchetsPerBar.crotchetsPerBar));
					float beatNumber = (posX - posXFromBarAndBeat) / (float)cellWidth % (float)levelEvent_SetCrotchetsPerBar.crotchetsPerBar + 1f;
					return new BarAndBeat(barNumber, beatNumber);
				}
			}
			Debug.LogError("this should never happen!");
			return new BarAndBeat(1, 1f);
		}

		public static BarAndBeat GetBarAndBeatWithColumn(float column, List<LevelEvent_SetCrotchetsPerBar> cpbEvents)
		{
			column = Mathf.Max(0f, column);
			List<LevelEvent_SetCrotchetsPerBar> list = new List<LevelEvent_SetCrotchetsPerBar>(cpbEvents);
			list.Reverse();
			foreach (LevelEvent_SetCrotchetsPerBar item in list)
			{
				float columnFromBarAndBeat = GetColumnFromBarAndBeat(new BarAndBeat(item.bar, 1f), cpbEvents, debug: false);
				if (column >= columnFromBarAndBeat)
				{
					int barNumber = item.bar + Mathf.FloorToInt((column - columnFromBarAndBeat) / (float)item.crotchetsPerBar);
					float beatNumber = (column - columnFromBarAndBeat) % (float)item.crotchetsPerBar + 1f;
					return new BarAndBeat(barNumber, beatNumber);
				}
			}
			Debug.LogError("this should never happen!");
			return new BarAndBeat(1, 1f);
		}

		public void UpdateSetBeatModifiersCache()
		{
			List<LevelEvent_SetRowXs> list = new List<LevelEvent_SetRowXs>();
			foreach (List<LevelEventControl_Base> eventControls_row in base.editor.eventControls_rows)
			{
				foreach (LevelEventControl_Base item in eventControls_row)
				{
					LevelEvent_Base levelEvent = item.GetComponent<LevelEventControl_Base>().levelEvent;
					if (levelEvent is LevelEvent_SetRowXs)
					{
						list.Add(levelEvent as LevelEvent_SetRowXs);
					}
				}
			}
			_setBeatModifiersEvents = list.OrderBy((LevelEvent_SetRowXs i) => i.sortOrder).ToList();
		}

		private void CreateNumbers(int columnStart, int barNumberStart, int barCount, int crotchetsPerBar)
		{
			RectTransform rectTransform = numbersContainer as RectTransform;
			int num = (columnStart + barCount * crotchetsPerBar + 1) * cellWidth;
			rectTransform.sizeDelta = new Vector2(num, rectTransform.sizeDelta.y);
			int num2 = barNumberStart - 1;
			for (int i = num2; i < num2 + barCount; i++)
			{
				bool num3 = cachedBarNumbers.Count > i;
				RectTransform rectTransform2 = (num3 ? cachedBarNumbers[i] : UnityEngine.Object.Instantiate(barNumberPrefab).GetComponent<RectTransform>());
				if (!num3)
				{
					string text = (i + 1).ToString();
					rectTransform2.gameObject.name = "Bar " + text;
					rectTransform2.SetParent(numbersContainer, worldPositionStays: false);
					rectTransform2.GetComponent<Text>().text = text;
					cachedBarNumbers.Add(rectTransform2);
				}
				else
				{
					rectTransform2.gameObject.SetActive(value: true);
				}
				float num4 = ((i == 0) ? 5f : 0f);
				float x = (float)((columnStart + (i - num2) * crotchetsPerBar) * cellWidth) + num4;
				rectTransform2.anchoredPosition = rectTransform2.anchoredPosition.WithX(x);
			}
		}

		private IEnumerator UpdateUIInternalCo()
		{
			float maxWidth = width;
			int maxBar = 1;
			maxUsedY = 0;
			int waveIndex = 0;
			foreach (LevelEventControl_Base levelEventControl in base.editor.eventControls)
			{
				LevelEvent_Base levelEvent = levelEventControl.levelEvent;
				if (hasZoomed || hasZoomedVert)
				{
					levelEventControl.UpdateUI();
				}
				if (levelEvent is LevelEvent_PlaySong && !string.IsNullOrEmpty(((LevelEvent_PlaySong)levelEvent).song.filename))
				{
					LevelEvent_PlaySong playSong = (LevelEvent_PlaySong)levelEvent;
					CoroutineWithData loadAudio = new CoroutineWithData(this, RDEditorUtils.AudioClipFromFilename(playSong.song.filename));
					yield return loadAudio.coroutine;
					RDAudioLoadResult obj = (RDAudioLoadResult)loadAudio.result;
					AudioClip clip = obj.clip;
					RDAudioLoadType type = obj.type;
					if (type != RDAudioLoadType.SuccessExternalClipLoaded && type != RDAudioLoadType.SuccessInternalClipLoaded)
					{
						printe("clip wasn't loaded successfully: " + playSong.song.filename);
						continue;
					}
					List<LevelEvent_SetBeatsPerMinute> list = setBeatsPerMinuteEvents;
					float offsetInSeconds = playSong.song.offsetInSeconds;
					float length = clip.length;
					float num = offsetInSeconds / clip.length;
					LevelEvent_SetBeatsPerMinute levelEvent_SetBeatsPerMinute = null;
					int num2 = 0;
					foreach (LevelEvent_SetBeatsPerMinute item in list)
					{
						if (item.sortOrder == playSong.sortOrder)
						{
							levelEvent_SetBeatsPerMinute = item;
						}
						else if (levelEvent_SetBeatsPerMinute != null && item.sortOrder > levelEvent_SetBeatsPerMinute.sortOrder)
						{
							float num3 = levelEvent_SetBeatsPerMinute.beatsPerMinute / 60f;
							float num4 = length * num3 * (float)cellWidth;
							float posXFromBarAndBeat = GetPosXFromBarAndBeat(item.barAndBeat);
							float posXFromBarAndBeat2 = GetPosXFromBarAndBeat(levelEvent_SetBeatsPerMinute.barAndBeat);
							float num5 = posXFromBarAndBeat - posXFromBarAndBeat2;
							float num6 = num5 / num4;
							SetWaveTexture(waveIndex, posXFromBarAndBeat2, num, num6, clip, num5);
							num += num6;
							maxWidth = Mathf.Max(posXFromBarAndBeat + num5, maxWidth);
							waveIndex++;
							levelEvent_SetBeatsPerMinute = item;
						}
						num2++;
					}
					if (num < 1f)
					{
						float num7 = levelEvent_SetBeatsPerMinute.beatsPerMinute / 60f;
						float num8 = length * num7 * (float)cellWidth;
						float posXFromBarAndBeat3 = GetPosXFromBarAndBeat(levelEvent_SetBeatsPerMinute.barAndBeat);
						float num9 = 1f - num;
						float num10 = num8 * num9;
						SetWaveTexture(waveIndex, posXFromBarAndBeat3, num, num9, clip, num10);
						maxWidth = Mathf.Max(posXFromBarAndBeat3 + num10, maxWidth);
						waveIndex++;
					}
				}
				else
				{
					maxBar = Mathf.Max(maxBar, levelEvent.bar);
				}
				if (levelEventControl.tabSection.tab == base.editor.currentTab && levelEventControl.levelEvent.GetType() != typeof(LevelEvent_Base))
				{
					maxUsedY = Mathf.Max(levelEventControl.levelEvent.y, maxUsedY);
				}
			}
			if (base.editor.currentTab == Tab.Decorations)
			{
				maxUsedY = base.tabSection_decorations.decorationHeaders.Count - 2;
			}
			if (base.editor.eventControls.Count > 0)
			{
				maxWidth = Mathf.Max(maxWidth, GetPosXFromBarAndBeat(maxBar, 0f));
			}
			maxWidth += (float)(2 * cellWidth * 8);
			for (; waveIndex < wavesContainer.childCount; waveIndex++)
			{
				UnityEngine.Object.Destroy(wavesContainer.GetChild(waveIndex).gameObject);
			}
			float num11 = center;
			float num12 = centerVert;
			scrollviewContent.sizeDelta = scrollviewContent.sizeDelta.WithX(maxWidth);
			scrollViewVertContent.SizeDeltaY((usedRowCount - scaledRowCellCount) * cellHeight);
			float num13 = scrollbarLinesContainer.rect.width;
			float num14 = num13 / scrollviewContent.sizeDelta.x;
			foreach (tlBookmark bookmark in bookmarks)
			{
				bookmark.lineRect.AnchorPosX(Mathf.Clamp(bookmark.rt.anchoredPosition.x * num14, 0f, num13));
			}
			if (hasZoomed)
			{
				CenterOnPosition(zoomFactors[zoomIndex] / zoomFactors[previousZoomIndex] * num11);
				previousZoomIndex = zoomIndex;
				hasZoomed = false;
			}
			if (hasZoomedVert)
			{
				CenterOnVertPosition(zoomVertFactors[zoomVertIndex] / zoomVertFactors[previousZoomVertIndex] * num12);
				previousZoomVertIndex = zoomVertIndex;
				hasZoomedVert = false;
			}
			int beatCount = Mathf.CeilToInt(maxWidth / (float)cellWidth);
			ResizeGrid(beatCount);
			RectTransform rowsListRect = base.tabSection_rows.rowsListRect;
			RectTransform listRect = base.tabSection_rooms.listRect;
			RectTransform listRect2 = base.tabSection_windows.listRect;
			float y = height - (float)(4 * cellHeight);
			rowsListRect.offsetMin = rowsListRect.offsetMin.WithY(y);
			listRect.offsetMin = listRect.offsetMin.WithY(y);
			listRect2.offsetMin = listRect2.offsetMin.WithY(y);
			base.tabSection_decorations.headersListRect.SizeDeltaY(cellHeight * (base.editor.currentPageDecorationsData.Count + 1));
			foreach (Transform item2 in numbersContainer)
			{
				item2.gameObject.SetActive(value: false);
			}
			int num15 = 0;
			int num16 = 0;
			List<LevelEvent_SetCrotchetsPerBar> list2 = setCrotchetsPerBarEvents;
			barColumns = new List<int>();
			barColumns.Add(0);
			foreach (LevelEvent_SetCrotchetsPerBar item3 in list2)
			{
				if (num15 != 0)
				{
					LevelEvent_SetCrotchetsPerBar levelEvent_SetCrotchetsPerBar = list2[num15 - 1];
					int num17 = item3.bar - levelEvent_SetCrotchetsPerBar.bar;
					CreateNumbers(num16, levelEvent_SetCrotchetsPerBar.bar, num17, levelEvent_SetCrotchetsPerBar.crotchetsPerBar);
					int bar = levelEvent_SetCrotchetsPerBar.bar;
					for (int i = bar; i < bar + num17; i++)
					{
						barColumns.Add(barColumns.Last() + levelEvent_SetCrotchetsPerBar.crotchetsPerBar);
					}
					num16 += num17 * levelEvent_SetCrotchetsPerBar.crotchetsPerBar;
				}
				if (num15 == list2.Count - 1)
				{
					int num18 = GetBarAndBeatWithPosX((int)maxWidth, list2).bar + 2;
					int num19 = num18 - item3.bar;
					CreateNumbers(num16, item3.bar, num19, item3.crotchetsPerBar);
					int bar2 = item3.bar;
					for (int j = bar2; j < bar2 + num19; j++)
					{
						barColumns.Add(barColumns.Last() + item3.crotchetsPerBar);
					}
					for (int k = num18 - 1; k < cachedBarNumbers.Count; k++)
					{
						RectTransform rectTransform = cachedBarNumbers[k];
						rectTransform.anchoredPosition = new Vector2(-100f, rectTransform.anchoredPosition.y);
					}
				}
				num15++;
			}
			if (shouldUpdateUIControls)
			{
				foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
				{
					eventControl.UpdateUI();
				}
			}
			isUpdatingUI = false;
			yield return null;
		}

		private void SetWaveTexture(int index, float quadX, float uvX, float uvWidth, AudioClip clip, float waveWidth)
		{
			GameObject obj = ((index < wavesContainer.childCount) ? wavesContainer.GetChild(index).gameObject : UnityEngine.Object.Instantiate(wavePrefab));
			RectTransform component = obj.GetComponent<RectTransform>();
			component.anchoredPosition = new Vector3(quadX, 0f);
			component.sizeDelta = component.sizeDelta.WithX(waveWidth);
			component.SetParent(wavesContainer, worldPositionStays: false);
			string key = clip.name;
			if (!cachedWaveTextures.ContainsKey(key))
			{
				cachedWaveTextures[key] = AudioSamplesToTexture.AudioClipToTexture(clip);
			}
			float num = (float)Screen.width / 352f;
			float value = waveWidth / uvWidth * num;
			RawImage component2 = obj.GetComponent<RawImage>();
			component2.texture = cachedWaveTextures[key];
			component2.material = UnityEngine.Object.Instantiate(component2.material);
			component2.material.name = "WaveMaterial";
			component2.material.SetFloat("_width", value);
			component2.uvRect = new Rect(uvX, 0f, uvWidth, 1f);
		}

		public void PlayPassiveSoundIfCooldown(string sound, float pitch = 1f)
		{
			if (!soundTriggeredThisFrame && soundTriggerTimer <= 0f)
			{
				scrConductor.PlayImmediately(sound, 1f, RDUtils.GetMixerGroup("LevelEditorPassive"), pitch, 0f, isBeatSound: false, ignoreListenerPause: true);
				soundTriggerTimer = soundTriggerCooldown;
				soundTriggeredThisFrame = true;
			}
		}
	}
}
