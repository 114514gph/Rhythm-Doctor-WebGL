using System;
using UnityEngine;

namespace Rewired
{
    // 控制器模板接口
    public interface IControllerTemplate { }
    public interface IControllerTemplateElement { }
    public interface IControllerTemplateButton : IControllerTemplateElement { }
    public interface IControllerTemplateAxis : IControllerTemplateElement { }
    public interface IControllerTemplateHat : IControllerTemplateElement { }
    public interface IControllerTemplateStick : IControllerTemplateElement { }
    public interface IControllerTemplateStick6D : IControllerTemplateElement { }
    public interface IControllerTemplateThrottle : IControllerTemplateElement { }
    public interface IControllerTemplateElementTarget { }
    
    // 控制器模板
    public interface IControllerTemplate<TSelf> : IControllerTemplate where TSelf : IControllerTemplate<TSelf> { }
    
    // 输入管理器基类
    
    // 外部工具接口
    public interface IExternalTools { }
    
    // 平台相关
    
    // 控制器相关
    public enum ControllerType { Joystick, Keyboard, Mouse, Custom, Gamepad, RacingWheel, FlightYoke, HOTAS, SixDof, DancePad, Guitar, DrumKit, ArcadePad, Unknown }
    public class ControllerIdentifier { }
    public class ControllerMap { }
    public class ControllerStatusChangedEventArgs : EventArgs { }
    public class InputActionEventData { }
    public class InputBehavior { }
    public class Player { }
    public class PlayerSaveData { }
    public class ControllerMapSaveData { }
    public class Joystick : Controller { }
    public interface IMouseInputSource { }
    public interface ITouchInputSource { }
    
    // 模板工厂
    public static class ControllerTemplateFactory
    {
        public static T CreateTemplate<T>(Controller controller) where T : IControllerTemplate { return default(T); }
    }
}

// 确保所有命名空间存在
namespace Rewired.Utils.Interfaces { }
namespace Rewired.Utils.Platforms { }
namespace Rewired.Utils.Libraries.TinyJson { }
namespace Rewired.Dev { }
namespace Rewired.Interfaces { }
namespace Rewired.Platforms { }

namespace Rewired
{
    public static class ControllerTypeExtensions
    {
        public static bool Equals(this ControllerType a, ControllerType b) { return a == b; }
    }
}
