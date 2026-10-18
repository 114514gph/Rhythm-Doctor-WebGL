using DG.Tweening;
using UnityEngine;
using System.Collections;

// WindowDancer 桩 - 模拟模式需要
public class WindowDancer : MonoBehaviour
{
    public WindowDancer() { }
    public WindowDancer(object a, object b) { }
    public int roomIndex;
    public bool enabled;
    public bool visible;
    public Window window;
    public Vector2 windowScale;
    public void UpdatePreset() { }
    public void SetCamPosition(Vector2 pos) { }
    public void SetCamPosition(float x, float y, float duration) { }
    public void SetCamPosition(Float2 pos, float duration, Ease ease) { }
    public void SetCamAngle(float angle) { }
    public void SetCamAngle(float angle, float duration, Ease ease) { }
    public bool pivotIsInCenterMode;
    public void ChangePreset(object preset) { }
    public void ChangePreset(object preset, float a, float b, float c, float d, float e) { }
    public void ChangePreset(object preset, bool a, bool b, bool c, bool d, bool e) { }
    public void SetScale(Vector2 scale) { }
    public void SetScale(float x, float y) { }
    public void SetScale(float x, float y, float z) { }
    public void SetScale(float x, float y, float z, float w) { }
    public void SetScale(Float2 scale, float duration, Ease ease, object pivot) { }
    public void SetScale(Float2 scale, float duration, Ease ease) { }
    public void SetPivot(Vector2 pivot) { }
    public void SetPivot(float x, float y, float z, float w, float duration) { }
    public void SetPivot(Float2 p, object anchorX, object anchorY, float duration, DG.Tweening.Ease ease) { }
    public void SetCamZoom(float zoom) { }
    public void SetCamZoom(float zoom, float duration, Ease ease) { }
    public bool forceNoYWrap;
    public bool forceNoXWrap;
    public void LateUpdate() { }
    public void CheckDefaultValues(bool check) { }
    public void SetVisible(bool visible) { }
    public void OnPauseChanged(bool paused) { }
    public void ShowGlitch(bool show) { }
    public void Cancel() { }
    public void OnDestroy() { }
    public void SetTimeShift(float timeShift) { }
    public void SetTimeShift(float timeShift, int roomIndex, bool force) { }
    public void SetTimeShift(float shift, float duration, DG.Tweening.Ease ease) { }
}

// UnityPlayerWindow 桩
public class UnityPlayerWindow : MonoBehaviour
{
    public static UnityPlayerWindow playerWindow { get { return null; } }
    public virtual void Show() { }
    public virtual void Show(bool visible) { }
    public virtual void ResetPosition() { }
    public virtual void ResetPosition(object arg) { }
    public virtual void SetTitle(string title) { }
    public virtual Vector2Int GetViewSize() { return Vector2Int.zero; }
    public virtual void SetViewSize(Vector2Int viewSize) { }
    public virtual void SetFrame(Vector2Int position, Vector2Int viewSize) { }
    public virtual void SetPosition(Vector2Int position, bool takesTaskbarIntoAccount = false) { }
    public virtual Vector2Int GetPosition() { return Vector2Int.zero; }
    public virtual Vector2Int GetDefaultCenter(WindowChoreographer choreographer) { return Vector2Int.zero; }
    public virtual void ResetSize(WindowChoreographer choreographer) { }
    public void SetCamPosition(Vector2 pos) { }
    public void SetCamPosition(float x, float y, float duration) { }
    public void SetCamPosition(Float2 pos, float duration, Ease ease) { }
    public void SetCamAngle(float angle) { }
    public void SetCamAngle(float angle, float duration, Ease ease) { }
    public bool pivotIsInCenterMode;
    public void ChangePreset(object preset) { }
    public void ChangePreset(object preset, float a, float b, float c, float d, float e) { }
    public void ChangePreset(object preset, bool a, bool b, bool c, bool d, bool e) { }
}

// RealWindowChoreographer 桩 - 防止编译错误
public class RealWindowChoreographer : WindowChoreographer
{
    public static float WindowedSwitchTime = 0f;
    public RealWindowChoreographer(bool mainWindowIsDancer = false) { }
    public override Vector2Int WindowDanceResolution { get { return new Vector2Int(1920, 1080); } }
    public override IEnumerator Setup(int dancerCount) { yield break; }
    public void UpdateWindowInputs() { }
    public void CheckInputState() { }
    public void CheckInputState(object a, object b) { }
    protected override IEnumerator PrepareForDancing() { yield break; }
}
