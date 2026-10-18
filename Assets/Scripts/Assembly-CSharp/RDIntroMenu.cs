using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class RDIntroMenu : RDBase
{
	public Transform LogoContainer;

	public tk2dSprite LogoStatic;

	public tk2dSpriteAnimator LogoAnimated;

	public Transform TextSwitchMobile;

	public Transform TextSwitchDesktop;

	public GameObject DoThisToStartDekstop;

	public GameObject DoThisToStartMobile;

	public GameObject TipDesktop;

	public GameObject TipMobile;

	[NonSerialized]
	public TextMesh tip;

	public TextSwitch textSwitch;

	public GameObject doThisToStart;

	private void Awake()
	{
		bool isMobile = scnBase.isMobile;
		Transform transform = (isMobile ? TextSwitchMobile : TextSwitchDesktop);
		textSwitch = transform.GetComponent<TextSwitch>();
		textSwitch.gameObject.SetActive(value: false);
		doThisToStart = (isMobile ? DoThisToStartMobile : DoThisToStartDekstop);
		doThisToStart.SetActive(value: false);
		tip = (isMobile ? TipMobile : TipDesktop).GetComponent<TextMesh>();
	}

	public void FadeOut(float duration = 1.8f)
	{
		StartCoroutine(FadeOutCoroutine(duration));
	}

	private IEnumerator FadeOutCoroutine(float duration)
	{
		textSwitch.gameObject.SetActive(value: false);
		LogoContainer.DOLocalMoveY(LogoContainer.localPosition.y + 40f, duration);
		LogoAnimated.GetComponent<tk2dSprite>().DOColor(RDPresetColors.WhiteZeroAlpha, duration);
		LogoStatic.DOColor(RDPresetColors.WhiteZeroAlpha, duration);
		(scnBase.isMobile ? DoThisToStartMobile : DoThisToStartDekstop).gameObject.SetActive(value: false);
		TipDesktop.gameObject.SetActive(value: false);
		TipMobile.gameObject.SetActive(value: false);
		yield return null;
	}
}
