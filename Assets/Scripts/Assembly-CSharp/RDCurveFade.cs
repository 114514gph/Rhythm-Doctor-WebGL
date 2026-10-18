using UnityEngine;

public class RDCurveFade : RDBase
{
	public RDAnimationCurves curves;

	private AnimationCurve currentCurve;

	public string fadingCurve
	{
		set
		{
			currentCurve = curves.Get(value);
		}
	}

	private void Update()
	{
		if (currentCurve != null && !base.game.audioPaused)
		{
			float time = ((float)(base.conductor.visualPos - base.conductor.startOfNextBar) + RDCalibration.calibration_v + RDCalibration.latency) / (base.conductor.crotchet * (float)base.conductor.crotchetsPerBar);
			RDBase.Vfx.hudCameraOverlay.alpha = currentCurve.Evaluate(time);
		}
	}
}
