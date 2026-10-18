using DG.Tweening;
using UnityEngine;

public class PulseGeneral : RDBase
{
	private Vector3 initialScale = Vector3.one;

	public float scale = 0.02f;

	private new scrConductor conductor;

	private void Start()
	{
		initialScale = base.transform.localScale;
		conductor = base.conductor;
	}

	private void Update()
	{
		if (base.enabled && conductor.lastOnBeatFrame == Time.frameCount)
		{
			base.transform.DOPunchScale(base.transform.localScale * scale, 0.1f).OnComplete(() =>
			{
				base.transform.localScale = initialScale;
			});
		}
	}
}
