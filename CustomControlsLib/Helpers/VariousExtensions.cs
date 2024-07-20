
namespace CustomControlsLib.Helpers
{
	using Windows.UI;

	public static class VariousExtensions
	{

		/// <summary>
		/// Converts a <see cref="Color"/> to a hexadecimal string representation.
		/// </summary>
		/// <param name="color">The color to convert.</param>
		/// <returns>The hexadecimal string representation of the color.</returns>
		public static string ToHexWithoutOpacity(this Color color)
		{
			return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
		}
	}
}
