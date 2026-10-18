using RDLevelEditor;

public class Level_SteinwayH : LevelBase
{
	public Level_SteinwayH(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
	}

	public override void preactions()
	{
	}

	public override void actions()
	{
	}

	public void ToggleBirdsVolume(bool on, bool solo)
	{
		float num = (on ? 0.9f : 0f);
		for (int i = 0; i < 4; i++)
		{
			base.rows[i].rowSoundVolMultiplier = ((solo && i != 0) ? 0f : num);
			base.rows[i].rowSoundPan = (GC.twoPlayerMode ? GC.PanP1 : (0.25f * (float)i));
		}
	}

	public void ToggleMrsStevensonVolume(bool on)
	{
		float rowSoundVolMultiplier = (on ? 1f : 0f);
		float rowSoundPan = (GC.twoPlayerMode ? GC.PanP2 : (-0.5f));
		base.rows[4].rowSoundVolMultiplier = rowSoundVolMultiplier;
		base.rows[4].rowSoundPan = rowSoundPan;
	}
}
