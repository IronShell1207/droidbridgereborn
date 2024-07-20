using DroidBridgeReborn.ViewModels;

namespace DroidBridgeReborn.Services
{
	public class ViewModelLocator
	{
		public NavigationHubViewModel GetNavigationHubViewModel => NavigationHubViewModel.Instance;

		public AdbServiceViewModel GetAdbServiceViewModel => AdbServiceViewModel.Instance;
	}
}