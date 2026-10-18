using System;
using UnityEngine;
using UnityEngine.Events;

namespace RhythmWeightlifter
{
	[Serializable]
	public class ExtraContainer
	{
		public RectTransform container;

		public string titleKey;

		public string bodyKey;

		public bool isOnlyMobile;

		public bool requiresFinishedCampaign;

		public UnityEvent onSelect;

		public UnityEvent onShow;

		[NonSerialized]
		public bool visible = true;
	}
}
