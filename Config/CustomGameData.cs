using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Spawn_Loadout_GoldKingZ.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Spawn_Loadout_GoldKingZ;

public class CustomGameData
{
    public static Dictionary<string, Dictionary<OSPlatform, string>> _customGameData = new();

    private readonly MemoryFunctionVoid<IntPtr, string, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, IntPtr>? GiveNamedItem2;

    public CustomGameData()
    {
        LoadCustomGameDataFromJson();

        GiveNamedItem2 = new(GetCustomGameDataKey("GiveNamedItem2"));
    }
    public void LoadCustomGameDataFromJson()
    {
        string jsonFilePath = $"{Configs.Shared.CookiesModule}/../../plugins/Spawn-Loadout-GoldKingZ/gamedata/Spawn_Loadout_gamedata.json";
        if (!File.Exists(jsonFilePath))
        {
            Helper.DebugMessage($"JSON file does not exist at path: {jsonFilePath}. Returning without loading custom game data.");
            return;
        }
        
        try
        {
            var jsonData = File.ReadAllText(jsonFilePath);
            var jsonObject = JObject.Parse(jsonData);

            foreach (var item in jsonObject.Properties())
            {
                string key = item.Name;
                var platformData = new Dictionary<OSPlatform, string>();
                var signatures = item.Value["signatures"];
                if (signatures != null)
                {
                    if (signatures["windows"] != null)
                    {
                        platformData[OSPlatform.Windows] = signatures["windows"]!.ToString();
                    }
                    if (signatures["linux"] != null)
                    {
                        platformData[OSPlatform.Linux] = signatures["linux"]!.ToString();
                    }
                }

                _customGameData[key] = platformData;
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Error loading custom game data: {ex.Message}");
        }
    }

    public string GetCustomGameDataKey(string key)
    {
        if (!_customGameData.TryGetValue(key, out var customGameData))
        {
            Helper.DebugMessage($"Invalid key {key}. Throwing exception.");
            throw new Exception($"Invalid key {key}");
        }

        OSPlatform platform;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            platform = OSPlatform.Linux;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            platform = OSPlatform.Windows;
        }
        else
        {
            Helper.DebugMessage("Unsupported platform. Throwing exception.");
            throw new Exception("Unsupported platform");
        }

        return customGameData.TryGetValue(platform, out var customData)
            ? customData
            : throw new Exception($"Missing custom data for {key} on {platform}");
    }
    public void PlayerGiveNamedItem(CCSPlayerController player, string item)
    {
        if (!player.PlayerPawn.IsValid) return;
        if (player.PlayerPawn.Value == null) return;
        if (!player.PlayerPawn.Value.IsValid) return;
        if (player.PlayerPawn.Value.ItemServices == null) return;

        if (GiveNamedItem2 is not null)
        {
            GiveNamedItem2.Invoke(player.PlayerPawn.Value.ItemServices.Handle, item, 0, 0, 0, 0, 0, 0);
        }
        else
        {
            player.GiveNamedItem(item);
        }
    }
}