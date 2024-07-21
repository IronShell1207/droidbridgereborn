using AdbCore.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace DroidBridgeReborn.Converters
{
	internal class AdbDeviceStateToBoolConverter : IValueConverter
	{
		public bool IsToVisibility { get; set; } = false;

		public bool IsInverted { get; set; } = false;

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is DeviceStateType currentState && parameter is string neededStateString &&
				DeviceStateType.TryParse(neededStateString, out DeviceStateType neededState))
			{
				if (IsToVisibility && !IsInverted)
				{
					return currentState == neededState ? Visibility.Visible : Visibility.Collapsed;
				}
				else if (IsToVisibility && IsInverted)
				{
					return currentState == neededState ? Visibility.Collapsed : Visibility.Visible;
				}
				else if (IsInverted)
				{
					return currentState != neededState;
				}
				return currentState == neededState;
			}

			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}