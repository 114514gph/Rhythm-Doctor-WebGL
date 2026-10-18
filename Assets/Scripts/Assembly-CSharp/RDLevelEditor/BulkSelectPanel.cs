using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class BulkSelectPanel : RDEditorBase
	{
		public GameObject barInput;

		public GameObject tagInput;

		public InputField tagText;

		public Text tagMixedText;

		public Toggle tagRunToggle;

		public Button barPlusButton;

		public Button barMinusButton;

		private bool initializing;

		private void Start()
		{
			barPlusButton.onClick.AddListener(() =>
			{
				base.editor.OffsetSelectedEventsByBar(1);
			});
			barMinusButton.onClick.AddListener(() =>
			{
				base.editor.OffsetSelectedEventsByBar(-1);
			});
		}

		public void Initialize()
		{
			initializing = true;
			ShowTagInput(base.editor.tagFieldMode);
			string text = base.editor.selectedControls[0].levelEvent.tag ?? "";
			bool flag = true;
			bool flag2 = base.editor.selectedControls[0].levelEvent.tagRunNormally;
			for (int i = 1; i < base.editor.selectedControls.Count; i++)
			{
				LevelEvent_Base levelEvent = base.editor.selectedControls[i].levelEvent;
				if ((levelEvent.tag ?? "") != text)
				{
					flag = false;
				}
				if (!levelEvent.tagRunNormally)
				{
					flag2 = false;
				}
				if (!flag && !flag2)
				{
					break;
				}
			}
			tagText.text = (flag ? text : "");
			tagMixedText.gameObject.SetActive(!flag);
			tagText.placeholder = ((!flag) ? tagMixedText : null);
			tagRunToggle.isOn = flag2;
			tagInput.GetComponent<RDColorHoverEventTrigger>().enterColor = base.editor.inspectorTitle.color;
			barInput.GetComponent<RDColorHoverEventTrigger>().enterColor = base.editor.inspectorTitle.color;
			initializing = false;
		}

		public void ShowTagInput(bool active)
		{
			barInput.SetActive(!active);
			tagInput.SetActive(active);
			base.editor.tagFieldMode = active;
			if (!initializing)
			{
				LevelEditorPlaySound("sndButtonClick");
			}
		}

		public void UpdateTag(string tag)
		{
			if (initializing)
			{
				return;
			}
			foreach (LevelEventControl_Base selectedControl in base.editor.selectedControls)
			{
				selectedControl.levelEvent.tag = tag;
				selectedControl.SaveAndUpdateUI();
			}
			LevelEditorPlaySound("sndEditorValueChange");
		}

		public void UpdateTagRunNormally(bool active)
		{
			if (!Application.isPlaying || initializing)
			{
				return;
			}
			foreach (LevelEventControl_Base selectedControl in base.editor.selectedControls)
			{
				selectedControl.levelEvent.tagRunNormally = active;
				selectedControl.SaveAndUpdateUI();
			}
			LevelEditorPlaySound("sndButtonRadio");
		}
	}
}
