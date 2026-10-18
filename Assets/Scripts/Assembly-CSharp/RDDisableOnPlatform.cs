using UnityEngine;

public class RDDisableOnPlatform : MonoBehaviour
{
	public enum RDPlatform
	{
		Mobile = 0,
		Desktop = 1
	}

	public RDPlatform disableIfPlatformIs;

	private void Awake()
	{
		if ((scnBase.isMobile && disableIfPlatformIs == RDPlatform.Mobile) || (!scnBase.isMobile && disableIfPlatformIs == RDPlatform.Desktop))
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
