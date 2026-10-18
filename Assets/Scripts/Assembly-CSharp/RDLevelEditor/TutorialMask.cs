using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class TutorialMask : MonoBehaviour
	{
		public RDInk inkEditor;

		public Image[] sides;

		public Image left;

		public Image top;

		public Image right;

		public Image bottom;

		public Image[] corners;

		public Image topLeft;

		public Image topRight;

		public Image bottomLeft;

		public Image bottomRight;

		private bool clicked;

		private bool rounded = true;

		private const int width = 704;

		private const int height = 396;

		private void Update()
		{
			if (!clicked && Input.GetMouseButtonDown(0) && MaskCast())
			{
				clicked = true;
			}
			if (clicked && Input.GetMouseButtonUp(0))
			{
				clicked = false;
				if (MaskCast())
				{
					base.gameObject.SetActive(value: false);
					scnEditor.instance.TutorialAdvance();
				}
			}
		}

		private bool MaskCast()
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = Input.mousePosition;
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerEventData, list);
			if (list.Count > 0)
			{
				foreach (RaycastResult item in list)
				{
					Image[] array = sides;
					foreach (Image image in array)
					{
						if (item.gameObject == image.gameObject)
						{
							return false;
						}
					}
				}
				if (rounded)
				{
					foreach (RaycastResult item2 in list)
					{
						Image[] array = corners;
						foreach (Image image2 in array)
						{
							if (item2.gameObject == image2.gameObject)
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		public void set(int xTL, int yTL, int xBR, int yBR, bool round = true)
		{
			base.gameObject.SetActive(value: true);
			rounded = round;
			left.rectTransform.sizeDelta = new Vector2Int(xTL, 0);
			top.rectTransform.sizeDelta = new Vector2Int(0, yTL);
			right.rectTransform.sizeDelta = new Vector2Int(xBR, 0);
			bottom.rectTransform.sizeDelta = new Vector2Int(0, yBR);
			if (round)
			{
				topLeft.rectTransform.anchoredPosition = new Vector2Int(xTL, -yTL);
				topRight.rectTransform.anchoredPosition = new Vector2Int(704 - xBR, -yTL);
				bottomLeft.rectTransform.anchoredPosition = new Vector2Int(xTL, yBR - 396);
				bottomRight.rectTransform.anchoredPosition = new Vector2Int(704 - xBR, yBR - 396);
				Image[] array = corners;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: true);
				}
			}
			else
			{
				Image[] array = corners;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: false);
				}
			}
		}
	}
}
