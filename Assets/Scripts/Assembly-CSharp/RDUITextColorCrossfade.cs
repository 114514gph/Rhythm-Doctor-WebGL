using UnityEngine;
using UnityEngine.UI;

public class RDUITextColorCrossfade : MonoBehaviour
{
	public Color colorA;

	public Color colorB;

	public float speed;

	private void Update()
	{
		float t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f;
		Color color = Color.Lerp(colorA, colorB, t);
		GetComponent<Text>().color = color;
	}
}
