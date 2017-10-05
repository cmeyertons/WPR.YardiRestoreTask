using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WPR.YardiRestoreTask
{
	public class FTPDirectoryResponse
	{
		private static readonly string RegexPattern = @"^([d-])([rwxt-]{3}){3}\s+\d{1,}\s+.*?(\d{1,})\s+(\w+\s+\d{1,2}\s+(?:\d{4})?)(\d{1,2}:\d{2})?\s+(.+?)\s?$";

		public List<FTPDirectoryItem> Items { get; private set; } = new List<FTPDirectoryItem>();

		public FTPDirectoryResponse(string responseBody)
		{
			Regex regex = new Regex(RegexPattern,
	RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			var matches = regex.Matches(responseBody);

			foreach (Match match in matches)
			{
				this.AddMatch(match);
			}
		}

		private void AddMatch(Match match)
		{
			if (match.Groups.Count < 7)
			{
				TaskLogger.Log($"Mismatch group count during regex -- {match.Value}");
				return;
			}
			
			FTPDirectoryItem item = new FTPDirectoryItem();

			item.IsDirectory = match.Groups[1].Value == "d";
			item.Permissions = match.Groups[2].Value;

			decimal fileSizeBytes;
			string fileSizeString = match.Groups[3].Value;

			if (!decimal.TryParse(fileSizeString, out fileSizeBytes))
			{
				TaskLogger.Log($"Could not parse file size: {fileSizeString}, fullvlaue: {match.Value}");
				return;
			}

			item.FileName = match.Groups[6].Value;
			item.FileSizeMB = fileSizeBytes / 1024 / 1024;

			var fullDateString = match.Groups[4].Value + match.Groups[5].Value;
			DateTime modifiedDate;
			if (!DateTime.TryParse(fullDateString, out modifiedDate))
			{
				TaskLogger.Log($"Could not parse date: {fullDateString}, full value: {match.Value}");
				return;
			}

			item.ModifiedDate = modifiedDate;

			this.Items.Add(item);
		}
	}
}
