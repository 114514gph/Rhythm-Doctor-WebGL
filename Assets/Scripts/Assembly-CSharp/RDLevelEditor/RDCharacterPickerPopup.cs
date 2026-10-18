using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class RDCharacterPickerPopup : RDEditorBase, IPointerExitHandler, IEventSystemHandler
	{
		public GameObject buttonPrefab;

		public Transform charactersContainer;

		public Text characterText;

		public Text characterInfo;

		public RectTransform panel;

		private CharacterButton[] characterButtons;

		private bool expressionsPage;

		private bool returnsExpression;

		private Character selectedCharacter;

		private string selectedCustomCharacter;

		private CharacterPicker characterPicker;

		private const float bpm = 100f;

		private float time;

		private int curBeat;

		private void Start()
		{
		}

		private void ClearButtons()
		{
			if (characterButtons != null)
			{
				CharacterButton[] array = characterButtons;
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i].gameObject);
				}
			}
			characterButtons = new CharacterButton[0];
			characterText.text = "";
		}

		public void ShowCharacters(CharacterPicker characterPicker, Character[] characters, bool returnsExpression = false, bool includeCustomChars = true, Character[] specialCharacters = null)
		{
			ShowWindow();
			ClearButtons();
			if (specialCharacters == null)
			{
				specialCharacters = Array.Empty<Character>();
			}
			List<string> list = new List<string>();
			string[] array = (includeCustomChars ? Directory.GetDirectories(LevelValidation.CustomCharactersPath) : Array.Empty<string>());
			foreach (string obj in array)
			{
				string text = obj.Split(Path.DirectorySeparatorChar)[^1];
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				if (RDFile.Exists(obj + directorySeparatorChar + text + ".json"))
				{
					list.Add(text);
				}
			}
			int num = characters.Length;
			int num2 = specialCharacters.Length;
			int count = list.Count;
			characterButtons = new CharacterButton[num + count + num2];
			for (int j = 0; j < characterButtons.Length; j++)
			{
				CharacterButton component = UnityEngine.Object.Instantiate(buttonPrefab, charactersContainer).GetComponent<CharacterButton>();
				int num3 = j;
				if (num3 >= 0 && num3 < num)
				{
					component.Setup(characters[num3]);
				}
				num3 -= num;
				if (num3 >= 0 && num3 < count)
				{
					component.Setup(Character.Custom, list[num3]);
				}
				num3 -= count;
				if (num3 >= 0 && num3 < num2)
				{
					component.SetupSpecial(specialCharacters[num3]);
				}
				num3 -= num2;
				component.xPositionInGrid = j % 6;
				component.yPositionInGrid = j / 6;
				characterButtons[j] = component;
			}
			this.characterPicker = characterPicker;
			this.returnsExpression = returnsExpression;
		}

		private CharacterButton CreateCharacterButton()
		{
			return UnityEngine.Object.Instantiate(buttonPrefab, charactersContainer).GetComponent<CharacterButton>();
		}

		public void ShowExpressions(Character character, string customCharacter = null, bool showWindow = false, bool endWithNone = false, bool includeNew = false)
		{
			if (showWindow)
			{
				ShowWindow();
			}
			ClearButtons();
			bool num = character == Character.Custom;
			CharacterButton component = UnityEngine.Object.Instantiate(buttonPrefab, charactersContainer).GetComponent<CharacterButton>();
			Dictionary<string, CustomAnimationClip>.KeyCollection keys;
			if (!num)
			{
				component.Setup(character, null, "neutral");
				keys = scrChar.baseCharacterAnimations[character].clips.Keys;
			}
			else
			{
				component.Setup(Character.Custom, customCharacter, "neutral");
				keys = base.editor.customCharacterData[customCharacter].clips.Keys;
				Debug.Log("SET UP " + customCharacter);
			}
			int num2 = 0;
			if (endWithNone)
			{
				num2++;
			}
			if (includeNew)
			{
				num2++;
			}
			int num3 = keys.Count + num2;
			characterButtons = new CharacterButton[num3];
			characterButtons[0] = component;
			int num4 = 1;
			foreach (string item in keys)
			{
				if (!(item == "neutral"))
				{
					if (num4 >= keys.Count)
					{
						Debug.Log("what how");
						break;
					}
					CharacterButton component2 = UnityEngine.Object.Instantiate(buttonPrefab, charactersContainer).GetComponent<CharacterButton>();
					component2.Setup(character, customCharacter, item);
					characterButtons[num4] = component2;
					num4++;
				}
			}
			if (endWithNone)
			{
				CharacterButton component3 = UnityEngine.Object.Instantiate(buttonPrefab, charactersContainer).GetComponent<CharacterButton>();
				component3.Setup(Character.None, null, "none");
				characterButtons[num3 - num2] = component3;
			}
			if (includeNew)
			{
				CharacterButton component4 = UnityEngine.Object.Instantiate(buttonPrefab, charactersContainer).GetComponent<CharacterButton>();
				component4.Setup(Character.New, null, RDString.Get("editor.custom"));
				characterButtons[num3 - 1] = component4;
			}
		}

		public void SelectCharacter(Character character, string customCharacter = null)
		{
			selectedCharacter = character;
			selectedCustomCharacter = customCharacter;
			if (returnsExpression)
			{
				ShowExpressions(character, customCharacter);
				return;
			}
			characterPicker.SetCharacter(character, customCharacter);
			characterPicker.onEndEdit(character, null, customCharacter);
			Hide();
		}

		public void SelectExpression(string expression)
		{
			characterPicker.SetExpression(expression);
			characterPicker.onEndEdit(selectedCharacter, expression, selectedCustomCharacter);
			Hide();
		}

		public void SelectExpressionCustom(string expression)
		{
			characterPicker.SetCustomExpression(expression);
			characterPicker.onEndEdit(Character.Custom, expression, selectedCustomCharacter);
		}

		public void SwitchToInput()
		{
			characterPicker.EnableCustomInput(enable: true);
			Hide();
		}

		private void ShowWindow()
		{
			ClearCharacterInfo();
			base.gameObject.SetActive(value: true);
			base.editor.popupBackground.SetActive(value: true);
			LevelEditorPlaySound("sndEditorPopupLarge", "LevelEditorInspectorPanel");
			panel.DOKill(complete: true);
			panel.localScale = Vector3.zero;
			panel.DOScale(Vector3.one, base.gc.roomsSelectionPopup_showAnimDuration).SetEase(base.gc.roomsSelectionPopup_showAnimEase).SetUpdate(UpdateType.Normal, isIndependentUpdate: true)
				.SetId("noKillOnReload");
		}

		public void Hide()
		{
			base.editor.popupBackground.SetActive(value: false);
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
				LevelEditorPlaySound("sndEditorPopdownLarge", "LevelEditorInspectorPanel");
			}
		}

		private void Update()
		{
			time += Time.unscaledDeltaTime;
			float num = 0.6f;
			bool flag = false;
			while (time > (float)curBeat * num)
			{
				curBeat++;
				flag = true;
			}
			if (flag && characterButtons != null)
			{
				CharacterButton[] array = characterButtons;
				foreach (CharacterButton obj in array)
				{
					obj.portrait.OnBeat();
					obj.playerCharacter.PlayFromFrame(1);
					obj.ottoCharacter.PlayFromFrame(1);
				}
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			DisplayName("");
			ClearCharacterInfo();
		}

		public void DisplayName(string name)
		{
			characterText.text = name;
			characterText.font = RDString.GetAppropiateFontForString(characterText.text);
		}

		public void SetCharacterInfo(Character character)
		{
			Text text = characterInfo;
			string text2;
			switch (character)
			{
			case Character.Custom:
				text2 = RDString.Get("editor.custom");
				break;
			case Character.Beat:
			case Character.Quaver:
			case Character.Clef:
			case Character.Treble:
				text2 = "UNBEATABLE";
				break;
			case Character.Rin:
			case Character.Marija:
			case Character.Buro:
				text2 = "Muse Dash";
				break;
			case Character.Wren:
			case Character.Canary:
				text2 = "Bits & Bops";
				break;
			case Character.Saturday:
			case Character.Allison:
				text2 = "vivid/stasis";
				break;
			case Character.Lune:
			case Character.Sophia:
				text2 = "Circle of Sparks";
				break;
			case Character.Cranky:
				text2 = "Beatblock";
				break;
			default:
				text2 = "";
				break;
			}
			text.text = text2;
		}

		public void ClearCharacterInfo()
		{
			characterInfo.text = "";
		}

		public void AssignCharacterPicker(CharacterPicker characterPicker, bool returnsExpression)
		{
			this.characterPicker = characterPicker;
			this.returnsExpression = returnsExpression;
		}
	}
}
