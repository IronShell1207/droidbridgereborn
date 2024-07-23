using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CustomControlsLib.Abstract;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using DroidBridgeReborn.Views.Controls.Dialogs;
using CommunityToolkit.Mvvm.Input;
using DroidBridgeReborn.ViewModels.Objects;
using Microsoft.Extensions.DependencyInjection;

namespace DroidBridgeReborn.ViewModels.Dialogs
{
	public class DevicesListDialogViewModel : ObservableObject, IDialog<ContentDialogResult>
	{
		public ObservableCollection<DeviceViewModel> Devices => DevicesListViewModel.Instance.Devices;
		public TaskCompletionSource<ContentDialogResult> DialogTaskCompletionResult { get; set; }
		public FrameworkElement Content { get; set; }

		/// <summary>
		/// Команда выполняющая метод <see cref="OnCloseDialog"/>.
		/// </summary>
		public RelayCommand CloseDialogCommand { get; }


		/// <summary>
		/// закрывает диалог
		/// </summary>
		private void OnCloseDialog()
		{
			DialogTaskCompletionResult.TrySetResult(ContentDialogResult.None);
		}
 
		/// <summary>
		/// Инициализирует экземпляр <see cref="DevicesListDialogViewModel"/>.
		/// </summary>
		public DevicesListDialogViewModel()
		{
			DialogTaskCompletionResult = new TaskCompletionSource<ContentDialogResult>();
			CloseDialogCommand = new RelayCommand(OnCloseDialog);

			var view = new ConnectedDevicesListDialog();
			view.DataContext = this;
			Content = view;
		}
	}
}