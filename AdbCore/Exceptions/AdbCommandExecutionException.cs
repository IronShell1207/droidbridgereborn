namespace AdbCore.Exceptions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class AdbCommandExecutionException : Exception
	{
		public string Message { get; set; }
		/// <summary>
		/// Инициализирует экземпляр <see cref="AdbCommandExecutionException"/>.
		/// </summary>
		public AdbCommandExecutionException(string message)
		{
			Message = message;
		}
	}
}
