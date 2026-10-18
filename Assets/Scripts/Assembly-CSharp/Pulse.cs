using DG.Tweening;
using UnityEngine;

public class Pulse : InvisibleEntity
{
	private Vector3 initialScale = Vector3.one;

	public float scale = 0.02f;

	private void Start()
	{
		initialScale = base.transform.localScale;
	}

	public override void OnBeat()
	{
		if (base.enabled)
		{
			base.transform.DOPunchScale(base.transform.localScale * scale, 0.1f).OnComplete(() =>
			{
				base.transform.localScale = initialScale;
			});
		}
	}
}
