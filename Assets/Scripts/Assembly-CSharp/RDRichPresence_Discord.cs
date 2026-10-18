using System;
using Discord;
using RDTools;
using UnityEngine;

public class RDRichPresence_Discord : RDRichPresenceHandler
{
	private global::Discord.Discord discord;

	private string currentUsername;

	private long currentUserID = -1L;

	private float nextReconnectTime;

	private const float reconnectInterval = 5f;

	public override string UserName => currentUsername;

	public override long UserId => currentUserID;

	public override DiscordPresence Mode => Persistence.GetDiscordPresence();

	public override void Setup()
	{
		TryInitDiscord();
	}

	private void TryInitDiscord()
	{
		try
		{
			discord = new global::Discord.Discord(477926053420072961L, 1uL);
		}
		catch (Exception arg)
		{
			RDBaseDll.printes($"Discord is not initialized: {arg}");
			Dispose();
		}
		if (discord != null)
		{
			try
			{
				discord.GetActivityManager().RegisterSteam(977950u);
			}
			catch (ResultException ex)
			{
				RDBaseDll.printes("Discord: failed to register steam launch method: " + ex.Message);
			}
			discord.GetUserManager().OnCurrentUserUpdate += () =>
			{
				User currentUser = discord.GetUserManager().GetCurrentUser();
				currentUsername = currentUser.Username;
				currentUserID = currentUser.Id;
			};
		}
	}

	private void ScheduleReconnect()
	{
		nextReconnectTime = Time.unscaledTime + 5f;
	}

	public override void Dispose()
	{
		discord?.Dispose();
		discord = null;
		ScheduleReconnect();
	}

	public override void Update()
	{
		if (discord == null)
		{
			if (Time.unscaledTime >= nextReconnectTime)
			{
				TryInitDiscord();
			}
			return;
		}
		try
		{
			discord.RunCallbacks();
		}
		catch (Exception)
		{
			Dispose();
		}
	}

	public override void ClearPresence()
	{
		discord?.GetActivityManager().ClearActivity((Result result) =>
		{
		});
	}

	public override void SetPresence(string state, string details)
	{
		if (discord != null)
		{
			if (!details.IsNullOrEmpty())
			{
				details = RDString.Get("discord." + details);
			}
			state = state.TruncateAnyEncoding(127);
			details = details.TruncateAnyEncoding(127);
			Activity activity = new Activity
			{
				State = state,
				Details = details,
				Assets = 
				{
					LargeImage = "rhythm_doctor_icon_for_fb_png",
					LargeText = "Samurai."
				}
			};
			discord?.GetActivityManager().UpdateActivity(activity, (Result result) =>
			{
			});
		}
	}

	public override string GetRWState()
	{
		return RDString.Get("rhythmWeightlifter.score").Replace("[score]", Persistence.GetLevelScore(Level.RhythmWeightlifter.ToString()).ToString());
	}

	public override string GetGameState(string cachedLevelName)
	{
		if (!string.IsNullOrEmpty(cachedLevelName))
		{
			return cachedLevelName;
		}
		return string.Empty;
	}

	public override string GetEditorState(string cachedSong)
	{
		if (!string.IsNullOrEmpty(cachedSong))
		{
			return RDString.Get("discord.editedLevel").Replace("[level]", cachedSong);
		}
		return string.Empty;
	}
}
