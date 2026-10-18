using System;

public class ClassicRowController : RDBase
{
	public scrBeatbox[] beatboxes;

	public float shineSpeed;

	[NonSerialized]
	public float shine;

	private void LateUpdate()
	{
		float num = 1f / base.conductor.crotchet * shineSpeed;
		shine += RDTime.deltaTimeIndependentPauseAffected * num;
		scrBeatbox[] array = beatboxes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].shine = shine + 0.4f;
		}
	}

	public void SetVisible(bool show)
	{
		scrBeatbox[] array = beatboxes;
		foreach (scrBeatbox scrBeatbox2 in array)
		{
			scrBeatbox2.visible = show;
			Entity beatSkip = scrBeatbox2.beatSkip;
			int visible;
			if (show)
			{
				scrBeatbox.XState xState = scrBeatbox2.CheckXState();
				visible = ((xState == scrBeatbox.XState.Gap || xState == scrBeatbox.XState.On) ? 1 : 0);
			}
			else
			{
				visible = 0;
			}
			beatSkip.visible = (byte)visible != 0;
		}
	}
}
