using System;
using System.Text.RegularExpressions;

namespace WPR.YardiRestoreTask
{
	public class FTPDirectoryItem
	{
		public bool IsDirectory { get; set; }
		public string Permissions { get; set; }
		public decimal FileSizeMB { get; set; }
		public DateTime ModifiedDate { get; set; }
		public string FileName { get; set; }
	}
}