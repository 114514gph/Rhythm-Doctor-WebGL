using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SleevePaintCrosshair : RDBase
{
	private const int positionOffset = 6;

	public RectTransform canvas;

	public float leftLimit;

	public float rightLimit;

	public float topLimit;

	public float bottomLimit;

	public float speed;

	public float consoleSpeedMultiplier;

	public float colorSpeed;

	public Color purple;

	public Camera currentCamera;

	public EventSystem eventSystem;

	[NonSerialized]
	public Vector2 screenPosition;

	private RectTransform rt;

	private Vector2 currentPosition;

	private Vector3 lastMousePos;

	private Image image;

	private Button currentButton;

	private EventTrigger currentEventTrigger;

	private PointerEventData pointer;

	private List<RaycastResult> raycastResult = new List<RaycastResult>();

	private void Start()
	{
		rt = GetComponent<RectTransform>();
		image = GetComponent<Image>();
		lastMousePos = Input.mousePosition;
		currentPosition = rt.anchoredPosition;
		pointer = new PointerEventData(eventSystem);
		if (rightLimit == -1f)
		{
			rightLimit = canvas.sizeDelta.x - 1f;
		}
		if (topLimit == -1f)
		{
			topLimit = canvas.sizeDelta.y - 1f;
		}
	}

	private void Update()
	{
		int num = Mathf.RoundToInt(canvas.sizeDelta.x);
		int num2 = Mathf.RoundToInt(canvas.sizeDelta.y);
		float t = Mathf.Sin(Time.unscaledTime * colorSpeed);
		image.color = new Color(Mathf.Lerp(purple.r, Color.yellow.r, t), Mathf.Lerp(purple.g, Color.yellow.g, t), Mathf.Lerp(purple.b, Color.yellow.b, t), 1f);
		Vector3 mousePosition = Input.mousePosition;
		if (mousePosition != lastMousePos)
		{
			currentPosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width) / (float)Screen.width * (float)num;
			currentPosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height) / (float)Screen.height * (float)num2;
			currentPosition.x -= ((float)num - rightLimit) / 2f;
			currentPosition.y -= ((float)num2 - topLimit) / 2f;
		}
		else
		{
			currentPosition += speed * (scnBase.isConsole ? consoleSpeedMultiplier : 1f) * RDInput.position;
		}
		currentPosition.x = Mathf.Clamp(currentPosition.x, leftLimit, rightLimit);
		currentPosition.y = Mathf.Clamp(currentPosition.y, bottomLimit, topLimit);
		rt.anchoredPosition = new Vector2(Mathf.RoundToInt(currentPosition.x) - 6, Mathf.RoundToInt(currentPosition.y) - 6);
		screenPosition = new Vector2((currentPosition.x + ((float)num - rightLimit) / 2f) / (float)num * (float)Screen.width, (currentPosition.y + ((float)num2 - topLimit) / 2f) / (float)num2 * (float)Screen.height);
		bool flag = false;
		pointer.position = screenPosition;
		eventSystem.RaycastAll(pointer, raycastResult);
		foreach (RaycastResult item in raycastResult)
		{
			Button component = item.gameObject.GetComponent<Button>();
			if (component != null)
			{
				flag = true;
				if (currentButton != component)
				{
					currentButton = component;
					if (currentButton.transition == Selectable.Transition.SpriteSwap)
					{
						(currentButton.targetGraphic as Image).sprite = currentButton.spriteState.highlightedSprite;
					}
					else
					{
						currentButton.Select();
					}
				}
				break;
			}
			currentEventTrigger = item.gameObject.GetComponent<EventTrigger>();
			if (currentEventTrigger != null)
			{
				flag = true;
				break;
			}
		}
		raycastResult.Clear();
		if (flag)
		{
			if (!Input.GetKeyDown(KeyCode.Z))
			{
				if (RDInput.anyPlayerPress || Input.GetMouseButtonDown(0))
				{
					if (currentButton != null)
					{
						currentButton.onClick.Invoke();
					}
					else
					{
						currentEventTrigger.OnPointerDown(pointer);
					}
				}
				else if ((RDInput.anyPlayerRelease || Input.GetMouseButtonUp(0)) && currentEventTrigger != null)
				{
					currentEventTrigger.OnPointerUp(pointer);
				}
			}
		}
		else if (currentButton != null)
		{
			if (currentButton.transition == Selectable.Transition.SpriteSwap)
			{
				(currentButton.targetGraphic as Image).sprite = currentButton.spriteState.selectedSprite;
			}
			else
			{
				eventSystem.SetSelectedGameObject(null);
			}
			currentButton = null;
		}
		lastMousePos = mousePosition;
	}

	private void OnGUI()
	{
	}
}
