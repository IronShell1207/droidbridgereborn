using System;
using CustomControlsLib.Enums;
using CustomControlsLib.Helpers;
using DroidBridgeReborn.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DroidBridgeReborn
{
	using DroidBridgeReborn.Helpers;
	using DroidBridgeReborn.Views.Pages;
	using Microsoft.UI.Xaml;
	using WinUIEx;

	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : WindowEx
	{
		public MainWindow()
		{
			this.InitializeComponent();
			ExtendsContentIntoTitleBar = true;
		}

		/// <summary>
		/// Обрабатывает загрузку.
		/// </summary>
		private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
		{
			NavigationHelper.SetupNavigationFrame(_mainFrame);
			CustomDialogsHelper.SetDialogContainer(_dialogsPresenter);
			
			NavigationHubViewModel.Instance.OnChangeSelectedTab(NavBarItem.AdbPage.ToString());
		}

		private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (sender is Grid grid)
			{
				_windowSizeTb.Text = $"{Math.Round(grid.ActualWidth,2)}x{Math.Round(grid.ActualHeight, 2)}";
			}
		}
	}
}