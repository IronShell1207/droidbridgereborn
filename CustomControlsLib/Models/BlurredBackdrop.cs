namespace CustomControlsLib.Models
{
	using WinUIEx;

	public class BlurredBackdrop : CompositionBrushBackdrop
	{
		protected override Windows.UI.Composition.CompositionBrush CreateBrush(
			Windows.UI.Composition.Compositor compositor) => compositor.CreateHostBackdropBrush();
	}
}
