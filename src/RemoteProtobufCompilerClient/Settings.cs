using Newtonsoft.Json;
using RemoteProtobufCompilerClient.ViewModels;
using System;
using System.IO;

namespace RemoteProtobufCompilerClient
{
	public class Settings
	{
		public static string SettingsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RemoteProtobufCompilerClient\";

		public string SettingsFilePath { get; private set; }

		public MainWindowModel Data { get; protected set; }

		public Settings(string fileName)
		{
			SettingsFilePath = Path.Combine(SettingsFolderPath, fileName);
		}

		public void Save()
		{
			Directory.CreateDirectory(SettingsFolderPath);

			WriteOutSettings(SettingsFilePath, Data);
		}

		public void Load()
		{
			Directory.CreateDirectory(SettingsFolderPath);

			if (File.Exists(SettingsFilePath))
				Data = ReadInSettings();
			else
			{
				var defaultSettings = GetDefaultSettings();

				WriteOutSettings(SettingsFilePath, defaultSettings);

				Data = defaultSettings;
			}
		}

		private MainWindowModel ReadInSettings()
		{
			var data = File.ReadAllText(SettingsFilePath);

			return JsonConvert.DeserializeObject<MainWindowModel>(data);
		}

		private static void WriteOutSettings(string settingsPath, MainWindowModel data)
		{
			var settingsContent = JsonConvert.SerializeObject(data);

			File.WriteAllText(settingsPath, settingsContent);
		}

		protected MainWindowModel GetDefaultSettings() =>
			new MainWindowModel
			{
				OutputType = "C#"
			};
	}
}