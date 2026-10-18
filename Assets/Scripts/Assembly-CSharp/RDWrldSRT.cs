using System;
using System.Collections;
using System.IO;
using RDLevelEditor;
using UnityEngine;
using UnityFileDialog;

public class RDWrldSRT : scnBase
{
	private const string LoadLevelString = "Press L to load a level";

	private bool didPressSpace;

	public TextMesh subtitle;

	public TextMesh pressSpace;

	public static string errorMessage;

	public static string loadedFilePath;

	private new void Awake()
	{
		base.Awake();
		scnBase._instance = this;
		scnBase.currentLevelSelect = "scnRDSRT";
		if (loadedFilePath != null)
		{
			UpdateLabel();
		}
		else if (string.IsNullOrEmpty(errorMessage))
		{
			pressSpace.text = "Press L to load a level";
		}
		else
		{
			pressSpace.text = "<color=red>" + errorMessage + "</color>\nPress L to load a level";
		}
	}

	private new void Start()
	{
		base.Start();
		base.conductor.SetBPM(82f);
		base.conductor.crotchetsPerBar = 4;
		base.conductor.OnPreBar();
		RDBase.Vfx.hudCameraOverlay.gameObject.SetActive(value: false);
	}

	private new void Update()
	{
		base.Update();
		if (Input.GetKey(KeyCode.Alpha1) && Input.GetKey(KeyCode.Alpha2) && Input.GetKey(KeyCode.Alpha3) && Input.GetKey(KeyCode.Alpha4))
		{
			pressSpace.text = "<color=red>Defaults deleted</color>\nPress L to load a level";
			PlayerPrefs.DeleteAll();
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			StartCoroutine(GetFile());
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			scnBase.GoToScene("scnSleevePaint");
		}
		if (!didPressSpace)
		{
			if (Input.GetKeyDown(KeyCode.Space) && !string.IsNullOrEmpty(loadedFilePath))
			{
				didPressSpace = true;
				scnBase.GoToLevelWithExternalPath(loadedFilePath);
			}
			scrVfxControl.CheckKeyComboForResolutionChange();
		}
	}

	private IEnumerator GetFile()
	{
		string text = FileBrowser.PickFile(scnEditor.GetLastUsedFolder(), RDString.Get("editor.dialog.levelFileFormat"), new string[3] { "rdlevel", "rdzip", "json" }, RDString.Get("editor.dialog.openFile"));
		if (!string.IsNullOrEmpty(text))
		{
			string text2 = Path.GetExtension(text).Replace(".", string.Empty).ToLowerInvariant();
			loadedFilePath = Uri.UnescapeDataString(text.Replace("file:", ""));
			if (text2 == "rdzip")
			{
				loadedFilePath = RDPackageInstaller.UnzipAndFindLevel(loadedFilePath);
				printe("new loaded file path: " + loadedFilePath);
			}
			Path.GetFileName(loadedFilePath);
			UpdateLabel();
		}
		yield break;
	}

	private void UpdateLabel()
	{
		pressSpace.text = "Press L to load a level\nPress space to start <color=yellow>" + Path.GetFileName(loadedFilePath) + "</color>";
	}

	public override void OnPreBar()
	{
		base.OnPreBar();
		if ((base.conductor.barNumber - 1) % 17 == 0)
		{
			base.conductor.PlaySong("sndIntro2", LevelBase.musicGroup);
		}
	}
}
