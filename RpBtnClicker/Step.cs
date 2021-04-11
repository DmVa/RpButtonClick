using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpBtnClicker
{
	public class Step
	{
		public List<Step> FormSteps { get; set; }
		public string ClassName { get;set;}
		public string Title { get; set; }
		public string Text { get; set; }
		public Actions Action { get; set; }
		public int WaitForSec { get; set; }
	}
}
