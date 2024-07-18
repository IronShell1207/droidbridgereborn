using Windows.Foundation;

namespace ObjectResizerBase.Helpers
{
	using ObjectResizerBase.Abstraction;
	using ObjectResizerBase.Enum;
	using ObjectResizerBase.Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class ResizableControlEx
	{
		/// <summary>
		/// Расчитывает положение углов.
		/// </summary>
		public static ResizeFrameModel CalculateCornerPositions(this ICanvasObject canvasObjectModel, double? rotationAngle = null)
		{
			var resizeFrame = new ResizeFrameModel();
			double rotation = rotationAngle != null ? rotationAngle.Value : canvasObjectModel.RotationAngle;
			double angleRadians = Math.PI * rotation / 180.0;

			double centerX = canvasObjectModel.HorizontalPosition + (canvasObjectModel.Width / 2);
			double centerY = canvasObjectModel.VerticalPosition + (canvasObjectModel.Height / 2);

			double newX = centerX + (Math.Cos(angleRadians) * (-(canvasObjectModel.Width / 2)) - Math.Sin(angleRadians) * (-(canvasObjectModel.Height / 2)));
			double newY = centerY + (Math.Sin(angleRadians) * (-(canvasObjectModel.Width / 2)) + Math.Cos(angleRadians) * (-(canvasObjectModel.Height / 2)));
			resizeFrame.LeftTopCornerPosition = new Point(newX, newY);

			double newRightX = newX + (Math.Cos(angleRadians) * canvasObjectModel.Width);
			double newRightY = newY + (Math.Sin(angleRadians) * canvasObjectModel.Width);

			resizeFrame.RightTopCornerPosition = new Point(newRightX, newRightY);

			double newBottomRightX = centerX + (Math.Cos(angleRadians) * (canvasObjectModel.Width / 2)) - Math.Sin(angleRadians) * (canvasObjectModel.Height / 2);
			double newBottomRightY = centerY + (Math.Sin(angleRadians) * (canvasObjectModel.Width / 2)) + Math.Cos(angleRadians) * (canvasObjectModel.Height / 2);
			resizeFrame.RightBottomCornerPosition = new Point(newBottomRightX, newBottomRightY);

			double newBottomLeftX = centerX - (Math.Cos(angleRadians) * (canvasObjectModel.Width / 2)) - Math.Sin(angleRadians) * (canvasObjectModel.Height / 2);
			double newBottomLeftY = centerY - (Math.Sin(angleRadians) * (canvasObjectModel.Width / 2)) + Math.Cos(angleRadians) * (canvasObjectModel.Height / 2);
			resizeFrame.LeftBottomCornerPosition = new Point(newBottomLeftX, newBottomLeftY);

			return resizeFrame;
		}

		/// <summary>
		/// Получает прямоугольник занимаего места элементом.
		/// </summary>
		public static Rect GetAsRect(this ResizeFrameModel frameModel)
		{
			return new Rect(frameModel.LeftTopCornerPosition, frameModel.RightBottomCornerPosition);
		}

		/// <summary>
		/// Получает левую линию прямоугольника с изображением.
		/// </summary>
		public static LineModel GetLeftLine(this ICanvasObject canvasObjectModel)
		{
			var firstMinimumPoint = GetMinimumHorizontalPoint(canvasObjectModel);
			var secondMinimumPoint = GetMinimumHorizontalPoint(canvasObjectModel, firstMinimumPoint);
			return new LineModel(firstMinimumPoint, secondMinimumPoint);
		}

		/// <summary>
		/// Получает минимальную позицию по вертикали.
		/// </summary>
		public static double GetMinimumVerticalPosition(this ICanvasObject canvasObjectModel)
		{
			var rectOfModel = CalculateCornerPositions(canvasObjectModel);
			var points = GetRectanglePoints(rectOfModel);
			double minPoint = points.Min(x => x.Y);
			return minPoint;
		}

		/// <summary>
		/// Корректирует тип точки, с которой идет взаимодействие.
		/// </summary>
		/// <param name="vertexName">Тип текущей точки.</param>
		/// <returns> <para> Тип точки по расположению относительно поворота. </para>
		/// <para>Нужно ли инвертировать дельту.</para>
		/// </returns>
		public static (RectVertexEnum, bool) CorrectPointEnum(this ICanvasObject canvasObjectModel, RectVertexEnum vertexName)
		{
			double rotationAngle = canvasObjectModel.RotationAngle;
			bool isInverted = false;

			// Если угол находится в левой верхней части.
			if ((rotationAngle >= -60 && rotationAngle <= 65 || rotationAngle >= 305))
			{
				return (vertexName, false);
			}

			// Если угол находится в правой верхней части.
			else if (rotationAngle >= 65 && rotationAngle < 120)
			{
				isInverted = vertexName != RectVertexEnum.TopLeft;
				return (RectEnumEx.AddResizeNumber(vertexName, 1), isInverted);
			}

			// Если угол находится в правой нижней части.
			else if (rotationAngle >= 120 && rotationAngle < 210)
			{
				return (RectEnumEx.AddResizeNumber(vertexName, 2), true);
			}

			// Если угол находится в левой нижней части.
			else if (rotationAngle >= 210 && rotationAngle < 305)
			{
				isInverted = vertexName == RectVertexEnum.BottomRight;
				return (RectEnumEx.AddResizeNumber(vertexName, 3), isInverted);
			}
			return (vertexName, false);
		}

		/// <summary>
		/// Корректирует тип точки, с которой идет взаимодействие.
		/// </summary>
		/// <param name="sideName">Тип текущей точки.</param>
		/// <returns> <para> Тип точки по расположению относительно поворота. </para>
		/// <para>Нужно ли инвертировать дельту.</para>
		/// </returns>
		public static (RectSide, bool) GetRectSideBasedOnRotation(this ICanvasObject canvasObjectModel, RectSide sideName)
		{
			double rotationAngle = canvasObjectModel.RotationAngle;
			bool isInverted = false;

			// Если угол находится в левой верхней части.
			if ((rotationAngle >= -60 && rotationAngle <= 65 || rotationAngle >= 305))
			{
				return (sideName, false);
			}

			// Если угол находится в правой верхней части.
			else if (rotationAngle >= 65 && rotationAngle < 120)
			{
				return (RectEnumEx.AddResizeNumber(sideName, 1), isInverted);
			}

			// Если угол находится в правой нижней части.
			else if (rotationAngle >= 120 && rotationAngle < 210)
			{
				isInverted = sideName == RectSide.Bottom;
				return (RectEnumEx.AddResizeNumber(sideName, 2), true);
			}

			// Если угол находится в левой нижней части.
			else if (rotationAngle >= 210 && rotationAngle < 305)
			{
				isInverted = sideName == RectSide.Right;
				return (RectEnumEx.AddResizeNumber(sideName, 3), isInverted);
			}
			return (sideName, false);
		}

		/// <summary>
		/// Получает минимальную позицию по вертикали.
		/// </summary>
		public static double GetMaximumVerticalPosition(this ICanvasObject canvasObjectModel)
		{
			var rectOfModel = CalculateCornerPositions(canvasObjectModel);
			var points = GetRectanglePoints(rectOfModel);
			return points.Max(x => x.Y);
		}

		/// <summary>
		/// Получает минимальную позицию по горизонтали.
		/// </summary>
		public static double GetMinimumHorizontalPosition(this ICanvasObject canvasObjectModel)
		{
			var rectOfModel = CalculateCornerPositions(canvasObjectModel);
			var points = GetRectanglePoints(rectOfModel);
			return points.Min(x => x.X);
		}

		/// <summary>
		/// Получает максимальную позицию по горизонтали.
		/// </summary>
		public static double GetMaximumHorizontalPosition(this ICanvasObject canvasObjectModel)
		{
			var rectOfModel = CalculateCornerPositions(canvasObjectModel);
			var points = GetRectanglePoints(rectOfModel);
			return points.Max(x => x.X);
		}

		/// <summary>
		/// Преобразует в прямоугольник из четырех точек.
		/// </summary>
		public static RectModel GetAsRectModel(this ICanvasObject canvasObjectModel)
		{
			var leftLine = GetLeftLine(canvasObjectModel);
			var rightLine = GetRightLine(canvasObjectModel);
			var rect = new RectModel(leftLine, rightLine);
			return rect;
		}

		/// <summary>
		/// Получает правую линию прямоугольника с изображением.
		/// </summary>
		public static LineModel GetRightLine(this ICanvasObject canvasObjectModel)
		{
			var firstMinimumPoint = GeMaximumHorizontalPoint(canvasObjectModel);
			var secondMinimumPoint = GeMaximumHorizontalPoint(canvasObjectModel, firstMinimumPoint);
			return new LineModel(firstMinimumPoint, secondMinimumPoint);
		}

		/// <summary>
		/// Получает максимальную координату по горизонтали.
		/// </summary>
		public static Point GeMaximumHorizontalPoint(this ICanvasObject canvasObjectModel, Point excludedPoint = default)
		{
			var rectOfModel = CalculateCornerPositions(canvasObjectModel);
			var points = GetRectanglePoints(rectOfModel);

			if (excludedPoint != default)
			{
				points.Remove(excludedPoint);
			}
			var maxHorizontalPosition = points.FirstOrDefault(x => x.X == points.Max(x => x.X));
			return maxHorizontalPosition;
		}

		/// <summary>
		/// Получает минимальную координату по горизонтали.
		/// </summary>
		public static Point GetMinimumHorizontalPoint(this ICanvasObject canvasObjectModel, Point excludedPoint = default)
		{
			var rectOfModel = CalculateCornerPositions(canvasObjectModel);
			var points = GetRectanglePoints(rectOfModel);

			if (excludedPoint != default)
			{
				points.Remove(excludedPoint);
			}
			var minimumHorizontalPosition = points.FirstOrDefault(x => x.X == points.Min(x => x.X));
			return minimumHorizontalPosition;
		}

		/// <summary>
		/// Получает точки прямоугольника (с коррекцией на поворот).
		/// </summary>
		private static List<Point> GetRectanglePoints(this ResizeFrameModel frameModel)
		{
			return new List<Point>(){frameModel.LeftTopCornerPosition, frameModel.LeftBottomCornerPosition, frameModel.RightBottomCornerPosition,
				frameModel.RightTopCornerPosition};
		}

		/// <summary>
		/// Изменяет размер до максимального.
		/// </summary>
		public static void ResizeToMaxInitSize(this ICanvasObject canvasObjectModel)
		{
			double bigSize = 448;
			double normalSize = 312;
			if (canvasObjectModel.Height > bigSize || canvasObjectModel.Width > bigSize)
			{
				bool isHorizontal = canvasObjectModel.Width > canvasObjectModel.Height;
				double sizeRatio = canvasObjectModel.Width / canvasObjectModel.Height;
				if (isHorizontal)
				{
					canvasObjectModel.Width = normalSize;
					canvasObjectModel.Height = normalSize / sizeRatio;
				}
				else
				{
					canvasObjectModel.Height = normalSize;
					canvasObjectModel.Width = canvasObjectModel.Height * sizeRatio;
				}
			}
		}
	}
}