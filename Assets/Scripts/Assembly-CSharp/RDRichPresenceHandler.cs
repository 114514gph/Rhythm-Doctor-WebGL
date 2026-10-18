public abstract class RDRichPresenceHandler
{
	public abstract string UserName { get; }

	public abstract long UserId { get; }

	public abstract DiscordPresence Mode { get; }

	public abstract void Setup();

	public abstract void Dispose();

	public abstract void Update();

	public abstract void ClearPresence();

	public abstract void SetPresence(string state, string details);

	public abstract string GetRWState();

	public abstract string GetGameState(string cachedSong);

	public abstract string GetEditorState(string cachedSong);
}
