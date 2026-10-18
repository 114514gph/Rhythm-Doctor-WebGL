using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class DecorationHeader : RDEditorBase
	{
		public int index;

		public static int decorationTabWhereAddButtonWasPressed = -1;

		public static int decorationTabToCreateEventIn = -1;

		[Header("Objects")]
		public Image background;

		public Button addButton;

		public Text rowNumberText;

		public RawImage spriteImage;

		public Sprite normalBackground;

		public Sprite checkeredBackground;

		private Color selectedColor = "A8E3C3".HexToColor().WithAlpha(174f / 255f);

		private Color unselectedColor = Color.black.WithAlpha(60f / 255f);

		public void Awake()
		{
		}

		public void AddButtonClick()
		{
			base.editor.DeselectAllEventControls();
			LevelEditorPlaySound("sndEditorPanelCreate", "LevelEditorActive");
			base.editor.ShowLevelEventSelectorPanel(filterMode: false, EventCategory.MakeDecoration);
			base.editor.selectedDecoration = null;
			decorationTabWhereAddButtonWasPressed = base.editor.selectedDecorationsTabPageIndex;
			decorationTabToCreateEventIn = base.editor.selectedDecorationsTabPageIndex;
			base.tabSection_decorations.UpdateUI();
		}

		public void ShowButtonClick()
		{
			Select();
		}

		public void Select()
		{
			LevelEvent_MakeDecorationBase decorationData = GetDecorationData(index, base.editor.selectedDecorationsTabPageIndex);
			if (decorationData != null)
			{
				ShowPanel(decorationData.decorationId);
			}
		}

		public static void ShowPanel(string decorationId)
		{
			scnEditor instance = scnEditor.instance;
			LevelEvent_MakeDecorationBase decorationData = GetDecorationData(decorationId);
			InspectorPanel inspectorPanel = decorationData.inspectorPanel;
			InspectorPanel.ShowPanel(inspectorPanel.transform, RDString.Get("editor.MakeDecoration"), Tab.Decorations);
			instance.selectedDecoration = decorationId;
			if (decorationData != null)
			{
				inspectorPanel.currentLevelEvent = decorationData;
				inspectorPanel.UpdateUI(inspectorPanel.currentLevelEvent);
				((TabSection_Decorations)instance.tabSections[4]).UpdateUI();
			}
			else
			{
				RDBaseDllDummy.printem("it's null!");
			}
		}

		public void UpdateUI(LevelEvent_MakeDecorationBase decorationData, bool isActive, bool isNextToLastDecoration)
		{
			int num = base.editor.decorationsData.IndexOf(decorationData) + 1;
			bool flag = false;
			bool flag2 = decorationTabWhereAddButtonWasPressed == base.editor.selectedDecorationsTabPageIndex;
			if (isActive)
			{
				if (decorationData is LevelEvent_Sprite levelEvent_Sprite)
				{
					spriteImage.enabled = false;
					background.enabled = true;
					background.sprite = (levelEvent_Sprite.visible ? normalBackground : checkeredBackground);
					string text = "Beans";
					string text2 = ((levelEvent_Sprite.character != Character.Custom) ? RDString.Get($"enum.Character.{levelEvent_Sprite.character}.short") : (levelEvent_Sprite.filename.IsNullOrEmpty() ? text : levelEvent_Sprite.filename));
					string arg = text2;
					Dictionary<string, CustomAnimationData> customCharacterData = base.editor.customCharacterData;
					if (!customCharacterData.ContainsKey(text) && text2 == text)
					{
						levelEvent_Sprite.UpdateCustomCharacter(justReturnErrors: false, editorRowHeader: true);
					}
					if (levelEvent_Sprite.character == Character.Custom && !customCharacterData.ContainsKey(text2))
					{
						RDBaseDllDummy.printem("Custom character data for " + text2 + " hasn't been loaded.");
					}
					else
					{
						CustomAnimationData customAnimationData = ((levelEvent_Sprite.character != Character.Custom) ? scrChar.baseCharacterAnimations[levelEvent_Sprite.character] : customCharacterData[text2]);
						if (levelEvent_Sprite.preview)
						{
							Texture2D mainTexture = customAnimationData.mainTexture;
							List<Vector2> uVsForSheetFrame = customAnimationData.GetUVsForSheetFrame(customAnimationData.rowPreviewFrame);
							spriteImage.texture = customAnimationData.mainTexture;
							float num2 = customAnimationData.rowPreviewOffset.x * 1f / (float)mainTexture.width;
							float num3 = customAnimationData.rowPreviewOffset.y * 1f / (float)mainTexture.height;
							float width = (float)customAnimationData.spriteSize.x * 1f / (float)mainTexture.width;
							float height = (float)customAnimationData.spriteSize.y * 1f / (float)mainTexture.height;
							spriteImage.uvRect = new Rect(uVsForSheetFrame[0].x + num2, uVsForSheetFrame[0].y + num3, width, height);
							spriteImage.enabled = true;
							spriteImage.rectTransform.sizeDelta = customAnimationData.spriteSize;
							float x = ((customAnimationData.spriteSize.x % 2 == 0) ? 0f : (-0.5f));
							float y = ((customAnimationData.spriteSize.y % 2 == 0) ? 0f : (-0.5f));
							spriteImage.rectTransform.anchoredPosition = new Vector2(x, y);
							if (customAnimationData.rowPreviewScale == 0f)
							{
								if (customAnimationData.spriteSize.y > 48)
								{
									spriteImage.rectTransform.sizeDelta /= (float)customAnimationData.spriteSize.y / 48f;
								}
							}
							else
							{
								spriteImage.rectTransform.sizeDelta *= customAnimationData.rowPreviewScale;
							}
							arg = "";
						}
					}
					spriteImage.material.SetInt("_HasPalette", 0);
					rowNumberText.text = $"{num}. {arg}";
					rowNumberText.enabled = true;
					flag = true;
				}
				else if (decorationData is LevelEvent_Text levelEvent_Text)
				{
					string text3 = (levelEvent_Text.decoName.IsNullOrEmpty() ? RDString.Get("editor.Text.decorationHeaderName") : levelEvent_Text.decoName);
					rowNumberText.text = num + ". " + text3;
				}
			}
			else
			{
				background.enabled = flag2;
				rowNumberText.enabled = flag2;
				if (flag2)
				{
					rowNumberText.text = RDString.Get("editor.MakeDecoration.decorationHeaderName");
				}
			}
			base.gameObject.SetActive(isActive | isNextToLastDecoration);
			rowNumberText.gameObject.SetActive(isActive | flag2);
			addButton.gameObject.SetActive(isNextToLastDecoration && !flag2);
			spriteImage.gameObject.SetActive(isActive & flag);
			UpdateBackgroundColor(decorationData);
		}

		public void UpdateBackgroundColor(LevelEvent_MakeDecorationBase decorationData)
		{
			bool flag = (decorationData != null && base.editor.lastUsedDecoration == decorationData.decorationId) || (decorationData == null && decorationTabWhereAddButtonWasPressed == base.editor.selectedDecorationsTabPageIndex);
			background.color = (flag ? selectedColor : unselectedColor);
		}

		public static int GetDecorationDataIndex(int decorationHeaderIndex, int page)
		{
			int num = 0;
			int num2 = 0;
			foreach (LevelEvent_MakeDecorationBase decorationsDatum in scnEditor.instance.decorationsData)
			{
				if (decorationsDatum.room == page)
				{
					if (decorationHeaderIndex == num2)
					{
						break;
					}
					num2++;
				}
				num++;
			}
			return num;
		}

		public static int GetDecorationDataIndex(string decorationId)
		{
			int num = 0;
			foreach (LevelEvent_MakeDecorationBase decorationsDatum in scnEditor.instance.decorationsData)
			{
				if (decorationsDatum.decorationId == decorationId)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public static LevelEvent_MakeDecorationBase GetDecorationData(int decorationHeaderIndex, int page)
		{
			int num = 0;
			foreach (LevelEvent_MakeDecorationBase decorationsDatum in scnEditor.instance.decorationsData)
			{
				if (decorationsDatum.room == page)
				{
					if (decorationHeaderIndex == num)
					{
						return decorationsDatum;
					}
					num++;
				}
			}
			return null;
		}

		public static LevelEvent_MakeDecorationBase GetDecorationData(string decorationId)
		{
			foreach (LevelEvent_MakeDecorationBase decorationsDatum in scnEditor.instance.decorationsData)
			{
				if (decorationsDatum.decorationId == decorationId)
				{
					return decorationsDatum;
				}
			}
			return null;
		}
	}
}
