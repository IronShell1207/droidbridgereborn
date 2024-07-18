using CommunityToolkit.Mvvm.ComponentModel;

namespace DroidBridgeReborn.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	internal class DevicePageViewModel : ObservableObject
	{
		/// <summary>
		/// Статичный экземпляр класса <see cref="DevicePageViewModel"/>.
		/// </summary>
		private static readonly Lazy<DevicePageViewModel> _instance = new((() => new DevicePageViewModel()));

		/// <summary>
		/// Статичный экземпляр класса <see cref="DevicePageViewModel"/>.
		/// </summary>
		public static DevicePageViewModel Instance => _instance.Value;
	}
}
