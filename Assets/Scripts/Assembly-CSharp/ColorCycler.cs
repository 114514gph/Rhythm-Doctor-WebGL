using UnityEngine;
using UnityEngine.UI;

public class ColorCycler : RDBase
{
	public Text text;

	public Color[] colors;

	public float colorDuration;

	private void Update()
	{
		if (colors.Length != 0)
		{
			int num = Mathf.FloorToInt(Time.unscaledTime % (colorDuration * (float)colors.Length) / colorDuration);
			text.color = colors[num];
		}
	}
}
