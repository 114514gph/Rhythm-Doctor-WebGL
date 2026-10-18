using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ClickEvent : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public UnityEvent PointerClick;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (PointerClick != null)
		{
			PointerClick.Invoke();
		}
	}
}
