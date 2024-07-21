using AdbCore.Abstraction;

namespace AdbCore.Models
{
	public class BatteryInfo : IBatteryInfo
	{
		public double VoltageCharging { get; set; } // in Volts
		public double BatteryMAH { get; set; } // in milliampere-hours (mAh)
		public double Temperature { get; set; } // in Celsius
		public double BatteryLevel { get; set; } // in percentage
		public bool IsWirelessCharging { get; set; }
		public bool IsUSBConnected { get; set; }
		public bool IsACPowered { get; set; }
		public double AmpsChargingRate { get; set; } // in Amperes
		public double ChargingWatts { get; set; } // in Watts
	}
}