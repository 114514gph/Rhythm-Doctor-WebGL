using UnityEngine;

public class DisableObjectIfNotDebug : RDBase
{
	public GameObject targetGO;

	private void Awake()
	{
		if (targetGO == null)
		{
			targetGO = base.gameObject;
		}
	}

	private void LateUpdate()
	{
		if (RDBase.debugSettings.Debug && !targetGO.activeSelf)
		{
			targetGO.SetActive(value: true);
		}
		if (!RDBase.debugSettings.Debug && targetGO.activeSelf)
		{
			targetGO.SetActive(value: false);
		}
	}
}
