using System;
using System.IO;

namespace WPR.YardiRestoreTask
{
	public class AutomationWorker
	{
		private readonly FTPWorker FTPWorker;
		private readonly StagingDbWorker StagingDbWorker;
		private readonly AzureSQLSender AzureSQLSender;

		public AutomationWorker()
		{

			this.FTPWorker = new FTPWorker();
			this.StagingDbWorker = new StagingDbWorker();
			this.AzureSQLSender = new AzureSQLSender();
		}

		public void Work()
		{
			try
			{
				string fileName = this.FTPWorker.DownloadLatestBackup();

				var dbInfo = this.StagingDbWorker.CreateStagingDatabase(fileName);

				this.AzureSQLSender.Send(dbInfo);
			}
			catch (Exception ex)
			{
				TaskLogger.Log(ex);
				TaskLogger.SendEmail(false);
				throw;
			}
			finally
			{
				Directory.Delete(Constants.TempPath);
			}

			TaskLogger.SendEmail(true);
		}
	}
}