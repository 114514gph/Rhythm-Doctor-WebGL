using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RDDialog : RDBase
{
	public delegate void DialogHideCallback();

	public Text message;

	public List<DialogButton> buttons;

	public Image overlay;

	public float showDuration = 0.3f;

	public Ease showEase = Ease.OutBack;

	public float hideDuration = 0.3f;

	public Ease hideEase = Ease.InBack;

	private Action<int> callback;

	[NonSerialized]
	public bool hideOnEnterPressed;

	public DialogHideCallback onHideComplete;

	private void Awake()
	{
		RDStringToUIText.Apply(message);
		foreach (DialogButton button in buttons)
		{
			RDStringToUIText.Apply(button.label);
		}
	}

	private void Update()
	{
		if (hideOnEnterPressed && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape)))
		{
			Hide();
		}
	}

	public void Show(string message, string[] options, Action<int> callback)
	{
		if (base.gameObject.activeSelf)
		{
			return;
		}
		if (base.editor != null && base.editor.fullscreen)
		{
			base.editor.FullscreenButtonClick();
		}
		if (options.Length > buttons.Count)
		{
			MonoBehaviour.print($"too many options ({options.Length}), please set less options than {buttons.Count}");
			return;
		}
		base.gameObject.SetActive(value: true);
		scrConductor.PlayImmediately("sndEditorPopupLarge", 1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
		ShowOverlay();
		this.message.text = message;
		foreach (DialogButton button in buttons)
		{
			button.gameObject.SetActive(value: false);
			button.ShowUnselected();
		}
		int num = 0;
		foreach (string text in options)
		{
			DialogButton dialogButton = buttons[num];
			dialogButton.gameObject.SetActive(value: true);
			dialogButton.label.text = text;
			if (num == 0)
			{
				dialogButton.ShowSelected();
			}
			num++;
		}
		RectTransform component = GetComponent<RectTransform>();
		Canvas.ForceUpdateCanvases();
		float textHeight = RDUtils.GetTextHeight(this.message, RDString.currentLanguageFont);
		component.sizeDelta = new Vector2(component.sizeDelta.x, textHeight + 50f);
		component.DOKill();
		float num2 = component.sizeDelta.y;
		component.anchoredPosition = component.anchoredPosition.WithY(num2);
		component.DOAnchorPos(Vector2.zero, showDuration).SetEase(showEase).SetUpdate(isIndependentUpdate: true);
		this.callback = callback;
		Narration.Say(message, NarrationCategory.Navigation, false, NarrationActionName.ToConfirmReturn, NarrationActionName.ToGoBack);
	}

	public void Hide(DialogHideCallback onComplete = null)
	{
		scrConductor.PlayImmediately("sndEditorPopdownLarge", 1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"));
		RectTransform component = GetComponent<RectTransform>();
		component.anchoredPosition = component.anchoredPosition.WithY(0f);
		float num = component.sizeDelta.y;
		component.DOAnchorPos(new Vector2(0f, num), hideDuration).SetEase(hideEase).SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				base.gameObject.SetActive(value: false);
				if (onComplete != null)
				{
					onComplete();
				}
				if (onHideComplete != null)
				{
					onHideComplete();
				}
			});
		HideOverlay();
	}

	public void ShowOverlay()
	{
		overlay.gameObject.SetActive(value: true);
		overlay.color = Color.clear;
		overlay.DOKill();
		overlay.DOColor(Color.black.WithAlpha(0.55f), showDuration).SetUpdate(isIndependentUpdate: true);
	}

	public void HideOverlay()
	{
		overlay.DOKill();
		overlay.DOColor(Color.clear, hideDuration).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
		{
			overlay.gameObject.SetActive(value: false);
		});
	}

	public void SelectedOption(DialogButton button)
	{
		int obj = buttons.IndexOf(button);
		if (callback != null)
		{
			callback(obj);
		}
	}
}
