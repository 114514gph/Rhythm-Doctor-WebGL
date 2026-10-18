using UnityEngine;
using UnityEngine.UI;

public class scrTextMobileDesktopSwitch : MonoBehaviour
{
	[TextArea(3, 10)]
	public string mobileText;

	[TextArea(3, 10)]
	public string desktopText;

	private void Start()
	{
		Text component = GetComponent<Text>();
		if (scnBase.isMobile)
		{
			component.text = mobileText;
		}
		else
		{
			component.text = desktopText;
		}
	}

	private void Update()
	{
	}
}
