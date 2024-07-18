namespace Utils.Helpers
{
	using System.Threading.Tasks;
	using System;
	using Windows.Networking.Connectivity;
	using Windows.Web.Http;
	using Enums;
	using Serilog;

	/// <summary>
	/// Сервис слежения за состоянием интернета.
	/// </summary>
	public sealed class NetworkService
	{
	 
		/// <summary>
		/// Экземпляр <see cref="NetworkService"/>.
		/// </summary>
		private static readonly Lazy<NetworkService> _instance = new(() => new NetworkService());

		/// <see cref="_instance"/>
		public static NetworkService Instance => _instance.Value;

		/// <summary>
		/// Есть ли подключение к интернету.
		/// </summary>
		public bool _isWebsiteInternetEnabled;

		/// <summary>
		/// Вебсайт для проверки интернета.
		/// </summary>
		private const string CHECK_WEBSITE_CHINA = "https://www.apple.com/itunes/";

		/// <summary>
		/// Информация о интернет соединении.
		/// </summary>
		public ConnectionInformation ConnectionInformation { get; } = new();

		/// <summary>
		/// Включен ли интернет.
		/// </summary>
		public bool IsInternetEnabled => ConnectionInformation.IsInternetAvailable || _isWebsiteInternetEnabled;

		/// <summary>
		/// Изменено состояние интернет соединения.
		/// </summary>
		public event EventHandler<ConnectionType> ConnectionStateChanged;

		/// <summary>
		/// Инициализирует экземпляр <see cref="NetworkService"/>
		/// </summary>
		internal NetworkService()
		{
			UpdateConnectionInformation();
			NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
		}

		/// <summary>
		/// Деструктор
		/// </summary>
		~NetworkService()
		{
			NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
		}

		/// <summary>
		/// Проверяет доступность интернет-ресурса.
		/// </summary>
		public async Task<bool> CheckInternetAccessAsync(string url = null)
		{
			Uri requestUri;
			if (string.IsNullOrWhiteSpace(url))
			{
				requestUri = new(CHECK_WEBSITE_CHINA);
			}
			else
			{
				requestUri = new(url);
			}

			try
			{
				using HttpClient client = new();
				using HttpResponseMessage response = await client.GetAsync(requestUri);
				response.EnsureSuccessStatusCode();

				_isWebsiteInternetEnabled = true;

				Log.Logger.Debug("CheckInternetAccessAsync true");

				return true;
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
				_isWebsiteInternetEnabled = false;
				return false;
			}
		}

		/// <summary>
		/// Запускает циклическую проверку доступности интернет-ресурса.
		/// </summary>
		public async Task<bool> StartInternetCheckingAsync(string url, uint seconds)
		{
			DateTime targetTime = DateTime.Now.AddSeconds(seconds);
			while (DateTime.Now <= targetTime)
			{
				bool isAvailable = await CheckInternetAccessAsync(url);
				if (isAvailable)
				{
					return true;
				}

				await Task.Delay(300);
			}

			return false;
		}

		/// <summary>
		/// Обрабатывает событие изменения статуса интернет-соединения.
		/// </summary>
		/// <param name="sender">Объект, который вызвал событие.</param>
		private async void OnNetworkStatusChanged(object sender)
		{
			await UpdateConnectionInformation();
			ConnectionStateChanged?.Invoke(this, ConnectionInformation.ConnectionType);
		}

		/// <summary>
		/// Пытается обновить информацию о интернет соединении.
		/// </summary>
		private async Task UpdateConnectionInformation()
		{
			try
			{
				ConnectionInformation.Update(NetworkInformation.GetInternetConnectionProfile());

				await CheckInternetAccessAsync();
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
				ConnectionInformation.Reset();
			}
		}
	}
}
