using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace Utils.Helpers
{
	/// <summary>
	/// Помощник по работе с файлами.
	/// </summary>
	public static class StorageFilesHelper
	{
		public static async Task<string> CreateTempFileGetPath(string desiredName)
		{
			var file = await CreateTemporaryFile(desiredName);
			if (file != null)
			{
				return file.Path;
			}
			return null;
		}

		public static async Task<StorageFile> CreateTemporaryFile(string desiredName)
		{
			StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
			return await tempFolder.CreateFileAsync(desiredName, CreationCollisionOption.GenerateUniqueName);
		}
		public static async Task<StorageFile> CopyFileToTempFolderAsync(StorageFile file)
		{
			StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
			return await file.CopyAsync(tempFolder, file.Name, NameCollisionOption.GenerateUniqueName);
		}
		public static async Task ClearTempFolder()
		{
			try
			{
				StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
				var filesInFolder = await tempFolder.GetFilesAsync();
				foreach (var file in filesInFolder)
				{
					await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
			}
		}

		public static async Task<bool> TrySaveFileAsync(StorageFile file, string suggestedName, string type)
		{
			try
			{
				FileSavePicker filePicker = new FileSavePicker();
				filePicker.FileTypeChoices.Add(type, new List<string>() { type });
				filePicker.SuggestedFileName = suggestedName;
				var pickedFile = await filePicker.PickSaveFileAsync();
				if (pickedFile != null)
				{
					await file.CopyAndReplaceAsync(pickedFile);
					return true;
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
			}

			return false;
		}

		public static async Task UnzipFileAsync(StorageFile zipFile, StorageFolder destinationFolder)
		{
			using var fileStream = await zipFile.OpenStreamForReadAsync();
			ZipArchive arc = new ZipArchive(fileStream);
			arc.ExtractToDirectory(destinationFolder.Path, true);
		}


		public static async Task<StorageFile> TryPickFileAsync(List<string> extensions, PickerLocationId pickerLocation)
		{
			try
			{
				FileOpenPicker filePicker = new FileOpenPicker();
				foreach (string extension in extensions)
				{
					filePicker.FileTypeFilter.Add(extension);
				}

				if (pickerLocation == PickerLocationId.PicturesLibrary)
				{
					filePicker.ViewMode = PickerViewMode.Thumbnail;
					filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
				}
				else
				{
					filePicker.SuggestedStartLocation = pickerLocation;
				}

				return await filePicker.PickSingleFileAsync();
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex, ex.Message);
			}

			return null;
		}
		public static async Task TryDeleteFile(this StorageFile file)
		{
			try
			{
				await file.DeleteAsync();
			}
			catch (Exception e)
			{
				// Отлов ошибки о использовании файла при удалении.
				Log.Logger.Error(e, e.Message);
				
			}
		}

		/// <summary>
		/// Получение <see cref="StorageFile"/> без создания файла на диске
		/// </summary>
		/// <param name="data">Данные, из которых будет создан StorageFile</param>
		/// <param name="extension">Расширение файла</param>
		/// <param name="copyData">Нужно ли копировать данные</param>
		/// <returns>Файл</returns>
		public static async Task<StorageFile> GetStorageFileAsync(byte[] data, string extension, bool copyData)
		{
			byte[] temp;

			if (copyData)
			{
				temp = new byte[data.Length];
				data.CopyTo(temp, 0);
			}
			else
			{
				temp = data;
			}

			return await StorageFile.CreateStreamedFileAsync(extension.StartsWith('.') ? extension : "." + extension, async request =>
			{
				try
				{
					await request.WriteAsync(temp.AsBuffer());
					request.Dispose();
				}
				catch (Exception ex)
				{
					request.FailAndClose(StreamedFileFailureMode.Incomplete);
				}
			}, null);
		}
	}
}
