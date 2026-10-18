using UnityEngine;

public class Bullet : RDBase
{
	public float speed = 100f;

	public double birthTime;

	public Vector3 InitialHeartLeftBorder;

	private bool mRemoveFlag;

	private void Update()
	{
		base.transform.position = InitialHeartLeftBorder + Vector3.right * speed * (float)(base.conductor.audioPos - (birthTime + (double)(4f * base.conductor.crotchet)));
	}

	private void LateUpdate()
	{
		if (mRemoveFlag)
		{
			base.gameObject.SetActive(value: false);
			Object.Destroy(base.gameObject);
		}
	}

	public void Remove()
	{
		mRemoveFlag = true;
	}
}
