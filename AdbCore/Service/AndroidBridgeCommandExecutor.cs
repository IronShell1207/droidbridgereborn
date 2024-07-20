using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using AdbCore.Abstraction;
using AdbCore.Exceptions;
using AdbCore.Models;

namespace AdbCore.Service
{
	public class AndroidBridgeCommandExecutor
	{
		private IADBServiceMonitor _adbServiceMonitor;

		/// <summary>
		/// Инициализирует экземпляр <see cref="AndroidBridgeCommandExecutor"/>.
		/// </summary>
		public AndroidBridgeCommandExecutor(IADBServiceMonitor adbUpdateService)
		{
			_adbServiceMonitor = adbUpdateService;
		}


		public async Task<AdbOutput> GetCommandResultAsync(string commandText, TimeSpan executionTimeout)
		{
		 	using Process process = GetAdbStartupProcess(commandText);

			return null;
		}

		private Process GetAdbStartupProcess(string commandText)
		{
			string executablePath = _adbServiceMonitor.CurrentAdbPath;
			if (string.IsNullOrWhiteSpace(executablePath) || _adbServiceMonitor.IsRunning == false)
				throw new AdbServerNotRunningException();

			var programStartInfo = new ProcessStartInfo(executablePath, commandText)
			{
				CreateNoWindow = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8
			};

			var process = new Process() { StartInfo = programStartInfo };
			return process;
		}

		public async Task StartCommandExecutionAsync(string commandText, Action<string> outputAction)
		{
			using Process process = GetAdbStartupProcess(commandText);
		}
	}
}