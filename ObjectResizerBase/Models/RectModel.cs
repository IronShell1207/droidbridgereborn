using Windows.Foundation;

namespace ObjectResizerBase.Models
{
	using System.Text;
	using System;

	/// <summary>
	/// Модель прямоугольника.
	/// </summary>
	public class RectModel
	{
		/// <summary>
		/// Левый верхний угол.
		/// </summary>
		public Point TopLeft { get; set; }

		/// <summary>
		/// Правый верхний угол.
		/// </summary>
		public Point TopRight { get; set; }

		/// <summary>
		/// Левый нижний угол.
		/// </summary>
		public Point BottomLeft { get; set; }

		/// <summary>
		/// Правый нижний угол.
		/// </summary>
		public Point BottomRight { get; set; }

		/// <summary>
		/// Конструктор, принимающий точки всех сторон.
		/// </summary>
		public RectModel(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight)
		{
			TopLeft = topLeft;
			TopRight = topRight;
			BottomLeft = bottomLeft;
			BottomRight = bottomRight;
		}

		/// <summary>
		/// Конструктор, принимающий левую и правую линию прямоугольника.
		/// </summary>
		public RectModel(LineModel leftLine, LineModel rightLine)
		{
			TopLeft = leftLine.Start;
			TopRight = rightLine.Start;
			BottomLeft = leftLine.End;
			BottomRight = rightLine.End;
		}

		/// <summary>
		/// Пересекается ли прямоугольник с другим.
		/// </summary>
		public static bool IsIntersectsWithOther(RectModel rect1, RectModel rect2)
		{
			bool xAxisOverlap = (rect1.TopLeft.X <= rect2.TopRight.X && rect1.TopRight.X >= rect2.TopLeft.X) ||
								(rect2.TopLeft.X <= rect1.TopRight.X && rect2.TopRight.X >= rect1.TopLeft.X);

			bool yAxisOverlap = (rect1.TopLeft.Y <= rect2.BottomLeft.Y && rect1.BottomLeft.Y >= rect2.TopLeft.Y) ||
								(rect2.TopLeft.Y <= rect1.BottomLeft.Y && rect2.BottomLeft.Y >= rect1.TopLeft.Y);

			return xAxisOverlap && yAxisOverlap;
		}

		/// <summary>
		/// Переводит в строку.
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"Top left corner: {Math.Round(TopLeft.X, 2)}x{Math.Round(TopLeft.Y, 2)} \n");
			sb.Append($"Bottom left corner: {Math.Round(BottomLeft.X, 2)}x{Math.Round(BottomLeft.Y, 2)} \n");
			sb.Append($"Top right corner: {Math.Round(TopRight.X, 2)}x{Math.Round(TopRight.Y, 2)} \n");
			sb.Append($"Bottom right corner: {Math.Round(BottomRight.X, 2)}x{Math.Round(BottomRight.Y, 2)} ");
			return sb.ToString();
		}

		/// <summary>
		/// Конструктор, принимающий список точек.
		/// </summary>
		public RectModel(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			TopLeft = new Point(x1, y1);
			TopRight = new Point(x2, y2);
			BottomLeft = new Point(x4, y4);
			BottomRight = new Point(x3, y3);
		}
	}
}
