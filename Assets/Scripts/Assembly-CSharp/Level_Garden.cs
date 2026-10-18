using System.Collections;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_Garden : LevelBase
{
	private const int lightsNumber = 16;

	private GameObject[] lightBackgrounds;

	private scrChar couple;

	private ParticleSystem leaves1;

	private ParticleSystem leaves2;

	private ParticleSystem leaves3;

	public Level_Garden(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		base.room2.SetRowContainerY(-30f);
		yield return null;
	}

	public void MoveCouple(int x)
	{
		float duration = 1.1370001f;
		couple.transform.DOLocalMoveX(x, duration).SetEase(Ease.InOutQuad);
	}

	public void ShowCouple()
	{
		couple.gameObject.SetActive(value: true);
		couple.PlayExpression("dancing");
		couple.shaderDataSource = couple;
		couple.shaderData = base.game.rows[0].ent.shaderData;
		couple.shaderData.SetPropertiesInRenderer(couple.shaderRenderer);
	}

	public void HideCouple()
	{
		couple.gameObject.SetActive(value: false);
	}

	public void ShowLeaves()
	{
		leaves1 = RDClass.Vfx.ShowParticlePreset("leaves1");
		leaves2 = RDClass.Vfx.ShowParticlePreset("leaves2");
		leaves3 = RDClass.Vfx.ShowParticlePreset("leaves3");
		leaves1.gameObject.SetActive(value: true);
		leaves2.gameObject.SetActive(value: true);
		leaves3.gameObject.SetActive(value: true);
	}

	public override void preactions()
	{
		base.conductor.visualBeatLengthMultiplier = 3f;
		if (base.conductor.barNumber == 1)
		{
			if (couple != null)
			{
				Object.Destroy(couple.gameObject);
			}
			couple = scrChar.Create(Character.DancingCouple, "dancing");
			GameObject gameObject = couple.gameObject;
			gameObject.transform.parent = base.game.rooms[0].rowContainer;
			gameObject.transform.LocalMoveXY(83f, 80f);
			gameObject.name = "CoupleDummy";
			couple.gameObject.SetActive(value: false);
		}
	}
}
