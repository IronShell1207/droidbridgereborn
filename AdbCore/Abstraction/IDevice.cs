using AdbCore.Enums;

namespace AdbCore.Abstraction
{
	
	public interface IDevice
	{
		DeviceStateType DeviceState { get; set; }
		string DeviceId { get; set; }
		string DeviceCodeName { get; set; }
		string DeviceModelName { get; set; }
	}
}
