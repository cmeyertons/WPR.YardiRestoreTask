﻿using System;
using System.Configuration;

namespace WPR.YardiRestoreTask
{
	public static class AppSettings
	{
		public static class FTP
		{
			public static string Host => ConfigurationManager.AppSettings.Get("FTP.Host");
			public static string Path => ConfigurationManager.AppSettings.Get("FTP.Path");
			public static string Username => ConfigurationManager.AppSettings.Get("FTP.Username");
			public static string Password => ConfigurationManager.AppSettings.Get("FTP.Password");
			public static string MatchWildcard => ConfigurationManager.AppSettings.Get("FTP.MatchWildcard");
			public static bool IsEnabled => ConfigurationManager.AppSettings.Get("FTP.IsEnabled")?.ToLower() == "true";
		}

		public static class Db
		{
			public static class Staging
			{
				public static string Server => ConfigurationManager.AppSettings.Get("Db.Staging.Server");
				public static bool IsEnabled => ConfigurationManager.AppSettings.Get("Db.Staging.IsEnabled")?.ToLower() == "true";
			}

			public static class Azure
			{
				public static string Server => ConfigurationManager.AppSettings.Get("Db.Azure.Server");
				public static string Name => ConfigurationManager.AppSettings.Get("Db.Azure.Name");
				public static string User => ConfigurationManager.AppSettings.Get("Db.Azure.User");
				public static string Password => ConfigurationManager.AppSettings.Get("Db.Azure.Password");
				public static string Edition => ConfigurationManager.AppSettings.Get("Db.Azure.Edition");
				public static string ServiceObjective => ConfigurationManager.AppSettings.Get("Db.Azure.ServiceObjective");
				public static bool IsEnabled => ConfigurationManager.AppSettings.Get("Db.Azure.IsEnabled")?.ToLower() == "true";
			}
		}

		public static class Email
		{
			public static string Host => ConfigurationManager.AppSettings.Get("Email.Host");
			public static int Port => Convert.ToInt32(ConfigurationManager.AppSettings.Get("Email.Port"));
			public static string Username => ConfigurationManager.AppSettings.Get("Email.Username");
			public static string Password => ConfigurationManager.AppSettings.Get("Email.Password");
			public static string Recipients => ConfigurationManager.AppSettings.Get("Email.Recipients");
		}
	}
}