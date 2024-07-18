
using System.Threading.Tasks;
using SystemHelpers.Static;
using Win.Utils.Helpers;
using WinUIEx;

namespace DroidBridgeReborn
{
	using System.Collections.Generic;
	using System.IO.Pipes;
	using System.Linq;
	using System.Threading;
	using System;
	using System.IO;
	using System.Text;
	using Microsoft.AppCenter.Crashes;
	using Serilog;


	public partial class App
	{
		private static Mutex Mutex = null;
		private static NamedPipeServerStream pipeServer;

		private const string PipeName = "DrBridgeRebornPipe";
		private void SetupNamedPipeServer()
		{
			pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message,
				PipeOptions.Asynchronous);
			pipeServer.BeginWaitForConnection(HandleClientConnection, null);
			Log.Logger.Debug("Pipe server launched");
		}

		private static void SendCommandToMainInstance(string command)
		{
			using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.Out))
			{
				try
				{
					pipeClient.Connect(500); // 500 ms timeout

					using (StreamWriter writer = new StreamWriter(pipeClient, Encoding.UTF8))
					{
						writer.Write(command);
					}
				}
				catch (Exception ex)
				{
					Crashes.TrackError(ex);
					Log.Logger.Error(ex, ex.Message);
				}
			}
		}

		private async void HandleClientConnection(IAsyncResult result)
		{
			try
			{
				Log.Logger.Debug("Pipe server connection established");
				pipeServer.EndWaitForConnection(result);

				using (StreamReader reader = new StreamReader(pipeServer))
				{
					string message = await reader.ReadToEndAsync();
					Log.Logger.Debug(message);
					
					if (message == "RestoreMainWindow")
					{
						TryToRestoreMainWindow();
					}
					else if (message == "Toast")
					{
						
					}
				}
			}
			catch (Exception ex)
			{
				Crashes.TrackError(ex);
			}
			finally
			{
				// Continue to wait for the next connection
				SetupNamedPipeServer();
			}
		}

		public static void TryToRestoreMainWindow()
		{
			DispatcherQueueHelper.EnqueueAsync(async () =>
			{
				if (MainWindow != null)
				{
					MainWindow.WindowState = WindowState.Normal;
					await Task.Delay(1000);
					if (MainWindow != null)
					{
						PinvokeWindowMethods.SetForegroundWindow(MainWindow.GetWindowHandle());
					}

				}
				else
				{
					CreateMainWindow();
					await Task.Delay(1000);
					if (MainWindow != null)
					{
						PinvokeWindowMethods.SetForegroundWindow(MainWindow.GetWindowHandle());
					}
				}
			});
		}

		private bool CheckSecondStart(string[] args)
		{
			Mutex = new Mutex(true, "DrBridgeReborn", out var createdNew);

			if (!createdNew)
			{
				string msg = "RestoreMainWindow";

				if (args.Any(x => x.StartsWith("----AppNotificationActivated")))
				{
					msg = "Toast";
				}

				SendCommandToMainInstance(msg);
				Environment.Exit(0);
				return false;
			}
			else
			{
				SetupNamedPipeServer();
			}

			return true;
		}
	}
}