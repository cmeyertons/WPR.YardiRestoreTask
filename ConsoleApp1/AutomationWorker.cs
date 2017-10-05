using System;

namespace WPR.YardiRestoreTask
{
	internal class AutomationWorker
	{
		private readonly FTPWorker FTPWorker;
		private readonly RestoreWorker RestoreWorker;

		public AutomationWorker()
		{
			this.FTPWorker = new FTPWorker();
			this.RestoreWorker = new RestoreWorker();
		}

		internal void Work()
		{
			try
			{
				string fileName = string.IsNullOrEmpty(AppSettings.Test.OverrideFileName)
					? this.FTPWorker.DownloadLatestBackup()
					: AppSettings.Test.OverrideFileName;
				
				this.RestoreWorker.Restore(fileName);
			}
			catch (Exception ex)
			{
				TaskLogger.Log(ex);
			}
		}
	}
}