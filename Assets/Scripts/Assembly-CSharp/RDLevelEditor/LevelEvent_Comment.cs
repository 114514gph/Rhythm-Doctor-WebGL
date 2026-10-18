using System;
using DG.Tweening;
using RDTools;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_Comment : LevelEvent_Base
	{
		public struct CommentColors
		{
			public Color textColor;

			public Color backgroundColor;

			public Color outlineColor;
		}

		private static readonly Color originalTextColor = "323232".HexToColor();

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("", InputField.LineType.MultiLineNewline, 900, 84, false, true)]
		public string text { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ColorInfo(false)]
		public ColorOrPalette color { get; set; } = RDConstants.data.colorPalette[10];

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Checkbox]
		public bool show { get; set; }

		public override string Encode()
		{
			usesY = tab != Tab.Decorations;
			return base.Encode();
		}

		public CommentColors GetCommentColors(bool editing = false)
		{
			Color backgroundColor = color.ToColor();
			bool flag = RDEditorUtils.ColorIsDark(backgroundColor);
			Color textColor = ((flag && !editing) ? Color.white : originalTextColor);
			return new CommentColors
			{
				textColor = textColor,
				backgroundColor = backgroundColor,
				outlineColor = (flag ? Color.white : Color.black)
			};
		}

		public override void Run()
		{
			string text = this.text;
			if (text.StartsWith("()=>"))
			{
				text = text.Remove(0, "()=>".Length);
				RunOnBeat(() =>
				{
					text = text.Trim();
					if (!string.IsNullOrEmpty(text))
					{
						if (text.StartsWith("create"))
						{
							text = text.RemoveTextIfStartsWith("create");
							string[] parameters = text.GetParameters();
							if (parameters != null && parameters.Length == 3)
							{
								string text2 = parameters[0];
								if (!text2.StartsWith("CustomParticles") || !base.conductor.isScrubbing)
								{
									float num = float.Parse(parameters[1]);
									float num2 = float.Parse(parameters[2]);
									string text3 = "PublicPrefabs/" + text2;
									UnityEngine.Object obj = Resources.Load(text3);
									if (!(obj == null))
									{
										GameObject obj2 = UnityEngine.Object.Instantiate(obj, scrGameManager.instance.parentContainer) as GameObject;
										if (obj2 == null)
										{
											RDClassDll.printem("Resource not found! " + text3);
										}
										obj2.transform.LocalMoveXY(num / 100f * base.vfx.RDWidth, num2 / 100f * base.vfx.RDHeight);
									}
								}
							}
						}
						else if (text.StartsWith("freeze"))
						{
							if (!base.conductor.isScrubbing)
							{
								text = text.RemoveTextIfStartsWith("freeze");
								string[] parameters2 = text.GetParameters();
								if (parameters2 != null && parameters2.Length <= 2)
								{
									PlayStyleChange unfreeze = RDEditorUtils.DecodeEnum<PlayStyleChange>(parameters2[0]);
									if (parameters2.Length == 2)
									{
										RDEditorUtils.DecodeEnum<PlayStyleChange>(parameters2[1]);
									}
									if (parameters2.Length == 2)
									{
										base.conductor.Freeze(unfreeze);
										Debug.LogWarning(" redundant second Freeze argument, consider cleaning up.");
									}
									else
									{
										base.conductor.Freeze(unfreeze);
									}
								}
							}
						}
						else if (text.StartsWith("unfreeze"))
						{
							if (!base.conductor.isScrubbing)
							{
								text = text.RemoveTextIfStartsWith("unfreeze");
								string[] parameters3 = text.GetParameters();
								if (parameters3 != null && parameters3.Length == 1)
								{
									PlayStyleChange changeStyle = RDEditorUtils.DecodeEnum<PlayStyleChange>(parameters3[0]);
									base.conductor.SetPlayStyle(changeStyle);
								}
							}
						}
						else if (text.StartsWith("setPlayStyle"))
						{
							if (!base.conductor.isScrubbing)
							{
								text = text.RemoveTextIfStartsWith("setPlayStyle");
								string[] parameters4 = text.GetParameters();
								if (parameters4 != null && parameters4.Length == 1)
								{
									PlayStyleChange changeStyle2 = RDEditorUtils.DecodeEnum<PlayStyleChange>(parameters4[0]);
									base.conductor.SetPlayStyle(changeStyle2);
								}
							}
						}
						else if (text.StartsWith("shockwave"))
						{
							text = text.RemoveTextIfStartsWith("shockwave");
							string[] parameters5 = text.GetParameters();
							if (parameters5 != null && parameters5.Length == 2)
							{
								switch (parameters5[0])
								{
								case "distortion":
									LevelEvent_Base.level.shockwaveDistortionMultiplier = RDEditorUtils.DecodeFloat(parameters5[1]);
									break;
								case "duration":
									LevelEvent_Base.level.shockwaveDurationMultiplier = RDEditorUtils.DecodeFloat(parameters5[1]);
									break;
								case "size":
									LevelEvent_Base.level.shockwaveSizeMultiplier = RDEditorUtils.DecodeFloat(parameters5[1]);
									break;
								}
							}
						}
						else if (text.StartsWith("roomOpacity"))
						{
							text = text.RemoveTextIfStartsWith("roomOpacity");
							string[] parameters6 = text.GetParameters();
							if (parameters6 != null && parameters6.Length == 3)
							{
								int num3 = RDEditorUtils.DecodeInt(parameters6[0]);
								float opacity = RDEditorUtils.DecodeFloat(parameters6[1]);
								float durationInCrotchets = RDEditorUtils.DecodeFloat(parameters6[2]);
								Material roomMaterial = base.game.rooms[num3].renderQuad.GetComponent<Renderer>().material;
								float duration = base.conductor.DurationBeatsToTime(base.beat - 1f, durationInCrotchets);
								DOTween.To(() => roomMaterial.GetFloat("_Opacity"), (float o) =>
								{
									roomMaterial.SetFloat("_Opacity", o);
								}, opacity, duration).OnComplete(() =>
								{
									roomMaterial.SetFloat("_Opacity", opacity);
								}).Done();
							}
						}
						else if (text.StartsWith("wavyRowsAmplitude"))
						{
							text = text.RemoveTextIfStartsWith("wavyRowsAmplitude");
							string[] parameters7 = text.GetParameters();
							if (parameters7 != null && parameters7.Length == 3)
							{
								int num4 = RDEditorUtils.DecodeInt(parameters7[0]);
								float amplitude = RDEditorUtils.DecodeFloat(parameters7[1]);
								float durationInCrotchets2 = RDEditorUtils.DecodeFloat(parameters7[2]);
								float duration2 = base.conductor.DurationBeatsToTime(base.beat - 1f, durationInCrotchets2);
								RDRoom targetRoom = base.game.rooms[num4];
								DOTween.To(() => targetRoom.wavyRowsAmplitude, (float a) =>
								{
									targetRoom.wavyRowsAmplitude = a;
								}, amplitude, duration2).OnComplete(() =>
								{
									targetRoom.wavyRowsAmplitude = amplitude;
								}).Done();
							}
						}
						else if (text.StartsWith("trueCameraMove"))
						{
							text = text.RemoveTextIfStartsWith("trueCameraMove");
							string[] parameters8 = text.GetParameters();
							if (parameters8 != null && parameters8.Length == 5)
							{
								int num5 = RDEditorUtils.DecodeInt(parameters8[0]);
								float x = RDEditorUtils.DecodeFloat(parameters8[1]);
								float num6 = RDEditorUtils.DecodeFloat(parameters8[2]);
								float durationInCrotchets3 = RDEditorUtils.DecodeFloat(parameters8[3]);
								Ease ease = RDEditorUtils.DecodeEnum<Ease>(parameters8[4]);
								float duration3 = base.conductor.DurationBeatsToTime(base.beat - 1f, durationInCrotchets3);
								Transform targetRoomCam = base.game.rooms[num5].camera.transform.parent;
								Vector3 targetVector = new Vector3(x, num6, targetRoomCam.localPosition.z);
								targetRoomCam.DOKill();
								targetRoomCam.DOLocalMove(targetVector, duration3).OnComplete(() =>
								{
									targetRoomCam.localPosition = targetVector;
								}).SetEase(ease)
									.Done();
							}
						}
						else if (text.StartsWith("windowSize"))
						{
							text = text.RemoveTextIfStartsWith("windowSize");
							string[] parameters9 = text.GetParameters();
							if (parameters9 != null && parameters9.Length == 3)
							{
								float num7 = RDEditorUtils.DecodeFloat(parameters9[0]);
								float num8 = RDEditorUtils.DecodeFloat(parameters9[1]);
								float easeDur = RDEditorUtils.DecodeFloat(parameters9[2]);
								base.game.windowChoreographer.dancers[0].SetScale(new Float2(num7 / 100f, num8 / 100f), easeDur, Ease.Linear);
							}
						}
						else if (text.StartsWith("roomPeek"))
						{
							text = text.RemoveTextIfStartsWith("roomPeek");
							string[] parameters10 = text.GetParameters();
							if (parameters10 != null)
							{
								bool flag = false;
								if (parameters10.Length == 3)
								{
									flag = Convert.ToBoolean(parameters10[2]);
								}
								else if (parameters10.Length != 2)
								{
									return;
								}
								int num9 = RDEditorUtils.DecodeInt(parameters10[0]);
								bool peekWindowMode = Convert.ToBoolean(parameters10[1]);
								base.game.rooms[num9].peekWindowMode = peekWindowMode;
								if (flag)
								{
									base.game.rooms[num9].PeekWindowModeUpdate();
								}
							}
						}
						else
						{
							RDClassDll.printem("method not found! " + text);
						}
					}
				});
			}
			else if (base.game.editorMode && show && !base.conductor.isScrubbing && base.editor.commentPlaybackEnabled)
			{
				RunOnBeat(() =>
				{
					InspectorPanel_CommentShow inspectorPanel_CommentShow = base.editor.inspectorPanelManager.Get<InspectorPanel_CommentShow>();
					InspectorPanel.ShowPanel(inspectorPanel_CommentShow.transform, RDString.Get("editor.Comment"), base.defaultTab);
					inspectorPanel_CommentShow.comment.text = text;
					RDEditorUtils.UpdateUIText(inspectorPanel_CommentShow.comment.textComponent, text);
					inspectorPanel_CommentShow.bar.text = RDString.Get("editor.bar") + ": " + base.bar;
					RDEditorUtils.UpdateUIText(inspectorPanel_CommentShow.bar, inspectorPanel_CommentShow.bar.text);
					inspectorPanel_CommentShow.beat.text = RDString.Get("editor.beat") + ": " + base.beat;
					RDEditorUtils.UpdateUIText(inspectorPanel_CommentShow.beat, inspectorPanel_CommentShow.beat.text);
					inspectorPanel_CommentShow.UpdateTextColor(GetCommentColors());
				});
			}
		}

		public override string CustomControlName()
		{
			return "Comment";
		}

		public override string GetTooltipText()
		{
			return text;
		}
	}
}
