using CustomControlsLib.Abstract;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DroidBridgeReborn.Views.Pages
{
	/// <summary>
	/// Page of device actions and info
	/// </summary>
	public sealed partial class DevicePage : Page
	{
		public DevicePage()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Обрабатывает навигацию на страницу.
		/// </summary>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (DataContext is INavigatable viewmodel)
			{
				viewmodel.OnNavigatedTo();
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			if (DataContext is INavigatable viewmodel)
			{
				viewmodel.OnNavigatedFrom();
			}

			base.OnNavigatedFrom(e);
		}

		private void RootGrid_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (sender is Grid grid)
			{
				if (grid.ActualWidth <= 870 )
				{
				
					Grid.SetColumn(_controlsItemsControl, 0);
					Grid.SetRow(_controlsItemsControl,1);
					_controlsItemsControl.MaxWidth = 600;
					_controlsItemsControl.Margin = new Thickness(10, 10, -15, 0);
				}
				else if (grid.ActualWidth > 870 )
				{
					Grid.SetColumn(_controlsItemsControl, 1);
					Grid.SetRow(_controlsItemsControl, 0);
					_controlsItemsControl.MaxWidth = 900;
					_controlsItemsControl.Margin = new Thickness(-5, -5, 0, 0);
				}
			}
		}
	}
}