using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmWeightlifter
{
	[Serializable]
	public class Weight
	{
		private const float intervalWaitMultiplierThreshold = 1.5f;

		public Transform point;

		public Transform sprite;

		public GameObject[] weights;

		public Transform weightsContainer;

		public float showXPos;

		public float hideXPos;

		public Text text;

		public float collarInnerDefaultXPos;

		public Transform collarInner;

		public Transform collarOuter;

		[NonSerialized]
		public List<float> hitTimes;

		[NonSerialized]
		public float hitTimer;

		[NonSerialized]
		public bool showing;

		private float lastY;

		[NonSerialized]
		private Barbell barbell;

		private Weight otherWeight;

		private float _y;

		private int _currentWeight;

		private bool _inconsistency;

		private const float WeightAnimDuration = 0.25f;

		private const float globalSdMin = 0.5f;

		private const float globalSdMax = 0.1f;

		private const float TextsFadeAnimDuration = 0.3f;

		private static scnRhythmWeightlifter gameInstance => scnRhythmWeightlifter.gameInstance;

		private bool IsP1Weight => this == barbell.p1Weight;

		public float y
		{
			get
			{
				return _y;
			}
			set
			{
				lastY = y;
				_y = Mathf.Clamp(value, barbell.minY, barbell.maxY + 1f);
				Position = Position.WithY(_y);
				if (text.gameObject.activeSelf)
				{
					gameInstance.CheckGameplayTextsFade();
				}
			}
		}

		public Vector3 Position
		{
			get
			{
				return point.localPosition;
			}
			set
			{
				point.localPosition = value;
				sprite.localPosition = value;
			}
		}

		public int CurrentWeight
		{
			get
			{
				return _currentWeight;
			}
			set
			{
				if (_currentWeight != value)
				{
					_currentWeight = value;
					for (int i = 0; i < weights.Length; i++)
					{
						weights[i].SetActive(i == _currentWeight - 1);
					}
				}
			}
		}

		public bool Inconsistency
		{
			get
			{
				return _inconsistency;
			}
			set
			{
				if (_inconsistency != value)
				{
					_inconsistency = value;
				}
			}
		}

		public bool ShouldUpdate => lastY != y;

		public void Setup(Barbell barbell, bool p1)
		{
			this.barbell = barbell;
			otherWeight = (p1 ? barbell.p2Weight : barbell.p1Weight);
			hitTimes = new List<float>();
		}

		public void Reset(float minY)
		{
			y = minY;
			Inconsistency = false;
			hitTimes = new List<float>();
		}

		public void Update(bool levelWin)
		{
			if (y > barbell.minY)
			{
				float num = (float)barbell.additiveWeight * 10f;
				float num2 = ((num < 60f) ? (-0.025f * num + 2.5f) : 1f);
				float num3 = (levelWin ? 25f : 5000f);
				float num4 = (float)barbell.additiveWeight * 255f / num3 * 0.8f * Time.deltaTime / 16f;
				num4 = ((Inconsistency && !levelWin) ? (12f * num2) : 1f) * (num4 / 305f) * barbell.yDistance * 1000f;
				y -= num4;
				if (y <= barbell.minY)
				{
					Inconsistency = false;
					hitTimes = new List<float>();
					(IsP1Weight ? gameInstance.dropP1 : gameInstance.dropP2).Play();
					if (IsP1Weight)
					{
						barbell.p1AltSound = false;
					}
					else
					{
						barbell.p2AltSound = false;
					}
				}
			}
			if (hitTimes.Count > 1 && !levelWin)
			{
				if (hitTimer > 0f)
				{
					hitTimer -= Time.deltaTime;
				}
				else if (!Inconsistency)
				{
					(IsP1Weight ? gameInstance.wobbleHigh : gameInstance.wobbleLow).Play();
					Inconsistency = true;
				}
			}
			int index = ((this != barbell.p1Weight) ? 1 : 0);
			barbell.weightRod.SetPosition(index, Position);
			UpdateSprites();
		}

		public void Lift()
		{
			float num = 0.16393442f * barbell.yDistance;
			if (gameInstance.isInfiniteMode)
			{
				num *= 220f / gameInstance.infiniteModeEffectiveBpm;
			}
			float num2 = num / (float)CurrentWeight;
			y += num2;
			hitTimes.Insert(0, Time.time);
			if (hitTimes.Count > 1)
			{
				hitTimer = (hitTimes[0] - hitTimes[1]) * 1.5f;
			}
		}

		public void UpdateSprites()
		{
			Vector3 vector = Position - otherWeight.Position;
			float angle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			sprite.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}

		public void Toggle(bool show)
		{
			Transform collarInnerParent = (show ? weightsContainer : sprite);
			if (!show)
			{
				showing = false;
				collarInner.DOKill();
				collarInner.SetParent(collarInnerParent);
				collarInner.DOLocalMoveX(collarInnerDefaultXPos, 0.25f).SetEase(Ease.OutExpo);
			}
			float targetXPos = (show ? showXPos : hideXPos);
			weightsContainer.DOKill();
			Ease ease = (show ? Ease.OutExpo : Ease.OutCirc);
			weightsContainer.DOLocalMoveX(targetXPos, 0.25f).SetEase(ease).OnComplete(() =>
			{
				if (show)
				{
					showing = true;
					collarInner.DOKill();
					collarInner.SetParent(collarInnerParent);
					weightsContainer.DOLocalMoveX(targetXPos - 0.5f, 0.1f).SetEase(Ease.OutCirc);
					weightsContainer.DOLocalMoveX(targetXPos + 0.4f, 0.15f).SetEase(Ease.OutCirc).SetDelay(0.1f);
					weightsContainer.DOLocalMoveX(targetXPos, 0.2f).SetEase(Ease.InOutCirc).SetDelay(0.25f);
				}
			});
		}

		public void AnimateOuterCollar()
		{
			float endValue = (IsP1Weight ? barbell.collarOuterXPos[CurrentWeight - 1] : (0f - barbell.collarOuterXPos[CurrentWeight - 1]));
			collarOuter.DOMoveX(endValue, 0.7f).SetEase(Ease.OutBounce).OnComplete(() =>
			{
				collarOuter.SetParent(weightsContainer);
			});
		}

		public void UpdateOuterCollarPosition()
		{
			float x = (IsP1Weight ? barbell.collarOuterXPos[CurrentWeight - 1] : (0f - barbell.collarOuterXPos[CurrentWeight - 1]));
			collarOuter.position = collarOuter.position.WithX(x);
		}

		public int CalculateConsistency()
		{
			float num = 0f;
			if (hitTimes.Count > 0 && otherWeight.hitTimes.Count > 0)
			{
				num = Mathf.Min(hitTimes[hitTimes.Count - 1], otherWeight.hitTimes[otherWeight.hitTimes.Count - 1]);
			}
			else
			{
				Debug.Log($"cheat code used: {hitTimes.Count}, {otherWeight.hitTimes.Count}");
			}
			float num2 = Time.time - num;
			List<float> list = new List<float>();
			List<float> list2 = new List<float>();
			for (int i = 0; i < hitTimes.Count - 1; i++)
			{
				list.Add(hitTimes[i + 1] - hitTimes[i]);
			}
			for (int j = 0; j < list.Count - 1; j++)
			{
				list2.Add(list[j + 1] / list[j] - 1f);
			}
			float average = 0f;
			list2.ForEach((float x) =>
			{
				average += x;
			});
			average /= list2.Count;
			float count = 0f;
			float runningDifference = 0f;
			list2.ForEach((float x) =>
			{
				runningDifference += Mathf.Pow(x - average, 2f);
				count++;
			});
			float num3 = Mathf.Sqrt(runningDifference / count);
			float num4 = (float)hitTimes.Count / num2;
			float num5 = Mathf.Max(0.5f, 0.5f * num4 / 4f);
			float num6 = Mathf.Max(0.1f, 0.1f * num4 / 4f);
			return Mathf.RoundToInt((num3 - num5) / (num6 - num5) * 50f);
		}

		public void UpdateText()
		{
			text.text = RDString.Get("rhythmWeightlifter.kgNumber").Replace("[weightNumber]", (CurrentWeight * 10).ToString());
			text.font = RDString.GetAppropiateFontForString(text.text);
			text.DOKill();
			text.DOFade(1f, 0.3f);
		}
	}
}
