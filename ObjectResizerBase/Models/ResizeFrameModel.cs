using Windows.Foundation;

namespace ObjectResizerBase.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class ResizeFrameModel
	{
		public Point LeftTopCornerPosition { get; set; }
		public Point RightTopCornerPosition { get; set; }
		public Point RightBottomCornerPosition { get; set; }
		public Point LeftBottomCornerPosition { get; set; }
	}
}
