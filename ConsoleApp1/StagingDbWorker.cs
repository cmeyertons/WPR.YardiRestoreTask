using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WPR.YardiRestoreTask
{
	public class StagingDbWorker
	{
		private const string LitespeedExeName = "SQLLiteSpeed.exe";
		private const string LitespeedExtractorName = "Extractor.exe";

		private readonly string LitespeedExe;
		private readonly string ExtractorExe;
		private readonly string DbServer;
		private readonly bool IsEnabled;

		public StagingDbWorker()
		{
			this.LitespeedExe = this.FindAndValidateTool(LitespeedExeName);
			this.ExtractorExe = this.FindAndValidateTool(LitespeedExtractorName);

			this.DbServer = AppSettings.Db.Staging.Server;

			if (string.IsNullOrEmpty(this.DbServer)) { throw new ArgumentNullException("Db.Server is missing"); }

			this.IsEnabled = AppSettings.Db.Staging.IsEnabled;
		}

		private string FindAndValidateTool(string tool)
		{
			string fullPath = $"{Constants.ToolsPath}\\{tool}";

			if (!File.Exists(fullPath))
			{
				throw new FileNotFoundException("Could not find file", fullPath);
			}

			return fullPath;
		}

		public StagingDbInfo CreateStagingDatabase(string fileName)
		{
			string dbName = this.IsEnabled
				? this.DoCreate(fileName)
				: this.GetDbName(fileName);

			return new StagingDbInfo()
			{
				Name = dbName,
				Server = this.DbServer, //TODO - this should spin up the server
			};
		}

		private string GetDbName(string fileName)
		{
			if (!File.Exists(fileName))
			{
				throw new Exception($"Could not find given file {fileName}");
			}

			var info = new FileInfo(fileName);

			return info.Name.Replace(".bak", "");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns>The database name that was restored</returns>
		private string DoCreate(string fileName)
		{
			string newFileName = this.ExtractToSqlBak(fileName);
			return this.RestoreDb(newFileName);
		}

		private string ExtractToSqlBak(string fileName)
		{
			var newFileName = fileName.Replace(".Lts.bak", ".bak");

			ProcessRunner runner = new ProcessRunner(this.ExtractorExe
				, $"-F {fileName} -E {newFileName}");

			runner.Run();

			return newFileName;
		}

		private string RestoreDb(string newFileName)
		{
			string dbName = this.GetDbName(newFileName);

			ProcessRunner runner = new ProcessRunner(this.LitespeedExe
				, $"-R Database -S \"{this.DbServer}\" -T -D \"{dbName}\" -F \"{newFileName}\" -W REPLACE");

			bool isSuccess = runner.Run();

			if (!isSuccess)
			{
				throw new Exception("Restore failed -- process terminating.");
			}

			return dbName;
		}

		private void Cleanup()
		{
			if (this.IsEnabled)
			{

			}
			//TODO drop database

			//TODO delete extracted file
		}
	}
}