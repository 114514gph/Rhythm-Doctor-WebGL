using System;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public abstract class SyllableInspectorPanel : InspectorPanel
	{
		public InputField id;

		public Text fragment;

		public InputField fadeOutDuration;

		protected new virtual void Awake()
		{
			base.Awake();
			AddOnEditListeners(null, id, fadeOutDuration);
			(fadeOutDuration as RDInputField).isNullableField = true;
		}

		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_SyllableBase levelEvent_SyllableBase = (LevelEvent_SyllableBase)levelEvent;
			fadeOutDuration.text = (levelEvent_SyllableBase.fadeOutDuration.HasValue ? levelEvent_SyllableBase.fadeOutDuration.ToString() : "");
		}

		protected override void SaveInternal(LevelEvent_Base levelEvent)
		{
			((LevelEvent_SyllableBase)levelEvent).fadeOutDuration = ((!fadeOutDuration.text.IsNullOrEmpty()) ? new float?(Convert.ToSingle(fadeOutDuration.text)) : ((float?)null));
		}
	}
}
