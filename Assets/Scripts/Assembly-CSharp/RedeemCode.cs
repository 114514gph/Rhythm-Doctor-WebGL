using DG.Tweening;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.UI;

public class RedeemCode : MonoBehaviour
{
	public EntitlementsService entitlements;

	public CanvasGroup canvasGroup;

	public InputField inputField;

	public Text redeemText;

	public Text messageText;

	public Button redeemButton;

	public Button cancelButton;

	private bool isLoading;

	public bool isEnabled => canvasGroup.interactable;

	private void Awake()
	{
		canvasGroup.alpha = 0f;
		CanvasGroup obj = canvasGroup;
		bool interactable = (canvasGroup.blocksRaycasts = false);
		obj.interactable = interactable;
		inputField.onSubmit.AddListener(ReedemCode);
		redeemButton.onClick.AddListener(ReedemCode);
		cancelButton.onClick.AddListener(Cancel);
	}

	private void Update()
	{
		if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.X))
		{
			EnablePanel(!canvasGroup.interactable);
		}
	}

	private void EnablePanel(bool enable)
	{
		if (enable)
		{
			canvasGroup.gameObject.SetActive(value: true);
		}
		CanvasGroup obj = canvasGroup;
		bool interactable = (canvasGroup.blocksRaycasts = enable);
		obj.interactable = interactable;
		canvasGroup.DOFade(enable ? 1f : 0f, 0.5f).OnComplete(() =>
		{
			if (enable)
			{
				inputField.Select();
			}
			else
			{
				canvasGroup.gameObject.SetActive(value: false);
			}
		});
	}

	private void SetLoading(bool enable)
	{
		isLoading = enable;
		redeemText.text = (enable ? RDString.Get("editor.loading") : RDString.Get("levelSelect.entitlement.redeem"));
	}

	public void ReedemCode()
	{
		ReedemCode(inputField.text);
	}

	public void ReedemCode(string code)
	{
		if (!isLoading)
		{
			SetLoading(enable: true);
			entitlements.RedeemCode(code, OnRedeemComplete);
		}
	}

	public void OnRedeemComplete(bool sucesfull, string message)
	{
		SetLoading(enable: false);
		messageText.text = RDString.Get(message);
		DOVirtual.DelayedCall(sucesfull ? 5f : 3f, () =>
		{
			Cancel();
		});
	}

	public void Cancel()
	{
		SetLoading(enable: false);
		EnablePanel(enable: false);
	}
}
