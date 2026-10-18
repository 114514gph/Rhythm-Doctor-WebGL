using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class CharacterButton : RDEditorBase, IPointerEnterHandler, IEventSystemHandler
	{
		public Button button;

		public CustomPortrait portrait;

		public int xPositionInGrid;

		public int yPositionInGrid;

		private string expression;

		public bool isCustomChar;

		public GameObject noCharacter;

		public GameObject newCharacter;

		public SpriteAnimation playerCharacter;

		public SpriteAnimation ottoCharacter;

		public readonly Character[] specialCharacters = new Character[5]
		{
			Character.None,
			Character.New,
			Character.Otto,
			Character.Player,
			Character.BlankCPU
		};

		public void Setup(Character character, string customCharacter = null, string expression = null)
		{
			this.expression = expression;
			if (specialCharacters.Contains(character))
			{
				SetupSpecial(character);
				return;
			}
			portrait.customCrotchet = 0.6f;
			portrait.customAnimation.ignoreTimescale = true;
			portrait.gameObject.SetActive(value: true);
			portrait.forceLoop = true;
			if (character == Character.Custom)
			{
				if (customCharacter == null)
				{
					return;
				}
				isCustomChar = true;
				button.transform.name = customCharacter;
				if (!base.editor.customCharacterData.ContainsKey(customCharacter))
				{
					LevelEvent_MakeRow.UpdateCustomCharacter(customCharacter, justReturnErrors: false, useEditorData: true, fromCustomCharactersFolder: true);
				}
				if (!portrait.Setup(customCharacter, expression ?? "neutral", checkEditorCharData: true))
				{
					return;
				}
			}
			else
			{
				portrait.Setup(character, expression ?? "neutral");
				button.transform.name = character.ToString();
			}
			CustomAnimationClip currentClip = portrait.customAnimation.currentClip;
			if (currentClip != null)
			{
				portrait.GetComponent<RectTransform>().sizeDelta = currentClip.portraitSize;
			}
			button.onClick.AddListener(() =>
			{
				Select();
				LevelEditorPlaySound("sndListClick", "LevelEditorInspectorPanel");
			});
		}

		public void SetupSpecial(Character character)
		{
			portrait.character = character;
			float singleSpriteDuration = 0.15f;
			playerCharacter.gameObject.SetActive(character == Character.Player);
			playerCharacter.singleSpriteDuration = singleSpriteDuration;
			ottoCharacter.gameObject.SetActive(character == Character.Otto);
			ottoCharacter.singleSpriteDuration = singleSpriteDuration;
			noCharacter.SetActive(character == Character.None || character == Character.BlankCPU);
			newCharacter.SetActive(character == Character.New || character == Character.Custom);
			button.onClick.AddListener(() =>
			{
				Select();
				LevelEditorPlaySound("sndListClick", "LevelEditorInspectorPanel");
			});
			portrait.gameObject.SetActive(value: false);
			button.transform.name = character.ToString();
		}

		private void Select()
		{
			RDCharacterPickerPopup characterPickerPopup = base.editor.characterPickerPopup;
			string text = expression;
			if (isCustomChar && string.IsNullOrEmpty(base.editor.openedFilePath))
			{
				RDEditorUtils.AskUserToSaveLevel(RDString.Get("editor.dialog.saveBeforeImporting"));
			}
			else if (text == null)
			{
				characterPickerPopup.SelectCharacter(portrait.character, portrait.customCharacter);
			}
			else if (portrait.character == Character.New)
			{
				characterPickerPopup.SwitchToInput();
			}
			else
			{
				characterPickerPopup.SelectExpression(text);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			string text;
			if (expression == null)
			{
				if (isCustomChar)
				{
					text = base.editor.customCharacterData[portrait.customCharacter].name;
					base.editor.characterPickerPopup.SetCharacterInfo(Character.Custom);
				}
				else
				{
					text = RDString.GetEnumValue(portrait.character);
					base.editor.characterPickerPopup.SetCharacterInfo(portrait.character);
				}
			}
			else
			{
				string withCheck = RDString.GetWithCheck("editor.expression." + expression, out var exists);
				text = (exists ? withCheck : expression);
				portrait.PlayExpression(expression);
			}
			base.editor.characterPickerPopup.DisplayName(text);
			float pitch = RDUtils.PitchSemitonesToPercent((float)(xPositionInGrid + yPositionInGrid) - 3f);
			LevelEditorPlaySound("sndListHover", "LevelEditorInspectorPanel", 1f, pitch);
		}
	}
}
