using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace WPR.YardiRestoreTask
{
	public class FTPWorker
	{
		private readonly string Host;
		private readonly string Path;
		private readonly string Username;
		private readonly string Password;
		private readonly string MatchWildcard;
		private readonly bool IsEnabled;

		public FTPWorker()
		{
			this.Host = AppSettings.FTP.Host;
			this.Path = AppSettings.FTP.Path;
			this.Username = AppSettings.FTP.Username;
			this.Password = AppSettings.FTP.Password; //TBD - should this be encrypted??
			this.MatchWildcard = AppSettings.FTP.MatchWildcard;
			this.IsEnabled = AppSettings.FTP.IsEnabled;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>The full file name that was downloaded</returns>
		public string DownloadLatestBackup()
		{
			if (!this.IsEnabled)
			{
				return this.GetCurrentBackupFile();
			}

			var file = this.GetLatestMatchingFile();
			this.DownloadFromFtp(file);

			return new FileInfo(file.Name).FullName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sftpFile"></param>
		/// <returns>Full file name</returns>
		public string DownloadFromFtp(SftpFile sftpFile)
		{
			string localFullFileName = $"{Constants.TempPath}\\{sftpFile.Name}";

			if (File.Exists(localFullFileName))
			{
				TaskLogger.Log($"Deleting existing {localFullFileName}");
				File.Delete(localFullFileName);
			}

			TaskLogger.Log($"Downloading {sftpFile.FullName} to {localFullFileName}...");

			using (var client = this.GetClient())
			{
				client.Connect();

				int currentPercentage = 0;

				using (var fs = File.OpenWrite(localFullFileName))
				{
					var ar = client.BeginDownloadFile(sftpFile.FullName, fs);

					while (!ar.AsyncWaitHandle.WaitOne(1000))
					{
						currentPercentage = this.LogPercentageUpdates(currentPercentage, fs.Length, sftpFile.Length / 1024 / 1024);
					}

					client.EndDownloadFile(ar);
				}
			}

			TaskLogger.Log($"Successfully downloaded {sftpFile} to executable directory");

			return localFullFileName;
		}

		private string GetCurrentBackupFile()
		{
			var file = Directory.EnumerateFiles(Constants.TempPath).FirstOrDefault(x => x.EndsWith(".bak"));

			if (string.IsNullOrEmpty(file))
			{
				throw new Exception("Could not find any backup files in current directory");
			}

			return file;
		}

		private int LogPercentageUpdates(int currentPercentage, decimal currentBytes, decimal fileSizeMB)
		{
			decimal currentMB = currentBytes / 1024 / 1024;

			int newPercentage = (int)Math.Floor(currentMB / fileSizeMB * 100);

			if (newPercentage > currentPercentage)
			{
				TaskLogger.Log($"Downloaded {newPercentage}%...");
				return newPercentage;
			}
			else
			{
				return currentPercentage;
			}
		}

		public SftpFile GetLatestMatchingFile()
		{
			TaskLogger.Log($"Getting files at {this.Host}");

			using (var client = this.GetClient())
			{
				client.Connect();

				var files = client.ListDirectory(this.Path).ToList();

				TaskLogger.Log($"Found {files.Count} files, finding latest with token {this.MatchWildcard}");

				var tokens = this.MatchWildcard.Split('*');

				var latestFile = files
					.Where(x => tokens.All(t => x.Name.Contains(t)))
					.OrderByDescending(x => x.LastWriteTimeUtc)
					.FirstOrDefault();
				
				if (latestFile == null)
				{
					throw new Exception($"Could not find file matching {this.MatchWildcard}");
				}

				return latestFile;
			}
		}

		private SftpClient GetClient()
		{
			var connectionInfo = new ConnectionInfo(this.Host, this.Username, new PasswordAuthenticationMethod(this.Username, this.Password));
			
			return new SftpClient(connectionInfo);
		}
	}
}