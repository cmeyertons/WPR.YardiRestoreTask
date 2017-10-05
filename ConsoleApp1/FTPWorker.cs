using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace WPR.YardiRestoreTask
{
	internal class FTPWorker
	{
		private readonly string Host;
		private readonly string Username;
		private readonly string Password;
		private readonly string MatchWildcard;

		public FTPWorker()
		{
			this.Host = AppSettings.FTP.Host;
			this.Username = AppSettings.FTP.Username;
			this.Password = AppSettings.FTP.Password; //TBD - should this be encrypted??
			this.MatchWildcard = AppSettings.FTP.MatchWildcard;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>The file name that was downloaded</returns>
		public string DownloadLatestBackup()
		{
			var file = this.GetLatestMatchingFile();

			var request = this.CreateRequest($"{this.Host}/{file.FileName}");

			request.Method = WebRequestMethods.Ftp.DownloadFile;

			if (File.Exists(file.FileName))
			{
				TaskLogger.Log($"Deleting existing {file.FileName}");
				File.Delete(file.FileName);
			}

			TaskLogger.Log($"Downloading {request.RequestUri}...");

			int currentPercentage = 0;

			using (Stream ftpStream = request.GetResponse().GetResponseStream())
			using (Stream fileStream = File.Create(file.FileName))
			{
				byte[] buffer = new byte[10240];
				int read;
				decimal total = 0;
				while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
				{
					fileStream.Write(buffer, 0, read);
					total += read;
					currentPercentage = this.LogPercentageUpdates(currentPercentage, total, file.FileSizeMB);
				}
			}

			var info = new FileInfo(file.FileName);

			TaskLogger.Log($"Successfully downloaded {file} to executable directory");

			return info.FullName;
		}

		private int LogPercentageUpdates(int currentPercentage, decimal currentBytes, decimal fileSizeMB)
		{
			decimal currentMB = currentBytes / 1024 / 1024;

			int newPercentage = (int)Math.Floor(currentMB / fileSizeMB * 100);

			if (newPercentage > currentPercentage + 5)
			{
				TaskLogger.Log($"Downloaded {newPercentage}%...");
				return newPercentage;
			}
			else
			{
				return currentPercentage;
			}
		}

		private FTPDirectoryItem GetLatestMatchingFile()
		{
			TaskLogger.Log($"Getting files at {this.Host}");

			FtpWebRequest request = this.CreateRequest(this.Host);  
			request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

			FtpWebResponse response = (FtpWebResponse)request.GetResponse();

			string responseBody;

			using (var stream = request.GetResponse().GetResponseStream())
			{
				StreamReader reader = new StreamReader(stream);
				responseBody = reader.ReadToEnd();
			}

			var ftpDirectoryResponse = new FTPDirectoryResponse(responseBody);

			TaskLogger.Log($"Found {ftpDirectoryResponse.Items.Count} files, finding latest with token {this.MatchWildcard}");

			var tokens = this.MatchWildcard.Split('*');

			var latestFile = ftpDirectoryResponse.Items
				.Where(x => tokens.All(t => x.FileName.Contains(t)))
				.OrderByDescending(x => x.ModifiedDate)
				.FirstOrDefault();

			if (latestFile == null)
			{
				throw new Exception($"Could not find file matching {this.MatchWildcard}");
			}

			TaskLogger.Log($"Using file {latestFile.FileName}");

			return latestFile;
		}

		private FtpWebRequest CreateRequest(string path)
		{
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
			request.Credentials = new NetworkCredential(this.Username, this.Password);
			return request;
		}
	}
}