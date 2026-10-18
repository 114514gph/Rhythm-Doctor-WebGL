using UnityEngine;

public class RDLightStrip : MonoBehaviour
{
	public GameObject LeftBar;

	public GameObject RightBar;

	public void Show(float x, float sliceWidth = 24f)
	{
		base.gameObject.SetActive(value: true);
		base.transform.MoveX(x);
		LeftBar.transform.LocalMoveX(-400f - sliceWidth / 2f);
		RightBar.transform.LocalMoveX(sliceWidth / 2f);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
}
