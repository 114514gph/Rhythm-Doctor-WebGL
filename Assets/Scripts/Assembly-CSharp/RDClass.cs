using RDLevelEditor;
using RDTools;

public class RDClass : RDClassDll
{
	public scrConductor conductor => scrConductor.instance;

	public scnBase wrldBase => scnBase.instance;

	public scnGame game => scnBase.instance as scnGame;

	public scrGameManager gm => scrGameManager.instance;

	public RDInk ink => scrGameManager.instance.inkDialogue;

	public static scrVfxControl Vfx => RDBase.Vfx;

	public RDConstants gc => RDConstants.data;

	public bool editorMode => scnEditor.instance != null;

	protected static DebugSettings debugSettings => DebugSettings.instance;

	public scnEditor editor => scnEditor.instance;

	public PlatformHelper platformHelper => PlatformHelper.instance;
}
