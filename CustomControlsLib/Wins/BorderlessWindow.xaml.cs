
using System;
using ABI.Microsoft.UI.Xaml.Controls;
using CustomControlsLib.Enums;
using CustomControlsLib.Models;

namespace CustomControlsLib.Wins
{
	using Windows.Foundation;
	using Microsoft.UI.Xaml;
	using Serilog;
	using SystemHelpers.Enums;
	using SystemHelpers.Static;
	using System.IO;
	using SystemHelpers.Enums;
	using SystemHelpers.Services;
	using SystemHelpers.Static;
	using WinUIEx;

	using static SystemHelpers.Static.PinvokeWindowMethods;

	/// <summary>
	/// Окно без фона и рамок.
	/// </summary>
	public partial class BorderlessWindow : WindowEx
	{
		public int Left { get; private set; } = 0;
		public int Top { get; private set; } = 0;

		private int currentStyle = 0;

		public BorderlessWindow()
		{
			this.InitializeComponent();
			Initialize();
			AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
			PositionChanged += BorderlessWindow_PositionChanged;
			//CaptureMouseClick(this, false);
		}

		public void SetContent(FrameworkElement element)
		{
			ContentFrame.Content =  element;
		}

		/// <summary>
		/// Делает окно прозрачным для клика.
		/// </summary>
		public void SetWindowTransparentToClick()
		{
			currentStyle = GetWindowLong(this.WindowHandle, WindowHelperEnums.GWL_EXSTYLE);
			WindowPositioningService.SetWindowLong(this.WindowHandle, WindowHelperEnums.GWL_EXSTYLE, currentStyle | (int)PinvokeWindowMethods.WindowStyles.WS_EX_TRANSPARENT | (int)WindowHelperEnums.WS_EX_LAYERED | (int)PinvokeWindowMethods.WindowStyles.WS_EX_TOPMOST);
		}

		/// <summary>
		/// Делает окно прозрачным для клика.
		/// </summary>
		public void UnsetWindowTransparentToClick()
		{
			int windowLong = (int)GetWindowLong(this.WindowHandle, (int)WindowHelperEnums.GWL_EXSTYLE);
			SetWindowLong(this.WindowHandle, (int)WindowHelperEnums.GWL_EXSTYLE, (IntPtr)(windowLong & -33));
		}

		/// <summary>
		/// Меняет тип фона окна.
		/// </summary>
		public void ChangeBackdropType(BackdropType backdropType)
		{
			if (backdropType == BackdropType.Transparent)
			{
				this.SystemBackdrop = new TransparentTintBackdrop();
			}
			else if (backdropType == BackdropType.Blur)
			{
				this.SystemBackdrop = new BlurredBackdrop();
			}
		}

		public void SetPosition(int x, int y)
		{
			var scale = (PinvokeWindowMethods.GetDpiForWindow(this.GetWindowHandle()) / 96.0);
			SetWindowPos(this.GetWindowHandle(), IntPtr.Zero, x, y, (int)(Width * scale), (int)(Height * scale), 0);
		}

		public void SetPosition(Point position)
		{
			var scale = (PinvokeWindowMethods.GetDpiForWindow(this.GetWindowHandle()) / 96.0);
			SetWindowPos(this.GetWindowHandle(), IntPtr.Zero, (int)position.X, (int)position.Y, (int)(Width * scale), (int)(Height * scale), 0);
		}

		/// <summary>
		/// Обрабатывает перемещение окна.
		/// </summary>
		private void BorderlessWindow_PositionChanged(object? sender, global::Windows.Graphics.PointInt32 e)
		{
			Left = e.X;
			Top = e.Y;
		}

		// This changes clickability
		public void CaptureMouseClick(Window window, bool condition)
		{
			try
			{
				var hwnd = window.GetWindowHandle();

				if (condition)
				{
					int windowLong = (int)GetWindowLong(hwnd, (int)WindowHelperEnums.GWL_EXSTYLE);
					SetWindowLong(hwnd, (int)WindowHelperEnums.GWL_EXSTYLE, (IntPtr)(windowLong & -33));
				}
				else
				{
					int windowLong = (int)GetWindowLong(hwnd, (int)WindowHelperEnums.GWL_EXSTYLE);
					SetWindowLong(hwnd, (int)WindowHelperEnums.GWL_EXSTYLE, (IntPtr)(windowLong | (int)PinvokeWindowMethods.WindowStyles.WS_EX_TRANSPARENT));
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
			}
		}
	}
}
