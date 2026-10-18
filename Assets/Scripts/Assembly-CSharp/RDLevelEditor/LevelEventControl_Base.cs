using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public abstract class LevelEventControl_Base : RDEditorBase
	{
		[Header("GameObjects")]
		public int paletteColor;

		[NonSerialized]
		public Color initialColor;

		public GameObject go;

		public Image image;

		public Image border;

		public Image conditional;

		public Image tagIndicator;

		public Color anyConditionsColor;

		public Color negatedConditionsColor;

		public Color mixedConditionsColor;

		public GameObject paletteIndicators;

		public Image[] paletteIndicatorImgs;

		public RectTransform leftBorder;

		public RectTransform rightBorder;

		public RectTransform topLeftCorner;

		public Image durationBorder;

		public Image durationFill;

		[Header("Variables")]
		public float selectedAlpha = 0.75f;

		public float focusedAlpha = 0.45f;

		public float deselectedAlpha = 0.35f;

		[NonSerialized]
		public LevelEvent_Base levelEvent = new LevelEvent_Base();

		[NonSerialized]
		public TabSection tabSection;

		[NonSerialized]
		public RectTransform rt;

		[NonSerialized]
		public LevelEventControlEventTrigger trigger;

		[NonSerialized]
		public bool visible = true;

		[NonSerialized]
		public bool selected;

		[NonSerialized]
		public bool isShownAsSelected;

		[NonSerialized]
		public bool shouldUpdateUI;

		[NonSerialized]
		public bool showingBorders;

		public Tab tab => tabSection.tab;

		public int bar
		{
			get
			{
				return levelEvent.bar;
			}
			set
			{
				levelEvent.bar = value;
			}
		}

		public float beat
		{
			get
			{
				return levelEvent.beat;
			}
			set
			{
				levelEvent.beat = value;
			}
		}

		public bool isBase => levelEvent.type == LevelEventType.None;

		public float rightPosition => rt.anchoredPosition.x + rt.sizeDelta.x;

		public float bottomPosition => rt.anchoredPosition.y - rt.sizeDelta.y;

		public Sprite iconSprite => GetIconFromName(levelEvent.name, tryGetPrimary: true);

		public Sprite secondaryIconSprite => GetIconFromName(levelEvent.name + "_Secondary");

		public List<LevelEventControl_Base> container
		{
			get
			{
				int tabIndex = tabSection.tabIndex;
				int row = levelEvent.row;
				return tabIndex switch
				{
					0 => base.editor.eventControls_sounds, 
					2 => base.editor.eventControls_actions, 
					1 => base.editor.eventControls_rows[row], 
					3 => base.editor.eventControls_rooms, 
					4 => base.editor.eventControls_decorations[row], 
					5 => base.editor.eventControls_windows, 
					_ => null, 
				};
			}
		}

		public int visualRowIndex
		{
			get
			{
				int num = 0;
				int num2 = 0;
				int room = base.editor.rowsData[levelEvent.row].room;
				foreach (LevelEvent_MakeRow rowsDatum in base.editor.rowsData)
				{
					if (rowsDatum.room == room)
					{
						if (num == levelEvent.row)
						{
							break;
						}
						num2++;
					}
					num++;
				}
				return num2;
			}
		}

		public void Awake()
		{
			if (image == null)
			{
				image = GetComponent<Image>();
			}
			rt = GetComponent<RectTransform>();
			initialColor = RDConstants.data.colorPalette[paletteColor].WithAlpha(image.color.a);
			go = base.gameObject;
			trigger = GetComponent<LevelEventControlEventTrigger>();
		}

		public void SaveAndUpdateUI()
		{
			SaveData();
			UpdateUI();
		}

		public void SaveData()
		{
			if (base.editor.selectedControls.Count == 1 && base.editor.changingState == 0)
			{
				levelEvent.SaveData();
			}
		}

		public void ShowDataOnInspector()
		{
			if (levelEvent.GetType().IsSubclassOf(typeof(LevelEvent_Base)))
			{
				InspectorPanel inspectorPanel = levelEvent.inspectorPanel;
				if (inspectorPanel != null)
				{
					inspectorPanel.Show(this);
				}
				return;
			}
			base.editor.conditionalsPanel.gameObject.SetActive(value: false);
			if (tab == Tab.Decorations)
			{
				LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase = base.editor.decorationsData[levelEvent.row];
				EventCategory categoryToShow = EventCategory.SpriteDecoration;
				if (levelEvent_MakeDecorationBase is LevelEvent_Sprite)
				{
					categoryToShow = EventCategory.SpriteDecoration;
				}
				else if (levelEvent_MakeDecorationBase is LevelEvent_Text)
				{
					categoryToShow = EventCategory.TextDecoration;
				}
				base.editor.ShowLevelEventSelectorPanel(filterMode: false, categoryToShow);
			}
			else
			{
				base.editor.ShowLevelEventSelectorPanel();
			}
		}

		public void UpdateUI()
		{
			shouldUpdateUI = true;
		}

		public void UpdateDurationBar(bool show)
		{
			if (!(durationBorder == null))
			{
				bool flag = false;
				if (show && levelEvent is IDurationHaver durationHaver)
				{
					float num = durationHaver.duration * (float)base.cellWidth - image.rectTransform.sizeDelta.x + 1f;
					durationBorder.rectTransform.SizeDeltaX(num);
					flag = num > 1f;
				}
				durationBorder.gameObject.SetActive(flag);
				if (flag)
				{
					durationBorder.color = image.color.WithAlpha(durationFill.color.a);
					durationFill.color = image.color.WithAlpha(durationFill.color.a);
				}
			}
		}

		public virtual void UpdateUIInternal()
		{
			this.image.DOKill(complete: true);
			if (base.editor.selectedControls.Contains(this))
			{
				ShowAsSelected();
			}
			else
			{
				ShowAsDeselected();
			}
			if (conditional != null)
			{
				bool flag = levelEvent.HasAnyConditionals();
				if (flag)
				{
					Color color = anyConditionsColor;
					bool flag2 = levelEvent.HasPositiveConditionals();
					bool flag3 = levelEvent.HasNegativeConditionals();
					if (flag2 & flag3)
					{
						color = mixedConditionsColor;
					}
					else if (flag2 && !flag3)
					{
						color = anyConditionsColor;
					}
					else if (!flag2 & flag3)
					{
						color = negatedConditionsColor;
					}
					conditional.color = color;
				}
				conditional.gameObject.SetActive(flag);
			}
			if (tagIndicator != null)
			{
				bool active = levelEvent.tag != null && levelEvent.tag != "";
				tagIndicator.color = (levelEvent.tagRunNormally ? base.gc.eventTagRunStripColor : base.gc.eventTagStripColor);
				tagIndicator.gameObject.SetActive(active);
			}
			if (paletteIndicators != null)
			{
				List<Color> paletteColorsUsed = levelEvent.GetPaletteColorsUsed();
				bool flag4 = paletteColorsUsed.Count > 0;
				paletteIndicators.SetActive(flag4);
				if (flag4)
				{
					int count = paletteColorsUsed.Count;
					int num = paletteIndicatorImgs.Length;
					if (count > num)
					{
						RDBaseDllDummy.printesw("this event has too many colors???");
					}
					else
					{
						int num2 = count - num;
						for (int i = num2; i < count; i++)
						{
							Image image = paletteIndicatorImgs[num - (i - num2) - 1];
							if (i < 0)
							{
								image.color = Color.clear;
							}
							else
							{
								image.color = paletteColorsUsed[i];
							}
						}
					}
				}
			}
			shouldUpdateUI = false;
		}

		public virtual void ShowAsSelected(bool notBeingActuallySelected = false)
		{
			selected = true;
			Color color = (levelEvent.active ? initialColor : Color.gray).WithAlpha(selectedAlpha);
			image.color = color;
			border.color = Color.white;
			Color color2 = Color.gray.WithAlpha(selectedAlpha);
			if (levelEvent is LevelEvent_AddClassicBeat || levelEvent is LevelEvent_AddFreeTimeBeat || levelEvent is LevelEvent_PulseFreeTimeBeat)
			{
				LevelEventControl_AddClassicBeat levelEventControl_AddClassicBeat = this as LevelEventControl_AddClassicBeat;
				levelEventControl_AddClassicBeat.heldbeatFill.color = (levelEvent.active ? levelEventControl_AddClassicBeat.heldbeatFillColor : color2);
				levelEventControl_AddClassicBeat.heldbeatBorder.color = (levelEvent.active ? levelEventControl_AddClassicBeat.heldbeatBorderColor : color2);
			}
			else if (levelEvent is LevelEvent_AddOneshotBeat)
			{
				LevelEventControl_AddClassicBeat levelEventControl_AddClassicBeat2 = this as LevelEventControl_AddClassicBeat;
				levelEventControl_AddClassicBeat2.skipshotFill.color = (levelEvent.active ? levelEventControl_AddClassicBeat2.skipshotFillColor : color2);
				levelEventControl_AddClassicBeat2.skipshotBorder.color = (levelEvent.active ? levelEventControl_AddClassicBeat2.skipshotBorderColor : color2);
				levelEventControl_AddClassicBeat2.subdivFill.color = (levelEvent.active ? levelEventControl_AddClassicBeat2.subdivFillColor : color2);
				levelEventControl_AddClassicBeat2.subdivBorder.color = (levelEvent.active ? levelEventControl_AddClassicBeat2.subdivBorderColor : color2);
				levelEventControl_AddClassicBeat2.heldbeatFill.color = (levelEvent.active ? levelEventControl_AddClassicBeat2.heldbeatFillColor : color2);
				levelEventControl_AddClassicBeat2.heldbeatBorder.color = (levelEvent.active ? levelEventControl_AddClassicBeat2.heldbeatBorderColor : color2);
			}
			if (notBeingActuallySelected)
			{
				isShownAsSelected = true;
			}
			else
			{
				base.transform.SetAsLastSibling();
			}
			UpdateDurationBar(show: true);
		}

		public virtual void ShowAsDeselected()
		{
			selected = false;
			isShownAsSelected = false;
			image.DOKill(complete: true);
			Color color = (levelEvent.active ? initialColor : Color.gray).WithAlpha(deselectedAlpha);
			image.color = color;
			if (levelEvent is LevelEvent_AddClassicBeat || levelEvent is LevelEvent_AddFreeTimeBeat || levelEvent is LevelEvent_PulseFreeTimeBeat)
			{
				LevelEventControl_AddClassicBeat levelEventControl_AddClassicBeat = this as LevelEventControl_AddClassicBeat;
				Color color2 = Color.gray.WithAlpha(deselectedAlpha);
				levelEventControl_AddClassicBeat.heldbeatFill.color = (levelEvent.active ? levelEventControl_AddClassicBeat.heldbeatFillColor.WithAlpha(0.5f) : color2);
				levelEventControl_AddClassicBeat.heldbeatBorder.color = (levelEvent.active ? levelEventControl_AddClassicBeat.heldbeatBorderColor.WithAlpha(0.5f) : color2);
			}
			if (levelEvent.active)
			{
				Color.RGBToHSV(image.color, out var H, out var S, out var V);
				S = 1f / 17f;
				V *= 200f / 255f;
				border.color = Color.HSVToRGB(H, S, V);
			}
			else
			{
				border.color = Color.gray;
			}
			UpdateDurationBar(show: false);
		}

		public static Sprite GetIconFromName(string levelEventName, bool tryGetPrimary = false)
		{
			if (tryGetPrimary)
			{
				Sprite sprite = Resources.Load<Sprite>("LevelEventIcons/" + levelEventName + "_Primary");
				if (sprite != null)
				{
					return sprite;
				}
			}
			Sprite sprite2 = Resources.Load<Sprite>("LevelEventIcons/" + levelEventName);
			if (sprite2 == null)
			{
				sprite2 = Resources.Load<Sprite>("LevelEventIcons/Default");
			}
			return sprite2;
		}

		public virtual void ShowBorders(bool show)
		{
			showingBorders = show;
			if ((bool)leftBorder)
			{
				leftBorder.gameObject.SetActive(show);
			}
			if ((bool)rightBorder)
			{
				rightBorder.gameObject.SetActive(show);
			}
			if ((bool)topLeftCorner)
			{
				topLeftCorner.gameObject.SetActive(show);
			}
		}

		public virtual void OnDelete()
		{
		}

		public virtual void OnDoubleClick()
		{
		}

		public void SnapBeatToGrid()
		{
			if (levelEvent.usesBeat)
			{
				float num = 1f / (float)base.editor.denominator;
				beat = Mathf.Round(beat / num) * num;
			}
		}

		public void SetPosWithDecorationTarget()
		{
			RectTransform component = GetComponent<RectTransform>();
			Vector2 anchoredPosition = component.anchoredPosition;
			anchoredPosition.x = base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(bar, beat));
			if (levelEvent.type == LevelEventType.None)
			{
				anchoredPosition.y = base.timeline.GetPosYFromRowIndex(levelEvent.y);
			}
			else
			{
				int num = 0;
				int num2 = -1;
				LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase = null;
				foreach (LevelEvent_MakeDecorationBase decorationsDatum in base.editor.decorationsData)
				{
					if (decorationsDatum.decorationId == levelEvent.target)
					{
						num2 = decorationsDatum.room;
						levelEvent_MakeDecorationBase = decorationsDatum;
					}
					num++;
				}
				if (num2 != -1)
				{
					int num3 = 0;
					foreach (LevelEvent_MakeDecorationBase decorationsDatum2 in base.editor.decorationsData)
					{
						if (num2 == decorationsDatum2.room)
						{
							if (levelEvent_MakeDecorationBase == decorationsDatum2)
							{
								anchoredPosition.y = base.timeline.GetPosYFromRowIndex(num3);
								break;
							}
							num3++;
						}
					}
				}
			}
			component.anchoredPosition = anchoredPosition;
			component.sizeDelta = new Vector2(Mathf.Min(14, base.cellWidth), base.cellHeight);
		}
	}
}
