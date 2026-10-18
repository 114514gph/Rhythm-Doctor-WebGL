using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class TabSection_Windows : TabSection
	{
		public Text[] labels;

		public RawImage[] previews;

		public Image[] previewBorders;

		public RectTransform listRect;

		[NonSerialized]
		public bool coloredWindows;

		public override void Setup()
		{
			int num = 0;
			Text[] array = labels;
			foreach (Text obj in array)
			{
				obj.text = RDString.Get("editor.windowIndex").Replace("[index]", (num + 1).ToString());
				RDStringToUIText.Apply(obj);
				num++;
			}
			base.Setup();
			num = 0;
			RawImage[] array2 = previews;
			foreach (RawImage p in array2)
			{
				RDEventTrigger rDEventTrigger = p.gameObject.AddComponent<RDEventTrigger>();
				RawImage previewRawImage = base.editor.gameView;
				rDEventTrigger.onPointerEnter = (PointerEventData s) =>
				{
					if (p.texture != null)
					{
						previewRawImage.texture = p.texture;
					}
				};
				rDEventTrigger.onPointerExit = (PointerEventData s) =>
				{
					scrVfxControl instance = scrVfxControl.instance;
					if (!(instance == null))
					{
						Camera camera = ((base.game.windowChoreographer is VirtualWindowChoreographer virtualWindowChoreographer) ? virtualWindowChoreographer.controller.camera : instance.hudCamera);
						previewRawImage.texture = camera.targetTexture;
					}
				};
				num++;
			}
			array = labels;
			for (int i = 0; i < array.Length; i++)
			{
				RDEventTrigger rDEventTrigger2 = array[i].gameObject.AddComponent<RDEventTrigger>();
				rDEventTrigger2.onPointerEnter = (PointerEventData s) =>
				{
					TurnColoredWindows(enabled: true);
				};
				rDEventTrigger2.onPointerExit = (PointerEventData s) =>
				{
					TurnColoredWindows(enabled: false);
				};
			}
		}

		private void TurnColoredWindows(bool enabled)
		{
			base.editor.coloredWindows = enabled;
			int num = 0;
			Text[] array = labels;
			foreach (Text obj in array)
			{
				Color effectColor = (enabled ? RDEditorConstants.windowColors[num] : "316169".HexToColor());
				obj.GetComponent<EightSidedOutline>().effectColor = effectColor;
				previewBorders[num].color = effectColor;
				num++;
			}
		}

		private void Update()
		{
			if (base.game == null || !base.game.simulateWD.gameObject.activeSelf || base.game.windowChoreographer == null || !(base.game.windowChoreographer is VirtualWindowChoreographer) || base.game.simulateWD.views == null)
			{
				return;
			}
			int num = 0;
			RawImage[] array = previews;
			foreach (RawImage rawImage in array)
			{
				VirtualWindowView[] views = base.game.simulateWD.views;
				bool flag = num < views.Length;
				if (flag)
				{
					VirtualWindowView virtualWindowView = views[num];
					rawImage.texture = virtualWindowView.rawImage.texture;
					rawImage.color = Color.white;
				}
				else
				{
					rawImage.texture = null;
					rawImage.color = Color.black;
				}
				labels[num].color = ((flag && base.game.windowChoreographer.dancers[num].visible) ? Color.white : Color.grey);
				num++;
			}
		}

		public override string DefaultControlName()
		{
			return "Window";
		}
	}
}
