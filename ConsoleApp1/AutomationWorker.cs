using System;

namespace WPR.YardiRestoreTask
{
	internal class AutomationWorker
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

		internal void Work()
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
			}
			finally
			{

			}
		}
	}
}