namespace UnityEngine.UI
{
	internal struct CharMovementInfo
	{
		public int charIndex;

		public LetterMovementType movementType;

		public float[] multipliers;

		public CharMovementInfo(int index, LetterMovementType movType, float[] mults)
		{
			charIndex = index;
			movementType = movType;
			multipliers = mults;
		}
	}
}
