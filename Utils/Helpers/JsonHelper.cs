using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System.Text;
using Windows.Storage;

namespace Utils;

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

/// <summary>
/// Сервис работающий с Json конфигами.
/// </summary>
public class JsonHelper
{
	/// <summary>
	/// Сохраняет входящий объект в json сериализуя его.
	/// </summary>
	/// <param name="inputObject">Входящий объект для сериализации.</param>
	/// <param name="file">Файл, куда будет записан json.</param>
	public static async Task SaveJsonAsync(object inputObject, StorageFile file)
	{
		using var clearingFileSteam = await file.OpenStreamForWriteAsync();
		clearingFileSteam.SetLength(0);
		clearingFileSteam.Close();
		await clearingFileSteam.DisposeAsync();

		using StreamWriter sw = new(await file.OpenStreamForWriteAsync());
		using JsonWriter jsonWriter = new JsonTextWriter(sw);

		JsonConverter jsonConverter = new BinaryConverter();
		var jsonSerializer = JsonSerializer.Create();
		jsonSerializer.Converters.Add(jsonConverter);
		jsonSerializer.Formatting = Formatting.Indented;
		jsonSerializer.Serialize(jsonWriter, inputObject);
		await sw.FlushAsync();
	}

	/// <summary>
	/// Сериализует объект.
	/// </summary>
	public static string SerializeJson(object inputObject, bool isNullIgnore = false, ISerializationBinder bulder = null, bool prettify = false )
	{
		if (inputObject == null)
			return null;

		StringBuilder stringBuilder = new StringBuilder();
		using TextWriter textWriter = new StringWriter(stringBuilder);
		using JsonWriter jsonWriter = new JsonTextWriter(textWriter);

		var jsonSerializerSettings = new JsonSerializerSettings
		{
			Formatting = prettify ? Formatting.Indented : Formatting.None,
			TypeNameHandling = TypeNameHandling.Auto, 
			NullValueHandling = NullValueHandling.Ignore,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore
		};
		if (bulder != null)
		{
			jsonSerializerSettings.SerializationBinder = bulder;
		}

		if (isNullIgnore)
		{
			jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
		}
		JsonConverter jsonConverter = new BinaryConverter();
		var jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
		jsonSerializer.Converters.Add(jsonConverter);
		jsonSerializer.Serialize(jsonWriter, inputObject);
		return stringBuilder.ToString();
	}

	/// <summary>
	/// Загружает json в объект нужного типа.
	/// </summary>
	/// <typeparam name="T">Выходной тип данных.</typeparam>
	/// <param name="path">Путь к файлу.</param>
	/// <returns>Объект типа <see cref="T"/>.</returns>
	public static T ParseJsonFromFile<T>(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				using StreamReader jsReader = new StreamReader(path);
				using JsonReader json = new JsonTextReader(jsReader);
				var settings = new JsonSerializerSettings()
				{
					TypeNameHandling = TypeNameHandling.All,
					MaxDepth = 256
				};
				var jsonSerializer = JsonSerializer.Create(settings);
				JsonConverter jsonConverter = new BinaryConverter();
				jsonSerializer.Converters.Add(jsonConverter);
				return jsonSerializer.Deserialize<T>(json);
			}
		}
		catch (Exception ex)
		{
			Log.Logger.Error(ex, ex.Message);
		}

		return default(T);
	}

	/// <summary>
	/// Загружает json в объект нужного типа.
	/// </summary>
	/// <typeparam name="T">Выходной тип данных.</typeparam>
	/// <param name="path">Путь к файлу.</param>
	/// <returns>Объект типа <see cref="T"/>.</returns>
	public static T DeserializeJsonFromFileWithTypes<T>(string path, ISerializationBinder bulder = null)
	{
		try
		{
			if (File.Exists(path))
			{
				using StreamReader jsReader = new StreamReader(path);
				using JsonReader json = new JsonTextReader(jsReader);
				var settings = new JsonSerializerSettings()
				{
					MaxDepth = 256,
					TypeNameHandling = TypeNameHandling.Auto,
				};
				if (bulder != null)
				{
					settings.SerializationBinder = bulder;
				}
				var jsonSerializer = JsonSerializer.Create(settings);
				JsonConverter jsonConverter = new BinaryConverter();
				jsonSerializer.Converters.Add(jsonConverter);
				return jsonSerializer.Deserialize<T>(json);
			}
		}
		catch (Exception ex)
		{
			Log.Logger.Error(ex, ex.Message);
		}

		return default(T);
	}

	public static T CloneObject<T>(object obj, ISerializationBinder bulder = null)
	{
		var serialized = SerializeJson(obj, bulder: bulder);
		return DeserializeJson<T>(serialized);
	}

	public static object CloneObject(object obj, ISerializationBinder bulder = null)
	{
		var serialized = SerializeJson(obj, bulder: bulder);
		return DeserializeJson(serialized, obj.GetType());
	}

	/// <summary>
	/// Парсит json текст в объект
	/// </summary>
	public static T DeserializeJson<T>(string jsonString)
	{
		using TextReader textReader = new StringReader(jsonString);
		using JsonReader json = new JsonTextReader(textReader);
		var serializerSettings = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.All,
			MaxDepth = 256
		};
		var jsonSerializer = JsonSerializer.Create(serializerSettings);
		JsonConverter jsonConverter = new BinaryConverter();
		jsonSerializer.Converters.Add(jsonConverter);
		return jsonSerializer.Deserialize<T>(json);
	}

	/// <summary>
	/// Парсит json текст в объект
	/// </summary>
	public static object DeserializeJson(string jsonString, Type objectType)
	{
		using TextReader textReader = new StringReader(jsonString);
		using JsonReader json = new JsonTextReader(textReader);
		var serializerSettings = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.All,
			MaxDepth = 256
		};
		var jsonSerializer = JsonSerializer.Create(serializerSettings);
		JsonConverter jsonConverter = new BinaryConverter();
		jsonSerializer.Converters.Add(jsonConverter);
		return jsonSerializer.Deserialize(json, objectType);
	}
}