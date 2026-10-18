public class Level_LesMis_Helper : RDBase
{
	private void Update()
	{
		scrConductor instance = scrConductor.instance;
		if (RDInput.anyPlayerPress && instance.barNumber == 33)
		{
			instance.Pause(isPause: false);
		}
	}
}
