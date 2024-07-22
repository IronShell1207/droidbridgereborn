using AdbCore.Abstraction;
using AdbCore.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DroidBridgeReborn.ViewModels.Objects
{
	public class DeviceViewModel : ObservableObject, IDevice
	{
		public BatteryInfoViewModel BatteryInfo { get; set; } = new BatteryInfoViewModel();

		public bool IsMoreInfoAvailable => DeviceState == DeviceStateType.Device;
		public static DeviceViewModel Empty { get; private set; } = new DeviceViewModel();

		/// <inheritdoc cref="DeviceState"/>
		private DeviceStateType _deviceState = DeviceStateType.Disconnected;

		/// <inheritdoc cref="DeviceCodeName"/>
		private string _deviceCodeName = "";

		/// <inheritdoc cref="DeviceName"/>
		private string _deviceName = "";

		/// <inheritdoc cref="DeviceRealName"/>
		private string _deviceRealName = "";

		public string DeviceId { get; set; } = "";

		/// <inheritdoc cref="AndroidVersion"/>
		private string _androidVersion;

		/// <summary>
		/// Версия android.
		/// </summary>
		public string AndroidVersion
		{
			get => _androidVersion;
			set => SetProperty(ref _androidVersion, value);
		}

		/// <inheritdoc cref="ChipName"/>
		private string _chipName;

		/// <summary>
		/// Процессор.
		/// </summary>
		public string ChipName
		{
			get => _chipName;
			set => SetProperty(ref _chipName, value);
		}

		/// <summary>
		/// Тип подключения.
		/// </summary>
		public DeviceStateType DeviceState
		{
			get => _deviceState;
			set
			{
				SetProperty(ref _deviceState, value);
				OnPropertyChanged(nameof(IsMoreInfoAvailable));
			}
		}

		/// <summary>
		/// Кодовое название.
		/// </summary>
		public string DeviceCodeName {
			get => _deviceCodeName;
			set => SetProperty(ref _deviceCodeName, value);
		}

		/// <summary>
		/// Название девайса.
		/// </summary>
		public string DeviceModelName {
			get => _deviceName;
			set => SetProperty(ref _deviceName, value);
		}

		/// <summary>
		/// Реальное название.
		/// </summary>
		public string DeviceRealName {
			get => _deviceRealName;
			set => SetProperty(ref _deviceRealName, value);
		}

		/// <inheritdoc cref="DeviceManufacturer"/>
		private string _deviceManufacturer;

		/// <summary>
		/// Производитель.
		/// </summary>
		public string DeviceManufacturer
		{
			get => _deviceManufacturer;
			set => SetProperty(ref _deviceManufacturer, value);
		}

		/// <summary>
		/// Инициализирует экземпляр <see cref="DeviceViewModel"/>.
		/// </summary>
		public DeviceViewModel()
		{ }

		/// <summary>
		/// Инициализирует экземпляр <see cref="DeviceViewModel"/>.
		/// </summary>
		public DeviceViewModel(IDevice device)
		{
			DeviceId = device.DeviceId;
			DeviceCodeName = device.DeviceCodeName;
			DeviceState = device.DeviceState;
			DeviceModelName = device.DeviceModelName;
		}
	}
}