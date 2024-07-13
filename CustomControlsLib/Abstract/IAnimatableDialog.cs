namespace CustomControlsLib.Abstract
{
	using System.Threading.Tasks;

	public interface IAnimatableDialog
	{
		void AnimateShow();

		Task AnimateCloseAsync();
	}
}
