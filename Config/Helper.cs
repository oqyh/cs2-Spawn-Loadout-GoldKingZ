using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using Newtonsoft.Json;
using Spawn_Loadout_GoldKingZ.Config;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace Spawn_Loadout_GoldKingZ;

public class Helper
{
    public static readonly string[] WeaponsList =
    {
        "weapon_ak47", "weapon_aug", "weapon_awp", "weapon_bizon", "weapon_cz75a", "weapon_deagle", "weapon_elite", "weapon_famas", "weapon_fiveseven", "weapon_g3sg1", "weapon_galilar",
        "weapon_glock", "weapon_hkp2000", "weapon_m249", "weapon_m4a1", "weapon_m4a1_silencer", "weapon_mac10", "weapon_mag7", "weapon_mp5sd", "weapon_mp7", "weapon_mp9", "weapon_negev",
        "weapon_nova", "weapon_p250", "weapon_p90", "weapon_revolver", "weapon_sawedoff", "weapon_scar20", "weapon_sg556", "weapon_ssg08", "weapon_tec9", "weapon_ump45", "weapon_usp_silencer", "weapon_xm1014",
        "weapon_decoy", "weapon_flashbang", "weapon_hegrenade", "weapon_incgrenade", "weapon_molotov", "weapon_smokegrenade","item_defuser", "item_cutters", "weapon_knife"
    };

    public static void AdvancedPlayerPrintToChat(CCSPlayerController? player, string message, params object[] args)
    {
        if (player == null || !player.IsValid) return;
        if (string.IsNullOrEmpty(message))return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                player.PrintToChat(" " + messages);
            }
        }else
        {
            player.PrintToChat(message);
        }
    }
    public static void Localized_PlayerPrintToChat(CCSPlayerController? player, string lang, string sublang, params object[] args)
    {
        if (player == null || !player.IsValid) return;
        var localizedValue = Configs.Shared.StringLocalizer![$"{lang}.{sublang}"];
        
        if (!string.IsNullOrEmpty(localizedValue) && localizedValue != $"{lang}.{sublang}")
        {
            AdvancedPlayerPrintToChat(player, Configs.Shared.StringLocalizer![$"{lang}.{sublang}"],args);
        }
        else
        {
            AdvancedPlayerPrintToChat(player, Configs.Shared.StringLocalizer![$"{lang}"],args);
        }
    }

    public static void AdvancedServerPrintToChatAll(string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message))return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                Server.PrintToChatAll(" " + messages);
            }
        }else
        {
            Server.PrintToChatAll(message);
        }
    }


    public static bool IsPlayerInGroupPermission(CCSPlayerController? player, string groups)
    {
        var excludedGroups = groups.Split(',');
        foreach (var group in excludedGroups)
        {
            switch (group[0])
            {
                case '#':
                    if (AdminManager.PlayerInGroup(player, group))
                        return true;
                    break;

                case '@':
                    if (AdminManager.PlayerHasPermissions(player, group))
                        return true;
                    break;

                default:
                    return false;
            }
        }
        return false;
    }

    public static void CreateGameData(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            var configData = new Dictionary<string, object>
            {
                ["GiveNamedItem2"] = new Dictionary<string, object>
                {
                    ["signatures"] = new Dictionary<string, string>
                    {
                        ["library"] = "server",
                        ["windows"] = "48 83 EC ? 48 C7 44 24 ? ? ? ? ? 45 33 C9 45 33 C0 C6 44 24 ? ? E8 ? ? ? ? 48 85 C0",
                        ["linux"] = "55 48 89 E5 41 57 41 56 41 55 41 54 53 48 83 EC ? 48 89 7D ? 48 85 F6 74"
                    }
                }
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = System.Text.Json.JsonSerializer.Serialize(configData, options);
            File.WriteAllText(jsonFilePath, json);
        }
    }
    public static void CreateWeaponsJson(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            var configData = new Dictionary<string, object>
            {
                ["ANY"] = new Dictionary<string, object>
                {
                    ["Remove_Ground_Weapons"] = 1,
                    ["LOADOUT_1"] = new Dictionary<string, object>
                    {
                        ["Flags"] = "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",
                        ["CT"] = "weapon_taser,weapon_decoy",
                        ["T"] = "weapon_taser,weapon_decoy",
                        ["CT_Refill_Nades"] = "weapon_decoy",
                        ["CT_Refill_Time_InSec"] = 30,
                        ["T_Refill_Nades"] = "weapon_decoy",
                        ["T_Refill_Time_InSec"] = 30
                    },
                    ["LOADOUT_2"] = new Dictionary<string, object>
                    {
                        ["Give_This_LoadOut_PerRound_Only"] = 1,
                        ["CT"] = "weapon_hkp2000,weapon_knife,weapon_smokegrenade",
                        ["T"] = "weapon_glock,weapon_knife,weapon_smokegrenade"
                    }
                },
                ["hns_"] = new Dictionary<string, object>
                {
                    ["LOADOUT_1"] = new Dictionary<string, object>
                    {
                        ["FLAGS"] = "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",
                        ["CT_Refill_Nades"] = "weapon_decoy",
                        ["CT_Refill_Time_InSec"] = 30,
                        ["T_Refill_Nades"] = "weapon_decoy",
                        ["T_Refill_Time_InSec"] = 30
                    },
                    ["LOADOUT_2"] = new Dictionary<string, object>
                    {
                        ["Give_This_LoadOut_PerRound_Only"] = 2,
                        ["CT"] = "weapon_decoy",
                        ["T"] = "weapon_decoy"
                    }
                },
                ["awp_lego_2"] = new Dictionary<string, object>
                {
                    ["Force_Strip_Players"] = true,
                    ["Remove_Knife"] = true,
                    ["Remove_BuyMenu"] = true,
                    ["Remove_Custom_Point_Server_Command"] = true,
                    ["Remove_Custom_Point_Client_Command"] = true,
                    ["Remove_Ground_Weapons"] = 1,
                    ["Delay_InXSecs_Give_LoadOuts"] = 1.0,
                    ["LOADOUT_1"] = new Dictionary<string, object>
                    {
                        ["CT"] = "weapon_ssg08",
                        ["T"] = "weapon_ssg08"
                    }
                }
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = System.Text.Json.JsonSerializer.Serialize(configData, options);
            File.WriteAllText(jsonFilePath, json);
        }
    }

    public static void ClearVariables()
    {
        foreach (var players in SpawnLoadoutGoldKingZ.Instance.g_Main.PlayerCleanUp.Values)
        {
            players.Timer?.Kill();
            players.Timer = null!;

            players.KillTheTimer = null!;
            players.KillTheTimer = null!;
        }
        SpawnLoadoutGoldKingZ.Instance.g_Main.PlayerCleanUp.Clear();
        SpawnLoadoutGoldKingZ.Instance.g_Main.loadoutsGivenPerPlayer.Clear();
        SpawnLoadoutGoldKingZ.Instance.g_Main.ClearJsonValues();

        SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveBuyMenu = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveKnife = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.ForceRemoveServerCommands = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.ForceRemoveClientCommands = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.ForceStripPlayers = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.ForceRemoveGroundWeapons = 0;
        SpawnLoadoutGoldKingZ.Instance.g_Main.DelayGiveLoadOut = 0.0f;
        Server.ExecuteCommand("sv_buy_status_override -1");
    }

    public static void GiveWeaponsToPlayer(CCSPlayerController player, string weapons)
    {
        string[] weaponList = weapons.Split(',');
        foreach (var weapon in weaponList)
        {
            if (player == null || !player.IsValid) return;

            if (!player.PawnIsAlive) return;

            var playerpawn = player.PlayerPawn;
            if (playerpawn == null || !playerpawn.IsValid) return;

            var playerpawnvalue = playerpawn.Value;
            if (playerpawnvalue == null || !playerpawnvalue.IsValid) return;

            var playerWeaponServices = playerpawnvalue.WeaponServices;
            if (playerWeaponServices == null) return;

            var playerMyWeapons = playerWeaponServices.MyWeapons;
            if (playerMyWeapons == null) return;

            bool hasWeapon = false;
            
            foreach (var weaponInventory in playerMyWeapons)
            {
                if (weaponInventory == null || !weaponInventory.IsValid) continue;
                
                var weaponInventoryValue = weaponInventory.Value;
                if (weaponInventoryValue == null || !weaponInventoryValue.IsValid) continue;
                
                var weaponDesignerName = weaponInventoryValue.DesignerName;
                if (weaponDesignerName == null || string.IsNullOrEmpty(weaponDesignerName)) continue;
                
                if (weaponDesignerName == weapon)
                {
                    hasWeapon = true;
                    break;
                }
            }

            if (!hasWeapon)
            {
                Configs.Shared.CustomFunctions!.PlayerGiveNamedItem(player, weapon);
            }
        }
    }


    public static void RemoveWeaponByName(CCSPlayerController? player, string weaponName)
    {
        Server.NextFrame(() =>
        {
            if (player == null || !player.IsValid) return;
            
            var playerpawn = player.PlayerPawn;
            if (playerpawn == null || !playerpawn.IsValid) return;

            var playerpawnvalue = playerpawn.Value;
            if (playerpawnvalue == null || !playerpawnvalue.IsValid) return;

            var playerWeaponServices = playerpawnvalue.WeaponServices;
            if (playerWeaponServices == null) return;

            var playerMyWeapons = playerWeaponServices.MyWeapons;
            if (playerMyWeapons == null) return;
            
            var WeaponInvetory = playerMyWeapons
                .Where(x => x != null && x.IsValid && x.Value != null && x.Value.IsValid && x.Value.DesignerName == weaponName).FirstOrDefault();

            if(WeaponInvetory == null || !WeaponInvetory.IsValid)return;
            
            var playerActiveWeapon = playerWeaponServices.ActiveWeapon;
            if(playerActiveWeapon == null || !playerActiveWeapon.IsValid || playerActiveWeapon.Value == null || !playerActiveWeapon.Value.IsValid)return;
            
            playerActiveWeapon.Raw = WeaponInvetory.Raw;
            
            var activeWeaponEntity = playerActiveWeapon.Value.As<CBaseEntity>();
            
            player.DropActiveWeapon();
            
            activeWeaponEntity.AddEntityIOEvent("Kill", activeWeaponEntity, null, "", 0.5f);
        });
        
    }
    
    public static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch (Exception ex)
        {
            DebugMessage(ex.Message);
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }
    public static void DebugMessage(string message)
    {
        if(!Configs.GetConfigData().EnableDebug)return;
        Console.WriteLine($"================================= [ Debug Spawn Loadout ] =================================");
        Console.WriteLine(message);
        Console.WriteLine("=========================================================================================");
    }

    
    public static void ClearGroundWeapons()
    {
        foreach (string Weapons in WeaponsList)
        {
            foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(Weapons))
            {
                if (entity == null) continue;
                if (entity.Entity == null) continue;
                if (entity.OwnerEntity == null) continue;
                if(entity.OwnerEntity.IsValid) continue;

                entity.AcceptInput("Kill");
            }
        }
    }
    public static void ClearMapCommands(bool ServerCommands, bool ClientCommands)
    {
        if(ServerCommands)
        {
            var pointservercommand = Utilities.FindAllEntitiesByDesignerName<CPointServerCommand>("point_servercommand");
            foreach (var ent in pointservercommand)
            {
                if (ent == null || !ent.IsValid)continue;
                ent.Remove();
            }

            Server.ExecuteCommand("mp_ct_default_primary \"\"");
            Server.ExecuteCommand("mp_ct_default_secondary \"\"");
            Server.ExecuteCommand("mp_ct_default_grenades \"\"");
            
            Server.ExecuteCommand("mp_t_default_primary \"\"");
            Server.ExecuteCommand("mp_t_default_grenades \"\"");
            Server.ExecuteCommand("mp_t_default_secondary \"\"");
        }

        if(ClientCommands)
        {
            var pointclientcommand = Utilities.FindAllEntitiesByDesignerName<CPointClientCommand>("point_clientcommand");
            foreach (var ent in pointclientcommand)
            {
                if (ent == null || !ent.IsValid)continue;
                ent.Remove();
            }

            Server.ExecuteCommand("mp_ct_default_primary \"\"");
            Server.ExecuteCommand("mp_ct_default_secondary \"\"");
            Server.ExecuteCommand("mp_ct_default_grenades \"\"");
            
            Server.ExecuteCommand("mp_t_default_primary \"\"");
            Server.ExecuteCommand("mp_t_default_grenades \"\"");
            Server.ExecuteCommand("mp_t_default_secondary \"\"");
        }
    }
    public static void SetValuesToGlobals()
    {
        SpawnLoadoutGoldKingZ.Instance.g_Main.ClearJsonValues();
        var jsonValues = SpawnLoadoutGoldKingZ.Instance.g_Main.GetJsonValues();
        if (jsonValues == null || jsonValues.Count == 0)return;

        if (jsonValues.ContainsKey("Force_Strip_Players"))
        {
            bool RemoveBuyMenu = jsonValues["Force_Strip_Players"] != null 
                                    ? (bool)jsonValues["Force_Strip_Players"] 
                                    : false;
            if (RemoveBuyMenu)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.ForceStripPlayers = true; 
            }
        }

        if (jsonValues.ContainsKey("Remove_BuyMenu"))
        {
            bool RemoveBuyMenu = jsonValues["Remove_BuyMenu"] != null 
                                    ? (bool)jsonValues["Remove_BuyMenu"] 
                                    : false;
            if (RemoveBuyMenu)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveBuyMenu = true; 
            }
        }
        if (jsonValues.ContainsKey("Remove_Knife"))
        {
            bool RemoveKnife = jsonValues["Remove_Knife"] != null 
                                    ? (bool)jsonValues["Remove_Knife"] 
                                    : false;
            if (RemoveKnife)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveKnife = true; 
            }
        }

        if (jsonValues.ContainsKey("Remove_Custom_Point_Server_Command"))
        {
            bool RemoveServerCommands = jsonValues["Remove_Custom_Point_Server_Command"] != null 
                                    ? (bool)jsonValues["Remove_Custom_Point_Server_Command"] 
                                    : false;
            if (RemoveServerCommands)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.ForceRemoveServerCommands = true; 
            }
        }

        if (jsonValues.ContainsKey("Remove_Custom_Point_Client_Command"))
        {
            bool RemoveClientCommands = jsonValues["Remove_Custom_Point_Client_Command"] != null 
                                    ? (bool)jsonValues["Remove_Custom_Point_Client_Command"] 
                                    : false;
            if (RemoveClientCommands)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.ForceRemoveClientCommands = true; 
            }
        }

        if (jsonValues.ContainsKey("Remove_Ground_Weapons"))
        {
            int RemoveGroundWeapons = jsonValues["Remove_Ground_Weapons"] != null 
                                    ? (int)jsonValues["Remove_Ground_Weapons"] 
                                    : 0;
            if (RemoveGroundWeapons != 0)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.ForceRemoveGroundWeapons = RemoveGroundWeapons; 
            }
        }

        if (jsonValues.ContainsKey("Delay_InXSecs_Give_LoadOuts"))
        {
            float DelayGiveLoadout = jsonValues["Delay_InXSecs_Give_LoadOuts"] != null 
                                    ? (float)jsonValues["Delay_InXSecs_Give_LoadOuts"] 
                                    : 0.0f;
            if (DelayGiveLoadout > 0.0f)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.DelayGiveLoadOut = DelayGiveLoadout; 
            }
        }

        if(SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveBuyMenu)
        {
            Server.ExecuteCommand("sv_buy_status_override 3");
        }

        bool GetBoolServerCommands = SpawnLoadoutGoldKingZ.Instance.g_Main.ForceRemoveServerCommands? true : false;
        bool GetBoolClientCommands = SpawnLoadoutGoldKingZ.Instance.g_Main.ForceRemoveClientCommands? true : false;
        ClearMapCommands(GetBoolServerCommands,GetBoolClientCommands);
    }
    public static void CleanUpPlayerWeapons(CCSPlayerController? player)
    {
        if (player == null || !player.IsValid ||  !SpawnLoadoutGoldKingZ.Instance.g_Main.PlayerCleanUp.ContainsKey(player)) return;

        var playerpawn = player.PlayerPawn;
        if (playerpawn == null || !playerpawn.IsValid) return;

        var playerpawnvalue = playerpawn.Value;
        if (playerpawnvalue == null || !playerpawnvalue.IsValid) return;

        var playerWeaponServices = playerpawnvalue.WeaponServices;
        if (playerWeaponServices == null) return;

        var playerMyWeapons = playerWeaponServices.MyWeapons;
        if (playerMyWeapons == null) return;

        foreach (var playerMyWeaponsCt in playerMyWeapons)
        {
            var weaponsValue = playerMyWeaponsCt.Value;

            if (weaponsValue == null || !weaponsValue.IsValid)
                continue;

            var weaponsDesignerName = weaponsValue.DesignerName;

            if (string.IsNullOrWhiteSpace(weaponsDesignerName))
                continue;

            if (weaponsDesignerName.Contains("weapon_knife") && !SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveKnife || 
                weaponsDesignerName.Contains("weapon_bayonet") && !SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveKnife || 
                weaponsDesignerName.Contains("weapon_c4"))
                continue;

            if (!SpawnLoadoutGoldKingZ.Instance.g_Main.PlayerCleanUp[player].WeaponsNeeded.Contains(weaponsDesignerName))
            {
                RemoveWeaponByName(player, weaponsDesignerName);
            }
        }
    }
    
}