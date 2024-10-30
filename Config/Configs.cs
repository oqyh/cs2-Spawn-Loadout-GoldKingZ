using Microsoft.Extensions.Localization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spawn_Loadout_GoldKingZ.Config
{
    public static class Configs
    {
        public static class Shared {
            public static string? CookiesModule { get; set; }
            public static IStringLocalizer? StringLocalizer { get; set; }
            public static CustomGameData? CustomFunctions { get; set; }
        }

        private static readonly string ConfigDirectoryName = "config";
        private static readonly string ConfigDirectoryName2 = "gamedata";
        private static readonly string ConfigFileName = "config.json";
        private static readonly string jsonFilePath2 = "Spawn_Loadout_gamedata.json";
        private static string? _jsonFilePath2;
        private static readonly string jsonFilePath3 = "Weapons_Settings.json";
        private static string? _jsonFilePath3;
        private static string? _configFilePath;
        private static ConfigData? _configData;

        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter()
            },
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static bool IsLoaded()
        {
            return _configData is not null;
        }

        public static ConfigData GetConfigData()
        {
            if (_configData is null)
            {
                Helper.DebugMessage($"Config not yet loaded.");
                throw new Exception("Config not yet loaded.");
            }
            
            return _configData;
        }

        public static ConfigData Load(string modulePath)
        {
            var configFileDirectory = Path.Combine(modulePath, ConfigDirectoryName);
            if(!Directory.Exists(configFileDirectory))
            {
                Directory.CreateDirectory(configFileDirectory);
            }

            var configFileDirectory2 = Path.Combine(modulePath, ConfigDirectoryName2);
            if(!Directory.Exists(configFileDirectory2))
            {
                Directory.CreateDirectory(configFileDirectory2);
            }
            

            _jsonFilePath2 = Path.Combine(configFileDirectory2, jsonFilePath2);
            Helper.CreateGameData(_jsonFilePath2);

            _jsonFilePath3 = Path.Combine(configFileDirectory, jsonFilePath3);
            Helper.CreateWeaponsJson(_jsonFilePath3);

            _configFilePath = Path.Combine(configFileDirectory, ConfigFileName);
            if (File.Exists(_configFilePath))
            {
                _configData = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(_configFilePath), SerializationOptions);
            }
            else
            {
                _configData = new ConfigData();
            }

            if (_configData is null)
            {
                Helper.DebugMessage($"Failed to load configs.");
                throw new Exception("Failed to load configs.");
            }

            SaveConfigData(_configData);
            
            return _configData;
        }

        private static void SaveConfigData(ConfigData configData)
        {
            if (_configFilePath is null)
            {
                Helper.DebugMessage($"Config not yet loaded.");
                throw new Exception("Config not yet loaded.");
            }
            string json = JsonSerializer.Serialize(configData, SerializationOptions);


            json = System.Text.RegularExpressions.Regex.Unescape(json);
            File.WriteAllText(_configFilePath, json, System.Text.Encoding.UTF8);
        }

        public class ConfigData
        {
            public string empty { get; set; }
            public string SL_Reload_Weapons_Settings_Flags { get; set; }
            public string SL_Reload_Weapons_Settings_CommandsInGame { get; set; }
            public string empty1 { get; set; }
            public bool EnableDebug { get; set; }
            public string empty2 { get; set; }
            public string Information_For_You_Dont_Delete_it { get; set; }
            
            public ConfigData()
            {
                empty = "----------------------------[ ↓ Spawn Loadout Configs ↓ ]---------------------------------";
                SL_Reload_Weapons_Settings_Flags = "@css/root,@css/admin";
                SL_Reload_Weapons_Settings_CommandsInGame = "!loadouts,!relaodloadouts,!restartloadouts,!loadout,!relaodloadout,!restartloadout";
                empty1 = "----------------------------[ ↓ Debug ↓ ]----------------------------------------------";
                EnableDebug = false;
                empty2 = "----------------------------[ ↓ Info For All Configs Above ↓ ]----------------------------";
                Information_For_You_Dont_Delete_it = " Vist  [https://github.com/oqyh/cs2-Spawn-Loadout-GoldKingZ/tree/main?tab=readme-ov-file#-configuration-] To Understand All Above";
            }
        }
    }
}