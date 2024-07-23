namespace CustomControlsLib.Helpers
{
	using CustomControlsLib.Abstract;
	using CustomControlsLib.Wins;
	using Microsoft.UI.Xaml;
	using Microsoft.UI.Xaml.Controls;
	using Serilog;
	using System;
	using System.Threading.Tasks;
	using SystemHelpers.Static;
	using System.Linq;
	using Windows.Foundation;

	public static class CustomDialogsHelper
	{
		private static Grid _dialogContainerGrid;

		/// <summary>
		/// Presenter для вывода диалогов.
		/// </summary>
		private static ContentPresenter _dialogPlacementSource;

		public static void SetDialogContainer(ContentPresenter presenter)
		{
			_dialogPlacementSource = presenter;
			_dialogContainerGrid = new Grid();
			_dialogPlacementSource.Content = _dialogContainerGrid;
		}

		/// <summary>
		/// Показывает кастомный диалог.
		/// </summary>
		public static async Task<T> ShowCustomDialog<T>(this IDialog<T> dialogModel,
			Func<Task> afterLoadFunc = null)
		{
			try
			{
				_dialogContainerGrid.Children.Add(dialogModel.Content);
				_dialogPlacementSource.Visibility = Visibility.Visible;

				if (dialogModel.Content is IAnimatableDialog animatableDialogModel)
					animatableDialogModel.AnimateShow();

				if (afterLoadFunc != null)
					await afterLoadFunc.Invoke();

				var result = await dialogModel.DialogTaskCompletionResult.Task;

				if (dialogModel.Content is IAnimatableDialog animatableDialogModelOnClose)
					await animatableDialogModelOnClose.AnimateCloseAsync();

				_dialogContainerGrid.Children.Remove(dialogModel.Content);

				return result;
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
			}
			finally
			{
				if (_dialogContainerGrid.Children.Any() == false)
				{
					_dialogPlacementSource.Visibility = Visibility.Collapsed;
				}
			}

			return default;
		}

		public static async Task ShowAndroidLikeToastNotifyAsync<T>(this IDialog<T> viewModel, TimeSpan duration)
		{
			if (viewModel.Content is FrameworkElement view)
			{
				try
				{
					BorderlessWindow window = new BorderlessWindow();
					TaskCompletionSource<bool> viewLoadCompletionSource = new TaskCompletionSource<bool>();
					view.SizeChanged += (st, e) => viewLoadCompletionSource.TrySetResult(true);

					window.SetContent(viewModel.Content);
					window.IsShownInSwitchers = true;
					window.IsMovable = true;
					window.IsClickThrougth = false;

					window.Activate();
					await viewLoadCompletionSource.Task;
					double targetWindowHeight = view.ActualHeight;
					double targetWindowWidth = view.ActualWidth;
					window.Height = targetWindowHeight;
					window.Width = targetWindowWidth;

					var display = DisplayService.Instance.GetDisplayModelFromPoint(new Point(0, 0));
					Point windowStartPosition = new Point(0, 0);
					if (display != null)
					{
						double verticalPosition = display.Bounds.Y + (display.Bounds.Height / 2 -
																	  ((targetWindowHeight * display.Scaling /
																		2)));
						double horizontalPosition = display.Bounds.X +
													(display.Bounds.Width / 2 -
													 ((targetWindowWidth * display.Scaling / 2)));
						windowStartPosition = new Point(horizontalPosition, verticalPosition);
					}

					window.SetPosition(windowStartPosition);

					if (viewModel.Content is IAnimatableDialog animatableDialogModel)
						animatableDialogModel.AnimateShow();

					var timeoutTask = Task.Delay(duration);
					var resultTask = await Task.WhenAny(viewModel.DialogTaskCompletionResult.Task, timeoutTask);

					if (viewModel.Content is IAnimatableDialog animatableDialogModelOnClose)
						await animatableDialogModelOnClose.AnimateCloseAsync();

					window.Close();
					window = null;
					return;
				}
				catch (Exception ex)
				{
					Log.Logger.Error(ex, ex.Message);
				}
			}
		}

		public static async Task<T> ShowCustomWindowDialog<T>(this IDialog<T> dialogModel,
			Func<Task> afterLoadFunc = null, Point callingPosition = default)
		{
			if (dialogModel.Content is FrameworkElement view)
			{
				try
				{
					if (dialogModel.Content is IDialogView dialogView)
						dialogView.ChangeToWindowStyle();
					
					BorderlessWindow window = new BorderlessWindow();
					TaskCompletionSource<bool> viewLoadCompletionSource = new TaskCompletionSource<bool>();
					view.SizeChanged += (st, e) => viewLoadCompletionSource.TrySetResult(true);
					window.SetContent(dialogModel.Content);
					window.IsShownInSwitchers = true;
					window.IsMovable = true;
					window.IsClickThrougth = false;

				
					window.Activate();

					await viewLoadCompletionSource.Task;

					double targetWindowHeight = view.ActualHeight;
					double targetWindowWidth = view.ActualWidth;
					window.Height = targetWindowHeight;
					window.Width = targetWindowWidth;

					var display = DisplayService.Instance.GetDisplayModelFromPoint(callingPosition);
					Point windowStartPosition = new Point(0, 0);
					if (display != null)
					{
						double verticalPosition = display.Bounds.Y + (display.Bounds.Height / 2 -
																	  ((targetWindowHeight * display.Scaling /
																		2)));
						double horizontalPosition = display.Bounds.X +
													(display.Bounds.Width / 2 -
													 ((targetWindowWidth * display.Scaling / 2)));
						windowStartPosition = new Point(horizontalPosition, verticalPosition);
					}

					//customDialogModel.SwitchColorToWindowType();

					window.SetPosition(windowStartPosition);

					if (dialogModel.Content is IAnimatableDialog animatableDialogModel)
						animatableDialogModel.AnimateShow();

					var result = await dialogModel.DialogTaskCompletionResult.Task;

					if (dialogModel.Content is IAnimatableDialog animatableDialogModelOnClose)
						await animatableDialogModelOnClose.AnimateCloseAsync();

					window.Close();
					window = null;
					return result;
				}
				catch (Exception ex)
				{
					Log.Logger.Error(ex, ex.Message);
				}
			}

			return default;
		}
	}
}