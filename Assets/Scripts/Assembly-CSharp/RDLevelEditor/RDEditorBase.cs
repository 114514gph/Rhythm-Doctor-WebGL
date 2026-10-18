using UnityEngine;

namespace RDLevelEditor
{
	public class RDEditorBase : RDBaseDllDummy
	{
		public const int SongTabIndex = 0;

		public const int RowsTabIndex = 1;

		public const int ActionsTabIndex = 2;

		public const int RoomsTabIndex = 3;

		public const int DecorationsTabIndex = 4;

		public const int WindowsTabIndex = 5;

		public const int AllRowsIndex = -1;

		public scnEditor editor => scnEditor.instance;

		public RDInspectorPanelManager ipm => editor.inspectorPanelManager;

		public Timeline timeline => scnEditor.instance.timelineScript;

		public scrConductor conductor => scrConductor.instance;

		public RDConstants constants => RDConstants.data;

		public scnGame game => scnEditor.gameInstance;

		public TabSection_Song tabSection_song => (TabSection_Song)editor.tabSections[0];

		public TabSection_Rows tabSection_rows => (TabSection_Rows)editor.tabSections[1];

		public TabSection_Actions tabSection_actions => (TabSection_Actions)editor.tabSections[2];

		public TabSection_Rooms tabSection_rooms => (TabSection_Rooms)editor.tabSections[3];

		public TabSection_Decorations tabSection_decorations => (TabSection_Decorations)editor.tabSections[4];

		public TabSection_Windows tabSection_windows => (TabSection_Windows)editor.tabSections[5];

		public RDConstants gc => RDConstants.data;

		public int cellWidth => timeline.cellWidth;

		public int cellHeight => timeline.cellHeight;

		protected static DebugSettings debugSettings => DebugSettings.instance;

		public T GetPanel<T>() where T : InspectorPanel
		{
			return editor.inspectorPanelManager.Get<T>();
		}

		public AudioSource LevelEditorPlaySound(string sound, string group = "LevelEditorParent", float volume = 1f, float pitch = 1f, float pan = 0f)
		{
			return scrConductor.PlayImmediately(sound, volume, RDUtils.GetMixerGroup(group), pitch, pan, isBeatSound: false, ignoreListenerPause: true);
		}
	}
}
