using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AdbCore.Abstraction;
using AdbCore.Enums;
using AdbCore.Exceptions;
using AdbCore.Models;
using AdbCore.Service.Base;
using Microsoft.AppCenter.Crashes;
using Serilog;

namespace AdbCore.Service
{
	
	public class AdbDevicesListUpdaterService : PeriodicUpdatedService
	{
		private IADBServiceMonitor _adbServiceMonitor;
		private AndroidBridgeCommandExecutor _commandExecutor;
		private SemaphoreSlim _semaphore = new SemaphoreSlim(1);
	
		private List<IDevice> _connectedDevices = new List<IDevice>();

		public event Action<IDevice> DeviceUpdated;
		public event Action<IDevice> DeviceRemoved;
		public event Action<IDevice> DeviceAdded;
		public event Action<AdbCommandExecutionException> ErrorOccurred;
		/// <summary>
		/// Инициализирует экземпляр <see cref="AdbDevicesListUpdaterService"/>.
		/// </summary>
		public AdbDevicesListUpdaterService(IADBServiceMonitor adbServiceMonitor, AndroidBridgeCommandExecutor commandExecutor)
		{
			_adbServiceMonitor = adbServiceMonitor;
			_commandExecutor = commandExecutor;
			_adbServiceMonitor.OnADBStatusChanged += _adbServiceMonitor_OnADBStatusChanged;
		}


		private void _adbServiceMonitor_OnADBStatusChanged(bool isRunning)
		{
			if (isRunning)
			{
				StartMonitoring();
			}
			else
			{
				StopMonitoring();
				_connectedDevices.Clear();
			}
		}

		protected override async void OnUpdateAction(object state)
		{
			try
			{
				await _semaphore.WaitAsync();
				if (!_adbServiceMonitor.IsRunning || _commandExecutor.CommandRunning)
					return;

				await UpdateDevicesListAsync();
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
				Crashes.TrackError(ex);
			}
			finally
			{
				_semaphore.Release();
			}
		}

		private async Task UpdateDevicesListAsync()
		{

			AdbOutput output = await _commandExecutor.GetCommandResultAsync("devices -l");
			if (output.IsError)
			{
				ErrorOccurred?.Invoke(new AdbCommandExecutionException(output.ErrorOutput));
				return;
			}
			var newDeviceList = ParseDevices(output.Lines);

			// Find removed devices
			var removedDevices = _connectedDevices.Where(d => newDeviceList.All(nd => nd.DeviceId != d.DeviceId)).ToList();
			foreach (var device in removedDevices)
			{
				_connectedDevices.Remove(device);
				DeviceRemoved?.Invoke(device);
			}

			// Find added or updated devices
			foreach (var newDevice in newDeviceList)
			{
				var existingDevice = _connectedDevices.FirstOrDefault(d => d.DeviceId == newDevice.DeviceId);
				if (existingDevice == null)
				{
					_connectedDevices.Add(newDevice);
					DeviceAdded?.Invoke(newDevice);
				}
				else if (HasDeviceChanged(existingDevice, newDevice))
				{
					UpdateDevice(existingDevice, newDevice);
					DeviceUpdated?.Invoke(existingDevice);
				}
			}
		}

		private List<IDevice> ParseDevices(string output)
		{
			var devices = new List<IDevice>();
			var lines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var line in lines.Skip(1))
			{
				var match = Regex.Match(line, @"^(?<deviceId>\S+)\s+(?<state>\S+)(?:\s+product:(?<product>\S+))?(?:\s+model:(?<model>\S+))?(?:\s+device:(?<device>\S+))?(?:\s+transport_id:\S+)?");
				if (match.Success)
				{
					var device = new DeviceModel
					{
						DeviceId = match.Groups["deviceId"].Value,
						DeviceState = ParseDeviceState(match.Groups["state"].Value),
						DeviceCodeName = match.Groups["product"].Value,
						DeviceModelName = match.Groups["model"].Value
					};
					devices.Add(device);
				}
			}
			return devices;
		}

		private DeviceStateType ParseDeviceState(string state)
		{
			return state switch
			{
				"device" => DeviceStateType.Device,
				"unauthorized" => DeviceStateType.Unauthorized,
				"offline" => DeviceStateType.Offline,
				"recovery" => DeviceStateType.Recovery,
				"bootloader" => DeviceStateType.Bootloader,
				"no permissions" => DeviceStateType.NoPermissions,
				"sideload" => DeviceStateType.Sideload,
				_ => DeviceStateType.Unknown,
			};
		}
		private bool HasDeviceChanged(IDevice existingDevice, IDevice newDevice)
		{
			return existingDevice.DeviceState != newDevice.DeviceState ||
			       existingDevice.DeviceCodeName != newDevice.DeviceCodeName ||
			       existingDevice.DeviceModelName != newDevice.DeviceModelName;
		}

		private void UpdateDevice(IDevice existingDevice, IDevice newDevice)
		{
			existingDevice.DeviceState = newDevice.DeviceState;
			existingDevice.DeviceCodeName = newDevice.DeviceCodeName;
			existingDevice.DeviceModelName = newDevice.DeviceModelName;
		}


		public async Task<Dictionary<string, string>> GetDevicePropertiesAsync(string deviceId)
		{
			var command = $"-s {deviceId} shell getprop";
			var output = await _commandExecutor.GetCommandResultAsync(command);
			if (output == null || output.IsError)
			{
				ErrorOccurred?.Invoke(new AdbCommandExecutionException(output?.ErrorOutput));
				return null;
			}

			var properties = new Dictionary<string, string>();
			var lines = output.Lines.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var line in lines)
			{
				var match = Regex.Match(line, @"\[(?<key>.*?)\]: \[(?<value>.*?)\]");
				if (match.Success)
				{
					properties[match.Groups["key"].Value] = match.Groups["value"].Value;
				}
			}

			return properties;
		}

		public async Task<BatteryInfo> GetBatteryInfoAsync(string deviceId)
		{
			var command = $"-s {deviceId} shell dumpsys battery";
			var output = await _commandExecutor.GetCommandResultAsync(command);
			if (output.IsError)
			{
				ErrorOccurred?.Invoke(new AdbCommandExecutionException(output.ErrorOutput));
				return null;
			}
			var batteryInfo = new BatteryInfo();
			double scale = 100; // Default scale value
			var lines = output.Lines.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var line in lines)
			{
				var parts = line.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length < 2)
					continue;

				var key = parts[0].Trim();
				var value = parts[1].Trim();

				switch (key)
				{
					case "AC powered":
					batteryInfo.IsACPowered = value.Equals("true", StringComparison.OrdinalIgnoreCase);
					break;
					case "USB powered":
					batteryInfo.IsUSBConnected = value.Equals("true", StringComparison.OrdinalIgnoreCase);
					break;
					case "Wireless powered":
					batteryInfo.IsWirelessCharging = value.Equals("true", StringComparison.OrdinalIgnoreCase);
					break;
					case "Max charging current":
					batteryInfo.AmpsChargingRate = ParseToDouble(value) / 1000/1000; // Convert mA to A
					break;
					case "Max charging voltage":
					batteryInfo.VoltageCharging = ParseToDouble(value) / 1000/1000; // Convert mV to V
					break;
					case "scale":
						scale = double.Parse(value);
						break;
					case "Charge counter":
					batteryInfo.BatteryMAH = ParseToDouble(value) / 1000; // Convert µAh to mAh
					break;
					case "level":
					batteryInfo.BatteryLevel = double.Parse(value);
					break;
					case "voltage":
					batteryInfo.VoltageCharging = ParseToDouble(value) / 1000; // Convert mV to V
					break;
					case "temperature":
					batteryInfo.Temperature = ParseToDouble(value) / 10; // Convert tenths of degree Celsius to degree Celsius
					break;
					
				}
			}

			batteryInfo.BatteryLevel = (batteryInfo.BatteryLevel  / scale)*100;

			// Calculate Charging Watts
			batteryInfo.ChargingWatts = (batteryInfo.VoltageCharging * batteryInfo.AmpsChargingRate);

			return batteryInfo;
		}

		private double ParseToDouble(string value)
		{
			if (double.TryParse(value, out double result))
			{
				return result;
			}
			return 0;
		}
	}
}
