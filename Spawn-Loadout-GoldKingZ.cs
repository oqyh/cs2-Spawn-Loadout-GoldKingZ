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

namespace Spawn_Loadout_GoldKingZ;

[MinimumApiVersion(276)]
public class SpawnLoadoutGoldKingZ : BasePlugin
{
    public override string ModuleName => "Give Weapons On Spawn (Depend The Map Name + Team Side)";
    public override string ModuleVersion => "1.0.6";
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
        Configs.Shared.CustomFunctions = new CustomGameData();
        Helper.SetValuesToGlobals();

        RegisterEventHandler<EventPlayerSpawn>(OnEventPlayerSpawn, HookMode.Post);
        RegisterEventHandler<EventGrenadeThrown>(OnEventGrenadeThrown, HookMode.Post);
        RegisterEventHandler<EventPlayerDeath>(OnEventPlayerDeath);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterEventHandler<EventRoundStart>(OnEventRoundStart);
        RegisterEventHandler<EventRoundPrestart>(OnEventRoundPrestart, HookMode.Pre);
        AddCommandListener("say", OnPlayerChat, HookMode.Post);
		AddCommandListener("say_team", OnPlayerChatTeam, HookMode.Post);  
    }
    
    public void OnMapStart(string mapname)
    {
        Helper.SetValuesToGlobals();

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
        if (@event == null) return HookResult.Continue;

        var player = @event.Userid;
        if (player == null || !player.IsValid) return HookResult.Continue;
        
        var nadename = @event.Weapon;
        if (nadename == null) return HookResult.Continue;

        var playerpawn = player.PlayerPawn;
        if (playerpawn == null || !playerpawn.IsValid) return HookResult.Continue;

        var playerpawnvalue = playerpawn.Value;
        if (playerpawnvalue == null || !playerpawnvalue.IsValid) return HookResult.Continue;

        var playerWeaponServices = playerpawnvalue.WeaponServices;
        if (playerWeaponServices == null) return HookResult.Continue;

        var playerMyWeapons = playerWeaponServices.MyWeapons;
        if (playerMyWeapons == null) return HookResult.Continue;
        
        
        var jsonValues = g_Main.GetJsonValues();
        if (jsonValues == null || jsonValues.Count == 0) return HookResult.Continue;

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


                int giveOncePerRound = loadoutValue.ContainsKey("Give_This_LoadOut_PerRound_Only") && loadoutValue["Give_This_LoadOut_PerRound_Only"] != null 
                    ? (int)loadoutValue["Give_This_LoadOut_PerRound_Only"] 
                    : 0;
                

                if (HasFlag != null && !string.IsNullOrEmpty(HasFlag) && !Helper.IsPlayerInGroupPermission(player, HasFlag)) continue;
                
                if (giveOncePerRound == 1 || giveOncePerRound == 2)
                {
                    if (!Helper.IsWarmup() || giveOncePerRound == 2)
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
                }

                if(player.TeamNum == (byte)CsTeam.CounterTerrorist && ctREFILENades != null)
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
                        }
                        
                    }
                }else if(player.TeamNum == (byte)CsTeam.Terrorist && tREFILENades != null)
                {
                    var tRefillNadesArray = tREFILENades.Split(',');
                    foreach (var grenade in tRefillNadesArray)
                    {
                        if ("weapon_" + nadename == grenade.Trim())
                        {
                            Server.NextFrame(() =>
                            {
                                AddTimer(ctRefillTime, () =>
                                {
                                    if(player != null && player.IsValid && player.PawnIsAlive && player.TeamNum == (byte)CsTeam.Terrorist)
                                    {
                                        Helper.GiveWeaponsToPlayer(player, "weapon_" + nadename);
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
    
        var playerpawn = player.PlayerPawn;
        if (playerpawn == null || !playerpawn.IsValid) return HookResult.Continue;

        var playerpawnvalue = playerpawn.Value;
        if (playerpawnvalue == null || !playerpawnvalue.IsValid) return HookResult.Continue;

        var playerWeaponServices = playerpawnvalue.WeaponServices;
        if (playerWeaponServices == null) return HookResult.Continue;

        var playerMyWeapons = playerWeaponServices.MyWeapons;
        if (playerMyWeapons == null) return HookResult.Continue;
        
        
        var jsonValues = g_Main.GetJsonValues();
        if (jsonValues == null || jsonValues.Count == 0) return HookResult.Continue;

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

                int giveOncePerRound = loadoutValue.ContainsKey("Give_This_LoadOut_PerRound_Only") && loadoutValue["Give_This_LoadOut_PerRound_Only"] != null 
                    ? (int)loadoutValue["Give_This_LoadOut_PerRound_Only"] 
                    : 0;

                if (HasFlag != null && !string.IsNullOrEmpty(HasFlag) && !Helper.IsPlayerInGroupPermission(player, HasFlag)) continue;
                
                if (giveOncePerRound == 1 || giveOncePerRound == 2)
                {
                    if (!Helper.IsWarmup() || giveOncePerRound == 2)
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
                }

                if(player.TeamNum == (byte)CsTeam.CounterTerrorist && ctWeapons != null)
                {
                    var weaponsCTSet = new HashSet<string>(ctWeapons.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim()));
                    
                    if(g_Main.ForceStripPlayers)
                    {
                        if (!g_Main.PlayerCleanUp.ContainsKey(player))
                        {
                            CounterStrikeSharp.API.Modules.Timers.Timer? timerz = AddTimer(0.1f, () => Helper.CleanUpPlayerWeapons(player), TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
                            g_Main.PlayerCleanUp.Add(player, new Globals.GetPlayerWeapons(player,timerz,null!,weaponsCTSet));
                        }
                        if (g_Main.PlayerCleanUp.ContainsKey(player))
                        {
                            g_Main.PlayerCleanUp[player].Player = player;
                            g_Main.PlayerCleanUp[player].Timer?.Kill();
                            g_Main.PlayerCleanUp[player].Timer = null!;
                            g_Main.PlayerCleanUp[player].Timer = AddTimer(0.1f, () => Helper.CleanUpPlayerWeapons(player), TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
                            var newWeapons = weaponsCTSet.Except(g_Main.PlayerCleanUp[player].WeaponsNeeded).ToHashSet();
                            g_Main.PlayerCleanUp[player].WeaponsNeeded.UnionWith(newWeapons);

                            g_Main.PlayerCleanUp[player].KillTheTimer?.Kill();
                            g_Main.PlayerCleanUp[player].KillTheTimer = AddTimer(3.0f, () => 
                            {
                                if (player != null && player.IsValid && g_Main.PlayerCleanUp.ContainsKey(player) && g_Main.PlayerCleanUp[player].Timer != null)
                                {
                                    g_Main.PlayerCleanUp[player].Timer?.Kill();
                                    g_Main.PlayerCleanUp[player].Timer = null!;
                                }
                            }, TimerFlags.STOP_ON_MAPCHANGE);
                        }
                    }else
                    {
                        foreach (var playerMyWeaponst in playerMyWeapons)
                        {
                            var weaponsValue = playerMyWeaponst.Value;

                            if (weaponsValue == null || !weaponsValue.IsValid)
                                continue;

                            var weaponsDesignerName = weaponsValue.DesignerName;

                            if (string.IsNullOrWhiteSpace(weaponsDesignerName))
                                continue;

                            if (weaponsDesignerName.Contains("weapon_knife") && !g_Main.RemoveKnife || 
                                weaponsDesignerName.Contains("weapon_bayonet") && !g_Main.RemoveKnife || 
                                weaponsDesignerName.Contains("weapon_c4"))
                                continue;

                            if (!weaponsCTSet.Contains(weaponsDesignerName))
                            {
                                Helper.RemoveWeaponByName(player, weaponsDesignerName);
                                
                            }
                        }
                    }

                    if (g_Main.ForceRemoveGroundWeapons == 1 || g_Main.ForceRemoveGroundWeapons == 2)
                    {
                        if (Helper.IsWarmup() || g_Main.ForceRemoveGroundWeapons == 2)
                        {
                            Helper.ClearGroundWeapons();
                        }
                    }
                    
                    foreach (var weaponCT in weaponsCTSet.Where(w => !string.IsNullOrWhiteSpace(w)))
                    {
                        Server.NextFrame(() =>
                        {
                            AddTimer(g_Main.DelayGiveLoadOut, () =>
                            {
                                Helper.GiveWeaponsToPlayer(player, weaponCT);
                            },TimerFlags.STOP_ON_MAPCHANGE);
                        });
                    }
                    
                }else if(player.TeamNum == (byte)CsTeam.Terrorist && tWeapons != null)
                {
                    var weaponsTSet = new HashSet<string>(tWeapons.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim()));

                    if(g_Main.ForceStripPlayers)
                    {
                        if (!g_Main.PlayerCleanUp.ContainsKey(player))
                        {
                            CounterStrikeSharp.API.Modules.Timers.Timer? timerz = AddTimer(0.1f, () => Helper.CleanUpPlayerWeapons(player), TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
                            g_Main.PlayerCleanUp.Add(player, new Globals.GetPlayerWeapons(player,timerz,null!,weaponsTSet));
                        }
                        if (g_Main.PlayerCleanUp.ContainsKey(player))
                        {
                            g_Main.PlayerCleanUp[player].Player = player;
                            g_Main.PlayerCleanUp[player].Timer?.Kill();
                            g_Main.PlayerCleanUp[player].Timer = null!;
                            g_Main.PlayerCleanUp[player].Timer = AddTimer(0.1f, () => Helper.CleanUpPlayerWeapons(player), TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
                            var newWeapons = weaponsTSet.Except(g_Main.PlayerCleanUp[player].WeaponsNeeded).ToHashSet();
                            g_Main.PlayerCleanUp[player].WeaponsNeeded.UnionWith(newWeapons);

                            g_Main.PlayerCleanUp[player].KillTheTimer?.Kill();
                            g_Main.PlayerCleanUp[player].KillTheTimer = AddTimer(3.0f, () => 
                            {
                                if (player != null && player.IsValid && g_Main.PlayerCleanUp.ContainsKey(player) && g_Main.PlayerCleanUp[player].Timer != null)
                                {
                                    g_Main.PlayerCleanUp[player].Timer?.Kill();
                                    g_Main.PlayerCleanUp[player].Timer = null!;
                                }
                            }, TimerFlags.STOP_ON_MAPCHANGE);
                        }
                    }else
                    {
                        foreach (var playerMyWeaponst in playerMyWeapons)
                        {
                            var weaponsValue = playerMyWeaponst.Value;

                            if (weaponsValue == null || !weaponsValue.IsValid)
                                continue;

                            var weaponsDesignerName = weaponsValue.DesignerName;

                            if (string.IsNullOrWhiteSpace(weaponsDesignerName))
                                continue;

                            if (weaponsDesignerName.Contains("weapon_knife") && !g_Main.RemoveKnife || 
                                weaponsDesignerName.Contains("weapon_bayonet") && !g_Main.RemoveKnife || 
                                weaponsDesignerName.Contains("weapon_c4"))
                                continue;

                            if (!weaponsTSet.Contains(weaponsDesignerName))
                            {
                                Helper.RemoveWeaponByName(player, weaponsDesignerName);
                                
                            }
                        }
                    }

                    if (g_Main.ForceRemoveGroundWeapons == 1 || g_Main.ForceRemoveGroundWeapons == 2)
                    {
                        if (Helper.IsWarmup() || g_Main.ForceRemoveGroundWeapons == 2)
                        {
                            Helper.ClearGroundWeapons();
                        }
                    }
                    
                    foreach (var weaponCT in weaponsTSet.Where(w => !string.IsNullOrWhiteSpace(w)))
                    {
                        Server.NextFrame(() =>
                        {
                            AddTimer(g_Main.DelayGiveLoadOut, () =>
                            {
                                Helper.GiveWeaponsToPlayer(player, weaponCT);
                            },TimerFlags.STOP_ON_MAPCHANGE);
                        });
                    }
                }
            }
        }
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


    private HookResult OnEventPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        var player = @event.Userid;
        if(player == null || !player.IsValid)return HookResult.Continue;

        if (g_Main.PlayerCleanUp.ContainsKey(player))
        {
            g_Main.PlayerCleanUp[player].Timer?.Kill();
            g_Main.PlayerCleanUp[player].Timer = null!;

            g_Main.PlayerCleanUp[player].KillTheTimer?.Kill();
            g_Main.PlayerCleanUp[player].KillTheTimer = null!;
        }
        return HookResult.Continue;
    }


    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid)return HookResult.Continue;

        if (g_Main.PlayerCleanUp.ContainsKey(player))
        {
            g_Main.PlayerCleanUp[player].Timer?.Kill();
            g_Main.PlayerCleanUp[player].Timer = null!;

            g_Main.PlayerCleanUp[player].KillTheTimer?.Kill();
            g_Main.PlayerCleanUp[player].KillTheTimer = null!;
            g_Main.PlayerCleanUp.Remove(player);
        }
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

    /* [ConsoleCommand("css_test", "test")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void GetJsonCorrectissoaaan(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null || !player.IsValid) return;
        Server.PrintToConsole($"Remove_BuyMenu: {g_Main.RemoveBuyMenu}");
        Server.PrintToConsole($"Remove_Custom_Point_Server_Command: {g_Main.ForceRemoveServerCommands}");
        Server.PrintToConsole($"Remove_Custom_Point_Client_Command: {g_Main.ForceRemoveClientCommands}");
        Server.PrintToConsole($"Remove_Ground_Weapons: {g_Main.ForceRemoveGroundWeapons}");
        Server.PrintToConsole($"Delay_InXSecs_Give_LoadOuts: {g_Main.DelayGiveLoadOut}");
        Server.PrintToConsole($"Force_Strip_Players: {g_Main.ForceStripPlayers}");      
    } */
}