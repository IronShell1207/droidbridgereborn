using CustomControlsLib.Helpers;

namespace DroidBridgeReborn
{
	using DroidBridgeReborn.Helpers;
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


		private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
		{
			NavigationHelper.SetupNavigationFrame(_mainFrame);
			CustomDialogsHelper.SetDialogContainer(_dialogsPresenter);
		}
	}
}