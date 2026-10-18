using UnityEngine;
using DG.Tweening;

public static partial class Tk2dExtensions
{
    public static Tweener DOFade(this tk2dSprite target, float endValue, float duration) { return new Tweener(); }
    public static Tweener DOScale(this tk2dSprite target, Vector3 endValue, float duration) { return new Tweener(); }
    public static Tweener DOMove(this tk2dSprite target, Vector3 endValue, float duration, bool snapping = false) { return new Tweener(); }
    public static void DOKill(this tk2dSprite target, bool complete = false) { }
    public static void DOPlay(this tk2dSprite target) { }
    public static void DOPause(this tk2dSprite target) { }
    public static void DOComplete(this tk2dSprite target) { }
    public static Tweener DOScaleY(this tk2dSprite target, float endValue, float duration) { return new Tweener(); }
    public static Tweener DOScaleX(this tk2dSprite target, float endValue, float duration) { return new Tweener(); }
    public static Tweener DOLocalMove(this tk2dSprite target, Vector3 endValue, float duration, bool snapping = false) { return new Tweener(); }
    public static Tweener DOLocalMoveX(this tk2dSprite target, float endValue, float duration) { return new Tweener(); }
    public static Tweener DOLocalMoveY(this tk2dSprite target, float endValue, float duration) { return new Tweener(); }
    public static Tweener DOMoveX(this tk2dSprite target, float endValue, float duration) { return new Tweener(); }
    public static Tweener DOMoveY(this tk2dSprite target, float endValue, float duration) { return new Tweener(); }
    public static Tweener DORotate(this tk2dSprite target, Vector3 endValue, float duration) { return new Tweener(); }
    public static Tweener DOShakePosition(this tk2dSprite target, float duration, Vector3 strength, int vibrato = 10, float randomness = 90f) { return new Tweener(); }
    public static Tweener DOPunchPosition(this tk2dSprite target, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1f) { return new Tweener(); }
    public static Tweener DOScale(this tk2dBaseSprite target, float endValue, float duration) { return new Tweener(); }
}

// tk2dSprite DOColor 扩展
public static partial class Tk2dExtensions
{
    public static DG.Tweening.Tweener DOColor(this tk2dSprite sprite, UnityEngine.Color color, float duration)
    {
        return null;
    }
}


// tk2dBaseSprite DOColor 扩展
public static partial class Tk2dExtensions
{
    public static DG.Tweening.Tweener DOColor(this tk2dBaseSprite sprite, UnityEngine.Color color, float duration)
    {
        return null;
    }
}
