using System;
using System.Diagnostics;
using Microsoft.AppCenter.Crashes;
using Serilog;

namespace AdbCore.Helpers
{
	
	public static class ProcessHelpers
	{

		public static void TryKill(this Process proc)
		{
			try
			{
				proc.Kill(true);
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
				Crashes.TrackError(ex);
			}
		}
	}
}
