using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_WEBGL
public class PlatformHelperWindows : PlatformHelper
{
    public Vector2Int GetCurrentMonitorResolution(IntPtr ptr) { return new Vector2Int(1920, 1080); }
    protected override List<Monitor> GetMonitors() { return new List<Monitor>(); }
    public override Vector2Int GetWindowDanceResolution() { return new Vector2Int(1920, 1080); }
}

public class PlatformHelperMac : PlatformHelper
{
    public Vector2Int GetCurrentMonitorResolution(IntPtr ptr) { return new Vector2Int(1920, 1080); }
    protected override List<Monitor> GetMonitors() { return new List<Monitor>(); }
    public override Vector2Int GetWindowDanceResolution() { return new Vector2Int(1920, 1080); }
}
#endif
