using UnityEngine;

public class RDCenterCamera : RDBase
{
	private void Start()
	{
		base.transform.localPosition = new Vector3(RDBase.Vfx.RDWidth / 2f, RDBase.Vfx.RDHeight / 2f);
	}
}
