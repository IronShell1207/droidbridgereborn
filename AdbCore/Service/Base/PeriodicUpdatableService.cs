using System;
using System.Threading;

namespace AdbCore.Service.Base
{
	public class PeriodicUpdatedService
	{
		internal readonly object _lock = new object();
		internal bool _isTimerRunning = false;
		internal Timer _updateTimer;

		public TimeSpan UpdateInterval { get; private set; } = default;

		public void ChangeInterval(TimeSpan newInterval)
		{
			lock (_lock)
			{
				UpdateInterval = newInterval;
				_updateTimer.Change(TimeSpan.Zero, UpdateInterval);
			}
		}

		public void Initialize(TimeSpan interval)
		{
			UpdateInterval = interval;
			_updateTimer = new Timer(OnUpdateAction, null, TimeSpan.Zero, UpdateInterval);
			_isTimerRunning = true;
			OnUpdateAction(null);
		}

		public void StartMonitoring()
		{
			if (_isTimerRunning)
				return;

			lock (_lock)
			{
				if (_updateTimer == null)
				{
					_updateTimer = new Timer(OnUpdateAction, null, TimeSpan.Zero, UpdateInterval);
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

		protected virtual void OnUpdateAction(object state)
		{
		}
	}
}