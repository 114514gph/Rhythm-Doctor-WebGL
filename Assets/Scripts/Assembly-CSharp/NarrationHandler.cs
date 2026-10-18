public abstract class NarrationHandler
{
	public abstract bool IsEnabled { get; }

	public abstract bool IsSpeaking { get; }

	protected abstract void InternalSay(string textToSay);

	protected abstract void InternalStop();

	protected abstract void InternalEnableAccessability(bool enable);

	protected abstract void InternalToggleSAPI(bool enable);

	protected abstract void InternalSetSpeed(int speed);

	protected abstract void InternalSetVolume(int volume);

	public void Say(string textToSay)
	{
		if (Narration.IsAvailable)
		{
			InternalSay(textToSay.RemoveRichTextColorTags());
		}
	}

	public void Stop()
	{
		if (Narration.IsAvailable)
		{
			InternalStop();
		}
	}

	public void EnableAccessability(bool enable)
	{
		if (Narration.IsAvailable)
		{
			InternalEnableAccessability(enable);
		}
	}

	public void ToggleSAPI(bool enable)
	{
		if (Narration.IsAvailable)
		{
			InternalToggleSAPI(enable);
		}
	}

	public void SetSpeed(int speed)
	{
		if (Narration.IsAvailable)
		{
			InternalSetSpeed(speed);
		}
	}

	public void SetVolume(int volume)
	{
		if (Narration.IsAvailable)
		{
			InternalSetVolume(volume);
		}
	}
}
