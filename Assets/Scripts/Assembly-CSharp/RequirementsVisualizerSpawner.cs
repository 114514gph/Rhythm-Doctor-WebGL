using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequirementsVisualizerSpawner : MonoBehaviour
{
	public LevelConditionVisualizer requirementsVisualizer;

	public LevelErrorContainer levelErrorContainer;

	public List<LevelSettingName> settings;

	public GameObject levelSettingsHelper;

	public Vector2 helperPosition;

	private RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	private void Start()
	{
		SettingErrorLocalization[] showableErrors = levelErrorContainer.showableErrors;
		foreach (SettingErrorLocalization settingErrorLocalization in showableErrors)
		{
			if (!settings.Contains(settingErrorLocalization.settingName))
			{
				continue;
			}
			foreach (SettingCondition error in settingErrorLocalization.errors)
			{
				if (error.recommended)
				{
					GameObject obj = Object.Instantiate(levelSettingsHelper, rectTransform);
					obj.GetComponent<RectTransform>().anchoredPosition = helperPosition;
					obj.GetComponent<Button>().onClick.AddListener(() =>
					{
						requirementsVisualizer.Show(rectTransform, settings);
					});
				}
			}
		}
	}
}
