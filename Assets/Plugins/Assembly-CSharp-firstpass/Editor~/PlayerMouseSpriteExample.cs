#if UNITY_EDITOR || !UNITY_WEBGL
using System;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class PlayerMouseSpriteExample : MonoBehaviour
	{
		[Tooltip("The Player that will control the mouse")]
		public int playerId;

		[Tooltip("The Rewired Action used for the mouse horizontal axis.")]
		public string horizontalAction = "MouseX";

		[Tooltip("The Rewired Action used for the mouse vertical axis.")]
		public string verticalAction = "MouseY";

		[Tooltip("The Rewired Action used for the mouse wheel axis.")]
		public string wheelAction = "MouseWheel";

		[Tooltip("The Rewired Action used for the mouse left button.")]
		public string leftButtonAction = "MouseLeftButton";

		[Tooltip("The Rewired Action used for the mouse right button.")]
		public string rightButtonAction = "MouseRightButton";

		[Tooltip("The Rewired Action used for the mouse middle button.")]
		public string middleButtonAction = "MouseMiddleButton";

		[Tooltip("The distance from the camera that the pointer will be drawn.")]
		public float distanceFromCamera = 1f;

		[Tooltip("The scale of the sprite pointer.")]
		public float spriteScale = 0.05f;

		[Tooltip("The pointer prefab.")]
		public GameObject pointerPrefab;

		[Tooltip("The click effect prefab.")]
		public GameObject clickEffectPrefab;

		[Tooltip("Should the hardware pointer be hidden?")]
		public bool hideHardwarePointer = true;

		[NonSerialized]
		private GameObject pointer;

		[NonSerialized]
		private PlayerMouse mouse;

		private void Awake()
		{
			pointer = UnityEngine.Object.Instantiate(pointerPrefab);
			pointer.transform.localScale = new Vector3(spriteScale, spriteScale, spriteScale);
			if (hideHardwarePointer)
			{
				Cursor.visible = false;
			}
			mouse = PlayerMouse.Factory.Create();
			mouse.set_playerId(playerId);
			mouse.get_xAxis().actionName = horizontalAction;
			mouse.get_yAxis().actionName = verticalAction;
			mouse.get_wheel().yAxis.actionName = wheelAction;
			mouse.get_leftButton().actionName = leftButtonAction;
			mouse.get_rightButton().actionName = rightButtonAction;
			mouse.get_middleButton().actionName = middleButtonAction;
			mouse.set_pointerSpeed(1f);
			mouse.get_wheel().yAxis.repeatRate = 5f;
			mouse.set_screenPosition(new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f));
			mouse.add_ScreenPositionChangedEvent(OnScreenPositionChanged);
			OnScreenPositionChanged(mouse.get_screenPosition());
		}

		private void Update()
		{
			if (ReInput.isReady)
			{
				pointer.transform.Rotate(Vector3.forward, mouse.get_wheel().yAxis.value * 20f);
				if (mouse.get_leftButton().justPressed)
				{
					CreateClickEffect(new Color(0f, 1f, 0f, 1f));
				}
				if (mouse.get_rightButton().justPressed)
				{
					CreateClickEffect(new Color(1f, 0f, 0f, 1f));
				}
				if (mouse.get_middleButton().justPressed)
				{
					CreateClickEffect(new Color(1f, 1f, 0f, 1f));
				}
			}
		}

		private void OnDestroy()
		{
			if (ReInput.isReady)
			{
				mouse.remove_ScreenPositionChangedEvent(OnScreenPositionChanged);
			}
		}

		private void CreateClickEffect(Color color)
		{
			GameObject obj = UnityEngine.Object.Instantiate(clickEffectPrefab);
			obj.transform.localScale = new Vector3(spriteScale, spriteScale, spriteScale);
			obj.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mouse.get_screenPosition().x, mouse.get_screenPosition().y, distanceFromCamera));
			obj.GetComponentInChildren<SpriteRenderer>().color = color;
			UnityEngine.Object.Destroy(obj, 0.5f);
		}

		private void OnScreenPositionChanged(Vector2 position)
		{
			Vector3 position2 = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, distanceFromCamera));
			pointer.transform.position = position2;
		}
	}
}
#endif
