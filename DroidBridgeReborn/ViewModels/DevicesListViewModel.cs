using AdbCore.Abstraction;
using AdbCore.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomControlsLib.Helpers;
using DroidBridgeReborn.ViewModels.Dialogs;
using DroidBridgeReborn.ViewModels.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DroidBridgeReborn.ViewModels
{
	public class DevicesListViewModel : ObservableObject
	{
		/// <summary>
		/// Статичный экземпляр класса <see cref="DevicesListViewModel"/>.
		/// </summary>
		private static readonly Lazy<DevicesListViewModel> _instance = new((() => new DevicesListViewModel()));

		/// <inheritdoc cref="DeviceConnectFieldText"/>
		private string _deviceConnectFieldText = string.Empty;

		private AdbDevicesListUpdaterService _devicesListUpdaterService;
		private object _deviceUpdateLock = new object();

		/// <summary>
		/// Инициализирует экземпляр <see cref="DevicesListViewModel"/>.
		/// </summary>
		public DevicesListViewModel()
		{
			OpenDevicesPickerDialogAsyncCommand = new AsyncRelayCommand(OnOpenDevicesPickerDialogAsync);
			ScanNetworkForDevicesAsyncCommand = new AsyncRelayCommand(OnScanNetworkForDevicesAsync);
			ConnectDevicAsyncCommand = new AsyncRelayCommand(OnConnectDeviceAsync, CanConnectADevice);

			var adbServiceupdater = App.ServiceProvider.GetRequiredService<IADBServiceMonitor>();
			adbServiceupdater.OnADBStatusChanged += AdbServiceupdater_OnADBStatusChanged;

			_devicesListUpdaterService = App.ServiceProvider.GetRequiredService<AdbDevicesListUpdaterService>();
			_devicesListUpdaterService.DeviceAdded += DevicesListUpdaterService_DeviceAdded;
			_devicesListUpdaterService.DeviceRemoved += DevicesListUpdaterService_DeviceRemoved;
			_devicesListUpdaterService.DeviceUpdated += DevicesListUpdaterService_DeviceAdded;
		}

		/// <summary>
		/// Статичный экземпляр класса <see cref="DevicesListViewModel"/>.
		/// </summary>
		public static DevicesListViewModel Instance => _instance.Value;

		/// <summary>
		/// Команда выполняющая метод <see cref="OnConnectDeviceAsync"/>.
		/// </summary>
		public AsyncRelayCommand ConnectDevicAsyncCommand { get; }

		/// <summary>
		/// Text field to connect a new one.
		/// </summary>
		public string DeviceConnectFieldText {
			get => _deviceConnectFieldText;
			set {
				SetProperty(ref _deviceConnectFieldText, value);
				ConnectDevicAsyncCommand.NotifyCanExecuteChanged();
			}
		}

		public ObservableCollection<DeviceViewModel> Devices { get; set; } =
											new ObservableCollection<DeviceViewModel>();

		/// <summary>
		/// Команда выполняющая метод <see cref="OnOpenDevicesPickerDialogAsync"/>.
		/// </summary>
		public AsyncRelayCommand OpenDevicesPickerDialogAsyncCommand { get; }

		/// <summary>
		/// Команда выполняющая метод <see cref="OnScanNetworkForDevicesAsync"/>.
		/// </summary>
		public AsyncRelayCommand ScanNetworkForDevicesAsyncCommand { get; }

		private void AdbServiceupdater_OnADBStatusChanged(bool isRunning)
		{
			lock (_deviceUpdateLock)
			{
				if (isRunning == false)
				{
					Devices.Clear();
				}
			}
		}

		private bool CanConnectADevice()
		{
			return string.IsNullOrWhiteSpace(DeviceConnectFieldText) == false;
		}

		private void DevicesListUpdaterService_DeviceAdded(AdbCore.Abstraction.IDevice newDevice)
		{
			lock (_deviceUpdateLock)
			{
				if (Devices.Any(x => x.DeviceId == newDevice.DeviceId))
				{
					var updatedDevice = Devices.FirstOrDefault(x => x.DeviceId == newDevice.DeviceId);
					updatedDevice.DeviceState = newDevice.DeviceState;
					updatedDevice.DeviceCodeName = newDevice.DeviceCodeName;
					updatedDevice.DeviceId = newDevice.DeviceId;
					updatedDevice.DeviceModelName = newDevice.DeviceModelName;

					return;
				}

				Devices.Add(new DeviceViewModel(newDevice));
			}
		}

		private void DevicesListUpdaterService_DeviceRemoved(IDevice obj)
		{
			lock (_deviceUpdateLock)
			{
				var deviceToRemove = Devices.FirstOrDefault(x => x.DeviceId == obj.DeviceId);
				if (deviceToRemove != null)
				{
					Devices.Remove(deviceToRemove);
				}
			}
		}

		/// <summary>
		/// Connecting a device
		/// </summary>
		private async Task OnConnectDeviceAsync()
		{
			var adbCommandsExecutor = App.ServiceProvider.GetRequiredService<AndroidBridgeCommandExecutor>();
			await adbCommandsExecutor.GetCommandResultAsync($"connect {DeviceConnectFieldText}");
			_devicesListUpdaterService.OnUpdateAction(null);
		}

		/// <summary>
		/// Открывает диалог девайсов
		/// </summary>
		private async Task OnOpenDevicesPickerDialogAsync()
		{
			var devicesDialogVm = new DevicesListDialogViewModel();
			await devicesDialogVm.ShowCustomDialog();
		}

		/// <summary>
		/// scan network for devices
		/// </summary>
		private async Task OnScanNetworkForDevicesAsync()
		{
			var dialogVm = new NetworkScannerDialogViewModel();
			var result = await dialogVm.ShowCustomDialog(dialogVm.ScanNetworkAsync);
			if (result == ContentDialogResult.Primary && dialogVm.SelectedDeviceIndex > -1)
			{
				DeviceConnectFieldText = dialogVm.NetworkDevices[dialogVm.SelectedDeviceIndex];
				await OnConnectDeviceAsync();
			}
		}
	}
}