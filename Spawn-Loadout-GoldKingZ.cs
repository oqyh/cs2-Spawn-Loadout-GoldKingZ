using Newtonsoft.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using Spawn_Loadout_GoldKingZ.Config;
namespace Spawn_Loadout_GoldKingZ;

public class SpawnLoadoutGoldKingZ : BasePlugin
{
    public override string ModuleName => "Give Weapons On Spawn (Depend The Map Name + Team Side)";
    public override string ModuleVersion => "1.0.1";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    
    private CustomGameData? CustomFunctions { get; set; }
    public override void Load(bool hotReload)
    {
        Configs.Load(ModuleDirectory);
        CustomFunctions = new CustomGameData();
        CreateDefaultWeaponsJson();
        RegisterEventHandler<EventPlayerConnectFull>(OnEventPlayerConnectFull);
        RegisterEventHandler<EventPlayerSpawn>(OnEventPlayerSpawn, HookMode.Post);
        RegisterEventHandler<EventGrenadeThrown>(OnEventGrenadeThrown, HookMode.Post);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterEventHandler<EventRoundStart>(OnEventRoundStart);
    }
    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV) return HookResult.Continue;
        var playerid = player.SteamID;

        if(!string.IsNullOrEmpty(Configs.GetConfigData().Vips) && Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().Vips))
        {
            if (!Globals.VipsFlag.ContainsKey(playerid))
            {
                Globals.VipsFlag.Add(playerid, true);
            }
        }

        return HookResult.Continue;
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
            if (player.TeamNum == (byte)CsTeam.CounterTerrorist)
            {
                string[] weaponsCT;
                weaponsCT = TeamWeapon["CT_Refill_Nades"]?.ToString()?.Split(',') ?? new string[0];
                float TimerCT = TeamWeapon?.ContainsKey("CT_Refill_Time_InSec") == true ? TeamWeapon["CT_Refill_Time_InSec"] : 1;

                foreach (string weaponCT in weaponsCT)
                {
                    if (string.IsNullOrWhiteSpace(weaponCT))continue;
                    if(Configs.GetConfigData().GiveOneTimeRefillNadesPerRound && Globals.NadeGived.ContainsKey(playerid))continue;
                    if("weapon_" + nadename == weaponCT)
                    {
                        Server.NextFrame(() =>
                        {
                            AddTimer(TimerCT, () =>
                            {
                                if (player != null && player.IsValid)
                                {
                                    CustomFunctions!.PlayerGiveNamedItem(player, "weapon_" + nadename);
                                    if (Configs.GetConfigData().GiveOneTimeRefillNadesPerRound && !Globals.NadeGived.ContainsKey(playerid))
                                    {
                                        Globals.NadeGived.Add(playerid, true);
                                    }
                                    
                                    foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(weaponCT))
                                    {
                                        if (entity.DesignerName != weaponCT) continue;
                                        if (entity == null) continue;
                                        if (entity.Entity == null) continue;
                                        if (entity.OwnerEntity == null) continue;
                                        if(entity.OwnerEntity.IsValid) continue;

                                        entity.AcceptInput("Kill");
                                    }
                                }
                            }, TimerFlags.STOP_ON_MAPCHANGE);
                        });
                    }
                }
                
                    

                if(Globals.VipsFlag.ContainsKey(playerid))
                {
                    string[] weaponsCTVip;
                    weaponsCTVip = TeamWeapon!["CT_Vip_Refill_Nades"]?.ToString()?.Split(',') ?? new string[0];
                    float TimerCTVip = TeamWeapon?.ContainsKey("CT_Vip_Refill_Time_InSec") == true ? TeamWeapon["CT_Vip_Refill_Time_InSec"] : 1;

                    foreach (string weaponCTVip in weaponsCTVip)
                    {
                        if (string.IsNullOrWhiteSpace(weaponCTVip))continue;
                        if(Configs.GetConfigData().Vips_GiveOneTimeRefillNadesPerRound && Globals.VipNadeGived.ContainsKey(playerid))continue;
                        if("weapon_" + nadename == weaponCTVip)
                        {
                            Server.NextFrame(() =>
                            {
                                AddTimer(TimerCTVip, () =>
                                {
                                    if (player != null && player.IsValid)
                                    {
                                        CustomFunctions!.PlayerGiveNamedItem(player, "weapon_" + nadename);
                                        if (Configs.GetConfigData().Vips_GiveOneTimeRefillNadesPerRound && !Globals.VipNadeGived.ContainsKey(playerid))
                                        {
                                            Globals.NadeGived.Add(playerid, true);
                                        }
                                        
                                        foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(weaponCTVip))
                                        {
                                            if (entity.DesignerName != weaponCTVip) continue;
                                            if (entity == null) continue;
                                            if (entity.Entity == null) continue;
                                            if (entity.OwnerEntity == null) continue;
                                            if(entity.OwnerEntity.IsValid) continue;

                                            entity.AcceptInput("Kill");
                                        }
                                    }
                                }, TimerFlags.STOP_ON_MAPCHANGE);
                            });
                        }
                    }
                }
                
            }else if (player.TeamNum == (byte)CsTeam.Terrorist)
            {
                string[] weaponsT;
                weaponsT = TeamWeapon["T_Refill_Nades"]?.ToString()?.Split(',') ?? new string[0];
                float TimerT = TeamWeapon?.ContainsKey("T_Refill_Time_InSec") == true ? TeamWeapon["T_Refill_Time_InSec"] : 1;

                foreach (string weaponT in weaponsT)
                {
                    if (string.IsNullOrWhiteSpace(weaponT))continue;
                    if(Configs.GetConfigData().GiveOneTimeRefillNadesPerRound && Globals.NadeGived.ContainsKey(playerid))continue;
                    if("weapon_" + nadename == weaponT)
                    {
                        Server.NextFrame(() =>
                        {
                            AddTimer(TimerT, () =>
                            {
                                if (player != null && player.IsValid)
                                {
                                    CustomFunctions!.PlayerGiveNamedItem(player, "weapon_" + nadename);
                                    if (Configs.GetConfigData().GiveOneTimeRefillNadesPerRound && !Globals.NadeGived.ContainsKey(playerid))
                                    {
                                        Globals.NadeGived.Add(playerid, true);
                                    }
                                    
                                    foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(weaponT))
                                    {
                                        if (entity.DesignerName != weaponT) continue;
                                        if (entity == null) continue;
                                        if (entity.Entity == null) continue;
                                        if (entity.OwnerEntity == null) continue;
                                        if(entity.OwnerEntity.IsValid) continue;

                                        entity.AcceptInput("Kill");
                                    }
                                }
                            }, TimerFlags.STOP_ON_MAPCHANGE);
                        });
                    }
                }
                
                    

                if(Globals.VipsFlag.ContainsKey(playerid))
                {
                    string[] weaponsTVip;
                    weaponsTVip = TeamWeapon!["T_Vip_Refill_Nades"]?.ToString()?.Split(',') ?? new string[0];
                    float TimerTVip = TeamWeapon?.ContainsKey("T_Vip_Refill_Time_InSec") == true ? TeamWeapon["T_Vip_Refill_Time_InSec"] : 1;

                    foreach (string weaponTVip in weaponsTVip)
                    {
                        if (string.IsNullOrWhiteSpace(weaponTVip))continue;
                        if(Configs.GetConfigData().Vips_GiveOneTimeRefillNadesPerRound && Globals.VipNadeGived.ContainsKey(playerid))continue;
                        if("weapon_" + nadename == weaponTVip)
                        {
                            Server.NextFrame(() =>
                            {
                                AddTimer(TimerTVip, () =>
                                {
                                    if (player != null && player.IsValid)
                                    {
                                        CustomFunctions!.PlayerGiveNamedItem(player, "weapon_" + nadename);
                                        if (Configs.GetConfigData().Vips_GiveOneTimeRefillNadesPerRound && !Globals.VipNadeGived.ContainsKey(playerid))
                                        {
                                            Globals.NadeGived.Add(playerid, true);
                                        }
                                        
                                        foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(weaponTVip))
                                        {
                                            if (entity.DesignerName != weaponTVip) continue;
                                            if (entity == null) continue;
                                            if (entity.Entity == null) continue;
                                            if (entity.OwnerEntity == null) continue;
                                            if(entity.OwnerEntity.IsValid) continue;

                                            entity.AcceptInput("Kill");
                                        }
                                    }
                                }, TimerFlags.STOP_ON_MAPCHANGE);
                            });
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
            if (player.TeamNum == (byte)CsTeam.CounterTerrorist)
            {
                string[] weaponsCT;
                weaponsCT = TeamWeapon["CT"]?.ToString()?.Split(',') ?? new string[0];
                foreach (string weapon in weaponsCT)
                {
                    if (string.IsNullOrWhiteSpace(weapon))continue;
                    if(Configs.GetConfigData().GiveOneTimeLoadOutPerRound && Globals.Gived.ContainsKey(playerid))continue;

                    CustomFunctions!.PlayerGiveNamedItem(player, weapon);

                    if (Configs.GetConfigData().GiveOneTimeLoadOutPerRound && !Globals.Gived.ContainsKey(playerid))
                    {
                        Globals.Gived.Add(playerid, true);
                    }
                    
                    foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(weapon))
                    {
                        if (entity.DesignerName != weapon) continue;
                        if (entity == null) continue;
                        if (entity.Entity == null) continue;
                        if (entity.OwnerEntity == null) continue;
                        if(entity.OwnerEntity.IsValid) continue;

                        entity.AcceptInput("Kill");
                    }
                    
                }
                

                if(Globals.VipsFlag.ContainsKey(playerid))
                {
                    string[] weaponsCTVip;
                    weaponsCTVip = TeamWeapon["CT_Vip"]?.ToString()?.Split(',') ?? new string[0];
                    foreach (string weapon in weaponsCTVip)
                    {
                        if (string.IsNullOrWhiteSpace(weapon))continue;
                        if(Configs.GetConfigData().Vips_GiveOneTimeLoadOutPerRound && Globals.Gived.ContainsKey(playerid))continue;

                        CustomFunctions!.PlayerGiveNamedItem(player, weapon);

                        if (Configs.GetConfigData().Vips_GiveOneTimeLoadOutPerRound && !Globals.Gived.ContainsKey(playerid))
                        {
                            Globals.Gived.Add(playerid, true);
                        }

                        foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(weapon))
                        {
                            if (entity.DesignerName != weapon) continue;
                            if (entity == null) continue;
                            if (entity.Entity == null) continue;
                            if (entity.OwnerEntity == null) continue;
                            if(entity.OwnerEntity.IsValid) continue;

                            entity.AcceptInput("Kill");
                        }
                    }
                }
                
            }else if (player.TeamNum == (byte)CsTeam.Terrorist)
            {
                string[] weaponsT;
                weaponsT = TeamWeapon["T"]?.ToString()?.Split(',') ?? new string[0];
                foreach (string weapon in weaponsT)
                {
                    if (string.IsNullOrWhiteSpace(weapon))continue;
                    if(Configs.GetConfigData().GiveOneTimeLoadOutPerRound && Globals.Gived.ContainsKey(playerid))continue;

                    CustomFunctions!.PlayerGiveNamedItem(player, weapon);

                    if (Configs.GetConfigData().GiveOneTimeLoadOutPerRound && !Globals.Gived.ContainsKey(playerid))
                    {
                        Globals.Gived.Add(playerid, true);
                    }
                    
                    foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(weapon))
                    {
                        if (entity.DesignerName != weapon) continue;
                        if (entity == null) continue;
                        if (entity.Entity == null) continue;
                        if (entity.OwnerEntity == null) continue;
                        if(entity.OwnerEntity.IsValid) continue;

                        entity.AcceptInput("Kill");
                    }
                    
                }
                

                if(Globals.VipsFlag.ContainsKey(playerid))
                {
                    string[] weaponsTVip;
                    weaponsTVip = TeamWeapon["T_Vip"]?.ToString()?.Split(',') ?? new string[0];
                    foreach (string weapon in weaponsTVip)
                    {
                        if (string.IsNullOrWhiteSpace(weapon))continue;
                        if(Configs.GetConfigData().Vips_GiveOneTimeLoadOutPerRound && Globals.Gived.ContainsKey(playerid))continue;

                        CustomFunctions!.PlayerGiveNamedItem(player, weapon);

                        if (Configs.GetConfigData().Vips_GiveOneTimeLoadOutPerRound && !Globals.Gived.ContainsKey(playerid))
                        {
                            Globals.Gived.Add(playerid, true);
                        }

                        foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(weapon))
                        {
                            if (entity.DesignerName != weapon) continue;
                            if (entity == null) continue;
                            if (entity.Entity == null) continue;
                            if (entity.OwnerEntity == null) continue;
                            if(entity.OwnerEntity.IsValid) continue;

                            entity.AcceptInput("Kill");
                        }
                    }
                }
            }
            
        }

        return HookResult.Continue;
    }

    private void CreateDefaultWeaponsJson()
    {
        var FolderConfig = Path.Combine(ModuleDirectory, "../../plugins/Spawn-Loadout-GoldKingZ/config/");
        var configcfg = Path.Combine(FolderConfig, "weapons.json");

        if (!Directory.Exists(FolderConfig) || !File.Exists(configcfg))
        {
            if (!Directory.Exists(FolderConfig))
            {
                Directory.CreateDirectory(FolderConfig);
            }

            var weaponsData = new 
            {
                ANY = new
                {
                    CT = "weapon_hkp2000,weapon_knife,weapon_smokegrenade",
                    CT_Refill_Nades = "",
                    CT_Refill_Time_InSec = 30,
                    CT_Vip = "weapon_taser,weapon_smokegrenade,weapon_decoy",
                    CT_Vip_Refill_Nades = "weapon_decoy",
                    CT_Vip_Refill_Time_InSec = 30,
                    T = "weapon_hkp2000,weapon_knife,weapon_smokegrenade",
                    T_Refill_Nades = "",
                    T_Refill_Time_InSec = 30,
                    T_Vip = "weapon_taser,weapon_smokegrenade,weapon_decoy",
                    T_Vip_Refill_Nades = "weapon_decoy",
                    T_Vip_Refill_Time_InSec = 30
                },
                de_ = new
                {
                    CT = "weapon_ssg08,weapon_deagle",
                    T = "weapon_ssg08,weapon_deagle"
                },
                awp_lego_2 = new
                {
                    CT = "weapon_awp,weapon_deagle",
                    CT_Vip = "weapon_taser",
                    T = "weapon_awp,weapon_deagle"
                }
            };


            var jsonContent = JsonConvert.SerializeObject(weaponsData, Formatting.Indented);

            File.WriteAllText(configcfg, jsonContent);
        }
    }


    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;

        var player = @event.Userid;

        if (player == null || !player.IsValid)return HookResult.Continue;

        Globals.VipsFlag.Remove(player.SteamID);
        Globals.Gived.Remove(player.SteamID);
        Globals.NadeGived.Remove(player.SteamID);
        Globals.VipGived.Remove(player.SteamID);
        Globals.VipNadeGived.Remove(player.SteamID);

        return HookResult.Continue;
    }
    private HookResult OnEventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        Globals.Gived.Clear();
        Globals.NadeGived.Clear();
        Globals.VipGived.Clear();
        Globals.VipNadeGived.Clear();

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