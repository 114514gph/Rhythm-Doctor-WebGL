using System;
using UnityEngine;
using System.Collections.Generic;

// DOTween Pro 缺失选项
namespace DG.Tweening.Plugins.Options
{
    public struct CircleOptions { public bool snapping; public CircleOptions(bool snapping) { this.snapping = snapping; } }
}

namespace DG.Tweening
{
    public enum SpiralMode { Expand, ExpandAbsolute, Fixed }
}

// Ink 剧情引擎桩


namespace Ink.Runtime
{
    public class Story
    {
        public Story(string content) { }
        public string Continue() { return ""; }
        public bool canContinue { get { return false; } }
        public void ChooseChoiceIndex(int index) { }
        public System.Collections.Generic.List<Choice> currentChoices { get { return new System.Collections.Generic.List<Choice>(); } }
        public void ResetState() { }
        public void ResetErrors() { }
        public string stateToJson() { return ""; }
        public void LoadState(string json) { }
        public System.Collections.Generic.Dictionary<string, object> variablesState { get { return new System.Collections.Generic.Dictionary<string, object>(); } }
        public void ObserveVariable(string name, VariableObserver observer) { }
        public delegate void VariableObserver(string name, object newValue);
    }
    public class Choice
    {
        public string text;
        public int index;
    }
}

namespace Ink
{
    public class Compiler
    {
        public Compiler(string content) { }
        public Ink.Runtime.Story Compile() { return new Ink.Runtime.Story(""); }
    }
}

// Rewired 缺失类型
namespace Rewired.Utils.Libraries
{
    public static class JSONParser { }
    public static class Json { }
}

namespace Rewired
{
    
    public enum ControllerElementType { Button, Axis, Element6D }
    public enum Pole { Positive, Negative }
    
    public class ElementAssignment
    {
        public int elementId;
        public int elementMapId;
        public ControllerElementType elementType;
        public Pole pole;
        public ElementAssignment() { }
        public ElementAssignment(int elementId, int elementMapId, ControllerElementType elementType, Pole pole)
        {
            this.elementId = elementId;
            this.elementMapId = elementMapId;
            this.elementType = elementType;
            this.pole = pole;
        }
    }
    
    public class Controller
    {
        public class Element { }
    }
    
    public class InputAction
    {
        public int id;
        public string name;
        public InputAction(int id, string name) { this.id = id; this.name = name; }
    }
    
    public class ActionElementMap
    {
        public int elementId;
        public int actionId;
        public ControllerElementType elementType;
        public Pole pole;
    }
}

// DoNotSerialize 属性
public class DoNotSerializeAttribute : Attribute { }

// DOTween 更多扩展
namespace DG.Tweening
{
    public static class DOTweenExtensions
    {
        public static Tween DOPlay(this Tween t) { return t; }
        public static Tween DOPause(this Tween t) { return t; }
        public static Tween DORestart(this Tween t) { return t; }
        public static Tween DORewind(this Tween t) { return t; }
        public static Tween DOComplete(this Tween t) { return t; }
        public static Tween DOKill(this Tween t) { return t; }
        public static Tween DOFlip(this Tween t) { return t; }
    }
}

// Rewired 更多缺失类型
namespace Rewired.Utils.Libraries
{
    public static class CLZF2
    {
        public static byte[] Compress(byte[] input) { return input; }
        public static byte[] Decompress(byte[] input) { return input; }
    }
}

namespace Rewired.Utils.Attributes
{
    public class PreserveAttribute : Attribute { }
}

// 完善 UserDataStore 基类
namespace Rewired
{
    public abstract class UserDataStore
    {
        public abstract void Save();
        public abstract void Load();
        public virtual void SaveControllerData(int playerId, ControllerType controllerType, int controllerId) { }
        public virtual void SaveControllerData(ControllerType controllerType, int controllerId) { }
        public virtual void SavePlayerData(int playerId) { }
        public virtual void SaveInputBehavior(int playerId, int behaviorId) { }
        public virtual void LoadControllerData(int playerId, ControllerType controllerType, int controllerId) { }
        public virtual void LoadControllerData(ControllerType controllerType, int controllerId) { }
        public virtual void LoadPlayerData(int playerId) { }
        public virtual void LoadInputBehavior(int playerId, int behaviorId) { }
        public virtual void OnInitialize() { }
        public virtual void OnControllerConnected(ControllerStatusChangedEventArgs e) { }
        public virtual void OnControllerPreDisconnect(ControllerStatusChangedEventArgs e) { }
        public virtual void OnControllerDisconnected(ControllerStatusChangedEventArgs e) { }
        public virtual void SaveControllerMap(int playerId, ControllerMap map) { }
    }
}

// 更多缺失类型
namespace Rewired.Utils
{
    namespace Interfaces { }
    namespace Platforms { }
}

namespace Rewired
{
    public interface IControllerTemplateYoke : IControllerTemplateElement { }
    public interface IControllerTemplateDPad : IControllerTemplateElement { }
    
    public class InputManager_Base : MonoBehaviour
    {
        protected virtual void OnInitialized() { }
        protected virtual void OnDeinitialized() { }
        protected virtual void DetectPlatform() { }
        protected virtual void CheckRecompile() { }
        protected virtual IExternalTools GetExternalTools() { return null; }
    }
}

// System.Text.Json 桩
namespace System.Text.Json
{
    public static class JsonSerializer
    {
        public static string Serialize(object obj) { return ""; }
        public static T Deserialize<T>(string json) { return default(T); }
    }
}

// MultiWindow.Window
namespace MultiWindow
{
    public class Window
    {
        public int id;
        public string title;
        public int width;
        public int height;
        public Window(string title, int width, int height) { this.title = title; this.width = width; this.height = height; }
        public void Show() { }
        public void Hide() { }
        public void Close() { }
    }
}

// MultiWindow 补充
namespace MultiWindow
{
    public class Icon { }
    public class BaseWindow
    {
        public int id;
        public string title;
        public virtual void Show() { }
        public virtual void Hide() { }
        public virtual void Close() { }
    }
    namespace Events { }
}

// Newtonsoft 桩
namespace Newtonsoft.Json
{
    public static class JsonConvert
    {
        public static string SerializeObject(object obj) { return ""; }
        public static T DeserializeObject<T>(string json) { return default(T); }
    }
    public class JsonPropertyAttribute : Attribute
    {
        public string PropertyName;
        public JsonPropertyAttribute(string propertyName) { PropertyName = propertyName; }
    }
}

// System.Collections.Immutable 桩
namespace System.Collections.Immutable
{
    public static class ImmutableHashSet
    {
        public static ImmutableHashSet<T> Create<T>() { return new ImmutableHashSet<T>(); }
        public static ImmutableHashSet<T> CreateRange<T>(IEnumerable<T> items) { return new ImmutableHashSet<T>(); }
    }
    public class ImmutableHashSet<T> : IEnumerable<T>
    {
        public ImmutableHashSet<T> Add(T item) { return this; }
        public ImmutableHashSet<T> Remove(T item) { return this; }
        public bool Contains(T item) { return false; }
        public IEnumerator<T> GetEnumerator() { yield break; }
        IEnumerator IEnumerable.GetEnumerator() { yield break; }
    }
}
namespace MultiWindow { namespace Events { } }

namespace Rewired.Utils
{
    namespace Interfaces { }
    namespace Platforms { }
}

// Rewired 更多命名空间
namespace Rewired.Dev { }
namespace Rewired.Interfaces { }

// 确保 Rewired.Utils 命名空间存在
namespace Rewired.Utils
{
    namespace Interfaces { }
    namespace Platforms { }
}
