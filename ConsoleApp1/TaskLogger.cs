using System;
using System.IO;

namespace WPR.YardiRestoreTask
{
	public static class TaskLogger
	{
		private static readonly string LogFile;

		static TaskLogger()
		{
			var timestamp = DateTime.Now.ToString("yyyy.MM.dd-hh.mm.ss");
			LogFile = $"{Constants.CurrentPath}\\{timestamp}.log";
		}
		//TODO - keep track of log and add send email API

		internal static void Log(Exception ex)
		{
			TaskLogger.Log(ex.Message);
		}

		internal static void Log(string msg)
		{
			Console.WriteLine(msg);
			LogToFile(msg);
		}

		private static void LogToFile(string msg)
		{
			File.AppendAllLines(LogFile, new string[] { msg });
		}
	}
}