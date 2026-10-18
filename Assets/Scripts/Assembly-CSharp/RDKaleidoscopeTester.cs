using UnityEngine;

public class RDKaleidoscopeTester : MonoBehaviour
{
	public Color colorMultiplierA = new Color(1f / 3f, 0f, 2f / 15f, 1f);

	public Color colorMultiplierB = new Color(0f, 2f / 15f, 1f / 3f, 1f);

	public float colorPowerA = 30f;

	public float colorPowerB = 30f;

	private void Update()
	{
		_ = scnGame.instance.currentLevel;
	}
}
