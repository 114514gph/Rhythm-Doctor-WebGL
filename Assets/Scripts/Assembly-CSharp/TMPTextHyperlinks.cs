using TMPro;
using UnityEngine;

public class TMPTextHyperlinks : RDBase
{
	public TextMeshProUGUI text;

	private void LateUpdate()
	{
		int num = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null);
		if (num != -1 && Input.GetMouseButtonDown(0))
		{
			TMP_LinkInfo tMP_LinkInfo = text.textInfo.linkInfo[num];
			RDBaseDllDummy.printem(tMP_LinkInfo.GetLinkID());
			Application.OpenURL(tMP_LinkInfo.GetLinkID());
		}
	}
}
