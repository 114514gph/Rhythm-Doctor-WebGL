using System;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScroller : MonoBehaviour
{
	public float speed;

	[NonSerialized]
	public bool finished;

	[NonSerialized]
	public RectTransform rt;

	private float targetPosY;

	private void Awake()
	{
		rt = GetComponent<RectTransform>();
	}

	private void Start()
	{
		Text componentInChildren = GetComponentInChildren<Text>();
		if ((scnMenu.instance?.creditsPages ?? scnExpoMenu.instance.creditsPages)[^1] == this)
		{
			targetPosY = componentInChildren.preferredHeight + rt.sizeDelta.y;
		}
		else
		{
			targetPosY = componentInChildren.preferredHeight + rt.sizeDelta.y / 5f;
		}
	}

	private void OnDisable()
	{
		finished = false;
		CreditsScroller creditsScroller = (scnMenu.instance?.creditsPages ?? scnExpoMenu.instance.creditsPages)[0];
		rt.anchoredPosition = ((creditsScroller == this) ? new Vector2(0f, 50f + creditsScroller.GetComponent<Credits>().extraY) : Vector2.zero);
	}

	private void Update()
	{
		bool downIsPressed = RDInput.downIsPressed;
		bool upIsPressed = RDInput.upIsPressed;
		float num = speed;
		if (RDInput.controlIsPressed)
		{
			num *= 5f;
		}
		if (upIsPressed)
		{
			num *= 15f;
		}
		else if (downIsPressed)
		{
			num *= -15f;
		}
		float num2 = num * Time.deltaTime;
		Vector2 anchoredPosition = rt.anchoredPosition;
		CreditsScroller[] array = scnMenu.instance?.creditsPages ?? scnExpoMenu.instance.creditsPages;
		if ((downIsPressed && array[0].rt.anchoredPosition.y > 50f) || (!downIsPressed && !array[^1].finished))
		{
			rt.anchoredPosition = new Vector3(0f, anchoredPosition.y + num2, 0f);
		}
		if (rt.anchoredPosition.y >= targetPosY)
		{
			finished = true;
		}
	}
}
