using UnityEngine;
using DG.Tweening;

#if UNITY_WEBGL

// TempoAudio 和 TempoSound
public static class TempoAudio
{
    public static TempoSound Play(string clip, float volume = 1f, float pan = 0f) { return null; }
    public static float Volume { get { return 0f; } set { } }
    public static void StartClock() { }
    public static double Clock { get { return 0.0; } }
}

public class TempoSound : MonoBehaviour
{
    public float Volume { get { return 0f; } set { } }
    public float Pan { get { return 0f; } set { } }
    public void Unschedule() { }
    public void Schedule() { }
    public void Schedule(float time) { }
    public void Play() { }
    public void Init() { }
    public void Init(AudioClip clip) { }
}

// UrlProtocolHandler
public static class UrlProtocolHandler
{
    public static void SetupProtocol() { }
    public static string GetArgument(string name) { return ""; }
    public static string GetArgument() { return ""; }
}

// scnRhythmWeightlifter（全局命名空间别名）
public class scnRhythmWeightlifter : MonoBehaviour
{
    public static scnRhythmWeightlifter instance { get { return null; } }
    public bool DogMode { get { return false; } set { } }
    public static object[] highScores { get { return new object[0]; } }
    public static scnRhythmWeightlifter gameInstance { get { return null; } }
}

// Tween 扩展方法
public static partial class TweenExtensions
{
    public static Tween PlayForward(this Tween t) { return t; }
    public static Tween PlayBackwards(this Tween t) { return t; }
    public static Tween onComplete(this Tween t, System.Action action) { return t; }
}

// Story 扩展方法
public static class StoryExtensions
{
    public static bool HasFunction(this Ink.Runtime.Story story, string functionName) { return false; }
}

// LocalizationManager 扩展方法
public static class LocalizationManagerExtensions
{
    public static object localizedStringProvider(this object manager) { return null; }
    public static void prefetch(this object manager) { }
}

// Perspective2DSprite 扩展方法
public static class Perspective2DSpriteExtensions
{
    public static Tweener DOJump(this Perspective2DSprite sprite, Vector2 endValue, float jumpPower, int numJumps, float duration, bool snapping = false) { return null; }
    public static Tweener DOMove(this Perspective2DSprite sprite, Vector3 endValue, float duration, bool snapping = false) { return null; }
}

// RDInputType_Joystick 桩
public class RDInputType_Joystick : RDInputType
{
    public RDInputType_Joystick(int playerId) : base(playerId) { }
    public RDInputType_Joystick() : base(0) { }
    public override bool Cancel(RDButtonState state) { return false; }
    public override bool Down(RDButtonState state) { return false; }
    public override bool FaceLeft(RDButtonState state) { return false; }
    public override bool FaceUp(RDButtonState state) { return false; }
    public override bool FinerControl(RDButtonState state) { return false; }
    public override bool Left(RDButtonState state) { return false; }
    public override bool Main(RDButtonState state) { return false; }
    public override bool Quit() { return false; }
    public override bool Restart() { return false; }
    public override bool Right(RDButtonState state) { return false; }
    public override bool Select(RDButtonState state) { return false; }
    public override bool Skip() { return false; }
    public override bool Up(RDButtonState state) { return false; }
}

// DOTweenModuleUI
public static class DOTweenModuleUI
{
    public static Tweener DOAnchorPos(this RectTransform rect, Transform target, float duration, bool snapping = false) { return null; }

}

// Entity 扩展方法
public static class EntityExtensions
{
    public static void DOKill(this Entity entity, bool complete = false) { }
}

// Tween 更多扩展方法
public static partial class TweenExtensions
{
    public static bool IsPlaying(this Tween t) { return false; }
    public static Tween Flip(this Tween t) { return t; }
}

// UnityPlayerWindow Windows/Mac（基类在 WindowStubs.cs 中）
public class UnityPlayerWindowWindows : UnityPlayerWindow
{
    public UnityPlayerWindowWindows(int a, int b) { }
}
public class UnityPlayerWindowMac : UnityPlayerWindow
{
    public UnityPlayerWindowMac(int a, int b) { }
}

// WindowsTTS
public class WindowsTTS : MonoBehaviour
{
    public static WindowsTTS instance { get { return null; } }
    public void ToggleSAPI() { }
}

// ReInput 扩展
public static class ReInput
{
    public class ControllerHelper
    {
        public Controller[] Controllers { get { return new Controller[0]; } }
        public Controller GetLastActiveController() { return null; }
        public Controller[] controllers { get { return new Controller[0]; } }
        public bool GetAnyButtonDown() { return false; }
        public Joystick[] Joysticks { get { return new Joystick[0]; } }
        public int joystickCount { get { return 0; } }
    }
    
    public class PlayerHelper
    {
        public Player GetPlayer(int id) { return null; }
        public Player GetPlayer(string name) { return null; }
    }
    
    public static ControllerHelper controllers { get { return new ControllerHelper(); } }
    public static PlayerHelper players { get { return new PlayerHelper(); } }
    public static event System.Action InitializedEvent;
    public static event System.Action ControllerConnectedEvent;
    public static event System.Action ControllerDisconnectedEvent;
}

public class Player
{
    public Controller[] controllers { get { return new Controller[0]; } }
    public Joystick GetLastActiveController() { return null; }
}

public partial class Controller
{
    public int id { get { return 0; } }
}

public class Joystick : Controller
{
}

public static partial class ControllerArrayExtensions
{
    public static int Count(this Joystick[] joysticks) { return joysticks.Length; }
}

// Reader 扩展
public static class ReaderExtensions
{
    public static bool IsTimestampMoreRecent(this object reader, string timestamp) { return false; }
}

// Reader 静态类扩展
public static class ReaderStaticExtensions
{
    public static bool IsTimestampMoreRecent(string timestamp) { return false; }
}

// Reload 和 JsonParser
public static class Reload
{
    public static void ReloadAll() { }
    public static void ReloadTarget(object target) { }
}
public static class JsonParser
{
    public static T FromJson<T>(string json) { return default(T); }
}

// RDTiledBackground 扩展方法
public static class RDTiledBackgroundExtensions
{
    public static void DOKill(this RDTiledBackground bg, bool complete = false) { }
}

// RDPublishPopup (RDLevelEditor 命名空间)
namespace RDLevelEditor
{
    public class RDPublishPopup : MonoBehaviour
    {
        public static RDPublishPopup instance { get { return null; } }
        public static string[] GetFilesForLevel(string levelPath, ref System.Collections.Generic.List<string> optionalFilesName, ref System.Collections.Generic.List<string> rdlevelFiles, object levelData = null, string mainLevelName = null) { return new string[0]; }
        public void Show() { }
        public void Hide() { }
    }
}

// FileDialog (UnityFileDialog 命名空间)
namespace UnityFileDialog
{
    public class FileDialog : System.IDisposable
    {
        public void SetTitle(string title) { }
        public void SetDirectory(string directory) { }
        public void AddFilter(string name, string extension) { }
        public void AddFilter(string name, string[] extensions) { }
        public string[] PickFiles() { return new string[0]; }
        public string PickFile() { return null; }
        public void Dispose() { }
    }
}

#endif
