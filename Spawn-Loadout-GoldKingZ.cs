using Newtonsoft.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using Spawn_Loadout_GoldKingZ.Config;
using Newtonsoft.Json.Linq;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;

namespace Spawn_Loadout_GoldKingZ;

public class SpawnLoadoutGoldKingZ : BasePlugin
{
    public override string ModuleName => "Give Weapons On Spawn (Depend The Map Name + Team Side)";
    public override string ModuleVersion => "1.0.8";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";

    public static SpawnLoadoutGoldKingZ Instance { get; set; } = new();
    public Globals g_Main = new();
    private readonly PlayerChat _PlayerChat = new();
    
    public override void Load(bool hotReload)
    {
        Instance = this;
        Configs.Load(ModuleDirectory);
        Configs.Shared.CookiesModule = ModuleDirectory;
        Configs.Shared.StringLocalizer = Localizer;

        _ = Task.Run(async () =>
        {
            await Helper.DownloadMissingFiles();
        });

        Configs.Shared.CustomFunctions = new CustomGameData();
        

        RegisterEventHandler<EventPlayerSpawn>(OnEventPlayerSpawn, HookMode.Post);
        RegisterEventHandler<EventGrenadeThrown>(OnEventGrenadeThrown, HookMode.Post);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterEventHandler<EventRoundStart>(OnEventRoundStart);
        RegisterEventHandler<EventRoundPrestart>(OnEventRoundPrestart, HookMode.Pre);
        AddCommandListener("say", OnPlayerChat, HookMode.Post);
		AddCommandListener("say_team", OnPlayerChatTeam, HookMode.Post);
        
        if(hotReload)
        {
            Helper.SetValues();
            bool GetBoolServerCommands = g_Main.ForceRemoveServerCommands? true : false;
            bool GetBoolClientCommands = g_Main.ForceRemoveClientCommands? true : false;
            Helper.ClearMapCommands(GetBoolServerCommands,GetBoolClientCommands);
        }
        
    }
    
    public void OnMapStart(string mapname)
    {
        Helper.SetValues();
        bool GetBoolServerCommands = g_Main.ForceRemoveServerCommands? true : false;
        bool GetBoolClientCommands = g_Main.ForceRemoveClientCommands? true : false;
        Helper.ClearMapCommands(GetBoolServerCommands,GetBoolClientCommands);
    }
    private HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo info)
	{
        if (player == null || !player.IsValid)return HookResult.Continue;
        _PlayerChat.OnPlayerChat(player, info, false);
        return HookResult.Continue;
    }
    private HookResult OnPlayerChatTeam(CCSPlayerController? player, CommandInfo info)
	{
        if (player == null || !player.IsValid)return HookResult.Continue;
        _PlayerChat.OnPlayerChat(player, info, true);
        return HookResult.Continue;
    }
    
    public HookResult OnEventGrenadeThrown(EventGrenadeThrown @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;

        var player = @event.Userid;
        if (player == null || !player.IsValid)return HookResult.Continue;

        var Weapon = @event.Weapon;
        if (Weapon == null)return HookResult.Continue;
        
        string nadename = "weapon_" + Weapon;

        var jsonValues = g_Main.JsonData;
        if (jsonValues == null)return HookResult.Continue;
        

        foreach (var loadout in jsonValues)
        {
            dynamic loadoutValue = loadout.Value;
            if (loadoutValue is JObject)
            {
                string key = loadout.Key;
                string loadoutName = loadoutValue.Name;

                string HasFlag = loadoutValue.ContainsKey("Flags") && loadoutValue["Flags"] != null
                    ? loadoutValue["Flags"].ToString()
                    : null!;

                string ctREFILENades = loadoutValue.ContainsKey("CT_Refill_Nades") && loadoutValue["CT_Refill_Nades"] != null
                    ? loadoutValue["CT_Refill_Nades"].ToString()
                    : null!;

                string tREFILENades = loadoutValue.ContainsKey("T_Refill_Nades") && loadoutValue["T_Refill_Nades"] != null
                    ? loadoutValue["T_Refill_Nades"].ToString()
                    : null!;

                int ctRefillTime = loadoutValue.ContainsKey("CT_Refill_Time_InSec") && loadoutValue["CT_Refill_Time_InSec"] != null
                    ? (int)loadoutValue["CT_Refill_Time_InSec"]
                    : 0;

                int tRefillTime = loadoutValue.ContainsKey("T_Refill_Time_InSec") && loadoutValue["T_Refill_Time_InSec"] != null
                    ? (int)loadoutValue["T_Refill_Time_InSec"]
                    : 0;

                bool Give_This_LoadOut_OneTime = loadoutValue.ContainsKey("Give_This_LoadOut_OneTime") && loadoutValue["Give_This_LoadOut_OneTime"] != null
                    ? (bool)loadoutValue["Give_This_LoadOut_OneTime"]
                    : false;

                int Give_This_LoadOut_On_Round_And_After = loadoutValue.ContainsKey("Give_This_LoadOut_On_Round_And_After") && loadoutValue["Give_This_LoadOut_On_Round_And_After"] != null
                    ? (int)loadoutValue["Give_This_LoadOut_On_Round_And_After"]
                    : -1;

                if (!string.IsNullOrEmpty(HasFlag) && !Helper.IsPlayerInGroupPermission(player, HasFlag)) continue;

                if (Give_This_LoadOut_On_Round_And_After > -1 && Helper.GetCurrentRound() < Give_This_LoadOut_On_Round_And_After)
                {
                    continue;
                }

                if (Give_This_LoadOut_OneTime)
                {
                    if (!g_Main.loadoutsGivenPerPlayer.ContainsKey(player))
                    {
                        g_Main.loadoutsGivenPerPlayer[player] = new HashSet<string>();
                    }

                    if (g_Main.loadoutsGivenPerPlayer[player].Contains(loadoutName))
                    {
                        Helper.AdvancedPlayerPrintToChat(player, Configs.Shared.StringLocalizer!["PrintChatToPlayer.Got.Loadout"]);
                        continue;
                    }
                    else
                    {
                        g_Main.loadoutsGivenPerPlayer[player].Add(loadoutName);
                    }
                }

                bool teamct = player.TeamNum == (byte)CsTeam.CounterTerrorist && !string.IsNullOrEmpty(ctREFILENades);
                bool teamt = player.TeamNum == (byte)CsTeam.Terrorist && !string.IsNullOrEmpty(tREFILENades);
                bool grenadematchct = !string.IsNullOrEmpty(ctREFILENades) && nadename.Contains(ctREFILENades.Trim());
                bool grenadematcht = !string.IsNullOrEmpty(tREFILENades) && nadename.Contains(tREFILENades.Trim());

                if (teamct && grenadematchct)
                {
                    Server.NextFrame(() =>
                    {
                        AddTimer(ctRefillTime, () =>
                        {
                            if (player != null && player.IsValid && player.PawnIsAlive && player.TeamNum == (byte)CsTeam.CounterTerrorist)
                            {
                                Helper.GiveWeaponsToPlayer(player, nadename);
                            }
                        }, TimerFlags.STOP_ON_MAPCHANGE);
                    });
                }
                else if (teamt && grenadematcht)
                {
                    Server.NextFrame(() =>
                    {
                        AddTimer(tRefillTime, () =>
                        {
                            if (player != null && player.IsValid && player.PawnIsAlive && player.TeamNum == (byte)CsTeam.Terrorist)
                            {
                                Helper.GiveWeaponsToPlayer(player, nadename);
                            }
                        }, TimerFlags.STOP_ON_MAPCHANGE);
                    });
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

        Helper.ForceStripePlayer(player);
        Helper.GivePlayerHealthNArmor(player);

        Server.NextFrame(() =>
        {
            AddTimer(g_Main.DelayGiveLoadOut, () =>
            {
                if (player == null || !player.IsValid) return;

                var jsonValues = g_Main.JsonData;
                if (jsonValues == null) return;

                foreach (var loadout in jsonValues)
                {
                    dynamic loadoutValue = loadout.Value;
                    if (loadoutValue is JObject)
                    {
                        string key = loadout.Key;
                        string loadoutName = loadoutValue.Name;

                        string HasFlag = loadoutValue.ContainsKey("Flags") && loadoutValue["Flags"] != null 
                        ? loadoutValue["Flags"].ToString() 
                        : null!;

                        string ctWeapons = loadoutValue.ContainsKey("CT") && loadoutValue["CT"] != null 
                            ? loadoutValue["CT"].ToString() 
                            : null!;

                        string tWeapons = loadoutValue.ContainsKey("T") && loadoutValue["T"] != null 
                            ? loadoutValue["T"].ToString() 
                            : null!;

                        bool Give_This_LoadOut_OneTime = loadoutValue.ContainsKey("Give_This_LoadOut_OneTime") && loadoutValue["Give_This_LoadOut_OneTime"] != null 
                            ? (bool)loadoutValue["Give_This_LoadOut_OneTime"] 
                            : false;
                        
                        bool Force_Give_This_LoadOut = loadoutValue.ContainsKey("Force_Give_This_LoadOut") && loadoutValue["Force_Give_This_LoadOut"] != null 
                            ? (bool)loadoutValue["Force_Give_This_LoadOut"] 
                            : false;

                        bool Force_Strip_CT_Players = loadoutValue.ContainsKey("Force_Strip_CT_Players") && loadoutValue["Force_Strip_CT_Players"] != null 
                            ? (bool)loadoutValue["Force_Strip_CT_Players"] 
                            : false;

                        bool Force_Strip_T_Players = loadoutValue.ContainsKey("Force_Strip_T_Players") && loadoutValue["Force_Strip_T_Players"] != null 
                            ? (bool)loadoutValue["Force_Strip_T_Players"] 
                            : false;

                        bool Remove_Knife_CT = loadoutValue.ContainsKey("Remove_Knife_CT") && loadoutValue["Remove_Knife_CT"] != null 
                            ? (bool)loadoutValue["Remove_Knife_CT"] 
                            : false;

                        bool Remove_Knife_T = loadoutValue.ContainsKey("Remove_Knife_T") && loadoutValue["Remove_Knife_T"] != null 
                            ? (bool)loadoutValue["Remove_Knife_T"] 
                            : false;

                        int Give_This_LoadOut_On_Round_And_After = loadoutValue.ContainsKey("Give_This_LoadOut_On_Round_And_After") && loadoutValue["Give_This_LoadOut_On_Round_And_After"] != null 
                            ? (int)loadoutValue["Give_This_LoadOut_On_Round_And_After"] 
                            : -1;

                        int Players_Health_CT = loadoutValue.ContainsKey("Players_Health_CT") && loadoutValue["Players_Health_CT"] != null 
                            ? (int)loadoutValue["Players_Health_CT"] 
                            : -1;
                        
                        int Players_Health_T = loadoutValue.ContainsKey("Players_Health_T") && loadoutValue["Players_Health_T"] != null 
                            ? (int)loadoutValue["Players_Health_T"] 
                            : -1;

                        int Players_Armor_CT = loadoutValue.ContainsKey("Players_Armor_CT") && loadoutValue["Players_Armor_CT"] != null 
                            ? (int)loadoutValue["Players_Armor_CT"] 
                            : -1;

                        int Players_Armor_T = loadoutValue.ContainsKey("Players_Armor_T") && loadoutValue["Players_Armor_T"] != null 
                            ? (int)loadoutValue["Players_Armor_T"] 
                            : -1;


                        if (!string.IsNullOrEmpty(HasFlag) && !Helper.IsPlayerInGroupPermission(player, HasFlag)) continue;

                        if (Give_This_LoadOut_On_Round_And_After > -1 && Helper.GetCurrentRound() < Give_This_LoadOut_On_Round_And_After)
                        {
                            continue;
                        }
                        

                        if (Give_This_LoadOut_OneTime)
                        {
                            if (!g_Main.loadoutsGivenPerPlayer.ContainsKey(player))
                            {
                                g_Main.loadoutsGivenPerPlayer[player] = new HashSet<string>();
                            }

                            if (g_Main.loadoutsGivenPerPlayer[player].Contains(loadoutName))
                            {
                                Helper.AdvancedPlayerPrintToChat(player, Configs.Shared.StringLocalizer!["PrintChatToPlayer.Got.Loadout"]);
                                continue;
                            }
                            else
                            {
                                g_Main.loadoutsGivenPerPlayer[player].Add(loadoutName);
                            }
                        }

                        Helper.ForceStripePlayer(player,Force_Strip_CT_Players,Force_Strip_T_Players,Remove_Knife_CT,Remove_Knife_T);
                        Helper.GivePlayerHealthNArmor(player,Players_Health_CT,Players_Health_T,Players_Armor_CT,Players_Armor_T);

                        bool teamct = player.TeamNum == (byte)CsTeam.CounterTerrorist && !string.IsNullOrEmpty(ctWeapons)?true:false;
                        bool teamt = player.TeamNum == (byte)CsTeam.Terrorist && !string.IsNullOrEmpty(tWeapons)?true:false;

                        if(teamct)
                        {
                            Helper.GiveWeaponsToPlayer(player, ctWeapons,Force_Give_This_LoadOut);
                        }else if(teamt)
                        {
                            Helper.GiveWeaponsToPlayer(player, tWeapons,Force_Give_This_LoadOut);
                        }
                    }
                }
                if (g_Main.ForceRemoveGroundWeapons)
                {
                    Helper.ClearGroundWeapons();
                }
            },TimerFlags.STOP_ON_MAPCHANGE);
        });
        return HookResult.Continue;
    }

    private HookResult OnEventRoundPrestart(EventRoundPrestart @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        bool GetBoolServerCommands = g_Main.ForceRemoveServerCommands? true : false;
        bool GetBoolClientCommands = g_Main.ForceRemoveClientCommands? true : false;
        Helper.ClearMapCommands(GetBoolServerCommands,GetBoolClientCommands);
        
        if(g_Main.RemoveBuyMenu)
        {
            Server.ExecuteCommand("sv_buy_status_override 3");
        }

        return HookResult.Continue;
    }

    private HookResult OnEventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        g_Main.loadoutsGivenPerPlayer.Clear();

        bool GetBoolServerCommands = g_Main.ForceRemoveServerCommands? true : false;
        bool GetBoolClientCommands = g_Main.ForceRemoveClientCommands? true : false;
        Helper.ClearMapCommands(GetBoolServerCommands,GetBoolClientCommands);

        if(g_Main.RemoveBuyMenu)
        {
            Server.ExecuteCommand("sv_buy_status_override 3");
        }

        return HookResult.Continue;
    }


    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid)return HookResult.Continue;

        if (g_Main.loadoutsGivenPerPlayer.ContainsKey(player))g_Main.loadoutsGivenPerPlayer.Remove(player);

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