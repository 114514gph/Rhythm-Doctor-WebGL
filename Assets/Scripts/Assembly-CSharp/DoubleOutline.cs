using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
[ExecuteInEditMode]
public class DoubleOutline : MonoBehaviour
{
	public Color color;

	public float distance = 2f;

	private void Update()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			if (GetComponents<Outline>().Length < 2)
			{
				base.gameObject.AddComponent<Outline>();
			}
			Outline[] components = GetComponents<Outline>();
			int num = 0;
			Outline[] array = components;
			foreach (Outline obj in array)
			{
				obj.effectColor = color;
				bool flag = num == 0;
				obj.effectDistance = new Vector2(flag ? distance : 0f, flag ? 0f : distance);
				num++;
			}
		}
	}
}
