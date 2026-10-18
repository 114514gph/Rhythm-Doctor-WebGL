using System.Collections;
using RDLevelEditor;
using UnityEngine;

public class Level_Tutorial_Lofi : LevelBase
{
	private Spider spider;

	public Level_Tutorial_Lofi(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		GameObject gameObject = LoadPublicPrefab("Spiders/Spiders");
		spider = gameObject.GetComponent<Spider>();
		spider.transform.LocalMoveX(112f);
		spider.transform.LocalMoveY(320f);
		spider.gameObject.SetActive(value: false);
		yield return null;
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

	public void LowerSpider(bool bigSpider)
	{
		if (!spider.gameObject.activeSelf)
		{
			spider.gameObject.SetActive(value: true);
		}
		spider.Lower(bigSpider);
	}

	public void RaiseSpider(bool bigSpider)
	{
		if (!spider.gameObject.activeSelf)
		{
			spider.gameObject.SetActive(value: true);
		}
		spider.Raise(bigSpider);
	}

	public void ToggleHand(bool enable)
	{
		spider.handTrans.gameObject.SetActive(enable);
	}
}
