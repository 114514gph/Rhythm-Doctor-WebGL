using UnityEngine;

public class RDSevenBeatArmy : InvisibleEntity
{
	private const float StartingZ = 10f;

	private const int Levels = 7;

	public float factor = 4f;

	public float time;

	public GameObject quadPrefab;

	public Transform zoom;

	public Transform pivot;

	public Transform quadsContainer;

	public float speed = 1f;

	public float borderOffset = 1f;

	public AnimationCurve curve;

	public Camera newFrontCam;

	public bool beatSynced;

	private GameObject[] mQuads = new GameObject[7];

	private RenderTexture mTexture;

	private Vector2 mResolution;

	public override void Awake()
	{
		base.Awake();
		mResolution = new Vector2(RDBase.Vfx.RDWidth, RDBase.Vfx.RDHeight);
		mTexture = new RenderTexture((int)mResolution.x * 2, (int)mResolution.y * 2, 16, RenderTextureFormat.ARGB32);
		mTexture.antiAliasing = 1;
		mTexture.filterMode = FilterMode.Point;
		mTexture.name = "SevenBeatArmy";
		EnableEffect(enabled: true);
		base.gameObject.tag = "Beat";
		base.transform.position = new Vector3(mResolution.x / 2f, mResolution.y / 2f, 10f);
		base.transform.localScale = Vector3.one;
		for (int i = 0; i < 7; i++)
		{
			GameObject gameObject = Object.Instantiate(quadPrefab);
			gameObject.SetActive(value: true);
			gameObject.name = "Quad " + i;
			gameObject.transform.parent = quadsContainer;
			gameObject.GetComponent<Renderer>().material.mainTexture = mTexture;
			gameObject.layer = GC.InfiniteZoomLayer;
			mQuads[i] = gameObject;
		}
		PositionObjects();
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		mTexture.Release();
	}

	private void Update()
	{
		PositionObjects();
		ZoomIn();
	}

	private void PositionObjects()
	{
		float num = (1f - 1f / factor) / 2f;
		float num2 = 1f;
		float num3 = 0f;
		for (float num4 = 0f; num4 < 7f; num4++)
		{
			Transform obj = mQuads[(int)num4].transform;
			obj.localPosition = new Vector3(0f, 0f - num3, 0f - num4);
			float num5 = 1f / Mathf.Pow(factor, num4);
			if (num5 < 0.001f)
			{
				num5 = 0f;
			}
			obj.localScale = Proportional2DScale(num5);
			num2 = num5;
			num3 += num2 * num * borderOffset;
		}
		float scale = (beatSynced ? (curve.Evaluate(time) * (factor - 1f) + 1f) : (curve.Evaluate(time % 1f) * (factor - 1f) + 1f));
		pivot.localScale = Proportional2DScale(scale);
		pivot.localPosition = new Vector3(0f, (0f - mResolution.y) * 0.5f * borderOffset, 0f);
		quadsContainer.localPosition = -pivot.localPosition;
		quadsContainer.localScale = new Vector3(mResolution.x, mResolution.y, 1f);
	}

	private void ZoomIn()
	{
		time += Time.deltaTime * speed;
	}

	public override void OnBeat()
	{
		if (beatSynced)
		{
			time = 0f;
		}
	}

	public void EnableEffect(bool enabled)
	{
		newFrontCam.enabled = enabled;
		bool multiroom = base.game.currentLevel.multiroom;
		RDRoom rDRoom = base.game.rooms[0];
		Camera camera = rDRoom.camera;
		camera.targetTexture = (enabled ? mTexture : rDRoom.renderTexture);
		camera.clearFlags = CameraClearFlags.Color;
		camera.backgroundColor = Color.clear;
		Camera camera2 = base.game.topCamera.camera;
		camera2.backgroundColor = Color.black;
		if (multiroom)
		{
			rDRoom.renderQuad.SetActive(!enabled);
		}
		else
		{
			camera2.clearFlags = (enabled ? CameraClearFlags.Color : CameraClearFlags.Depth);
		}
	}

	private Vector3 Vector3ZeroWithZ(float z)
	{
		return new Vector3(0f, 0f, z);
	}

	private Vector3 Proportional2DScale(float scale)
	{
		return new Vector3(scale, scale, 1f);
	}

	public void JustShowASimpleScreen()
	{
		int num = 0;
		GameObject[] array = mQuads;
		foreach (GameObject gameObject in array)
		{
			if (num > 0)
			{
				gameObject.SetActive(value: false);
			}
			num++;
		}
		Camera hudCamera = base.gm.hudCamera;
		hudCamera.depth = 0.9f;
		hudCamera.clearFlags = CameraClearFlags.Depth;
		hudCamera.backgroundColor = Color.black;
		newFrontCam.clearFlags = CameraClearFlags.Color;
		newFrontCam.backgroundColor = Color.black;
		pivot.localScale = Vector3.one;
		base.enabled = false;
	}
}
