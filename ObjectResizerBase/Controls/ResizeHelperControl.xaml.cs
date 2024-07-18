using System;
using System.Numerics;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using ObjectResizerBase.Abstraction;
using ObjectResizerBase.Enum;
using ObjectResizerBase.Helpers;
using Serilog;
using Windows.System;
using Windows.UI.Core;

namespace ObjectResizerBase.Controls
{
	public sealed partial class ResizeHelperControl : UserControl
	{
		/// <summary>
		/// Свойство зависимости <see cref="CanMultiselect"/>
		/// </summary>
		public static readonly DependencyProperty CanMultiselectProperty = DependencyProperty.Register(
			"CanMultiselect", typeof(bool), typeof(ResizeHelperControl), new PropertyMetadata(false));

		/// <summary>
		/// Свойство summs
		/// </summary>
		public bool CanMultiselect
		{
			get => (bool)GetValue(CanMultiselectProperty);
			set => SetValue(CanMultiselectProperty, value);
		}

		private bool _isRotating = false;
		private bool _isResizing = false;
		private bool _isMoving = false;
		private double _zoomFactor = 1.0f;
		private double _lastAspectRatio = 1.0f;
		private Point _lastSize = new Point();
 
		public event EventHandler ControlManipulationStarted;
		public event EventHandler ControlManipulationEnded;
		public event Action<ICanvasObject, Point, Point> ControlSizeChanged;
		private void UpdateZoomFactor()
		{
			if (GetWidgetBuilderScroller() is ScrollViewer scroller)
			{
				_zoomFactor = scroller.ZoomFactor;
			}
		}

		/// <summary>
		/// Обрабатывает событие взаимодействия с изображением.
		/// </summary>
		protected void OnResizeControl_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			if (_isResizing)
				return;

			if (DataContext is ICanvasObject canvasObjectModel && canvasObjectModel.IsSelected)
			{
				double widthDelta = (canvasObjectModel.Width - canvasObjectModel.Width * e.Delta.Scale);
				double heightDelta = (canvasObjectModel.Height - canvasObjectModel.Height * e.Delta.Scale);

				double newWidth = canvasObjectModel.Width - widthDelta;
				double newHeight = canvasObjectModel.Height - heightDelta;
				if (newHeight < MAX_SIZE && newWidth < MAX_SIZE && newHeight > MIN_SIZE &&
				    newWidth > MIN_SIZE)
				{
					canvasObjectModel.Width = newWidth;
					canvasObjectModel.Height = newHeight;
					canvasObjectModel.HorizontalPosition = canvasObjectModel.HorizontalPosition + widthDelta / 2;
					canvasObjectModel.VerticalPosition = canvasObjectModel.VerticalPosition + heightDelta / 2;
				}

				canvasObjectModel.RotationAngle += e.Delta.Rotation;
				canvasObjectModel.RotationAngle = Math.Round(canvasObjectModel.RotationAngle, 2);

				if (canvasObjectModel.RotationAngle > 360)
				{
					canvasObjectModel.RotationAngle = canvasObjectModel.RotationAngle - 360;
				}
				else if (canvasObjectModel.RotationAngle < -360)
				{
					canvasObjectModel.RotationAngle = canvasObjectModel.RotationAngle + 360;
				}

				if (!_isRotating)
				{
					MoveObject(e);
				}
			}
		}

		/// <summary>
		/// Занимается перемещением объекта.
		/// </summary>
		private void MoveObject(ManipulationDeltaRoutedEventArgs e)
		{
			
			double horizontalDelta = (e.Delta.Translation.X / _zoomFactor);
			double verticalDelta = (e.Delta.Translation.Y / _zoomFactor);
			if (_isResizing == false && DataContext is ICanvasObject canvasObjectModel)
			{
				if (IsInsideBounds(new Point(horizontalDelta, 0), false))
				{
					canvasObjectModel.HorizontalPosition =
						canvasObjectModel.HorizontalPosition + horizontalDelta;

					canvasObjectModel.VerticalPosition =
						canvasObjectModel.VerticalPosition + verticalDelta;
					canvasObjectModel.VerticalPosition = Math.Round(canvasObjectModel.VerticalPosition, 1);
					canvasObjectModel.HorizontalPosition = Math.Round(canvasObjectModel.HorizontalPosition, 1);
				}
			}
		}

		private void CanvasControl_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if (DataContext is ICanvasObject model)
			{
				if (model.IsInEditMode == false)
					return;

				// Получает состояние нажатия кнопки клавиатуры.
				var controlKeyState = CanMultiselect ? Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control) : 0;
				
				model.CallSelectionMethod((controlKeyState & CoreVirtualKeyStates.Down) != 0);
			}
		}

		internal void SubscribeToManipulations()
		{
			ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.Rotate | ManipulationModes.Scale;

			ManipulationDelta += OnResizeControl_ManipulationDelta;
			Tapped += CanvasControl_Tapped;
		}

		private Point _lastPosition = new Point();
		public ResizeHelperControl()
		{
			this.InitializeComponent();
			Loaded += ResizeHelperControl_Loaded;
			Unloaded += ResizeHelperControl_Unloaded;
		}

		private void ResizeHelperControl_Unloaded(object sender, RoutedEventArgs e)
		{
			ManipulationDelta -= OnResizeControl_ManipulationDelta;
			Tapped -= CanvasControl_Tapped;
		}

		private void ResizeHelperControl_Loaded(object sender, RoutedEventArgs e)
		{
			SubscribeToManipulations();
		}


		/// <summary>
		/// Обрабатывает событие перемещения курсора во время изменения размера.
		/// </summary>
		protected void BottomRightResizeCorner_OnPointerMoved(object sender, PointerRoutedEventArgs e)
		{
			if (_isRotating )
				return;

			if (_isResizing && DataContext is ICanvasObject CanvasObjectModel)
			{
				try
				{
					var rectangle = (Rectangle)sender;
					var prtPoint = e.GetCurrentPoint(rectangle);
					if (prtPoint?.Properties.IsLeftButtonPressed == true)
					{
						var position = prtPoint.Position;

						double yDelta = (position.Y - _lastPosition.Y) / _zoomFactor;

						double newYPosition = CanvasObjectModel.VerticalPosition;
						double xDelta = (position.X - _lastPosition.X) / _zoomFactor;
						double newXPosition = CanvasObjectModel.HorizontalPosition;

						RectVertexEnum resizeVertex = RectEnumEx.GetRectEnumByName(rectangle.Name);
						var newResizePoint = CanvasObjectModel.CorrectPointEnum(resizeVertex);
						bool isInverted = newResizePoint.Item2;
						resizeVertex = newResizePoint.Item1;
						switch (resizeVertex)
						{
							case RectVertexEnum.TopLeft:
							CalculateSizeForTopLeftCornerResizing(yDelta, xDelta, ref newYPosition,
								ref newXPosition, isInverted);
							break;

							case RectVertexEnum.BottomRight:
							CalculateBottomRightResizeCornerResizing(yDelta, xDelta, ref newYPosition,
								ref newXPosition, isInverted);
							break;

							case RectVertexEnum.TopRight:
							CalculateTopRightResizeCornerResizing(yDelta, xDelta, ref newYPosition,
								ref newXPosition, isInverted);
							break;

							case RectVertexEnum.BottomLeft:
							CalculateBottomLeftResizeCornerResizing(yDelta, xDelta, ref newYPosition,
								ref newXPosition, isInverted);
							break;
						}

						CanvasObjectModel.Width = Math.Round(CanvasObjectModel.Width, 1);
						CanvasObjectModel.Height = Math.Round(CanvasObjectModel.Height, 1);
						CanvasObjectModel.VerticalPosition = Math.Round(newYPosition, 1);
						CanvasObjectModel.HorizontalPosition = Math.Round(newXPosition, 1);
					}
				}
				catch (Exception ex)
				{
					Log.Logger.Warning(ex, $"Error at trying to resize image" + ex.Message);
				}

			}
		}

		private double MIN_SIZE = 15.0;
		private double MAX_SIZE = 1500;
		
		/// <summary> 
		/// Обрабатывает ресайз относительно левого нижнего угла.
		/// </summary>
		/// <param name="yDelta">Дельта по Y.</param>
		/// <param name="xDelta">Дельта по Х.</param>
		/// <param name="newYPosition">Новая координата по Y.</param>
		/// <param name="newXPosition">Новая координата по X.</param>
		protected void CalculateBottomLeftResizeCornerResizing(double yDelta, double xDelta, ref double newYPosition,
			ref double newXPosition, bool isInverted = false)
		{
			if (isInverted)
			{
				yDelta = -yDelta;
				xDelta = -xDelta;
			}

			if (DataContext is ICanvasObject canvasObjectModel)
			{
				bool isNewWidthMoreThatMinimum = ActualWidth - xDelta > MIN_SIZE;
				bool isNewHeightLessMax = ActualWidth - xDelta < MAX_SIZE + MIN_SIZE;
				if (isNewWidthMoreThatMinimum && isNewHeightLessMax)
				{
					newXPosition = canvasObjectModel.HorizontalPosition + xDelta > 0
						? canvasObjectModel.HorizontalPosition + xDelta
						: 0;
				}

				if (IsInsideBounds(new Point(xDelta, yDelta)))
				{
					canvasObjectModel.Width = ActualWidth - xDelta > MIN_SIZE
						? isNewHeightLessMax
							? ActualWidth - xDelta
							: canvasObjectModel.Width
						: canvasObjectModel.Width;

					if (canvasObjectModel.IsAspectRatioLocked)
					{
						canvasObjectModel.Height = canvasObjectModel.Width / _lastAspectRatio;
					}
					else
					{
						canvasObjectModel.Height = ActualHeight + yDelta > MIN_SIZE
							? isNewHeightLessMax
								? ActualHeight + yDelta
								: canvasObjectModel.Height
							: canvasObjectModel.Height;
						_lastAspectRatio = canvasObjectModel.Width / canvasObjectModel.Height;
					}
				}
			}
		}


		/// <summary>
		/// Обрабатывает ресайз относительно правого нижнего угла.
		/// </summary>
		/// <param name="yDelta">Дельта по Y.</param>
		/// <param name="xDelta">Дельта по Х.</param>
		/// <param name="newYPosition">Новая координата по Y.</param>
		/// <param name="newXPosition">Новая координата по X.</param>
		protected void CalculateBottomRightResizeCornerResizing(double yDelta, double xDelta, ref double newYPosition,
			ref double newXPosition, bool isInverted = false)
		{
			if (isInverted)
			{
				yDelta = -yDelta;
				xDelta = -xDelta;
			}

			if (DataContext is ICanvasObject canvasObjectModel)
			{
				bool isNewHeightLessMax = ActualHeight + yDelta < MAX_SIZE;
				if (IsInsideBounds(new Point(xDelta, yDelta)))
				{
					canvasObjectModel.Height = ActualHeight + yDelta > MIN_SIZE
						? isNewHeightLessMax
							? ActualHeight + yDelta
							: canvasObjectModel.Height
						: canvasObjectModel.Height;

					if (canvasObjectModel.IsAspectRatioLocked)
					{
						canvasObjectModel.Width = canvasObjectModel.Height * _lastAspectRatio;
					}
					else
					{
						canvasObjectModel.Width = ActualWidth + xDelta > MIN_SIZE
							? isNewHeightLessMax
								? ActualWidth + xDelta
								: canvasObjectModel.Width
							: canvasObjectModel.Width;
						_lastAspectRatio = canvasObjectModel.Width / canvasObjectModel.Height;
					}
				}
			}
		}


		/// <summary>
		/// Обрабатывает ресайз относительно левого верхнего угла.
		/// </summary>
		/// <param name="yDelta">Дельта по Y.</param>
		/// <param name="xDelta">Дельта по Х.</param>
		/// <param name="newYPosition">Новая координата по Y.</param>
		/// <param name="newXPosition">Новая координата по X.</param>
		protected void CalculateSizeForTopLeftCornerResizing(double yDelta, double xDelta, ref double newYPosition,
			ref double newXPosition, bool isInverted = false)
		{
			if (isInverted)
			{
				yDelta = -yDelta;
				xDelta = -xDelta;
			}

			if (DataContext is ICanvasObject canvasObjectModel)
			{

				bool isNewWidthMoreThatMinimum = ActualWidth - yDelta > MIN_SIZE;
				bool isNewHeightMoreThatMinimum = ActualHeight - yDelta > MIN_SIZE;
				bool isNewHeightLessMax = ActualHeight - yDelta < MAX_SIZE;

				if (isNewWidthMoreThatMinimum && isNewHeightMoreThatMinimum && isNewHeightLessMax)
				{
					if (IsInsideBounds(new Point(xDelta, yDelta)))
					{

						newXPosition = canvasObjectModel.HorizontalPosition + yDelta > 0
							? canvasObjectModel.HorizontalPosition + yDelta
							: 0;

						newYPosition = canvasObjectModel.VerticalPosition + yDelta > 0
							? canvasObjectModel.VerticalPosition + yDelta
							: 0;
					}
				}

				if (canvasObjectModel.VerticalPosition <= 0 ||
				    canvasObjectModel.HorizontalPosition <= 0)
				{
					return;
				}

				if (IsInsideBounds(new Point(xDelta, yDelta)))
				{
					double newHeight = isNewHeightMoreThatMinimum
						? isNewHeightLessMax
							? ActualHeight - yDelta
							: canvasObjectModel.Height
						: canvasObjectModel.Height;
					double newWidth = ActualWidth - xDelta > MIN_SIZE
						? isNewHeightLessMax
							? ActualWidth - xDelta
							: canvasObjectModel.Width
						: canvasObjectModel.Width;

					if (newHeight >= MIN_SIZE && newHeight <= MAX_SIZE && newWidth >= MIN_SIZE &&
					    newWidth <= MAX_SIZE)
					{
						canvasObjectModel.Height = newHeight;
					}
					else
					{
						newXPosition = canvasObjectModel.HorizontalPosition;
						newYPosition = canvasObjectModel.VerticalPosition;
						return;
					}

					canvasObjectModel.Width = canvasObjectModel.Height * _lastAspectRatio;
				}
			}
		}

		/// <summary>
		/// Находится ли новая точка в рамке.
		/// </summary>
		protected bool IsInsideBounds(Point delta, bool checkVerticalPosition = true)
		{
			var bounds = GetBoundsSize();
			double minOffset = -100;
			double maxOffset = 100;
			if (DataContext is ICanvasObject canvasObjectModel)
			{
				var minVertical = canvasObjectModel.GetMinimumVerticalPosition();
				var maxVertical = canvasObjectModel.GetMaximumVerticalPosition();
				var leftPos = canvasObjectModel.GetMinimumHorizontalPosition();
				var rightPos = canvasObjectModel.GetMaximumHorizontalPosition();

				if (checkVerticalPosition)
				{
					if (leftPos + delta.X >= minOffset && minVertical + delta.Y >= minOffset)
					{
						if (rightPos + delta.X<= bounds.Width + maxOffset && maxVertical + delta.Y <= bounds.Height + maxOffset)
						{
							return true;
						}
					}
				}
				else
				{
					if (leftPos + delta.X >= minOffset)
					{
						if (rightPos + delta.X <= bounds.Width + maxOffset)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Обрабатывает ресайз относительно правого верхнего угла.
		/// </summary>
		/// <param name="yDelta">Дельта по Y.</param>
		/// <param name="xDelta">Дельта по Х.</param>
		/// <param name="newYPosition">Новая координата по Y.</param>
		/// <param name="newXPosition">Новая координата по X.</param>
		protected void CalculateTopRightResizeCornerResizing(double yDelta, double xDelta, ref double newYPosition,
			ref double newXPosition, bool isInverted = false)
		{
			if (DataContext is ICanvasObject canvasObjectModel)
			{
				if (isInverted)
				{
					yDelta = -yDelta;
					xDelta = -xDelta;
				}

				bool isNewHeightMoreThatMinimum = ActualHeight - yDelta > MIN_SIZE;
				bool isNewHeightLessMax = ActualHeight - yDelta < MAX_SIZE;
				if (isNewHeightMoreThatMinimum && isNewHeightLessMax)
				{
					newYPosition = canvasObjectModel.VerticalPosition + yDelta > 0
						? canvasObjectModel.VerticalPosition + yDelta
						: 0;
				}

				double newHeight = ActualHeight - yDelta > MIN_SIZE
					? isNewHeightLessMax
						? ActualHeight - yDelta
						: canvasObjectModel.Height
					: canvasObjectModel.Height;
				if (IsInsideBounds(new Point(xDelta, yDelta)))
				{
					canvasObjectModel.Height = newHeight;
					if (canvasObjectModel.IsAspectRatioLocked)
					{
						canvasObjectModel.Width = newHeight * _lastAspectRatio;
					}
					else
					{
						canvasObjectModel.Width = ActualWidth + xDelta > MIN_SIZE
							? isNewHeightLessMax
								? ActualWidth + xDelta
								: canvasObjectModel.Width
							: canvasObjectModel.Width;

						_lastAspectRatio = canvasObjectModel.Width / canvasObjectModel.Height;
					}
				}
			}
		}


		/// <summary>
		/// Получает размер родительского элемента.
		/// </summary>
		protected Size GetBoundsSize()
		{
			try
			{
				object parent = this;
				while (parent != null)
				{
					parent = (parent as FrameworkElement).Parent;
				
					if (parent is ICanvasObjectParent canvasObjectParent)
						break;
				}

				if (parent is FrameworkElement gridParent)
				{
					return gridParent.ActualSize.ToSize();
				}

				return new Size(500, 500);
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
			}

			return new Size(1000, 1000);
		}

		private ScrollViewer GetWidgetBuilderScroller()
		{
			try
			{
				object parent = this;
				while (parent.GetType() != typeof(ScrollViewer) && parent != null)
				{
					parent = (parent as FrameworkElement).Parent;
				}

				ScrollViewer scrollViewer = (ScrollViewer)parent;
				return scrollViewer;
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
			}

			return null;
		}

		/// <summary>
		/// Обрабатывает перемещение мыши при горизонтальном воздействии.
		/// </summary>
		protected void HorizontalResizeLine_OnPointerMoved(object sender, PointerRoutedEventArgs e)
		{
			if (_isRotating)
				return;

			if (DataContext is ICanvasObject canvasObjectModel)
			{
				bool isRotated = Math.Round(canvasObjectModel.RotationAngle) > 5;
					
				if (_isResizing)
				{
					try
					{
						var line = (Rectangle)sender;
						var prtPoint = e.GetCurrentPoint(line);
						if (prtPoint?.Properties.IsLeftButtonPressed == true)
						{
							var position = prtPoint.Position;

							double xDelta = (position.X - _lastPosition.X) * _zoomFactor;
							double newXPosition = canvasObjectModel.HorizontalPosition;

							switch (line.Name)
							{
								case "LeftResizeLine":
									if (!isRotated)
									{
										newXPosition = canvasObjectModel.HorizontalPosition + xDelta;
									}

									canvasObjectModel.Width = ActualWidth - xDelta > MIN_SIZE
										? ActualWidth - xDelta < MAX_SIZE + MIN_SIZE
											? ActualWidth - xDelta
											: MAX_SIZE
										: MIN_SIZE;
									break;

								case "RightResizeLine":
									canvasObjectModel.Width = ActualWidth + xDelta > MIN_SIZE
										? ActualWidth + xDelta < MAX_SIZE + MIN_SIZE
											? ActualWidth + xDelta
											: MAX_SIZE
										: MIN_SIZE;
									break;
							}

							if (newXPosition != canvasObjectModel.HorizontalPosition)
							{
								canvasObjectModel.HorizontalPosition = newXPosition;
							}
						}
					}
					catch (Exception ex)
					{
						Log.Logger.Warning(ex, $"Error at trying to resize image" + ex.Message);
					}
				}
			}
		}

		/// <summary>
		/// Обрабатывает отпускание клавиши мыши.
		/// </summary>
		protected void ImageCanvasControl_OnPointerReleased(object sender, PointerRoutedEventArgs e)
		{
			_isRotating = false;
			_isResizing = false;
			ControlManipulationEnded?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Обрабатывает событие нажатия на линию изменения размера.
		/// </summary>
		protected void ResizeLine_OnPointerPressed(object sender, PointerRoutedEventArgs e)
		{
			_isRotating = false;
			if (_isMoving)
				return;

			if (DataContext is ICanvasObject canvasObjectModel)
			{
				
				_lastAspectRatio = canvasObjectModel.Width / canvasObjectModel.Height;
		 
				_lastSize = new Point(canvasObjectModel.Width, canvasObjectModel.Height);
				_isResizing = true;
				UpdateZoomFactor();
				var line = (Rectangle)sender;
				line.CapturePointer(e.Pointer);
				_lastPosition = e.GetCurrentPoint(line).Position;

				_isResizing = true;
				e.Handled = true;
				ControlManipulationStarted?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Обрабатывает событие отпускания клавиши с линии изменения размера.
		/// </summary>
		protected void ResizeLine_OnPointerReleased(object sender, PointerRoutedEventArgs e)
		{

			_isResizing = false;
			bool isResizeLine = (sender as FrameworkElement).Name.EndsWith("ResizeLine");

			var line = (Rectangle)sender;
			line.ReleasePointerCaptures();
			UpdateZoomFactor();
			_isResizing = false;
			e.Handled = true;
			if (DataContext is ICanvasObject CanvasObjectModel)
			{
				_lastAspectRatio = CanvasObjectModel.Width / CanvasObjectModel.Height;
				CanvasObjectModel.Height = Math.Round(CanvasObjectModel.Height, 1);
				CanvasObjectModel.Width =  Math.Round(CanvasObjectModel.Width, 1);

				ControlSizeChanged?.Invoke(CanvasObjectModel, _lastSize,
					new Point(CanvasObjectModel.Width, CanvasObjectModel.Height));
				ControlManipulationEnded?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Обрабатывает потерю фокуса курсором.
		/// </summary>
		protected void TopLeftResizeCorner_OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
		{
			_isResizing = false;
			var line = (Rectangle)sender;
			line.ReleasePointerCaptures();
			_isResizing = false;

			ControlManipulationEnded?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Обрабатывает событие перемещения курсора от горизональных линий (по оси Y).
		/// </summary>
		protected void VertialResizeLine_OnPointerMoved(object sender, PointerRoutedEventArgs e)
		{
			if (_isRotating)
				return;

			if (DataContext is ICanvasObject canvasObjectModel)
			{
				bool isRotated = Math.Round(canvasObjectModel.RotationAngle) > 5;

				if (_isResizing)
				{
					try

					{
						var line = (Rectangle)sender;
						var prtPoint = e.GetCurrentPoint(line);
						if (prtPoint?.Properties.IsLeftButtonPressed == true)
						{
							var position = prtPoint.Position;

							double yDelta = (position.Y - _lastPosition.Y) * _zoomFactor;
							double newYPosition = canvasObjectModel.VerticalPosition;
							switch (line.Name)
							{
								case "TopResizeLine":
									if (!isRotated)
									{
										newYPosition = canvasObjectModel.VerticalPosition + yDelta;
									}

									canvasObjectModel.Height = ActualHeight - yDelta > MIN_SIZE
										? ActualHeight - yDelta < MAX_SIZE + MIN_SIZE
											? ActualHeight - yDelta
											: MAX_SIZE
										: MIN_SIZE;

									break;

								case "BottomResizeLine":
									canvasObjectModel.Height = ActualHeight + yDelta > MIN_SIZE
										? ActualHeight + yDelta < MAX_SIZE + MIN_SIZE
											? ActualHeight + yDelta
											: MAX_SIZE
										: MIN_SIZE;
									break;
							}

							if (newYPosition != canvasObjectModel.VerticalPosition)
							{
								canvasObjectModel.VerticalPosition = newYPosition;
							}
						}
					}
					catch (Exception ex)
					{
						Log.Logger.Warning(ex, $"Error at trying to resize image" + ex.Message);
					}
				}
			}
		}

	}
}
