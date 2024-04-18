using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using Newtonsoft.Json;
using CounterStrikeSharp.API.Modules.Utils;
using Spawn_Loadout_GoldKingZ.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Memory;

namespace Spawn_Loadout_GoldKingZ;

public class SpawnLoadoutGoldKingZ : BasePlugin
{
    public override string ModuleName => "Give Weapons On Spawn (Depend The Map Name + Team Side)";
    public override string ModuleVersion => "1.0.0";
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
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
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

        string jsonFilePath = $"{ModuleDirectory}/../../plugins/Spawn-Loadout-GoldKingZ/Weapons/Weapons.json";

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
                    if(Configs.GetConfigData().GiveOneTimePerRound && Globals.Gived.ContainsKey(playerid))continue;

                    CustomFunctions!.PlayerGiveNamedItem(player, weapon);

                    if (Configs.GetConfigData().GiveOneTimePerRound && !Globals.Gived.ContainsKey(playerid))
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
                        if(Configs.GetConfigData().GiveOneTimePerRound && Globals.Gived.ContainsKey(playerid))continue;

                        CustomFunctions!.PlayerGiveNamedItem(player, weapon);

                        if (Configs.GetConfigData().GiveOneTimePerRound && !Globals.Gived.ContainsKey(playerid))
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
                    if(Configs.GetConfigData().GiveOneTimePerRound && Globals.Gived.ContainsKey(playerid))continue;

                    CustomFunctions!.PlayerGiveNamedItem(player, weapon);

                    if (Configs.GetConfigData().GiveOneTimePerRound && !Globals.Gived.ContainsKey(playerid))
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
                        if(Configs.GetConfigData().GiveOneTimePerRound && Globals.Gived.ContainsKey(playerid))continue;

                        CustomFunctions!.PlayerGiveNamedItem(player, weapon);

                        if (Configs.GetConfigData().GiveOneTimePerRound && !Globals.Gived.ContainsKey(playerid))
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
        var FolderConfig = Path.Combine(ModuleDirectory, "../../plugins/Spawn-Loadout-GoldKingZ/Weapons/");
        var configcfg = Path.Combine(FolderConfig, "Weapons.json");

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
                    CT = "weapon_hkp2000,weapon_knife",
                    CT_Vip = "weapon_taser,weapon_smokegrenade",
                    T = "weapon_glock,weapon_knife",
                    T_Vip = "weapon_taser,weapon_smokegrenade",
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

        Globals.Gived.Remove(player.SteamID);
        Globals.VipsFlag.Remove(player.SteamID);

        return HookResult.Continue;
    }
    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if(!Configs.GetConfigData().GiveOneTimePerRound || @event == null)return HookResult.Continue;

        Globals.Gived.Clear();

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