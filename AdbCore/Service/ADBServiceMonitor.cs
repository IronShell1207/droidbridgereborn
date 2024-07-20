namespace AdbCore.Service
{
	using AdbCore.Abstraction;
	using System;
	using System.Diagnostics;
	using System.Threading;
	using Serilog;

	public class ADBServiceMonitor : IADBServiceMonitor
	{
		private readonly object _lock = new object();

		private string _currentAdbPath;

		private bool _isTimerRunning = false;
		private Timer _updateTimer;

		public void Initialize(TimeSpan interval)
		{
			UpdateInterval = interval;
			_updateTimer = new Timer(CheckAdbServiceStatus, null, TimeSpan.Zero, UpdateInterval);
			_isTimerRunning = true;
		}


		public event Action<string> OnADBPathChanged;

		public event Action<bool> OnADBStatusChanged;

		public string CurrentAdbPath
		{
			get => _currentAdbPath;
			private set 
			{
				if (_currentAdbPath != value)
				{
					_currentAdbPath = value;
					OnADBPathChanged?.Invoke(value);
				}
			}
		}

		public bool IsRunning { get; private set; }
		public TimeSpan UpdateInterval { get; private set; } = default;

		public void StartMonitoring()
		{
			if (_isTimerRunning)
				return;

			lock (_lock)
			{
				if (_updateTimer == null)
				{
					_updateTimer = new Timer(CheckAdbServiceStatus, null, TimeSpan.Zero, UpdateInterval);
				}

				_updateTimer.Change(TimeSpan.Zero, UpdateInterval);
				_isTimerRunning = true;
			}
		}

		public void StopMonitoring()
		{
			if (_isTimerRunning == false)
				return;
			lock (_lock)
			{
				_updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
				_isTimerRunning = false;
			}
		}

		private void CheckAdbServiceStatus(object state)
		{
			bool currentStatus = IsAdbRunning();
			if (currentStatus != IsRunning)
			{
				IsRunning = currentStatus;
				OnADBStatusChanged?.Invoke(IsRunning);
			}
		}

		public void ChangeInterval(TimeSpan newInterval)
		{
			lock (_lock)
			{
				UpdateInterval = newInterval;
				_updateTimer.Change(TimeSpan.Zero, UpdateInterval);
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