using UnityEngine;

internal class SetVisibilityRequirement : RDBase
{
	[SerializeField]
	private bool requireDev = true;

	private void Awake()
	{
		bool active = !requireDev || RDBase.isDev;
		base.gameObject.SetActive(active);
	}
}
