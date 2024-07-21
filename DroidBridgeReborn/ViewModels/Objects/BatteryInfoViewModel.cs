using AdbCore.Abstraction;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DroidBridgeReborn.ViewModels.Objects
{
	public class BatteryInfoViewModel : ObservableObject, IBatteryInfo
	{
		private double _voltageCharging;
		private double _batteryMah;
		private double _temperature;
		private double _batteryLevel;
		private bool _isWirelessCharging;
		private bool _isUsbConnected;
		private bool _isAcPowered;
		private double _ampsChargingRate;
		private double _chargingWatts;

		public double VoltageCharging {
			get => _voltageCharging;
			set => SetProperty(ref _voltageCharging, value);
		}

		public double BatteryMAH {
			get => _batteryMah;
			set => SetProperty(ref _batteryMah, value);
		}

		public double Temperature {
			get => _temperature;
			set => SetProperty(ref _temperature, value);
		}

		public double BatteryLevel {
			get => _batteryLevel;
			set => SetProperty(ref _batteryLevel, value);
		}

		public bool IsWirelessCharging
		{
			get => _isWirelessCharging;
			set
			{
				SetProperty(ref _isWirelessCharging, value);
				OnPropertyChanged(nameof(IsCharging));
			}
		}

		public bool IsUSBConnected
		{
			get => _isUsbConnected;
			set
			{
				SetProperty(ref _isUsbConnected, value);
				OnPropertyChanged(nameof(IsCharging));
			}
		}

		public bool IsACPowered
		{
			get => _isAcPowered;
			set
			{
				SetProperty(ref _isAcPowered, value);
				OnPropertyChanged(nameof(IsCharging));
			}
		}

		public bool IsCharging => IsACPowered || IsUSBConnected || IsWirelessCharging;

		public double AmpsChargingRate {
			get => _ampsChargingRate;
			set => SetProperty(ref _ampsChargingRate, value);
		}

		public double ChargingWatts {
			get => _chargingWatts;
			set => SetProperty(ref _chargingWatts, value);
		}

		public void UpdateBatteryInfo(IBatteryInfo batteryInfo)
		{
			AmpsChargingRate = batteryInfo.AmpsChargingRate;
			BatteryLevel = batteryInfo.BatteryLevel;
			BatteryMAH = batteryInfo.BatteryMAH;
			ChargingWatts = batteryInfo.ChargingWatts;
			IsACPowered = batteryInfo.IsACPowered;
			IsUSBConnected = batteryInfo.IsUSBConnected;
			IsWirelessCharging = batteryInfo.IsWirelessCharging;
			Temperature = batteryInfo.Temperature;
			VoltageCharging = batteryInfo.VoltageCharging;
		}
	}
}