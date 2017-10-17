using System;
using System.IO;
using System.Text;

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
		
		public static void SendEmail(bool isSuccess)
		{
			string status = isSuccess ? "succeeded" : "FAILED";
			string subject = $"Yardi restore: {status}";
			EmailSender sender = new EmailSender();
			sender.Send(subject, File.ReadAllText(LogFile));
		}

		public static void Log(Exception ex)
		{
			TaskLogger.Log(ex.ToFlattenedString());
		}

		public static void Log(string msg)
		{
			Console.WriteLine(msg);
			LogToFile(msg);
		}

		private static void LogToFile(string msg)
		{
			var lines = msg.Split('\n');
			File.AppendAllLines(LogFile, lines);
		}
	}

	public static class Extend_Exception
	{
		public static string ToFlattenedString(this Exception ex)
		{
			var sb = new StringBuilder();

			while (ex != null)
			{
				sb.AppendLine(ex.ToString());
				ex = ex.InnerException;
			}

			return sb.ToString();
		}
	}
}