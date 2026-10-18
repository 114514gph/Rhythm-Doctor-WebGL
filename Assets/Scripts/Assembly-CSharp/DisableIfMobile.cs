using UnityEngine;

public class DisableIfMobile : MonoBehaviour
{
	public bool negate;

	private void Start()
	{
		if (scnBase.isMobile != negate)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
