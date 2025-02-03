using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Newtonsoft.Json;
using Spawn_Loadout_GoldKingZ.Config;

namespace Spawn_Loadout_GoldKingZ;

public class Globals
{
    public bool RemoveBuyMenu = false;
    public bool RemoveKnifeCT = false;
    public bool RemoveKnifeT = false;
    public bool ForceRemoveServerCommands = false;
    public bool ForceRemoveClientCommands = false;
    public bool ForceStripCTPlayers = false;
    public bool ForceStripTPlayers = false;
    public bool ForceRemoveGroundWeapons = false;
    public int GiveArmorCT = -1;
    public int GiveArmorT = -1;
    public int GiveHealthCT = -1;
    public int GiveHealthT = -1;
    public float DelayGiveLoadOut = 0.0f;

    public Dictionary<CCSPlayerController, HashSet<string>> loadoutsGivenPerPlayer = new Dictionary<CCSPlayerController, HashSet<string>>();
    public Dictionary<string, dynamic>? JsonData { get; set; }
}