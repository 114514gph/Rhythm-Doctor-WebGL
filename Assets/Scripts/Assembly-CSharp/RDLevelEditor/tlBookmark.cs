using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class tlBookmark : RDEditorBase
	{
		public const string barKey = "bar";

		public const string beatKey = "beat";

		public const string colorKey = "color";

		public Image image;

		public RectTransform rt;

		public Color[] baseColors;

		public Color[] highlightedColors;

		[NonSerialized]
		public Image lineImage;

		[NonSerialized]
		public RectTransform lineRect;

		private Color currentColor;

		private Color currentHighlightedColor;

		[NonSerialized]
		public int currentColorIndex;

		private bool highlighted;

		public bool isSelected => highlighted;

		private void Awake()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(base.timeline.bookmarkLinePrefab, base.timeline.scrollbarLinesContainer);
			lineImage = gameObject.GetComponent<Image>();
			lineRect = gameObject.GetComponent<RectTransform>();
			currentColor = baseColors[0];
			currentHighlightedColor = highlightedColors[0];
			UpdateImageColor();
		}

		public void NextColor()
		{
			SetColor(currentColorIndex + 1);
		}

		public void PreviousColor()
		{
			SetColor(currentColorIndex - 1);
		}

		public void SetColor(int index)
		{
			int num = index;
			if (num >= baseColors.Length)
			{
				num = 0;
			}
			else if (num < 0)
			{
				num = baseColors.Length - 1;
			}
			currentColor = baseColors[num];
			currentHighlightedColor = highlightedColors[num];
			currentColorIndex = num;
			UpdateImageColor();
		}

		public void SetHighlighted(bool on)
		{
			highlighted = on;
			UpdateImageColor();
		}

		private void UpdateImageColor()
		{
			Color color = (highlighted ? currentHighlightedColor : currentColor);
			image.color = color;
			lineImage.color = color;
		}

		public void DestroyLine()
		{
			if (lineRect != null)
			{
				UnityEngine.Object.Destroy(lineRect.gameObject);
			}
		}
	}
}
