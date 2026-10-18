using System;
using UnityEngine;

[Serializable]
public class PauseMenuModeData
{
	public PauseModeName name;

	public string localizedText;

	public bool useLevelDetail;

	[Header("Categories")]
	public bool useCategories;

	public string defaultCategoryLocalizationKey;

	public Sprite categoryIcon;

	public PauseContentName[] contentNames;
}
