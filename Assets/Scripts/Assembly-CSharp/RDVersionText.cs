using UnityEngine;
using UnityEngine.UI;

public class RDVersionText : MonoBehaviour
{
	public Text text;

	private int page;

	private void Awake()
	{
		SetPage(0);
	}

	public void NextPage()
	{
		SetPage((page == 0) ? 1 : 0);
	}

	private void SetPage(int page)
	{
		this.page = page;
		string text = "";
		if (page == 0)
		{
			text = "7th Beat Games";
		}
		this.text.text = ((page == 0) ? (text + " / v" + Application.version) : $"r{Releases.releaseNumber} ({RDBase.platform}, {Releases.buildCommit}, {Releases.buildDate})");
	}
}
