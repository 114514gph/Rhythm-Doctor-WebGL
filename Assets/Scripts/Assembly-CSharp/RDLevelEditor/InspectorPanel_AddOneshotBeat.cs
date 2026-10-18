using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_AddOneshotBeat : InspectorPanel
	{
		private const float DefaultHeight = 104f;

		private const float PropertyHeight = 17f;

		private const float SubdivAnchorY = -69f;

		public Toggle useSubdiv;

		public Toggle skipshot;

		public Toggle heartshot;

		public Toggle freezeshot;

		public Toggle burnshot;

		public Toggle heldshot;

		public Image subdivIcon;

		public Sprite[] subdivIconSprites;

		public InputField tick;

		public Button tickLabel;

		public InputField loops;

		public InputField loopInterval;

		public GameObject loopIntervalProperty;

		public GameObject subdivisionsContainer;

		public InputField subdivisions;

		public RDEventTrigger[] subdivButtons;

		public Image firstSubdivButtonIcon;

		public Color subdivButtonOnColor;

		public Color subdivButtonOffColor;

		public Color subdivButtonExtraColor;

		public InputField delay;

		public GameObject delayProperty;

		public GameObject heart;

		public GameObject subdivSoundField;

		public GameObject holdCueField;

		public ToggleGroup subdivSound;

		public ToggleGroup holdCue;

		public GameObject breakButton;

		public GameObject switchToSetWave;

		public RectTransform breakWaveHolder;

		public GameObject beatsoundContainer;

		public InputField beatsound;

		private bool showingSubdivTick;

		private new void Awake()
		{
			base.Awake();
			AddOnEditListeners(() =>
			{
				currentEventControl.ShowBorders(show: true);
			}, loops, loopInterval, delay, freezeshot, burnshot, heldshot, useSubdiv, subdivisions);
			AddOnEditListeners(null, tick, skipshot, heartshot, subdivSound, holdCue, beatsound);
			for (int num = 0; num < subdivButtons.Length; num++)
			{
				int x = num + 1;
				RDEventTrigger obj = subdivButtons[num];
				obj.onPointerDown = (PointerEventData e) =>
				{
					if (e.button == PointerEventData.InputButton.Left)
					{
						ActivateSubdivButton(x);
					}
				};
				obj.onPointerEnter = (PointerEventData e) =>
				{
					LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = (LevelEvent_AddOneshotBeat)currentLevelEvent;
					if (e.dragging && e.button == PointerEventData.InputButton.Left && levelEvent_AddOneshotBeat.subdivisions != x)
					{
						ActivateSubdivButton(x);
					}
				};
			}
			tickLabel.onClick.AddListener(() =>
			{
				showingSubdivTick = !showingSubdivTick;
				UpdateUI(currentLevelEvent as LevelEvent_AddOneshotBeat);
			});
			beatsoundContainer.SetActive(RDBase.isDev);
			void ActivateSubdivButton(int buttonIndex)
			{
				string sound = ((buttonIndex == 1) ? "sndSquareshot" : ("sndTriangleshot" + buttonIndex));
				LevelEditorPlaySound(sound, "LevelEditorActive");
				subdivisions.text = buttonIndex.ToString();
				SaveInternal(currentLevelEvent);
				currentEventControl.UpdateUI();
				currentEventControl.ShowBorders(show: true);
			}
		}

		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = (LevelEvent_AddOneshotBeat)levelEvent;
			position.barAndBeat = levelEvent_AddOneshotBeat.barAndBeat;
			bool flag = levelEvent_AddOneshotBeat.freezeBurnMode == FreezeBurnMode.Burnshot && levelEvent_AddOneshotBeat.pulseType == OneshotPulseType.Triangle;
			showingSubdivTick &= flag;
			Text component = tickLabel.GetComponent<Text>();
			component.raycastTarget = flag;
			component.color = (showingSubdivTick ? base.gc.colorPalette[1] : Color.white);
			tickLabel.GetComponent<RDColorHoverEventTrigger>().exitColor = component.color;
			tick.text = (showingSubdivTick ? levelEvent_AddOneshotBeat.subdivTickOverride.ToString() : levelEvent_AddOneshotBeat.tick.ToString());
			loops.text = levelEvent_AddOneshotBeat.loops.ToString();
			loopInterval.text = levelEvent_AddOneshotBeat.interval.ToString();
			useSubdiv.isOn = levelEvent_AddOneshotBeat.pulseType == OneshotPulseType.Square || levelEvent_AddOneshotBeat.pulseType == OneshotPulseType.Triangle;
			skipshot.isOn = levelEvent_AddOneshotBeat.skipshot;
			heartshot.isOn = levelEvent_AddOneshotBeat.pulseType == OneshotPulseType.Heart;
			bool flag2 = levelEvent_AddOneshotBeat.freezeBurnMode == FreezeBurnMode.Freezeshot;
			bool flag3 = levelEvent_AddOneshotBeat.freezeBurnMode == FreezeBurnMode.Burnshot;
			freezeshot.isOn = flag2;
			burnshot.isOn = flag3;
			heldshot.isOn = levelEvent_AddOneshotBeat.hold;
			loopIntervalProperty.SetActive(((levelEvent_AddOneshotBeat.loops > 0) | flag2 | flag3) || levelEvent_AddOneshotBeat.hold || levelEvent_AddOneshotBeat.skipshot);
			bool flag4 = levelEvent_AddOneshotBeat.pulseType == OneshotPulseType.Square || levelEvent_AddOneshotBeat.pulseType == OneshotPulseType.Triangle;
			subdivSoundField.gameObject.SetActive(flag4);
			subdivSound.SetValue(levelEvent_AddOneshotBeat.subdivSound ? "on" : "off");
			holdCueField.gameObject.SetActive(levelEvent_AddOneshotBeat.hold);
			holdCue.SetValue(levelEvent_AddOneshotBeat.holdCue.ToString());
			delayProperty.SetActive(flag2);
			delay.text = levelEvent_AddOneshotBeat.delay.ToString();
			breakButton.SetActive(levelEvent_AddOneshotBeat.loops > 0);
			switchToSetWave.SetActive(levelEvent_AddOneshotBeat.loops == 0);
			float num = -69f;
			float num2 = -69f;
			float num3 = 104f;
			if (flag4)
			{
				num -= 34f;
				num2 -= 34f;
				num3 += 34f;
			}
			if (levelEvent_AddOneshotBeat.hold)
			{
				num -= 17f;
				num3 += 17f;
			}
			bool flag5 = levelEvent_AddOneshotBeat.pulseType == OneshotPulseType.Triangle;
			subdivisionsContainer.SetActive(flag4);
			subdivIcon.sprite = subdivIconSprites[flag5 ? 1 : 0];
			if (flag4)
			{
				UpdateSubdivButtons(levelEvent_AddOneshotBeat.subdivisions);
				subdivisions.text = levelEvent_AddOneshotBeat.subdivisions.ToString();
			}
			breakWaveHolder.AnchorPosY(num);
			holdCueField.GetComponent<RectTransform>().AnchorPosY(num2);
			base.content.sizeDelta = base.content.sizeDelta.WithY(num3);
			beatsound.text = levelEvent_AddOneshotBeat.sound?.filename;
		}

		protected override void SaveInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = (LevelEvent_AddOneshotBeat)levelEvent;
			if (showingSubdivTick)
			{
				levelEvent_AddOneshotBeat.subdivTickOverride = float.Parse(tick.text);
			}
			else
			{
				levelEvent_AddOneshotBeat.tick = float.Parse(tick.text);
			}
			levelEvent_AddOneshotBeat.pulseType = (useSubdiv.isOn ? OneshotPulseType.Square : (heartshot.isOn ? OneshotPulseType.Heart : OneshotPulseType.Wave));
			levelEvent_AddOneshotBeat.freezeBurnMode = (freezeshot.isOn ? FreezeBurnMode.Freezeshot : (burnshot.isOn ? FreezeBurnMode.Burnshot : FreezeBurnMode.None));
			if (useSubdiv.isOn)
			{
				int num = (levelEvent_AddOneshotBeat.subdivisions = int.Parse(subdivisions.text));
				if (num > 1)
				{
					levelEvent_AddOneshotBeat.pulseType = OneshotPulseType.Triangle;
				}
			}
			else
			{
				levelEvent_AddOneshotBeat.subdivisions = 0;
			}
			levelEvent_AddOneshotBeat.skipshot = skipshot.isOn;
			levelEvent_AddOneshotBeat.hold = heldshot.isOn;
			levelEvent_AddOneshotBeat.interval = float.Parse(loopInterval.text);
			int num3 = int.Parse(loops.text);
			if (num3 > 0 && levelEvent_AddOneshotBeat.loops == 0)
			{
				levelEvent_AddOneshotBeat.interval = levelEvent_AddOneshotBeat.tick * 2f;
			}
			levelEvent_AddOneshotBeat.loops = num3;
			levelEvent_AddOneshotBeat.delay = float.Parse(delay.text);
			if (subdivSoundField.gameObject.activeSelf)
			{
				levelEvent_AddOneshotBeat.subdivSound = subdivSound.GetBoolValueFromActiveToggle();
			}
			if (holdCueField.gameObject.activeSelf)
			{
				levelEvent_AddOneshotBeat.holdCue = holdCue.GetEnumValueFromActiveToggle(HoldCueType.Auto);
			}
			levelEvent_AddOneshotBeat.Validate();
			levelEvent_AddOneshotBeat.CapXPosForPositiveFreezeBurnCueTime();
			levelEvent_AddOneshotBeat.sound = (beatsound.text.IsNullOrEmpty() ? ((SoundDataStruct?)null) : new SoundDataStruct?(new SoundDataStruct(beatsound.text)));
			UpdateUIInternal(levelEvent);
		}

		public void Clone()
		{
			LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = new LevelEvent_AddOneshotBeat();
			levelEvent_AddOneshotBeat.CopyFrom(currentEventControl.levelEvent, copyBarAndBeat: false);
			levelEvent_AddOneshotBeat.bar = currentEventControl.levelEvent.bar;
			float num = float.Parse(loopInterval.text);
			levelEvent_AddOneshotBeat.beat = currentEventControl.levelEvent.beat;
			float posXFromBarAndBeat = base.timeline.GetPosXFromBarAndBeat(levelEvent_AddOneshotBeat.barAndBeat);
			posXFromBarAndBeat += num * (float)base.cellWidth;
			levelEvent_AddOneshotBeat.barAndBeat = base.timeline.GetBarAndBeatWithPosX(posXFromBarAndBeat);
			LevelEventControl_Base levelEventControl_Base = base.editor.CreateEventControl(levelEvent_AddOneshotBeat, Tab.Rows);
			base.editor.SelectEventControl(levelEventControl_Base);
			levelEventControl_Base.UpdateUI();
			loopInterval.text = num.ToString();
		}

		private void Update()
		{
			bool flag = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.H);
			heart.GetComponent<RectTransform>().localScale = (flag ? Vector3.one : Vector3.zero);
		}

		public void UpdateSubdivButtons(int subdiv)
		{
			bool flag = subdiv > 5;
			for (int i = 0; i < subdivButtons.Length; i++)
			{
				Image component = subdivButtons[i].GetComponent<Image>();
				if (flag)
				{
					component.color = subdivButtonExtraColor;
					continue;
				}
				bool flag2 = i < subdiv;
				component.color = (flag2 ? subdivButtonOnColor : subdivButtonOffColor);
			}
			firstSubdivButtonIcon.sprite = subdivIconSprites[(subdiv > 1) ? 3 : 2];
		}

		public void SwitchControlToSetOneshotWave()
		{
			using (new SaveStateScope())
			{
				LevelEditorPlaySound("sndEditorEventCreate", "LevelEditorActive");
				base.editor.SetLevelEventControlType(LevelEventType.SetOneshotWave, copyRow: true);
			}
		}

		public void BreakIntoOneshotBeats()
		{
			using (new SaveStateScope())
			{
				LevelEditorPlaySound("sndEditorEventBreak", "LevelEditorActive");
				LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = (LevelEvent_AddOneshotBeat)currentEventControl.levelEvent;
				for (int i = 0; i < levelEvent_AddOneshotBeat.loops + 1; i++)
				{
					LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat2 = new LevelEvent_AddOneshotBeat();
					levelEvent_AddOneshotBeat2.CopyBasePropertiesFrom(levelEvent_AddOneshotBeat, copyBarAndBeat: true, copyRow: true);
					levelEvent_AddOneshotBeat2.tick = levelEvent_AddOneshotBeat.tick;
					levelEvent_AddOneshotBeat2.freezeBurnMode = levelEvent_AddOneshotBeat.freezeBurnMode;
					levelEvent_AddOneshotBeat2.delay = levelEvent_AddOneshotBeat.delay;
					if (levelEvent_AddOneshotBeat2.freezeBurnMode != FreezeBurnMode.None && levelEvent_AddOneshotBeat2.delay != 0f)
					{
						levelEvent_AddOneshotBeat2.interval = levelEvent_AddOneshotBeat.interval;
					}
					levelEvent_AddOneshotBeat2.pulseType = levelEvent_AddOneshotBeat.pulseType;
					if (levelEvent_AddOneshotBeat2.pulseType == OneshotPulseType.Square || levelEvent_AddOneshotBeat2.pulseType == OneshotPulseType.Triangle)
					{
						levelEvent_AddOneshotBeat2.subdivisions = levelEvent_AddOneshotBeat.subdivisions;
					}
					levelEvent_AddOneshotBeat2.sound = (beatsound.text.IsNullOrEmpty() ? ((SoundDataStruct?)null) : new SoundDataStruct?(new SoundDataStruct(beatsound.text)));
					LevelEventControl_Base levelEventControl_Base = base.editor.CreateEventControl(levelEvent_AddOneshotBeat2, Tab.Rows);
					float num = (float)i * levelEvent_AddOneshotBeat.interval;
					levelEvent_AddOneshotBeat2.beat = (float)((double)levelEvent_AddOneshotBeat2.beat + (double)num);
					float posXFromBarAndBeat = base.timeline.GetPosXFromBarAndBeat(levelEventControl_Base.levelEvent.barAndBeat);
					levelEventControl_Base.levelEvent.barAndBeat = base.timeline.GetBarAndBeatWithPosX(posXFromBarAndBeat);
					levelEventControl_Base.UpdateUI();
				}
				base.editor.DeleteEventControl(currentEventControl, selectControlToTheLeft: false);
			}
		}

		public void CreateNurseCue()
		{
			using (new SaveStateScope())
			{
				LevelEvent_AddOneshotBeat data = (LevelEvent_AddOneshotBeat)currentEventControl.levelEvent;
				bool flag = data.loops > 0;
				bool flag2 = data.pulseType == OneshotPulseType.Square && data.subdivSound && !flag;
				if (data.tick <= 0f || (flag && data.interval < data.tick))
				{
					LevelEditorPlaySound("sndEditorErrorSmall", "LevelEditorActive");
					return;
				}
				if (!flag || data.tick * 2f == data.interval)
				{
					bool flag3 = data.tick < 1f;
					OneshotPhraseToSay phrase = (flag2 ? (flag3 ? OneshotPhraseToSay.SayReaDyGetSetOne : OneshotPhraseToSay.SayGetSetOne) : ((!flag3) ? OneshotPhraseToSay.SayGetSetGo : OneshotPhraseToSay.SayReaDyGetSetGoNew));
					AddRDGS(phrase, data.tick * (float)(flag3 ? 4 : 2));
				}
				else if (flag && data.tick == data.interval)
				{
					AddRDGS(OneshotPhraseToSay.SayReaDyGetSetGoNew, data.tick * 1.5f, 0.5f);
					AddRDGS(OneshotPhraseToSay.JustSayDy, data.tick * 2f);
					AddRDGS(OneshotPhraseToSay.JustSayRea, data.tick * 2.5f);
				}
				else
				{
					AddRDGS(OneshotPhraseToSay.JustSayGo, 0f);
					AddRDGS(OneshotPhraseToSay.JustSaySet, data.interval - data.tick);
					AddRDGS(OneshotPhraseToSay.JustSayGet, data.interval);
					AddRDGS(OneshotPhraseToSay.JustSayDy, data.interval * 2f - data.tick);
					AddRDGS(OneshotPhraseToSay.JustSayRea, data.interval * 2f);
				}
				if (flag)
				{
					AddRDGS(OneshotPhraseToSay.JustSayStop, -1f * (data.interval * (float)data.loops + data.tick));
				}
				LevelEditorPlaySound("sndEditorEventCreate", "LevelEditorActive");
				base.editor.ShowTabSection(Tab.Song, animateTab: false);
				void AddRDGS(OneshotPhraseToSay phraseToSay, float beatOffset, float tickMultiplier = 1f)
				{
					LevelEvent_SayReadyGetSetGo levelEvent_SayReadyGetSetGo = new LevelEvent_SayReadyGetSetGo();
					levelEvent_SayReadyGetSetGo.CopyBasePropertiesFrom(data, copyBarAndBeat: true, copyRow: false);
					levelEvent_SayReadyGetSetGo.y = 0;
					levelEvent_SayReadyGetSetGo.phraseToSay = phraseToSay;
					levelEvent_SayReadyGetSetGo.tick = data.tick * tickMultiplier;
					levelEvent_SayReadyGetSetGo.room = 4;
					float beat = levelEvent_SayReadyGetSetGo.beat - beatOffset;
					LevelEventControl_Base levelEventControl_Base = base.editor.CreateEventControl(levelEvent_SayReadyGetSetGo, Tab.Song);
					float posXFromBarAndBeat = base.timeline.GetPosXFromBarAndBeat(levelEventControl_Base.levelEvent.bar, beat);
					levelEventControl_Base.levelEvent.barAndBeat = base.timeline.GetBarAndBeatWithPosX(posXFromBarAndBeat);
					if (levelEventControl_Base.levelEvent.barAndBeat.bar == 1 && levelEventControl_Base.levelEvent.barAndBeat.beat == 1f)
					{
						levelEventControl_Base.levelEvent.barAndBeat.beat += 0.001f;
					}
					levelEventControl_Base.UpdateUI();
				}
			}
		}
	}
}
