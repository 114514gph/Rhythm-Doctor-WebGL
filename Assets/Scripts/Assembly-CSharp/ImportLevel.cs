using System;
using System.Collections.Generic;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.UI;

public class ImportLevel : MonoBehaviour
{
	public Button checkIssuesButton;

	public GameObject progressImage;

	public GameObject progressText;

	public Text infoText;

	public CustomLevelData customLevel;

	[NonSerialized]
	public RDLevelSettings settings;

	[NonSerialized]
	public bool isUrl;

	[NonSerialized]
	public string path;

	[NonSerialized]
	public List<LevelErrorName> errorsName;

	[NonSerialized]
	public bool canToggleRemoveButton;

	[NonSerialized]
	public bool canToggleCheckIssuesButton;

	[NonSerialized]
	public string errorToken;

	[NonSerialized]
	public Text progress;

	private void Awake()
	{
		progress = progressText.GetComponent<Text>();
	}

	private void Start()
	{
		canToggleRemoveButton = true;
		if (RDString.isCJK || Persistence.GetReducePixelFontUsage())
		{
			infoText.fontSize = 6;
		}
	}
}
