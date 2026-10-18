using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/UI/tk2dUITextInput")]
public class tk2dUITextInput : MonoBehaviour
{
	public tk2dUIItem selectionBtn;

	public tk2dTextMesh inputLabel;

	public tk2dTextMesh emptyDisplayLabel;

	public GameObject unSelectedStateGO;

	public GameObject selectedStateGO;

	public GameObject cursor;

	public float fieldLength = 1f;

	public int maxCharacterLength = 30;

	public string emptyDisplayText;

	public bool isPasswordField;

	public string passwordChar = "*";

	[SerializeField]
	[HideInInspector]
	private tk2dUILayout layoutItem;

	private bool isSelected;

	private bool wasStartedCalled;

	private bool wasOnAnyPressEventAttached;

	private bool useTouchScreenKeyboard;

	private bool listenForKeyboardText;

	private bool isDisplayTextShown;

	public Action<tk2dUITextInput> OnTextChange;

	public string SendMessageOnTextChangeMethodName = "";

	private string text = "";

	public tk2dUILayout LayoutItem
	{
		get
		{
			return layoutItem;
		}
		set
		{
			if (layoutItem != value)
			{
				if (layoutItem != null)
				{
					layoutItem.OnReshape -= LayoutReshaped;
				}
				layoutItem = value;
				if (layoutItem != null)
				{
					layoutItem.OnReshape += LayoutReshaped;
				}
			}
		}
	}

	public GameObject SendMessageTarget
	{
		get
		{
			if (selectionBtn != null)
			{
				return selectionBtn.sendMessageTarget;
			}
			return null;
		}
		set
		{
			if (selectionBtn != null && selectionBtn.sendMessageTarget != value)
			{
				selectionBtn.sendMessageTarget = value;
			}
		}
	}

	public bool IsFocus => isSelected;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			if (text != value)
			{
				text = value;
				if (text.Length > maxCharacterLength)
				{
					text = text.Substring(0, maxCharacterLength);
				}
				FormatTextForDisplay(text);
				if (isSelected)
				{
					SetCursorPosition();
				}
			}
		}
	}

	private void Awake()
	{
		SetState();
		ShowDisplayText();
	}

	private void Start()
	{
		wasStartedCalled = true;
		if (tk2dUIManager.Instance__NoCreate != null)
		{
			tk2dUIManager.Instance.OnAnyPress += AnyPress;
		}
		wasOnAnyPressEventAttached = true;
	}

	private void OnEnable()
	{
		if (wasStartedCalled && !wasOnAnyPressEventAttached && tk2dUIManager.Instance__NoCreate != null)
		{
			tk2dUIManager.Instance.OnAnyPress += AnyPress;
		}
		if (layoutItem != null)
		{
			layoutItem.OnReshape += LayoutReshaped;
		}
		selectionBtn.OnClick += InputSelected;
	}

	private void OnDisable()
	{
		if (tk2dUIManager.Instance__NoCreate != null)
		{
			tk2dUIManager.Instance.OnAnyPress -= AnyPress;
			if (!useTouchScreenKeyboard && listenForKeyboardText)
			{
				tk2dUIManager.Instance.OnInputUpdate -= ListenForKeyboardTextUpdate;
			}
		}
		wasOnAnyPressEventAttached = false;
		selectionBtn.OnClick -= InputSelected;
		listenForKeyboardText = false;
		if (layoutItem != null)
		{
			layoutItem.OnReshape -= LayoutReshaped;
		}
	}

	public void SetFocus()
	{
		SetFocus(focus: true);
	}

	public void SetFocus(bool focus)
	{
		if (!IsFocus & focus)
		{
			InputSelected();
		}
		else if (IsFocus && !focus)
		{
			InputDeselected();
		}
	}

	private void FormatTextForDisplay(string modifiedText)
	{
		if (isPasswordField)
		{
			int length = modifiedText.Length;
			char paddingChar = ((passwordChar.Length > 0) ? passwordChar[0] : '*');
			modifiedText = "";
			modifiedText = modifiedText.PadRight(length, paddingChar);
		}
		inputLabel.text = modifiedText;
		inputLabel.Commit();
		for (float num = inputLabel.GetComponent<Renderer>().bounds.size.x / inputLabel.transform.lossyScale.x; num > fieldLength; num = inputLabel.GetComponent<Renderer>().bounds.size.x / inputLabel.transform.lossyScale.x)
		{
			modifiedText = modifiedText.Substring(1, modifiedText.Length - 1);
			inputLabel.text = modifiedText;
			inputLabel.Commit();
		}
		if (modifiedText.Length == 0 && !listenForKeyboardText)
		{
			ShowDisplayText();
		}
		else
		{
			HideDisplayText();
		}
	}

	private void ListenForKeyboardTextUpdate()
	{
		bool flag = false;
		string text = this.text;
		string inputString = Input.inputString;
		for (int i = 0; i < inputString.Length; i++)
		{
			char c = inputString[i];
			if (c == "\b"[0])
			{
				if (this.text.Length != 0)
				{
					text = this.text.Substring(0, this.text.Length - 1);
					flag = true;
				}
			}
			else if (c != "\n"[0] && c != "\r"[0] && c != '\t' && c != '\u001b')
			{
				text += c;
				flag = true;
			}
		}
		if (flag)
		{
			Text = text;
			NotifyTextChange();
		}
	}

	private void NotifyTextChange()
	{
		if (OnTextChange != null)
		{
			OnTextChange(this);
		}
		if (SendMessageTarget != null && SendMessageOnTextChangeMethodName.Length > 0)
		{
			SendMessageTarget.SendMessage(SendMessageOnTextChangeMethodName, this, SendMessageOptions.RequireReceiver);
		}
	}

	private void InputSelected()
	{
		if (text.Length == 0)
		{
			HideDisplayText();
		}
		isSelected = true;
		if (!useTouchScreenKeyboard && !listenForKeyboardText)
		{
			tk2dUIManager.Instance.OnInputUpdate += ListenForKeyboardTextUpdate;
		}
		listenForKeyboardText = true;
		SetState();
		SetCursorPosition();
	}

	private void InputDeselected()
	{
		if (text.Length == 0)
		{
			ShowDisplayText();
		}
		isSelected = false;
		if (!useTouchScreenKeyboard && listenForKeyboardText)
		{
			tk2dUIManager.Instance.OnInputUpdate -= ListenForKeyboardTextUpdate;
		}
		listenForKeyboardText = false;
		SetState();
	}

	private void AnyPress()
	{
		if (isSelected && tk2dUIManager.Instance.PressedUIItem != selectionBtn)
		{
			InputDeselected();
		}
	}

	private void SetState()
	{
		tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(unSelectedStateGO, !isSelected);
		tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(selectedStateGO, isSelected);
		tk2dUIBaseItemControl.ChangeGameObjectActiveState(cursor, isSelected);
	}

	private void SetCursorPosition()
	{
		float num = 1f;
		float num2 = 0.002f;
		if (inputLabel.anchor == TextAnchor.MiddleLeft || inputLabel.anchor == TextAnchor.LowerLeft || inputLabel.anchor == TextAnchor.UpperLeft)
		{
			num = 2f;
		}
		else if (inputLabel.anchor == TextAnchor.MiddleRight || inputLabel.anchor == TextAnchor.LowerRight || inputLabel.anchor == TextAnchor.UpperRight)
		{
			num = -2f;
			num2 = 0.012f;
		}
		if (text.EndsWith(" "))
		{
			tk2dFontChar tk2dFontChar2 = ((!inputLabel.font.inst.useDictionary) ? inputLabel.font.inst.chars[32] : inputLabel.font.inst.charDict[32]);
			num2 += tk2dFontChar2.advance * inputLabel.scale.x / 2f;
		}
		float num3 = inputLabel.GetComponent<Renderer>().bounds.extents.x / base.gameObject.transform.lossyScale.x;
		cursor.transform.localPosition = new Vector3(inputLabel.transform.localPosition.x + (num3 + num2) * num, cursor.transform.localPosition.y, cursor.transform.localPosition.z);
	}

	private void ShowDisplayText()
	{
		if (!isDisplayTextShown)
		{
			isDisplayTextShown = true;
			if (emptyDisplayLabel != null)
			{
				emptyDisplayLabel.text = emptyDisplayText;
				emptyDisplayLabel.Commit();
				tk2dUIBaseItemControl.ChangeGameObjectActiveState(emptyDisplayLabel.gameObject, isActive: true);
			}
			tk2dUIBaseItemControl.ChangeGameObjectActiveState(inputLabel.gameObject, isActive: false);
		}
	}

	private void HideDisplayText()
	{
		if (isDisplayTextShown)
		{
			isDisplayTextShown = false;
			tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(emptyDisplayLabel.gameObject, isActive: false);
			tk2dUIBaseItemControl.ChangeGameObjectActiveState(inputLabel.gameObject, isActive: true);
		}
	}

	private void LayoutReshaped(Vector3 dMin, Vector3 dMax)
	{
		fieldLength += dMax.x - dMin.x;
		string text = this.text;
		this.text = "";
		Text = text;
	}
}
