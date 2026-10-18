using UnityEngine;

public class RDGarden : RDEnvironment
{
	public GameObject dayContainer;

	public GameObject nightContainer;

	public void SetNightMode(bool nightMode)
	{
		dayContainer.SetActive(!nightMode);
		nightContainer.SetActive(nightMode);
	}
}
