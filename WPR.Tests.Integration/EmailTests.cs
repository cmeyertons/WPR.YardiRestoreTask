using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPR.YardiRestoreTask;

namespace WPR.Tests.Integration
{
	[TestClass]
	public class EmailTests
	{
		[TestMethod]
		public void EmailSendSuccess()
		{
			TaskLogger.Log("test1");
			TaskLogger.Log("test2");
			TaskLogger.Log("test3");
			TaskLogger.SendEmail(false);
		}
	}
}
