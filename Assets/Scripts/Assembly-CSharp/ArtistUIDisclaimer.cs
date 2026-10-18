using UnityEngine;
using UnityEngine.UI;

public class ArtistUIDisclaimer : RDBase
{
	public const string ArtistToken = "[artist]";

	public const string SelectedArtistKey = "editor.ArtistDisclaimer.selected";

	public const string ApprovalLevelToken = "editor.ArtistDisclaimer.";

	public const string ConditionKey = "editor.ArtistDisclaimer.condition";

	public const string ConditionDescKey = "editor.ArtistDisclaimer.conditionDescription";

	public const string ConditionDeclinedDescKey = "editor.ArtistDisclaimer.conditionDeclinedDescription";

	public const string AcceptConditionKey = "editor.ArtistDisclaimer.acceptCondition";

	public const string TitleKey = "editor.ArtistDisclaimer";

	public const string ConfirmKey = "editor.ArtistDisclaimer.confirm";

	public const string CancelKey = "editor.ArtistDisclaimer.cancel";

	public const string CloseKey = "editor.ArtistDisclaimer.close";

	public Text selectedArtistText;

	public Text approvalLevelText;

	public Text conditionText;

	public Text conditionDescriptionText;

	public Text conditionCheckText;

	public GameObject confirm;

	public GameObject cancel;

	public GameObject close;

	public GameObject next;

	public GameObject disclaimerPage;

	public GameObject confirmationPage;

	public Toggle conditionCheckToggle;

	public Text titleText;

	public Text confirmText;

	public Text cancelText;

	public Text closeText;

	public RectTransform rectTransform;

	public GameObject fader;

	private InputField artistField;

	private ArtistData currentArtistData;

	private bool onlyChecking;

	private ApprovalLevelBadge ApprovalLevelBadge
	{
		get
		{
			return base.editor.levelSettingsPanel.approvalLevelBadge;
		}
		set
		{
			base.editor.levelSettingsPanel.approvalLevelBadge = value;
		}
	}

	private void Awake()
	{
		titleText.text = RDString.Get("editor.ArtistDisclaimer");
		titleText.font = RDString.GetAppropiateFontForString(titleText.text);
		confirmText.text = RDString.Get("editor.ArtistDisclaimer.confirm");
		confirmText.font = RDString.GetAppropiateFontForString(confirmText.text);
		cancelText.text = RDString.Get("editor.ArtistDisclaimer.cancel");
		cancelText.font = RDString.GetAppropiateFontForString(cancelText.text);
		closeText.text = RDString.Get("editor.ArtistDisclaimer.close");
		closeText.font = RDString.GetAppropiateFontForString(closeText.text);
	}

	public void SetData(ArtistData data, InputField artistField, bool onlyCheckingMode)
	{
		currentArtistData = data;
		this.artistField = artistField;
		onlyChecking = onlyCheckingMode;
		selectedArtistText.text = RDString.Get("editor.ArtistDisclaimer.selected").Replace("[artist]", data.name);
		selectedArtistText.font = RDString.GetAppropiateFontForString(selectedArtistText.text);
		string key = "editor.ArtistDisclaimer." + data.approvalLevel;
		approvalLevelText.text = RDString.Get(key);
		approvalLevelText.font = RDString.GetAppropiateFontForString(approvalLevelText.text);
		conditionText.text = RDString.Get("editor.ArtistDisclaimer.condition").Replace("[artist]", data.name);
		conditionText.font = RDString.GetAppropiateFontForString(conditionText.text);
		string key2 = ((data.approvalLevel == ApprovalLevel.Declined) ? "editor.ArtistDisclaimer.conditionDeclinedDescription" : "editor.ArtistDisclaimer.conditionDescription");
		conditionDescriptionText.text = RDString.Get(key2);
		conditionDescriptionText.font = RDString.GetAppropiateFontForString(conditionDescriptionText.text);
		if (data.approvalLevel == ApprovalLevel.Declined)
		{
			next.SetActive(value: false);
			cancel.SetActive(value: false);
			close.SetActive(value: true);
		}
		else
		{
			Debug.Log(onlyChecking);
			next.SetActive(!onlyChecking);
			cancel.SetActive(value: true);
			close.SetActive(value: false);
			if (!onlyChecking)
			{
				confirm.GetComponent<Button>().interactable = false;
				conditionCheckToggle.isOn = false;
			}
		}
		fader.SetActive(value: true);
		base.gameObject.SetActive(value: true);
		base.editor.levelSettingsPanel.HideArtistDropdown();
		if (!onlyChecking)
		{
			base.editor.levelSettings.artist = currentArtistData.name;
			artistField.text = currentArtistData.name;
			ApprovalLevelBadge.UpdateUI(currentArtistData.approvalLevel, onlyColor: true);
		}
		ChangePage(toConfirmationPage: false);
	}

	public void ShowArtistDisclaimer()
	{
		Application.OpenURL("https://7thbe.at/verified-artists/rd/artist/" + currentArtistData.id);
	}

	public void Confirm()
	{
		if (!onlyChecking)
		{
			base.editor.levelSettings.artist = currentArtistData.name;
			artistField.text = currentArtistData.name;
			ApprovalLevelBadge.UpdateUI(currentArtistData.approvalLevel, onlyColor: true);
		}
		Hide();
	}

	public void Cancel()
	{
		_ = onlyChecking;
		Hide();
	}

	public void ChangePage(bool toConfirmationPage)
	{
		disclaimerPage.SetActive(!toConfirmationPage);
		confirmationPage.SetActive(toConfirmationPage);
	}

	private void Hide()
	{
		if (!onlyChecking)
		{
			base.editor.levelSettingsPanel.HideArtistDropdown();
		}
		fader.SetActive(value: false);
		base.gameObject.SetActive(value: false);
		base.editor.popupBlocker.gameObject.SetActive(value: false);
	}
}
