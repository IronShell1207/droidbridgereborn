using CustomControlsLib.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace DroidBridgeReborn.Converters
{
	using System;

	internal class NavBarItemToBoolConverter : IValueConverter
	{
		public bool IsToVisibility { get; set; } = false;

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is NavBarItem selectedItem && parameter is string strParam &&
				NavBarItem.TryParse(strParam, out NavBarItem currentBarItem))
			{
				if (IsToVisibility)
				{
					return selectedItem == currentBarItem ? Visibility.Visible : Visibility.Collapsed;
				}

				return selectedItem == currentBarItem;
			}

			if (IsToVisibility)
			{
				return Visibility.Collapsed;
			}

			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (value is bool isSelected && parameter is string strParam &&
				NavBarItem.TryParse(strParam, out NavBarItem currentBarItem))
			{
				if (isSelected)
					return currentBarItem;
			}

			return default;
		}
	}
}