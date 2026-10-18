using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening.Plugins.Options;

// DOTween 核心类型补充
namespace DG.Tweening.Core
{
    public partial class TweenerCore<T1, T2, TPlugOptions> : Tweener where TPlugOptions : struct
    {
        public TweenerCore() : base() { }
        public new TweenerCore<T1, T2, TPlugOptions> SetDelay(float delay) { base.SetDelay(delay); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> SetLoops(int loops, LoopType loopType = LoopType.Restart) { base.SetLoops(loops, loopType); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> SetEase(Ease ease) { base.SetEase(ease); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> OnComplete(TweenCallback callback) { base.OnComplete(callback); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> OnStart(TweenCallback callback) { base.OnStart(callback); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> OnKill(TweenCallback callback) { base.OnKill(callback); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> OnUpdate(Action callback) { base.OnUpdate(callback); return this; }
        public TweenerCore<T1, T2, TPlugOptions> SetOptions(TPlugOptions options) { return this; }
    }
}



// Rewired 补充
namespace Rewired
{
    public class InputMapper
    {
        public static InputMapper instance { get; } = new InputMapper();
        public void Start() { }
        public void Stop() { }
        public bool isRunning { get { return false; } }
    }
    
    
    public struct AxisRange
    {
        public float min;
        public float max;
        public AxisRange(float min, float max) { this.min = min; this.max = max; }
    }
    
    public interface IControllerTemplateThumbStick : IControllerTemplateElement { }
    
    public class ControllerTemplate
    {
        public static T Create<T>(Controller controller) where T : IControllerTemplate { return default(T); }
    }
}


// 其他缺失类型
public class ActionIdFieldInfoAttribute : Attribute
{
    public string friendlyName;
    public string categoryName;
    public ActionIdFieldInfoAttribute(string friendlyName = "", string categoryName = "") { this.friendlyName = friendlyName; this.categoryName = categoryName; }
}

public class ActionIdFieldInfo
{
    public string friendlyName;
    public string categoryName;
}

namespace DG.Tweening.Core
{
    public partial class TweenerCore<T1, T2, TPlugOptions>
    {
        public T2 endValue;
        public T1 startValue;
        public object WaitForCompletion() { return null; }
        public new TweenerCore<T1, T2, TPlugOptions> From(T1 fromValue) { startValue = fromValue; return this; }
        public new TweenerCore<T1, T2, TPlugOptions> SetEase(EaseFunction ease) { base.SetEase(ease); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> SetEase(AnimationCurve ease) { base.SetEase(ease); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> SetUpdate(bool isIndependentUpdate) { base.SetUpdate(isIndependentUpdate); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> SetUpdate(UpdateType type, bool isIndependentUpdate = false) { base.SetUpdate(type, isIndependentUpdate); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> SetRelative(bool isRelative) { base.SetRelative(isRelative); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> Play() { base.Play(); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> Kill() { base.Kill(); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> Kill(bool complete) { base.Kill(complete); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> OnPlay(Action callback) { base.OnPlay(callback); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> OnPause(Action callback) { base.OnPause(callback); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> Pause() { base.Pause(); return this; }
        public new TweenerCore<T1, T2, TPlugOptions> OnStepComplete(Action callback) { return this; }
    }
}

namespace DG.Tweening
{
    public partial class Tweener
    {
        public new Tweener From(float fromValue) { return this; }
        public new Tweener SetEase(EaseFunction ease) { base.SetEase(ease); return this; }
        public new Tweener SetEase(AnimationCurve ease) { base.SetEase(ease); return this; }
    }

    public partial class Sequence
    {
        public Sequence JoinCallback(Action callback) { return this; }
    }

    public static partial class DOTween
    {
        public static List<Tween> TweensByTarget(object target) { return new List<Tween>(); }
    }

}