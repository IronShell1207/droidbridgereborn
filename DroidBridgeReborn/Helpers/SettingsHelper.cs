using Utils.Helpers.LocalSettingsHelper;

namespace DroidBridgeReborn.Helpers
{
	using System;

	internal class SettingsHelper
	{
		private static LocalSettingsHelper<double> _adbDevicesListUpdateIntervalSetting =
			new LocalSettingsHelper<double>("AdbDevicesListUpdateInterval");

		private static LocalSettingsHelper<double> _adbStatusUpdateIntervalSetting =
			new LocalSettingsHelper<double>("AdbServiceUpdateInterval");

		private static LocalSettingsHelper<string> _adbLastPath = new LocalSettingsHelper<string>("AdbPathSaved");
		private static LocalSettingsHelper<string> _scrcpyLastPath = new LocalSettingsHelper<string>("ScrcpyPathSaved");

		public static LocalSettingsHelper<bool> IsStartingMinimizedSetting => new("IsStartMinimized");

		public static TimeSpan GetAdbUpdateInterval()
		{
			if (_adbStatusUpdateIntervalSetting.Exists == false)
			{
				_adbStatusUpdateIntervalSetting.Value = 5.0;
			}
			return TimeSpan.FromSeconds(_adbStatusUpdateIntervalSetting.Value);
		}

		public static TimeSpan GetDevicesListUpdateInterval()
		{
			if (_adbDevicesListUpdateIntervalSetting.Exists == false)
			{
				_adbDevicesListUpdateIntervalSetting.Value = 5.0;
			}
			return TimeSpan.FromSeconds(_adbDevicesListUpdateIntervalSetting.Value);
		}
		public static void SetDevicesListUpdateInterval(TimeSpan interval)
		{
			_adbDevicesListUpdateIntervalSetting.Value = interval.TotalSeconds;
		}

		public static void SetAdbUpdateInterval(TimeSpan interval)
		{
			_adbStatusUpdateIntervalSetting.Value = interval.TotalSeconds;
		}

		public static string GetScrcpyLastPath()
		{
			return _scrcpyLastPath.Value;
		}

		public static string GetAdbLastPath()
		{
			return _adbLastPath.Value;
		}
	}
}