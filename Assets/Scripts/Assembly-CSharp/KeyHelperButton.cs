using System;
using UnityEngine;
using UnityEngine.UI;

public class KeyHelperButton : MonoBehaviour
{
	private const float joystickWidth = 9f;

	private const float smallWidth = 12f;

	private const float mediumWidth = 19f;

	private const float bigWidth = 23f;

	private const float normalHeight = 10f;

	private const float pressedHeight = 9f;

	public RDInputAction inputAction;

	public KeyCode keyCode;

	public KeyCode[] ignoreKeys;

	public RectTransform rect;

	public Image background;

	public Text label;

	public Image icon;

	[NonSerialized]
	public bool pressed;

	[NonSerialized]
	public bool foundTypeInfo;

	public void UpdateKeyboardLabels()
	{
		string text = null;
		float x = 12f;
		icon.enabled = false;
		if (keyCode != KeyCode.None)
		{
			if (keyCode == KeyCode.LeftArrow)
			{
				text = "←";
			}
			else if (keyCode == KeyCode.RightArrow)
			{
				text = "→";
			}
			else if (keyCode == KeyCode.Escape)
			{
				text = "esc";
				x = 23f;
			}
			else if (keyCode == KeyCode.Tab)
			{
				text = "tab";
			}
			else if (keyCode == KeyCode.UpArrow)
			{
				icon.enabled = true;
			}
			else if (keyCode == KeyCode.DownArrow)
			{
				icon.enabled = true;
			}
			else if (keyCode == KeyCode.Space || keyCode == KeyCode.Return)
			{
				icon.enabled = true;
				x = 23f;
			}
			else
			{
				text = ((!keyCode.ToString().StartsWith("Alpha")) ? keyCode.ToString() : keyCode.ToString().Replace("Alpha", string.Empty));
			}
		}
		else
		{
			x = 9f;
		}
		rect.sizeDelta = rect.sizeDelta.WithX(x);
		label.text = text;
		label.enabled = !label.text.IsNullOrEmpty();
	}

	private void Update()
	{
		if (inputAction != RDInputAction.None)
		{
			if (inputAction == RDInputAction.Main)
			{
				bool flag = false;
				KeyCode[] array = ignoreKeys;
				for (int i = 0; i < array.Length; i++)
				{
					if (Input.GetKey(array[i]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					pressed = RDInput.anyPlayerIsPressed;
				}
			}
			else if (inputAction == RDInputAction.Cancel)
			{
				pressed = RDInput.cancelIsPressed;
			}
			else if (inputAction == RDInputAction.Left)
			{
				pressed = RDInput.leftIsPressed;
			}
			else if (inputAction == RDInputAction.Right)
			{
				pressed = RDInput.rightIsPressed;
			}
			else if (inputAction == RDInputAction.Up)
			{
				pressed = RDInput.upIsPressed;
			}
			else if (inputAction == RDInputAction.Down)
			{
				pressed = RDInput.downIsPressed;
			}
			else if (inputAction == RDInputAction.Skip)
			{
				pressed = RDInput.skipPressed;
			}
			else if (inputAction == RDInputAction.Restart)
			{
				pressed = RDInput.restartPressed;
			}
			else if (inputAction == RDInputAction.Quit)
			{
				pressed = RDInput.quitPressed;
			}
		}
		else
		{
			pressed = Input.GetKey(keyCode);
		}
		float y = (pressed ? 9f : 10f);
		rect.sizeDelta = rect.sizeDelta.WithY(y);
	}

	public void UpdateControllerType()
	{
		KeyHelperInfo currentInfo = KeyHelper.currentInfo;
		ButtonInfo buttonInfo = inputAction switch
		{
			RDInputAction.Main => currentInfo.bottomAction, 
			RDInputAction.Cancel => currentInfo.rightAction, 
			RDInputAction.Up => currentInfo.upDirection, 
			RDInputAction.Down => currentInfo.downDirection, 
			RDInputAction.Left => currentInfo.leftDirection, 
			RDInputAction.Right => currentInfo.rightDirection, 
			_ => default, 
		};
		foundTypeInfo = buttonInfo.sprite != null;
		if (foundTypeInfo)
		{
			background.sprite = buttonInfo.sprite;
			background.color = buttonInfo.color;
		}
		bool flag = currentInfo.type == ControllerType.Keyboard;
		label.gameObject.SetActive(flag);
		icon.gameObject.SetActive(flag);
		if (flag)
		{
			UpdateKeyboardLabels();
		}
		else
		{
			rect.sizeDelta = rect.sizeDelta.WithX(9f);
		}
	}
}
