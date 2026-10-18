using System;
using DG.Tweening;
using UnityEngine;

public class WindowDancer : RDClass
{
	public const float MaxWindowScale = 2f;

	public int roomIndex = -1;

	public WindowDancePresetInfo currentPresetInfo = WindowDancePresetInfo.Default();

	private float timeSpentOnCurrentPreset;

	private float transitionV;

	public Vector2 position;

	private Tween xPosTween;

	private Tween yPosTween;

	private Tween angleTween;

	private float angle;

	private Tween ampXTween;

	private Tween ampYTween;

	private Vector2 amplitude;

	private Tween xPivotTween;

	private Tween yPivotTween;

	private Vector2 pivot = new Vector2(0.5f, 0.5f);

	private Vector2 pivotEdgeV = Vector2.zero;

	private Vector2 cachedPivot;

	private Tween xScaleTween;

	private Tween yScaleTween;

	private Tween xCamTween;

	private Tween yCamTween;

	private Tween zoomCamTween;

	private Tween angleCamTween;

	private Vector2 _windowScale = Vector2.one;

	private float lastShakeTime;

	private Vector2 lastShakeOffset;

	private Vector2 currentShakeTarget;

	private Tween timeShiftTween;

	private float timeShift;

	public bool forceNoXWrap;

	public bool forceNoYWrap;

	private Vector2 screenResolution;

	public Vector2Int viewSize;

	private Vector2Int currentWindowCenterPos;

	private Vector2Int cachedWindowCenterPos;

	private float timeOfLastReset;

	private bool presetChanged = true;

	public Window window;

	public bool enabled = true;

	public bool paused;

	public bool visible;

	private WindowChoreographer choreographer;

	private float edgeAnchorStartScale = 1f;

	private float edgeAnchorStartPivot = 0.5f;

	public WindowDancePreset preset => currentPresetInfo.preset;

	public SamePresetBehavior samePresetBehavior => currentPresetInfo.samePresetBehavior;

	public float speed => currentPresetInfo.speed;

	public float frequency => currentPresetInfo.frequency;

	public float period => currentPresetInfo.period;

	public EasingType easeType => currentPresetInfo.easeType;

	public Ease subEase => currentPresetInfo.subEase;

	public float easeDur => currentPresetInfo.easeDur;

	public Ease ease => currentPresetInfo.ease;

	private Vector2 xyScreenCoords => position * screenResolution;

	public bool pivotIsInCenterMode => pivotEdgeV == Vector2.zero;

	private float timeSinceLastReset => Time.timeSinceLevelLoad - timeOfLastReset;

	public Vector2 windowScale
	{
		get
		{
			if (!base.game.paused || base.editorMode || choreographer is VirtualWindowChoreographer)
			{
				return _windowScale;
			}
			return Vector2.one;
		}
		set
		{
			_windowScale = value;
		}
	}

	public WindowDancer(Window window, WindowChoreographer choreographer)
	{
		this.window = window;
		this.choreographer = choreographer;
	}

	public void LateUpdate()
	{
		if (!paused && enabled)
		{
			(Vector2Int, Vector2Int) tuple = UpdatePreset();
			SetWindow(tuple.Item1, tuple.Item2);
		}
	}

	public void ShowGlitch(bool show)
	{
		if (base.game.startTheGameCalled)
		{
			RDClass.Vfx.fxGlitch.visible = show;
		}
		else
		{
			RDClass.Vfx.uiFxGlitch.SetActive(show);
		}
	}

	public void CheckDefaultValues(bool setup = false)
	{
		if ((screenResolution.x == 0f || screenResolution.y == 0f || viewSize == Vector2Int.zero) | setup)
		{
			if (setup)
			{
				window.Reset(choreographer);
			}
			screenResolution = choreographer.WindowDanceResolution;
			currentWindowCenterPos = window.GetDefaultCenter(choreographer);
			viewSize = window.GetViewSize();
			cachedWindowCenterPos = currentWindowCenterPos;
			position = currentWindowCenterPos / screenResolution;
			_windowScale = Vector2.one;
			pivot = new Vector2(0.5f, 0.5f);
			pivotEdgeV = Vector2.zero;
		}
	}

	public void Cancel()
	{
		enabled = false;
		window.Reset(choreographer);
	}

	public void ChangePreset(WindowDancePresetInfo presetInfo, bool usingX = true, bool usingY = true, bool usingAngle = true, bool usingAmpX = true, bool usingAmpY = true)
	{
		UpdatePreset();
		WindowDancePresetInfo windowDancePresetInfo = default;
		windowDancePresetInfo.CopyFrom(currentPresetInfo);
		currentWindowCenterPos = window.GetCenter();
		cachedWindowCenterPos = currentWindowCenterPos;
		Vector2 v = (cachedPivot - new Vector2(0.5f, 0.5f)) * viewSize;
		cachedWindowCenterPos += Vector2Int.RoundToInt(v);
		Vector2 vector = cachedWindowCenterPos / screenResolution;
		screenResolution = choreographer.WindowDanceResolution;
		viewSize = window.GetViewSize();
		currentPresetInfo = presetInfo;
		presetChanged = preset != windowDancePresetInfo.preset || samePresetBehavior == SamePresetBehavior.Reset;
		if (!presetChanged)
		{
			timeSpentOnCurrentPreset += timeSinceLastReset;
			currentPresetInfo.speed = windowDancePresetInfo.speed;
			currentPresetInfo.frequency = windowDancePresetInfo.frequency;
			if (preset == WindowDancePreset.Wrap)
			{
				currentPresetInfo.angle = windowDancePresetInfo.angle;
			}
		}
		else
		{
			timeSpentOnCurrentPreset = 0f;
			if (!usingX)
			{
				currentPresetInfo.pos.x = vector.x;
			}
			if (!usingY)
			{
				currentPresetInfo.pos.y = vector.y;
			}
			windowDancePresetInfo.amplitude = Vector2.zero;
			if (preset == WindowDancePreset.ShakePer)
			{
				lastShakeTime = 0f;
				lastShakeOffset = Vector2.zero;
				currentShakeTarget = UnityEngine.Random.insideUnitCircle;
			}
			else if (preset == WindowDancePreset.Move)
			{
				position = vector;
			}
		}
		timeOfLastReset = Time.timeSinceLevelLoad;
		if (presetChanged)
		{
			xPosTween?.Kill();
			yPosTween?.Kill();
			angleTween?.Kill();
			ampXTween?.Kill();
			ampYTween?.Kill();
			transitionV = 0f;
			DOTween.To(() => transitionV, (float x) =>
			{
				transitionV = x;
			}, 1f, easeDur).SetEase(ease).Done();
		}
		if (usingX)
		{
			xPosTween?.Kill();
			xPosTween = DOTween.To(() => position.x, (float x) =>
			{
				position.x = x;
			}, presetInfo.pos.x, easeDur).SetEase(ease).Done();
		}
		if (usingY)
		{
			yPosTween?.Kill();
			yPosTween = DOTween.To(() => position.y, (float y) =>
			{
				position.y = y;
			}, presetInfo.pos.y, easeDur).SetEase(ease).Done();
		}
		if (usingAngle)
		{
			angleTween?.Kill();
			angleTween = DOTween.To(() => angle, (float a) =>
			{
				angle = a;
			}, presetInfo.angle, easeDur).SetEase(ease).Done();
		}
		if (usingAmpX)
		{
			ampXTween?.Kill();
			ampXTween = DOTween.To(() => amplitude.x, (float a) =>
			{
				amplitude.x = a;
			}, presetInfo.amplitude.x, easeDur).SetEase(ease).Done();
		}
		if (usingAmpY)
		{
			ampYTween?.Kill();
			ampYTween = DOTween.To(() => amplitude.y, (float a) =>
			{
				amplitude.y = a;
			}, presetInfo.amplitude.y, easeDur).SetEase(ease).Done();
		}
	}

	public void SetScale(Float2 scale, float easeDur, Ease ease, PivotAnchorType anchorType = PivotAnchorType.None)
	{
		if (scale.xUsed)
		{
			xScaleTween?.Kill();
			xScaleTween = DOTween.To(() => _windowScale.x, (float x) =>
			{
				_windowScale.x = x;
			}, scale.x, easeDur).SetEase(ease);
			if (anchorType == PivotAnchorType.LeftEdge || anchorType == PivotAnchorType.RightEdge)
			{
				xPivotTween.Kill();
				pivotEdgeV.x = 0f;
				edgeAnchorStartScale = _windowScale.x;
				edgeAnchorStartPivot = pivot.x;
				xScaleTween.OnUpdate(() =>
				{
					float num = ((anchorType == PivotAnchorType.RightEdge) ? (1f - edgeAnchorStartPivot) : (0f - edgeAnchorStartPivot));
					float num2 = (_windowScale.x - edgeAnchorStartScale) * num;
					pivot.x = edgeAnchorStartPivot + num2 / _windowScale.x;
				});
			}
			xScaleTween.Done();
		}
		if (!scale.yUsed)
		{
			return;
		}
		yScaleTween?.Kill();
		yScaleTween = DOTween.To(() => _windowScale.y, (float y) =>
		{
			_windowScale.y = y;
		}, scale.y, easeDur).SetEase(ease);
		if (anchorType == PivotAnchorType.BottomEdge || anchorType == PivotAnchorType.TopEdge)
		{
			yPivotTween.Kill();
			pivotEdgeV.y = 0f;
			edgeAnchorStartScale = _windowScale.y;
			edgeAnchorStartPivot = pivot.y;
			yScaleTween.OnUpdate(() =>
			{
				float num = ((anchorType == PivotAnchorType.TopEdge) ? (1f - edgeAnchorStartPivot) : (0f - edgeAnchorStartPivot));
				float num2 = (_windowScale.y - edgeAnchorStartScale) * num;
				pivot.y = edgeAnchorStartPivot + num2 / _windowScale.y;
			});
		}
		yScaleTween.Done();
	}

	public void SetPivot(Float2 piv, ReferenceType refX, ReferenceType refY, float easeDur, Ease ease)
	{
		if (piv.xUsed)
		{
			xPivotTween?.Kill();
			if (refX == ReferenceType.Edge)
			{
				xPivotTween = DOTween.To(() => pivotEdgeV.x, (float x) =>
				{
					pivotEdgeV.x = x;
				}, 1f, easeDur).SetEase(ease).Done();
			}
			else
			{
				pivotEdgeV.x = 0f;
				pivot.x = cachedPivot.x;
				xPivotTween = DOTween.To(() => pivot.x, (float x) =>
				{
					pivot.x = x;
				}, piv.x, easeDur).SetEase(ease).Done();
			}
		}
		if (!piv.yUsed)
		{
			return;
		}
		yPivotTween?.Kill();
		if (refY == ReferenceType.Edge)
		{
			yPivotTween = DOTween.To(() => pivotEdgeV.y, (float y) =>
			{
				pivotEdgeV.y = y;
			}, 1f, easeDur).SetEase(ease).Done();
			return;
		}
		pivotEdgeV.y = 0f;
		pivot.y = cachedPivot.y;
		yPivotTween = DOTween.To(() => pivot.y, (float y) =>
		{
			pivot.y = y;
		}, piv.y, easeDur).SetEase(ease).Done();
	}

	public void SetCamPosition(Float2 position, float easeDur, Ease ease)
	{
		if (position.xUsed)
		{
			xCamTween?.Kill();
			float endValue = position.x / 100f - 0.5f;
			xCamTween = window.material.DOFloat(endValue, "_PosX", easeDur).SetEase(ease).Done();
		}
		if (position.yUsed)
		{
			yCamTween?.Kill();
			float endValue2 = position.y / 100f - 0.5f;
			yCamTween = window.material.DOFloat(endValue2, "_PosY", easeDur).SetEase(ease).Done();
		}
	}

	public void SetCamAngle(float angle, float easeDur, Ease ease)
	{
		angleCamTween.Kill();
		float endValue = angle * MathF.PI / 180f;
		angleCamTween = window.material.DOFloat(endValue, "_Angle", easeDur).SetEase(ease).Done();
	}

	public void SetCamZoom(float zoom, float easeDur, Ease ease)
	{
		float endValue = zoom / 100f;
		zoomCamTween?.Kill();
		zoomCamTween = window.material.DOFloat(endValue, "_Scale", easeDur).SetEase(ease).Done();
	}

	public void SetTimeShift(float shift, float easeDur, Ease ease)
	{
		timeShiftTween?.Kill();
		timeShiftTween = DOTween.To(() => timeShift, (float t) =>
		{
			timeShift = t;
		}, shift, easeDur).SetEase(ease).Done();
	}

	private Vector2 GetBaseViewSize()
	{
		int windowScaleForDancing = choreographer.GetWindowScaleForDancing();
		return new Vector2(Mathf.RoundToInt(352 * windowScaleForDancing), Mathf.RoundToInt(198 * windowScaleForDancing));
	}

	public (Vector2Int, Vector2Int) UpdatePreset()
	{
		Vector2 baseViewSize = GetBaseViewSize();
		CheckDefaultValues();
		Vector2 vector = baseViewSize * _windowScale;
		float num = timeSinceLastReset + timeSpentOnCurrentPreset + timeShift;
		Vector2 vector2 = Vector2.zero;
		Vector2 vector3 = (presetChanged ? (currentPresetInfo.pos * screenResolution) : xyScreenCoords);
		Vector2 vector4 = (presetChanged ? currentPresetInfo.amplitude : amplitude);
		float x = vector4.x;
		float num2 = (presetChanged ? currentPresetInfo.angle : angle);
		switch (preset)
		{
		case WindowDancePreset.Move:
			vector2 = xyScreenCoords;
			break;
		case WindowDancePreset.Sway:
		{
			float num4 = num * frequency;
			float num5 = Mathf.Repeat(num4, 1f);
			bool flag = Mathf.FloorToInt(num4) % 2 != 0;
			float num6 = Diagonal(num2, screenResolution);
			Vector2 vector6 = x * num6 * RDUtils.AngleToVec(num2);
			if (easeType == EasingType.Mirror)
			{
				if (flag)
				{
					num5 = 1f - num5;
				}
				vector6 *= DOVirtual.EasedValue(0f, 1f, num5, subEase);
			}
			else
			{
				float num7 = (flag ? 1f : 0f);
				vector6 *= DOVirtual.EasedValue(num7, 1f - num7, num5, subEase);
			}
			vector2 = vector3 + vector6;
			break;
		}
		case WindowDancePreset.Wrap:
		{
			Vector2 bounds = screenResolution + vector;
			float num9 = Diagonal(num2, bounds);
			float num10 = Diagonal(num2 + MathF.PI / 2f, bounds);
			Vector2 vector7 = Mathf.Sin(MathF.PI * 2f * num * frequency) * (x / 2f) * num10 * RDUtils.AngleToVec(num2 + MathF.PI / 2f);
			Vector2 vector8 = vector3 + vector / 2f;
			float num11 = num * speed;
			Vector2 vector9 = num9 * RDUtils.AngleToVec(num2) * num11;
			vector2 = RDUtils.RepeatVector(vector8 + vector9 + vector7, bounds, forceNoXWrap, forceNoYWrap) - vector / 2f;
			break;
		}
		case WindowDancePreset.Ellipse:
		{
			float num8 = num2 - MathF.PI * 2f * speed * num;
			vector2 = vector3 + vector4 * screenResolution * RDUtils.AngleToVec(num8);
			break;
		}
		case WindowDancePreset.ShakePer:
		{
			if ((num - lastShakeTime) * frequency >= 1f)
			{
				lastShakeOffset = currentShakeTarget;
				Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
				currentShakeTarget = new Vector2(insideUnitCircle.x, insideUnitCircle.y);
				lastShakeTime = num;
			}
			float num3 = Mathf.Max(period, 0.001f);
			float lifetimePercentage = Mathf.Clamp((num - lastShakeTime) / num3, 0f, 1f);
			float t = DOVirtual.EasedValue(0f, 1f, lifetimePercentage, subEase);
			Vector2 vector5 = RDUtils.LerpUnclampedVector(lastShakeOffset, currentShakeTarget, t);
			vector5 *= vector4 * screenResolution;
			vector2 = vector3 + vector5;
			break;
		}
		}
		if (presetChanged && preset != WindowDancePreset.Move)
		{
			vector2 = RDUtils.LerpUnclampedVector(cachedWindowCenterPos, vector2, transitionV);
		}
		Vector2 vector10 = vector2 / screenResolution;
		cachedPivot = new Vector2(Mathf.LerpUnclamped(pivot.x, vector10.x, pivotEdgeV.x), Mathf.LerpUnclamped(pivot.y, vector10.y, pivotEdgeV.y));
		Vector2 vector11 = (new Vector2(0.5f, 0.5f) - cachedPivot) * vector;
		vector2 += vector11;
		return (vector2.RoundToInt(), vector.RoundToInt());
	}

	private void SetWindow(Vector2Int centerPosition, Vector2Int viewSize)
	{
		currentWindowCenterPos = centerPosition;
		Vector2Int vector2Int = currentWindowCenterPos - (window.GetBorderSize() + viewSize) / 2;
		window.SetFrame(vector2Int, viewSize);
		this.viewSize = viewSize;
	}

	public float Diagonal(float ang, Vector2 bounds)
	{
		float num = Mathf.Repeat(ang, MathF.PI);
		float num2 = Mathf.Atan2(bounds.y, bounds.x);
		bool num3 = Mathf.Abs(MathF.PI / 2f - num) < MathF.PI / 2f - num2;
		float f = (num3 ? ang : (MathF.PI / 2f - ang));
		return Mathf.Abs((num3 ? bounds.y : bounds.x) / Mathf.Sin(f));
	}

	public void SetVisible(bool visible)
	{
		this.visible = visible;
		window.Show(visible);
	}

	public void OnPauseChanged(bool paused)
	{
		this.paused = paused;
		if (choreographer is VirtualWindowChoreographer)
		{
			return;
		}
		if (window is CustomWindow)
		{
			bool show = !paused && visible;
			window.Show(show);
			return;
		}
		bool show2 = paused || visible;
		window.Show(show2);
		if (paused)
		{
			window.Reset(choreographer);
		}
	}

	public void OnDestroy()
	{
		window.OnDestroy();
	}
}
