using UnityEngine.UI;

public class EnableGraphicIfDebug : RDBase
{
	public Graphic targetGraphic;

	private void Awake()
	{
		if (targetGraphic == null)
		{
			targetGraphic = GetComponent<Graphic>();
		}
	}

	private void LateUpdate()
	{
		targetGraphic.enabled = RDBase.debugSettings.Debug;
	}
}
