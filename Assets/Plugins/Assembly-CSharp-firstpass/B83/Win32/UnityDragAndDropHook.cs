using System;
using System.Collections.Generic;
using System.Text;

namespace B83.Win32
{
	public class UnityDragAndDropHook
	{
		public delegate void DroppedFilesEvent(List<string> aPathNames, POINT aDropPoint);

		private uint threadId = WinAPI.GetCurrentThreadId();

		private IntPtr mainWindow;

		private HookProc m_Callback;

		private IntPtr m_Hook;

		public event DroppedFilesEvent OnDroppedFiles;

		public UnityDragAndDropHook()
		{
			if (threadId != 0)
			{
				mainWindow = GetMainWindow(threadId, "UnityWndClass");
			}
		}

		public static IntPtr GetMainWindow(uint aThreadId, string aClassName = null)
		{
			IntPtr win = IntPtr.Zero;
			Window.EnumThreadWindows(aThreadId, (IntPtr W, IntPtr _) =>
			{
				if (Window.IsWindowVisible(W) && aClassName != null && Window.GetClassName(W) == aClassName)
				{
					win = W;
				}
				return true;
			}, IntPtr.Zero);
			return win;
		}

		public void InstallHook()
		{
			IntPtr moduleHandle = WinAPI.GetModuleHandle(null);
			m_Callback = Callback;
			m_Hook = WinAPI.SetWindowsHookEx(HookType.WH_GETMESSAGE, m_Callback, moduleHandle, threadId);
			WinAPI.DragAcceptFiles(mainWindow, fAccept: true);
		}

		public void UninstallHook()
		{
			WinAPI.UnhookWindowsHookEx(m_Hook);
			m_Hook = IntPtr.Zero;
		}

		private IntPtr Callback(int code, IntPtr wParam, ref MSG lParam)
		{
			if (code == 0 && lParam.message == WM.DROPFILES)
			{
				WinAPI.DragQueryPoint(lParam.wParam, out var pos);
				uint num = WinAPI.DragQueryFile(lParam.wParam, uint.MaxValue, null, 0u);
				StringBuilder stringBuilder = new StringBuilder(1024);
				List<string> list = new List<string>();
				for (uint num2 = 0u; num2 < num; num2++)
				{
					int num3 = (int)WinAPI.DragQueryFile(lParam.wParam, num2, stringBuilder, 1024u);
					list.Add(stringBuilder.ToString(0, num3));
					stringBuilder.Length = 0;
				}
				WinAPI.DragFinish(lParam.wParam);
				if (OnDroppedFiles != null)
				{
					OnDroppedFiles(list, pos);
				}
			}
			return WinAPI.CallNextHookEx(m_Hook, code, wParam, ref lParam);
		}
	}
}
