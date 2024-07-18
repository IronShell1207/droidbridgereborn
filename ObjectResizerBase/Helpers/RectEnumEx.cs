namespace ObjectResizerBase.Helpers
{
	using ObjectResizerBase.Enum;

	/// <summary>
	/// Вспомогающие методы для точек прямоугольника.
	/// </summary>
	public static class RectEnumEx
	{
		/// <summary>
		/// Получает вершину по названию.
		/// </summary>
		/// <param name="name">Название контрола с точкой.</param>
		/// <returns>Вершина прямоугольника</returns>
		public static RectVertexEnum GetRectEnumByName(string name)
		{
			return name switch
			{
				"TopLeftResizeCorner" => RectVertexEnum.TopLeft,
				"BottomRightResizeCorner" => RectVertexEnum.BottomRight,
				"TopRightResizeCorner" => RectVertexEnum.TopRight,
				"BottomLeftResizeCorner" => RectVertexEnum.BottomLeft,
				_ => RectVertexEnum.TopLeft
			};
		}

		/// <summary>
		/// Получает сторону прямоугольника по названию.
		/// </summary>
		public static RectSide GetRectSideByName(string name)
		{
			return name switch
			{
				"BottomResizeLine" => RectSide.Bottom,
				"RightResizeLine" => RectSide.Right,
				"TopResizeLine" => RectSide.Top,
				"LeftResizeLine" => RectSide.Left,
				_ => RectSide.Top
			};
		}

		/// <summary>
		/// Прибавляет индекс к получаемой вершине.
		/// </summary>
		/// <param name="current">Текущая вершина.</param>
		/// <param name="addingNumber">Добавляющий номер (по часовой стрелке индекс точки)</param>
		public static RectVertexEnum AddResizeNumber(RectVertexEnum current, int addingNumber)
		{
			int currentIndex = (int)current;
			int newIndex = addingNumber + currentIndex;
			if (newIndex > 3)
			{
				newIndex = newIndex - 4;
			}

			return (RectVertexEnum)newIndex;
		}

		/// <summary>
		/// Прибавляет индекс к получаемой вершине.
		/// </summary>
		/// <param name="current">Текущая вершина.</param>
		/// <param name="addingNumber">Добавляющий номер (по часовой стрелке индекс точки)</param>
		public static RectSide AddResizeNumber(RectSide current, int addingNumber)
		{
			int currentIndex = (int)current;
			int newIndex = addingNumber + currentIndex;
			if (newIndex > 3)
			{
				newIndex = newIndex - 4;
			}

			return (RectSide)newIndex;
		}

		/// <summary>
		/// Корректирует значение поворота для выбранной вершины прямоугольника.
		/// </summary>
		public static double FixAngleRotationByRectPoint(RectVertexEnum rectVertex, double currentAngle)
		{
			double returningAngle = currentAngle;

			if (rectVertex == RectVertexEnum.TopLeft)
				returningAngle = currentAngle;
			else if (rectVertex == RectVertexEnum.TopRight)
				returningAngle = currentAngle + 90;
			else if (rectVertex == RectVertexEnum.BottomRight)
				returningAngle = currentAngle + 180;
			else if (rectVertex == RectVertexEnum.BottomLeft)
				returningAngle = currentAngle + 270;

			returningAngle = returningAngle > 305 ? returningAngle - 305 : returningAngle;

			return returningAngle;
		}
	}
}
