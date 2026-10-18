namespace RDLevelEditor
{
	public class RDBetaLock : RDEditorBase
	{
		private void Awake()
		{
			base.gameObject.SetActive(value: false);
			if (GC.onBetaBranch)
			{
				base.gameObject.SetActive(value: true);
			}
		}
	}
}
