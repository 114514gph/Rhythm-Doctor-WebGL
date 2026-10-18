using System.IO;
using System.Text.RegularExpressions;
using RDLevelEditor;
using UnityEngine;

public class RDRichPresence : RDBase
{
	public static RDRichPresence Instance;

	private RDRichPresenceHandler richPresenceHandler;

	public string cachedSong = "";

	public string cachedLevelName = "";

	public bool isCustomLevel;

	public LevelType cachedLevelType;

	public static bool IsAvailable
	{
		get
		{
			if (!scnBase.isDesktop)
			{
				return scnBase.isXbox;
			}
			return true;
		}
	}

	public static string UserName => Instance?.richPresenceHandler?.UserName;

	public static long UserId => Instance?.richPresenceHandler?.UserId ?? (-1);

	private void Awake()
	{
		if (!IsAvailable)
		{
			Object.Destroy(base.gameObject);
		}
		if (Instance == null)
		{
			Instance = this;
			richPresenceHandler = new RDRichPresence_Discord();
			richPresenceHandler.Setup();
			Object.DontDestroyOnLoad(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDisable()
	{
		if (IsAvailable)
		{
			richPresenceHandler?.ClearPresence();
			richPresenceHandler?.Dispose();
		}
	}

	private void Update()
	{
		if (!IsAvailable)
		{
			return;
		}
		richPresenceHandler?.Update();
		scnEditor instance = scnEditor.instance;
		_ = scnGame.instance;
		string text = string.Empty;
		if ((bool)instance)
		{
			if (!string.IsNullOrEmpty(instance.levelSettings.song))
			{
				text = Regex.Replace(instance.levelSettings.song, "<\\/?color.*?>", string.Empty);
			}
			else if (!string.IsNullOrEmpty(instance.openedFilePath))
			{
				text = Path.GetFileName(instance.openedFilePath);
			}
		}
		bool flag = false;
		if (text != cachedSong)
		{
			cachedSong = text;
			flag = true;
		}
		if (flag)
		{
			UpdatePresence();
		}
	}

	public void UpdatePresence()
	{
		if (!IsAvailable)
		{
			return;
		}
		DiscordPresence mode = richPresenceHandler.Mode;
		if (mode == DiscordPresence.Disabled)
		{
			richPresenceHandler.ClearPresence();
			return;
		}
		string details = string.Empty;
		string state = string.Empty;
		bool flag = mode == DiscordPresence.Full;
		bool flag2 = flag || mode == DiscordPresence.NoEditor;
		bool flag3 = flag2 || mode == DiscordPresence.NoCustom;
		if (base.cls != null)
		{
			details = "inCustomLevelSelect";
		}
		else if (scnBase.instance is scnRhythmWeightlifter)
		{
			details = "rhythmWeightlifter";
			if (flag3)
			{
				state = richPresenceHandler.GetRWState();
			}
		}
		else if (scnBase.instance is scnMenu)
		{
			cachedSong = "";
			details = "inMainMenu";
		}
		else if (scnEditor.instance != null)
		{
			details = "inLevelEditor";
			if (flag)
			{
				state = richPresenceHandler.GetEditorState(cachedSong);
			}
		}
		else if (scnBase.instance is scnExpoMenu || scnBase.instance is scnLevelSelect)
		{
			cachedSong = "";
			details = "inLevelSelect";
		}
		else if ((object)scnGame.instance != null)
		{
			cachedSong = "";
			details = "";
			switch (cachedLevelType)
			{
			case LevelType.Tutorial:
				state = RDString.Get("discord.tutorial");
				break;
			case LevelType.Cutscene:
				details = "inLevelSelect";
				break;
			default:
				details = "defibrillating";
				if (isCustomLevel ? flag2 : flag3)
				{
					state = richPresenceHandler.GetGameState(cachedLevelName);
				}
				break;
			}
		}
		richPresenceHandler.SetPresence(state, details);
	}
}
