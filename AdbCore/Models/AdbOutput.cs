namespace AdbCore.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class AdbOutput
	{
		public string Lines { get; set; } = string.Empty;

		public string ErrorOutput { get; set; }

		public bool IsError { get; set; } = false;
		/// <summary>
		/// Инициализирует экземпляр <see cref="AdbOutput"/>.
		/// </summary>
		public AdbOutput(string lines, bool isError = false)
		{
			Lines = lines;
			IsError = isError;
		}
	}
}
