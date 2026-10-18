using UnityEngine;

public class RDCreditsNames : RDBase
{
	public Transform winston;

	public Transform hafiz;

	public Transform giacomo;

	public Transform ashley;

	public Transform alberto;

	public void HideAll()
	{
		winston.gameObject.SetActive(value: false);
		hafiz.gameObject.SetActive(value: false);
		giacomo.gameObject.SetActive(value: false);
		ashley.gameObject.SetActive(value: false);
		alberto.gameObject.SetActive(value: false);
	}

	private void Start()
	{
		HideAll();
		base.transform.parent = RDBase.Vfx.rooms[0].backgroundContainerCamera;
		RDBase.Vfx.CenterThis(base.transform);
	}

	public void ShowThis(Transform t)
	{
		HideAll();
		t.gameObject.SetActive(value: true);
	}
}
