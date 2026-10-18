using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelImporterInfoSection : MonoBehaviour
{
	public List<ImportLevel> levels;

	public string titleKey;

	public Text titleText;

	public Button clearButton;

	public void Add(ImportLevel level, string infoText, int siblingIndex, bool showCheckIssues)
	{
		level.infoText.text = infoText;
		level.infoText.font = RDString.GetAppropiateFontForString(infoText);
		level.transform.SetSiblingIndex(siblingIndex);
		levels.Add(level);
		titleText.text = RDString.Get(titleKey) + " (" + levels.Count + ")";
		titleText.font = RDString.GetAppropiateFontForString(titleText.text);
		if (levels.Count == 1)
		{
			base.gameObject.SetActive(value: true);
		}
	}

	public void Remove(ImportLevel level)
	{
		levels.Remove(level);
		titleText.text = RDString.Get(titleKey) + " (" + levels.Count + ")";
		if (levels.Count == 0)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Clear()
	{
		foreach (ImportLevel level in levels)
		{
			if (level != null)
			{
				Object.Destroy(level.gameObject);
			}
		}
		levels = new List<ImportLevel>();
		base.gameObject.SetActive(value: false);
	}
}
