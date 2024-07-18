using WinUIEx;

namespace Utils.Models
{
	public class BlurredBackdrop : CompositionBrushBackdrop
	{
		protected override Windows.UI.Composition.CompositionBrush CreateBrush(
			Windows.UI.Composition.Compositor compositor) => compositor.CreateHostBackdropBrush();
	}
}
