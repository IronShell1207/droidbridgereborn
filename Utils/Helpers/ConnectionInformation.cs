namespace Utils.Helpers
{
	using System.Collections.Generic;
	using Utils.Enums;
	using Windows.Networking.Connectivity;

	/// <summary>
	/// Информация сетевого подключения.
	/// </summary>
	public sealed class ConnectionInformation
	{
		private readonly List<string> _networkNames = new List<string>();

		/// <summary>
		/// Данные о стоимости сетевого подключения.
		/// </summary>
		public ConnectionCost ConnectionCost { get; private set; }

		/// <summary>
		/// Тип сетевого подключения.
		/// </summary>
		public ConnectionType ConnectionType { get; private set; }

		/// <summary>
		/// Уровень сетевого подключения.
		/// </summary>
		public NetworkConnectivityLevel ConnectivityLevel { get; private set; }

		/// <summary>
		/// Доступность сетевого подключения.
		/// </summary>
		public bool IsInternetAvailable { get; private set; }

		/// <summary>
		/// Сетевое подключение имеет ограничения.
		/// </summary>
		public bool IsInternetOnMeteredConnection => ConnectionCost != null && ConnectionCost.NetworkCostType != NetworkCostType.Unrestricted;

		/// <summary>
		/// Сетевые имена.
		/// </summary>
		public IReadOnlyList<string> NetworkNames => _networkNames.AsReadOnly();

		/// <summary>
		/// Уровень сигнала сетевого подключения.
		/// </summary>
		public byte? SignalStrength { get; private set; }

		/// <summary>
		/// Сбрасывает свойства соединение.
		/// </summary>
		internal void Reset()
		{
			_networkNames.Clear();

			ConnectionType = ConnectionType.NotConnected;
			ConnectivityLevel = NetworkConnectivityLevel.None;
			IsInternetAvailable = false;
			ConnectionCost = null;
			SignalStrength = null;
		}

		/// <summary>
		/// Обновляет свойства соединения.
		/// </summary>
		/// <param name="profile">Профиль соединения.</param>
		internal void Update(ConnectionProfile profile)
		{
			if (profile == null)
			{
				Reset();

				return;
			}

			_networkNames.Clear();

			uint ianaInterfaceType = profile.NetworkAdapter?.IanaInterfaceType ?? 0;

			ConnectionType = ianaInterfaceType switch
			{
				6 => ConnectionType.Ethernet,
				71 => ConnectionType.WiFi,
				243 or 244 => ConnectionType.Mobile,
				_ => ConnectionType.NotConnected,
			};

			IReadOnlyList<string> names = profile.GetNetworkNames();
			if (names?.Count > 0)
			{
				_networkNames.AddRange(names);
			}

			ConnectivityLevel = profile.GetNetworkConnectivityLevel();

			IsInternetAvailable = ConnectivityLevel switch
			{
				NetworkConnectivityLevel.None or NetworkConnectivityLevel.LocalAccess => false,
				_ => true,
			};

			ConnectionCost = profile.GetConnectionCost();
			SignalStrength = profile.GetSignalBars();
		}
	}
}
