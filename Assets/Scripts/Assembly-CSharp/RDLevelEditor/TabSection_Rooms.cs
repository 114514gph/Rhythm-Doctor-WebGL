using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class TabSection_Rooms : TabSection
	{
		[Header("Tab-Specific")]
		public Text[] labels;

		public RawImage[] previews;

		public RectTransform listRect;

		public override void Setup()
		{
			int num = 0;
			Text[] array = labels;
			foreach (Text obj in array)
			{
				obj.text = RDString.Get("editor.roomIndex").Replace("[index]", (num + 1).ToString());
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
		}

		private void Update()
		{
			if (base.game == null || base.game.rooms == null)
			{
				return;
			}
			int num = 0;
			RDRoom[] rooms = base.game.rooms;
			foreach (RDRoom rDRoom in rooms)
			{
				RawImage rawImage = previews[num];
				if (rDRoom.camera.enabled)
				{
					rawImage.texture = rDRoom.renderTexture;
					rawImage.texture.filterMode = FilterMode.Point;
					rawImage.color = Color.white;
				}
				else
				{
					rawImage.texture = null;
					rawImage.color = Color.black;
				}
				num++;
			}
		}

		public override string DefaultControlName()
		{
			return "ShowRooms";
		}
	}
}
