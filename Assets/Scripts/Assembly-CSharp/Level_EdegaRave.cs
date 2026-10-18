using System;
using System.Collections;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_EdegaRave : LevelBase
{
	public ShatteredGlass glass;

	public Level_EdegaRave(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		GameObject gameObject = LoadPublicPrefab("ShatteredGlass/ShatteredGlass");
		glass = gameObject.GetComponent<ShatteredGlass>();
		glass.transform.position = base.room2.transform.position;
		glass.gameObject.SetActive(value: false);
		cutsceneBarsDesc = new (int, int)[1] { (20, 25) };
		yield return null;
	}

	public void TintRecordsRoomForeground(int room, string colorHex, float opacity, float beats, string easeStr)
	{
		Ease ease = Enum.Parse<Ease>(easeStr);
		Color endValue = colorHex.HexToColor();
		tk2dSprite foreground = base.game.rooms[room].recordsRoom.GetComponent<RDRecordsRoom>().foreground;
		MeshRenderer component = foreground.GetComponent<MeshRenderer>();
		float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, beats);
		component.material.DOColor(endValue, "_ColorOverlay", duration).SetEase(ease);
		foreground.DOColor(foreground.color.WithAlpha(opacity), duration).SetEase(ease);
	}

	public void ShatterGlass()
	{
		glass.Shatter(base.room2);
	}

	public void SkipRank()
	{
		skipRankText = true;
	}
}
