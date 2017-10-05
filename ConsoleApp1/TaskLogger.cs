using System;

namespace WPR.YardiRestoreTask
{
	internal class TaskLogger
	{
		//TODO - keep track of log and add send email API

		internal static void Log(Exception ex)
		{
			Console.WriteLine(ex.Message);
		}

		internal static void Log(string msg)
		{
			Console.WriteLine(msg);
		}
	}
}