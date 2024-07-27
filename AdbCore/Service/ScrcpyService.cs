using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Channels;
using AdbCore.Abstraction;
using System.Text;

namespace AdbCore.Service
{
	public class ScrcpyService
	{
		public event EventHandler<string> OutputRecieved;

		public event EventHandler SessionEnded;

		private string _executablePath;
		public IDevice Device { get; private set; }

		public ScrcpyService(IDevice device, string executablePath)
		{
			_executablePath = executablePath;
			Device = device;
		}

		public async IAsyncEnumerable<string> StartCommandExecutionAsync(string commandText, Action<string> outputAction, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			var programStartInfo = new ProcessStartInfo(_executablePath, commandText)
			{
				CreateNoWindow = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8
			};

			var process = new Process() { StartInfo = programStartInfo };

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