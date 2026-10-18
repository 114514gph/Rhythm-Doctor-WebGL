using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FakePopup : RDBase
{
	public RectTransform singlePopup;

	public RectTransform manyPopupsContainer;

	private List<RectTransform> popups;

	private Coroutine onePopupCoroutine;

	public static FakePopup Load(RuntimePlatform platform)
	{
		string text = ((platform != RuntimePlatform.Android) ? "iOS" : "Android");
		string text2 = text;
		return Object.Instantiate(Resources.Load<GameObject>("FakePopup_" + text2), scnGame.instance.uiCanvas.transform).GetComponent<FakePopup>();
	}

	public void ShowSinglePopup()
	{
		if (!base.conductor.isScrubbing)
		{
			singlePopup.gameObject.SetActive(value: true);
			scrConductor.PlayImmediately("note", 1f, RDUtils.GetMixerGroup("Distractions"), RDTime.speed);
			Narration.Say(RDString.Get("level.InsomniacHard.distraction"), NarrationCategory.Notification, false);
			onePopupCoroutine = StartCoroutine(ShowSinglePopupCo());
		}
	}

	private IEnumerator ShowSinglePopupCo()
	{
		RectTransform t = singlePopup;
		Material material = singlePopup.GetComponent<Image>().material;
		material.SetColor("_ColorOverlay", Color.white.WithAlpha(0f));
		material.SetFloat("_Opacity", 1f);
		scrQuad roomOverlay = RDBase.Vfx.GetRoomOverlay(-1);
		roomOverlay.sprite.color = Color.clear;
		roomOverlay.sprite.DOFade(0.5f, 0.5f);
		float initialPosY = t.anchoredPosition3D.y;
		t.DOAnchorPos(Vector2.zero, 0.5f, snapping: true).SetEase(Ease.InOutQuad);
		yield return new WaitForSeconds(6f);
		t.DOAnchorPos3D(Vector2.up * initialPosY, 0.5f, snapping: true).SetEase(Ease.InOutQuad);
		roomOverlay.sprite.DOFade(0f, 0.5f);
	}

	public void ShowManyPopups()
	{
		manyPopupsContainer.gameObject.SetActive(value: true);
		manyPopupsContainer.localScale = Vector3.one;
		RectTransform[] componentsInChildren = manyPopupsContainer.GetComponentsInChildren<RectTransform>();
		popups = new List<RectTransform>();
		popups.AddRange(componentsInChildren);
		popups.Remove(manyPopupsContainer);
		int num = 0;
		foreach (RectTransform popup in popups)
		{
			popup.localScale = Vector3.zero;
			popup.DOScale(Vector3.one, 0.4f).SetDelay((float)num * 0.04f).SetEase(Ease.InOutBack)
				.OnComplete(() =>
				{
					scrConductor.PlayImmediately("note", 1f, RDUtils.GetMixerGroup("Distractions"), RDTime.speed);
					Narration.Say(RDString.Get("level.InsomniacHard.distractionShort"), NarrationCategory.Notification, true);
				});
			num++;
		}
	}

	public void KillOnePopup()
	{
		if (onePopupCoroutine != null)
		{
			StopCoroutine(onePopupCoroutine);
			Material material = singlePopup.GetComponent<Image>().material;
			material.SetColor("_ColorOverlay", Color.white);
			material.DOFloat(0f, "_Opacity", 0.5f);
			RDBase.Vfx.GetRoomOverlay(-1).sprite.DOFade(0f, 0.5f).OnComplete(() =>
			{
				singlePopup.gameObject.SetActive(value: false);
			});
		}
	}

	public void KillManyPopups()
	{
		if (popups == null)
		{
			return;
		}
		int num = 0;
		foreach (RectTransform popup in popups)
		{
			popup.DOScale(Vector3.one * 0f, 0.2f).SetDelay((float)num * 0.015f).SetEase(Ease.InOutBack);
			num++;
		}
		DOVirtual.DelayedCall(0.015f * (float)popups.Count + 0.2f, () =>
		{
			manyPopupsContainer.gameObject.SetActive(value: false);
		});
	}
}
