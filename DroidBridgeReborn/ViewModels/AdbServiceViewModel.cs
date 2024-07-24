using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using AdbCore.Abstraction;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Win.Utils.Helpers;
using AdbCore.Service;

namespace DroidBridgeReborn.ViewModels
{
	public class AdbServiceViewModel : ObservableObject
	{
		/// <summary>
		/// Статичный экземпляр класса <see cref="AdbServiceViewModel"/>.
		/// </summary>
		private static readonly Lazy<AdbServiceViewModel> _instance = new((() => new AdbServiceViewModel()));

		/// <summary>
		/// Статичный экземпляр класса <see cref="AdbServiceViewModel"/>.
		/// </summary>
		public static AdbServiceViewModel Instance => _instance.Value;

		/// <inheritdoc cref="IsServerRunning"/>
		private bool _isServerRunning;

		/// <summary>
		/// Запущен ли adb.
		/// </summary>
		public bool IsServerRunning {
			get => _isServerRunning;
			set => SetProperty(ref _isServerRunning, value);
		}

		/// <inheritdoc cref="ServerExecutablePath"/>
		private string _serverExecutablePath;

		/// <summary>
		/// Текущий экземпляр сервиса.
		/// </summary>
		public string ServerExecutablePath
		{
			get => _serverExecutablePath;
			set => SetProperty(ref _serverExecutablePath, value);
		}

		/// <summary>
		/// Инициализирует экземпляр <see cref="AdbServiceViewModel"/>.
		/// </summary>
		public AdbServiceViewModel()
		{
			StartStopServerAsyncCommand = new AsyncRelayCommand(OnStartStopServerAsync);
			var adbMonService = App.ServiceProvider.GetRequiredService<IADBServiceMonitor>();
			adbMonService.OnADBStatusChanged += AdbMonService_OnADBStatusChanged;
			adbMonService.OnADBPathChanged += AdbMonService_OnADBPathChanged;

			ServerExecutablePath = adbMonService.CurrentAdbPath;
			IsServerRunning = adbMonService.IsRunning;
		}


		/// <summary>
		/// Команда выполняющая метод <see cref="OnStartStopServerAsync"/>.
		/// </summary>
		public AsyncRelayCommand StartStopServerAsyncCommand { get; }


		/// <summary>
		/// Включает или останавливает серве
		/// </summary>
		private async Task OnStartStopServerAsync()
		{
			var cmdsExecutor = App.ServiceProvider.GetRequiredService<AndroidBridgeCommandExecutor>();
			if (IsServerRunning)
			{
				await cmdsExecutor.StopServerAndKillAllProcesses();
				IsServerRunning = false;
			}
			else
			{
				await cmdsExecutor.StartAdbServer();
			}
		}
  

		private void AdbMonService_OnADBPathChanged(string path)
		{
			DispatcherQueueHelper.EnqueueAsync(() =>
			{
				ServerExecutablePath = path;
			});
		}

		private void AdbMonService_OnADBStatusChanged(bool isRunning)
		{
			DispatcherQueueHelper.EnqueueAsync(() =>
			{
				IsServerRunning = isRunning;
			});
		}
	}
}