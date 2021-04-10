using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Web.Script.Serialization;


namespace RpBtnClicker
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
				throw new ApplicationException("first argument should be script file");

			Script script = CreateScript(args);
			SerializeScript(script);
			script = LoadScript(args[0]);
			if (string.IsNullOrEmpty(script?.WindowTitle))
				throw new ApplicationException("Script window title not defined");

			//RunScript(script);
		}

		private static Script LoadScript(string fileName)
		{
			string jsonStr = File.ReadAllText(fileName);
			JavaScriptSerializer js = new JavaScriptSerializer();
			Script script = js.Deserialize<Script>(jsonStr);
			return script;
		}

		private static void SerializeScript(Script script)
		{
			JavaScriptSerializer js = new JavaScriptSerializer();
			var jsonString = js.Serialize(script);
			jsonString = JsonFormatter.FormatOutput(jsonString);
			File.WriteAllText("runInstaller.json", jsonString);
		}

		private static void RunScript(Script script)
		{
			int hwnd = WinApi.FindWindow(null, script.WindowTitle);
			if (hwnd == 0)
			{
				System.Diagnostics.Process.Start($"\"{script.Process}\"");
				Thread.Sleep(2500);
			}
			hwnd = WinApi.FindWindow(null, script.WindowTitle);
			if (hwnd == 0)
				throw new ApplicationException($"cannot find window {script.WindowTitle}");

			if (script.Steps == null)
				throw new ApplicationException($"steps not defined");
			foreach(var step in script.Steps)
			{
				RunStep(hwnd, step);
			}
		}


		private static void RunStep(int hwndWindow, Step step)
		{
			var sp = new Stopwatch();
			sp.Start();
			int spend = 0;
			IntPtr ctrl = IntPtr.Zero;
			while (spend <= step.WaitForSec && ctrl == IntPtr.Zero)
			{
				ctrl = WinApiHelper.GetChildByClassNameAndTitle((IntPtr)hwndWindow, step.ClassName, step.Title);
				if (ctrl == IntPtr.Zero)
				{
					Thread.Sleep(1000);
				}
				spend = (int) sp.Elapsed.TotalSeconds;
			}

			sp.Stop();

			if (ctrl == IntPtr.Zero)
			{
				throw new ApplicationException($"cannot find class {step.ClassName} with title {step.Title}");
			}

			switch (step.Action)
			{
				case Actions.Click:
					WinApi.SendMessage((int)ctrl, WinApi.BN_CLICKED, 0, IntPtr.Zero);
				break;
				default:
					throw new NotSupportedException($"{step.Action} not supported");
			}
		}

		private static Script CreateScript(string[] args)
		{
			var script = new Script();
			script.Process = @"c:\Install\C1\Coric Web Installer.exe";
			script.WindowTitle = "Installer";
			script.Steps = new List<Step>();
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Install", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Finish", Action = Actions.Click, WaitForSec = 300 });
			return script;
		}
	}
}
