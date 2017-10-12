using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace WPR.YardiRestoreTask
{
	internal class AzureSQLSender
	{
		private const string SqlPackageExeName = "SqlPackage.exe";
		private readonly string SqlPackageExe;


		private readonly string AzureServer;
		private readonly string AzureDb;
		private readonly string AzureUser;
		private readonly string AzurePassword;
		private readonly string DbEdition;
		private readonly string DbServiceObjective;
		private readonly string Timestamp;
		private readonly bool IsEnabled;

		public AzureSQLSender()
		{
			this.SqlPackageExe = $"{Constants.ToolsPath}\\{SqlPackageExeName}";

			if (!File.Exists(this.SqlPackageExe))
			{
				throw new Exception($"Could not find {this.SqlPackageExe}");
			}
			
			//TEMP - TODO remove
			//this.SqlPackageExe = @"C:\Program Files (x86)\Microsoft SQL Server\140\DAC\bin\SqlPackage.exe";

			this.AzureServer = AppSettings.Db.Azure.Server;
			this.AzureDb = AppSettings.Db.Azure.Name;
			this.AzureUser = AppSettings.Db.Azure.User;
			this.AzurePassword = AppSettings.Db.Azure.Password;
			this.DbEdition = AppSettings.Db.Azure.Edition;
			this.DbServiceObjective = AppSettings.Db.Azure.ServiceObjective;
			this.Timestamp = DateTime.Now.ToString("yyyyMMddhhmmss");

			this.IsEnabled = AppSettings.Db.Azure.IsEnabled;

			this.TestConnection();
		}

		private void TestConnection()
		{
			using (var conn = this.GetMasterConnection())
			{
				var command = new SqlCommand("select 1", conn);
				var ret = (int)command.ExecuteScalar();

				if (ret != 1)
				{
					throw new Exception("Could not verify connection");
				}
			}
		}

		internal void Send(StagingDbInfo stagingDbInfo)
		{
			if (this.IsEnabled)
			{
				this.DoSend(stagingDbInfo);
			}
		}

		private void DoSend(StagingDbInfo stagingDbInfo)
		{
			string fileName = this.CreateBacpacFile(stagingDbInfo);
			this.ImportToAzure(stagingDbInfo.Name, fileName);
			this.ReplaceExistingDb(stagingDbInfo.Name);
		}

		private string CreateBacpacFile(StagingDbInfo stagingDbInfo)
		{
			string fileName = $"{Constants.CurrentPath}\\{stagingDbInfo.Name}.bacpac";

			ProcessRunner runner = new ProcessRunner(this.SqlPackageExe
				, $"/Action:Export /ssn:{stagingDbInfo.Server} /sdn:{stagingDbInfo.Name} /tf:{fileName}");

			bool isSuccess = runner.Run();

			if (!isSuccess)
			{
				throw new Exception("Could not create .bacpac -- process terminated.");
			}

			return fileName;
		}

		private void ImportToAzure(string dbName, string fileName)
		{
			if (dbName == this.AzureDb)
			{
				//TODO - drop this DB first??
				throw new Exception($"Trying to import to Azure, but {dbName} already exists.");
			}

			//this should be unique
			var args = $"/Action:import /tsn:tcp:{this.AzureServer},1433 /tdn:{dbName} /tu:{this.AzureUser} /tp:{this.AzurePassword} /sf:{fileName} /p:DataBaseEdition={this.DbEdition}";

			if (!string.IsNullOrEmpty(this.DbServiceObjective))
			{
				args += $" /p:DatabaseServiceObjective={this.DbServiceObjective}";
			}

			ProcessRunner runner = new ProcessRunner(this.SqlPackageExe
				, args);

			bool isSuccess = runner.Run();

			if (!isSuccess)
			{
				throw new Exception("Could not import to Azure -- process terminated.");
			}
		}

		private SqlConnection GetMasterConnection()
		{
			var builder = new SqlConnectionStringBuilder()
			{
				DataSource = this.AzureServer,
				InitialCatalog = "master",
				UserID = this.AzureUser,
				Password = this.AzurePassword,
			};

			var conn = new SqlConnection(builder.ConnectionString);
			conn.Open();

			return conn;
		}

		private void ReplaceExistingDb(string sourceDbName)
		{
			var commandText = $@"drop database [{this.AzureDb}];
alter database[{sourceDbName}] modify name = [{this.AzureDb}];";

			using (var connection = this.GetMasterConnection())
			{
				TaskLogger.Log($"Executing SQL:");
				TaskLogger.Log(commandText);
				var command = new SqlCommand(commandText, connection);
				command.ExecuteNonQuery();
			}
		}
	}
}