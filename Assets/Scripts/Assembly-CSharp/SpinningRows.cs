using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpinningRows : RDBase
{
	public class SpinningRowData
	{
		private float rotation;

		private float wavyRotationTime;

		private Tween rotationTween;

		private Tween wavyAmplitudeTween;

		private Tween wavyFrequencyTween;

		private float constantRotationPerSecond;

		private float wavyAmplitude;

		private float wavyFrequency;

		public bool active = true;

		public void Tick(RowEntity row, RowEntity mainRow)
		{
			if (active)
			{
				rotation += Time.deltaTime * constantRotationPerSecond;
				wavyRotationTime += Time.deltaTime * wavyFrequency;
				float num = Mathf.Sin(MathF.PI * wavyRotationTime) * wavyAmplitude;
				float num2 = rotation + num;
				row.rowContainer.eulerAngles = Vector3.forward * num2;
				row.heartContainer.eulerAngles = Vector3.zero;
				bool flag = Mathf.Repeat(num2 - 90f, 360f) < 180f;
				row.rowContainer.localScale = row.rowContainer.localScale.WithY(Mathf.Abs(row.rowContainer.localScale.y) * (float)((!flag) ? 1 : (-1)));
				row.heartContainer.localScale = row.heartContainer.localScale.WithY(Mathf.Abs(row.heartContainer.localScale.y) * (float)((!flag) ? 1 : (-1)));
				if (!(row == mainRow))
				{
					row.transform.position = mainRow.transform.position;
				}
			}
		}

		public void Rotate(float toRotation, float duration, Ease ease)
		{
			constantRotationPerSecond = 0f;
			rotationTween?.Kill();
			rotationTween = DOTween.To(() => rotation, (float x) =>
			{
				rotation = x;
			}, toRotation, duration).SetEase(ease);
		}

		public void SetConstantRotation(float toRotation, float duration, Ease ease)
		{
			rotationTween.Kill();
			rotationTween = DOTween.To(() => constantRotationPerSecond, (float x) =>
			{
				constantRotationPerSecond = x;
			}, toRotation, duration).SetEase(ease);
		}

		public void SetWavyAmpltiude(float toAmplitude, float duration, Ease ease)
		{
			wavyAmplitudeTween?.Kill();
			wavyAmplitudeTween = DOTween.To(() => wavyAmplitude, (float x) =>
			{
				wavyAmplitude = x;
			}, toAmplitude, duration).SetEase(ease);
		}

		public void SetWavyFrequency(float toFrequency, float duration, Ease ease)
		{
			wavyFrequencyTween?.Kill();
			wavyFrequencyTween = DOTween.To(() => wavyFrequency, (float x) =>
			{
				wavyFrequency = x;
			}, toFrequency, duration).SetEase(ease);
		}
	}

	public RowEntity mainRow;

	public Dictionary<RowEntity, SpinningRowData> spinningRows = new Dictionary<RowEntity, SpinningRowData>();

	public SpinningRows Setup(RowEntity mainRow)
	{
		this.mainRow = mainRow;
		spinningRows.TryAdd(mainRow, new SpinningRowData());
		return this;
	}

	public void Connect(RowEntity row)
	{
		if (row == mainRow)
		{
			mainRow.heart.ChangeSprite("Heart_Joined");
			return;
		}
		spinningRows.TryAdd(row, new SpinningRowData());
		row.appearance = RowVisibilityMode.OnlyRow;
		row.Show(ShowAnimationType.InstantNoEffects, setup: false, keepCharacterVisible: false);
		row.targetCharacterRowEnt = mainRow;
		row.customPosition = true;
		Merge(row, withEffects: false);
	}

	public void Disconnect(RowEntity row)
	{
		row.heart.ChangeSprite("Heart");
		if (!(row == mainRow))
		{
			spinningRows.Remove(row);
			row.appearance = RowVisibilityMode.Visible;
			row.Show(ShowAnimationType.InstantNoEffects, setup: false, keepCharacterVisible: false);
			row.targetCharacterRowEnt = row;
		}
	}

	public void Merge(RowEntity row, bool withEffects = true)
	{
		mainRow.heart.ChangeSprite("Heart_Joined");
		row.heart.SetMisses(row.rowMisses + mainRow.rowMisses);
		row.Hide(keepCharacterVisible: false, withEffects);
	}

	public void Split(RowEntity row)
	{
		mainRow.heart.ChangeSprite("Heart_Right");
		row.heart.ChangeSprite("Heart_Left");
		mainRow.heart.SetMisses(mainRow.rowMisses);
		row.heart.SetMisses(row.rowMisses);
		row.appearance = RowVisibilityMode.OnlyCharacter;
		row.Show(ShowAnimationType.InstantWithEffects, setup: false, keepCharacterVisible: false);
	}

	private void LateUpdate()
	{
		foreach (RowEntity key in spinningRows.Keys)
		{
			spinningRows[key].Tick(key, mainRow);
		}
	}
}
