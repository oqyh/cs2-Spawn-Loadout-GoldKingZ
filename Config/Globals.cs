using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Newtonsoft.Json;
using Spawn_Loadout_GoldKingZ.Config;

namespace Spawn_Loadout_GoldKingZ;

public class Globals
{
    public bool RemoveBuyMenu = false;
    public bool RemoveKnife = false;
    public bool ForceRemoveServerCommands = false;
    public bool ForceRemoveClientCommands = false;
    public bool ForceStripPlayers = false;
    public int ForceRemoveGroundWeapons = 0;
    public int GiveArmor = -1;
    public int GiveHealth = -1;
    public float DelayGiveLoadOut = 0.0f;

    public class GetPlayerWeapons
    {
        public CCSPlayerController Player { get; set; }
        public CounterStrikeSharp.API.Modules.Timers.Timer Timer{ get; set; }
        public CounterStrikeSharp.API.Modules.Timers.Timer KillTheTimer{ get; set; }

        public HashSet<string> WeaponsNeeded { get; set; }
        public GetPlayerWeapons(CCSPlayerController player, CounterStrikeSharp.API.Modules.Timers.Timer timer, CounterStrikeSharp.API.Modules.Timers.Timer killTheTimer, HashSet<string> weaponsNeeded)
        {
            Player = player;
            Timer = timer;
            KillTheTimer = killTheTimer;
            WeaponsNeeded = weaponsNeeded;
        }
    }
    public Dictionary<CCSPlayerController, GetPlayerWeapons> PlayerCleanUp = new Dictionary<CCSPlayerController, GetPlayerWeapons>();
    public  Dictionary<CCSPlayerController, HashSet<string>> loadoutsGivenPerPlayer = new Dictionary<CCSPlayerController, HashSet<string>>();

    public Dictionary<string, dynamic> jsonValuesCache = new Dictionary<string, dynamic>();
    public Dictionary<string, dynamic> GetJsonValues()
    {
        if (jsonValuesCache.Count > 0) 
        {
            Helper.DebugMessage("Returning cached jsonValuesCache.");
            return jsonValuesCache;
        }

        string mapname = NativeAPI.GetMapName();
        if (string.IsNullOrEmpty(mapname))
        {
            Helper.DebugMessage("Map Name Is Empty. Can't GetJsonValues.");
            return jsonValuesCache;
        }

        int underscoreIndex = mapname.IndexOf('_');
        int nextUnderscoreIndex = mapname.IndexOf('_', underscoreIndex + 1);
        
        string prefix = underscoreIndex != -1 ? mapname.Substring(0, underscoreIndex + 1) : mapname;
        string prefix2 = underscoreIndex != -1 && nextUnderscoreIndex != -1 ? mapname.Substring(0, nextUnderscoreIndex + 1) : mapname;

        string jsonFilePath = $"{Configs.Shared.CookiesModule}/../../plugins/Spawn-Loadout-GoldKingZ/config/Weapons_Settings.json";

        if (!File.Exists(jsonFilePath)) 
        {
            Helper.DebugMessage($"JSON file does not exist at path: {jsonFilePath}. Returning cached jsonValuesCache.");
            return jsonValuesCache;
        }

        string jsonContent = File.ReadAllText(jsonFilePath);
        if (string.IsNullOrEmpty(jsonContent)) 
        {
            Helper.DebugMessage("JSON content is empty. Returning cached jsonValuesCache.");
            return jsonValuesCache;
        }

        dynamic jsonData = JsonConvert.DeserializeObject(jsonContent)!;
        dynamic GetMap = null!;

        if (jsonData.ContainsKey("ANY"))
        {
            GetMap = jsonData["ANY"];
        }
        else if (jsonData.ContainsKey(prefix))
        {
            GetMap = jsonData[prefix];
        }
        else if (jsonData.ContainsKey(prefix2))
        {
            GetMap = jsonData[prefix2];
        }
        else if (jsonData.ContainsKey(mapname))
        {
            GetMap = jsonData[mapname];
        }
        
        if (GetMap != null)
        {
            foreach (var item in GetMap)
            {
                jsonValuesCache[item.Name] = item.Value;
            }
        }
        else
        {
            Helper.DebugMessage("GetMap is null. Returning cached jsonValuesCache.");
        }

        return jsonValuesCache;
    }

    public void ClearJsonValues()
    {
        jsonValuesCache.Clear();
    }
}