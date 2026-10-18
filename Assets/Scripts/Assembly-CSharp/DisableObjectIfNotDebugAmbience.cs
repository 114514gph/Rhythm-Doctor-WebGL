using UnityEngine;

public class DisableObjectIfNotDebugAmbience : RDBase
{
	public GameObject targetGO;

	public bool OnlyOnDesktop;

	private void Awake()
	{
		if (targetGO == null)
		{
			targetGO = base.gameObject;
		}
	}

	private void LateUpdate()
	{
		if (!OnlyOnDesktop || !scnBase.isMobile)
		{
			if (RDBase.debugSettings.DebugAmbience && !targetGO.activeSelf)
			{
				targetGO.SetActive(value: true);
			}
			if (!RDBase.debugSettings.DebugAmbience && targetGO.activeSelf)
			{
				targetGO.SetActive(value: false);
			}
		}
	}
}
