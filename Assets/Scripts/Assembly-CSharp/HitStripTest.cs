using UnityEngine;

public class HitStripTest : RDBase
{
	public RDHitStrip hitStrip;

	private void Awake()
	{
		Time.timeScale = 1f;
	}

	private void Update()
	{
		if (Time.frameCount == 30)
		{
			hitStrip.Flash(hold: true);
		}
	}
}
