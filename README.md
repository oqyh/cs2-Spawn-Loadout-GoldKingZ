## .:[ Join Our Discord For Support ]:.

<a href="https://discord.com/invite/U7AuQhu"><img src="https://discord.com/api/guilds/651838917687115806/widget.png?style=banner2"></a>

***
# [CS2] Spawn-Loadout-GoldKingZ (1.0.8)

### Give Weapons On Spawn (Depend The Map Name + Team Side + Vips)

![Untitled](https://github.com/oqyh/cs2-Spawn-Loadout-GoldKingZ/assets/48490385/21912d61-9127-42ed-bee7-0bdf17929769)


## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

## .:[ Configuration ]:.

> [!CAUTION]
> Config Located In ..\addons\counterstrikesharp\plugins\Spawn-Loadout-GoldKingZ\config\config.json                                           
>

```json
{

//---------------------------[ ↓ Spawn Loadout Configs ↓ ]--------------------------------

  //Flags Who Can Reload Weapons_Settings.json (Making Empty "" Means Everyone Has Access)
  "SL_Reload_Weapons_Settings_Flags": "@css/root,@css/admin",

  // Command Ingame To Reload Weapons_Settings.json
  "SL_Reload_Weapons_Settings_CommandsInGame": "!loadouts,!relaodloadouts,!restartloadouts,!loadout,!relaodloadout,!restartloadout",

//----------------------------[ ↓ Utilities ↓ ]----------------------------------------------
  
  //Auto Update Signatures
  "AutoUpdateSignatures": true,

  //Enable Debug Will Print Server Console If You Face Any Issue
  "EnableDebug": false

}
```


## .:[ Weapons Configuration ]:.

> [!CAUTION]
> Weapon Config Located In ..\addons\counterstrikesharp\plugins\Spawn-Loadout-GoldKingZ\config\Weapons_Settings.json                                        

> [!WARNING]
> "ANY" Will Override All Maps, if you make it "awp_" it will give to loadout to any map start with `awp_ `                                          
> Maps Override Path Example //"ANY" > "awp_" > "awp_lego_" > "awp_lego_2"

```json
{
	"ANY"://<Map Name>
	{
		//==========================
		//    Global Options
		//==========================
		// "Force_Strip_CT_Players": true //Force To Strip CT Players [To All LoadOuts] (If false Or Not Been Used This Will Disabled)
		// "Force_Strip_T_Players": true //Force To Strip T Players [To All LoadOuts] (If false Or Not Been Used This Will Disabled)
		// "Remove_BuyMenu": true //Remove Buy Menu (If false Or Not Been Used This Will Disabled)
		// "Remove_Custom_Point_Server_Command": true //Remove point_servercommand Custom ConVars (If false Or Not Been Used This Will Disabled)
		// "Remove_Custom_Point_Client_Command": true //Remove point_clientcommand Custom ConVars (If false Or Not Been Used This Will Disabled)
		// "Players_Health_CT": 35 //Give ALL CT Players Health 35 [To All LoadOuts] (If -1 Or Not Been Used This Will Disabled)
		// "Players_Health_T": 35 //Give ALL T Players Health 35 [To All LoadOuts] (If -1 Or Not Been Used This Will Disabled)
		// "Players_Armor_CT": 35 //Give ALL CT Players Armor 35 [To All LoadOuts] (If -1 Or Not Been Used This Will Disabled)
		// "Players_Armor_T": 35 //Give ALL T Players Armor 35 [To All LoadOuts] (If -1 Or Not Been Used This Will Disabled)
		// "Remove_Ground_Weapons_After_Give_Loadouts": true //Remove Ground Weapons After Give Loadouts (If false Or Not Been Used This Will Disabled)
		// "Delay_InXSecs_Give_LoadOuts": 0.2 //Delay On Give LoadOuts 0.2 Secs (If 0.0 Or Not Been Used This Will Return To 0.0)
		// "Remove_Knife_CT": true //Remove CT Players Knifes [To All LoadOuts] (If false Or Not Been Used This Will Disabled)
		// "Remove_Knife_T": true //Remove T Players Knifes [To All LoadOuts] (If false Or Not Been Used This Will Disabled)
		//==========================

		"LOADOUT_1"://LoadOut 1 <LOADOUT_X>
		{
			//Important Note: If Player Get Loadout 1 Example AK47 and in Loadout 2 You Give AWP LoadOut 1 Will Override LoadOut 2 Because Slot Where Rifle At, is Gived
			//Make Sure Vips/Flags Always LOADOUT_1 To Override LOADOUT_2
			
			//==========================
			// Specific LOADOUT Options
			//==========================
			// "Flags": "@css/root,@css/admin,@css/vip,#css/admin,#css/vip" //Give This LoadOut For These Flags Only (If Empty Or Not Been Used This Will Return To Give Anyone)
			// "CT": "weapon_hkp2000,weapon_knife,weapon_smokegrenade" //CT Loadout
			// "T": "weapon_glock,weapon_knife,weapon_smokegrenade" //T Loadout
			// "CT_Refill_Nades": "weapon_decoy" //Give Auto Refill (ex: decoy) CT Side
			// "CT_Refill_Time_InSec": 30 //Every 30 Secs Give CT_Refill_Nades
			// "T_Refill_Nades": "weapon_decoy" //Give Auto Refill (ex: decoy) T Side
			// "T_Refill_Time_InSec": 30 //Every 30 Secs Give T_Refill_Nades
			// "Force_Strip_CT_Players": true // Force Strip CT Players Who Has This LoadOut (If false Or Not Been Used This Will Disabled)
			// "Force_Strip_T_Players": true // Force Strip T Players Who Has This LoadOut (If false Or Not Been Used This Will Disabled)
			// "Players_Health_CT": 35 //Give ALL CT Players Health 35 Who Has This LoadOut (If -1 Or Not Been Used This Will Disabled)
			// "Players_Health_T": 35 //Give ALL T Players Health 35 Who Has This LoadOut (If -1 Or Not Been Used This Will Disabled)
			// "Players_Armor_CT": 35 //Give ALL CT Players Armor 35 Who Has This LoadOut (If -1 Or Not Been Used This Will Disabled)
			// "Players_Armor_T": 35 //Give ALL T Players Armor 35 Who Has This LoadOut (If -1 Or Not Been Used This Will Disabled)
			// "Remove_Knife_CT": true //Remove CT Players Knifes Who Has This LoadOut (If false Or Not Been Used This Will Disabled)
			// "Remove_Knife_T": true //Remove T Players Knifes Who Has This LoadOut (If false Or Not Been Used This Will Disabled)
			// "Force_Give_This_LoadOut": true // Force Give This LoadOut (By Swaping His Current Weapons) (If false Or Not Been Used This Will Disabled)
			// "Give_This_LoadOut_OneTime": true // Give This LoadOut On Every New Round One Time (If false Or Not Been Used This Will Disabled)
			// "Give_This_LoadOut_On_Round_And_After": 1 // Only Give This LoadOut On Round (0 = Warmup || 1 = Round One etc...) (If -1 Or Not Been Used This Will Disabled)
			//==========================
		},
		"LOADOUT_2"://LoadOut 2
		{
		  "CT": "weapon_hkp2000,weapon_knife,weapon_smokegrenade",
		  "T": "weapon_hkp2000,weapon_knife,weapon_smokegrenade"
		}
		//You Can Add More LOADOUT_X As You Like
	}
}

```


![329846165-ba02c700-8e0b-4ebe-bc28-103b796c0b2e](https://github.com/oqyh/cs2-Game-Manager/assets/48490385/3df7caa9-34a7-47da-94aa-8d682f59e85d)


## .:[ Language ]:.
```json
{
	//==========================
	//        Colors
	//==========================
	//{Yellow} {Gold} {Silver} {Blue} {DarkBlue} {BlueGrey} {Magenta} {LightRed}
	//{LightBlue} {Olive} {Lime} {Red} {Purple} {Grey}
	//{Default} {White} {Darkred} {Green} {LightYellow}
	//==========================
	//        Other
	//==========================
	//{nextline} = Print On Next Line
	//==========================
	
	"PrintChatToPlayer.Not.Allowed.ToReload": "{green}Gold KingZ {grey}| {darkred}You Dont Have Access To This Command.",
	"PrintChatToPlayer.Plugin.Reloaded": "{green}Gold KingZ {grey}| {lime}LoadOut Plugin Reloaded!",
	"PrintChatToPlayer.Got.Loadout": "{green}Gold KingZ {grey}| You Already Got Loadout, You Will Not Be Given This Round."
}
```


> [!TIP]
> You Can Find Weapons List In https://developer.valvesoftware.com/wiki/Counter-Strike_2/Weapons                                        
>

```
Primary
===============
weapon_ak47
weapon_aug
weapon_awp
weapon_famas
weapon_g3sg1
weapon_galilar
weapon_m249
weapon_m4a1
weapon_m4a1_silencer
weapon_mag7
weapon_mp5sd
weapon_mp7
weapon_mp9
weapon_negev
weapon_nova
weapon_p90
weapon_sawedoff
weapon_scar20
weapon_sg556
weapon_ssg08
weapon_mac10
weapon_ump45
weapon_xm1014
weapon_bizon
--------------------------


Secondary
=============== 
weapon_cz75a
weapon_deagle
weapon_elite
weapon_fiveseven
weapon_glock
weapon_hkp2000
weapon_p250
weapon_revolver
weapon_tec9
weapon_usp_silencer
--------------------------


Grenade
===============
weapon_decoy
weapon_flashbang
weapon_hegrenade
weapon_incgrenade
weapon_molotov
weapon_smokegrenade
--------------------------


Equipment
===============
item_kevlar //Kevlar Vest
item_assaultsuit //Kevlar + Helmet
weapon_taser
item_cutters //Rescue Kit
item_defuser //Defuse Kit
--------------------------


Miscellaneous
===============
weapon_c4
weapon_healthshot
weapon_shield
item_heavyassaultsuit
weapon_snowball
weapon_knife
weapon_knife_t
--------------------------
```

## .:[ Change Log ]:.
```
(1.0.8)
-Rework On Plugin
-Fix On Giving Weapons
-Added Players_Armor_CT
-Added Players_Armor_T
-Added Players_Health_CT
-Added Players_Health_T
-Added Remove_Knife_CT
-Added Remove_Knife_T
-Added Force_Strip_CT_Players
-Added Force_Strip_T_Players
-Added Force_Give_This_LoadOut
-Added Give_This_LoadOut_On_Round_And_After

(1.0.7)
-Update GiveNamedItem2
-Added AutoUpdateSignatures Into Config
-Added Players_Health
-Added Players_Armor


(1.0.6)
-Rework Method On Giving And Removing Weapons
-Fix Some Bugs
-Fix Crashs On Giving Loadouts
-Fix Force_Strip_Players
-Fix Delay Give Loadouts
-Added Remove_BuyMenu
-Added Remove_Knife
-Added Remove_Custom_Point_Server_Command
-Added Remove_Custom_Point_Client_Command
-Added Give_This_LoadOut_PerRound_Only
-Added Delay_InXSecs_Give_LoadOuts
-Added Custom Gamedata In The Plugin it Self

(1.0.5)
-Fix ForceStripPlayers For Windows And Lunix

(1.0.4)
-Fix ForceStripPlayers

(1.0.3)
-Fix ForceStripPlayers

(1.0.2)
-Rework Loadout Method 
-Deleted config.json 
-Removed GiveOneTimeLoadOutPerRound
-Removed GiveOneTimeRefillNadesPerRound
-Removed Vips_GiveOneTimeLoadOutPerRound
-Removed Vips_GiveOneTimeRefillNadesPerRound
-Removed Vips
-Added ForceStripPlayers
-Added DeleteGroundWeapons
-Added LOADOUT_X Many Loadout as You Like
-Added FLAGS
-Added GiveThisLoadOutPerRoundOnly

(1.0.1)
-Upgrade Net.7 To Net.8
-Fix Some Bugs 
-Fixed GiveOneTimePerRound
-Added GiveOneTimeLoadOutPerRound
-Added GiveOneTimeRefillNadesPerRound
-Added Vips_GiveOneTimeLoadOutPerRound
-Added Vips_GiveOneTimeRefillNadesPerRound
-Added Refill_Nades
-Added Refill_Time_InSec

(1.0.0)
-Initial Release
```

## .:[ Donation ]:.

If this project help you reduce time to develop, you can give me a cup of coffee :)

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/oQYh)
