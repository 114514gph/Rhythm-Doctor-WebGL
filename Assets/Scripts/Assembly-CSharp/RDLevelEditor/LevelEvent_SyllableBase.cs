namespace RDLevelEditor
{
	public abstract class LevelEvent_SyllableBase : LevelEvent_Base, IDurationHaver
	{
		[JsonProperty("", "", null, "", false, true, "EnableFadeOutDurationIf", null)]
		public float? fadeOutDuration { get; set; }

		public float duration
		{
			get
			{
				if (!(base.editor != null))
				{
					return float.NaN;
				}
				return fadeOutDuration ?? fallbackFadeOutDuration;
			}
			set
			{
				fadeOutDuration = value;
			}
		}

		protected abstract float fallbackFadeOutDuration { get; }

		public bool EnableFadeOutDurationIf()
		{
			return fadeOutDuration.HasValue;
		}
	}
}
