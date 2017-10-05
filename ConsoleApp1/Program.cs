using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPR.YardiRestoreTask
{
	class Program
	{
		static void Main(string[] args)
		{
			AutomationWorker worker = new AutomationWorker();

			worker.Work();
		}
	}
}
