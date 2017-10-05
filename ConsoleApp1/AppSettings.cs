using System.Configuration;

namespace WPR.YardiRestoreTask
{
	internal static class AppSettings
	{
		public static class Litespeed
		{
			public static string InstallPath => ConfigurationManager.AppSettings.Get("Litespeed.InstallPath");
		}

		public static class FTP
		{
			public static string Host => ConfigurationManager.AppSettings.Get("FTP.Host");
			public static string Username => ConfigurationManager.AppSettings.Get("FTP.Username");
			public static string Password => ConfigurationManager.AppSettings.Get("FTP.Password");
			public static string MatchWildcard => ConfigurationManager.AppSettings.Get("FTP.MatchWildcard");
		}

		public static class Db
		{
			public static string Server => ConfigurationManager.AppSettings.Get("Db.Server");
			public static string Name => ConfigurationManager.AppSettings.Get("Db.Name");
		}

		public static class Test
		{
			public static string OverrideFileName => ConfigurationManager.AppSettings.Get(@"Test.OverrideFileName");
		}
	}
}