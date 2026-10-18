using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

// DOTween 核心扩展方法桩
public static class ShortcutExtensions
{
    // Transform - Vector3 tweens return TweenerCore<Vector3,Vector3,VectorOptions>
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMove(this Transform target, Vector3 endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveX(this Transform target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveY(this Transform target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveZ(this Transform target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalMove(this Transform target, Vector3 endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalMoveX(this Transform target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalMoveY(this Transform target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalMoveZ(this Transform target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DORotate(this Transform target, Vector3 endValue, float duration, RotateMode mode = RotateMode.Fast) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalRotate(this Transform target, Vector3 endValue, float duration, RotateMode mode = RotateMode.Fast) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOScale(this Transform target, float endValue, float duration) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOScale(this Transform target, Vector3 endValue, float duration) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleX(this Transform target, float endValue, float duration) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleY(this Transform target, float endValue, float duration) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleZ(this Transform target, float endValue, float duration) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOLookAt(this Transform target, Vector3 towards, float duration, AxisConstraint axisConstraint = AxisConstraint.None, Vector3? up = null) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOPunchPosition(this Transform target, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOPunchRotation(this Transform target, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOPunchScale(this Transform target, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static Tweener DOShakePosition(this Transform target, float duration, float strength = 1, int vibrato = 10, float randomness = 90, bool snapping = false, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full) { return new Tweener(); }
    public static Tweener DOShakePosition(this Transform target, float duration, Vector3 strength, int vibrato = 10, float randomness = 90, bool snapping = false, bool fadeOut = true) { return new Tweener(); }
    public static Tweener DOShakeRotation(this Transform target, float duration, float strength = 1, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full) { return new Tweener(); }
    public static Tweener DOShakeScale(this Transform target, float duration, float strength = 1, int vibrato = 10, float randomness = 90, bool fadeOut = true, ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full) { return new Tweener(); }
    public static Tweener DOBlendableMoveBy(this Transform target, Vector3 byValue, float duration, bool snapping = false) { return new Tweener(); }
    public static Tweener DOBlendableLocalMoveBy(this Transform target, Vector3 byValue, float duration, bool snapping = false) { return new Tweener(); }
    public static Tweener DOBlendableRotateBy(this Transform target, Vector3 byValue, float duration, RotateMode mode = RotateMode.Fast) { return new Tweener(); }
    public static Tweener DOBlendableLocalRotateBy(this Transform target, Vector3 byValue, float duration, RotateMode mode = RotateMode.Fast) { return new Tweener(); }
    public static Tweener DOBlendableScaleBy(this Transform target, Vector3 byValue, float duration) { return new Tweener(); }

    // Material
    public static TweenerCore<Color, Color, ColorOptions> DOColor(this Material target, Color endValue, float duration) { return new TweenerCore<Color, Color, ColorOptions>(); }
    public static TweenerCore<Color, Color, ColorOptions> DOColor(this Material target, Color endValue, string property, float duration) { return new TweenerCore<Color, Color, ColorOptions>(); }
    public static TweenerCore<Color, Color, ColorOptions> DOColor(this Material target, Color endValue, int propertyID, float duration) { return new TweenerCore<Color, Color, ColorOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFade(this Material target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFade(this Material target, float endValue, string property, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFloat(this Material target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFloat(this Material target, float endValue, string property, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFloat(this Material target, float endValue, int propertyID, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static Tweener DOVector(this Material target, Vector4 endValue, float duration) { return new Tweener(); }
    public static TweenerCore<Vector4, Vector4, VectorOptions> DOVector(this Material target, Vector4 endValue, string property, float duration) { return new TweenerCore<Vector4, Vector4, VectorOptions>(); }
    public static TweenerCore<Vector4, Vector4, VectorOptions> DOVector(this Material target, Vector4 endValue, int propertyID, float duration) { return new TweenerCore<Vector4, Vector4, VectorOptions>(); }
    public static Tweener DOGradientColor(this Material target, Gradient endValue, float duration) { return new Tweener(); }
    public static Tweener DOGradientColor(this Material target, Gradient endValue, string property, float duration) { return new Tweener(); }
    public static Tweener DOOffset(this Material target, Vector2 endValue, float duration) { return new Tweener(); }
    public static Tweener DOTiling(this Material target, Vector2 endValue, float duration) { return new Tweener(); }

    // Light
    public static TweenerCore<Color, Color, ColorOptions> DOColor(this Light target, Color endValue, float duration) { return new TweenerCore<Color, Color, ColorOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOIntensity(this Light target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOShadowStrength(this Light target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }

    // Camera
    public static TweenerCore<float, float, FloatOptions> DOAspect(this Camera target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<Color, Color, ColorOptions> DOColor(this Camera target, Color endValue, float duration) { return new TweenerCore<Color, Color, ColorOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFieldOfView(this Camera target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOOrthoSize(this Camera target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static Tweener DOPixelRect(this Camera target, Rect endValue, float duration) { return new Tweener(); }
    public static Tweener DORect(this Camera target, Rect endValue, float duration) { return new Tweener(); }
    public static TweenerCore<float, float, FloatOptions> DONearClip(this Camera target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFarClip(this Camera target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }

    // AudioSource
    public static TweenerCore<float, float, FloatOptions> DOFade(this AudioSource target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOPitch(this AudioSource target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }

    // SpriteRenderer
    public static TweenerCore<Color, Color, ColorOptions> DOColor(this SpriteRenderer target, Color endValue, float duration) { return new TweenerCore<Color, Color, ColorOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFade(this SpriteRenderer target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFlipX(this SpriteRenderer target, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFlipY(this SpriteRenderer target, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOSize(this SpriteRenderer target, Vector2 endValue, float duration) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }

    // Rigidbody
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMove(this Rigidbody target, Vector3 endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveX(this Rigidbody target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveY(this Rigidbody target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveZ(this Rigidbody target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DORotate(this Rigidbody target, Vector3 endValue, float duration, RotateMode mode = RotateMode.Fast) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }

    // Rigidbody2D
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOMove(this Rigidbody2D target, Vector2 endValue, float duration, bool snapping = false) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveX(this Rigidbody2D target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveY(this Rigidbody2D target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DORotate(this Rigidbody2D target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }

    // RectTransform
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorPos(this RectTransform target, Vector2 endValue, float duration, bool snapping = false) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorPosX(this RectTransform target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorPosY(this RectTransform target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorMax(this RectTransform target, Vector2 endValue, float duration, bool snapping = false) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorMin(this RectTransform target, Vector2 endValue, float duration, bool snapping = false) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOPivot(this RectTransform target, Vector2 endValue, float duration) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOPivotX(this RectTransform target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOPivotY(this RectTransform target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOSizeDelta(this RectTransform target, Vector2 endValue, float duration, bool snapping = false) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOSizeDeltaX(this RectTransform target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOSizeDeltaY(this RectTransform target, float endValue, float duration, bool snapping = false) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }
    public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalMove(this RectTransform target, Vector3 endValue, float duration, bool snapping = false) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
    public static Tweener DOJump(this RectTransform target, Vector2 endValue, float jumpPower, int numJumps, float duration, bool snapping = false) { return new Tweener(); }
    public static Tweener DOLocalJump(this Transform target, Vector3 endValue, float jumpPower, int numJumps, float duration, bool snapping = false) { return new Tweener(); }

    // Graphic (UI)
    public static TweenerCore<Color, Color, ColorOptions> DOColor(this UnityEngine.UI.Graphic target, Color endValue, float duration) { return new TweenerCore<Color, Color, ColorOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFade(this UnityEngine.UI.Graphic target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }

    // Image
    public static TweenerCore<float, float, FloatOptions> DOFillAmount(this UnityEngine.UI.Image target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFade(this UnityEngine.UI.Image target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<Color, Color, ColorOptions> DOColor(this UnityEngine.UI.Image target, Color endValue, float duration) { return new TweenerCore<Color, Color, ColorOptions>(); }

    // Text
    public static TweenerCore<Color, Color, ColorOptions> DOColor(this UnityEngine.UI.Text target, Color endValue, float duration) { return new TweenerCore<Color, Color, ColorOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFade(this UnityEngine.UI.Text target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static Tweener DOText(this UnityEngine.UI.Text target, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null) { return new Tweener(); }

    // Outline
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOScale(this UnityEngine.UI.Outline target, Vector2 endValue, float duration) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }
    public static TweenerCore<float, float, FloatOptions> DOFade(this UnityEngine.UI.Outline target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
    public static TweenerCore<Color, Color, ColorOptions> DOColor(this UnityEngine.UI.Outline target, Color endValue, float duration) { return new TweenerCore<Color, Color, ColorOptions>(); }

    // CanvasGroup
    public static TweenerCore<float, float, FloatOptions> DOFade(this CanvasGroup target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }

    // Transform.DOPath
    public static Tweener DOPath(this Transform target, Vector3[] path, float duration, PathType pathType = PathType.Linear, PathMode pathMode = PathMode.Full3D, int resolution = 10, Color? gizmoColor = null) { return new Tweener(); }
    public static Tweener DOLocalPath(this Transform target, Vector3[] path, float duration, PathType pathType = PathType.Linear, PathMode pathMode = PathMode.Full3D, int resolution = 10, Color? gizmoColor = null) { return new Tweener(); }

    // float
    public static TweenerCore<float, float, FloatOptions> DOTween(this float target, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
}

// 更多扩展方法
public static class ShortcutExtensions2
{
    // DOKill, DOPlay, DOPause, DORestart 等
    public static void DOKill(this Transform target, bool complete = false) { }
    public static void DOPlay(this Transform target) { }
    public static void DOPause(this Transform target) { }
    public static void DORestart(this Transform target, bool includeDelay = true) { }
    public static void DORewind(this Transform target, bool includeDelay = true) { }
    public static void DOComplete(this Transform target, bool withCallbacks = false) { }
    public static void DOFlip(this Transform target) { }
    public static void DOGoto(this Transform target, float to, bool andPlay = false) { }
    public static void DOTogglePause(this Transform target) { }

    public static void DOKill(this RectTransform target, bool complete = false) { }
    public static void DOPlay(this RectTransform target) { }
    public static void DOPause(this RectTransform target) { }
    public static void DORestart(this RectTransform target, bool includeDelay = true) { }

    public static void DOKill(this UnityEngine.UI.Image target, bool complete = false) { }
    public static void DOPlay(this UnityEngine.UI.Image target) { }
    public static void DOPause(this UnityEngine.UI.Image target) { }
    public static void DORestart(this UnityEngine.UI.Image target, bool includeDelay = true) { }

    public static void DOKill(this UnityEngine.UI.Text target, bool complete = false) { }
    public static void DOPlay(this UnityEngine.UI.Text target) { }
    public static void DOPause(this UnityEngine.UI.Text target) { }

    public static void DOKill(this Material target, bool complete = false) { }
    public static void DOPlay(this Material target) { }
    public static void DOPause(this Material target) { }

    public static void DOKill(this Camera target, bool complete = false) { }
    public static void DOPlay(this Camera target) { }
    public static void DOPause(this Camera target) { }

    public static void DOKill(this CanvasGroup target, bool complete = false) { }
    public static void DOPlay(this CanvasGroup target) { }
    public static void DOPause(this CanvasGroup target) { }

    public static void DOKill(this AudioSource target, bool complete = false) { }
    public static void DOPlay(this AudioSource target) { }
    public static void DOPause(this AudioSource target) { }

    public static void DOKill(this Light target, bool complete = false) { }
    public static void DOPlay(this Light target) { }
    public static void DOPause(this Light target) { }

    public static void DOKill(this SpriteRenderer target, bool complete = false) { }
    public static void DOPlay(this SpriteRenderer target) { }
    public static void DOPause(this SpriteRenderer target) { }

    public static void DOKill(this Rigidbody target, bool complete = false) { }
    public static void DOKill(this Rigidbody2D target, bool complete = false) { }

    // tk2d 扩展
    public static Tweener DOFade(this tk2dTiledSprite target, float endValue, float duration) { return new Tweener(); }
    public static Tweener DOColor(this tk2dTiledSprite target, Color endValue, float duration) { return new Tweener(); }
    public static Tweener DOScale(this tk2dTiledSprite target, Vector3 endValue, float duration) { return new Tweener(); }
}

// SteamApps
namespace Steamworks
{
    public static partial class SteamApps
    {
        public static bool BIsAppInstalled(AppId_t appID) { return false; }
        public static AppId_t GetAppInstallDir(AppId_t appID, string folder, uint folderBufferSize) { return new AppId_t(); }
        public static uint GetAppBuildId() { return 0; }
        public static string GetAppInstallDir(AppId_t appID) { return ""; }
        public static bool IsDlcInstalled(AppId_t appID) { return false; }
    }
}

// DOVirtual
public static class DOVirtual
{
    public static TweenerCore<float, float, FloatOptions> Float(float from, float to, float duration, TweenCallback<float> onVirtualUpdate) { return new TweenerCore<float, float, FloatOptions>(); }
    public static Tweener Vector3(Vector3 from, Vector3 to, float duration, TweenCallback<Vector3> onVirtualUpdate) { return new Tweener(); }
    public static Tweener Color(Color from, Color to, float duration, TweenCallback<Color> onVirtualUpdate) { return new Tweener(); }
    public static Tweener Int(int from, int to, float duration, TweenCallback<int> onVirtualUpdate) { return new Tweener(); }
    public static float EasedValue(float startValue, float endValue, float time, float duration, Ease ease, float overshoot = 1.70158f, float period = 0) { return 0; }
    public static float EasedValue(float startValue, float endValue, float time, float duration, EaseFunction ease, float overshoot = 1.70158f, float period = 0) { return 0; }
    public static Tweener DelayedCall(float delay, TweenCallback callback, bool ignoreTimeScale = true) { return new Tweener(); }
    public static float EasedValue(float time, float duration, Ease ease, float overshoot = 1.70158f, float period = 0) { return 0; }
    public static float EasedValue(float startValue, float endValue, float time, float duration, Ease ease) { return 0; }
    public static float EasedValue(float startValue, float endValue, float time, Ease ease) { return 0; }
}

public delegate void TweenCallback<in T>(T value);


// TweenCallback
public delegate void TweenCallback();


// 更多扩展方法
public static class ShortcutExtensions3
{
    public static Tweener DOAnchorPos3D(this RectTransform target, Vector3 endValue, float duration, bool snapping = false) { return new Tweener(); }
    public static Tweener DOAnchorPos3DX(this RectTransform target, float endValue, float duration, bool snapping = false) { return new Tweener(); }
    public static Tweener DOAnchorPos3DY(this RectTransform target, float endValue, float duration, bool snapping = false) { return new Tweener(); }
    public static Tweener DOAnchorPos3DZ(this RectTransform target, float endValue, float duration, bool snapping = false) { return new Tweener(); }
}

// DOTween 静态类补充
public static partial class DOTweenExtensions
{
    public static Ease defaultEaseType { get { return Ease.Linear; } }
    public static Tweener Shake(Vector3 position, float duration, float strength, int vibrato, float randomness) { return new Tweener(); }
}

// Debugger
public static class Debugger
{
    public static void Log(object message) { }
    public static void LogWarning(object message) { }
    public static void LogError(object message) { }
}

namespace DG.Tweening
{
    public static partial class DOTween
    {
        public static Tweener ToAlpha(System.Func<float> getter, System.Action<float> setter, float endValue, float duration) { return null; }
        public static Tweener Punch(System.Func<Vector3> getter, System.Action<Vector3> setter, Vector3 punch, float duration, int vibrato, float elasticity = 1f) { return null; }
        public static Tweener Punch(System.Func<Vector3> getter, System.Action<Vector3> setter, Vector3 punch, float duration) { return null; }
    }
}


// AudioSource 扩展
public static class AudioSourceExtensions
{
    public static Tweener DOFadeStop(this AudioSource target, float duration) { return new Tweener(); }
}

// RectTransform 扩展
public static class RectTransformExtensions
{
    public static Tweener DOShakeAnchorPos(this RectTransform target, float duration, Vector2 strength, int vibrato, float randomness) { return new Tweener(); }
}

// Transform 扩展
public static class TransformExtensions
{
    public static Tweener DOJump(this Transform target, Vector3 endValue, float jumpPower, int numJumps, float duration, bool snapping = false) { return new Tweener(); }
}


public static class Tk2dSpriteExtra
{
    public static void DOComplete(this tk2dSprite target, bool withCallbacks = false) { }
}

public static class RawImageExtensions
{
    public static void DOKill(this UnityEngine.UI.RawImage target, bool complete = false) { }
    public static Tweener DOShakePosition(this Camera target, float duration, float strength, int vibrato, float randomness) { return new Tweener(); }
}


public static class ScrollRectExtensions
{
    public static Tweener DOVerticalNormalizedPos(this ScrollRect target, float endValue, float duration) { return new Tweener(); }
    public static Tweener DOHorizontalNormalizedPos(this ScrollRect target, float endValue, float duration) { return new Tweener(); }
}
