namespace CustomControlsLib.Abstract
{ 
	using Microsoft.UI.Xaml;
	using System.Threading.Tasks;

	public interface IDialog<T>
	{
		/// <summary>
		/// Событие завершания работы с диалогом.
		/// </summary>
		public TaskCompletionSource<T> DialogTaskCompletionResult { get; set; }

		/// <summary>
		/// Контент диалога.
		/// </summary>
		public FrameworkElement Content { get; set; }
	}
}