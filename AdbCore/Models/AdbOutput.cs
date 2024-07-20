namespace AdbCore.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class AdbOutput
	{
		public List<string> Lines { get; set; } = new List<string>();

		public bool IsError { get; set; } = false;
		/// <summary>
		/// Инициализирует экземпляр <see cref="AdbOutput"/>.
		/// </summary>
		public AdbOutput(List<string> lines, bool isError = false)
		{
			Lines = lines;
			IsError = isError;
		}
	}
}
