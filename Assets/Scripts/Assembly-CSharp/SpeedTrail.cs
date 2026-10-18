using UnityEngine;

[RequireComponent(typeof(tk2dSprite))]
public class SpeedTrail : MonoBehaviour
{
	public int length = 5;

	private tk2dSprite orig;

	private tk2dSprite[] trails;

	private MeshRenderer origRenderer;

	private Vector3 prevPos;

	private Quaternion prevRot;

	private Vector3 prevScale;

	private float frameTime;

	private int lifetime;

	private bool init;

	private void Init()
	{
		orig = GetComponent<tk2dSprite>();
		origRenderer = GetComponent<MeshRenderer>();
		trails = new tk2dSprite[length];
		for (int i = 0; i < length; i++)
		{
			GameObject obj = new GameObject("(Trail) " + base.gameObject.name);
			obj.transform.parent = base.transform.parent;
			obj.transform.position = base.transform.position;
			MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
			tk2dSprite tk2dSprite2 = obj.AddComponent<tk2dSprite>();
			meshRenderer.sortingLayerID = origRenderer.sortingLayerID;
			meshRenderer.sortingOrder = origRenderer.sortingOrder - 1 - i;
			tk2dSprite2.color = new Color(0f, 0f, 0f, 0f);
			trails[i] = tk2dSprite2;
		}
		prevPos = base.transform.position;
		prevRot = base.transform.rotation;
		prevScale = base.transform.localScale;
		init = true;
	}

	private void SetTrailActive(bool active)
	{
		if (active && !init)
		{
			Init();
		}
		tk2dSprite[] array = trails;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(active);
		}
	}

	private void OnDisable()
	{
		SetTrailActive(active: false);
	}

	private void OnEnable()
	{
		SetTrailActive(active: true);
	}

	private void OnDestroy()
	{
		if (!init)
		{
			return;
		}
		tk2dSprite[] array = trails;
		foreach (tk2dSprite tk2dSprite2 in array)
		{
			if (tk2dSprite2 != null)
			{
				Object.Destroy(tk2dSprite2.gameObject);
			}
		}
	}

	private void LateUpdate()
	{
		MeshRenderer meshRenderer = origRenderer;
		if ((object)meshRenderer == null || !meshRenderer.enabled)
		{
			tk2dSprite[] array = trails;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = Color.clear;
			}
			return;
		}
		for (int j = 0; j < length - 1; j++)
		{
			tk2dSprite tk2dSprite2 = trails[j];
			Transform transform = tk2dSprite2.transform;
			if (lifetime - (length - 1 - j) > 0)
			{
				if (j < length - 2)
				{
					Transform transform2 = trails[j + 1].transform;
					transform.position = transform2.position;
					transform.rotation = transform2.rotation;
					transform.localScale = transform2.localScale;
				}
				else
				{
					transform.position = prevPos;
					transform.rotation = prevRot;
					transform.localScale = prevScale;
				}
				tk2dSprite2.SetSprite(orig.Collection, orig.spriteId);
				tk2dSprite2.color = orig.color.WithAlpha(orig.color.a * ((float)j / (float)length));
			}
		}
		prevPos = base.transform.position;
		prevRot = base.transform.rotation;
		prevScale = base.transform.localScale;
		lifetime++;
	}
}
