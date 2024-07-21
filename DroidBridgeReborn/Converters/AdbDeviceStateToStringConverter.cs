using AdbCore.Enums;
using Microsoft.UI.Xaml.Data;
using System;

namespace DroidBridgeReborn.Converters
{
	public class AdbDeviceStateToStringConverter : IValueConverter

	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is DeviceStateType state)
			{
				return ParseDeviceState(state);
			}

			return "Disconnected";
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return DeviceStateType.Disconnected;
		}

		private string ParseDeviceState(DeviceStateType state)
		{
			return state switch
			{
				DeviceStateType.Disconnected => "Disconnected",
				DeviceStateType.Device => "Connected",
				DeviceStateType.Unauthorized => "Unauthorized",
				DeviceStateType.Offline => "Offline",
				DeviceStateType.Recovery => "Recovery",
				DeviceStateType.Bootloader => "Bootloader",
				DeviceStateType.NoPermissions => "No permissions",
				DeviceStateType.Sideload => "Sideload",
				DeviceStateType.Unknown => "Unknown",
			};
		}
	}
}