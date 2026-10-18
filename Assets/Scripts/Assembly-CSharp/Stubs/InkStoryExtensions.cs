using System;
using Ink.Runtime;

// Ink.Runtime.Story 扩展方法 - 真实 DLL 缺少 BindExternalFunction
namespace Ink.Runtime
{
    public static class StoryExtensions
    {
        public static void BindExternalFunction(this Story story, string funcName, Action func) { }
        public static void BindExternalFunction<T1>(this Story story, string funcName, Action<T1> func) { }
        public static void BindExternalFunction<T1, T2>(this Story story, string funcName, Action<T1, T2> func) { }
        public static void BindExternalFunction<T1, T2, T3>(this Story story, string funcName, Action<T1, T2, T3> func) { }
        public static void BindExternalFunction<T1, T2, T3, T4>(this Story story, string funcName, Action<T1, T2, T3, T4> func) { }
        public static void BindExternalFunction(this Story story, string funcName, Func<object> func) { }
        public static void BindExternalFunction<T1>(this Story story, string funcName, Func<T1, object> func) { }
        public static void BindExternalFunction<T1, T2>(this Story story, string funcName, Func<T1, T2, object> func) { }
        public static void BindExternalFunctionGeneral(this Story story, string funcName, Func<object[], object> func) { }
        public static bool HasFunction(this Story story, string name) { return false; }
        public static object ChoosePathString(this Story story, string path) { return null; }
        public static object ChoosePathString(this Story story, string path, object resetCallStack) { return null; }
    }
}
