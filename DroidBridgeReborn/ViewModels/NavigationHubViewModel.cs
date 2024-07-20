using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroidBridgeReborn.Helpers;

namespace DroidBridgeReborn.ViewModels
{
	using CustomControlsLib.Enums;
	using DroidBridgeReborn.Views.Pages;
	using System;

	public class NavigationHubViewModel : ObservableObject
	{
		/// <summary>
		/// Статичный экземпляр класса <see cref="NavigationHubViewModel"/>.
		/// </summary>
		private static readonly Lazy<NavigationHubViewModel> _instance = new((() => new NavigationHubViewModel()));

		private NavBarItem _currentPageName = NavBarItem.None;

		private Dictionary<NavBarItem, Type> _navBarItems = new Dictionary<NavBarItem, Type>();

		/// <summary>
		/// Инициализирует экземпляр <see cref="NavigationHubViewModel"/>.
		/// </summary>
		public NavigationHubViewModel()
		{
			ChangeSelectedTabCommand = new RelayCommand<object>(OnChangeSelectedTab);
			CollectNavItems();
		}

		/// <summary>
		/// Статичный экземпляр класса <see cref="NavigationHubViewModel"/>.
		/// </summary>
		public static NavigationHubViewModel Instance => _instance.Value;

		/// <summary>
		/// Команда выполняющая метод <see cref="OnChangeSelectedTab"/>.
		/// </summary>
		public RelayCommand<object> ChangeSelectedTabCommand { get; }

		public NavBarItem SelectedPageType {
			get => _currentPageName;
			set => SetProperty(ref _currentPageName, value);
		}

		private void CollectNavItems()
		{
			_navBarItems.Add(NavBarItem.AdbPage, typeof(AdbSetupPage));
			_navBarItems.Add(NavBarItem.DevicePage, typeof(DevicePage));
			_navBarItems.Add(NavBarItem.SettingsPage, typeof(SettingsPage));
		}

		/// <summary>
		/// Переключает вкладку
		/// </summary>
		public void OnChangeSelectedTab(object parameter)
		{
			if (parameter is string pageName && NavBarItem.TryParse(pageName, out NavBarItem selectedItem))
			{
				if (SelectedPageType == selectedItem)
					return;

				SelectedPageType = selectedItem;

				if (_navBarItems.TryGetValue(selectedItem, out Type pageType))
				{
					NavigationHelper.NavigateTo(pageType);
				}
			}
		}
	}
}