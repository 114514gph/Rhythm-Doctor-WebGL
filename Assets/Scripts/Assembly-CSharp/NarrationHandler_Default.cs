public class NarrationHandler_Default : NarrationHandler
{
	private bool startup;

	public override bool IsEnabled => UAP_AccessibilityManager.IsEnabled();

	public override bool IsSpeaking => UAP_AccessibilityManager.IsSpeaking();

	protected override void InternalSay(string textToSay)
	{
		UAP_AccessibilityManager.Say(textToSay);
	}

	protected override void InternalStop()
	{
		UAP_AccessibilityManager.StopSpeaking();
	}

	protected override void InternalEnableAccessability(bool enable)
	{
		UAP_AccessibilityManager.EnableAccessibility(enable, enable);
		if (scnBase.isDesktop && !scnBase.isMobile)
		{
			UAP_AccessibilityManager.PauseAccessibility(pause: true);
		}
	}

	protected override void InternalToggleSAPI(bool enable)
	{
		if (!startup)
		{
			// WindowsTTS.ToggleSAPI(enable);
			UAP_AccessibilityManager.EnableMagicGestures(!scnBase.isDesktop);
			startup = true;
		}
	}

	protected override void InternalSetSpeed(int speed)
	{
		UAP_AccessibilityManager.SetSpeechRate(speed);
	}

	protected override void InternalSetVolume(int volume)
	{
		UAP_AccessibilityManager.SetSpeechVolume(volume);
	}
}
