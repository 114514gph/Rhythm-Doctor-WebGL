#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class PlatformHelperMac : PlatformHelper
{
	public static class Native
	{
		[DllImport("RDMacPlugin")]
		public static extern void Setup();

		[DllImport("RDMacPlugin")]
		public static extern void SetWindowFrame(int x, int y, int w, int h);

		[DllImport("RDMacPlugin")]
		public static extern int GetScreenScalingFactor();

		[DllImport("RDMacPlugin")]
		public static extern void SetupURLHandler();

		[DllImport("RDMacPlugin")]
		public static extern bool URLEventWasTriggered();

		[DllImport("RDMacPlugin")]
		public static extern IntPtr FetchURLString(out int stringLength);

		[DllImport("RDMacPlugin")]
		public static extern void DisposeURLString();

		[DllImport("RDMacPlugin")]
		public static extern IntPtr QuickTest(out int stringLength);

		[DllImport("RDMacPlugin")]
		public static extern int GetScreenCount();

		[DllImport("RDMacPlugin")]
		public static extern Rectangle GetScreenRect(int screenIndex);
	}

	public static int maxTop;

	public const int MacMenuHeight = 22;

	private const string PluginName = "RDMacPlugin";

	public PlatformHelperMac()
	{
		Debug.Log("PlatformHelperMac constructor");
		Native.Setup();
		playerWindow = new UnityPlayerWindowMac(-1, null);
	}

	public override Vector2Int GetWindowDanceResolution()
	{
		Rectangle monitorsRect = GetMonitorsRect(base.Monitors, onlyHorizontal: true);
		return new Vector2Int(monitorsRect.Right - monitorsRect.Left, monitorsRect.Bottom - monitorsRect.Top);
	}

	protected override List<Monitor> GetMonitors()
	{
		int num = 1;
		List<Monitor> list = new List<Monitor>();
		maxTop = int.MinValue;
		for (int i = 0; i < num; i++)
		{
			Rectangle screenRect = Native.GetScreenRect(i);
			int screenScalingFactor = Native.GetScreenScalingFactor();
			maxTop = Math.Max(screenRect.Top, maxTop);
			Monitor item = new Monitor(screenRect, screenScalingFactor);
			list.Add(item);
		}
		for (int j = 0; j < num; j++)
		{
			Rectangle bounds = list[j].bounds;
			bounds.Top = maxTop - bounds.Top;
			bounds.Bottom = maxTop - bounds.Bottom;
			list[j].bounds = bounds;
		}
		return list;
	}

	public static string GetURLString()
	{
		int stringLength = 0;
		IntPtr source = Native.FetchURLString(out stringLength);
		byte[] array = new byte[stringLength];
		Marshal.Copy(source, array, 0, stringLength);
		Native.DisposeURLString();
		return Encoding.UTF8.GetString(array);
	}
}

#endif
