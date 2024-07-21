using AdbCore.Abstraction;
using DroidBridgeReborn.ViewModels;

namespace DroidBridgeReborn
{
	using AdbCore.Service;
	using DroidBridgeReborn.Helpers;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.UI.Xaml;
	using Serilog;
	using Serilog.Sinks.SystemConsole.Themes;
	using System;
	using System.Threading.Tasks;
	using SystemHelpers.Static;
	using Win.Utils.Helpers;

	public partial class App : Application
	{
		private static bool _servicesInitialized = false;
		public static MainWindow MainWindow { get; private set; }
		public static IServiceProvider ServiceProvider { get; private set; }

		public App()
		{
			this.InitializeComponent();
			SetupLogger();
			ConfigureServices();
			DispatcherQueueHelper.SetCurrentThread();
#if !DEBUG
			UnhandledException += App_UnhandledException;
#endif
		}

		private void SetupLogger()
		{
#if DEBUG
			PinvokeWindowMethods.AllocConsole();
#endif
			Log.Logger = new LoggerConfiguration().MinimumLevel.Verbose()
				.WriteTo.Console(theme: AnsiConsoleTheme.Code)
				.CreateLogger();
		}

		public static async void CreateMainWindow()
		{
			MainWindow = new MainWindow();
			MainWindow.Activate();

			if (_servicesInitialized)
			{
				//MainWindow.Initialize();
			}
		}

		public async Task InitializeServices()
		{
			if (_servicesInitialized)
				return;

			var adbService = ServiceProvider.GetRequiredService<IADBServiceMonitor>();
			adbService.Initialize(SettingsHelper.GetAdbUpdateInterval());
			var devicesUpdateService = ServiceProvider.GetRequiredService<AdbDevicesListUpdaterService>();
			devicesUpdateService.Initialize(SettingsHelper.GetDevicesListUpdateInterval());
			_servicesInitialized = true;
			_ = DevicePageViewModel.Instance;
			_ = DevicesListViewModel.Instance;
		}

		protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
		{
			string[] cmdLaunchArgs = Environment.GetCommandLineArgs();

			if (!CheckSecondStart(cmdLaunchArgs))
				return;

			base.OnLaunched(args);

			if (SettingsHelper.IsStartingMinimizedSetting.Value == false)
				CreateMainWindow();

			await InitializeServices();

			if (MainWindow != null)
			{
				//MainWindow.Initialize();
			}
		}

		private void ConfigureServices()
		{
			var services = new ServiceCollection();
			services.AddSingleton<IADBServiceMonitor, ADBServiceMonitor>();
			services.AddSingleton<AndroidBridgeCommandExecutor>();
			services.AddSingleton<AdbDevicesListUpdaterService>();
			ServiceProvider = services.BuildServiceProvider();
			
			Log.Logger.Information("Services initialized");
		}
	}
}