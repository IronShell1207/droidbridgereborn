using Windows.Foundation;

namespace ObjectResizerBase.Models
{
	using System;

	/// <summary>
	/// Модель линии.
	/// </summary>
	public class LineModel
	{
		/// <summary>
		/// Точки начала и конца.
		/// </summary>
		private Point _start = default;

		private Point _end = default;

		/// <summary>
		/// Точка начала.
		/// </summary>
		public Point Start {
			get { return _start; }
			set {
				if (_end == default || value.X < _end.X || (value.X == _end.X && value.Y < _end.Y))
					_start = value;
				else
				{
					_start = _end;
					_end = value;
				}
			}
		}

		/// <summary>
		/// Точка конца.
		/// </summary>
		public Point End {
			get { return _end; }
			set {
				if (_start == default || value.X > _start.X || (value.X == _start.X && value.Y > _start.Y))
					_end = value;
				else
				{
					_end = _start;
					_start = value;
				}
			}
		}

		/// <summary>
		/// Инициализирует экземпляр <see cref="LineModel"/>.
		/// </summary>
		public LineModel()
		{
		}

		/// <summary>
		/// Получает максимальную точку линии.
		/// </summary>
		public Point GetMaxPoint()
		{
			return new Point(Math.Max(Start.X, End.X), Math.Max(Start.Y, End.Y));
		}

		/// <summary>
		/// Получает минимальную точку линии.
		/// </summary>
		/// <returns></returns>
		public Point GetMinPoint()
		{
			return new Point(Math.Min(Start.X, End.X), Math.Min(Start.Y, End.Y));
		}

		/// <summary>
		/// Пересекается ли линия с прямоугольником.
		/// </summary>
		/// <param name="rect">Прямоугольник.</param>
		public bool IntersectsRectangle(RectModel rect)
		{
			return LineIntersectsLine(Start, End, rect.TopLeft, rect.TopRight) ||
						  LineIntersectsLine(Start, End, rect.TopRight, rect.BottomRight) ||
						  LineIntersectsLine(Start, End, rect.BottomRight, rect.BottomLeft) ||
						  LineIntersectsLine(Start, End, rect.BottomLeft, rect.TopLeft) ||
						  IsPointInsideRectangle(Start, rect) ||
						  IsPointInsideRectangle(End, rect);
		}

		/// <summary>
		/// Пересекает ли линия другую линию.
		/// </summary>
		private bool LineIntersectsLine(Point line1Start, Point line1End, Point line2Start, Point line2End)
		{
			double denominator = ((line2End.Y - line2Start.Y) * (line1End.X - line1Start.X)) - ((line2End.X - line2Start.X) * (line1End.Y - line1Start.Y));

			if (denominator == 0)
				return false;

			double ua = (((line2End.X - line2Start.X) * (line1Start.Y - line2Start.Y)) - ((line2End.Y - line2Start.Y) * (line1Start.X - line2Start.X))) / denominator;
			double ub = (((line1End.X - line1Start.X) * (line1Start.Y - line2Start.Y)) - ((line1End.Y - line1Start.Y) * (line1Start.X - line2Start.X))) / denominator;

			return ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1;
		}

		/// <summary>
		/// Находится ли точка линии в прямоугольнике.
		/// </summary>
		private bool IsPointInsideRectangle(Point point, RectModel rect)
		{
			return point.X >= rect.TopLeft.X && point.X <= rect.TopRight.X && point.Y >= rect.TopLeft.Y && point.Y <= rect.BottomLeft.Y;
		}

		/// <summary>
		/// Инициализирует экземпляр <see cref="LineModel"/>.
		/// </summary>
		public LineModel(Point start, Point end)
		{
			Start = start;
			End = end;
		}

		/// <summary>
		/// Получает высоту линии.
		/// </summary>
		public double GetHeight()
		{
			var maxPoint = GetMaxPoint();
			var minPoint = GetMinPoint();
			return maxPoint.Y - minPoint.Y;
		}

		/// <summary>
		/// Получает ширину линии.
		/// </summary>
		public double GetWidth()
		{
			var maxPoint = GetMaxPoint();
			var minPoint = GetMinPoint();
			return maxPoint.X - minPoint.X;
		}
	}
}
