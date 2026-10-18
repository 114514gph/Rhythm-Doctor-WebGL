using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class RDColorPickerPopup : RDEditorBase
	{
		private const string ErrorColor = "ffddee";

		private const float maxY = -54f;

		private const float minY = -130f;

		public GameObject alphaSlider;

		public GameObject alphaInputField;

		public RectTransform panel;

		public CUIColorPicker cuiColorPicker;

		public InputField hexColor;

		public InputField rInput;

		public InputField gInput;

		public InputField bInput;

		public InputField aInput;

		public ToggleGroup paletteColor;

		public GameObject cuiBlock;

		public string lastValidColor = "ffddee";

		private ColorPicker colorPicker;

		private bool isAnyRGBAInputFocused;

		private List<Toggle> _paletteToggles = new List<Toggle>();

		public int paletteIndex
		{
			get
			{
				int num = -1;
				for (int i = 0; i < paletteToggles.Count; i++)
				{
					if (paletteToggles[i].isOn)
					{
						num = i;
						break;
					}
				}
				return num - 1;
			}
		}

		public bool usingPaletteColor => paletteIndex >= 0;

		private List<Toggle> paletteToggles
		{
			get
			{
				if (_paletteToggles.Count == 0)
				{
					Toggle[] componentsInChildren = paletteColor.transform.GetComponentsInChildren<Toggle>();
					foreach (Toggle item in componentsInChildren)
					{
						_paletteToggles.Add(item);
					}
				}
				return _paletteToggles;
			}
		}

		public string color
		{
			get
			{
				if (usingPaletteColor)
				{
					return "pal" + paletteIndex;
				}
				if (!RDEditorUtils.StringIsValidHexColor(hexColor.text, colorPicker.storesAlpha))
				{
					return lastValidColor;
				}
				return hexColor.text;
			}
			set
			{
				string s = value;
				UpdatePaletteToggles(value, out var hexValueOfPaletteColor);
				if (!hexValueOfPaletteColor.IsNullOrEmpty())
				{
					s = (colorPicker.storesAlpha ? hexValueOfPaletteColor : hexValueOfPaletteColor.Remove(hexValueOfPaletteColor.Length - 2));
				}
				if (RDEditorUtils.StringIsValidHexColor(s, colorPicker.storesAlpha))
				{
					hexColor.text = value;
					lastValidColor = hexColor.text;
					cuiColorPicker.Color = lastValidColor.HexToColor();
				}
			}
		}

		private void Start()
		{
			hexColor.onValueChanged.AddListener(delegate
			{
				if (RDEditorUtils.StringIsValidHexColor(hexColor.text, colorPicker.storesAlpha) && hexColor.isFocused)
				{
					lastValidColor = hexColor.text;
					cuiColorPicker.Color = lastValidColor.HexToColor();
				}
			});
			foreach (Toggle paletteToggle in paletteToggles)
			{
				ClickEvent orAddComponent = paletteToggle.gameObject.GetOrAddComponent<ClickEvent>();
				orAddComponent.PointerClick = new UnityEvent();
				orAddComponent.PointerClick.AddListener(() =>
				{
					if (usingPaletteColor)
					{
						lastValidColor = "pal" + paletteIndex;
						colorPicker.border.color = Color.white;
					}
					else
					{
						lastValidColor = hexColor.text;
						colorPicker.border.color = Color.black;
					}
					cuiColorPicker.Color = lastValidColor.HexOrPaletteIndexToColor(colorPicker.storesAlpha);
					SetCUIInteractable(!usingPaletteColor);
				});
			}
		}

		private void Update()
		{
			isAnyRGBAInputFocused = rInput.isFocused || gInput.isFocused || bInput.isFocused || aInput.isFocused;
			if (!isAnyRGBAInputFocused && RDEditorUtils.StringIsValidHexColor(hexColor.text, colorPicker.storesAlpha))
			{
				Color color = hexColor.text.HexToColor();
				rInput.text = Convert.ToInt32(color.r * 255f).ToString();
				gInput.text = Convert.ToInt32(color.g * 255f).ToString();
				bInput.text = Convert.ToInt32(color.b * 255f).ToString();
				aInput.text = Convert.ToInt32(color.a * 255f).ToString();
			}
			if (!hexColor.isFocused)
			{
				UpdateHexInputColor();
			}
		}

		public void SetResultColorFromRGBAInputs(InputField currentRGBAInput)
		{
			if (isAnyRGBAInputFocused)
			{
				if (string.IsNullOrEmpty(currentRGBAInput.text))
				{
					currentRGBAInput.text = "0";
				}
				else if (Convert.ToInt32(currentRGBAInput.text) < 0)
				{
					currentRGBAInput.text = "0";
				}
				else if (Convert.ToInt32(currentRGBAInput.text) > 255)
				{
					currentRGBAInput.text = "255";
				}
				Color color = new Color((float)Convert.ToInt32(rInput.text) / 255f, (float)Convert.ToInt32(gInput.text) / 255f, (float)Convert.ToInt32(bInput.text) / 255f, (float)Convert.ToInt32(aInput.text) / 255f);
				cuiColorPicker.Color = color;
			}
		}

		private void UpdateHexInputColor()
		{
			hexColor.text = ColorUtility.ToHtmlStringRGBA(cuiColorPicker.Color);
		}

		public void Show(ColorPicker colorPicker)
		{
			this.colorPicker = colorPicker;
			RectTransform component = GetComponent<RectTransform>();
			Vector2 zero = Vector2.zero;
			if (colorPicker.storesAlpha)
			{
				alphaSlider.SetActive(value: true);
				alphaInputField.SetActive(value: true);
				zero = zero.WithY(117f);
				hexColor.characterLimit = 8;
				hexColor.GetComponent<RectTransform>().sizeDelta = new Vector2(47f, 13f);
			}
			else
			{
				alphaSlider.SetActive(value: false);
				alphaInputField.SetActive(value: false);
				zero = zero.WithY(106f);
				hexColor.characterLimit = 6;
				hexColor.GetComponent<RectTransform>().sizeDelta = new Vector2(35f, 13f);
			}
			if (colorPicker.canUsePalette)
			{
				paletteColor.gameObject.SetActive(value: true);
				zero = zero.WithX(152f);
				for (int i = 1; i < paletteToggles.Count; i++)
				{
					PalettePickerButton component2 = paletteToggles[i].GetComponent<PalettePickerButton>();
					Color color = base.editor.colorPalette[i - 1].HexToColor();
					component2.SetImageColor(colorPicker.storesAlpha ? color : color.WithAlpha(1f));
				}
			}
			else
			{
				paletteColor.gameObject.SetActive(value: false);
				zero = zero.WithX(105f);
			}
			panel.sizeDelta = zero;
			alphaSlider.GetComponent<AlphaSlider>().result = colorPicker.sample;
			cuiColorPicker.startColor = colorPicker.color.HexOrPaletteIndexToColor(colorPicker.storesAlpha);
			cuiColorPicker.result = colorPicker.sample.gameObject;
			hexColor.text = ColorUtility.ToHtmlStringRGBA(colorPicker.sample.color);
			UpdatePaletteToggles(colorPicker.color);
			float x = ((colorPicker.transform as RectTransform).sizeDelta.x / 2f + 1f) * (float)Screen.width / base.editor.canvas.GetComponent<CanvasScaler>().referenceResolution.x;
			component.position = colorPicker.sample.rectTransform.position.AddX(x);
			Vector2 anchoredPosition = component.anchoredPosition;
			float num = 0f;
			if (anchoredPosition.y < -130f)
			{
				num = -130f - anchoredPosition.y;
			}
			else if (anchoredPosition.y > -54f)
			{
				num = -54f - anchoredPosition.y;
			}
			panel.AnchorPosY(num * 1f);
			base.gameObject.SetActive(value: true);
			base.editor.popupBackground.SetActive(value: true);
			LevelEditorPlaySound("sndEditorPopupLarge", "LevelEditorInspectorPanel");
			component.DOKill(complete: true);
			component.localScale = Vector3.zero;
			component.DOScale(Vector3.one, base.gc.roomsSelectionPopup_showAnimDuration).SetEase(base.gc.roomsSelectionPopup_showAnimEase).SetUpdate(UpdateType.Normal, isIndependentUpdate: true)
				.SetId("noKillOnReload");
		}

		public void Hide()
		{
			base.editor.popupBackground.SetActive(value: false);
			if (base.gameObject.activeSelf)
			{
				colorPicker.color = color;
				colorPicker.onEndEdit(color);
				LevelEditorPlaySound("sndEditorPopdownLarge", "LevelEditorInspectorPanel");
				RectTransform component = GetComponent<RectTransform>();
				component.DOKill(complete: true);
				component.localScale = Vector3.one;
				component.DOScale(Vector3.zero, base.gc.roomsSelectionPopup_hideAnimDuration).SetEase(base.gc.roomsSelectionPopup_hideAnimEase).SetUpdate(UpdateType.Normal, isIndependentUpdate: true)
					.OnComplete(() =>
					{
						base.gameObject.SetActive(value: false);
					});
			}
		}

		private void UpdatePaletteToggles(string col)
		{
			UpdatePaletteToggles(col, out var _);
		}

		private void UpdatePaletteToggles(string col, out string hexValueOfPaletteColor)
		{
			hexValueOfPaletteColor = "";
			if (col.StartsWith("pal"))
			{
				if (int.TryParse(col.Replace("pal", ""), out var result))
				{
					if (result >= 0 && result < 21)
					{
						SetPaletteToggleValue(result + 1);
						hexValueOfPaletteColor = base.editor.colorPalette[result];
					}
					else
					{
						SetPaletteToggleValue(0);
					}
				}
				else
				{
					SetPaletteToggleValue(0);
				}
			}
			else
			{
				SetPaletteToggleValue(0);
			}
		}

		private void SetPaletteToggleValue(int index)
		{
			for (int i = 0; i < paletteToggles.Count; i++)
			{
				paletteToggles[i].isOn = i == index;
			}
			SetCUIInteractable(index == 0);
		}

		public void SetCUIInteractable(bool interactable)
		{
			cuiBlock.SetActive(!interactable);
			cuiColorPicker.interactable = interactable;
		}

		public void SelectUsingCrosshair()
		{
			if (base.editor.isPlaying)
			{
				base.game.TogglePauseGame();
				LevelEditorPlaySound("sndEditorTransportPause", "LevelEditorActive");
				base.editor.UpdateUI_PlayPauseButton();
			}
			RenderTexture renderTexture = null;
			if (base.editor.gameView.texture != null)
			{
				renderTexture = base.editor.gameView.texture as RenderTexture;
			}
			if (renderTexture != null)
			{
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = renderTexture;
				Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, mipChain: false);
				texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
				RenderTexture.active = active;
				base.editor.textureForCrosshair = texture2D;
			}
			else
			{
				base.editor.textureForCrosshair = null;
				printe("Texture is null");
			}
			base.editor.pickerUsingCrosshair = colorPicker;
		}
	}
}
