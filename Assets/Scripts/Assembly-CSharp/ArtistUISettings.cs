using UnityEngine.UI;

public class ArtistUISettings : RDBase
{
	public Text artistText;

	public Button button;

	public ApprovalLevelBadge badge;

	public void SetData(string artist, ApprovalLevel approvalLevel)
	{
		base.gameObject.SetActive(value: true);
		artistText.text = artist;
		artistText.font = RDString.GetAppropiateFontForString(artistText.text);
		badge.button.enabled = false;
		badge.UpdateUI(approvalLevel);
	}
}
