using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdbCore.Abstraction;
using AdbCore.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using DroidBridgeReborn.ViewModels.Objects;
using Microsoft.Extensions.DependencyInjection;
using Windows.Devices.Input;

namespace DroidBridgeReborn.ViewModels
{
	public class DevicesListViewModel : ObservableObject
	{
		private object _deviceUpdateLock = new object();
		public ObservableCollection<DeviceViewModel> Devices { get; set; } =
			new ObservableCollection<DeviceViewModel>();

		/// <summary>
		/// Статичный экземпляр класса <see cref="DevicesListViewModel"/>.
		/// </summary>
		private static readonly Lazy<DevicesListViewModel> _instance = new((() => new DevicesListViewModel()));

		/// <summary>
		/// Статичный экземпляр класса <see cref="DevicesListViewModel"/>.
		/// </summary>
		public static DevicesListViewModel Instance => _instance.Value;

		private AdbDevicesListUpdaterService _devicesListUpdaterService;
		/// <summary>
		/// Инициализирует экземпляр <see cref="DevicesListViewModel"/>.
		/// </summary>
		public DevicesListViewModel()
		{
			var adbServiceupdater = App.ServiceProvider.GetRequiredService<IADBServiceMonitor>();
			adbServiceupdater.OnADBStatusChanged += AdbServiceupdater_OnADBStatusChanged;
			var devicesListUpdaterService = App.ServiceProvider.GetRequiredService<AdbDevicesListUpdaterService>();
			devicesListUpdaterService.DeviceAdded += DevicesListUpdaterService_DeviceAdded;
			devicesListUpdaterService.DeviceRemoved += DevicesListUpdaterService_DeviceRemoved;
			devicesListUpdaterService.DeviceUpdated += DevicesListUpdaterService_DeviceAdded;
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
	}
}
