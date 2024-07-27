namespace AdbCore.Abstraction
{
	using System;

	public interface IADBServiceMonitor
	{
		event Action<bool> OnADBStatusChanged;
		event Action<string> OnADBPathChanged;
		string CurrentAdbPath { get; set; }
		bool IsRunning { get; }
		void StartMonitoring();
		void StopMonitoring();
		void ChangeInterval(TimeSpan interval);
		void Initialize(TimeSpan interval);
		void UpdateStatus();
	}
}
