using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace RDLevelEditor
{
	public class LevelEvent_Base : RDClass
	{
		public const int InvalidRow = -10;

		public const int AllRows = -1;

		protected const string uidKey = "uid";

		protected const string barKey = "bar";

		protected const string beatKey = "beat";

		protected const string typeKey = "type";

		protected const string conditionalKey = "if";

		protected const string tagKey = "tag";

		protected const string tagRunKey = "runTag";

		protected const string rowKey = "row";

		protected const string yKey = "y";

		protected const string roomsKey = "rooms";

		protected const string activeKey = "active";

		protected const string targetKey = "target";

		protected const string tabKey = "tab";

		public List<int> conditionals = new List<int>();

		public List<string> globalConditionals = new List<string>();

		public string tag;

		public bool tagRunNormally;

		public float conditionalDuration;

		public LevelEventExecutionTime executionTime = LevelEventExecutionTime.OnBar;

		public BarAndBeat barAndBeat;

		public readonly LevelEventType type;

		public readonly LevelEventInfo info;

		public RoomsUsage roomsUsage;

		public int uid;

		public int[] rooms = new int[1];

		public bool active = true;

		public bool tagActive = true;

		public int _row = -10;

		public int y;

		public bool usesY = true;

		public int sortOrderOffset;

		public bool usesPrepare;

		public bool prepared;

		public bool isDragging;

		public string target;

		public Tab tab = Tab.None;

		public int timesRun;

		private static StringBuilder sb = new StringBuilder(1000);

		private const int RunOnBeatMaxDepth = 100;

		private static int runOnBeatRecursionDepth = 0;

		private static bool exitingRecursion = false;

		public string debugStatsCustom = "";

		public int row
		{
			get
			{
				return _row;
			}
			set
			{
				_row = value;
			}
		}

		private bool usesBar => info.attribute.usesBar;

		public bool usesBeat => info?.attribute.usesBeat ?? true;

		private bool usesType => info.attribute.usesType;

		public virtual bool hasTaggedSpecialMethod => false;

		public scrVfxControl vfx => scrVfxControl.instance;

		public static LevelBase level => scnGame.instance.currentLevel;

		public string name => type.ToString();

		public int sortOrder => (int)((float)(bar * 10000) + beat * 100f + (float)sortOrderOffset);

		public bool isOneshotRowEvent
		{
			get
			{
				if (!(this is LevelEvent_AddOneshotBeat))
				{
					return this is LevelEvent_SetOneshotWave;
				}
				return true;
			}
		}

		public bool isBaseEvent => GetType() == typeof(LevelEvent_Base);

		public Tab defaultTab
		{
			get
			{
				Dictionary<Tab, List<LevelEventType>> levelEventTabs = RDEditorConstants.levelEventTabs;
				Tab tab = Tab.None;
				foreach (KeyValuePair<Tab, List<LevelEventType>> item in levelEventTabs)
				{
					if (item.Value.Contains(type))
					{
						tab = item.Key;
						break;
					}
				}
				if (tab != Tab.None)
				{
					if (levelEventTabs.ContainsKey(this.tab) && levelEventTabs[this.tab].Contains(type))
					{
						return this.tab;
					}
					return tab;
				}
				return Tab.None;
			}
		}

		public bool isClassicRowEvent
		{
			get
			{
				if (!(this is LevelEvent_AddClassicBeat) && !(this is LevelEvent_SetRowXs) && !(this is LevelEvent_AddFreeTimeBeat))
				{
					return this is LevelEvent_PulseFreeTimeBeat;
				}
				return true;
			}
		}

		public bool isDecorationTabEvent
		{
			get
			{
				if (!(this is LevelEvent_Comment))
				{
					return RDEditorConstants.levelEventTabs[Tab.Decorations].Contains(type);
				}
				return false;
			}
		}

		public LevelEvent_MakeDecorationBase ownerDecoration
		{
			get
			{
				if (isDecorationTabEvent)
				{
					if (base.editor != null)
					{
						return base.editor.decorationsData[row];
					}
					return RDLevelData.current.decorations[row];
				}
				return null;
			}
		}

		public float beat
		{
			get
			{
				if (!isDragging && !usesBeat)
				{
					return 1f;
				}
				return barAndBeat.beat;
			}
			set
			{
				barAndBeat.beat = ((isDragging || usesBeat) ? value : 1f);
			}
		}

		public int bar
		{
			get
			{
				return barAndBeat.bar;
			}
			set
			{
				barAndBeat.bar = value;
			}
		}

		public float bar1000timesbeat => (float)barAndBeat.bar * 1000f + barAndBeat.beat;

		public int room
		{
			get
			{
				if (rooms.Length == 0)
				{
					return 0;
				}
				return rooms[0];
			}
			set
			{
				rooms = new int[1] { value };
			}
		}

		public InspectorPanel inspectorPanel
		{
			get
			{
				Type type = Type.GetType("RDLevelEditor.InspectorPanel_" + name);
				InspectorPanel inspectorPanel = base.editor.inspectorPanelManager.Get(type);
				if (inspectorPanel != null)
				{
					return inspectorPanel;
				}
				Debug.LogWarning("Couldn't find \"" + name + "\" inspector panel.");
				return null;
			}
		}

		public bool usesWindowDance => this is LevelEvent_NewWindowDance || this is LevelEvent_WindowResize || this is LevelEvent_SetWindowContent || this is LevelEvent_SetMainWindow || this is LevelEvent_RenameWindow || this is LevelEvent_HideWindow;

		public LevelEvent_Base()
		{
			string text = GetType().Name.Replace("LevelEvent_", "");
			type = ((text != "Base") ? RDUtils.ParseEnum(text, LevelEventType.None) : LevelEventType.None);
			if (type != LevelEventType.None)
			{
				if (!GC.levelEventsInfo.ContainsKey(type))
				{
					throw new Exception(name + " is missing a LevelEventInfoAttribute. Maybe make this more descriptive later");
				}
				info = GC.levelEventsInfo[type];
				GenerateNewUID();
				Init();
				MethodInfo method = GetType().GetMethod("Prepare");
				usesPrepare = method.GetBaseDefinition().DeclaringType != method.DeclaringType;
			}
		}

		public void GenerateNewUID()
		{
			uid = RXRandom.Int();
		}

		public virtual void OnCreate()
		{
			foreach (NullablePropertyInfo item in from nullable in info.propertiesInfo.OfType<NullablePropertyInfo>()
				where nullable.offByDefault
				select nullable)
			{
				item.propertyInfo.SetValue(this, null);
			}
		}

		public virtual void Init()
		{
			executionTime = info.attribute.executionTime;
			sortOrderOffset = info.attribute.sortOffset;
			usesY = !info.attribute.usesTargetId;
			roomsUsage = info.attribute.roomsUsage;
			row = info.attribute.defaultRow;
		}

		public void DecodeBaseProperties(Dictionary<string, object> dict)
		{
			if (base.editor != null && base.editor.changingState != 0)
			{
				uid = (int)dict["uid"];
			}
			bar = ((!usesBar || !dict.ContainsKey("bar")) ? 1 : ((int)dict["bar"]));
			beat = ((usesBeat && dict.ContainsKey("beat")) ? Convert.ToSingle(dict["beat"]) : 1f);
			if (dict.ContainsKey("y"))
			{
				object obj = dict["y"];
				y = ((obj is int) ? ((int)obj) : 0);
			}
			if (dict.ContainsKey("active"))
			{
				active = (bool)dict["active"];
			}
			if (dict.ContainsKey("if"))
			{
				object obj2 = dict["if"];
				if (obj2 is List<object>)
				{
					conditionals = new List<int>(RDEditorUtils.DecodeIntArray(obj2));
					conditionalDuration = 0f;
				}
				else
				{
					string[] array = (obj2 as string).Split('d');
					string[] array2 = array[0].Split('&');
					conditionals = new List<int>();
					globalConditionals = new List<string>();
					char[] separator = new char[1] { '~' };
					string[] array3 = array2;
					foreach (string text in array3)
					{
						if (text.IsNullOrEmpty())
						{
							continue;
						}
						string s = text.Split(separator, StringSplitOptions.RemoveEmptyEntries)[0];
						bool flag = text[0] == '~';
						if (int.TryParse(s, out var result))
						{
							if (flag)
							{
								conditionals.Add(-result - 1);
							}
							else
							{
								conditionals.Add(result);
							}
						}
						else if (!string.IsNullOrEmpty(text))
						{
							string text2;
							if (!flag)
							{
								text2 = text;
							}
							else
							{
								string text3 = text;
								text2 = text3.Substring(1, text3.Length - 1);
							}
							string item = text2;
							if (Conditionals.usedGlobalConditionals.Contains(item))
							{
								globalConditionals.Add(text);
							}
						}
					}
					conditionalDuration = Convert.ToSingle(array[1]);
				}
			}
			if (dict.ContainsKey("tag"))
			{
				tag = dict["tag"] as string;
				if (string.IsNullOrWhiteSpace(tag))
				{
					tag = null;
				}
			}
			if (dict.ContainsKey("runTag"))
			{
				tagRunNormally = (bool)dict["runTag"];
			}
			if (dict.ContainsKey("tab"))
			{
				string text4 = dict["tab"] as string;
				if (text4 == "Sprites" && RDLevelData.current.settings.version < 68)
				{
					text4 = Tab.Decorations.ToString();
				}
				tab = Enum.Parse<Tab>(text4);
				if (tab == Tab.Decorations)
				{
					DecodeTargetId(dict);
				}
			}
			else if (this is LevelEvent_Comment)
			{
				tab = Tab.Actions;
			}
			rooms = (dict.TryGetValue("rooms", out var value) ? RDEditorUtils.DecodeIntArray(value) : (rooms = new int[1]));
		}

		public virtual void Decode(Dictionary<string, object> dict)
		{
			DecodeBaseProperties(dict);
			if (info.usesRow)
			{
				row = Convert.ToInt32(dict["row"]);
			}
			if (info.attribute.usesTargetId)
			{
				DecodeTargetId(dict);
			}
			foreach (BasePropertyInfo item in info.propertiesInfo)
			{
				if (item.onlyUI)
				{
					continue;
				}
				string text = item.name;
				if (dict.ContainsKey(text))
				{
					try
					{
						object value = item.Decode(dict[text]);
						item.propertyInfo.SetValue(this, value);
					}
					catch (Exception)
					{
						Debug.LogError("Failed to decode property '" + text + "' in event '" + name + "'. See following exception");
						throw;
					}
				}
				else if (item is NullablePropertyInfo)
				{
					item.propertyInfo.SetValue(this, null);
				}
			}
		}

		public virtual string Encode()
		{
			RDLevelData.eventEncode.Length = 0;
			EncodeAdd(EncodeBaseProperties());
			if (info.usesRow)
			{
				EncodeAdd(EncodeRow());
			}
			if (info.attribute.usesTargetId)
			{
				EncodeAdd(EncodeTargetId());
			}
			foreach (BasePropertyInfo item in info.propertiesInfo)
			{
				if (!item.onlyUI && (item.enableIf == null || item.enableIf(this)) && (item.saveIf == null || item.saveIf(this)))
				{
					object value = item.propertyInfo.GetValue(this);
					if (!(item is NullablePropertyInfo) || value != null)
					{
						EncodeAdd(item.Encode(item.name, value, isLastKey: false));
					}
				}
			}
			while (true)
			{
				StringBuilder eventEncode = RDLevelData.eventEncode;
				char c = eventEncode[eventEncode.Length - 1];
				if (c != ',' && c != ' ')
				{
					break;
				}
				RDLevelData.eventEncode.Length--;
			}
			return RDLevelData.eventEncode.ToString();
			static void EncodeAdd(string s)
			{
				RDLevelData.eventEncode.Append(s);
			}
		}

		public virtual IEnumerator Prepare()
		{
			prepared = true;
			yield break;
		}

		public virtual void OnDelete()
		{
		}

		public virtual void Run()
		{
			RDEditorUtils.NotImplemented();
		}

		public virtual void RunPrebar()
		{
		}

		public virtual void TaggedActionVariant()
		{
		}

		public virtual string CustomControlName()
		{
			return null;
		}

		public virtual string GetTooltipText()
		{
			return null;
		}

		public void CopyFrom(LevelEvent_Base levelEvent, bool copyBarAndBeat)
		{
			CopyBasePropertiesFrom(levelEvent, copyBarAndBeat, copyRow: true);
			CopyFromInternal(levelEvent);
		}

		public void CopyBasePropertiesFrom(LevelEvent_Base levelEvent, bool copyBarAndBeat, bool copyRow)
		{
			if (copyBarAndBeat)
			{
				bar = levelEvent.bar;
				beat = levelEvent.beat;
				y = levelEvent.y;
			}
			active = levelEvent.active;
			rooms = levelEvent.rooms;
			tag = levelEvent.tag;
			tagRunNormally = levelEvent.tagRunNormally;
			conditionalDuration = levelEvent.conditionalDuration;
			target = levelEvent.target;
			tab = levelEvent.tab;
			if (copyRow)
			{
				row = levelEvent.row;
				roomsUsage = levelEvent.roomsUsage;
			}
			conditionals = new List<int>(levelEvent.conditionals);
			globalConditionals = new List<string>(levelEvent.globalConditionals);
		}

		public virtual void CopyFromInternal(LevelEvent_Base levelEvent)
		{
			foreach (BasePropertyInfo item in info.propertiesInfo)
			{
				PropertyInfo propertyInfo = item.propertyInfo;
				if (!(propertyInfo.SetMethod == null))
				{
					propertyInfo.SetValue(this, item.MakeCopy(propertyInfo.GetValue(levelEvent)));
				}
			}
		}

		protected string EncodeRow()
		{
			return RDEditorUtils.EncodeInt("row", row);
		}

		protected string EncodeRoom()
		{
			return RDEditorUtils.EncodeIntArray("rooms", rooms);
		}

		public void SanitizeRoomsData()
		{
			switch (roomsUsage)
			{
			case RoomsUsage.NotUsed:
				if (rooms.Length != 0)
				{
					rooms = new int[0];
				}
				break;
			case RoomsUsage.OneRoom:
				if (rooms.Length == 0)
				{
					rooms = new int[1];
				}
				else if (rooms.Length != 1)
				{
					rooms = new int[1] { rooms[0] };
				}
				break;
			case RoomsUsage.ManyRooms:
				if (rooms.Length == 0)
				{
					rooms = new int[1];
				}
				else if (Array.IndexOf(rooms, 4) != -1)
				{
					rooms = new int[1];
				}
				break;
			case RoomsUsage.ManyRoomsAndOnTop:
				break;
			}
		}

		public LevelEvent_Base Clone()
		{
			LevelEvent_Base obj = Activator.CreateInstance(GetType()) as LevelEvent_Base;
			obj.CopyFrom(this, copyBarAndBeat: true);
			return obj;
		}

		protected string EncodeBaseProperties(bool lastValue = false)
		{
			sb.Clear();
			string text = ((base.editor != null && base.editor.savingState) ? RDEditorUtils.EncodeInt("uid", uid) : "");
			string text2 = (active ? "" : RDEditorUtils.EncodeBool("active", boolValue: false));
			string text3 = ((roomsUsage != RoomsUsage.NotUsed) ? RDEditorUtils.EncodeIntArray("rooms", rooms) : "");
			string text4 = string.Empty;
			if (conditionals.Count > 0 || globalConditionals.Count > 0)
			{
				string text5 = "";
				string[] array = globalConditionals.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					if (text5 != "")
					{
						text5 += "&";
					}
					text5 += array[i];
				}
				int[] array2 = conditionals.ToArray();
				for (int j = 0; j < array2.Length; j++)
				{
					int num = array2[j];
					bool flag = false;
					if (num < 0)
					{
						num = -num - 1;
						flag = true;
					}
					if (text5 != "")
					{
						text5 += "&";
					}
					if (flag)
					{
						text5 += "~";
					}
					text5 += num;
				}
				text5 = text5 + "d" + conditionalDuration;
				text4 = RDEditorUtils.EncodeString("if", text5);
			}
			string text6 = (usesBar ? RDEditorUtils.EncodeInt("bar", bar) : "");
			string text7 = (usesBeat ? RDEditorUtils.EncodeFloat("beat", beat) : "");
			string text8 = (string.IsNullOrEmpty(tag) ? "" : RDEditorUtils.EncodeString("tag", tag));
			string text9 = (tagRunNormally ? RDEditorUtils.EncodeBool("runTag", boolValue: true) : "");
			string text10 = (usesY ? RDEditorUtils.EncodeInt("y", y) : "");
			bool num2 = RDEditorConstants.levelEventTabs.Values.Count((List<LevelEventType> events) => events.Contains(type)) > 1;
			string text11 = (num2 ? RDEditorUtils.EncodeString("tab", tab.ToString()) : "");
			string text12 = (usesType ? RDEditorUtils.EncodeString("type", name) : "");
			string text13 = ((num2 && tab == Tab.Decorations) ? EncodeTargetId() : "");
			sb.AppendMany(text, text6, text7, text10, text12, text4, text8, text9, text2, text3, text11, text13);
			if (lastValue)
			{
				StringBuilder stringBuilder = sb;
				if (stringBuilder[stringBuilder.Length - 2] == ',')
				{
					StringBuilder stringBuilder2 = sb;
					if (stringBuilder2[stringBuilder2.Length - 1] == ' ')
					{
						goto IL_0352;
					}
				}
				StringBuilder stringBuilder3 = sb;
				if (stringBuilder3[stringBuilder3.Length - 1] == ',')
				{
					goto IL_0352;
				}
			}
			goto IL_0374;
			IL_0352:
			sb.Length -= 2;
			sb.Append(" ");
			goto IL_0374;
			IL_0374:
			return sb.ToString();
		}

		public string EncodeTargetId()
		{
			return RDEditorUtils.EncodeString("target", target);
		}

		public void DecodeTargetId(Dictionary<string, object> dict)
		{
			target = RDEditorUtils.DecodeString(dict["target"]);
			List<LevelEvent_MakeDecorationBase> decorations = RDLevelData.current.decorations;
			int num = 0;
			foreach (LevelEvent_MakeDecorationBase item in decorations)
			{
				if (item.decorationId == target)
				{
					row = num;
					if (this is LevelEvent_Move levelEvent_Move && item is LevelEvent_Sprite sprite)
					{
						levelEvent_Move.sprite = sprite;
					}
					break;
				}
				num++;
			}
			if (num == decorations.Count)
			{
				Debug.LogWarning("didn't found matching id " + target + ", setting to 0");
				row = 0;
			}
		}

		public List<Color> GetPaletteColorsUsed(bool[] paletteIndicesUsed = null)
		{
			List<Color> list = new List<Color>();
			bool flag = paletteIndicesUsed != null;
			PropertyInfo[] properties = GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.PropertyType != typeof(ColorOrPalette) && propertyInfo.PropertyType != typeof(ColorOrPalette?) && propertyInfo.PropertyType != typeof(string))
				{
					continue;
				}
				object value = propertyInfo.GetValue(this);
				int result;
				if (value is ColorOrPalette { isPalette: not false } colorOrPalette)
				{
					list.Add(colorOrPalette.ToColor(useEditorPalette: true).WithAlpha(1f));
					if (flag)
					{
						paletteIndicesUsed[colorOrPalette.GetIndex()] = true;
					}
				}
				else if (value is string text && propertyInfo.Name.Contains("Color", StringComparison.OrdinalIgnoreCase) && text.StartsWith("pal") && int.TryParse(text.Replace("pal", ""), out result) && result >= 0 && result < 21)
				{
					list.Add(text.HexOrPaletteIndexToColor(usePaletteAlpha: false, useEditorPalette: true));
					if (flag)
					{
						paletteIndicesUsed[result] = true;
					}
				}
			}
			return list;
		}

		public void RunOnBeat(Action action, bool forcePrebar = false, List<LevelEvent_Base> eventsAffectedByConditional = null, float beatOverrideZI = -1f, bool forceNoIncrementTimesRun = false, bool forceCheckConditionsEarlier = false, bool forceMinusLatency = false)
		{
			if (!exitingRecursion)
			{
				if (runOnBeatRecursionDepth + 1 >= 100)
				{
					Debug.LogError($"Maximum RunOnBeat() recursion depth ({100}) reached! Stopping recursion...");
					exitingRecursion = true;
				}
				else
				{
					string debugData = null;
					runOnBeatRecursionDepth++;
					Action actionCache = action;
					action = () =>
					{
						actionCache();
						level.LevelEventWasRun(this);
					};
					if (level.runningFromTag || tag == null || tagRunNormally || (base.conductor.isScrubbing && level.runTaggedEventsWhileScrubbing))
					{
						bool flag = false;
						List<ConditionalID> allConditionals = GetAllConditionals();
						if (tag != null && level.runningFromTag)
						{
							if (conditionalDuration > 0f)
							{
								double num = base.conductor.audioPos - base.conductor.startOfNextBarRefreshOnlyOnBar;
								float timeOfEnd = Time.time + base.conductor.DurationBeatsToTime((float)num, conditionalDuration);
								TimedAction item = new TimedAction(hasTaggedSpecialMethod ? new Action(TaggedActionVariant) : action, allConditionals, timeOfEnd, this);
								base.game.timedActions.Add(item);
							}
							else
							{
								bool flag2 = false;
								foreach (ConditionalID item3 in allConditionals)
								{
									var (conditional, negated) = FetchConditional(item3);
									if (conditional == null)
									{
										continue;
									}
									flag2 = true;
									if (conditional.Check(this, negated))
									{
										continue;
									}
									goto IL_02ce;
								}
								if (!flag2 || !base.conductor.isScrubbingRefreshOnBar)
								{
									if (!forceNoIncrementTimesRun)
									{
										timesRun++;
									}
									TaggedActionVariant();
									if (!hasTaggedSpecialMethod)
									{
										action();
									}
								}
							}
						}
						else
						{
							float beatZeroIndexed = beat - 1f;
							if (beatOverrideZI != -1f)
							{
								beatZeroIndexed = beatOverrideZI;
							}
							if ((executionTime == LevelEventExecutionTime.OnPrebar) | forcePrebar)
							{
								if (allConditionals.Count((ConditionalID C) => !FetchConditional(C).conditional.info.attribute.isConstant) == 0)
								{
									if (!forceNoIncrementTimesRun)
									{
										timesRun++;
									}
									action();
									goto IL_02ce;
								}
								flag = true;
							}
							bool checkOnlyConditionsBefore = forceCheckConditionsEarlier;
							if (checkOnlyConditionsBefore)
							{
								flag = true;
							}
							scrExecuteOnCertainBeat.Add(() =>
							{
								if (conditionalDuration > 0f)
								{
									float timeOfEnd2 = Time.time + base.conductor.DurationBeatsToTime(beatZeroIndexed, conditionalDuration);
									TimedAction item2 = new TimedAction(action, allConditionals, timeOfEnd2, this);
									base.game.timedActions.Add(item2);
								}
								else if (RDLevelData.current != null && !CheckConditionals())
								{
									if (this is LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat)
									{
										foreach (AudioSource prebarAudioSrc in levelEvent_AddOneshotBeat.prebarAudioSrcs)
										{
											prebarAudioSrc.mute = true;
										}
									}
								}
								else if (checkOnlyConditionsBefore)
								{
									scrExecuteOnCertainBeat.Add(() =>
									{
										if (!forceNoIncrementTimesRun)
										{
											timesRun++;
										}
										action();
									}, beatZeroIndexed, debugData);
								}
								else
								{
									if (!forceNoIncrementTimesRun)
									{
										timesRun++;
									}
									action();
								}
							}, beatZeroIndexed, debugData, flag | forceMinusLatency, calledCloseToNextBarMaybe: false, type);
						}
					}
				}
			}
			goto IL_02ce;
			IL_02ce:
			runOnBeatRecursionDepth = Math.Max(runOnBeatRecursionDepth - 1, 0);
			if (runOnBeatRecursionDepth == 0)
			{
				exitingRecursion = false;
			}
		}

		public bool? HasConditional(int conditionalId)
		{
			if (conditionals.Contains(conditionalId))
			{
				return false;
			}
			if (conditionals.Contains(-conditionalId - 1))
			{
				return true;
			}
			return null;
		}

		public bool? HasGlobalConditional(string conditionalGId)
		{
			if (globalConditionals.Contains(conditionalGId))
			{
				return false;
			}
			if (globalConditionals.Contains("~" + conditionalGId))
			{
				return true;
			}
			return null;
		}

		public List<ConditionalID> GetAllConditionals()
		{
			List<ConditionalID> list = (from x in globalConditionals.ToList()
				select new ConditionalID(x)).ToList();
			foreach (int conditional in conditionals)
			{
				list.Add(new ConditionalID(conditional.ToString()));
			}
			return list;
		}

		public bool HasAnyConditionals()
		{
			if (conditionals.Count <= 0)
			{
				return globalConditionals.Count > 0;
			}
			return true;
		}

		public bool HasPositiveConditionals()
		{
			if (!conditionals.Exists((int i) => i >= 0))
			{
				return globalConditionals.Exists((string i) => !i.StartsWith("~"));
			}
			return true;
		}

		public bool HasNegativeConditionals()
		{
			if (!conditionals.Exists((int i) => i < 0))
			{
				return globalConditionals.Exists((string i) => i.StartsWith("~"));
			}
			return true;
		}

		public void SetConditional(int conditionalId, string gid, bool? state)
		{
			if (!gid.IsNullOrEmpty())
			{
				if (Conditionals.usedGlobalConditionals.Contains(gid))
				{
					globalConditionals.Remove(gid);
					globalConditionals.Remove("~" + gid);
					if (state.HasValue)
					{
						globalConditionals.Add((state == false) ? gid : ("~" + gid));
					}
				}
			}
			else
			{
				conditionals.Remove(conditionalId);
				conditionals.Remove(-conditionalId - 1);
				if (state == false)
				{
					conditionals.Add(conditionalId);
				}
				else if (state == true)
				{
					conditionals.Add(-conditionalId - 1);
				}
			}
		}

		private static (Conditional conditional, bool negated) FetchConditional(ConditionalID cond)
		{
			if (!cond.global)
			{
				return (conditional: RDLevelData.current.conditionals.SingleOrDefault((Conditional x) => x.id == cond.numID), negated: cond.negated);
			}
			return (conditional: Conditionals.GetGlobalConditionals().SingleOrDefault((Conditional x) => x.gid == cond.id), negated: cond.negated);
		}

		public bool CheckConditionals(bool checkConstantOnly = false)
		{
			foreach (ConditionalID allConditional in GetAllConditionals())
			{
				var (conditional, negated) = FetchConditional(allConditional);
				if (conditional != null && (!checkConstantOnly || conditional.info.attribute.isConstant) && !conditional.Check(this, negated))
				{
					return false;
				}
			}
			return true;
		}

		public void SaveData()
		{
			if (inspectorPanel != null && !isBaseEvent)
			{
				using (new SaveStateScope())
				{
					inspectorPanel.Save(this, isNew: false);
				}
			}
		}

		public override string ToString()
		{
			return $"{debugStatsCustom} {name} | {barAndBeat} | row: {row} | target: {target} | execution: {executionTime}\nfields:\n{PublicFieldsToString(this)}";
		}

		private static string PublicFieldsToString(object o)
		{
			IEnumerable<string> values = o.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).Select(FormatField);
			return string.Join("\n", values);
			string FormatField(PropertyInfo field)
			{
				object value;
				try
				{
					value = field.GetValue(o);
				}
				catch (Exception arg)
				{
					return $"{field.Name}: {arg}";
				}
				return $"<b>{field.Name}:</b> {value}";
			}
		}

		public float BarAndBeatToAbsoluteBeat(BarAndBeat barAndBeat)
		{
			float num = barAndBeat.beat;
			int[] crotchetsInEachBar = level.crotchetsInEachBar;
			for (int i = 1; i < barAndBeat.bar; i++)
			{
				int num2 = ((i >= crotchetsInEachBar.Length) ? crotchetsInEachBar[^1] : crotchetsInEachBar[i]);
				num += (float)num2;
			}
			return num - 1f;
		}

		public float BarAndBeatToAbsoluteTime(BarAndBeat barAndBeat)
		{
			float num = BarAndBeatToAbsoluteBeat(barAndBeat);
			List<BPMChange> bpmChanges = level.bpmChanges;
			float num2 = 0f;
			if (bpmChanges.Count == 0)
			{
				num2 = num * 60f / 100f;
			}
			else
			{
				float bpm = bpmChanges[0].bpm;
				float num3 = 0f;
				for (int i = 0; i < bpmChanges.Count; i++)
				{
					BPMChange bPMChange = bpmChanges[i];
					float num4 = BarAndBeatToAbsoluteBeat(new BarAndBeat(bPMChange.bar, bPMChange.beat));
					if (num < num4)
					{
						num2 += (num - num3) * 60f / bpm;
						break;
					}
					num2 += (num4 - num3) * 60f / bpm;
					if (i + 1 == bpmChanges.Count)
					{
						num2 += (num - num4) * 60f / bPMChange.bpm;
						break;
					}
					num3 = num4;
					bpm = bPMChange.bpm;
				}
			}
			return num2;
		}
	}
}
