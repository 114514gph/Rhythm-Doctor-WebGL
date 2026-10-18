using System;
using System.IO;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.UI;

public class InspectorPanel_CharacterTemplateGenerator : InspectorPanel
{
	public CharacterTemplateGenerator generator;

	public CharacterPicker character;

	public InputField sizeX;

	public InputField sizeY;

	public InputField frameCount;

	public InputField ccName;

	public Button generateButton;

	private Coroutine generatorCo;

	private bool blockSave;

	private new void Awake()
	{
		base.Awake();
		generator = new CharacterTemplateGenerator();
		CharacterPicker characterPicker = character;
		characterPicker.onEndEdit = (CharacterPicker.EndHandler)Delegate.Combine(characterPicker.onEndEdit, (CharacterPicker.EndHandler)delegate
		{
			Save();
		});
		sizeX.onEndEdit.AddListener(delegate
		{
			Save();
		});
		sizeY.onEndEdit.AddListener(delegate
		{
			Save();
		});
		frameCount.onEndEdit.AddListener(delegate
		{
			Save();
		});
		ccName.onEndEdit.AddListener(delegate
		{
			Save();
		});
		generateButton.onClick.AddListener(DoGenerate);
		CharacterPicker characterPicker2 = character;
		characterPicker2.onEndEdit = (CharacterPicker.EndHandler)Delegate.Combine(characterPicker2.onEndEdit, (CharacterPicker.EndHandler)((Character character, string expression, string customCharName) =>
		{
			if (character == Character.New)
			{
				this.character.SetCharacter(Character.Custom);
			}
			Save();
			UpdateUI();
		}));
		character.availableCharacters = RDEditorConstants.AvailableCharacters;
	}

	private void DoGenerate()
	{
		if (generatorCo != null)
		{
			StopCoroutine(generatorCo);
		}
		Save();
		generateButton.GetComponentInChildren<Text>();
		string path = RDEditorUtils.GetCurrentLevelFolderPath();
		if (path.IsNullOrEmpty())
		{
			path = Path.Combine(LevelValidation.CustomCharactersPath, generator.name);
			if (!RDDirectory.Exists(path))
			{
				RDDirectory.CreateDirectory(path);
			}
		}
		generatorCo = StartCoroutine(generator.Generate(path, (bool success) =>
		{
			if (!success)
			{
				UpdateUI();
			}
			else
			{
				scnEditor instance = scnEditor.instance;
				InspectorPanel_MakeRow inspectorPanel_MakeRow = instance.inspectorPanelManager.Get<InspectorPanel_MakeRow>();
				InspectorPanel.ShowPanel(inspectorPanel_MakeRow.transform, RDString.Get("editor.MakeRow"), Tab.Rows);
				instance.selectedRowIndex = generator.targetRow;
				instance.lastUsedRow = generator.targetRow;
				if (generator.targetRow < instance.rowsData.Count)
				{
					inspectorPanel_MakeRow.currentLevelEvent = instance.rowsData[generator.targetRow];
					inspectorPanel_MakeRow.UpdateUI(inspectorPanel_MakeRow.currentLevelEvent);
					((TabSection_Rows)instance.tabSections[1]).UpdateUI();
					inspectorPanel_MakeRow.LoadCustomCharacterFromPath(Path.Combine(path, generator.name + ".json"));
				}
			}
		}));
		UpdateUI();
	}

	private void OnEnable()
	{
		UpdateUI();
	}

	private void UpdateUI()
	{
		blockSave = true;
		Text componentInChildren = generateButton.GetComponentInChildren<Text>();
		generateButton.interactable = !generator.running;
		componentInChildren.text = (generator.running ? RDString.Get("editor.CharacterTemplateGenerator.generating") : RDString.Get("editor.CharacterTemplateGenerator.generate"));
		character.SetCharacter(generator.character);
		ccName.text = generator.name;
		sizeX.text = generator.resolution.x.ToString();
		sizeY.text = generator.resolution.y.ToString();
		frameCount.text = generator.frameCount.ToString();
		bool active = generator.character == Character.Custom;
		frameCount.transform.parent.gameObject.SetActive(active);
		sizeX.transform.parent.parent.gameObject.SetActive(active);
		blockSave = false;
	}

	private void Save()
	{
		if (!blockSave)
		{
			generator.character = character.character;
			generator.name = ccName.text;
			generator.resolution = new Vector2Int(int.Parse(sizeX.text), int.Parse(sizeY.text));
			generator.frameCount = int.Parse(frameCount.text);
			generator.ValidateValues();
			UpdateUI();
		}
	}
}
