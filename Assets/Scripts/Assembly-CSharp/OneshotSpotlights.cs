using DG.Tweening;
using UnityEngine;

public class OneshotSpotlights : RDBase
{
	public GameObject[] containers;

	public Transform[] transforms;

	public tk2dSprite[] sprites;

	public bool isRoomSpotlight;

	private void Awake()
	{
		GameObject[] array = containers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		if (isRoomSpotlight)
		{
			SetupForRooms();
		}
	}

	private void Start()
	{
		RotateSpotlight(transforms[0], -55f);
		RotateSpotlight(transforms[1], -122f);
		RotateSpotlight(transforms[2], 55f);
		RotateSpotlight(transforms[3], 122f);
		static void RotateSpotlight(Transform t, float angle)
		{
			Vector3 endValue = new Vector3(0f, 0f, angle);
			t.DOLocalRotate(endValue, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuart);
		}
	}

	public void SetupForRooms()
	{
		tk2dSprite[] array = sprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.layer = 0;
		}
	}

	private void LateUpdate()
	{
		if (!base.game.windowDancing || isRoomSpotlight)
		{
			Layout();
		}
	}

	public void Layout(int windowIndex = -1)
	{
		float num = ((windowIndex != -1) ? base.game.windowChoreographer.dancers[windowIndex].windowScale.y : 1f);
		float num2 = RDBase.Vfx.RDHeight / 2f * num + 15f;
		transforms[0].LocalMoveY(0f - num2);
		transforms[1].LocalMoveY(0f - num2);
		transforms[2].LocalMoveY(num2);
		transforms[3].LocalMoveY(num2);
	}

	public void Show(int index, bool fromTop = false)
	{
		if (fromTop)
		{
			index += 2;
		}
		AppearSpotlight(index);
	}

	public void Hide(int index, bool fromTop = false)
	{
		if (fromTop)
		{
			index += 2;
		}
		containers[index].SetActive(value: false);
	}

	public void Flash(int index, bool fromTop = false)
	{
		if (fromTop)
		{
			index += 2;
		}
		FlashSpotlight(index);
	}

	public void Show(bool fromTop = false)
	{
		Show(0, fromTop);
		Show(1, fromTop);
	}

	public void Hide(bool fromTop = false)
	{
		Hide(0, fromTop);
		Hide(1, fromTop);
	}

	public void Flash(bool fromTop = false)
	{
		Flash(0, fromTop);
		Flash(1, fromTop);
	}

	private void AppearSpotlight(int index)
	{
		containers[index].SetActive(value: true);
		sprites[index].transform.DOScaleX(1f, 0.12f).SetEase(Ease.OutExpo).From(new Vector3(0.8f, 1f, 1f));
	}

	private void FlashSpotlight(int index)
	{
		containers[index].SetActive(value: true);
		Transform target = sprites[index].transform;
		Timer.Add(() =>
		{
			containers[index].SetActive(value: false);
		}, 0.12f);
		target.DOScaleX(1f, 0.1f).SetEase(Ease.OutExpo).From(new Vector3(0.8f, 1f, 1f));
	}

	private void OnDestroy()
	{
		Transform[] array = transforms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DOKill();
		}
	}
}
