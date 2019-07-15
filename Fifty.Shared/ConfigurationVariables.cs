using System;
using System.Runtime.CompilerServices;

namespace Fifty.Shared
{
	public static class ConfigurationVariables
	{
		public static string LavuConfigPeel => GetStringValue("LavuConfigPeel");

		public static string LavuConfigFifty => GetStringValue("LavuConfigFifty");

		public static string SmartsheetToken => GetStringValue("SmartsheetToken");

		public static long SheetId5030 => GetLongValue("SheetId5030");

		public static long SheetIdLog => GetLongValue("SheetIdLog");

		public static long SheetIdPeel => GetLongValue("SheetIdPeel");

		public static string GetStringValue(string key)
		{
			var value = Environment.GetEnvironmentVariable(key);
			if (!string.IsNullOrWhiteSpace(value))
			{
				return value;
			}

			throw new Exception($"Configuration excption - missing {key} - check launchSettings.json");
		}

		public static long GetLongValue(string key)
		{
			var value = Environment.GetEnvironmentVariable(key);
			if (long.TryParse(value, out var parsedValue))
			{
				return parsedValue;
			}

			throw new Exception($"Configuration excption - missing {key} - check launchSettings.json");
		}
	}
}
