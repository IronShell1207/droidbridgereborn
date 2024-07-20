using CommunityToolkit.Mvvm.ComponentModel;
using CustomControlsLib.Abstract;
using CustomControlsLib.Helpers;
using DroidBridgeReborn.Views.Controls.Dialogs;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;

namespace DroidBridgeReborn.Services
{
	public class ToastNotificationService : ObservableObject, IDialog<bool>
	{
		/// <inheritdoc cref="NotifyText"/>
		private string _notifyText;

		/// <summary>
		/// Инициализирует экземпляр <see cref="ToastNotificationService"/>.
		/// </summary>
		public ToastNotificationService()
		{
			DialogTaskCompletionResult = new TaskCompletionSource<bool>();
			var view = new ToastView();
			view.DataContext = this;
			Content = view;
		}

		public FrameworkElement Content { get; set; }

		public TaskCompletionSource<bool> DialogTaskCompletionResult { get; set; }

		/// <summary>
		/// Текст.
		/// </summary>
		public string NotifyText {
			get => _notifyText;
			set => SetProperty(ref _notifyText, value);
		}

		public static async void ShowToastNotify(string message, TimeSpan duration = default)
		{
			if (duration == default)
				duration = TimeSpan.FromMilliseconds(1899);

			var instance = new ToastNotificationService();
			var delay = Task.Delay(duration);
			instance.NotifyText = message;

			await instance.ShowAndroidLikeToastNotifyAsync(duration);
			instance.CloseDialog();
		}

		private void CloseDialog()
		{
			DialogTaskCompletionResult.TrySetResult(false);
		}
	}
}