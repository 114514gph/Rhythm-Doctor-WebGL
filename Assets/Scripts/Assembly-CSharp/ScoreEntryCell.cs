using UnityEngine;
using UnityEngine.UI;

namespace RhythmWeightlifter
{
	public class ScoreEntryCell : RDBase
	{
		public Image background;

		public Text rank;

		public Image icon;

		public Text initials;

		public Text score;

		public Image foreground;

		private bool isPlayer;

		private float blinkingTime;

		public void Setup(bool isPlayer, int rank, ScoreEntry scoreEntry)
		{
			this.rank.text = $"{rank}.";
			icon.sprite = Resources.Load<Sprite>($"RWL_icon_{scoreEntry.character}");
			initials.text = RDString.Get(scoreEntry.initialsKey);
			score.text = scoreEntry.score.ToString();
			this.isPlayer = isPlayer;
			if (isPlayer)
			{
				background.enabled = true;
				background.rectTransform.SizeDeltaY(0f);
				foreground.enabled = true;
			}
		}

		public void PlayBlink()
		{
			blinkingTime = 4.5f;
		}

		private void LateUpdate()
		{
			if (isPlayer && blinkingTime > 0f)
			{
				Text text = rank;
				Text text2 = initials;
				Color color = (score.color = ((Time.unscaledTime % 0.25f < 0.125f) ? Color.black : "7C7AF7".HexToColor()));
				Color color3 = (text2.color = color);
				text.color = color3;
				blinkingTime -= Time.unscaledDeltaTime;
			}
		}
	}
}
