using System;
using UnityEngine;
using DG.Tweening.Plugins.Options;

namespace DG.Tweening.Plugins.Options
{
    public struct FloatOptions { public bool snapping; public FloatOptions(bool snapping) { this.snapping = snapping; } }
    public struct Vector3Options { public bool snapping; public bool axisConstraint; public Vector3Options(bool snapping = false, bool axisConstraint = false) { this.snapping = snapping; this.axisConstraint = axisConstraint; } }
    public struct QuaternionOptions { public bool useShortest360; public QuaternionOptions(bool useShortest360) { this.useShortest360 = useShortest360; } }
    public struct ColorOptions { public bool alphaOnly; public ColorOptions(bool alphaOnly) { this.alphaOnly = alphaOnly; } }
    public struct StringOptions { public bool richTextEnabled; public ScrambleMode scrambleMode; public string scrambleChars; public StringOptions(bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null) { this.richTextEnabled = richTextEnabled; this.scrambleMode = scrambleMode; this.scrambleChars = scrambleChars; } }
    public struct PathOptions { public bool isClosedPath; public AxisConstraint lockPosition; public AxisConstraint lockRotation; public OrientMode orientMode; public PathOptions(bool isClosedPath = false, AxisConstraint lockPosition = AxisConstraint.None, AxisConstraint lockRotation = AxisConstraint.None, OrientMode orientMode = OrientMode.LookAtPosition) { this.isClosedPath = isClosedPath; this.lockPosition = lockPosition; this.lockRotation = lockRotation; this.orientMode = orientMode; } }
    public enum AxisConstraint { None, X, Y, Z, W }
    public enum OrientMode { None, LookAtPosition, LookAtTransform }
    public struct NoOptions { }
}

namespace DG.Tweening.Plugins.Core
{
    public class ABSTweenPlugin<T1, T2, TPlugOptions> where TPlugOptions : struct { }
    public class TweenPlugin<T1, T2, TPlugOptions> : ABSTweenPlugin<T1, T2, TPlugOptions> where TPlugOptions : struct { }
    public class Vector3Plugin : TweenPlugin<Vector3, Vector3, Vector3Options> { }
    public class Vector2Plugin : TweenPlugin<Vector2, Vector2, Vector3Options> { }
    public class FloatPlugin : TweenPlugin<float, float, FloatOptions> { }
    public class ColorPlugin : TweenPlugin<Color, Color, ColorOptions> { }
    public class QuaternionPlugin : TweenPlugin<Quaternion, Quaternion, QuaternionOptions> { }
    public class StringPlugin : TweenPlugin<string, string, StringOptions> { }
    public class RectPlugin : TweenPlugin<Rect, Rect, NoOptions> { }
    public class UintPlugin : TweenPlugin<uint, uint, NoOptions> { }
    public class IntPlugin : TweenPlugin<int, int, NoOptions> { }
    public class LongPlugin : TweenPlugin<long, long, NoOptions> { }
}

namespace DG.Tweening.Core.Enums
{
    public enum UpdateMode { Update, LateUpdate, FixedUpdate, Manual }
    public enum SpecialStartupMode { None, SetLookAt, SetShake, SetPunch }
    public enum Operation { Complete, Despawn, Flip, Goto, Pause, Play, Restart, Rewind, Start }
}

namespace DG.Tweening.Core
{
    public class TweenManager { }
    public class TweenDescriptor { }
    public class TweenParams
    {
        public TweenParams SetDelay(float delay) { return this; }
        public TweenParams SetLoops(int loops, LoopType loopType = LoopType.Restart) { return this; }
        public TweenParams SetEase(Ease ease) { return this; }
        public TweenParams SetUpdate(UpdateType updateType, bool isIndependentUpdate = false) { return this; }
        public TweenParams SetId(object id) { return this; }
        public TweenParams SetTarget(object target) { return this; }
        public TweenParams OnComplete(Action callback) { return this; }
        public TweenParams OnStart(Action callback) { return this; }
        public TweenParams OnKill(Action callback) { return this; }
    }
}



// 补充缺失类型
namespace DG.Tweening.Plugins.Options
{
    public struct VectorOptions { public bool snapping; public bool axisConstraint; public VectorOptions(bool snapping = false, bool axisConstraint = false) { this.snapping = snapping; this.axisConstraint = axisConstraint; } }
}

namespace DG.Tweening.Plugins.Core.PathCore
{
    public class Path { }
    public class CatmullRomDecoder { }
    public class LinearDecoder { }
    public class CubicBezierDecoder { }
}
