using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class CharacterPicker : RDEditorBase, IPointerEnterHandler, IEventSystemHandler
	{
		public delegate void EndHandler(Character character, string expression = null, string customCharacter = null);

		public bool portraitPicker;

		public bool returnsExpression;

		public bool includesCustomChars = true;

		public bool includesNew;

		public bool includesNone;

		public Text label;

		public bool emptyIsIgnore;

		[NonSerialized]
		public Character[] availableCharacters;

		[NonSerialized]
		public string customCharacter;

		[NonSerialized]
		public Character character;

		[NonSerialized]
		public string expression;

		[NonSerialized]
		public GameObject textInput;

		public EndHandler onEndEdit;

		private bool doctahShield;

		private bool isDoctah;

		public Character[] GetCharsToShow(bool showAll)
		{
			if (!showAll)
			{
				return availableCharacters;
			}
			Character[] array = (Character[])Enum.GetValues(typeof(Character));
			if (!RDBase.isDev)
			{
				return array.Except(RDEditorConstants.UnsafeEditorCharacters).ToArray();
			}
			return array;
		}

		public void ShowCharacterPickerPopup()
		{
			bool flag = Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl);
			Character[] charsToShow = GetCharsToShow(flag);
			List<Character> list = new List<Character>();
			if (includesNone && !flag)
			{
				list.Add(Character.None);
			}
			if (includesNew && !flag)
			{
				list.Add(Character.New);
			}
			base.editor.characterPickerPopup.ShowCharacters(this, charsToShow, returnsExpression, includesCustomChars, list.ToArray());
			if (isDoctah)
			{
				LevelEditorPlaySound("sndDoctahHehehe", "LevelEditorInspectorPanel");
				GetComponent<Image>().sprite = base.editor.emojiSprite;
				isDoctah = false;
				doctahShield = false;
			}
		}

		public void ShowExpressionsForCharacter(Character character, string customCharacter = null, bool includeNone = false, bool includeNew = false)
		{
			RDCharacterPickerPopup characterPickerPopup = base.editor.characterPickerPopup;
			characterPickerPopup.AssignCharacterPicker(this, returnsExpression: true);
			characterPickerPopup.ShowExpressions(character, customCharacter, showWindow: true, includeNone, includeNew);
		}

		public void OnPointerEnter(PointerEventData pointerData)
		{
			if (portraitPicker)
			{
				Image component = GetComponent<Image>();
				if (doctahShield)
				{
					doctahShield = false;
				}
				else if (UnityEngine.Random.Range(0, 40) == 1)
				{
					component.sprite = base.editor.doctahSprite;
					isDoctah = true;
					doctahShield = true;
					LevelEditorPlaySound("sndDoctahWeh", "LevelEditorInspectorPanel");
				}
				else
				{
					component.sprite = base.editor.emojiSprite;
					isDoctah = false;
				}
			}
		}

		public void SetCharacter(Character character, string customCharacter = null)
		{
			if (!portraitPicker && character != Character.New)
			{
				this.character = character;
				this.customCharacter = customCharacter;
				UpdateLabel((character == Character.Custom && customCharacter != null) ? customCharacter : RDString.Get($"enum.Character.{character}.short"));
			}
		}

		public void SetExpression(string expression)
		{
			if (!portraitPicker)
			{
				if (emptyIsIgnore && (expression == "" || expression == "none"))
				{
					this.expression = "";
					UpdateLabel("<color=#8b0000>" + RDString.Get("editor.expression.none") + "</color>");
					return;
				}
				this.expression = expression;
				string withCheck = RDString.GetWithCheck("editor.expression." + expression, out var exists);
				base.name = (exists ? withCheck : expression);
				UpdateLabel(base.name);
			}
		}

		public void SetCustomExpression(string expression)
		{
			if (!portraitPicker)
			{
				this.expression = expression;
				UpdateLabel(expression);
			}
		}

		public void SetCustomExpression(string expression, string ifEmpty)
		{
			SetCustomExpression((expression.Length > 0) ? expression : ifEmpty);
		}

		private void UpdateLabel(string str)
		{
			if (label != null)
			{
				label.text = str;
				label.font = RDString.GetAppropiateFontForString(str);
			}
		}

		public void EnableCustomInput(bool enable)
		{
			if ((bool)textInput)
			{
				base.gameObject.SetActive(!enable);
				textInput.SetActive(enable);
			}
		}
	}
}
