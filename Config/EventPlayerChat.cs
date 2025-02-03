using CounterStrikeSharp.API.Core;
using Spawn_Loadout_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Commands;

namespace Spawn_Loadout_GoldKingZ;

public class PlayerChat
{
    public HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo info, bool TeamChat)
	{
        if (player == null || !player.IsValid)return HookResult.Continue;
        var playerid = player.SteamID;
        var eventmessage = info.ArgString;
        eventmessage = eventmessage.TrimStart('"');
        eventmessage = eventmessage.TrimEnd('"');
        
        if (string.IsNullOrWhiteSpace(eventmessage)) return HookResult.Continue;
        string trimmedMessageStart = eventmessage.TrimStart();
        string message = trimmedMessageStart.TrimEnd();

        string[] SL_Reload_Weapons_Settings_CommandsInGames = Configs.GetConfigData().SL_Reload_Weapons_Settings_CommandsInGame.Split(',');
        if (SL_Reload_Weapons_Settings_CommandsInGames.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            if (!string.IsNullOrEmpty(Configs.GetConfigData().SL_Reload_Weapons_Settings_Flags) && !Helper.IsPlayerInGroupPermission(player, Configs.GetConfigData().SL_Reload_Weapons_Settings_Flags))
            {
                Helper.AdvancedPlayerPrintToChat(player, Configs.Shared.StringLocalizer!["PrintChatToPlayer.Not.Allowed.ToReload"]);
                return HookResult.Continue;
            }
            Helper.ClearVariables();
            Helper.SetValues(player);
            Helper.AdvancedPlayerPrintToChat(player, Configs.Shared.StringLocalizer!["PrintChatToPlayer.Plugin.Reloaded"]); 
        }
        return HookResult.Continue;
    }
}