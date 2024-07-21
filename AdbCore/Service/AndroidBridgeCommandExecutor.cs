using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading;
using System.Threading.Tasks;
using AdbCore.Abstraction;
using AdbCore.Exceptions;
using AdbCore.Models;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Serilog;

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

		public async Task<AdbOutput> GetCommandResultAsync(string commandText, TimeSpan executionTimeout = default, CancellationToken cancellationToken = default)
		{
			try
			{
				using Process process = GetAdbStartupProcess(commandText);
				process.Start();
				string output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
				string errorOutput = await process.StandardError.ReadToEndAsync(cancellationToken);
				await process.WaitForExitAsync(cancellationToken);

				Log.Logger.Debug(output);
				Log.Logger.Debug(errorOutput);
				//var lines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
				var adbOutput = new AdbOutput(output, errorOutput.Trim().Length >1);
				adbOutput.ErrorOutput = errorOutput;
				return adbOutput;
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
				Microsoft.AppCenter.Crashes.Crashes.TrackError(ex);
			}

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

		public async IAsyncEnumerable<string> StartCommandExecutionAsync(string commandText, Action<string> outputAction, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			using Process process = GetAdbStartupProcess(commandText);
			var outputlines = new List<string>();
			var outputChannel = Channel.CreateUnbounded<string>();

			async Task ReadStreamAsync(StreamReader reader)
			{
				try
				{
					while (!reader.EndOfStream)
					{
						var line = await reader.ReadLineAsync(cancellationToken);
						if (line != null)
						{
							await outputChannel.Writer.WriteAsync(line, cancellationToken);
						}
					}
				}
				catch (OperationCanceledException) { }
				finally
				{
					outputChannel.Writer.Complete();
				}
			}

			process.Start();

			var outputTask = ReadStreamAsync(process.StandardOutput);
			var errorTask = ReadStreamAsync(process.StandardError);

			await foreach (var line in outputChannel.Reader.ReadAllAsync(cancellationToken))
			{
				yield return line;
			}

			await Task.WhenAll(outputTask, errorTask);

			await process.WaitForExitAsync(cancellationToken);
		}
	}
}