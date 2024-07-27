using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdbCore.Exceptions;
using AdbCore.Service;
using CommunityToolkit.Mvvm.Input;
using DroidBridgeReborn.Views.Controls.Dialogs.Contents;
using Microsoft.AppCenter.Crashes;
using Microsoft.UI.Xaml.Controls;
using Serilog;

namespace DroidBridgeReborn.ViewModels.Dialogs
{
	public class NetworkScannerDialogViewModel : BaseDialogViewModel
	{
		/// <inheritdoc cref="IsNotConnected"/>
		private bool _isNotConnected;

		/// <inheritdoc cref="IsScanning"/>
		private bool _isScanning;

		/// <inheritdoc cref="SelectedDeviceIndex"/>
		private int _selectedDeviceIndex = -1;

		private CancellationTokenSource _tokenSource;

		/// <summary>
		/// Инициализирует экземпляр <see cref="NetworkScannerDialogViewModel"/>.
		/// </summary>
		public NetworkScannerDialogViewModel()
		{
			ChooseDeviceToConnectCommand = new RelayCommand(OnChooseDeviceToConnect, ()=>SelectedDeviceIndex >-1);
			ScanDevicesAsyncRelayCommand = new AsyncRelayCommand(ScanNetworkAsync);
			CancelScanningCommand = new RelayCommand(OnCancelScanning);
			DialogTitle = "Scan for devices";
			NetworkDevices.CollectionChanged += NetworkDevices_CollectionChanged;

			var dialogContentView = new NetworkScanDialogView();
			dialogContentView.DataContext = this;
			DialogContent = dialogContentView;
		}



		/// <summary>
		/// Команда выполняющая метод <see cref="OnCancelScanning"/>.
		/// </summary>
		public RelayCommand CancelScanningCommand { get; }

		/// <summary>
		/// Нет ли устройств.
		/// </summary>
		public bool IsNoDevices => NetworkDevices.Count == 0 && !IsScanning;

		/// <summary>
		/// Нет ли соединения.
		/// </summary>
		public bool IsNotConnected {
			get => _isNotConnected;
			set => SetProperty(ref _isNotConnected, value);
		}

		/// <summary>
		/// Идет ли сканирование.
		/// </summary>
		public bool IsScanning {
			get => _isScanning;
			set => SetProperty(ref _isScanning, value);
		}

		public ObservableCollection<string> NetworkDevices { get; set; } = new ObservableCollection<string>();

		public AsyncRelayCommand ScanDevicesAsyncRelayCommand { get; set; }

		/// <summary>
		/// Index of selected device.
		/// </summary>
		public int SelectedDeviceIndex
		{
			get => _selectedDeviceIndex;
			set
			{
				SetProperty(ref _selectedDeviceIndex, value);
				ChooseDeviceToConnectCommand.NotifyCanExecuteChanged();
			}
		}

		public override void OnCancel()
		{
			base.OnCancel();
			if (_tokenSource != null)
			{
				_tokenSource.Cancel();
			}
		}

		public async Task ScanNetworkAsync()
		{
			try
			{
				IsNotConnected = false;
				IsScanning = true;
				SelectedDeviceIndex = -1;
				_tokenSource = new CancellationTokenSource();
				NetworkDevices.Clear();

				var devices = await NetworkPingHelper.GetAndroidDevicesAsync(_tokenSource.Token);
				if (devices != null)
				{
					devices.ForEach(NetworkDevices.Add);
				}

				if (NetworkDevices.Any())
				{
					SelectedDeviceIndex = 0;
				}
			}
			catch (NoConnectionException)
			{
				IsNotConnected = true;
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
				Crashes.TrackError(ex);
			}
			finally
			{
				_tokenSource.Dispose();
				_tokenSource = null;
				IsScanning = false;
			}
		}

		private void NetworkDevices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(IsNoDevices));
		}

		/// <summary>
		/// Отменяет сканирование
		/// </summary>
		private void OnCancelScanning()
		{
			if (_tokenSource != null)
			{
				_tokenSource.Cancel();
			}
		}

		/// <summary>
		/// Команда выполняющая метод <see cref="OnChooseDeviceToConnect"/>.
		/// </summary>
		public RelayCommand ChooseDeviceToConnectCommand { get; }


		/// <summary>
		/// Выбирает девайс для подключения
		/// </summary>
		private void OnChooseDeviceToConnect()
		{
			DialogTaskCompletionResult.TrySetResult(ContentDialogResult.Primary);
		}

	}
}