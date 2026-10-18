using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_InputField : PropertyControl
	{
		public InputField inputField;

		public ExpInputField expInputField;

		public Text unit;

		private MethodAutocompleteUI autocompleteUI;

		private bool alreadyUpdating;

		public override GameObject controlObject => inputField.gameObject;

		private InputFieldAttribute controlAttribute => (InputFieldAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			inputField = ((base.propertyInfo.NullableUnderlying is FloatExpressionPropertyInfo) ? expInputField : inputField);
			inputField.gameObject.SetActive(value: true);
			string text = ((base.propertyInfo is StringPropertyInfo) ? null : controlAttribute.unit);
			TextAnchor alignment = ((!(base.propertyInfo is StringPropertyInfo)) ? TextAnchor.MiddleRight : TextAnchor.UpperLeft);
			int num = ((base.propertyInfo is StringPropertyInfo) ? Math.Max(16, controlAttribute.inputFieldHeight) : controlAttribute.inputFieldHeight);
			float num2 = (float)(controlAttribute.multiline ? Property.propertySize : Property.controlSize) / ((text != null) ? 2f : 1f);
			RectTransform component = inputField.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(num2, component.sizeDelta.y);
			if (text != null)
			{
				string withCheck = RDString.GetWithCheck("editor." + text, out var exists);
				if (exists)
				{
					text = withCheck;
				}
				unit.text = text;
				RDStringToUIText.Apply(unit);
				unit.gameObject.SetActive(value: true);
				component = unit.GetComponent<RectTransform>();
				component.sizeDelta = new Vector2(num2 - 4f, component.sizeDelta.y);
				component.anchoredPosition = new Vector2(num2 + 4f, 0f);
			}
			InputField obj = inputField;
			BasePropertyInfo nullableUnderlying = base.propertyInfo.NullableUnderlying;
			InputField.ContentType contentType;
			if (!(nullableUnderlying is IntPropertyInfo))
			{
				if (!(nullableUnderlying is FloatPropertyInfo))
				{
					if (!(nullableUnderlying is FloatExpressionPropertyInfo))
					{
						if (!(nullableUnderlying is StringPropertyInfo))
						{
							throw new ArgumentOutOfRangeException();
						}
						contentType = InputField.ContentType.Standard;
					}
					else
					{
						contentType = InputField.ContentType.DecimalNumber;
					}
				}
				else
				{
					contentType = InputField.ContentType.DecimalNumber;
				}
			}
			else
			{
				contentType = InputField.ContentType.IntegerNumber;
			}
			obj.contentType = contentType;
			inputField.lineType = controlAttribute.lineType;
			inputField.textComponent.alignment = alignment;
			inputField.characterLimit = controlAttribute.characterLimit;
			if (base.propertyInfo is StringPropertyInfo)
			{
				component = inputField.textComponent.GetComponent<RectTransform>();
				if (Persistence.GetReducePixelFontUsage())
				{
					component.offsetMin = new Vector2(2f, 2f);
					component.offsetMax = new Vector2(-2f, -2f);
				}
				else
				{
					component.offsetMin = new Vector2(4f, 4f);
					component.offsetMax = new Vector2(-4f, -4f);
				}
			}
			SetPropertyControlHeight(num);
			if (controlAttribute.autocomplete)
			{
				autocompleteUI = UnityEngine.Object.FindObjectOfType<MethodAutocompleteUI>(includeInactive: true);
				inputField.onValueChanged.AddListener((string s) =>
				{
					if (!alreadyUpdating)
					{
						autocompleteUI.Search(inputField, s);
					}
				});
			}
			inputField.onEndEdit.AddListener((string _) =>
			{
				action(DisableAutocomplete, "sndEditorValueChange");
			});
		}

		private void DisableAutocomplete()
		{
			if (controlAttribute.autocomplete && autocompleteUI != null && !autocompleteUI.isHovering)
			{
				DOVirtual.DelayedCall(0.15f, () =>
				{
					autocompleteUI.HidePopup();
				});
			}
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			object eventValue = GetEventValue(levelEvent);
			inputField.text = eventValue?.ToString();
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			alreadyUpdating = true;
			BasePropertyInfo nullableUnderlying = base.propertyInfo.NullableUnderlying;
			object obj;
			if (!(nullableUnderlying is IntPropertyInfo intPropertyInfo))
			{
				if (!(nullableUnderlying is FloatPropertyInfo floatPropertyInfo))
				{
					if (!(nullableUnderlying is FloatExpressionPropertyInfo floatExpressionPropertyInfo))
					{
						if (!(nullableUnderlying is StringPropertyInfo))
						{
							throw new ArgumentOutOfRangeException();
						}
						obj = inputField.text;
					}
					else
					{
						obj = floatExpressionPropertyInfo.Clamp(FloatExpression.FromString(inputField.text));
					}
				}
				else
				{
					obj = Mathf.Clamp(float.Parse(inputField.text), floatPropertyInfo.min, floatPropertyInfo.max);
				}
			}
			else
			{
				obj = Mathf.Clamp(int.Parse(inputField.text), intPropertyInfo.min, intPropertyInfo.max);
			}
			object obj2 = obj;
			inputField.text = obj2?.ToString();
			SetEventValue(levelEvent, obj2);
			alreadyUpdating = false;
		}

		private void OnDisable()
		{
			autocompleteUI?.HidePopup();
		}
	}
}
