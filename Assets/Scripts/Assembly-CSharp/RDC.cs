using RDLevelEditor;

public static class RDC
{
	public static bool pauseEditorAtTheEndOfThisFrame;

	public static RDConstants data => RDConstants.data;

	public static WindowMovement windowMovement
	{
		get
		{
			if (!scnBase.isMobile && !scnBase.isXbox && !scnBase.isSwitch)
			{
				if (!(scnEditor.instance != null))
				{
					return DebugSettings.instance.WindowMovement;
				}
				return Persistence.GetLevelEditorWindowMovement();
			}
			return WindowMovement.Simulate;
		}
		set
		{
			DebugSettings.instance.WindowMovement = value;
		}
	}

	public static bool windowDance => windowMovement != WindowMovement.Off;

	public static bool retrieveEnglishOnly
	{
		get
		{
			return data.retrieveEnglishOnly;
		}
		set
		{
			data.retrieveEnglishOnly = value;
		}
	}

	public static bool retrieveKoreanOnly
	{
		get
		{
			return data.retrieveKoreanOnly;
		}
		set
		{
			data.retrieveKoreanOnly = value;
		}
	}

	public static bool retrieveChineseOnly
	{
		get
		{
			return data.retrieveChineseOnly;
		}
		set
		{
			data.retrieveChineseOnly = value;
		}
	}

	public static bool compileOnlyLastImportedInk
	{
		get
		{
			return data.compileOnlyLastImportedInk;
		}
		set
		{
			data.compileOnlyLastImportedInk = value;
		}
	}

	public static bool booth
	{
		get
		{
			return data.booth;
		}
		set
		{
			data.booth = value;
		}
	}

	public static void UpdateWindowMovement(WindowMovement wm)
	{
		windowMovement = wm;
		Persistence.SetWindowMovement(windowMovement);
		PlatformHelper.instance.ResetWindowStageValues();
		Persistence.generalPrefs.Save();
	}
}
