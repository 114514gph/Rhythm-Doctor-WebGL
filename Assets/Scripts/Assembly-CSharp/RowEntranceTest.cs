using System;
using DG.Tweening;
using UnityEngine;

namespace RowEntranceTest
{
	public class RowEntranceTest : MonoBehaviour
	{
		[Serializable]
		public struct TweenInfo
		{
			public float from;

			public float to;

			public float extraDuration;

			public Ease ease;

			public float random;
		}

		public RDWaveRenderer_Pulse lineEntrance;

		public Transform heart;

		private Sequence sequence;

		public float duration;

		public TweenInfo heartScale;

		public TweenInfo length;

		public TweenInfo width;

		public TweenInfo height;

		public TweenInfo frequency;

		public TweenInfo offset;

		public TweenInfo triangleLock;

		public bool loop;

		public bool doRandomOffset;

		private void Awake()
		{
		}

		private void Start()
		{
			Play();
		}

		private void OnValidate()
		{
			Play();
		}

		public void Play()
		{
			sequence?.Kill();
			float num = duration;
			RDWaveRenderer.GetShaderPropertyIDs();
			sequence = (Sequence)DOTween.Sequence().AppendCallback(() =>
			{
				lineEntrance.gameObject.SetActive(value: true);
				lineEntrance.visible = true;
			}).Append(lineEntrance.transform.DOScaleX(282f, num).From(0f).SetEase(width.ease))
				.Join(lineEntrance.transform.DOLocalMoveX(141f, num).From(0f).SetEase(width.ease))
				.Join(lineEntrance.material.DOFloat(282f, RDWaveRenderer.canvasWidthID, num).From(0f).SetEase(width.ease))
				.Join(heart.transform.DOLocalMoveX(0f, num).From(-282f).SetEase(width.ease))
				.Join(heart.transform.DOScale(1f, num * (1f + heartScale.extraDuration)).From(heartScale.from).SetEase(heartScale.ease))
				.Join(lineEntrance.material.DOVector(Vector4X(282f * length.to), RDWaveRenderer.sineWidthID, num * (1f + length.extraDuration)).From(Vector4X(length.from)).SetEase(length.ease))
				.Join(lineEntrance.material.DOVector(Vector4X(frequency.to), RDWaveRenderer.triangleFrequencyID, num * (1f + frequency.extraDuration)).From(Vector4X(frequency.from * (1f + UnityEngine.Random.Range(0f - frequency.random, frequency.random)))))
				.Join(lineEntrance.material.DOVector(Vector4X(height.to), RDWaveRenderer.sineHeightID, num * (1f + height.extraDuration)).From(Vector4X(height.from)).SetEase(height.ease))
				.Join(lineEntrance.material.DOVector(Vector4X(offset.to), RDWaveRenderer.triangleOffsetID, num * (1f + offset.extraDuration)).From(Vector4X(offset.from * (1f + UnityEngine.Random.Range(0f - offset.random, offset.random)))))
				.Join(lineEntrance.material.DOVector(Vector4X(triangleLock.to), RDWaveRenderer.triangleOffsetLockID, num * (1f + triangleLock.extraDuration)).From(Vector4X(triangleLock.from * (1f + UnityEngine.Random.Range(0f - triangleLock.random, triangleLock.random)))))
				.Done()
				.SetLoops(loop ? (-1) : 0);
			static Vector4 Vector4X(float x)
			{
				return new Vector4(x, 0f, 0f, 0f);
			}
		}
	}
}
