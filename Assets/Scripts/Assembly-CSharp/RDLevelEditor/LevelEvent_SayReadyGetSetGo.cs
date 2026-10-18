using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_SayReadyGetSetGo : LevelEvent_Base
	{
		public AudioMixerGroup oneshotVoiceGroup;

		public AudioMixerGroup oneshotClicksGroup;

		public const string midfixRDGS = "RDGS_";

		public const string midfixAndStop = "AndStop_";

		public const string midfixCountEnglish = "CountEnglish";

		public bool prebar;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[]
		{
			"SayReaDyGetSetGoNew", "SayGetSetGo", "SayReaDyGetSetOne", "SayGetSetOne", "JustSayAnd", "JustSayStop", "JustSayAndStop", "JustSayRea", "JustSayDy", "JustSayGet",
			"JustSaySet", "JustSayGo", "SaySwitch", "SayWatch", "SayListen", "Count1", "Count2", "Count3", "Count4", "Count5",
			"Count6", "Count7", "Count8", "Count9", "Count10", "SayReadyGetSetGo", "JustSayReady"
		}, false, true)]
		public OneshotPhraseToSay phraseToSay { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[] { "Nurse", "NurseTired", "NurseSwing", "NurseSwingCalm", "IanExcited", "IanCalm", "IanSlow", "NoneBottom", "NoneTop" }, false, false)]
		public GameVoiceSource voiceSource { get; set; } = GameVoiceSource.Nurse;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public float tick { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Slider("%", null, 0f, true, false)]
		[IntInfo(0, 200)]
		public int volume { get; set; } = 100;

		[JsonProperty("", "", null, "", false, true, "EnableIfPhraseHasTick", null)]
		[Button("BreakIntoSeparateEvents", "editor.SayReadyGetSetGo.break")]
		public bool breakButton => false;

		[JsonProperty("", "", null, "", false, true, "EnableIfNumber", null)]
		[Button("ChangeToSetCountSound", "editor.SayReadyGetSetGo.automate")]
		public bool countButton => false;

		public bool IsNurse
		{
			get
			{
				bool result = false;
				if (voiceSource == GameVoiceSource.Nurse || voiceSource == GameVoiceSource.NurseTired || voiceSource == GameVoiceSource.NurseSwing || voiceSource == GameVoiceSource.NurseSwingCalm || voiceSource == GameVoiceSource.NoneBottom)
				{
					result = true;
				}
				return result;
			}
		}

		public bool EnableIfPhraseHasTick()
		{
			OneshotPhraseToSay oneshotPhraseToSay = phraseToSay;
			return oneshotPhraseToSay == OneshotPhraseToSay.SayReaDyGetSetGoNew || oneshotPhraseToSay == OneshotPhraseToSay.SayReadyGetSetGo || oneshotPhraseToSay == OneshotPhraseToSay.SayGetSetGo || oneshotPhraseToSay == OneshotPhraseToSay.SayGetSetOne || oneshotPhraseToSay == OneshotPhraseToSay.SayReaDyGetSetOne;
		}

		public bool EnableIfNumber()
		{
			return phraseToSay.ToString().StartsWith("Count");
		}

		public void BreakIntoSeparateEvents()
		{
			using (new SaveStateScope())
			{
				base.inspectorPanel.LevelEditorPlaySound("sndEditorEventBreak", "LevelEditorActive");
				OneshotPhraseToSay[] wordSequence = GetWordSequence(phraseToSay);
				int num = 0;
				for (int i = 0; i < wordSequence.Length; i++)
				{
					LevelEvent_SayReadyGetSetGo levelEvent_SayReadyGetSetGo = new LevelEvent_SayReadyGetSetGo();
					levelEvent_SayReadyGetSetGo.CopyBasePropertiesFrom(this, copyBarAndBeat: true, copyRow: true);
					levelEvent_SayReadyGetSetGo.voiceSource = voiceSource;
					levelEvent_SayReadyGetSetGo.tick = tick;
					levelEvent_SayReadyGetSetGo.volume = volume;
					levelEvent_SayReadyGetSetGo.phraseToSay = wordSequence[i];
					LevelEventControl_Base levelEventControl_Base = base.editor.CreateEventControl(levelEvent_SayReadyGetSetGo, Tab.Song);
					if (phraseToSay == OneshotPhraseToSay.SayReadyGetSetGo && num == 1)
					{
						num++;
					}
					levelEvent_SayReadyGetSetGo.beat = (float)((double)levelEvent_SayReadyGetSetGo.beat + (double)(tick * (float)num));
					float posXFromBarAndBeat = base.inspectorPanel.timeline.GetPosXFromBarAndBeat(levelEventControl_Base.levelEvent.barAndBeat);
					levelEventControl_Base.levelEvent.barAndBeat = base.inspectorPanel.timeline.GetBarAndBeatWithPosX(posXFromBarAndBeat);
					levelEventControl_Base.UpdateUI();
					num++;
				}
				base.editor.DeleteEventControl(base.inspectorPanel.currentEventControl, selectControlToTheLeft: false);
			}
		}

		public void ChangeToSetCountSound()
		{
			using (new SaveStateScope())
			{
				base.inspectorPanel.LevelEditorPlaySound("sndEditorEventCreate", "LevelEditorActive");
				LevelEvent_SetCountingSound levelEvent_SetCountingSound = new LevelEvent_SetCountingSound();
				levelEvent_SetCountingSound.CopyBasePropertiesFrom(this, copyBarAndBeat: true, copyRow: false);
				levelEvent_SetCountingSound.volume = volume;
				int num = base.editor.rowsData.FindIndex((LevelEvent_MakeRow r) => r.rowType == RowType.Oneshot);
				bool flag = num != -1;
				levelEvent_SetCountingSound.row = (flag ? num : 0);
				if (voiceSource == GameVoiceSource.IanExcited)
				{
					levelEvent_SetCountingSound.voiceSource = (flag ? CountingVoiceSource.IanCountEnglish : CountingVoiceSource.IanCount);
				}
				else if (voiceSource == GameVoiceSource.IanCalm)
				{
					levelEvent_SetCountingSound.voiceSource = (flag ? CountingVoiceSource.IanCountEnglishCalm : CountingVoiceSource.IanCountCalm);
				}
				else if (voiceSource == GameVoiceSource.IanSlow)
				{
					levelEvent_SetCountingSound.voiceSource = (flag ? CountingVoiceSource.IanCountEnglishSlow : CountingVoiceSource.IanCountSlow);
				}
				else
				{
					levelEvent_SetCountingSound.voiceSource = ((!flag) ? CountingVoiceSource.JyiCount : CountingVoiceSource.JyiCountEnglish);
				}
				LevelEventControl_Base levelEventControl_Base = base.editor.CreateEventControl(levelEvent_SetCountingSound, Tab.Song);
				levelEventControl_Base.UpdateUI();
				base.editor.SelectEventControl(levelEventControl_Base);
				base.editor.DeleteEventControl(base.inspectorPanel.currentEventControl, selectControlToTheLeft: false);
			}
		}

		public override void OnCreate()
		{
			base.OnCreate();
			base.room = 4;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (!dict.ContainsKey("rooms"))
			{
				base.room = 4;
			}
		}

		public override IEnumerator Prepare()
		{
			OneshotPhraseToSay[] wordSequence = GetWordSequence(phraseToSay);
			float num = BarAndBeatToAbsoluteBeat(barAndBeat);
			int num2 = 0;
			for (int i = 0; i < wordSequence.Length; i++)
			{
				LevelEvent_SayReadyGetSetGo levelEvent_SayReadyGetSetGo = new LevelEvent_SayReadyGetSetGo();
				levelEvent_SayReadyGetSetGo.CopyBasePropertiesFrom(this, copyBarAndBeat: true, copyRow: true);
				levelEvent_SayReadyGetSetGo.prepared = true;
				levelEvent_SayReadyGetSetGo.phraseToSay = wordSequence[i];
				levelEvent_SayReadyGetSetGo.voiceSource = voiceSource;
				levelEvent_SayReadyGetSetGo.tick = tick;
				levelEvent_SayReadyGetSetGo.volume = volume;
				if (phraseToSay == OneshotPhraseToSay.SayReadyGetSetGo && num2 == 1)
				{
					num2++;
				}
				levelEvent_SayReadyGetSetGo.barAndBeat = Timeline.GetBarAndBeatWithColumn(num + tick * (float)num2, LevelEvent_Base.level.cpbEvents);
				LevelEvent_Base.level.dynamicallyAddedEvents.Add(levelEvent_SayReadyGetSetGo);
				num2++;
			}
			prepared = true;
			yield return null;
		}

		public override void RunPrebar()
		{
			prebar = true;
			RunInternal();
		}

		public override void Run()
		{
			prebar = false;
			RunInternal();
		}

		private void RunInternal()
		{
			if (!LevelEvent_Base.level.noGetSet)
			{
				oneshotVoiceGroup = RDUtils.GetMixerGroup("RDGSVoice");
				oneshotClicksGroup = RDUtils.GetMixerGroup("RDGSClicks");
				switch (phraseToSay)
				{
				case OneshotPhraseToSay.JustSayReady:
					SayReady(0f);
					break;
				case OneshotPhraseToSay.JustSayGet:
					SayGet(0f);
					break;
				case OneshotPhraseToSay.JustSaySet:
					SaySet(0f);
					break;
				case OneshotPhraseToSay.JustSayGo:
					SayGo(0f);
					break;
				case OneshotPhraseToSay.JustSayRea:
					SayRea(0f);
					break;
				case OneshotPhraseToSay.JustSayDy:
					SayDy(0f);
					break;
				case OneshotPhraseToSay.JustSayAnd:
					SayAnd(0f);
					break;
				case OneshotPhraseToSay.JustSayAndStop:
					SayAndStop(0);
					break;
				case OneshotPhraseToSay.JustSayStop:
					SayStop(0f);
					break;
				case OneshotPhraseToSay.SaySwitch:
					SaySwitch(0f);
					break;
				case OneshotPhraseToSay.SayWatch:
					SayWatch(0f);
					break;
				case OneshotPhraseToSay.SayListen:
					SayListen(0f);
					break;
				case OneshotPhraseToSay.Count1:
					SayCount(0f, 1);
					break;
				case OneshotPhraseToSay.Count2:
					SayCount(0f, 2);
					break;
				case OneshotPhraseToSay.Count3:
					SayCount(0f, 3);
					break;
				case OneshotPhraseToSay.Count4:
					SayCount(0f, 4);
					break;
				case OneshotPhraseToSay.Count5:
					SayCount(0f, 5);
					break;
				case OneshotPhraseToSay.Count6:
					SayCount(0f, 6);
					break;
				case OneshotPhraseToSay.Count7:
					SayCount(0f, 7);
					break;
				case OneshotPhraseToSay.Count8:
					SayCount(0f, 8);
					break;
				case OneshotPhraseToSay.Count9:
					SayCount(0f, 9);
					break;
				case OneshotPhraseToSay.Count10:
					SayCount(0f, 10);
					break;
				case OneshotPhraseToSay.SayReadyGetSetGo:
					break;
				}
			}
		}

		private float GetTimeFromBeat(float i)
		{
			return base.conductor.BeatToTime(base.beat + i * tick - 1f);
		}

		public void PlaySound(string soundName, float time, string midfix = "RDGS_", AudioMixerGroup group = null)
		{
			if (base.conductor.isScrubbing || base.conductor.barNumber != base.bar || voiceSource == GameVoiceSource.NoneBottom || voiceSource == GameVoiceSource.NoneTop)
			{
				return;
			}
			string text = "";
			if (voiceSource == GameVoiceSource.Nurse)
			{
				text = "sndJyi - ";
			}
			else if (voiceSource == GameVoiceSource.NurseTired)
			{
				text = ((!new string[11]
				{
					"Rea", "Dy", "Get", "Set", "Go", "AndStop", "And", "Stop", "Switch", "Watch",
					"Listen"
				}.Contains(soundName)) ? "sndJyi - " : "sndJyiTired - ");
			}
			else if (voiceSource == GameVoiceSource.NurseSwing)
			{
				text = ((!new string[5] { "Rea", "Dy", "Get", "Set", "Go" }.Contains(soundName)) ? "sndJyi - " : "sndJyiSwing - ");
			}
			else if (voiceSource == GameVoiceSource.NurseSwingCalm)
			{
				if (!new string[5] { "Rea", "Dy", "Get", "Set", "Go" }.Contains(soundName))
				{
					voiceSource = GameVoiceSource.NurseTired;
					PlaySound(soundName, time, midfix, group);
					return;
				}
				text = "sndJyiSwingCalm - ";
			}
			else if (voiceSource == GameVoiceSource.IanExcited)
			{
				text = "sndIan - ";
			}
			else if (voiceSource == GameVoiceSource.IanCalm)
			{
				text = ((!new string[5] { "AndStop", "And", "Stop", "Watch", "Listen" }.Contains(soundName)) ? "sndIanCalm - " : "sndIan - ");
			}
			else if (voiceSource == GameVoiceSource.IanSlow)
			{
				if (new string[6] { "AndStop", "And", "Stop", "Switch", "Watch", "Listen" }.Contains(soundName))
				{
					voiceSource = GameVoiceSource.IanCalm;
					PlaySound(soundName, time, midfix, group);
					return;
				}
				text = "sndIanSlow - ";
			}
			base.conductor.PlayBeat(text + midfix + soundName, time, (float)volume * 0.01f, group);
		}

		private void SayRea(float beat)
		{
			float beatOverrideZI = base.beat - 1f + beat * tick;
			float time = GetTimeFromBeat(beat);
			if (prebar)
			{
				RunOnBeat(() =>
				{
					base.conductor.PlayBeat("sndClickLow", time, 1f, oneshotClicksGroup);
					PlaySound("Rea", time, "RDGS_", oneshotVoiceGroup);
				}, forcePrebar: true, null, beatOverrideZI, forceNoIncrementTimesRun: true);
			}
			else
			{
				RunOnBeat(() =>
				{
					LevelEvent_Base.level.BeepReady(base.conductor.crotchet * tick, !IsNurse, rooms);
				}, forcePrebar: false, null, beatOverrideZI, forceNoIncrementTimesRun: false, forceCheckConditionsEarlier: true);
			}
		}

		private void SayDy(float beat)
		{
			float beatOverrideZI = base.beat - 1f + beat * tick;
			float time = GetTimeFromBeat(beat);
			if (prebar)
			{
				RunOnBeat(() =>
				{
					base.conductor.PlayBeat("sndClickLow", time, 1f, oneshotClicksGroup);
					PlaySound("Dy", time, "RDGS_", oneshotVoiceGroup);
				}, forcePrebar: true, null, beatOverrideZI, forceNoIncrementTimesRun: true);
			}
			else
			{
				RunOnBeat(() =>
				{
					LevelEvent_Base.level.BeepReady(base.conductor.crotchet * tick, !IsNurse, rooms);
				}, forcePrebar: false, null, beatOverrideZI, forceNoIncrementTimesRun: false, forceCheckConditionsEarlier: true);
			}
		}

		private void SayReady(float beat)
		{
			float beatOverrideZI = base.beat + beat - 1f;
			float time = GetTimeFromBeat(beat);
			if (!prebar)
			{
				return;
			}
			RunOnBeat(() =>
			{
				base.conductor.PlayBeat("sndClickLow", time, 1f, oneshotClicksGroup);
				if (IsNurse)
				{
					PlaySound("Ready", time, "RDGS_", oneshotVoiceGroup);
				}
			}, forcePrebar: true, null, beatOverrideZI, forceNoIncrementTimesRun: true);
		}

		private void SayGet(float beat)
		{
			float beatOverrideZI = base.beat - 1f + beat * tick;
			float time = GetTimeFromBeat(beat);
			if (prebar)
			{
				RunOnBeat(() =>
				{
					base.conductor.PlayBeat("sndClickMid", time, 1f, oneshotClicksGroup);
					PlaySound("Get", time, "RDGS_", oneshotVoiceGroup);
				}, forcePrebar: true, null, beatOverrideZI, forceNoIncrementTimesRun: true);
			}
			else
			{
				RunOnBeat(() =>
				{
					LevelEvent_Base.level.BeepGet(base.conductor.crotchet * tick, !IsNurse, rooms);
				}, forcePrebar: false, null, beatOverrideZI, forceNoIncrementTimesRun: false, forceCheckConditionsEarlier: true);
			}
		}

		private void SaySet(float beat)
		{
			float beatOverrideZI = base.beat - 1f + beat * tick;
			float time = GetTimeFromBeat(beat);
			if (prebar)
			{
				RunOnBeat(() =>
				{
					base.conductor.PlayBeat("sndClickMid", time, 1f, oneshotClicksGroup);
					PlaySound("Set", time, "RDGS_", oneshotVoiceGroup);
				}, forcePrebar: true, null, beatOverrideZI, forceNoIncrementTimesRun: true);
			}
			else
			{
				RunOnBeat(() =>
				{
					LevelEvent_Base.level.BeepSet(base.conductor.crotchet * tick, !IsNurse, rooms);
				}, forcePrebar: false, null, beatOverrideZI, forceNoIncrementTimesRun: false, forceCheckConditionsEarlier: true);
			}
		}

		private void SayGo(float beat)
		{
			float beatOverrideZI = base.beat - 1f + beat * tick;
			float time = GetTimeFromBeat(beat);
			if (prebar)
			{
				RunOnBeat(() =>
				{
					base.conductor.PlayBeat("sndClickHi", time, 1f, oneshotClicksGroup);
					PlaySound("Go", time, "RDGS_", oneshotVoiceGroup);
				}, forcePrebar: true, null, beatOverrideZI, forceNoIncrementTimesRun: true);
			}
			else
			{
				RunOnBeat(() =>
				{
					LevelEvent_Base.level.BeepGo(base.conductor.crotchet * tick, !IsNurse, rooms);
				}, forcePrebar: false, null, beatOverrideZI, forceNoIncrementTimesRun: false, forceCheckConditionsEarlier: true);
			}
		}

		private void SayStop(float beat)
		{
			float time = GetTimeFromBeat(beat);
			if (prebar)
			{
				RunOnBeat(() =>
				{
					base.conductor.PlayBeat("sndClickHi", time, 1f, oneshotClicksGroup);
					PlaySound("Stop", time, "AndStop_", oneshotVoiceGroup);
				}, forcePrebar: true, null, base.beat - 1f);
			}
		}

		private void SayAnd(float beat)
		{
			float time = GetTimeFromBeat(beat);
			if (prebar)
			{
				RunOnBeat(() =>
				{
					PlaySound("And", time, "AndStop_", oneshotVoiceGroup);
				}, forcePrebar: true, null, base.beat - 1f);
			}
		}

		private void SayAndStop(int beat)
		{
			float time = GetTimeFromBeat(beat);
			if (prebar)
			{
				RunOnBeat(() =>
				{
					PlaySound("AndStop", time, "AndStop_", oneshotVoiceGroup);
				}, forcePrebar: true, null, base.beat - 1f);
			}
		}

		public void SayCount(float beat, int number)
		{
			float beatOverrideZI = base.beat - 1f + beat * tick;
			float time = GetTimeFromBeat(beat);
			if (prebar)
			{
				RunOnBeat(() =>
				{
					PlaySound(number.ToString(), time, "CountEnglish", oneshotVoiceGroup);
				}, forcePrebar: true, null, base.beat - 1f);
			}
			else
			{
				RunOnBeat(() =>
				{
					LevelEvent_Base.level.BeepGo(0f, !IsNurse, rooms);
				}, forcePrebar: false, null, beatOverrideZI, forceNoIncrementTimesRun: false, forceCheckConditionsEarlier: true);
			}
		}

		private void SaySwitch(float beat)
		{
			float time = GetTimeFromBeat(beat);
			if (prebar)
			{
				RunOnBeat(() =>
				{
					PlaySound("Switch", time, string.Empty, oneshotVoiceGroup);
				}, forcePrebar: true, null, base.beat - 1f);
			}
		}

		private void SayWatch(float beat)
		{
			float time = GetTimeFromBeat(beat);
			if (prebar)
			{
				RunOnBeat(() =>
				{
					PlaySound("Watch", time, string.Empty, oneshotVoiceGroup);
				}, forcePrebar: true, null, base.beat - 1f);
			}
		}

		private void SayListen(float beat)
		{
			float time = GetTimeFromBeat(beat);
			if (prebar)
			{
				RunOnBeat(() =>
				{
					PlaySound("Listen", time, string.Empty, oneshotVoiceGroup);
				}, forcePrebar: true, null, base.beat - 1f);
			}
		}

		public static OneshotPhraseToSay[] GetWordSequence(OneshotPhraseToSay phrase)
		{
			return phrase switch
			{
				OneshotPhraseToSay.SayReaDyGetSetGoNew => new OneshotPhraseToSay[5]
				{
					OneshotPhraseToSay.JustSayRea,
					OneshotPhraseToSay.JustSayDy,
					OneshotPhraseToSay.JustSayGet,
					OneshotPhraseToSay.JustSaySet,
					OneshotPhraseToSay.JustSayGo
				}, 
				OneshotPhraseToSay.SayReadyGetSetGo => new OneshotPhraseToSay[4]
				{
					OneshotPhraseToSay.JustSayReady,
					OneshotPhraseToSay.JustSayGet,
					OneshotPhraseToSay.JustSaySet,
					OneshotPhraseToSay.JustSayGo
				}, 
				OneshotPhraseToSay.SayReaDyGetSetOne => new OneshotPhraseToSay[5]
				{
					OneshotPhraseToSay.JustSayRea,
					OneshotPhraseToSay.JustSayDy,
					OneshotPhraseToSay.JustSayGet,
					OneshotPhraseToSay.JustSaySet,
					OneshotPhraseToSay.Count1
				}, 
				OneshotPhraseToSay.SayGetSetOne => new OneshotPhraseToSay[3]
				{
					OneshotPhraseToSay.JustSayGet,
					OneshotPhraseToSay.JustSaySet,
					OneshotPhraseToSay.Count1
				}, 
				OneshotPhraseToSay.SayGetSetGo => new OneshotPhraseToSay[3]
				{
					OneshotPhraseToSay.JustSayGet,
					OneshotPhraseToSay.JustSaySet,
					OneshotPhraseToSay.JustSayGo
				}, 
				_ => new OneshotPhraseToSay[0], 
			};
		}

		public override string GetTooltipText()
		{
			return $"{RDString.GetEnumValue(voiceSource)}\n{volume}%";
		}
	}
}
