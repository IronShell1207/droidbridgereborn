using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;
using Windows.Storage;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.AppCenter.Crashes;
using Serilog;
using Utils.Helpers;

namespace DroidBridgeReborn.Services
{
	internal class AdbExecutablesService
	{
		private const string _adbExFolderName = "AdbEx";

		public static StorageFolder AdbExecutablesFolder { get; private set; }
		public static async Task<string> GetAdbPathAsync()
		{
			try
			{
				if (AdbExecutablesFolder == null)
					await CheckAndUnpackExecutablesAsync();
				
				
				var adbFile = await AdbExecutablesFolder.GetFileAsync("adb.exe");
				return adbFile.Path;
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
				Crashes.TrackError(ex);
			}

			return string.Empty;
		}

		public static async Task<string> GetScrcpyPathAsync()
		{
			try
			{
				if (AdbExecutablesFolder == null)
					await CheckAndUnpackExecutablesAsync();
				

				var adbFile = await AdbExecutablesFolder.GetFileAsync("scrcpy.exe");
				return adbFile.Path;
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
				Crashes.TrackError(ex);
			}

			return string.Empty;
		}

		public static async Task CheckAndUnpackExecutablesAsync()
		{
			var baseFolder = await
				ApplicationData.Current.LocalFolder.CreateFolderAsync(_adbExFolderName,
					CreationCollisionOption.OpenIfExists);
			var innerFolders = await baseFolder.GetFoldersAsync();

			var orderedFolders = innerFolders.OrderByDescending(x => x.DateCreated);
			bool isAdbUnpacked = false;

			foreach (var folder in orderedFolders)
			{
				bool containsfiles = await folder.FileExistsAsync("adb.exe") ||
				                     await folder.FileExistsAsync("scrcpy.exe");

				if (containsfiles)
				{
					isAdbUnpacked = true;
					AdbExecutablesFolder = folder;
					break;
				}
			}

			if (!isAdbUnpacked)
			{
				var archFile =
					await StorageFile.GetFileFromApplicationUriAsync(
						new Uri("ms-appx:///Assets/Executable/scrcpy-win64.zip"));
				if (archFile == null)
					return;

				await StorageFilesHelper.UnzipFileAsync(archFile, baseFolder);
				await CheckAndUnpackExecutablesAsync();
			}
		}
	}
}