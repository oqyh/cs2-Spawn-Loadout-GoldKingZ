using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;

namespace Spawn_Loadout_GoldKingZ;

public class Helper
{
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
        Globals.Gived.Clear();
        Globals.VipsFlag.Clear();
    }
}