public class DisableIfEditor : RDBase
{
	public bool negate;

	private void Start()
	{
		if (base.editorMode != negate)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
