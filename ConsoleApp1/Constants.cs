using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPR.YardiRestoreTask
{
    public class Constants
    {
		public static string CurrentPath => AppDomain.CurrentDomain.BaseDirectory;
		public static string ToolsPath => $"{CurrentPath}\\Tools";
	}
}
