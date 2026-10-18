using UnityEngine;

public class NarrationTest : scnBase
{
	public static UAP_AudioQueue audioQueue;

	[Header("Settings")]
	public int speechRate;

	[Header("Say on change")]
	public int sayInt;

	private int lastSayInt;

	public float sayFloat;

	private float lastSayFloat;

	public string sayString;

	private string lastSayString;

	public string sayStringInterruptable;

	private string lastSayStringInterruptable;

	public ElementType elementType;

	private ElementType lastElementType;

	public int elementItemsNumber;

	private int lastElementItemsNumber;

	private new void Start()
	{
		UAP_AccessibilityManager.EnableAccessibility(enable: true, readNotification: true);
		UAP_AccessibilityManager.PauseAccessibility(pause: true);
		lastSayInt = sayInt;
		lastSayFloat = sayFloat;
		lastSayString = sayString;
		lastElementType = elementType;
		lastElementItemsNumber = elementItemsNumber;
	}

	private new void Update()
	{
		_ = lastSayInt;
		_ = sayInt;
		lastSayInt = sayInt;
		_ = lastSayFloat;
		_ = sayFloat;
		lastSayFloat = sayFloat;
		if (lastSayString != sayString)
		{
			Narration.Say(sayString, NarrationCategory.Notification, false);
		}
		lastSayString = sayString;
		if (lastElementType != elementType || elementItemsNumber != lastElementItemsNumber)
		{
			Narration.Say(sayString, NarrationCategory.Notification, -1, -1, elementType, elementItemsNumber.ToString(), null, false);
		}
		lastElementType = elementType;
		lastElementItemsNumber = elementItemsNumber;
	}

	public void Uninterruptable()
	{
		UAP_AccessibilityManager.Say(sayString, canBeInterrupted: false, allowVoiceOver: true, UAP_AudioQueue.EInterrupt.All);
	}

	public void Interruptable()
	{
		UAP_AccessibilityManager.Say(sayStringInterruptable, canBeInterrupted: true, allowVoiceOver: true, UAP_AudioQueue.EInterrupt.All);
	}
}
