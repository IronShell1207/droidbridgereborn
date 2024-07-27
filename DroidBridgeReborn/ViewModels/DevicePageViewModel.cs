using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdbCore.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CustomControlsLib.Abstract;
using DroidBridgeReborn.Helpers;
using DroidBridgeReborn.Services;
using DroidBridgeReborn.ViewModels.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Win.Utils.Helpers;

namespace DroidBridgeReborn.ViewModels
{
	using AdbCore.Abstraction;
	using System;

	public class DevicePageViewModel : ObservableObject, INavigatable
	{
		/// <summary>
		/// Статичный экземпляр класса <see cref="DevicePageViewModel"/>.
		/// </summary>
		private static readonly Lazy<DevicePageViewModel> _instance = new((() => new DevicePageViewModel()));

		private DispatcherTimer _deviceInfoUpdateTimer = new DispatcherTimer();

		/// <inheritdoc cref="SelectedDevice"/>
		private DeviceViewModel _selectedDevice = DeviceViewModel.Empty;

		/// <summary>
		/// Инициализирует экземпляр <see cref="DevicePageViewModel"/>.
		/// </summary>
		public DevicePageViewModel()
		{
			DevicesListViewModel.Instance.Devices.CollectionChanged += Devices_CollectionChanged;
			_deviceInfoUpdateTimer.Interval = TimeSpan.FromSeconds(10);
			_deviceInfoUpdateTimer.Tick += DeviceInfoUpdateTimer_Tick;
			var devicesUpdateService = App.ServiceProvider.GetRequiredService<AdbDevicesListUpdaterService>();
			devicesUpdateService.ErrorOccurred += DevicesUpdateService_ErrorOccurred;
			var adbServiceupdater = App.ServiceProvider.GetRequiredService<IADBServiceMonitor>();
			adbServiceupdater.OnADBStatusChanged += AdbServiceupdater_OnADBStatusChanged;
		}

		/// <summary>
		/// Статичный экземпляр класса <see cref="DevicePageViewModel"/>.
		/// </summary>
		public static DevicePageViewModel Instance => _instance.Value;

		public bool DeviceNotConnected => SelectedDevice == DeviceViewModel.Empty;

		/// <summary>
		/// Выбранный девайс.
		/// </summary>
		public DeviceViewModel SelectedDevice {
			get => _selectedDevice;
			set {
				
					SetProperty(ref _selectedDevice, value);
					OnPropertyChanged(nameof(DeviceNotConnected));
					Task.Run(async()=> await DispatcherQueueHelper.EnqueueAsync(UpdateDeviceProps));
			}
		}

		public void OnNavigatedFrom()
		{
			var devicesUpdateService = App.ServiceProvider.GetRequiredService<AdbDevicesListUpdaterService>();
			devicesUpdateService.ErrorOccurred -= DevicesUpdateService_ErrorOccurred;
			_deviceInfoUpdateTimer.Stop();
		}

		public async void OnNavigatedTo()
		{
			var devicesUpdateService = App.ServiceProvider.GetRequiredService<AdbDevicesListUpdaterService>();
			devicesUpdateService.ErrorOccurred += DevicesUpdateService_ErrorOccurred;
			if (SelectedDevice == DeviceViewModel.Empty)
			{
				if (DevicesListViewModel.Instance.Devices.Any())
				{
					SelectedDevice = DevicesListViewModel.Instance.Devices[0];
					await UpdateDeviceProps();
				}
			}

			_deviceInfoUpdateTimer.Start();
		}

		private void AdbServiceupdater_OnADBStatusChanged(bool isRunning)
		{
			if (isRunning == false)
			{
				SelectedDevice = DeviceViewModel.Empty;
			}
		}

		private async void DeviceInfoUpdateTimer_Tick(object sender, object e)
		{
			await UpdateDeviceBatteryInfo();
		}

		private async void Devices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (DevicesListViewModel.Instance.Devices.Any())
			{
				if (SelectedDevice == DeviceViewModel.Empty || SelectedDevice == null)
				{
					_selectedDevice = DevicesListViewModel.Instance.Devices.FirstOrDefault();
					OnPropertyChanged(string.Empty);
					await UpdateDeviceProps();
					await UpdateDeviceBatteryInfo();
				}
			}
			else
			{
				_selectedDevice = DeviceViewModel.Empty;
				OnPropertyChanged(string.Empty);
			}
		}

		private async void DevicesUpdateService_DeviceRemoved(AdbCore.Abstraction.IDevice device)
		{
			if (SelectedDevice.DeviceId == device.DeviceId)
			{
				SelectedDevice = DeviceViewModel.Empty;
				await UpdateDeviceProps();
			}
		}

		private void DevicesUpdateService_ErrorOccurred(AdbCore.Exceptions.AdbCommandExecutionException obj)
		{
			ToastNotificationService.ShowToastNotify(obj.Message, TimeSpan.FromSeconds(1.7));
		}

		private async Task UpdateDeviceBatteryInfo()
		{
			if (SelectedDevice != null && string.IsNullOrWhiteSpace(SelectedDevice?.DeviceId) == false && SelectedDevice != DeviceViewModel.Empty)
			{
				var devicesUpdateService = App.ServiceProvider.GetRequiredService<AdbDevicesListUpdaterService>();
				var batteryInfo = await devicesUpdateService.GetBatteryInfoAsync(SelectedDevice.DeviceId);
				if (batteryInfo != null)
				{
					SelectedDevice.BatteryInfo?.UpdateBatteryInfo(batteryInfo);
				}
			}
		}

		private async Task UpdateDeviceProps()
		{
			if (SelectedDevice != null & string.IsNullOrWhiteSpace(SelectedDevice?.DeviceId) == false)
			{
				var devicesUpdateService = App.ServiceProvider.GetRequiredService<AdbDevicesListUpdaterService>();
				var deviceProps = await devicesUpdateService.GetDevicePropertiesAsync(SelectedDevice.DeviceId);
				if (deviceProps != null && deviceProps.Any())
				{
					SelectedDevice.DeviceManufacturer = deviceProps.GetValueOrDefault("ro.product.manufacturer");
					SelectedDevice.DeviceRealName = deviceProps.GetValueOrDefault("ro.product.model");
					SelectedDevice.AndroidVersion = deviceProps.GetValueOrDefault("ro.build.version.release");
					SelectedDevice.ChipName = deviceProps.GetValueOrDefault("ro.hardware.chipname");
				}
				await UpdateDeviceBatteryInfo();
			}
		}
	}
}