using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RpBtnClicker
{
	public class WinApi
	{
		public const int BN_CLICKED = 245;
		public const int WM_CLOSE = 16;

		/// <summary>
		/// The FindWindow API
		/// </summary>
		/// <param name="lpClassName">the class name for the window to search for</param>
		/// <param name="lpWindowName">the name of the window to search for</param>
		/// <returns></returns>
		[DllImport("User32.dll")]
		public static extern Int32 FindWindow(String lpClassName, String lpWindowName);

		/// <summary>
		/// The SendMessage API
		/// </summary>
		/// <param name="hWnd">handle to the required window</param>
		/// <param name="msg">the system/Custom message to send</param>
		/// <param name="wParam">first message parameter</param>
		/// <param name="lParam">second message parameter</param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SendMessage(int hWnd, int msg, int wParam, IntPtr lParam);

		/// <summary>
		/// The FindWindowEx API
		/// </summary>
		/// <param name="parentHandle">a handle to the parent window </param>
		/// <param name="childAfter">a handle to the child window to start search after</param>
		/// <param name="className">the class name for the window to search for</param>
		/// <param name="windowTitle">the name of the window to search for</param>
		/// <returns></returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int cch);
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out Int32 lpdwProcessId);
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern Int32 GetWindowTextLength(IntPtr hWnd);
		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);
		public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);
	}
}
