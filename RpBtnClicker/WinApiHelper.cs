using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static RpBtnClicker.WinApi;

namespace RpBtnClicker
{
	public class WinApiHelper
	{
		public static IntPtr GetWindowByTitle(string title)
		{
			int hwnd = WinApi.FindWindow(null, "Installer");
			return (IntPtr)hwnd;
		}

		public static IntPtr GetChildByClassNameAndTitle(IntPtr parent, string className, string title)
		{
			List<IntPtr> children = GetChildWindows(parent);
			IntPtr foundChild = IntPtr.Zero;
			foreach (var child in children)
			{
				if (foundChild != IntPtr.Zero)
					break;

				if (child == IntPtr.Zero)
					continue;


				string windowClassName = GetClassName(child);

				if (windowClassName.ToUpper().Contains(className.ToUpper()))
				{
					IntPtr hwndChild = WinApi.FindWindowEx(parent, IntPtr.Zero, windowClassName, title);
					if (hwndChild == IntPtr.Zero)
						hwndChild = WinApi.FindWindowEx(parent, IntPtr.Zero, windowClassName, "&" + title);

					if (hwndChild != IntPtr.Zero)
					{
						foundChild = hwndChild;
					}
				}

				if (foundChild == IntPtr.Zero)
					foundChild =  GetChildByClassNameAndTitle(child, className, title);
			}

			return foundChild;
		}

		public static string GetClassName(IntPtr hwnd)
		{
			int nRet;
			// Pre-allocate 256 characters, since this is the maximum class name length.
			StringBuilder className = new StringBuilder(256);
			//Get the window class name
			nRet = WinApi.GetClassName(hwnd, className, className.Capacity);
			if (nRet != 0)
			{
				return className.ToString().Trim();
			}
			else
			{
				return string.Empty;
			}
		}

		public static List<IntPtr> GetChildWindows(IntPtr parent)
		{
			List<IntPtr> result = new List<IntPtr>();
			GCHandle listHandle = GCHandle.Alloc(result);
			try
			{
				EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
				EnumChildWindows(parent, childProc,	 GCHandle.ToIntPtr(listHandle));
			}
			finally
			{
				if (listHandle.IsAllocated)
					listHandle.Free();
			}
			return result;
		}

		private static bool EnumWindow(IntPtr handle, IntPtr pointer)
		{
			GCHandle gch = GCHandle.FromIntPtr(pointer);
			List<IntPtr> list = gch.Target as List<IntPtr>;
			if (list == null)
			{
				throw new InvalidCastException("GCHandle Target could not be	 cast as List<IntPtr>");

			}
			list.Add(handle);
			//  You can modify this to check to see if you want to cancel theoperation, then return a null here

			return true;
		}
	}
}
