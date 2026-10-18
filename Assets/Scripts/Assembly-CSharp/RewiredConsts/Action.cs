using Rewired.Dev;

namespace RewiredConsts
{
	public static class Action
	{
		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "gameplayInput")]
		public const int gameplayInput = 54;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "gameplayCancel")]
		public const int gameplayCancel = 60;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "faceLeft")]
		public const int faceLeft = 61;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "faceUp")]
		public const int faceUp = 62;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "menuSelect")]
		public const int menuSelect = 53;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "menuCancel")]
		public const int menuCancel = 52;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "finerControl")]
		public const int finerControl = 55;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "up")]
		public const int up = 56;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "down")]
		public const int down = 57;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "left")]
		public const int left = 58;

		[ActionIdFieldInfo(categoryName = "Default", friendlyName = "right")]
		public const int right = 59;
	}
}
