using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomControlsLib.Abstract;
using DroidBridgeReborn.Views.Controls.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DroidBridgeReborn.ViewModels.Dialogs
{
	public class BaseDialogViewModel : ObservableObject, IDialog<ContentDialogResult>
	{
		/// <inheritdoc cref="DialogContent"/>
		private FrameworkElement _dialogContent;

		/// <inheritdoc cref="DialogTitle"/>
		private string _dialogTitle;

		/// <summary>
		/// Инициализирует экземпляр <see cref="BaseDialogViewModel"/>.
		/// </summary>
		public BaseDialogViewModel()
		{
			CancelCommand = new RelayCommand(OnCancel);
			DialogTaskCompletionResult = new TaskCompletionSource<ContentDialogResult>();
			var view = new BaseDialogView();
			view.DataContext = this;
			Content = view;
		}

		/// <summary>
		/// Команда выполняющая метод <see cref="OnCancel"/>.
		/// </summary>
		public RelayCommand CancelCommand { get; }

		public FrameworkElement Content { get; set; }

		/// <summary>
		/// контента диалога.
		/// </summary>
		public FrameworkElement DialogContent {
			get => _dialogContent;
			set => SetProperty(ref _dialogContent, value);
		}

		public TaskCompletionSource<ContentDialogResult> DialogTaskCompletionResult { get; set; }

		/// <summary>
		/// Заголовок диалога.
		/// </summary>
		public string DialogTitle {
			get => _dialogTitle;
			set => SetProperty(ref _dialogTitle, value);
		}

		/// <summary>
		/// Отмена
		/// </summary>
		public virtual void OnCancel()
		{
			DialogTaskCompletionResult.TrySetResult(ContentDialogResult.None);
		}
	}
}