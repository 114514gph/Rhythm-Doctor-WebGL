using UnityEngine;

public class RDCredits : MonoBehaviour
{
	public Logo logo;

	public Credits credits;

	public GameObject main;

	public GameObject preorders;

	public HorizontalCredits hCred;

	public HorizontalCredits hCredCJK;

	public void SwitchToPreorders()
	{
		base.transform.LocalMoveXY(3000f, 500f);
		main.SetActive(value: false);
		preorders.SetActive(value: true);
		hCred.Populate();
		float num = Mathf.Max(hCred.topText.preferredWidth, hCred.bottomText.preferredWidth);
		hCredCJK.Populate();
		hCredCJK.transform.LocalMoveX(num + 352f + 40f);
	}
}
