using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPR.YardiRestoreTask;

namespace WPR.Tests.Integration
{
	[TestClass]
    public class FTPTests
    {
		[TestMethod]
		public void ListDirectoryTest()
		{
			FTPWorker ftp = new FTPWorker();
			
			var file = ftp.GetLatestMatchingFile();

			Assert.IsNotNull(file);

			ftp.DownloadFromFtp(file);
		}
    }
}
