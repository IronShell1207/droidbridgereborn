using AdbCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AdbCore.Service
{
	public class NetworkPingHelper 
	{
		public static async Task<List<string>> GetAndroidDevicesAsync(CancellationToken cancellationToken)
		{
			var activeIps = await ScanLocalNetworkAsync(cancellationToken);
			var networkDevices = new List<string>();

			foreach (var ip in activeIps)
			{
				networkDevices.Add(ip);
			}

			return networkDevices;
		}

		private static async Task<List<string>> ScanLocalNetworkAsync(CancellationToken cancellationToken)
		{
			var activeIps = new List<string>();
			var tasks = new List<Task>();

			string baseIp = GetLocalIPAddress().Substring(0, GetLocalIPAddress().LastIndexOf('.') + 1);

			for (int i = 1; i < 255; i++)
			{
				if (cancellationToken.IsCancellationRequested)
					break;

				string ip = baseIp + i;
				tasks.Add(Task.Run(async () =>
				{
					if (await PingAsync(ip))
					{
						lock (activeIps)
						{
							activeIps.Add(ip);
						}
					}
				}, cancellationToken));
			}

			await Task.WhenAll(tasks);
			return activeIps;
		}

		private static async Task<bool> PingAsync(string ip)
		{
			using (Ping ping = new Ping())
			{
				try
				{
					PingReply reply = await ping.SendPingAsync(ip, 100);
					return reply.Status == IPStatus.Success;
				}
				catch
				{
					return false;
				}
			}
		}

		public static string GetLocalIPAddress()
		{
			foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up &&
					(networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
					 networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
				{
					foreach (var ip in networkInterface.GetIPProperties().UnicastAddresses)
					{
						if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							return ip.Address.ToString();
						}
					}
				}
			}

			throw new NoConnectionException("Local IP Address Not Found!");
		}

	 
	}
}