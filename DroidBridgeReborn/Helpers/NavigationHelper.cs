using Microsoft.UI.Xaml.Controls;
using System;

namespace DroidBridgeReborn.Helpers
{
	public class NavigationHelper
	{
		private static Frame _frame;

		public static void SetupNavigationFrame(Frame frame)
		{
			_frame = frame;
		}

		public static void NavigateTo(Type pageType, object navigationParameter = null)
		{
			_frame.Navigate(pageType, navigationParameter);
		}

		public static void NavigateBack()
		{
			_frame.GoBack();
		}
	}
}