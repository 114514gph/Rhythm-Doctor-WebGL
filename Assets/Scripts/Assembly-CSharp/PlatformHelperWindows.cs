#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using MultiWindow;
using UnityEngine;

public class PlatformHelperWindows : PlatformHelper
{
	public struct POINT
	{
		public int x;

		public int y;
	}

	public enum DMDO : uint
	{
		DMDO_DEFAULT = 0u,
		DMDO_90 = 1u,
		DMDO_180 = 2u,
		DMDO_270 = 3u
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DEVMODE
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string dmDeviceName;

		public ushort dmSpecVersion;

		public ushort dmDriverVersion;

		public ushort dmSize;

		public ushort dmDriverExtra;

		public uint dmFields;

		public POINT dmPosition;

		public DMDO dmDisplayOrientation;

		public uint dmDisplayFixedOutput;

		public ushort dmColor;

		public short dmDuplex;

		public short dmYResolution;

		public short dmTTOption;

		public short dmCollate;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string dmFormName;

		public ushort dmLogPixels;

		public uint dmBitsPerPel;

		public uint dmPelsWidth;

		public uint dmPelsHeight;

		public uint dmDisplayFlags;

		public uint dmDisplayFrequency;

		public uint dmICMMethod;

		public uint dmICMIntent;

		public uint dmMediaType;

		public uint dmDitherType;

		public uint dmReserved1;

		public uint dmReserved2;

		public uint dmPanningWidth;

		public uint dmPanningHeight;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct MonitorInfoEx
	{
		public int Size;

		public Rectangle Monitor;

		public Rectangle WorkArea;

		public uint Flags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string DeviceName;

		public void Init()
		{
			Size = 104;
			DeviceName = string.Empty;
		}
	}

	public enum DpiType
	{
		Effective = 0,
		Angular = 1,
		Raw = 2
	}

	public static class SWP
	{
		public static readonly int NOSIZE = 1;

		public static readonly int NOMOVE = 2;

		public static readonly int NOZORDER = 4;

		public static readonly int NOREDRAW = 8;

		public static readonly int NOACTIVATE = 16;

		public static readonly int DRAWFRAME = 32;

		public static readonly int FRAMECHANGED = 32;

		public static readonly int SHOWWINDOW = 64;

		public static readonly int HIDEWINDOW = 128;

		public static readonly int NOCOPYBITS = 256;

		public static readonly int NOOWNERZORDER = 512;

		public static readonly int NOREPOSITION = 512;

		public static readonly int NOSENDCHANGING = 1024;

		public static readonly int DEFERERASE = 8192;

		public static readonly int ASYNCWINDOWPOS = 16384;
	}

	public enum GetWindowType
	{
		GW_HWNDFIRST = 0,
		GW_HWNDLAST = 1,
		GW_HWNDNEXT = 2,
		GW_HWNDPREV = 3,
		GW_OWNER = 4,
		GW_CHILD = 5,
		GW_ENABLEDPOPUP = 6
	}

	public enum DeviceCap
	{
		DRIVERVERSION = 0,
		TECHNOLOGY = 2,
		HORZSIZE = 4,
		VERTSIZE = 6,
		HORZRES = 8,
		VERTRES = 10,
		BITSPIXEL = 12,
		PLANES = 14,
		NUMBRUSHES = 16,
		NUMPENS = 18,
		NUMMARKERS = 20,
		NUMFONTS = 22,
		NUMCOLORS = 24,
		PDEVICESIZE = 26,
		CURVECAPS = 28,
		LINECAPS = 30,
		POLYGONALCAPS = 32,
		TEXTCAPS = 34,
		CLIPCAPS = 36,
		RASTERCAPS = 38,
		ASPECTX = 40,
		ASPECTY = 42,
		ASPECTXY = 44,
		SHADEBLENDCAPS = 45,
		LOGPIXELSX = 88,
		LOGPIXELSY = 90,
		SIZEPALETTE = 104,
		NUMRESERVED = 106,
		COLORRES = 108,
		PHYSICALWIDTH = 110,
		PHYSICALHEIGHT = 111,
		PHYSICALOFFSETX = 112,
		PHYSICALOFFSETY = 113,
		SCALINGFACTORX = 114,
		SCALINGFACTORY = 115,
		VREFRESH = 116,
		DESKTOPVERTRES = 117,
		DESKTOPHORZRES = 118,
		BLTALIGNMENT = 119
	}

	private enum ShowWindowCommands
	{
		Hide = 0,
		Normal = 1,
		ShowMinimized = 2,
		Maximize = 3,
		ShowMaximized = Maximize,
		ShowNoActivate = 4,
		Show = 5,
		Minimize = 6,
		ShowMinNoActive = 7,
		ShowNA = 8,
		Restore = 9,
		ShowDefault = 10,
		ForceMinimize = 11
	}

	public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	public delegate bool MonitorEnumProc(IntPtr hDesktop, IntPtr hdc, ref Rectangle pRect, int dwData);

	private const int DefaultDpi = 96;

	private IntPtr taskbarHandle;

	private Rectangle windowDanceResolutionRect;

	private bool hasCheckMultiMonitorSupport;

	private bool _supportsMultiMonitor;

	private List<Rectangle> monitorRects;

	private MonitorEnumProc enumWindowsCallback;

	private const int ENUM_CURRENT_SETTINGS = -1;

	private const int ENUM_REGISTRY_SETTINGS = -2;

	private const int CCHDEVICENAME = 32;

	private bool SupportsMultiMonitor
	{
		get
		{
			if (!hasCheckMultiMonitorSupport)
			{
				hasCheckMultiMonitorSupport = true;
				string operatingSystem = SystemInfo.operatingSystem;
				_supportsMultiMonitor = operatingSystem.Contains("Windows 10") || operatingSystem.Contains("Windows 8.1") || operatingSystem.Contains("Windows 11");
			}
			return _supportsMultiMonitor;
		}
	}

	public PlatformHelperWindows()
	{
		playerWindow = new UnityPlayerWindowWindows(-1, null);
	}

	public override Vector2Int GetWindowDanceResolution()
	{
		windowDanceResolutionRect = GetMonitorsRect(base.Monitors, onlyHorizontal: false);
		return new Vector2Int(windowDanceResolutionRect.Right - windowDanceResolutionRect.Left, windowDanceResolutionRect.Bottom - windowDanceResolutionRect.Top);
	}

	public Vector2Int GetBottomLeftDesktopPoint()
	{
		Vector2Int result = new Vector2Int(windowDanceResolutionRect.Left, windowDanceResolutionRect.Top);
		foreach (Monitor monitor in base.Monitors)
		{
			if (monitor.bounds.Left > windowDanceResolutionRect.Left && monitor.bounds.Left < windowDanceResolutionRect.Right && monitor.bounds.Top > windowDanceResolutionRect.Top && monitor.bounds.Top < windowDanceResolutionRect.Bottom)
			{
				if (monitor.bounds.Left < result.x)
				{
					result.x = monitor.bounds.Left;
				}
				if (monitor.bounds.Top > result.y)
				{
					result.y = monitor.bounds.Top;
				}
			}
		}
		return result;
	}

	public bool IsCurrentlyFocused(IntPtr windowHandle)
	{
		return windowHandle == GetForegroundWindow();
	}

	protected override List<Monitor> GetMonitors()
	{
		UpdateCurrentMonitorsRect(SupportsMultiMonitor);
		List<Monitor> list = new List<Monitor>();
		if (!SupportsMultiMonitor || monitorRects.Count == 1)
		{
			list.Add(new Monitor(monitorRects[0], 100));
			currentMonitorIndex = 0;
			return list;
		}
		GetWindowRect(PlayerWindow.Instance.Hwnd, out var lpRect);
		Vector2Int vector2Int = new Vector2Int((lpRect.Left + lpRect.Right) / 2, (lpRect.Top + lpRect.Bottom) / 2);
		currentMonitorIndex = 0;
		foreach (Rectangle monitorRect in monitorRects)
		{
			if (vector2Int.y >= monitorRect.Top && vector2Int.y <= monitorRect.Bottom && vector2Int.x >= monitorRect.Left && vector2Int.x <= monitorRect.Right)
			{
				break;
			}
			currentMonitorIndex++;
		}
		if (currentMonitorIndex > monitorRects.Count - 1)
		{
			currentMonitorIndex = 0;
		}
		if (RDC.windowMovement == WindowMovement.OneScreen)
		{
			list.Add(new Monitor(monitorRects[currentMonitorIndex], 100));
			return list;
		}
		List<Monitor> list2 = new List<Monitor>();
		foreach (Rectangle monitorRect2 in monitorRects)
		{
			IntPtr hmonitor = MonitorFromPoint(new Point(monitorRect2.Left + 1, monitorRect2.Top + 1), 2u);
			uint dpiX = 96u;
			uint dpiY = 96u;
			GetDpiForMonitor(hmonitor, DpiType.Effective, out dpiX, out dpiY);
			int scale = (int)(1f * (float)dpiX / 96f * 100f);
			Monitor item = new Monitor(monitorRect2, scale);
			list2.Add(item);
		}
		list = new List<Monitor>();
		int scale2 = list2[currentMonitorIndex].scale;
		int num = 0;
		foreach (Monitor item2 in list2)
		{
			if (item2.scale == scale2)
			{
				list.Add(item2);
				if (num == currentMonitorIndex)
				{
					currentMonitorIndex = list.Count - 1;
				}
			}
			num++;
		}
		if (currentMonitorIndex > list.Count - 1)
		{
			currentMonitorIndex = list.Count - 1;
		}
		return list;
	}

	public Resolution GetCurrentMonitorResolution(IntPtr windowHandle)
	{
		if (windowHandle == IntPtr.Zero)
		{
			windowHandle = PlayerWindow.Instance.Hwnd;
		}
		UpdateCurrentMonitorsRect(SupportsMultiMonitor);
		Vector2Int windowCenter = GetWindowCenter(windowHandle);
		Rectangle rectangle = monitorRects[0];
		foreach (Rectangle monitorRect in monitorRects)
		{
			if (windowCenter.x >= monitorRect.Left && windowCenter.x <= monitorRect.Right && windowCenter.y >= monitorRect.Top && windowCenter.y <= monitorRect.Bottom)
			{
				rectangle = monitorRect;
				break;
			}
		}
		return new Resolution
		{
			width = rectangle.Right - rectangle.Left,
			height = rectangle.Bottom - rectangle.Top
		};
	}

	public Vector2Int GetWindowSize(IntPtr windowHandle)
	{
		GetWindowRect(windowHandle, out var lpRect);
		return new Vector2Int(lpRect.Right - lpRect.Left, lpRect.Bottom - lpRect.Top);
	}

	public bool IsInWindowDanceRect(IntPtr wHandle)
	{
		Vector2Int windowCenter = GetWindowCenter(wHandle);
		Vector2Int windowSize = GetWindowSize(wHandle);
		if (windowCenter.x - windowSize.x / 2 > windowDanceResolutionRect.Left && windowCenter.x + windowSize.x / 2 < windowDanceResolutionRect.Right && windowCenter.y - windowSize.y / 2 > windowDanceResolutionRect.Top)
		{
			return windowCenter.y + windowSize.y / 2 < windowDanceResolutionRect.Bottom;
		}
		return false;
	}

	public Vector2Int GetWindowPosition(IntPtr wHandle)
	{
		GetWindowRect(wHandle, out var lpRect);
		int y = base.WindowDanceResolution.y;
		Vector2Int bottomLeftDesktopPoint = GetBottomLeftDesktopPoint();
		return new Vector2Int(lpRect.Left - bottomLeftDesktopPoint.x, y - (lpRect.Bottom - bottomLeftDesktopPoint.y));
	}

	public Vector2Int GetWindowCenter(IntPtr wHandle)
	{
		GetWindowRect(wHandle, out var lpRect);
		return new Vector2((float)(lpRect.Left + lpRect.Right) / 2f, (float)(lpRect.Top + lpRect.Bottom) / 2f).RoundToInt();
	}

	public void UpdateCurrentMonitorsRect(bool allMonitors)
	{
		monitorRects = new List<Rectangle>();
		if (enumWindowsCallback == null)
		{
			enumWindowsCallback = delegate(IntPtr hDesktop, IntPtr hdc, ref Rectangle pRect, int d)
			{
				monitorRects.Add(pRect);
				return allMonitors;
			};
		}
		EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, enumWindowsCallback, 0);
	}

	public int GetCurrentMonitor(IntPtr windowHandle)
	{
		UpdateCurrentMonitorsRect(SupportsMultiMonitor);
		Vector2Int windowCenter = GetWindowCenter(windowHandle);
		int num = 0;
		foreach (Rectangle monitorRect in monitorRects)
		{
			if (windowCenter.y >= monitorRect.Top && windowCenter.y <= monitorRect.Bottom && windowCenter.x >= monitorRect.Left && windowCenter.x <= monitorRect.Right)
			{
				break;
			}
			num++;
		}
		if (num > monitorRects.Count - 1)
		{
			num = 0;
		}
		return num;
	}

	public Vector2Int GetMonitorCenter(int monitorIndex)
	{
		if (monitorIndex > monitorRects.Count - 1)
		{
			monitorIndex = 0;
		}
		Rectangle rectangle = monitorRects[monitorIndex];
		return new Vector2Int((rectangle.Right + rectangle.Left) / 2, (rectangle.Bottom + rectangle.Top) / 2) - GetBottomLeftDesktopPoint();
	}

	public Vector2Int GetWindowMonitorCenter(IntPtr windowHandle)
	{
		return GetMonitorCenter(GetCurrentMonitor(windowHandle));
	}

	public bool IsMinimized(IntPtr handle)
	{
		return IsIconic(handle);
	}

	public Vector2Int GetCorner(Func<Vector2Int, Vector2, bool> cornerCompare)
	{
		List<Vector2Int> monitorsVertices = GetMonitorsVertices(base.Monitors);
		float num = float.MaxValue;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		float num4 = float.MinValue;
		foreach (Vector2Int item in monitorsVertices)
		{
			Vector2 vector = item;
			if (vector.x < num)
			{
				num = vector.x;
			}
			if (vector.x > num2)
			{
				num2 = vector.x;
			}
			if (vector.y < num3)
			{
				num3 = vector.y;
			}
			if (vector.y > num4)
			{
				num4 = vector.y;
			}
		}
		Vector2 vector2 = new Vector2(num + (num2 - num) / 2f, num3 + (num4 - num3) / 2f);
		Vector2Int result = default;
		float num5 = 0f;
		foreach (Vector2Int item2 in monitorsVertices)
		{
			if (cornerCompare(item2, vector2))
			{
				float num6 = Vector2.Distance(item2, vector2);
				if (num6 > num5)
				{
					num5 = num6;
					result = item2;
				}
			}
		}
		return result;
	}

	public string GetControlText(IntPtr hWnd)
	{
		int num = SendMessage((int)hWnd, 14, 0, 0).ToInt32();
		if (num == 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(num + 1);
		SendMessage(hWnd, 13u, stringBuilder.Capacity, stringBuilder);
		return stringBuilder.ToString();
	}

	public static string GetWindowTitle(IntPtr handle)
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		if (GetWindowText(handle, stringBuilder, 256) <= 0)
		{
			return null;
		}
		return stringBuilder.ToString();
	}

	public void FocusWindow(IntPtr customWindowHandle)
	{
		uint windowThreadProcessId = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
		uint currentThreadId = GetCurrentThreadId();
		if (windowThreadProcessId != 0)
		{
			if (windowThreadProcessId != currentThreadId)
			{
				AttachThreadInput(windowThreadProcessId, currentThreadId, fAttach: true);
				BringWindowToTop(customWindowHandle);
				ShowWindow(customWindowHandle, ShowWindowCommands.Show.GetHashCode());
				AttachThreadInput(windowThreadProcessId, currentThreadId, fAttach: false);
			}
			else
			{
				BringWindowToTop(customWindowHandle);
				ShowWindow(customWindowHandle, ShowWindowCommands.Show.GetHashCode());
			}
		}
	}

	public override Vector2Int TranslateWindowPos(Vector2Int bottomLeftPosition, Vector2Int windowSize)
	{
		int y = base.WindowDanceResolution.y - bottomLeftPosition.y - windowSize.y;
		Vector2Int vector2Int = new Vector2Int(bottomLeftPosition.x, y);
		return GetBottomLeftDesktopPoint() + vector2Int;
	}

	[DllImport("user32.dll")]
	public static extern bool IsIconic(IntPtr handle);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

	[DllImport("user32.dll")]
	public static extern int SetWindowText(IntPtr hWnd, string text);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string className, string windowName);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

	[DllImport("user32.dll")]
	public static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

	[DllImport("gdi32.dll")]
	public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

	[DllImport("user32.dll")]
	public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

	[DllImport("user32.dll")]
	public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr SendMessage(int hWnd, int Msg, int wparam, int lparam);

	[DllImport("user32.dll")]
	public static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool BringWindowToTop(IntPtr hWnd);

	[DllImport("user32.dll")]
	internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll")]
	public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

	[DllImport("kernel32.dll")]
	public static extern uint GetCurrentThreadId();

	[DllImport("user32.dll")]
	public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

	[DllImport("user32")]
	public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lpRect, MonitorEnumProc callback, int dwData);

	[DllImport("User32.dll")]
	public static extern IntPtr MonitorFromPoint([In] Point pt, [In] uint dwFlags);

	[DllImport("Shcore.dll")]
	public static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, out uint dpiX, out uint dpiY);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr GetDC(IntPtr hWnd);

	[DllImport("user32.dll")]
	private static extern bool SetProcessDPIAware();

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

	[DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "EnumDisplaySettingsW", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnumDisplaySettings([MarshalAs(UnmanagedType.LPWStr)] string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

	[DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "EnumDisplaySettingsExW", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnumDisplaySettingsEx([MarshalAs(UnmanagedType.LPWStr)] string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode, uint dwFlags);
}

#endif
