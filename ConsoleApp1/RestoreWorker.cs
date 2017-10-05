using System;
using System.Diagnostics;
using System.IO;

namespace WPR.YardiRestoreTask
{
	internal class RestoreWorker
	{
		private const string LitespeedExeName = "sqllitespeed.exe";
		private const string DefaultInstallPath = @"C:\Program Files\Quest Software\LiteSpeed\SQL Server\Engine";
		private readonly string LitespeedExe;
		private readonly string DbServer;
		private readonly string DbName;

		public RestoreWorker()
		{
			this.LitespeedExe = $@"{AppSettings.Litespeed.InstallPath}\{LitespeedExeName}";

			if (!File.Exists(this.LitespeedExe))
			{
				this.LitespeedExe = $@"{DefaultInstallPath}\{LitespeedExeName}";
			}

			if (!File.Exists(this.LitespeedExe))
			{
				throw new FileNotFoundException("Could not find Litespeed installation, please check app settings.");
			}

			this.DbServer = AppSettings.Db.Server;
			this.DbName = AppSettings.Db.Name;

			if (string.IsNullOrEmpty(this.DbServer)) { throw new ArgumentNullException("Db.Server is missing"); }
			if (string.IsNullOrEmpty(this.DbName)) { throw new ArgumentNullException("Db.Name is missing"); }
		}

		internal void Restore(string fileName)
		{
			ProcessStartInfo info = new ProcessStartInfo();

			var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");

			//info.RedirectStandardOutput = true;
			//info.CreateNoWindow = true;
			info.FileName = this.LitespeedExe;
			//info.UseShellExecute = false;
			info.Arguments = $"-R Database -S \"{this.DbServer}\" -T -D \"{this.DbName}_{timeStamp}\" -F \"{fileName}\" -W REPLACE";

			var proc = Process.Start(info);

			proc.WaitForExit();
		}
	}
}