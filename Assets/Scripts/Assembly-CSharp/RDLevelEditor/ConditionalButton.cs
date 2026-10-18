using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ConditionalButton : RDBase
	{
		[NonSerialized]
		public int conditionId;

		[NonSerialized]
		public string conditionGId;

		public Button conditional;

		public Image background;

		public RDEventTrigger eventTrigger;

		public GameObject outline;

		public Image outlineImage;

		public Image icon;

		public GameObject negativeIcon;

		public Text conditionalName;

		private bool _selected;

		private bool _negated;

		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected = value;
				outline.SetActive(_selected);
			}
		}

		public bool Negated
		{
			get
			{
				return _negated;
			}
			set
			{
				_negated = value;
				if (_negated)
				{
					outlineImage.color = new Color(1f, 0.01568627f, 0.01568627f);
				}
				else
				{
					outlineImage.color = new Color(0.01568628f, 1f, 1f);
				}
			}
		}
	}
}
