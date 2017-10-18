using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPR.YardiRestoreTask
{
    public static class Constants
    {
		static Constants()
		{
			Directory.CreateDirectory(ToolsPath);
			Directory.CreateDirectory(TempPath);
		}

		public static string CurrentPath => AppDomain.CurrentDomain.BaseDirectory;
		public static string ToolsPath => $"{CurrentPath}Tools";
		public static string TempPath => $"{CurrentPath}Temp";
	}
}
