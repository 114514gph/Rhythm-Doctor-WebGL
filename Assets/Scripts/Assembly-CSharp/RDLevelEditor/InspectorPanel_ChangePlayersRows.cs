using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_ChangePlayersRows : InspectorPanel
	{
		public Dropdown playerMode;

		public Text noRowsDescription;

		public GameObject rowsContainer;

		public RowAndPlayer[] rows;

		private List<ToggleGroup> toggleGroups;

		private List<CharacterPicker> cpuCharacterPickers;

		private new void Awake()
		{
			base.Awake();
			toggleGroups = new List<ToggleGroup>();
			cpuCharacterPickers = new List<CharacterPicker>();
			int num = 0;
			RowAndPlayer[] array = rows;
			foreach (RowAndPlayer rowAndPlayer in array)
			{
				ToggleGroup toggleGroup = rowAndPlayer.toggleGroup;
				CharacterPicker cpuCharacterPicker = rowAndPlayer.cpuCharacterPicker;
				AddOnEditListeners(null, toggleGroup, cpuCharacterPicker);
				toggleGroups.Add(toggleGroup);
				cpuCharacterPickers.Add(cpuCharacterPicker);
				cpuCharacterPicker.availableCharacters = RDEditorConstants.AvailableCPUCharacters;
				cpuCharacterPicker.onEndEdit = (CharacterPicker.EndHandler)Delegate.Combine(cpuCharacterPicker.onEndEdit, (CharacterPicker.EndHandler)((Character character, string expression, string customCharName) =>
				{
					if (character == Character.Otto)
					{
						base.editor.ShowOttoHeart();
					}
				}));
				RectTransform component = rowAndPlayer.GetComponent<RectTransform>();
				component.anchoredPosition = component.anchoredPosition.WithY((float)num * -21f - 1f);
				num++;
			}
			playerMode.AddOptionsWithEnumValues<PlayerMode>();
			AddOnEditListeners(null, playerMode);
		}

		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_ChangePlayersRows levelEvent_ChangePlayersRows = (LevelEvent_ChangePlayersRows)levelEvent;
			int count = base.editor.eventControls_rows.Count;
			bool flag = count == 0;
			noRowsDescription.gameObject.SetActive(flag);
			rowsContainer.gameObject.SetActive(!flag);
			playerMode.transform.parent.gameObject.SetActive(!flag);
			if (flag)
			{
				base.content.sizeDelta = base.content.sizeDelta.WithY(100f);
				return;
			}
			for (int i = 0; i < rows.Length; i++)
			{
				bool flag2 = i < count;
				rows[i].gameObject.SetActive(flag2);
				if (flag2)
				{
					LevelEvent_MakeRow levelEvent_MakeRow = base.editor.rowsData[i];
					rows[i].text.text = levelEvent_MakeRow.GetRowString(shortName: true);
				}
				else
				{
					rows[i].text.text = "";
				}
				if (levelEvent_ChangePlayersRows.players[i] == RDPlayer.NoChange)
				{
					toggleGroups[i].SetAllTogglesOff();
				}
				else
				{
					toggleGroups[i].SetValue(levelEvent_ChangePlayersRows.players[i].ToString());
				}
				cpuCharacterPickers[i].SetCharacter(levelEvent_ChangePlayersRows.cpuMarkers[i]);
				rows[i].UpdateCPUMarkerImage();
			}
			base.content.sizeDelta = base.content.sizeDelta.WithY(67f + (float)count * 21f);
			playerMode.value = ((levelEvent_ChangePlayersRows.playerMode != PlayerMode.OnePlayer) ? ((levelEvent_ChangePlayersRows.playerMode == PlayerMode.TwoPlayers) ? 1 : 2) : 0);
		}

		protected override void SaveInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_ChangePlayersRows levelEvent_ChangePlayersRows = (LevelEvent_ChangePlayersRows)levelEvent;
			for (int i = 0; i < base.editor.eventControls_rows.Count; i++)
			{
				if (!rowsContainer.activeSelf)
				{
					break;
				}
				ToggleGroup toggleGroup = toggleGroups[i];
				bool flag = toggleGroup.GetIndexFromActiveToggle() == -1;
				levelEvent_ChangePlayersRows.players[i] = (flag ? RDPlayer.NoChange : toggleGroup.GetEnumValueFromActiveToggle(RDPlayer.P1));
				levelEvent_ChangePlayersRows.cpuMarkers[i] = cpuCharacterPickers[i].character;
			}
			levelEvent_ChangePlayersRows.playerMode = ((playerMode.value != 0) ? ((playerMode.value == 1) ? PlayerMode.TwoPlayers : PlayerMode.OneOrTwoPlayers) : PlayerMode.OnePlayer);
			UpdateUIInternal(levelEvent);
		}
	}
}
