using System.Collections;
using RDLevelEditor;
using UnityEngine;

public class Level_Blurred : LevelBase
{
	public Level_Blurred(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		cutsceneBarsDesc = new (int, int)[2]
		{
			(26, 47),
			(0, 9)
		};
		yield return null;
	}

	public void RemoveTVs()
	{
		base.room0.hospitalWard.GetComponent<RDMainWard>().ToggleTVs(enabled: false, hideEntirely: true);
	}

	public void DisableCharacterTinting(int row)
	{
		RowEntity ent = base.game.rows[row].ent;
		ent.tintableEntities.Remove(ent);
		ent.character.usesOwnShaderData = true;
	}

	public void TintCharacter(int row, string colorHex, float duration)
	{
		colorHex.TrimStart('#');
		Color tintColor = colorHex.HexToColor();
		base.game.rows[row].ent.character.TweenColor(Color.black.WithAlpha(0.5f), tintColor, duration);
	}
}
