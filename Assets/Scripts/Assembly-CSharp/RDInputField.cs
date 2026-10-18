using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RDInputField : InputField, IPointerClickHandler, IEventSystemHandler
{
	[NonSerialized]
	public bool isNullableField;

	private string lastValue;

	private bool selected;

	private new void Awake()
	{
		lastValue = base.text;
		base.onEndEdit.AddListener(Validate);
	}

	protected virtual bool IsInvalid(string s)
	{
		return false;
	}

	private void Validate(string s)
	{
		selected = false;
		if (IsInvalid(s))
		{
			base.text = lastValue;
			return;
		}
		bool flag = base.contentType == ContentType.IntegerNumber;
		bool flag2 = base.contentType == ContentType.DecimalNumber;
		if (!(flag | flag2))
		{
			lastValue = base.text;
			return;
		}
		int result = 0;
		float result2 = 0f;
		if (isNullableField && s.IsNullOrEmpty())
		{
			lastValue = base.text;
		}
		else if ((flag && int.TryParse(s, out result)) || (flag2 && float.TryParse(s, out result2)))
		{
			base.text = (flag ? ((float)result) : result2).ToString();
			lastValue = base.text;
		}
		else
		{
			base.text = lastValue;
		}
	}

	public override void OnPointerClick(PointerEventData pointerEventData)
	{
		if (!selected && pointerEventData.button == PointerEventData.InputButton.Left)
		{
			lastValue = base.text;
			selected = true;
			ActivateInputField();
		}
	}
}
