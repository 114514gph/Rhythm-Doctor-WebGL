using UnityEngine;

public class RDSpaceBackground : MonoBehaviour
{
	public Camera starsCam;

	public Camera mainCam;

	public Transform planet;

	public ParticleSystem stars;

	public ParticleSystem asteroids;

	public Transform bgPlane;

	public GameObject ship;

	[HideInInspector]
	public bool isCloseup;

	public void SwitchToCloseupMode()
	{
		isCloseup = true;
		ship.SetActive(value: false);
		planet.localPosition = new Vector3(-500f, -97f, -3070f);
		planet.localRotation = Quaternion.Euler(0f, -100f, -72f);
		asteroids.transform.localPosition = new Vector3(-500f, -60f, -2840f);
		ParticleSystem.MainModule main = asteroids.main;
		main.simulationSpeed = 3f;
		stars.transform.localPosition = new Vector3(-730f, 1072f, -700f);
		stars.transform.localRotation = Quaternion.Euler(-90f, -70.365f, 0f);
		ParticleSystem.ShapeModule shape = stars.shape;
		shape.scale = new Vector3(0f, 600f, 600f);
		bgPlane.localPosition = new Vector3(10f, 10f, 13.6f);
		mainCam.fieldOfView = 50f;
		Color backgroundColor = mainCam.backgroundColor;
		mainCam.clearFlags = CameraClearFlags.Depth;
		starsCam.clearFlags = CameraClearFlags.Color;
		starsCam.backgroundColor = backgroundColor;
		starsCam.depth = -3f;
	}
}
