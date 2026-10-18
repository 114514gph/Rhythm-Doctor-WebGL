using System.Collections;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_Unbeatable : LevelBase
{
	private scrChar trebleChar;

	private scrChar clefChar;

	private RDTiledBackground slidingPortrait;

	private Texture2D[] portraits;

	private bool loadedTextures;

	public Level_Unbeatable(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		if (slidingPortrait != null)
		{
			Object.Destroy(slidingPortrait.gameObject);
		}
		if (!loadedTextures)
		{
			loadedTextures = true;
			portraits = new Texture2D[4]
			{
				RDEditorUtils.LoadLevelTexture(scnGame.levelToLoadSource, "quaverportrait.png"),
				RDEditorUtils.LoadLevelTexture(scnGame.levelToLoadSource, "beatportrait.png"),
				RDEditorUtils.LoadLevelTexture(scnGame.levelToLoadSource, "trebleportrait.png"),
				RDEditorUtils.LoadLevelTexture(scnGame.levelToLoadSource, "clefportrait.png")
			};
		}
		GameObject gameObject = Object.Instantiate(scnGame.instance.customBackground);
		gameObject.transform.SetParent(scrGameManager.instance.parentContainer);
		gameObject.layer = 11;
		gameObject.transform.localPosition = new Vector3(420f, 80f, 10f);
		gameObject.name = "SlidingPortrait";
		slidingPortrait = gameObject.GetComponent<RDTiledBackground>();
		slidingPortrait.gameObject.SetActive(value: false);
		yield return null;
	}

	public override void preactions()
	{
		trebleChar = base.rows[4].ent.character;
		clefChar = base.rows[3].ent.character;
	}

	public override void actions()
	{
		if (base.conductor.barNumber != 59)
		{
			return;
		}
		scrExecuteOnUpdate.Add(() =>
		{
			if (clefChar.customAnimation.currentClip != null)
			{
				string name = clefChar.customAnimation.currentClip.name;
				trebleChar.customAnimation.currentClip = trebleChar.customAnimation.data.clips[name];
			}
		});
	}

	public void ShowPortrait(int portrait)
	{
		slidingPortrait.DOKill();
		Texture2D[] textures = new Texture2D[1] { portraits[portrait] };
		slidingPortrait.Show(textures, ContentMode.Center, FilterMode.Point, Color.white, RDSortingLayer.Foreground);
		slidingPortrait.transform.MoveX(420f);
		slidingPortrait.transform.DOMoveX(295f, 1.5f).SetEase(Ease.OutSine);
		slidingPortrait.material.color = Color.white;
		slidingPortrait.material.DOFade(0f, 1f).SetDelay(0.7f);
		slidingPortrait.gameObject.SetActive(value: true);
	}
}
