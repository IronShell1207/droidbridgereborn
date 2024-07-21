using AdbCore.Abstraction;
using AdbCore.Enums;

namespace AdbCore.Models
{
	public class DeviceModel : IDevice
	{
		public DeviceStateType DeviceState { get; set; }
		public string DeviceId { get; set; }
		public string DeviceCodeName { get; set; }
		public string DeviceModelName { get; set; }
	}
}