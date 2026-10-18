using UnityEngine;

public class SubdivisionCharCenterRotation : RDBase
{
	private void Update()
	{
		float z = base.transform.parent.parent.localEulerAngles.z;
		float z2 = base.transform.parent.localEulerAngles.z;
		base.transform.localEulerAngles = new Vector3(0f, 0f, 0f - z - z2);
		float num = ((base.transform.position.x > RDBase.Vfx.RDWidth * 0.5f) ? (-1f) : 1f);
		base.transform.localScale = new Vector3(num, 1f, 1f);
	}
}
