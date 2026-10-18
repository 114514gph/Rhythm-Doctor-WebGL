using System.Collections.Generic;
using UnityEngine;

public class Narration : RDBase
{
	[HideInInspector]
	public static bool lockToggleHotkeys;

	[HideInInspector]
	public static bool printToConsole;

	[SerializeField]
	private NarrationData data;

	private static Narration instance;

	private NarrationHandler narrationHandler;

	private List<NarrationAction> currentActions = new List<NarrationAction>();

	private List<(NarrationCategory category, string message)> messages = new List<(NarrationCategory, string)>();

	private bool isEnabled;

	private bool navigationMessagesEnabled = true;

	private string currentSpeech = string.Empty;

	private string instructionSpeech = string.Empty;

	private int lastNarratedRowID = -1;

	private string lastNonInstructionSpeech = string.Empty;

	private ElementType lastElementType;

	private int cachedItemsLength;

	private bool firstCheckToSAPI;

	[HideInInspector]
	public static bool IsEnabled => instance.isEnabled;

	[HideInInspector]
	public static bool IsAvailable
	{
		get
		{
			if (!scnBase.isMac && !scnBase.isWindows)
			{
				return scnBase.isLinux;
			}
			return true;
		}
	}

	public static string ShortPause => RDString.Get("narration.shortPause");

	public static string ShortPause1 => RDString.Get("narration.shortPause1");

	public static string ShortPause2 => RDString.Get("narration.shortPause2");

	public static string LongPause => RDString.Get("narration.longPause");

	public static string LongPause1 => RDString.Get("narration.longPause1");

	public static string LongPause2 => RDString.Get("narration.longPause2");

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			narrationHandler = new NarrationHandler_Default();
			Object.DontDestroyOnLoad(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if (IsAvailable)
		{
			if (!firstCheckToSAPI)
			{
				firstCheckToSAPI = true;
				ToggleSAPI(Persistence.GetForceSAPINarration());
			}
			if (Input.GetKeyDown(KeyCode.F8) && !lockToggleHotkeys)
			{
				ToggleNarration();
			}
			if (isEnabled)
			{
				HandleInputs();
				HandleQueueMessages();
			}
		}
	}

	private void HandleInputs()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			printToConsole = true;
			SayInstructions();
		}
		if (Input.GetKeyDown(KeyCode.F5))
		{
			Say(lastNonInstructionSpeech, NarrationCategory.Navigation, false);
		}
		if (Input.GetKeyDown(KeyCode.F6) && !lockToggleHotkeys)
		{
			ToggleNarrationDescription();
		}
		if (Input.GetKeyDown(KeyCode.F7) && !lockToggleHotkeys)
		{
			ToggleNarrationSubtitles();
		}
	}

	private void HandleQueueMessages()
	{
		if (messages.Count > 0 && !narrationHandler.IsSpeaking)
		{
			narrationHandler.Say(messages[0].message);
			messages.RemoveAt(0);
		}
	}

	private void ToggleNarrationDescription()
	{
		bool num = !InternalCheckCategoryEnabled(NarrationCategory.Description);
		Persistence.SetNarrateDescriptions(num);
		string sound = (num ? "sndNarrationDescriptionEnabled" : "sndNarrationDescriptionDisabled");
		if (!num)
		{
			RemoveMessagesOfCategory(NarrationCategory.Description);
		}
		scrConductor.PlayImmediately(sound, 1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
		Say(RDString.Get(num ? "narration.descriptionsEnabled" : "narration.descriptionsDisabled"), NarrationCategory.Notification, false);
	}

	private void ToggleNarrationSubtitles()
	{
		bool num = !InternalCheckCategoryEnabled(NarrationCategory.Subtitles);
		Persistence.SetNarrateSubtitles(num);
		string sound = (num ? "sndNarrationSubtitlesEnabled" : "sndNarrationSubtitlesDisabled");
		if (!num)
		{
			RemoveMessagesOfCategory(NarrationCategory.Subtitles);
		}
		scrConductor.PlayImmediately(sound, 1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
		Say(RDString.Get(num ? "narration.subtitlesEnabled" : "narration.subtitlesDisabled"), NarrationCategory.Notification, false);
	}

	private void ToggleNarration()
	{
		Persistence.SetNarration(!isEnabled);
		LoadNarration(feedback: true);
		if (isEnabled)
		{
			Say(RDString.Get("narration.enabled"), NarrationCategory.Notification, false);
		}
	}

	private void RemoveMessagesOfCategory(NarrationCategory category)
	{
		messages.RemoveAll(((NarrationCategory category, string message) item) => item.category == category);
	}

	private void AddAvailableActions(NarrationActionName actionName)
	{
		if (currentActions == null)
		{
			currentActions = new List<NarrationAction>();
		}
		NarrationAction[] actions = data.actions;
		foreach (NarrationAction narrationAction in actions)
		{
			if (narrationAction.actionName == actionName)
			{
				currentActions.Add(narrationAction);
				break;
			}
		}
	}

	private void InternalSet(string speech, int itemIndex = -1, int itemsLength = -1, ElementType elementType = ElementType.None, object typeData = null, string description = null, bool isDialogue = false)
	{
		string text = speech;
		speech = string.Empty;
		if ((uint)(elementType - 7) <= 1u || (uint)(elementType - 10) <= 1u)
		{
			speech = text;
			if (ReportPositionEnabled(isMenuOrList: true))
			{
				speech = AddPause(speech, ShortPause);
				speech += RDString.Get("narration." + elementType);
				speech = speech.Replace("[itemsLength]", itemsLength.ToString());
				cachedItemsLength = itemsLength;
			}
		}
		else
		{
			if (elementType == ElementType.Button)
			{
				text = RDString.Get("narration.Button").Replace("[name]", text);
			}
			if (elementType == ElementType.Tab)
			{
				text = RDString.Get("narration.Tab").Replace("[name]", text);
			}
			if (elementType == ElementType.Link)
			{
				text = RDString.Get("narration.Link").Replace("[name]", text);
			}
			if (elementType == ElementType.Inputfield)
			{
				text = RDString.Get("narration.Inputfield").Replace("[name]", text);
			}
			if (currentSpeech.Contains("[currentSelection]"))
			{
				currentSpeech = currentSpeech.Replace("[currentSelection]", text);
			}
			else
			{
				speech += text;
			}
		}
		if (elementType == ElementType.Selection)
		{
			speech = AddPause(speech, ShortPause) + typeData.ToString();
		}
		if (!description.IsNullOrEmpty())
		{
			speech = AddPause(speech, ShortPause) + description;
		}
		if (ReportPositionEnabled(isMenuOrList: false) && itemIndex >= 0 && (itemsLength >= 0 || cachedItemsLength >= 0))
		{
			speech = AddPause(speech, ShortPause) + RDString.Get("narration.itemIndex");
			speech = speech.Replace("[index]", itemIndex.ToString());
			if (itemsLength == -10)
			{
				itemsLength = cachedItemsLength;
			}
			speech = speech.Replace("[itemsLength]", itemsLength.ToString());
		}
		if (!isDialogue)
		{
			speech = AddPause(speech, LongPause);
		}
		currentSpeech += speech;
		lastElementType = elementType;
	}

	private void InternalSay(string speech, NarrationCategory category, int itemIndex, int itemsLength, ElementType elementType, object typeData, string description, bool flipCategoryQueueBehaviour = false, params NarrationActionName[] actions)
	{
		if (InternalCheckCategoryEnabled(category))
		{
			InternalSet(speech, itemIndex, itemsLength, elementType, typeData, description, category == NarrationCategory.Dialogue);
		}
		bool flag = category == NarrationCategory.Instruction;
		if (flag || (actions != null && actions.Length != 0))
		{
			InternalSetInstructions(flag ? speech : string.Empty, actions);
		}
		if (category != NarrationCategory.Instruction)
		{
			lastNonInstructionSpeech = currentSpeech;
		}
		if (narrationHandler.IsEnabled && InternalCheckCategoryEnabled(category))
		{
			bool flag2 = category != NarrationCategory.Navigation;
			if (flipCategoryQueueBehaviour)
			{
				flag2 = !flag2;
			}
			if (category == NarrationCategory.Fallback)
			{
				Debug.LogWarning("Unimplemented narration category for message: \"" + currentSpeech + "\"");
			}
			if (printToConsole)
			{
				RDBaseDllDummy.printem($"QUEUE: {flag2}, CATEGORY: {category}, {currentSpeech}");
			}
			if (flag2)
			{
				messages.Add((category, currentSpeech));
			}
			else
			{
				messages = new List<(NarrationCategory, string)>();
				narrationHandler.Stop();
				narrationHandler.Say(currentSpeech);
			}
			currentSpeech = string.Empty;
		}
	}

	private void InternalSetInstructions(string speech = "", params NarrationActionName[] actions)
	{
		instructionSpeech = speech;
		if (actions != null)
		{
			foreach (NarrationActionName actionName in actions)
			{
				AddAvailableActions(actionName);
			}
		}
		if (currentActions.Count > 0)
		{
			for (int j = 0; j < currentActions.Count; j++)
			{
				if (j == 0)
				{
					instructionSpeech = AddPause(instructionSpeech, LongPause);
				}
				else
				{
					instructionSpeech += ShortPause;
				}
				instructionSpeech += currentActions[j].GetNarration();
			}
		}
		if (printToConsole)
		{
			RDBaseDllDummy.printem("Set instructions: " + instructionSpeech);
		}
		currentActions = new List<NarrationAction>();
	}

	private void SayInstructions()
	{
		Say(instructionSpeech, NarrationCategory.Instruction, true);
	}

	private void InternalNarrateRowInfo(Row row, NarrateInfoType infoType, bool soundOnly = false, bool narrateSkipBeats = true, string customPattern = null, bool skipsUnstable = false, RDPlayer customPlayer = RDPlayer.AutoDetect, int? customRowLength = null)
	{
		if (!isEnabled)
		{
			return;
		}
		Character character = row.ent.character.character;
		string text = ((character == Character.Custom) ? row.ent.character.customAnimation.data.name : RDString.Get("enum.Character." + character.ToString() + ".short"));
		string text2 = string.Empty;
		string key = string.Empty;
		int num = 0;
		switch (infoType)
		{
		case NarrateInfoType.Connect:
			key = "narration.patient.connect";
			break;
		case NarrateInfoType.Update:
			key = "narration.patient.update";
			break;
		case NarrateInfoType.Disconnect:
			key = "narration.patient.disconnect";
			break;
		case NarrateInfoType.Online:
			key = "narration.doctor.announceOnline";
			num = 1;
			break;
		case NarrateInfoType.Offline:
			key = "narration.doctor.announceOffline";
			num = 2;
			break;
		}
		if (((soundOnly && infoType == NarrateInfoType.Update) & narrateSkipBeats) && lastNarratedRowID == row.id)
		{
			text2 = GetSkipSpeech(row, customPattern, skipsUnstable);
		}
		else
		{
			if (!soundOnly)
			{
				text2 = RDString.Get(key).Replace("[patientName]", text);
				switch (num)
				{
				case 1:
					text2 = text2.Replace("[doctorName]", RDString.Get("narration.doctor." + row.cpuCharacter));
					text2 = text2.Replace("[online]", RDString.Get("narration.doctor.online"));
					break;
				case 2:
					text2 = text2.Replace("[doctorName]", RDString.Get("narration.doctor." + row.cpuCharacter));
					text2 = text2.Replace("[offline]", RDString.Get("narration.doctor.offline"));
					break;
				}
			}
			else if (infoType != NarrateInfoType.Offline)
			{
				text2 = text;
			}
			if (row.rowType == RowType.Classic && infoType != NarrateInfoType.Disconnect)
			{
				text2 += GetRowLengthSpeech(row, customRowLength);
				if (narrateSkipBeats)
				{
					text2 += " ";
					text2 += GetSkipSpeech(row, customPattern, skipsUnstable);
				}
			}
		}
		if (GC.twoPlayerMode)
		{
			string text3 = ((customPlayer == RDPlayer.AutoDetect) ? GetPlayerSpeech(row) : GetPlayerSpeech(customPlayer));
			if (text3 != string.Empty)
			{
				text2 = text3 + " " + text2;
			}
		}
		Say(text2, NarrationCategory.Notification, false);
		lastNarratedRowID = row.id;
	}

	private string GetRowLengthSpeech(Row row, int? customRowLength)
	{
		int num = customRowLength ?? row.rowLengthForNarration;
		if (num == 7 || num < 0)
		{
			return string.Empty;
		}
		string text = RDString.Get("narration.patient.rowLength").Replace("[amount]", num.ToString());
		return " " + text;
	}

	private string GetSkipSpeech(Row row, string customPattern, bool skipsUnstable)
	{
		bool[] array = new bool[6];
		array = ((customPattern != null) ? Row.ShowBeatsFromString(customPattern) : row.show);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i])
			{
				text += i + 1;
				text += " ";
			}
		}
		string text2 = ((!(text == string.Empty)) ? RDString.Get("narration.patient.skipBeats").Replace("[beats]", text.TrimEnd()) : RDString.Get("narration.patient.skipBeatsNone"));
		return text2.Replace("[unstable]", skipsUnstable ? RDString.Get("narration.patient.skipsUnstable") : string.Empty);
	}

	private string GetPlayerSpeech(Row row)
	{
		return GetPlayerSpeech(row.multiplayerToChangeInto);
	}

	private string GetPlayerSpeech(RDPlayer player)
	{
		return player switch
		{
			RDPlayer.P1 => RDString.Get("narration.gameplay.player1"), 
			RDPlayer.P2 => RDString.Get("narration.gameplay.player2"), 
			_ => string.Empty, 
		};
	}

	private string AddPause(string speech, string newPause)
	{
		if (speech.IsNullOrEmpty())
		{
			return speech;
		}
		string item = RDString.Get("narration.shortPause");
		string item2 = RDString.Get("narration.shortPause1");
		string item3 = RDString.Get("narration.shortPause2");
		string item4 = RDString.Get("narration.longPause");
		string item5 = RDString.Get("narration.longPause1");
		string item6 = RDString.Get("narration.longPause2");
		List<string> obj = new List<string> { item, item2, item3, item4, item5, item6 };
		bool flag = false;
		foreach (string item7 in obj)
		{
			if (speech.EndsWith(item7))
			{
				flag = true;
			}
			else if (speech.EndsWith(item7[0]))
			{
				flag = true;
				speech = speech.TrimEnd(item7[0]) + item7;
			}
		}
		if (!flag)
		{
			speech += newPause;
		}
		return speech;
	}

	private void InternalClear(bool clearInstructions = true)
	{
		currentSpeech = string.Empty;
		if (clearInstructions)
		{
			instructionSpeech = string.Empty;
		}
	}

	private bool InternalCheckCategoryEnabled(NarrationCategory category)
	{
		return category switch
		{
			NarrationCategory.Description => Persistence.GetNarrateDescriptions(), 
			NarrationCategory.Subtitles => Persistence.GetNarrateSubtitles(), 
			NarrationCategory.Navigation => navigationMessagesEnabled, 
			_ => Persistence.GetNarration(), 
		};
	}

	private bool ReportPositionEnabled(bool isMenuOrList)
	{
		switch ((scnLevelSelect.instance != null) ? Persistence.GetLevelSelectReportPosition() : Persistence.GetOtherMenusReportPosition())
		{
		case ReportPosition.Always:
			return true;
		case ReportPosition.Never:
			return false;
		case ReportPosition.Once:
			if (isMenuOrList)
			{
				return true;
			}
			if (lastElementType != ElementType.VerticalMenu && lastElementType != ElementType.VerticalList && lastElementType != ElementType.HorizontalMenu)
			{
				return lastElementType == ElementType.HorizontalList;
			}
			return true;
		default:
			return false;
		}
	}

	private void EnableAccessability(bool feedback = false)
	{
		StopSpeaking();
		if (feedback)
		{
			PlayNarrationFeedback(isEnabled);
		}
		narrationHandler.EnableAccessability(isEnabled);
	}

	public static void Set(string speech, ElementType elementType, int itemsLength)
	{
		instance.InternalSet(speech, -1, itemsLength, elementType);
	}

	public static void Set(string speech, int itemIndex = -1, int itemsLength = -1, ElementType elementType = ElementType.None, object typeData = null, string description = null, bool isDialogue = false)
	{
		instance.InternalSet(speech, itemIndex, itemsLength, elementType, typeData, description, isDialogue);
	}

	public static void Say(string speech, NarrationCategory category, bool flipCategoryQueueBehaviour = false, params NarrationActionName[] actions)
	{
		instance.InternalSay(speech, category, -1, -1, ElementType.None, null, null, flipCategoryQueueBehaviour, actions);
	}

	public static void Say(string speech, NarrationCategory category, int itemIndex, int itemsLength, ElementType elementType, object typeData, string description, bool flipCategoryQueueBehaviour = false, params NarrationActionName[] actions)
	{
		instance.InternalSay(speech, category, itemIndex, itemsLength, elementType, typeData, description, flipCategoryQueueBehaviour, actions);
	}

	public static void SetInstructions(string speech = "", params NarrationActionName[] actions)
	{
		instance.InternalSetInstructions(speech, actions);
	}

	public static void NarrateRowInfo(Row row, NarrateInfoType infoType, bool soundOnly = false, bool narrateSkipBeats = true, string customPattern = null, bool skipsUnstable = false, RDPlayer customPlayer = RDPlayer.AutoDetect, int? customRowLength = -1)
	{
		instance.InternalNarrateRowInfo(row, infoType, soundOnly, narrateSkipBeats, customPattern, skipsUnstable, customPlayer, customRowLength);
	}

	public static bool CheckCategoryEnabled(NarrationCategory category)
	{
		return instance.InternalCheckCategoryEnabled(category);
	}

	public static void Clear(bool clearInstructions = true)
	{
		instance.InternalClear(clearInstructions);
	}

	public static void Stop()
	{
		instance.narrationHandler.Stop();
	}

	public static void StopSpeaking()
	{
		Clear();
		Stop();
	}

	public static void InitializeNarrateRowInfo()
	{
		instance.lastNarratedRowID = -1;
	}

	public static void ToggleNavigationMessages(bool toggle)
	{
		instance.navigationMessagesEnabled = toggle;
		if (!toggle)
		{
			instance.RemoveMessagesOfCategory(NarrationCategory.Navigation);
		}
	}

	public static void PlayNarrationFeedback(bool enabled)
	{
		scrConductor.PlayImmediately(enabled ? "sndNarrationEnabled" : "sndNarrationDisabled", 1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
	}

	public static void LoadNarration(bool feedback = false)
	{
		instance.isEnabled = IsAvailable && Persistence.GetNarration();
		instance.EnableAccessability(feedback);
		instance.narrationHandler.SetSpeed(Persistence.GetSpeechRate());
		instance.narrationHandler.SetVolume(Persistence.GetSpeechVolume());
	}

	public static void ForceNarrationOn(bool feedback = false)
	{
		instance.isEnabled = IsAvailable;
		instance.EnableAccessability();
	}

	public static void ToggleSAPI(bool enable)
	{
		instance.narrationHandler.ToggleSAPI(enable);
	}

	public static void SetSpeed(int speed)
	{
		instance.narrationHandler.SetSpeed(speed);
	}

	public static void SetVolume(int volume)
	{
		instance.narrationHandler.SetVolume(volume);
	}
}
