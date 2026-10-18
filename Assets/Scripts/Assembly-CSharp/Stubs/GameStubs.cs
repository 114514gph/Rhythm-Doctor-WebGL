using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RDClassDll 桩
public partial class RDClassDll 
{
    public override string ToString() { return base.ToString(); }
}

// TweenCallback 桩

// System.Media 桩
namespace System.Media
{
    public class SoundPlayer
    {
        public SoundPlayer() { }
        public SoundPlayer(string location) { }
        public void Play() { }
        public void PlaySync() { }
        public void Stop() { }
        public void Load() { }
    }
}

// Immutable 集合补充
namespace System.Collections.Immutable
{
    public class ImmutableDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value) { return this; }
        public ImmutableDictionary<TKey, TValue> Remove(TKey key) { return this; }
        public bool ContainsKey(TKey key) { return false; }
        public TValue this[TKey key] { get { return default(TValue); } set { } }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { yield break; }
        IEnumerator IEnumerable.GetEnumerator() { yield break; }
        public static ImmutableDictionary<TKey, TValue> Empty { get { return new ImmutableDictionary<TKey, TValue>(); } }
    }
    public struct ImmutableArray<T> : IEnumerable<T>
    {
        public int Length { get { return 0; } }
        public T this[int index] { get { return default(T); } }
        public IEnumerator<T> GetEnumerator() { yield break; }
        public int IndexOf(T item) { return -1; }
        IEnumerator IEnumerable.GetEnumerator() { yield break; }
        public static ImmutableArray<T> Empty { get { return new ImmutableArray<T>(); } }
        public static ImmutableArray<T> CreateRange(IEnumerable<T> items) { return new ImmutableArray<T>(); }
    }
    public static class ImmutableDictionary
    {
        public static ImmutableDictionary<TKey, TValue> Create<TKey, TValue>() { return new ImmutableDictionary<TKey, TValue>(); }
    }
    public static class ImmutableArray
    {
        public static ImmutableArray<T> Create<T>() { return new ImmutableArray<T>(); }
        public static ImmutableArray<T> Create<T>(T a) { return new ImmutableArray<T>(); }
        public static ImmutableArray<T> Create<T>(T a, T b) { return new ImmutableArray<T>(); }
        public static ImmutableArray<T> Create<T>(T a, T b, T c) { return new ImmutableArray<T>(); }
        public static ImmutableArray<T> Create<T>(T a, T b, T c, T d) { return new ImmutableArray<T>(); }
        public static ImmutableArray<T> Create<T>(T a, T b, T c, T d, T e) { return new ImmutableArray<T>(); }
        public static ImmutableArray<T> Create<T>(T a, T b, T c, T d, T e, T f) { return new ImmutableArray<T>(); }
        public static ImmutableArray<T> CreateRange<T>(IEnumerable<T> items) { return new ImmutableArray<T>(); }
    }
}

// ImmutableList
namespace System.Collections.Immutable
{
    public class ImmutableList<T> : IEnumerable<T>
    {
        public int Count { get { return 0; } }
        public T this[int index] { get { return default(T); } }
        public ImmutableList<T> Add(T item) { return this; }
        public ImmutableList<T> Remove(T item) { return this; }
        public IEnumerator<T> GetEnumerator() { yield break; }
        public int IndexOf(T item) { return -1; }
        IEnumerator IEnumerable.GetEnumerator() { yield break; }
        public static ImmutableList<T> Empty { get { return new ImmutableList<T>(); } }
    }
    public static class ImmutableList
    {
        public static ImmutableList<T> Create<T>() { return new ImmutableList<T>(); }
        public static ImmutableList<T> CreateRange<T>(IEnumerable<T> items) { return new ImmutableList<T>(); }
    }
}

// EaseFunction 由 DG.Tweening 提供

// System.Text.Unicode
namespace System.Text.Unicode
{
    public static partial class UnicodeRanges { }
}

// MultiWindow.Events
namespace MultiWindow
{
    namespace Events { }
}

// MultiWindow.Size
namespace MultiWindow
{
    public struct Size
    {
        public int width;
        public int height;
        public Size(int width, int height) { this.width = width; this.height = height; }
    }
}

// UnicodeRange
public class UnicodeRange {
        public uint FirstCodePoint { get; set; }
        public uint LastCodePoint { get; set; }
        public uint Length { get { return LastCodePoint - FirstCodePoint; } }
        public string name;
        public int firstCodePoint;
        public int length;
        public UnicodeRange(string name, int firstCodePoint, int length) { this.name = name; this.firstCodePoint = firstCodePoint; this.length = length; }
    }
    public class UnicodeRangeOld
{
    public string name;
    public int firstCodePoint;
    public int length;
    public UnicodeRangeOld(string name, int firstCodePoint, int length) { this.name = name; this.firstCodePoint = firstCodePoint; this.length = length; }
}

// Steamworks 类型（Assembly-CSharp 需要）
namespace Steamworks
{
    public class UserStatsReceived_t
    {
        public ulong steamIDUser;
        public EResult result;
        public AppId_t gameID;
    }
    public class UserStatsStored_t
    {
        public ulong steamIDUser;
        public EResult result;
        public AppId_t gameID;
    }
}

// PlayerWindow
public class PlayerWindow { public static PlayerWindow Instance; public System.IntPtr Hwnd; public void ResetPosition(object arg) { } }

// 确保 Assembly-CSharp 也能访问这些命名空间
namespace Rewired.Dev { }
namespace Rewired.Utils.Libraries.TinyJson { }
namespace Rewired.Interfaces
{
    public interface ILocalizedStringProvider
    {
        
        bool TryGetLocalizedString(string key, out string value);
    }
}

// SteamApps 补充
namespace Steamworks
{
    public static class SteamApps2
    {
        public static uint GetCurrentBetaName(out string betaName, int bufferSize) { betaName = ""; return 0; }
    }
}

// RDClassDll 补充
public partial class RDClassDll
{
    public static void printem(params object[] args) { }
    public static void printe(params object[] args) { }
    public static void print(params object[] args) { }
}

// ReInput
namespace Rewired
{
    public static partial class ReInput
    {
        public static PlayerManager players { get; } = new PlayerManager();
        public static InputManager_Base inputManager { get; set; }
        public static bool isReady { get; } = true;
    }

    public class PlayerManager
    {
        public Player GetPlayer(int id) { return new Player(); }
        public Player GetPlayer(string name) { return new Player(); }
        public Player this[int id] { get { return new Player(); } }
    }
}

// SteamUserStats
namespace Steamworks
{
    public static class SteamUserStats
    {
        public static bool RequestCurrentStats() { return true; }
        public static bool GetStat(string name, out int data) { data = 0; return true; }
        public static bool GetStat(string name, out float data) { data = 0; return true; }
        public static bool SetStat(string name, int data) { return true; }
        public static bool SetStat(string name, float data) { return true; }
        public static bool StoreStats() { return true; }
        public static bool SetAchievement(string name) { return true; }
        public static bool GetAchievement(string name, out bool achieved) { achieved = false; return true; }
        public static bool ClearAchievement(string name) { return true; }
        public static SteamAPICall_t RequestGlobalStats(int historyDays) { return new SteamAPICall_t(); }
    }
}

// FileBrowser
public static partial class FileBrowser
{
    public static bool OpenFilePanel(string title, string directory, string extension, System.Action<string> onDone) { return false; }
    public static bool SaveFilePanel(string title, string directory, string defaultName, string extension, System.Action<string> onDone) { return false; }
}

// Registry
namespace Microsoft.Win32
{
    public static class Registry
    {
        public static readonly object CurrentUser = new object();
        public static readonly object LocalMachine = new object();
    }
}

// Ink.Runtime 由 Ink-Libraries.dll 提供，不做桩

// Rewired Player 补充
namespace Rewired
{
    public partial class Player
    {
        public Player.ControllerHelper controllers { get { return new Player.ControllerHelper(); } }
        public Controller GetFirstControllerWithButton(string actionName) { return null; }
        public bool GetButton(string actionName) { return false; }
        public bool GetButtonDown(string actionName) { return false; }
        public bool GetButtonUp(string actionName) { return false; }
        public float GetAxis(string actionName) { return 0; }
    }

    public static partial class ReInput
    {
        public static ControllerManager controllers { get; } = new ControllerManager();
        public static LocalizationManager localization { get; } = new LocalizationManager();
    }

    public partial class ControllerManager
    {
        public Joystick[] joysticks { get { return new Joystick[0]; } }
        public Keyboard keyboard { get { return new Keyboard(); } }
        public Mouse mouse { get { return new Mouse(); } }
        public object GetLastActiveController() { return null; }
    }

    public class LocalizationManager
    {
        public string GetString(string key) { return key; }
    }

    public class Keyboard : Controller { }
    public class Mouse : Controller { }
}

// SteamFriends
namespace Steamworks
{
    public struct CSteamID { }
    public enum EPersonaState { k_EPersonaStateOffline, k_EPersonaStateOnline, k_EPersonaStateBusy, k_EPersonaStateAway, k_EPersonaStateSnooze, k_EPersonaStateLookingToTrade, k_EPersonaStateLookingToPlay }

    public static class SteamFriends
    {
        public static string GetPersonaName() { return "Player"; }
        public static SteamId GetSteamID() { return new SteamId(); }
        public static bool RequestUserInformation(CSteamID steamID, bool requireNameOnly) { return true; }
        public static int GetFriendCount(int friendFlags) { return 0; }
        public static CSteamID GetFriendByIndex(int iFriend, int friendFlags) { return new CSteamID(); }
        public static string GetFriendPersonaName(CSteamID steamID) { return ""; }
        public static EPersonaState GetFriendPersonaState(CSteamID steamID) { return EPersonaState.k_EPersonaStateOffline; }
    }
}

// Controller 扩展
namespace Rewired
{
    public class Controller
    {
        public ActionElementMap[] maps { get { return new ActionElementMap[0]; } }
        public Rewired.ControllerType type { get { return Rewired.ControllerType.Unknown; } }
        public System.Guid hardwareTypeGuid { get { return System.Guid.Empty; } }
    }

}

// ===== 批量桩代码 =====

// Controller 数组扩展
namespace Rewired
{
    public static partial class ControllerArrayExtensions
    {
        public static void AddController(this Controller[] controllers, Controller controller, bool removeFromOtherPlayers = false) { }
        public static int joystickCount(this Controller[] controllers) { return 0; }
        public static Joystick[] Joysticks(this Controller[] controllers) { return new Joystick[0]; }
    }

    public partial class ControllerManager
    {
        public int joystickCount { get { return 0; } }
        public Joystick[] Joysticks { get { return new Joystick[0]; } }
    }

    public class ControllerStatusChangedEventArgs
    {
        public Controller controller { get; set; }
        public ControllerType controllerType { get; set; }
    }

    public partial class Player
    {
        public class ControllerHelper
    {
        public ActionElementMap[] maps { get { return new ActionElementMap[0]; } }
        public Joystick[] Joysticks { get { return new Joystick[0]; } }
        public int joystickCount { get { return 0; } }
        public void AddController(object controller, bool removeFromOtherPlayers = false) { }
        public void RemoveController(object controller) { }
        public void RemoveController(Controller controller) { }
    }
    }

    public static partial class ReInput
    {
        public static event System.Action<ControllerStatusChangedEventArgs> ControllerDisconnectedEvent;
    }
}

// Ink.Runtime.Story / VariablesState / Choice 由 Ink-Libraries.dll 提供

// Localization 补充
namespace Rewired
{

    public static class LocalizationExtensions
    {
        public static string GetLocalizedString(this object localization, string key) { return key; }
        public static void SetLanguage(this object localization, string lang) { }
    }

}

// SA.GoogleDoc.LangCode 由真实程序集提供，不做桩


// RDPluginsUtils
public static class RDPluginsUtils
{
    public static string externalDialoguesDataPath { get { return ""; } }
    public static Ink.Runtime.Story CompileInk(string path) { return null; }
}

// Immutable 集合扩展
public static class ImmutableExtensions
{
    public static System.Collections.Immutable.ImmutableHashSet<T> ToImmutableHashSet<T>(this IEnumerable<T> source) { return null; }
    public static System.Collections.Immutable.ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) { return null; }
}




// SteamIntegration 桩
public static partial class SteamIntegration
{
    public static bool IsSteamInBigPictureMode => false;
    public static bool IsSteamRunning() { return false; }
    public static void UnlockAchievement(string id) { }
    public static void UnlockAchievement(string id, bool force) { }
    public static void UnlockAchievement(object achievement, bool storeStats) { }
    public static void StoreStats() { }
    public static void Setup() { }
    public static void OpenConnection() { }
    public static ulong userSteamID { get { return 0; } }
    public static void CheckCallbacks() { }
}

// SteamId 补充

// CustomWindow 桩
public class CustomWindow : MonoBehaviour
{
    public static CustomWindow Instance { get; set; }
    public void SetSize(int width, int height) { }
    public void SetPosition(int x, int y) { }
    public void Move(int x, int y) { }
}

// SteamIntegration 补充
public static partial class SteamIntegration
{
    public static bool initialized { get; set; }
}

// WindowChoreographer 补充

// Controller 补充
public partial class Controller
{
    public object maps { get { return null; } }
    public Rewired.ControllerType type { get; set; }
    public System.Guid hardwareTypeGuid { get; set; }
}

// Localization 补充
public static partial class Localization
{
    public static string GetLocalizedString(string key) { return key; }
    public static string GetLocalizedString(string key, params object[] args) { return key; }
    public static List<SystemLanguage> AvailableLanguages { get { return new List<SystemLanguage>(); } }
    public static List<SystemLanguage> AlphaOnlyLanguages { get { return new List<SystemLanguage>(); } }
    public static object CurrentLanguage { get { return null; } set { } }
    public static void SetLanguage(object lang) { }
    public static bool ExistsLocalizedString(string key) { return true; }
    public static string DefaultSection { get { return ""; } }
    public static string GetLocalizedString(string key, string section, object lang) { return key; }
}

// Reader 补充 - externalDialoguesDataPath 可写
public static partial class Reader
{
    public static string externalDialoguesDataPath { get { return ""; } set { } }
    public static string externalLocalizationDataPath { get { return ""; } set { } }
    public static string localizationPassword { get { return ""; } set { } }
}

// UnicodeRanges 补充 - 已存在，扩展成员
namespace System.Text.Unicode
{
    public static partial class UnicodeRanges
    {
        public static UnicodeRange CjkUnifiedIdeographs { get { return new UnicodeRange("", 0x4E00, 0x9FFF); } }
        public static UnicodeRange CjkUnifiedIdeographsExtensionA { get { return new UnicodeRange("", 0x3400, 0x4DBF); } }
        public static UnicodeRange CjkCompatibilityIdeographs { get { return new UnicodeRange("", 0xF900, 0xFAFF); } }
        public static UnicodeRange HangulCompatibilityJamo { get { return new UnicodeRange("", 0x3130, 0x318F); } }
        public static UnicodeRange HangulSyllables { get { return new UnicodeRange("", 0xAC00, 0xD7AF); } }
        public static UnicodeRange HangulJamo { get { return new UnicodeRange("", 0x1100, 0x11FF); } }
        public static UnicodeRange HangulJamoExtendedA { get { return new UnicodeRange("", 0xA960, 0xA97F); } }
        public static UnicodeRange HangulJamoExtendedB { get { return new UnicodeRange("", 0xD7B0, 0xD7FF); } }
        public static UnicodeRange Katakana { get { return new UnicodeRange("", 0x30A0, 0x30FF); } }
        public static UnicodeRange KatakanaPhoneticExtensions { get { return new UnicodeRange("", 0x30FB, 0x30FF); } }
        public static UnicodeRange Hiragana { get { return new UnicodeRange("", 0x3040, 0x309F); } }
        public static UnicodeRange HalfWidthKatakana { get { return new UnicodeRange("", 0xFF65, 0xFF9F); } }
    }
}

// Controller 数组扩展
public static partial class ControllerArrayExtensions
{
    public static object maps { get { return null; } }
    public static Controller[] Joysticks(this Controller[] controllers) { return controllers; }
}


// SteamWorkshop 补充
public static partial class SteamWorkshop
{
    public static bool OverlayEnabled() { return false; }
    public static int ItemsSubscribed { get { return 0; } }
    public static int itemsUnsubscribed { get { return 0; } set { } }
    public static int ItemsInstalled { get { return 0; } }
    public static void Initialize() { }
    public static void OpenWorkshop() { }
    public static bool isDownloadingItem { get { return false; } }
    public static object[] GetLocalItems() { return new object[0]; }
    public static void ClearItemsInfoCache() { }
    public static void CheckDownloadInfo() { }
}


// FileBrowser 补充
public static partial class FileBrowser
{
    public static string PickFile(string title, string directory, string extension) { return ""; }
}

// SystemSounds 桩
namespace System.Media
{
    public class SystemSounds
    {
        public static SystemSound Asterisk { get { return new SystemSound(); } }
        public static SystemSound Beep { get { return new SystemSound(); } }
        public static SystemSound Hand { get { return new SystemSound(); } }
        public static SystemSound Question { get { return new SystemSound(); } }
        public static SystemSound Exclamation { get { return new SystemSound(); } }
    }
    public class SystemSound
    {
        public void Play() { }
    }
}


// RDClassDll 补充
public partial class RDClassDll
{
    public static void printesw(object message) { }
}

// LocalizationManager 补充
public partial class LocalizationManager
{
    public object localizedStringProvider { get { return null; } }
}

// FileBrowser 补充
public static partial class FileBrowser
{
    public static string PickFile(string title, string directory, string extension, bool multiselect) { return ""; }
}


// Reader 补充已在上方定义

// SteamWorkshop 补充
public static partial class SteamWorkshop
{
    public static void ShowItemOnWorkshop(ulong itemId) { }
    public static void UnsubscribeItem(ulong itemId) { }
}



// SteamWorkshop 补充
public static partial class SteamWorkshop
{
    public static string GetCurrentDownloadingItemTitle { get { return ""; } }
}

// FileBrowser 补充
public static partial class FileBrowser
{
    public static string[] PickFiles(string title, string directory, string extension, bool multiselect) { return new string[0]; }
}

// CustomLevelSyringe 补充
public partial class CustomLevelSyringe
{
    public void DOKill(bool complete = false) { }
}

// ControllerHelper 定义
namespace Rewired
{
    public class ControllerHelper
    {
        public Player.ControllerHelper controllers { get { return new Player.ControllerHelper(); } }
        public Controller[] Controllers { get { return new Controller[0]; } }
        public Joystick[] Joysticks { get { return new Joystick[0]; } }
    }
}



// ControllerArrayExtensions 补充
namespace Rewired
{
    public static partial class ControllerArrayExtensions
    {
    }
}

// FileBrowser 补充
public static partial class FileBrowser
{
    public static string[] PickFiles(string title, string directory, string[] extensions, bool multiselect) { return new string[0]; }
    public static string PickFile(string title, string directory, string[] extensions, bool multiselect) { return ""; }
}

// scrChar 补充
public partial class scrChar
{
    public void DOKill(bool complete = false) { }
}

// Controller[] 扩展
namespace Rewired
{
    public static partial class ControllerArrayExtensions
    {
    }
}

// LocalizationManager 补充
public partial class LocalizationManager
{
}

// FileBrowser 补充
public static partial class FileBrowser
{
    public static string PickFile(string title, string directory, string extension, string defaultName) { return ""; }
    public static string[] PickFiles(string title, string directory, string extension, string defaultName) { return new string[0]; }
}

// ControllerArrayExtensions 补充
namespace Rewired
{
    public static partial class ControllerArrayExtensions
    {
    }
}

// SteamId 补充
namespace Steamworks
{
    public partial class SteamId
    {
        public ulong m_SteamID { get { return 0; } }
    }
    
}

// Controller[] 扩展
namespace Rewired
{
    public static partial class ControllerArrayExtensions
    {
    }
}

// Player maps 属性
namespace Rewired
{
    public partial class Player
    {
        public PlayerMapStore maps { get { return new PlayerMapStore(); } }
    }

    public class PlayerMapStore
    {
        public void SetMapsEnabled(bool enabled, Rewired.ControllerType type, int categoryId, int layoutId) { }
    }
}

// PlayerControllerHelper 类
namespace Rewired
{
    public class PlayerControllerHelper
    {
        public PlayerMapStore maps { get { return new PlayerMapStore(); } }
        public Controller[] Controllers { get { return new Controller[0]; } }
        public Joystick[] Joysticks { get { return new Joystick[0]; } }
        public int joystickCount { get { return 0; } }
        public void AddController(object controller, bool removeFromOtherPlayers = false) { }
        public void RemoveController(object controller) { }
    }
}

// FileBrowser 补充
public static partial class FileBrowser
{
    public static string PickFile(string directory, string extension, string[] filters, string title) { return ""; }
    public static string[] PickFiles(string directory, string extension, string[] filters, string title) { return new string[0]; }
    public static bool SaveFile(string title, string directory, string defaultName, string extension) { return false; }
    public static bool SaveFile(string title, string directory, string defaultName, string[] extensions) { return false; }
}

public static partial class FileBrowser
{
    public static string SaveFile(string title, string directory, string defaultName, string extension, bool multiselect) { return ""; }
}

public static class ImmutableExtensions2
{
    public static System.Collections.Immutable.ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector) { return System.Collections.Immutable.ImmutableDictionary<TKey, TValue>.Empty; }
}

public static class ExtensionAssociation
{
    public static string GetAssociatedExtension(string extension) { return extension; }
    public static string CheckForFileExtension(string path) { return ""; }
    public static void CheckExtensionAssociation() { }
}

public static partial class FileBrowser
{
    public static string SaveFile(string directory, string defaultName, string extension, string[] filters, string title) { return ""; }
}

public static class SteamIntegrationStub
{
    public static void UnlockAchievement(string id) { }
    public static void UnlockAchievement(string id, bool force) { }
    public static void UnlockAchievement(object achievement, bool storeStats) { }
}

public class Bitmap : System.IDisposable { public Bitmap() { } public Bitmap(string path) { } public Bitmap(int w) { } public Bitmap(int w, int h) { } public Bitmap(int w, int h, int format) { } public int Width { get; set; } public int Height { get; set; } public void Dispose() { } }

public static partial class RDBaseDll
{
    public static void printes(string s) { }
}

public static class ActionElementMapExtensions
{
    public static void SetMapsEnabled(this object[] maps, bool enabled, Rewired.ControllerType type, int categoryId, int layoutId) { }
}

public static class DictContainsExt
{
    public static bool Contains<K, V>(this Dictionary<K, V> dict, K key) { return dict.ContainsKey(key); }
}




public static class ImmutableExt
{
    public static System.Collections.Immutable.ImmutableList<T> ToImmutableList<T>(this IEnumerable<T> source) { return System.Collections.Immutable.ImmutableList<T>.Empty; }
    public static System.Collections.Immutable.ImmutableArray<T> ToImmutableArray<T>(this IEnumerable<T> source) { return System.Collections.Immutable.ImmutableArray<T>.Empty; }
}

public static class ImmutableListExt
{
    public static T Find<T>(this System.Collections.Immutable.ImmutableList<T> list, System.Predicate<T> match) { return default(T); }
}

public static class ImmutableDictExt
{
    public static IEnumerable<TValue> Values<TKey, TValue>(this System.Collections.Immutable.ImmutableDictionary<TKey, TValue> dict) { yield break; }
}

public static class GameObjectExt
{
    public static GameObject tip;
}


// API 桩
public static class API
{
    public static void RetrievePublicSheetData(string sheetId, bool drawProgressBar, bool flag, int page) { }
}
