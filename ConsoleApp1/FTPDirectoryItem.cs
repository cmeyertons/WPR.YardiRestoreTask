using System;
using System.Text.RegularExpressions;

namespace WPR.YardiRestoreTask
{
	public class FTPDirectoryItem
	{
		public bool IsDirectory { get; internal set; }
		public string Permissions { get; internal set; }
		public decimal FileSizeMB { get; internal set; }
		public DateTime ModifiedDate { get; internal set; }
		public string FileName { get; internal set; }
	}
}