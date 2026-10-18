using System;
using UnityEngine.UI;

public class TimeDisplay : RDBase
{
	public Text text;

	private int prevSecond = -1;

	private void Awake()
	{
		RDStringToUIText.Apply(text);
	}

	private void Update()
	{
		DateTime now = DateTime.Now;
		if (now.Second != prevSecond)
		{
			prevSecond = now.Second;
			string newValue = now.ToString("hh:mm");
			string text = now.ToString("tt");
			string text2 = RDString.Get("computer.time" + text).Replace("[time]", newValue);
			if (now.Second % 2 == 0)
			{
				text2 = text2.Replace(":", "<color=#00000000>:</color>");
			}
			this.text.text = text2;
		}
	}
}
