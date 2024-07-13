namespace ObjectResizerBase.Abstraction
{
	public interface ICanvasObject
	{
		public double Width { get; set; }
		public double Height { get; set; }
		public double HorizontalPosition { get; set; }
		public double VerticalPosition { get; set; }
		public double RotationAngle { get; set; }

		public bool IsInEditMode { get; set; }
		public bool IsSelected { get; set; }
		public int ZPosition { get; set; }
		public bool IsPositionLocked { get; set; }
		public bool IsAspectRatioLocked { get; set; }
		void CallSelectionMethod(bool keepOldSelections);
	}
}
