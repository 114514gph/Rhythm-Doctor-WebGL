using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmWeightlifter
{
	public class Ending : MonoBehaviour
	{
		public RectTransform endingContainer;

		public RectTransform titleContainer;

		public Text titleText;

		public RectTransform resultsPanel;

		public ScoreEntryCell[] scoreEntryCells;

		public GameObject endingDownArrow;

		private Sequence highScoreSequence;

		public Image resultArt;

		public Text resultDescription;

		public int playerHighScoreRank;

		private const float titleYStart = 30f;

		private const float minTitleY = -73f;

		private const float titleAndResultsYDistance = 30f;

		private const float resultsPanelYStart = -173f;

		private const float resultsPanelYEnd = -44f;

		public void ShowScoreboard(int playerScore)
		{
			List<ScoreEntry> source = new List<ScoreEntry>(scnRhythmWeightlifter.highScores)
			{
				new ScoreEntry
				{
					character = Character.Athlete,
					score = playerScore,
					initialsKey = "rhythmWeightlifter.initialsAthlete"
				}
			};
			source = source.OrderByDescending((ScoreEntry x) => x.score).ToList();
			int num = 0;
			int num2 = 0;
			ScoreEntryCell[] array = scoreEntryCells;
			foreach (ScoreEntryCell obj in array)
			{
				ScoreEntry scoreEntry = source[num];
				int rank = num + 1;
				bool flag = scoreEntry.character == Character.Athlete;
				obj.Setup(flag, rank, scoreEntry);
				if (flag)
				{
					num2 = num;
					playerHighScoreRank = rank;
				}
				num++;
			}
			endingContainer.gameObject.SetActive(value: true);
			titleContainer.gameObject.SetActive(value: true);
			resultsPanel.gameObject.SetActive(value: true);
			endingDownArrow.gameObject.SetActive(value: false);
			resultArt.gameObject.SetActive(value: false);
			resultDescription.gameObject.SetActive(value: false);
			titleContainer.AnchorPosY(30f);
			resultsPanel.AnchorPosY(-173f);
			ScoreEntryCell playerCell = scoreEntryCells[num2];
			RectTransform rectTransform = playerCell.background.rectTransform;
			string scoreboardNarration = RDString.Get("narration.rhythmWeightlifter." + ((playerHighScoreRank == 0) ? "scoreboardWin" : "scoreboardLose")).Replace("[score]", playerScore.ToString()).Replace("[ianScore]", source[0].score.ToString())
				.Replace("[place]", playerHighScoreRank.ToString());
			highScoreSequence = (Sequence)DOTween.Sequence().PrependInterval(0.5f).AppendCallback(() =>
			{
				RWPlaySound("sndChampionshipTitleFall");
			})
				.Append(titleContainer.DOAnchorPosY(-73f, 0.4f).SetEase(Ease.InQuad))
				.AppendCallback(() =>
				{
					Narration.Say(titleText.text, NarrationCategory.Notification, false);
					RWPlaySound("sndChampionshipTitleLand");
				})
				.Append(titleContainer.DOAnchorPosY(-63f, 0.2f).SetEase(Ease.OutQuad))
				.Append(titleContainer.DOAnchorPosY(-73f, 0.2f).SetEase(Ease.InQuad))
				.AppendCallback(() =>
				{
					RWPlaySound("sndChampionshipTitleLand", 0.25f, 0.98f);
				})
				.Append(titleContainer.DOAnchorPosY(-71f, 0.1f).SetEase(Ease.OutQuad))
				.Append(titleContainer.DOAnchorPosY(-73f, 0.1f).SetEase(Ease.InQuad))
				.AppendCallback(() =>
				{
					RWPlaySound("sndChampionshipTitleLand", 0.2f, 0.98f);
				})
				.AppendInterval(1.5f)
				.AppendCallback(() =>
				{
					RWPlaySound("sndChampionshipResultsEnter");
				})
				.Append(resultsPanel.DOAnchorPosY(-44f, 1f).SetEase(Ease.OutQuart))
				.OnUpdate(() =>
				{
					float num4 = resultsPanel.anchoredPosition.y + 30f;
					if (titleContainer.anchoredPosition.y < num4)
					{
						titleContainer.AnchorPosY(num4);
					}
				})
				.AppendCallback(() =>
				{
					Narration.Say(RDString.Get("narration.rhythmWeightlifter.scoreboard"), NarrationCategory.Description, false);
				})
				.AppendInterval(1f)
				.AppendCallback(() =>
				{
					RWPlaySound("sndChampionshipEntryExpand");
				})
				.Append(rectTransform.DOSizeDelta(rectTransform.sizeDelta.WithY(13f), 0.3f).SetEase(Ease.OutBack))
				.AppendCallback(() =>
				{
					playerCell.PlayBlink();
					RWPlaySound("sndChampionshipEntryBlink");
				})
				.Join(playerCell.foreground.DOFade(0f, 1.5f).SetEase(Ease.InQuad))
				.AppendCallback(() =>
				{
					Narration.Say(scoreboardNarration, NarrationCategory.Description, false);
				})
				.AppendInterval(3.5f)
				.AppendCallback(() =>
				{
					ShowDownArrow();
				});
		}

		public void ShowReward()
		{
			string keySuffix = playerHighScoreRank switch
			{
				1 => "Best", 
				2 => "Good", 
				3 => "Good", 
				_ => "Bad", 
			};
			string resultDescriptionString = RDString.Get("rhythmWeightlifter.endingResult" + keySuffix);
			resultDescription.font = RDString.GetAppropiateFontForString(resultDescriptionString);
			string[] lines = resultDescriptionString.Split('\n');
			resultDescription.gameObject.SetActive(value: true);
			resultDescription.text = "";
			resultArt.sprite = Resources.Load<Sprite>("RWL_ending_" + keySuffix);
			DOTween.Sequence(titleContainer.DOAnchorPosX(-250f, 0.7f).SetEase(Ease.InCubic).OnStart(() =>
			{
				RWPlaySound("sndChampionshipResultsExit");
			})).Join(resultsPanel.DOAnchorPosX(250f, 0.7f).SetEase(Ease.InCubic)).AppendCallback(() =>
			{
				endingDownArrow.SetActive(value: false);
			})
				.AppendInterval(2f)
				.AppendCallback(() =>
				{
					resultArt.gameObject.SetActive(value: true);
					RWPlaySound("sndChampionshipEnding" + keySuffix);
					Narration.Say(RDString.Get("narration.rhythmWeightlifter.ending" + keySuffix), NarrationCategory.Description, false);
				})
				.AppendCallback(() =>
				{
					Narration.Say(RDString.Get("narration.rhythmWeightlifter.endingTextPrefix"), NarrationCategory.Description, false);
				})
				.AppendInterval(2f)
				.AppendCallback(() =>
				{
					resultDescription.text = lines[0];
					RWPlaySound("sndChampionshipDialogue_0");
					Narration.Say(lines[0], NarrationCategory.Notification, false);
				})
				.AppendInterval(2f)
				.AppendCallback(() =>
				{
					resultDescription.text = lines[0] + "\n" + lines[1];
					RWPlaySound("sndChampionshipDialogue_1");
					Narration.Say(lines[1], NarrationCategory.Notification, false);
				})
				.AppendInterval(2f)
				.AppendCallback(() =>
				{
					resultDescription.text = resultDescriptionString;
					RWPlaySound("sndChampionshipDialogue_2");
					Narration.Say(lines[2], NarrationCategory.Notification, false);
				})
				.AppendCallback(() =>
				{
					ShowDownArrow();
				});
		}

		private void ShowDownArrow()
		{
			if (!endingDownArrow.activeSelf)
			{
				endingDownArrow.SetActive(value: true);
				if (Narration.IsEnabled)
				{
					scrConductor.PlayImmediately("sndPressToContinue", 1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"));
				}
			}
		}

		private AudioSource RWPlaySound(string sound, float volume = 1f, float pitch = 1f)
		{
			return scrConductor.PlayImmediately(sound, volume, RDUtils.GetMixerGroup("IanDesktop"), pitch);
		}
	}
}
