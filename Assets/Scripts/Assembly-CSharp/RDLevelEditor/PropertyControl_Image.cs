using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_Image : PropertyControl
	{
		public InputField filename;

		public Button browse;

		private string[] values;

		public override GameObject controlObject => filename.gameObject;

		private ImageAttribute controlAttribute => (ImageAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			Text component = filename.placeholder.GetComponent<Text>();
			component.text = RDString.Get("editor.noImage");
			RDStringToUIText.Apply(component);
			browse.onClick.AddListener(() =>
			{
				ShowFileSelector(() =>
				{
					action(null);
				});
			});
			filename.onEndEdit.AddListener((string _) =>
			{
				action(null, "sndEditorValueChange");
			});
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			object eventValue = GetEventValue(levelEvent);
			values = Array.ConvertAll((object[])eventValue, Convert.ToString);
			filename.text = ((values.Length == 0) ? string.Empty : ((values.Length == 1) ? values[0] : RDString.Get("editor.multipleImages")));
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			SetEventValue(levelEvent, values);
		}

		public void ShowFileSelector(Action registerForChange)
		{
			values = RDEditorUtils.ShowFileSelectorForImages(RDString.Get("editor.dialog.selectImage"), -1L) ?? Array.Empty<string>();
			filename.text = ((values.Length == 0) ? string.Empty : ((values.Length == 1) ? values[0] : RDString.Get("editor.multipleImages")));
			registerForChange();
		}
	}
}
