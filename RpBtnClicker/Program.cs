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
			//SerializeScript(script, args[0]);
			script = LoadScript(args[0]);
			if (string.IsNullOrEmpty(script?.WindowTitle))
				throw new ApplicationException("Script window title not defined");

			RunScript(script, args[1]);
		}

		private static Script LoadScript(string fileName)
		{
			string jsonStr = File.ReadAllText(fileName);
			JavaScriptSerializer js = new JavaScriptSerializer();
			Script script = js.Deserialize<Script>(jsonStr);
			return script;
		}

		private static void SerializeScript(Script script, string fileName)
		{
			JavaScriptSerializer js = new JavaScriptSerializer();
			var jsonString = js.Serialize(script);
			jsonString = JsonFormatter.FormatOutput(jsonString);
			File.WriteAllText(fileName, jsonString);
		}

		private static void RunScript(Script script, string process)
		{
			int hwnd = WinApi.FindWindow(null, script.WindowTitle);
			if (hwnd == 0)
			{
				System.Diagnostics.Process.Start($"{process}");
				Thread.Sleep(2500);
			}
			hwnd = WinApi.FindWindow(null, script.WindowTitle);
			if (hwnd == 0)
				throw new ApplicationException($"cannot find window {script.WindowTitle}");

			if (script.Steps == null)
				throw new ApplicationException($"steps not defined");
			foreach(var step in script.Steps)
			{
				if (step.Action == Actions.FindForm)
				{
					RunFormSteps(hwnd, step);
				}
				else
				{
					RunStep(hwnd, step);
				}
			}
		}

		private static void RunFormSteps(int hwndWindow, Step step)
		{
			List<IntPtr> ctrls = WinApiHelper.GetFormControls((IntPtr)hwndWindow, step.ClassName, step.Title, step.FormSteps);
			for(int idx = 0; idx<step.FormSteps.Count;idx++)
			{
				var ctrlStep = step.FormSteps[idx];
				var ctrl = ctrls[idx];
				WinApi.SendMessage((int)ctrl, WinApi.WM_SETTEXT, 0, ctrlStep.Text);
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
				case Actions.SetText:
					WinApi.SendMessage((int)ctrl,WinApi.WM_SETTEXT, 0, step.Text);
					break;
				default:
					throw new NotSupportedException($"{step.Action} not supported");
			}
		}

		private static Script CreateScript(string[] args)
		{
			var script = new Script();
			//script.Process = @"c:\Install\C1\Coric Web Installer.exe";
			script.WindowTitle = "Installer";
			script.Steps = new List<Step>();
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() {
				Action = Actions.FindForm, WaitForSec = 1,
				FormSteps = new List<Step>()
				{
					new Step() { ClassName = "edit", Title = "", Action = Actions.SetText, WaitForSec = 1,  Text="uw4BN6Ms#!86rL" },
					new Step() { ClassName = "edit", Title = "", Action = Actions.SetText, WaitForSec = 1,  Text="CoricComPlus" },
					new Step() { ClassName = "edit", Title = "", Action = Actions.SetText, WaitForSec = 1,  Text="SCDOM" },
					new Step() { ClassName = "edit", Title = "", Action = Actions.SetText, WaitForSec = 1,  Text="C1AppPool" }
				}
			} 
			);
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step()
			{
				Action = Actions.FindForm,
				WaitForSec = 1,
				FormSteps = new List<Step>()
				{
					new Step() { ClassName = "edit", Title = "", Action = Actions.SetText, WaitForSec = 1,  Text="uw4BN6Ms#!86rL" },
					new Step() { ClassName = "edit", Title = "", Action = Actions.SetText, WaitForSec = 1,  Text="CoricComPlus" },
					new Step() { ClassName = "edit", Title = "", Action = Actions.SetText, WaitForSec = 1,  Text="SCDOM" },
					new Step() { ClassName = "edit", Title = "", Action = Actions.SetText, WaitForSec = 1,  Text="C1AppPool" }
				}
			}
			);
			script.Steps.Add(new Step() { ClassName = "button", Title = "Next >", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Install", Action = Actions.Click, WaitForSec = 1 });
			script.Steps.Add(new Step() { ClassName = "button", Title = "Finish", Action = Actions.Click, WaitForSec = 300 });
			return script;
		}
	}
}
