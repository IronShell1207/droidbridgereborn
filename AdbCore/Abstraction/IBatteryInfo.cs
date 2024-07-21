namespace AdbCore.Abstraction
{
	public interface IBatteryInfo
	{
		double VoltageCharging { get; set; } // in Volts
		double BatteryMAH { get; set; } // in milliampere-hours (mAh)
		double Temperature { get; set; } // in Celsius
		double BatteryLevel { get; set; } // in percentage
		bool IsWirelessCharging { get; set; }
		bool IsUSBConnected { get; set; }
		bool IsACPowered { get; set; }
		double AmpsChargingRate { get; set; } // in Amperes
		double ChargingWatts { get; set; } // in Watts
	}
}