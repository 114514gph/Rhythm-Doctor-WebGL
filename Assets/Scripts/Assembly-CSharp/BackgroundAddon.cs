using UnityEngine;

public class BackgroundAddon : MonoBehaviour
{
	public Vector2 velocity;

	private void Update()
	{
		Vector3 vector = GetComponent<tk2dTiledSprite>().GetCurrentSpriteDef().untrimmedBoundsData[1];
		Vector3 translation = new Vector3(velocity.x * Time.deltaTime * vector.x, velocity.y * Time.deltaTime * vector.y, 0f);
		Vector3 localPosition = base.transform.localPosition;
		base.transform.localPosition = new Vector3(FlooredDivision(localPosition.x, vector.x), FlooredDivision(localPosition.y, vector.y), 0f);
		base.transform.Translate(translation);
	}

	private float FlooredDivision(float a, float n)
	{
		return a - n * Mathf.Floor(a / n);
	}
}
