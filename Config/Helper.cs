using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using Newtonsoft.Json;
using Spawn_Loadout_GoldKingZ.Config;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json.Linq;

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

    public static readonly string[] primaryWeapons = {
        "weapon_ak47", "weapon_aug", "weapon_awp", "weapon_famas", "weapon_g3sg1", 
        "weapon_galilar", "weapon_m249", "weapon_m4a1", "weapon_m4a1_silencer", 
        "weapon_mag7", "weapon_mp5sd", "weapon_mp7", "weapon_mp9", "weapon_negev", 
        "weapon_nova", "weapon_p90", "weapon_sawedoff", "weapon_scar20", "weapon_sg556", 
        "weapon_ssg08", "weapon_mac10", "weapon_ump45", "weapon_xm1014", "weapon_bizon"
    };

    public static readonly string[] secondaryWeapons = {
        "weapon_cz75a", "weapon_deagle", "weapon_elite", "weapon_fiveseven", "weapon_glock", 
        "weapon_hkp2000", "weapon_p250", "weapon_revolver", "weapon_tec9", "weapon_usp_silencer"
    };

    public static readonly string[] grenades = {
    "weapon_decoy", "weapon_flashbang", "weapon_hegrenade",
    "weapon_incgrenade", "weapon_molotov", "weapon_smokegrenade"
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


    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        var excludedGroups = groups.Split(',');
        foreach (var group in excludedGroups)
        {
            switch (group[0])
            {
                case '#':
                    if (AdminManager.PlayerInGroup(player, group))
                    {
                        return true;
                    }
                    break;

                case '@':
                    if (AdminManager.PlayerHasPermissions(player, group))
                    {
                        return true;
                    }
                    break;

                default:
                    if (AdminManager.PlayerInGroup(player, group))
                    {
                        return true;
                    }
                    break;
            }
        }
        return false;
    }

    public static void ClearVariables()
    {
        SpawnLoadoutGoldKingZ.Instance.g_Main.loadoutsGivenPerPlayer.Clear();
        SpawnLoadoutGoldKingZ.Instance.g_Main.JsonData?.Clear();

        SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveBuyMenu = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveKnifeCT = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveKnifeT = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.ForceRemoveServerCommands = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.ForceRemoveClientCommands = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.ForceStripCTPlayers = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.ForceStripTPlayers = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.ForceRemoveGroundWeapons = false;
        SpawnLoadoutGoldKingZ.Instance.g_Main.DelayGiveLoadOut = 0.0f;
        SpawnLoadoutGoldKingZ.Instance.g_Main.GiveHealthCT = -1;
        SpawnLoadoutGoldKingZ.Instance.g_Main.GiveHealthT = -1;
        SpawnLoadoutGoldKingZ.Instance.g_Main.GiveArmorCT = -1;
        SpawnLoadoutGoldKingZ.Instance.g_Main.GiveArmorT = -1;

        Server.ExecuteCommand("sv_buy_status_override -1");
    }

    public static void SetValues(CCSPlayerController player = null!)
    {
        try
        {
            string jsonFilePath = $"{Configs.Shared.CookiesModule}/../../plugins/Spawn-Loadout-GoldKingZ/config/Weapons_Settings.json";

            if (!File.Exists(jsonFilePath))
            {
                if (player != null && player.IsValid)
                    player.PrintToChat(" \x06[Spawn-Loadout] \x02 JSON file does not exist. Weapons_Settings.json In config Folder");
                DebugMessage($"JSON file does not exist. Weapons_Settings.json In config Folder");
                return;
            }

            string jsonContent = File.ReadAllText(jsonFilePath);

            if (string.IsNullOrEmpty(jsonContent))
            {
                if (player != null && player.IsValid)
                    player.PrintToChat(" \x06[Spawn-Loadout] \x02 JSON content is empty.");
                DebugMessage("JSON content is empty.");
                return;
            }

            dynamic jsonData = JsonConvert.DeserializeObject(jsonContent)!;

            if (jsonData == null)
            {
                DebugMessage("Failed to deserialize JSON content.");
                return;
            }

            string mapname = NativeAPI.GetMapName();
            if (string.IsNullOrEmpty(mapname))
            {
                if (player != null && player.IsValid)
                    player.PrintToChat(" \x06[Spawn-Loadout] \x02 Map name is empty.");
                DebugMessage("Map Name Is Empty.");
                return;
            }

            int underscoreIndex = mapname.IndexOf('_');
            int nextUnderscoreIndex = mapname.IndexOf('_', underscoreIndex + 1);

            string prefix = underscoreIndex != -1 ? mapname.Substring(0, underscoreIndex + 1) : mapname;
            string prefix2 = underscoreIndex != -1 && nextUnderscoreIndex != -1 ? mapname.Substring(0, nextUnderscoreIndex + 1) : mapname;

            dynamic selectedMap = null!;

            if (jsonData.ContainsKey("ANY"))
            {
                selectedMap = jsonData["ANY"];
            }
            else if (jsonData.ContainsKey(prefix))
            {
                selectedMap = jsonData[prefix];
            }
            else if (jsonData.ContainsKey(prefix2))
            {
                selectedMap = jsonData[prefix2];
            }
            else if (jsonData.ContainsKey(mapname))
            {
                selectedMap = jsonData[mapname];
            }

            if (selectedMap != null)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.JsonData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(selectedMap.ToString());
                SetValuesToGlobals();
                if (player != null && player.IsValid)
                    player.PrintToChat(" \x06[Spawn-Loadout] \x03 Weapons_Settings.json Loaded Successfully");
            }
            else
            {
                if (player != null && player.IsValid)
                    player.PrintToChat(" \x06[Spawn-Loadout] \x02 No matching map found in Weapons_Settings.json.");
                DebugMessage("No matching map found in Weapons_Settings.json.");
            }
        }
        catch (JsonReaderException ex)
        {
            if (player != null && player.IsValid)
                player.PrintToChat(" \x06[Spawn-Loadout] \x02 Error On Weapons_Settings.json " + ex.Message);
            DebugMessage($"Error On Weapons_Settings.json {ex.Message}");
        }
    }

    public static void GiveWeaponsToPlayer(CCSPlayerController player, string weapons, bool force = false)
    {
        if (string.IsNullOrEmpty(weapons)) return;

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

        List<string> totallWeaponsList = new List<string>();
        foreach (var weaponInventory in playerMyWeapons)
        {
            if (weaponInventory == null || !weaponInventory.IsValid) continue;

            var weaponInventoryValue = weaponInventory.Value;
            if (weaponInventoryValue == null || !weaponInventoryValue.IsValid) continue;

            var weaponDesignerName = weaponInventoryValue.DesignerName;
            if (weaponDesignerName == null || string.IsNullOrEmpty(weaponDesignerName)) continue;

            totallWeaponsList.Add(weaponDesignerName);
        }

        bool hasPrimaryWeapon = totallWeaponsList.Any(weapon => primaryWeapons.Contains(weapon));
        bool hasSecondaryWeapon = totallWeaponsList.Any(weapon => secondaryWeapons.Contains(weapon));

        string[] weaponList = weapons.Split(',');
        foreach (var weaponz in weaponList)
        {
            if (totallWeaponsList.Contains(weaponz))continue;
            if (!force && hasPrimaryWeapon && primaryWeapons.Contains(weaponz))continue;
            if (!force && hasSecondaryWeapon && secondaryWeapons.Contains(weaponz))continue;

            if (force)
            {
                if (primaryWeapons.Contains(weaponz))
                {
                    var existingPrimaryWeapons = totallWeaponsList.Where(w => primaryWeapons.Contains(w)).ToList();
                    foreach (var existingPrimaryWeapon in existingPrimaryWeapons)
                    {
                        RemoveWeaponByName(player, existingPrimaryWeapon);
                        totallWeaponsList.Remove(existingPrimaryWeapon);
                    }
                }

                if (secondaryWeapons.Contains(weaponz))
                {
                    var existingSecondaryWeapons = totallWeaponsList.Where(w => secondaryWeapons.Contains(w)).ToList();
                    foreach (var existingSecondaryWeapon in existingSecondaryWeapons)
                    {
                        RemoveWeaponByName(player, existingSecondaryWeapon);
                        totallWeaponsList.Remove(existingSecondaryWeapon);
                    }
                }
            }

            Configs.Shared.CustomFunctions!.PlayerGiveNamedItem(player, weaponz);
            totallWeaponsList.Add(weapons);
        }
    }

    public static void ForceStripePlayer(CCSPlayerController player, bool stripWeapons_CT = false, bool stripWeapons_T = false, bool stripKnife_CT = false, bool stripKnife_T = false)
    {
        if (player == null || !player.IsValid || !player.PawnIsAlive) return;

        var playerPawn = player.PlayerPawn;
        if (playerPawn == null || !playerPawn.IsValid) return;

        var playerPawnValue = playerPawn.Value;
        if (playerPawnValue == null || !playerPawnValue.IsValid) return;

        var playerWeaponServices = playerPawnValue.WeaponServices;
        if (playerWeaponServices == null) return;

        var playerMyWeapons = playerWeaponServices.MyWeapons;
        if (playerMyWeapons == null) return;

        bool stripWeapons = false;
        bool stripKnife = false;

        if (stripWeapons_CT && player.TeamNum == (byte)CsTeam.CounterTerrorist)
            stripWeapons = true;
        else if (stripWeapons_T && player.TeamNum == (byte)CsTeam.Terrorist)
            stripWeapons = true;
        else if (SpawnLoadoutGoldKingZ.Instance.g_Main.ForceStripCTPlayers && player.TeamNum == (byte)CsTeam.CounterTerrorist)
            stripWeapons = true;
        else if (SpawnLoadoutGoldKingZ.Instance.g_Main.ForceStripTPlayers && player.TeamNum == (byte)CsTeam.Terrorist)
            stripWeapons = true;


        if (stripKnife_CT && player.TeamNum == (byte)CsTeam.CounterTerrorist)
            stripKnife = true;
        else if (stripKnife_T && player.TeamNum == (byte)CsTeam.Terrorist)
            stripKnife = true;
        else if (SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveKnifeCT && player.TeamNum == (byte)CsTeam.CounterTerrorist)
            stripKnife = true;
        else if (SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveKnifeT && player.TeamNum == (byte)CsTeam.Terrorist)
            stripKnife = true;

        if (stripWeapons)
        {
            foreach (var weaponInventory in playerMyWeapons)
            {
                if (weaponInventory == null || !weaponInventory.IsValid) continue;

                var weaponInventoryValue = weaponInventory.Value;
                if (weaponInventoryValue == null || !weaponInventoryValue.IsValid) continue;

                var weaponDesignerName = weaponInventoryValue.DesignerName;
                if (weaponDesignerName == null || string.IsNullOrEmpty(weaponDesignerName)) continue;

                if (weaponDesignerName.Contains("weapon_knife") && !stripKnife || 
                    weaponDesignerName.Contains("weapon_bayonet") && !stripKnife ||
                    weaponDesignerName.Contains("weapon_c4"))
                {
                    continue;
                }

                RemoveWeaponByName(player, weaponDesignerName);
            }
        }
    }

    public static void RemoveWeaponByName(CCSPlayerController? player, string weaponName)
    {
        if (string.IsNullOrEmpty(weaponName)) return;

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
        Server.NextFrame(() =>
        {
            if(activeWeaponEntity != null && activeWeaponEntity.IsValid)
            {
                activeWeaponEntity.AddEntityIOEvent("Kill", activeWeaponEntity, null, "", 0.3f);
            }
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

    public static int GetCurrentRound()
    {
        var gameRules = GetGameRules();
        if (gameRules == null)
        {
            return -1;
        }

        int rounds = IsWarmup() ? gameRules.TotalRoundsPlayed : gameRules.TotalRoundsPlayed + 1;

        return rounds;
    }

    public static void DebugMessage(string message)
    {
        if (!Configs.GetConfigData().EnableDebug) return;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"[Debug Spawn Loadout]: " + message);
        Console.ResetColor();
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
        var jsonValues = SpawnLoadoutGoldKingZ.Instance.g_Main.JsonData;
        if (jsonValues == null || jsonValues.Count == 0)return;

        if (jsonValues.ContainsKey("Force_Strip_CT_Players"))
        {
            bool Force_Strip_CT_Players = jsonValues["Force_Strip_CT_Players"] != null 
                                    ? (bool)jsonValues["Force_Strip_CT_Players"] 
                                    : false;
            if (Force_Strip_CT_Players)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.ForceStripCTPlayers = true; 
            }
        }

        if (jsonValues.ContainsKey("Force_Strip_T_Players"))
        {
            bool Force_Strip_T_Players = jsonValues["Force_Strip_T_Players"] != null 
                                    ? (bool)jsonValues["Force_Strip_T_Players"] 
                                    : false;
            if (Force_Strip_T_Players)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.ForceStripTPlayers = true; 
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

        if (jsonValues.ContainsKey("Players_Health_CT"))
        {
            int Players_Health_CT = jsonValues["Players_Health_CT"] != null 
                                    ? (int)jsonValues["Players_Health_CT"] 
                                    : -1;
            if (Players_Health_CT > -1)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.GiveHealthCT = Players_Health_CT; 
            }
        }
        if (jsonValues.ContainsKey("Players_Health_T"))
        {
            int Players_Health_T = jsonValues["Players_Health_T"] != null 
                                    ? (int)jsonValues["Players_Health_T"] 
                                    : -1;
            if (Players_Health_T > -1)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.GiveHealthT = Players_Health_T; 
            }
        }

        if (jsonValues.ContainsKey("Players_Armor_CT"))
        {
            int Players_Armor_CT = jsonValues["Players_Armor_CT"] != null 
                                    ? (int)jsonValues["Players_Armor_CT"] 
                                    : -1;
            if (Players_Armor_CT > -1)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.GiveArmorCT = Players_Armor_CT; 
            }
        }

        if (jsonValues.ContainsKey("Players_Armor_T"))
        {
            int Players_Armor_T = jsonValues["Players_Armor_T"] != null 
                                    ? (int)jsonValues["Players_Armor_T"] 
                                    : -1;
            if (Players_Armor_T > -1)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.GiveArmorT = Players_Armor_T; 
            }
        }

        if (jsonValues.ContainsKey("Remove_Knife_CT"))
        {
            bool Remove_Knife_CT = jsonValues["Remove_Knife_CT"] != null 
                                    ? (bool)jsonValues["Remove_Knife_CT"] 
                                    : false;
            if (Remove_Knife_CT)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveKnifeCT = true; 
            }
        }

        if (jsonValues.ContainsKey("Remove_Knife_T"))
        {
            bool Remove_Knife_T = jsonValues["Remove_Knife_T"] != null 
                                    ? (bool)jsonValues["Remove_Knife_T"] 
                                    : false;
            if (Remove_Knife_T)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.RemoveKnifeT = true; 
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

        if (jsonValues.ContainsKey("Remove_Ground_Weapons_After_Give_Loadouts"))
        {
            bool Remove_Ground_Weapons_After_Give_Loadouts = jsonValues["Remove_Ground_Weapons_After_Give_Loadouts"] != null 
                                    ? (bool)jsonValues["Remove_Ground_Weapons_After_Give_Loadouts"] 
                                    : false;
            if (Remove_Ground_Weapons_After_Give_Loadouts)
            {
                SpawnLoadoutGoldKingZ.Instance.g_Main.ForceRemoveGroundWeapons = true; 
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

    public static async Task DownloadMissingFiles()
    {
        string baseFolderPath = Configs.Shared.CookiesModule!;

        string gamedataFileName = "gamedata/Spawn_Loadout_gamedata.json";
        string gamedataGithubUrl = "https://raw.githubusercontent.com/oqyh/cs2-Spawn-Loadout-GoldKingZ/main/Resources/Spawn_Loadout_gamedata.json";
        string gamedataFilePath = Path.Combine(baseFolderPath, gamedataFileName);
        string gamedataDirectoryPath = Path.GetDirectoryName(gamedataFilePath)!;
        await CheckAndDownloadFile(gamedataFilePath, gamedataGithubUrl, gamedataDirectoryPath);


        string settingsFileName = "config/Weapons_Settings.json";
        string settingsGithubUrl = "https://raw.githubusercontent.com/oqyh/cs2-Spawn-Loadout-GoldKingZ/main/Resources/Weapons_Settings.json";
        string settingsFilePath = Path.Combine(baseFolderPath, settingsFileName);
        string settingsDirectoryPath = Path.GetDirectoryName(settingsFilePath)!;
        await DownloadFileIfNotExists(settingsFilePath, settingsGithubUrl, settingsDirectoryPath);
    }
    public static async Task DownloadFileIfNotExists(string filePath, string githubUrl, string directoryPath)
    {
        if (!File.Exists(filePath))
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            await DownloadFileFromGithub(githubUrl, filePath);
        }
    }

    public static async Task<bool> CheckAndDownloadFile(string filePath, string githubUrl, string directoryPath)
    {
        if (!File.Exists(filePath))
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            await DownloadFileFromGithub(githubUrl, filePath);
            return true;
        }
        else
        {
            if (Configs.GetConfigData().AutoUpdateSignatures)
            {
                bool isFileDifferent = await IsFileDifferent(filePath, githubUrl);
                if (isFileDifferent)
                {
                    File.Delete(filePath);
                    await DownloadFileFromGithub(githubUrl, filePath);
                    return true;
                }
            }
            
        }

        return false;
    }


    public static async Task<bool> IsFileDifferent(string localFilePath, string githubUrl)
    {
        try
        {
            byte[] localFileBytes = await File.ReadAllBytesAsync(localFilePath);
            string localFileHash = GetFileHash(localFileBytes);

            using (HttpClient client = new HttpClient())
            {
                byte[] githubFileBytes = await client.GetByteArrayAsync(githubUrl);
                string githubFileHash = GetFileHash(githubFileBytes);
                return localFileHash != githubFileHash;
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"Error comparing files: {ex.Message}");
            return false;
        }
    }

    public static string GetFileHash(byte[] fileBytes)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(fileBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    public static async Task DownloadFileFromGithub(string url, string destinationPath)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                byte[] fileBytes = await client.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(destinationPath, fileBytes);
            }
            catch (Exception ex)
            {
                DebugMessage($"Error downloading file: {ex.Message}");
            }
        }
    }

    public static void GivePlayerHealthNArmor(CCSPlayerController player, int CThealth = -1, int Thealth = -1, int CTarmor = -1, int Tarmor = -1)
    {
        Server.NextFrame(() =>
        {
        if(player == null || !player.IsValid || !player.PawnIsAlive || 
        player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid )return;

        if(CThealth > -1 || Thealth > -1)
        {
            if(CThealth > -1 && player.TeamNum == (byte)CsTeam.CounterTerrorist)
            {
                player.PlayerPawn.Value.Health = CThealth;
                Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
            }
            if(Thealth > -1 && player.TeamNum == (byte)CsTeam.Terrorist)
            {
                player.PlayerPawn.Value.Health = Thealth;
                Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
            }
            
        }else
        {
            if(SpawnLoadoutGoldKingZ.Instance.g_Main.GiveHealthCT > -1 || SpawnLoadoutGoldKingZ.Instance.g_Main.GiveHealthT > -1)
            {
                if(SpawnLoadoutGoldKingZ.Instance.g_Main.GiveHealthCT > -1 && player.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    player.PlayerPawn.Value.Health = SpawnLoadoutGoldKingZ.Instance.g_Main.GiveHealthCT;
                    Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
                }
                if(SpawnLoadoutGoldKingZ.Instance.g_Main.GiveHealthT > -1 && player.TeamNum == (byte)CsTeam.Terrorist)
                {
                    player.PlayerPawn.Value.Health = SpawnLoadoutGoldKingZ.Instance.g_Main.GiveHealthT;
                    Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
                }
            }
        }


        if(CTarmor > -1 || Tarmor > -1)
        {
            if(CTarmor > -1 && player.TeamNum == (byte)CsTeam.CounterTerrorist)
            {
                player.PlayerPawn.Value.ArmorValue = CTarmor;
                Utilities.SetStateChanged(player.PlayerPawn.Value, "CCSPlayerPawn", "m_ArmorValue");
            }
            if(Tarmor > -1 && player.TeamNum == (byte)CsTeam.Terrorist)
            {
                player.PlayerPawn.Value.ArmorValue = Tarmor;
                Utilities.SetStateChanged(player.PlayerPawn.Value, "CCSPlayerPawn", "m_ArmorValue");
            }
            
        }else
        {
            if(SpawnLoadoutGoldKingZ.Instance.g_Main.GiveArmorCT > -1 || SpawnLoadoutGoldKingZ.Instance.g_Main.GiveArmorT > -1)
            {
                if(SpawnLoadoutGoldKingZ.Instance.g_Main.GiveArmorCT > -1 && player.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    player.PlayerPawn.Value.ArmorValue = SpawnLoadoutGoldKingZ.Instance.g_Main.GiveArmorCT;
                    Utilities.SetStateChanged(player.PlayerPawn.Value, "CCSPlayerPawn", "m_ArmorValue");
                }
                if(SpawnLoadoutGoldKingZ.Instance.g_Main.GiveArmorT > -1 && player.TeamNum == (byte)CsTeam.Terrorist)
                {
                    player.PlayerPawn.Value.ArmorValue = SpawnLoadoutGoldKingZ.Instance.g_Main.GiveArmorT;
                    Utilities.SetStateChanged(player.PlayerPawn.Value, "CCSPlayerPawn", "m_ArmorValue");
                }
            }
        }
        });
    }
}