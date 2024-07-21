namespace AdbCore.Service
{
	using AdbCore.Abstraction;
	using System;
	using System.Diagnostics;
	using System.Threading;
	using Serilog;
	using AdbCore.Service.Base;

	public class ADBServiceMonitor : PeriodicUpdatedService, IADBServiceMonitor
	{
		private string _currentAdbPath;

		public event Action<string> OnADBPathChanged;

		public event Action<bool> OnADBStatusChanged;

		public string CurrentAdbPath {
			get => _currentAdbPath;
			private set {
				if (_currentAdbPath != value)
				{
					_currentAdbPath = value;
					OnADBPathChanged?.Invoke(value);
				}
			}
		}

		public bool IsRunning { get; private set; }

		protected override void OnUpdateAction(object state)
		{
			bool currentStatus = IsAdbRunning();
			if (currentStatus != IsRunning)
			{
				IsRunning = currentStatus;
				OnADBStatusChanged?.Invoke(IsRunning);
			}
		}

		private bool IsAdbRunning()
		{
			Process[] processes = Process.GetProcessesByName("adb");
			foreach (Process process in processes)
			{
				try
				{
					if (string.IsNullOrWhiteSpace(process.MainModule.FileName) == false)
					{
						CurrentAdbPath = process.MainModule.FileName;
						return true;
					}
				}
				catch (Exception ex)
				{
					Log.Logger.Error(ex, ex.Message);
				}
			}
			return false;
		}
	}
}