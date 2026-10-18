using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DG.Tweening
{
    public enum Ease
    {
        Linear, InSine, OutSine, InOutSine,
        InQuad, OutQuad, InOutQuad,
        InCubic, OutCubic, InOutCubic,
        InQuart, OutQuart, InOutQuart,
        InQuint, OutQuint, InOutQuint,
        InExpo, OutExpo, InOutExpo,
        InCirc, OutCirc, InOutCirc,
        InElastic, OutElastic, InOutElastic,
        InBack, OutBack, InOutBack,
        InBounce, OutBounce, InOutBounce,
        Flash, InFlash, OutFlash, InOutFlash,
        Internal_Zero, INTERNAL_Zero, Unset
    }

    public enum LoopType { Restart, Yoyo, Incremental }
    public enum UpdateType { Normal, Late, Fixed, Manual }
    public enum PathType { Linear, CatmullRom, CubicBezier }
    public enum PathMode { Ignore, Full3D, TopDown2D, Sidescroller2D }
    public enum RotateMode { Fast,
    LocalAxisAdd,
    FastBeyond360, Fast360, WorldAxis, LocalAxis }
    public enum LinkBehaviour { Rewind, Restart, Complete, Kill, Cancel, None }
    public enum RewindCallbackMode { FireOnRewind, DontFireOnRewind }
    public enum AutoPlay { None, AutoPlaySequences, AutoPlayTweeners, All }
    public enum TweenType { Tweener, Sequence }
    public delegate float EaseFunction(float time, float duration, float overshootOrAmplitude, float period);
    public enum ScrambleMode { None, All, Numerals, Uppercase, Lowercase, Custom }
    public enum ShakeRandomnessMode { Full, Harmonic }

    public partial class Tween : IDisposable
    {
        public static implicit operator Tween(bool value) { return new Tween(); }
        public bool active;
        public bool isPlaying;
        public bool isComplete;
        public float fullPosition;
        public bool isRelative;
        public float Duration() { return duration; }
        public Tween SetEase(Ease ease, float overshoot, float period) { return this; }
        public float ElapsedPercentage { get { return 0; } }
        public Tweener Done() { return new Tweener(); }
        public bool isBackwards;
        public float duration;
        public float totalDuration;
        public float delay;
        public int loops;
        public LoopType loopType;
        public Ease ease;
        public float timeScale = 1;
        public object target;
        public string id;

        public Tween() { }
        public virtual void Dispose() { }
        public virtual Tween Play() { isPlaying = true; return this; }
        public virtual Tween Pause() { isPlaying = false; return this; }
        public virtual Tween Restart() { return this; }
        public virtual Tween Rewind() { return this; }
        public virtual Tween Complete() { isComplete = true; return this; }
        public virtual Tween Complete(bool withCallbacks) { isComplete = true; return this; }
        public virtual Tween Goto(float to, bool andPlay = false) { return this; }
        public virtual Tween Kill() { active = false; return this; }
        public virtual Tween Kill(bool complete) { active = false; return this; }
        public virtual Tween SetDelay(float delay) { this.delay = delay; return this; }
        public virtual Tween SetLoops(int loops, LoopType loopType = LoopType.Restart) { this.loops = loops; this.loopType = loopType; return this; }
        public virtual Tween SetEase(Ease ease) { this.ease = ease; return this; }
        public virtual Tween SetEase(AnimationCurve ease) { return this; }
        public virtual Tween SetEase(EaseFunction ease) { return this; }
        public virtual Tween SetEase(Ease ease, float overshoot) { return this; }
        public virtual Tween SetUpdate(UpdateType updateType, bool isIndependentUpdate = false) { return this; }
        public virtual Tween SetUpdate(bool isIndependentUpdate) { return this; }
        public virtual Tween SetLink(GameObject target, LinkBehaviour behaviour) { return this; }
        public virtual Tween SetId(object id) { this.id = id?.ToString(); return this; }
        public virtual Tween SetTarget(object target) { this.target = target; return this; }
        public virtual Tween SetSpeedBased(bool isSpeedBased) { return this; }
        public virtual Tween SetRelative(bool isRelative) { return this; }
        public virtual Tween From() { return this; }
        public virtual Tween From(bool isRelative) { return this; }
        public virtual Tween OnComplete(TweenCallback callback) { return this; }
        public virtual Tween OnStart(TweenCallback callback) { return this; }
        public virtual Tween OnKill(TweenCallback callback) { return this; }
        public virtual Tween AppendCallback(TweenCallback callback) { return this; }
        public virtual Tween AppendInterval(float interval) { return this; }
        public virtual Tween Append(Tween tween) { return this; }
        public virtual Tween Join(Tween tween) { return this; }
        public virtual Tween OnPlay(Action callback) { return this; }
        public virtual Tween OnPause(Action callback) { return this; }
        public Vector3 position;
        public virtual Tween OnRewind(Action callback) { return this; }
        public virtual Tween OnStepComplete(Action callback) { return this; }
        public virtual Tween OnUpdate(Action callback) { return this; }
        public virtual Tween OnWaypointChange(Action<int> callback) { return this; }
        public virtual Tween SetOptions(bool snapping) { return this; }
        public virtual Tween SetOptions(bool option1, bool option2) { return this; }
    }

    public partial class Tweener : Tween
    {
        public Tweener() : base() { }
        public static implicit operator Sequence(Tweener tweener) { return new Sequence(); }
        public new Tweener Play() { base.Play(); return this; }
        public new Tweener Pause() { base.Pause(); return this; }
        public new Tweener Restart() { base.Restart(); return this; }
        public new Tweener Rewind() { base.Rewind(); return this; }
        public new Tweener Complete() { base.Complete(); return this; }
        public new Tweener Kill() { base.Kill(); return this; }
        public new Tweener Kill(bool complete) { base.Kill(complete); return this; }
        public new Tweener SetDelay(float delay) { base.SetDelay(delay); return this; }
        public new Tweener SetLoops(int loops, LoopType loopType = LoopType.Restart) { base.SetLoops(loops, loopType); return this; }
        public new Tweener SetEase(Ease ease) { base.SetEase(ease); return this; }
        public new Tweener SetUpdate(UpdateType updateType, bool isIndependentUpdate = false) { base.SetUpdate(updateType, isIndependentUpdate); return this; }
        public new Tweener SetLink(GameObject target, LinkBehaviour behaviour) { base.SetLink(target, behaviour); return this; }
        public new Tweener SetId(object id) { base.SetId(id); return this; }
        public new Tweener SetTarget(object target) { base.SetTarget(target); return this; }
        public new Tweener SetSpeedBased(bool isSpeedBased) { base.SetSpeedBased(isSpeedBased); return this; }
        public new Tweener SetRelative(bool isRelative) { base.SetRelative(isRelative); return this; }
        public new Tweener From() { base.From(); return this; }
        public new Tweener From(bool isRelative) { base.From(isRelative); return this; }
        public new Tweener OnComplete(TweenCallback callback) { base.OnComplete(callback); return this; }
        public new Tweener OnStart(TweenCallback callback) { base.OnStart(callback); return this; }
        public new Tweener OnKill(TweenCallback callback) { base.OnKill(callback); return this; }
        public new Tweener OnPlay(Action callback) { base.OnPlay(callback); return this; }
        public new Tweener OnPause(Action callback) { base.OnPause(callback); return this; }
        public new Tweener OnUpdate(Action callback) { base.OnUpdate(callback); return this; }
        public Tweener ChangeEndValue(object endValue) { return this; }
        public Tweener ChangeStartValue(object startValue) { return this; }
        public Tweener ChangeValues(object startValue, object endValue) { return this; }
        public Tweener ChangeDuration(float duration) { this.duration = duration; return this; }
    }

    public partial class Sequence : Tween
    {
        public Sequence() : base() { }
        public Sequence Append(Tween tween) { return this; }
        public Sequence AppendCallback(Action callback) { return this; }
        public Sequence AppendInterval(float interval) { return this; }
        public Sequence Insert(float atPosition, Tween tween) { return this; }
        public Sequence InsertCallback(float atPosition, Action callback) { return this; }
        public Sequence Join(Tween tween) { return this; }
        public Sequence Prepend(Tween tween) { return this; }
        public Sequence PrependCallback(Action callback) { return this; }
        public Sequence PrependInterval(float interval) { return this; }
        public new Sequence Play() { base.Play(); return this; }
        public new Sequence Pause() { base.Pause(); return this; }
        public new Sequence Restart() { base.Restart(); return this; }
        public new Sequence Kill() { base.Kill(); return this; }
        public new Sequence Kill(bool complete) { base.Kill(complete); return this; }
        public new Sequence SetDelay(float delay) { base.SetDelay(delay); return this; }
        public new Sequence SetLoops(int loops, LoopType loopType = LoopType.Restart) { base.SetLoops(loops, loopType); return this; }
        public new Sequence SetEase(Ease ease) { base.SetEase(ease); return this; }
        public TweenCallback onComplete;
        public new Sequence OnComplete(TweenCallback callback) { base.OnComplete(callback); return this; }
        public new Sequence OnStart(TweenCallback callback) { base.OnStart(callback); return this; }
        public new Sequence OnKill(TweenCallback callback) { base.OnKill(callback); return this; }
    }

    public static partial class DOTween
    {
        public static float timeScale = 1;
        public static float maxUnityTimeScale = 1000;
        public static int maxActiveTweens = 200;
        public static int maxActiveSequences = 50;

        public static Sequence Sequence() { return new Sequence(); }
        public static Sequence Sequence(Tween tween) { return new Sequence(); }
        public static Tweener To(object target, string property, object endValue, float duration) { return new Tweener(); }

        public static Tweener To(DOGetter<Quaternion> getter, DOSetter<Quaternion> setter, Quaternion endValue, float duration) { return new Tweener(); }
        public static Tweener To(DOGetter<int> getter, DOSetter<int> setter, int endValue, float duration) { return new Tweener(); }
                public static TweenerCore<Vector3, Vector3, VectorOptions> To(Func<Vector3> getter, Action<Vector3> setter, Vector3 endValue, float duration) { return new TweenerCore<Vector3, Vector3, VectorOptions>(); }
                public static TweenerCore<Vector2, Vector2, VectorOptions> To(Func<Vector2> getter, Action<Vector2> setter, Vector2 endValue, float duration) { return new TweenerCore<Vector2, Vector2, VectorOptions>(); }
        public static TweenerCore<float, float, FloatOptions> To(DOGetter<float> getter, DOSetter<float> setter, float endValue, float duration) { return new TweenerCore<float, float, FloatOptions>(); }
        public static TweenerCore<Color, Color, ColorOptions> To(DOGetter<Color> getter, DOSetter<Color> setter, Color endValue, float duration) { return new TweenerCore<Color, Color, ColorOptions>(); }
        public static TweenerCore<Color, Color, ColorOptions> ToAlpha(DOGetter<Color> getter, DOSetter<Color> setter, float endValue, float duration) { return new TweenerCore<Color, Color, ColorOptions>(); }
        public static void Kill(object target) { }
        public static void Kill(object target, bool complete) { }
        public static void Complete(object target) { }
        public static void Complete(object target, bool withCallbacks) { }
        public static void Restart(object target) { }
        public static void Restart(object target, bool includeDelay) { }
        public static void KillAll(bool complete, string id) { }
        public static void Pause(object target) { }
        public static void Play(object target) { }
        public static void Rewind(object target) { }
        public static void Rewind(object target, bool includeDelay) { }
        public static bool IsTweening(object target) { return false; }
        public static bool IsPlaying(object target) { return false; }
        public static void Init() { }
        public static void SetTweensCapacity(int maxTweens, int maxSequences) { }
        public static void Clear() { }
        public static void Validate() { }
        public static List<Tween> PausedTweens() { return new List<Tween>(); }
        public static List<Tween> PlayingTweens() { return new List<Tween>(); }
    }

    public delegate T DOGetter<out T>();
    public delegate void DOSetter<in T>(T value);

    public abstract class ABSAnimationComponent : MonoBehaviour
    {
        public bool autoPlay = true;
        public bool autoKill = true;
        public float duration = 1;
        public int loops = 1;
        public LoopType loopType = LoopType.Restart;
        public Ease easeType = Ease.OutQuad;
        public AnimationCurve easeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public string id;
        public bool isActive;
        public bool isPlaying;
        public bool isComplete;
        public float fullPosition;
        public bool isRelative;
        public float Duration { get { return 0; } }
        public ABSAnimationComponent SetEase(Ease ease, float overshoot, float period) { return this; }
        public float ElapsedPercentage { get { return 0; } }
        public Tweener Done() { return new Tweener(); }
        public Tween tween;
        public virtual void CreateTween() { }
        public virtual void Restart() { }
        public virtual void DORestart(bool includeDelay = true) { }
        public virtual void DOPlay() { }
        public virtual void DOPause() { }
        public virtual void DOTogglePause() { }
        public virtual void DORewind() { }
        public virtual void DOComplete() { }
        public virtual void DOKill() { }
        public virtual void DOFlip() { }
        public virtual void DORestart() { }
        public virtual void DOPlayBackwards() { }
        public virtual void DOPlayForward() { }
        public virtual void DOGotoAndPause(float to) { }
        public virtual void DOGotoAndPlay(float to) { }
        public virtual void DOGoto(float to, bool andPlay = false) { }
        public virtual void DOSmoothRewind() { }
    }
}

namespace DG.Tweening.Core.Easing
{
    public static class EaseManager
    {
        public static float Evaluate(Ease ease, float time, float duration, float overshootOrAmplitude, float period) { return 0; }
    }
}




namespace DG.Tweening
{
    public partial class Tween
    {
        public bool IsActive() { return active; }
    }
    public partial class Sequence
    {
        public bool IsActive() { return active; }
    }
}

namespace DG.Tweening
{
    public static partial class DOTween
    {
        public static LogBehaviour logBehaviour { get; set; }
    }
    public enum LogBehaviour { Default, Verbose, ErrorsOnly }
}


namespace DG.Tweening
{
    public static partial class DOTween
    {
        public static void KillAll() { }
        public static void KillAll(bool complete) { }
    }
}



namespace DG.Tweening
{
    public partial class Tween
    {
        public float Duration(bool includeLoops) { return duration; }
    }
}
