using System;
using Windows.ApplicationModel.Resources;
using Microsoft.UI.Xaml.Data;

namespace DroidBridgeReborn.Converters
{
	class AdbStatusToStringConverter :IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is bool bo)
			{
				var resLoader = new ResourceLoader();
				return bo ? resLoader.GetString("RunningUid") : resLoader.GetString("NotRunningUid");
			}

			return "error";
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return false;
		}
	}
}
