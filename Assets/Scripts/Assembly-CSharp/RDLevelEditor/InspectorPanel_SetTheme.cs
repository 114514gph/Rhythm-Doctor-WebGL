namespace RDLevelEditor
{
	public class InspectorPanel_SetTheme : InspectorPanel
	{
		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetTheme levelEvent_SetTheme = (LevelEvent_SetTheme)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "variant" && levelEvent_SetTheme.EnableVariantIf())
				{
					((PropertyControl_ToggleGroup)property.control).SetThemeOptions(RDEditorConstants.ThemeVariants[levelEvent_SetTheme.preset], $"enum.RDTheme.{levelEvent_SetTheme.preset}");
				}
				if (property.name == "positionX" && levelEvent_SetTheme.EnablePositionFieldIf())
				{
					UpdateScrollBounds(property, levelEvent_SetTheme.preset);
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetTheme levelEvent_SetTheme = (LevelEvent_SetTheme)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "variant" && levelEvent_SetTheme.EnableVariantIf())
				{
					((PropertyControl_ToggleGroup)property.control).SetThemeOptions(RDEditorConstants.ThemeVariants[levelEvent_SetTheme.preset], $"enum.RDTheme.{levelEvent_SetTheme.preset}");
				}
				if (property.name == "positionX" && levelEvent_SetTheme.EnablePositionFieldIf())
				{
					UpdateScrollBounds(property, levelEvent_SetTheme.preset);
				}
				property.Save(levelEvent);
			}
		}

		public void UpdateScrollBounds(Property property, RDTheme preset)
		{
			if (RDEditorConstants.EditorThemeBounds.TryGetValue(preset, out var value))
			{
				((PropertyControl_Slider)property.control).UpdateBounds(value.x, value.y);
			}
		}
	}
}
