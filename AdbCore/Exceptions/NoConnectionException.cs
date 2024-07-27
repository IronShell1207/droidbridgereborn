namespace AdbCore.Exceptions
{
	using System;

	public class NoConnectionException : Exception
	{
		public string Message { get; set; }

		/// <summary>
		/// Инициализирует экземпляр <see cref="NoConnectionException"/>.
		/// </summary>
		public NoConnectionException(string message)
		{
			Message = message;
		}
	}
}