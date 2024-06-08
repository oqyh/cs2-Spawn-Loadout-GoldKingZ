using Newtonsoft.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using Spawn_Loadout_GoldKingZ.Config;
using Newtonsoft.Json.Linq;
using System.Net;
using CounterStrikeSharp.API.Modules.Entities;

namespace Spawn_Loadout_GoldKingZ;

public class SpawnLoadoutGoldKingZ : BasePlugin
{
    public override string ModuleName => "Give Weapons On Spawn (Depend The Map Name + Team Side)";
    public override string ModuleVersion => "1.0.3";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    
    public override void Load(bool hotReload)
    {
        Configs.Shared.CookiesModule = ModuleDirectory;
        Configs.Shared.CustomFunctions = new CustomGameData();
        Helper.CreateDefaultWeaponsJson();
        RegisterEventHandler<EventPlayerSpawn>(OnEventPlayerSpawn, HookMode.Post);
        RegisterEventHandler<EventGrenadeThrown>(OnEventGrenadeThrown, HookMode.Post);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterEventHandler<EventRoundStart>(OnEventRoundStart);
    }
    public HookResult OnEventGrenadeThrown(EventGrenadeThrown @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
        var player = @event.Userid;
        var nadename = @event.Weapon;
        if (player == null || !player.IsValid) return HookResult.Continue;
        var playerid = player.SteamID;
        
        string mapname = Globals.SMapName;
        int underscoreIndex = Globals.SMapName.IndexOf('_');
        int nextUnderscoreIndex = Globals.SMapName.IndexOf('_', underscoreIndex + 1);
        string prefix = underscoreIndex != -1 ? Globals.SMapName.Substring(0, underscoreIndex + 1) : Globals.SMapName;
        string prefix2 = underscoreIndex != -1 ? Globals.SMapName.Substring(0, nextUnderscoreIndex + 1) : Globals.SMapName;

        string jsonFilePath = $"{ModuleDirectory}/../../plugins/Spawn-Loadout-GoldKingZ/config/weapons.json";

        if (!File.Exists(jsonFilePath))
        {
            return HookResult.Continue;
        }

        string jsonString = File.ReadAllText(jsonFilePath);
        dynamic jsonData = JsonConvert.DeserializeObject(jsonString)!;

        dynamic TeamWeapon = null!;
        if(jsonData.ContainsKey("ANY"))
        {
           TeamWeapon = jsonData["ANY"]; 
        }else if(jsonData.ContainsKey(prefix))
        {
            TeamWeapon = jsonData[prefix]; 
        }else if(jsonData.ContainsKey(prefix2))
        {
            TeamWeapon = jsonData[prefix2]; 
        }else if(jsonData.ContainsKey(mapname))
        {
            TeamWeapon = jsonData[mapname]; 
        }

        if (TeamWeapon != null)
        {
            foreach (var loadout in TeamWeapon.Properties())
            {
                string loadoutName = loadout.Name;
                JToken loadoutValueToken = loadout.Value;
                if (loadoutValueToken.Type == JTokenType.Object)
                {
                    JObject loadoutValue = (JObject)loadoutValueToken;

                    string HasFlag = loadoutValue.ContainsKey("FLAGS") ? loadoutValue["FLAGS"]!.ToString() : null!;
                    string ctREFILENades = loadoutValue.ContainsKey("CT_Refill_Nades") ? loadoutValue["CT_Refill_Nades"]!.ToString() : null!;
                    string tREFILENades = loadoutValue.ContainsKey("T_Refill_Nades") ? loadoutValue["T_Refill_Nades"]!.ToString() : null!;
                    int ctRefillTime = loadoutValue.ContainsKey("CT_Refill_Time_InSec") ? (int)loadoutValue["CT_Refill_Time_InSec"]! : 1;
                    int tRefillTime = loadoutValue.ContainsKey("T_Refill_Time_InSec") ? (int)loadoutValue["T_Refill_Time_InSec"]! : 1;
                    bool giveOncePerRound = loadoutValue.ContainsKey("GiveThisLoadOutPerRoundOnly") ? (bool)loadoutValue["GiveThisLoadOutPerRoundOnly"]! : false;

                    if (HasFlag != null && !string.IsNullOrEmpty(HasFlag) && !Helper.IsPlayerInGroupPermission(player, HasFlag)) continue;
                    if (giveOncePerRound)
                    {
                        if (!Globals.loadoutsGivenPerPlayer.ContainsKey(playerid))
                        {
                            Globals.loadoutsGivenPerPlayer[playerid] = new HashSet<string>();
                        }

                        if (Globals.loadoutsGivenPerPlayer[playerid].Contains(loadoutName))
                        {
                            continue;
                        }
                        else
                        {
                            Globals.loadoutsGivenPerPlayer[playerid].Add(loadoutName);
                        }
                    }

                    if (player.TeamNum == (byte)CsTeam.CounterTerrorist && ctREFILENades != null)
                    {
                        var ctRefillNadesArray = ctREFILENades.Split(',');
                        foreach (var grenade in ctRefillNadesArray)
                        {
                            if ("weapon_" + nadename == grenade.Trim())
                            {
                                Server.NextFrame(() =>
                                {
                                    AddTimer(ctRefillTime, () =>
                                    {
                                        if(player != null && player.IsValid && player.PawnIsAlive && player.TeamNum == (byte)CsTeam.CounterTerrorist)
                                        {
                                            Helper.GiveWeaponsToPlayer(player, "weapon_" + nadename);
                                        }
                                    }, TimerFlags.STOP_ON_MAPCHANGE);
                                });
                                break;
                            }
                        }
                    }else if (player.TeamNum == (byte)CsTeam.Terrorist && tREFILENades != null)
                    {
                        var tRefillNadesArray = tREFILENades.Split(',');
                        foreach (var grenade in tRefillNadesArray)
                        {
                            if ("weapon_" + nadename == grenade.Trim())
                            {
                                Server.NextFrame(() =>
                                {
                                    AddTimer(tRefillTime, () =>
                                    {
                                        if(player != null && player.IsValid && player.PawnIsAlive && player.TeamNum == (byte)CsTeam.Terrorist)
                                        {
                                            Helper.GiveWeaponsToPlayer(player, "weapon_" + nadename);
                                        }
                                    }, TimerFlags.STOP_ON_MAPCHANGE);
                                });
                                break;
                            }
                        }
                    }
                }
            }
        }

        return HookResult.Continue;
    }
    
    private HookResult OnEventPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
        var player = @event.Userid;
        if (player == null || !player.IsValid) return HookResult.Continue;
        var playerid = player.SteamID;

        string mapname = Globals.SMapName;
        int underscoreIndex = Globals.SMapName.IndexOf('_');
        int nextUnderscoreIndex = Globals.SMapName.IndexOf('_', underscoreIndex + 1);
        string prefix = underscoreIndex != -1 ? Globals.SMapName.Substring(0, underscoreIndex + 1) : Globals.SMapName;
        string prefix2 = underscoreIndex != -1 ? Globals.SMapName.Substring(0, nextUnderscoreIndex + 1) : Globals.SMapName;

        string jsonFilePath = $"{ModuleDirectory}/../../plugins/Spawn-Loadout-GoldKingZ/config/weapons.json";

        if (!File.Exists(jsonFilePath))
        {
            return HookResult.Continue;
        }

        string jsonString = File.ReadAllText(jsonFilePath);
        JObject jsonData = JObject.Parse(jsonString);

        JObject TeamWeapon = null!;
        if (jsonData.ContainsKey("ANY"))
        {
            TeamWeapon = (JObject)jsonData["ANY"]!;
        }
        else if (jsonData.ContainsKey(prefix))
        {
            TeamWeapon = (JObject)jsonData[prefix]!;
        }
        else if (jsonData.ContainsKey(prefix2))
        {
            TeamWeapon = (JObject)jsonData[prefix2]!;
        }
        else if (jsonData.ContainsKey(mapname))
        {
            TeamWeapon = (JObject)jsonData[mapname]!;
        }

        if (TeamWeapon != null)
        {
            Server.NextFrame(() =>
            {
                bool forceStripPlayers = false;
                bool AutoDeleteGroundWeapns = false;

                if (TeamWeapon.ContainsKey("ForceStripPlayers") && TeamWeapon["ForceStripPlayers"]!.Type == JTokenType.Boolean)
                {
                    forceStripPlayers = (bool)TeamWeapon["ForceStripPlayers"]!;
                }

                if (TeamWeapon.ContainsKey("DeleteGroundWeapons") && TeamWeapon["DeleteGroundWeapons"]!.Type == JTokenType.Boolean)
                {
                    AutoDeleteGroundWeapns = (bool)TeamWeapon["DeleteGroundWeapons"]!;
                }

                if(forceStripPlayers)
                {
                    AddTimer(0.1f, () =>
                    {
                        Helper.DropAllWeapons(player);
                    });
                }

                if(AutoDeleteGroundWeapns)
                {
                    AddTimer(3.0f, () =>
                    {
                        Helper.ClearGroundWeapons();
                    });
                }
                
                foreach (var loadout in TeamWeapon.Properties())
                {
                    string loadoutName = loadout.Name;
                    JToken loadoutValueToken = loadout.Value;
                    if (loadoutValueToken.Type == JTokenType.Object)
                    {
                        JObject loadoutValue = (JObject)loadoutValueToken;
                        string HasFlag = loadoutValue.ContainsKey("FLAGS") ? loadoutValue["FLAGS"]!.ToString() : null!;
                        string ctWeapons = loadoutValue.ContainsKey("CT") ? loadoutValue["CT"]!.ToString() : null!;
                        string tWeapons = loadoutValue.ContainsKey("T") ? loadoutValue["T"]!.ToString() : null!;
                        bool giveOncePerRound = loadoutValue.ContainsKey("GiveThisLoadOutPerRoundOnly") ? (bool)loadoutValue["GiveThisLoadOutPerRoundOnly"]! : false;
                        

                        if (HasFlag != null && !string.IsNullOrEmpty(HasFlag) && !Helper.IsPlayerInGroupPermission(player, HasFlag)) continue;
                        if (giveOncePerRound)
                        {
                            if (!Globals.loadoutsGivenPerPlayer.ContainsKey(playerid))
                            {
                                Globals.loadoutsGivenPerPlayer[playerid] = new HashSet<string>();
                            }

                            if (Globals.loadoutsGivenPerPlayer[playerid].Contains(loadoutName))
                            {
                                continue;
                            }
                            else
                            {
                                Globals.loadoutsGivenPerPlayer[playerid].Add(loadoutName);
                            }
                        }
                        float delay = 0.1f;
                        if(forceStripPlayers)
                        {
                            delay = 2.0f;
                        }

                        AddTimer(delay, () =>
                        {
                            if (player != null && player.IsValid && player.TeamNum == (byte)CsTeam.CounterTerrorist && ctWeapons != null)
                            {
                                string[] weaponsCT = ctWeapons.Split(',');
                                foreach (var Weapon in weaponsCT)
                                {
                                    if (string.IsNullOrWhiteSpace(Weapon))continue;
                                    Helper.GiveWeaponsToPlayer(player, Weapon);
                                    foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(Weapon))
                                    {
                                        if (entity.DesignerName != Weapon) continue;
                                        if (entity == null) continue;
                                        if (entity.Entity == null) continue;
                                        if (entity.OwnerEntity == null) continue;
                                        if (entity.OwnerEntity.IsValid) continue;
                                        entity.AcceptInput("Kill");
                                    }
                                    
                                }
                            }
                            else if (player != null && player.IsValid && player.TeamNum == (byte)CsTeam.Terrorist && tWeapons != null)
                            {
                                string[] weaponsT = tWeapons.Split(',');
                                foreach (var Weapon in weaponsT)
                                {
                                    
                                    if (string.IsNullOrWhiteSpace(Weapon))continue;
                                    Helper.GiveWeaponsToPlayer(player, Weapon);
                                    foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(Weapon))
                                    {
                                        if (entity.DesignerName != Weapon) continue;
                                        if (entity == null) continue;
                                        if (entity.Entity == null) continue;
                                        if (entity.OwnerEntity == null) continue;
                                        if (entity.OwnerEntity.IsValid) continue;
                                        entity.AcceptInput("Kill");
                                    }
                                }  
                            }
                        });
                    }
                }
            });
        }

        return HookResult.Continue;
    }


    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid)return HookResult.Continue;

        Globals.loadoutsGivenPerPlayer.Remove(player.SteamID);

        return HookResult.Continue;
    }
    private HookResult OnEventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        Globals.loadoutsGivenPerPlayer.Clear();

        return HookResult.Continue;
    }
    public void OnMapEnd()
    {
        Helper.ClearVariables();
    }
    public override void Unload(bool hotReload)
    {
        Helper.ClearVariables();
    }
}