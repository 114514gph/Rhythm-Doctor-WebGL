using UnityEngine;
using UnityEngine.UI;

public class RDWrldLevelURL : scnBase
{
	public InputField url;

	public InputField find;

	public Text status;

	public Button downloadButton;

	public Button playButton;

	public Button playerButton;

	public Text playerButtonLabel;

	public Text numberOfLevelsToDownload;

	public RectTransform scrollViewContent;

	public GameObject listItemPrefab;

	public Toggle skipDownloadedLevels;
}
