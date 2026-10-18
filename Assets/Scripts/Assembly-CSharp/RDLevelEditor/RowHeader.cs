using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class RowHeader : RDEditorBase
	{
		public int index;

		[Header("Objects")]
		public Image background;

		public Button addButton;

		public Text rowNumberText;

		public RawImage characterImage;

		public Image rowTypeIcon;

		public Image muteBeats;

		public Sprite classicIcon;

		public Sprite oneshotIcon;

		public Sprite normalBackground;

		public Sprite checkeredBackground;

		public void Awake()
		{
			characterImage.material = new Material(base.gc.UIPaletteMaterial);
		}

		public void AddButtonClick()
		{
			LevelEvent_MakeRow levelEvent_MakeRow = new LevelEvent_MakeRow();
			levelEvent_MakeRow.rooms = new int[1] { base.editor.selectedRowsTabPageIndex };
			levelEvent_MakeRow.character = RDEditorConstants.AvailableRandomCharacters.GetRandomItem();
			string text = RDUtils.GetDefaultPulseSoundForCharacter(levelEvent_MakeRow.character, removePrefix: true);
			if (text.IsNullOrEmpty())
			{
				text = RDEditorConstants.BeatSounds.GetRandomItem().ToString();
				if (text == "None")
				{
					text = "Shaker";
				}
			}
			levelEvent_MakeRow.pulseSound = new SoundData(text, "pulseSound", "pulseSoundVolume", "pulseSoundPitch", "pulseSoundPan", "pulseSoundOffset");
			base.editor.DeselectAllEventControls();
			base.editor.AddNewRow(levelEvent_MakeRow);
			base.tabSection_rows.UpdateUI();
			LevelEditorPlaySound("sndEditorPanelCreate", "LevelEditorActive");
			Select();
		}

		public void ShowButtonClick()
		{
			Select();
		}

		public void Select()
		{
			ShowPanel(GetRowDataIndex(index, base.editor.selectedRowsTabPageIndex));
		}

		public void MuteBeatsClick()
		{
			int rowDataIndex = GetRowDataIndex(index, base.editor.selectedRowsTabPageIndex);
			LevelEvent_MakeRow levelEvent_MakeRow = base.editor.rowsData[rowDataIndex];
			levelEvent_MakeRow.muteBeats = !levelEvent_MakeRow.muteBeats;
			muteBeats.enabled = levelEvent_MakeRow.muteBeats;
		}

		public static void ShowPanel(int rowIndex)
		{
			scnEditor instance = scnEditor.instance;
			InspectorPanel_MakeRow inspectorPanel_MakeRow = instance.inspectorPanelManager.Get<InspectorPanel_MakeRow>();
			InspectorPanel.ShowPanel(inspectorPanel_MakeRow.transform, RDString.Get("editor.MakeRow"), Tab.Rows);
			instance.selectedRowIndex = rowIndex;
			instance.lastUsedRow = rowIndex;
			if (rowIndex < instance.rowsData.Count)
			{
				inspectorPanel_MakeRow.currentLevelEvent = instance.rowsData[rowIndex];
				inspectorPanel_MakeRow.UpdateUI(inspectorPanel_MakeRow.currentLevelEvent);
				((TabSection_Rows)instance.tabSections[1]).UpdateUI();
			}
		}

		public void UpdateUI(LevelEvent_MakeRow rowData, bool isActive, bool isNextToLastRow)
		{
			int num = base.editor.rowsData.IndexOf(rowData) + 1;
			if (isActive)
			{
				muteBeats.enabled = rowData.muteBeats;
				characterImage.enabled = false;
				background.enabled = true;
				rowTypeIcon.enabled = true;
				rowTypeIcon.sprite = ((rowData.rowType == RowType.Oneshot) ? oneshotIcon : classicIcon);
				background.sprite = (rowData.hideAtStart ? checkeredBackground : normalBackground);
				Character character = rowData.character;
				string text = ((rowData.character == Character.Custom) ? rowData.customCharacterName : rowData.character.ToString());
				string text2 = text;
				bool flag = true;
				CustomAnimationData value;
				if (character == Character.Custom)
				{
					if (!base.editor.customCharacterData.TryGetValue(text, out value))
					{
						RDBaseDllDummy.printem("Custom character data for " + text + " hasn't been loaded.");
						flag = false;
					}
				}
				else
				{
					value = scrChar.baseCharacterAnimations[character];
				}
				if (flag)
				{
					Texture2D mainTexture = value.mainTexture;
					List<Vector2> uVsForSheetFrame = value.GetUVsForSheetFrame(value.rowPreviewFrame);
					characterImage.texture = value.mainTexture;
					float num2 = value.rowPreviewOffset.x * 1f / (float)mainTexture.width;
					float num3 = value.rowPreviewOffset.y * 1f / (float)mainTexture.height;
					float width = (float)value.spriteSize.x * 1f / (float)mainTexture.width;
					float height = (float)value.spriteSize.y * 1f / (float)mainTexture.height;
					characterImage.uvRect = new Rect(uVsForSheetFrame[0].x + num2, uVsForSheetFrame[0].y + num3, width, height);
					characterImage.enabled = true;
					characterImage.rectTransform.sizeDelta = value.spriteSize;
					float x = ((value.spriteSize.x % 2 == 0) ? 0f : (-0.5f));
					float y = ((value.spriteSize.y % 2 == 0) ? 0f : (-0.5f));
					characterImage.rectTransform.anchoredPosition = new Vector2(x, y);
					if (value.rowPreviewScale == 0f)
					{
						if (value.spriteSize.y > 48)
						{
							characterImage.rectTransform.sizeDelta /= (float)value.spriteSize.y / 48f;
						}
					}
					else
					{
						characterImage.rectTransform.sizeDelta *= value.rowPreviewScale;
					}
					text2 = "";
				}
				characterImage.material.SetInt("_HasPalette", 0);
				rowNumberText.text = num + ".";
				if (!base.editor.scrubbing && (bool)base.game && base.game.rows[num - 1].isShadowRealtime)
				{
					rowNumberText.text += $"({base.game.rows[num - 1].shadowOrHostIDRealtime + 1})";
				}
				else if (rowData.mimicsRow)
				{
					rowNumberText.text += $"({rowData.rowToMimic + 1})";
				}
				Text text3 = rowNumberText;
				text3.text = text3.text + " " + text2;
				rowNumberText.enabled = true;
			}
			else
			{
				background.enabled = false;
				rowNumberText.enabled = false;
				rowTypeIcon.enabled = false;
				muteBeats.enabled = false;
			}
			if (isActive)
			{
				Color color = "00cdff".HexToColor();
				Color color2 = "ff0000".HexToColor();
				Color color3 = "808080".HexToColor();
				if (GC.twoPlayerMode)
				{
					if (rowData.player == RDPlayer.P1)
					{
						rowNumberText.color = color;
					}
					else if (rowData.player == RDPlayer.P2)
					{
						rowNumberText.color = color2;
					}
					else
					{
						rowNumberText.color = color3;
					}
				}
				else if (rowData.player == RDPlayer.CPU)
				{
					rowNumberText.color = color3;
				}
				else
				{
					rowNumberText.color = Color.white;
				}
			}
			base.gameObject.SetActive(isActive | isNextToLastRow);
			rowNumberText.gameObject.SetActive(isActive);
			addButton.gameObject.SetActive(isNextToLastRow);
			characterImage.gameObject.SetActive(isActive);
			UpdateBackgroundColor(rowData);
		}

		public void UpdateBackgroundColor(LevelEvent_MakeRow rowData)
		{
			bool flag = rowData != null && base.editor.lastUsedRow == base.editor.rowsData.IndexOf(rowData);
			background.color = (flag ? "00D5FF".HexToColor().WithAlpha(174f / 255f) : Color.black.WithAlpha(60f / 255f));
		}

		public static int GetRowDataIndex(int rowHeaderIndex, int page)
		{
			int num = 0;
			int num2 = 0;
			foreach (LevelEvent_MakeRow rowsDatum in scnEditor.instance.rowsData)
			{
				if (rowsDatum.room == page)
				{
					if (rowHeaderIndex == num2)
					{
						break;
					}
					num2++;
				}
				num++;
			}
			return num;
		}

		public static int CharacterToSpriteId(Character character)
		{
			RDConstants data = RDConstants.data;
			_ = data.collection_char;
			tk2dSpriteAnimation animations_char = data.animations_char;
			string text = character.ToString();
			string expressionPassed = GC.characterExpressions[character].expressionPassed;
			string text2 = text + "_" + expressionPassed;
			tk2dSpriteAnimationClip clipByName = animations_char.GetClipByName(text2);
			if (clipByName == null)
			{
				return -1;
			}
			return clipByName.frames[^1].spriteId;
		}
	}
}
