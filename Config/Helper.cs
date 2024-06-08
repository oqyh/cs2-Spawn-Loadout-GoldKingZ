using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using Newtonsoft.Json;
using Spawn_Loadout_GoldKingZ.Config;

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
    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        var excludedGroups = groups.Split(',');
        foreach (var group in excludedGroups)
        {
            if (group.StartsWith("#"))
            {
                if (AdminManager.PlayerInGroup(player, group))
                    return true;
            }
            else if (group.StartsWith("@"))
            {
                if (AdminManager.PlayerHasPermissions(player, group))
                    return true;
            }
        }
        return false;
    }

    public static void ClearVariables()
    {
        Globals.loadoutsGivenPerPlayer.Clear();
    }
    public static List<CCSPlayerController> GetAllController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected).ToList();
        return playerList;
    }

    public static void GiveWeaponsToPlayer(CCSPlayerController player, string weapons)
    {
        if(player != null && player.IsValid)
        {
            string[] weaponList = weapons.Split(',');
            foreach (var weapon in weaponList)
            {
                Configs.Shared.CustomFunctions!.PlayerGiveNamedItem(player, weapon);

                foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(weapon))
                {
                    if (entity.DesignerName != weapon) continue;
                    if (entity == null) continue;
                    if (entity.Entity == null) continue;
                    if (entity.OwnerEntity == null) continue;
                    if (entity.OwnerEntity.IsValid) continue;

                    entity.AcceptInput("Kill");
                    break;
                }
            }
        }
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

    public static void DropAllWeapons(CCSPlayerController player)
    {
        if(player == null || !player.IsValid)return;
        if(player.PlayerPawn == null || !player.PlayerPawn.IsValid)return;
        if(!player.PawnIsAlive)return;
        if(player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid)return;
        if(player.PlayerPawn.Value.WeaponServices == null || player.PlayerPawn.Value.WeaponServices.MyWeapons == null)return;

        foreach (var weapon in player.PlayerPawn.Value.WeaponServices.MyWeapons)
        {
            if (weapon == null || !weapon.IsValid) continue;
            var weaponValue = weapon.Value;
            if (weaponValue == null || !weaponValue.IsValid) continue;
            if (weaponValue.DesignerName != null && weaponValue.DesignerName.Contains("weapon_knife") || weaponValue.DesignerName != null && weaponValue.DesignerName.Contains("weapon_c4"))continue;
            if(weaponValue.DesignerName == null)continue;
            Utilities.RemoveItemByDesignerName(player, weaponValue.DesignerName);
        }
        
    }
    public static void CreateDefaultWeaponsJson()
    {
        if (Configs.Shared.CookiesModule == null) return;
        var FolderConfig = Path.Combine(Configs.Shared.CookiesModule, "../../plugins/Spawn-Loadout-GoldKingZ/config/");
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
                    LOADOUT_1 = new
                    {
                        FLAGS = "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",
                        CT = "weapon_taser,weapon_decoy",
                        T = "weapon_taser,weapon_decoy",
                        CT_Refill_Nades = "weapon_decoy",
                        CT_Refill_Time_InSec = 30,
                        T_Refill_Nades = "weapon_decoy",
                        T_Refill_Time_InSec = 30
                    },
                    LOADOUT_2 = new
                    {
                        GiveThisLoadOutPerRoundOnly = true,
                        CT = "weapon_hkp2000,weapon_knife,weapon_smokegrenade",
                        T = "weapon_hkp2000,weapon_knife,weapon_smokegrenade"
                    }
                },
                hns_ = new
                {
                    LOADOUT_1 = new
                    {
                        FLAGS = "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",
                        CT_Refill_Nades = "weapon_decoy",
                        CT_Refill_Time_InSec = 30,
                        T_Refill_Nades = "weapon_decoy",
                        T_Refill_Time_InSec = 30
                    },
                    LOADOUT_2 = new
                    {
                        GiveThisLoadOutPerRoundOnly = true,
                        CT = "weapon_decoy,weapon_knife",
                        T = "weapon_decoy,weapon_knife"
                    }
                },
                awp_lego_2_cs2 = new
                {
                    ForceStripPlayers = true,
                    DeleteGroundWeapons = true,
                    LOADOUT_1 = new
                    {
                        FLAGS = "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",
                        CT = "weapon_deagle",
                        T = "weapon_deagle"
                    },
                    LOADOUT_2 = new
                    {
                        CT = "weapon_awp,weapon_knife",
                        T = "weapon_awp,weapon_knife"
                    }
                }
            };

            var jsonContent = JsonConvert.SerializeObject(weaponsData, Formatting.Indented);

            File.WriteAllText(configcfg, jsonContent);
        }
    }
}