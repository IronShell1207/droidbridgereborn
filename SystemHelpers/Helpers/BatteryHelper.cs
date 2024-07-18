using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Power;

namespace SystemHelpers.Helpers
{
	public class BatteryHelper
	{
		public static async Task<BatteryReport> GetDeviceBatteryInfoAsync()
		{
			var deviceInfo = await DeviceInformation.FindAllAsync(Battery.GetDeviceSelector());
			foreach (DeviceInformation device in deviceInfo)
			{
				try
				{
					// Create battery object
					var battery = await Battery.FromIdAsync(device.Id);

					// Get report
					var report = battery.GetReport();
					return report;
				}
				catch (Exception ex)
				{
					Serilog.Log.Logger.Error(ex, ex.Message);
					Microsoft.AppCenter.Crashes.Crashes.TrackError(ex);
				}
			}

			return null;
		}
	}
}