# [CS2] Spawn-Loadout-GoldKingZ (1.0.4)

### Give Weapons On Spawn (Depend The Map Name + Team Side + Vips)

![Untitled](https://github.com/oqyh/cs2-Spawn-Loadout-GoldKingZ/assets/48490385/21912d61-9127-42ed-bee7-0bdf17929769)


## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)


## .:[ Weapons Configuration ]:.

> [!CAUTION]
> Weapon Config Located In ..\addons\counterstrikesharp\plugins\Spawn-Loadout-GoldKingZ\config\weapons.json                                         

> [!WARNING]
> "ANY" Will Override All Maps, if you make it "awp_" it will give to loadout to any map start with `awp_ `                                          
> Maps Override Path Example //"ANY" > "awp_" > "awp_lego_" > "awp_lego_2"

```json

{
	"ANY"://<Map Name>
	{
		"ForceStripPlayers": true, //Force Strip All Weapons From Player Before You Give LoadOuts (Default Is False Or If Not Used It Will Set False)
		"DeleteGroundWeapons": true, //Delete Ground Weapons After Give LoadOuts (Default Is False Or If Not Used It Will Set False)
		"LOADOUT_1"://LoadOut 1 <LOADOUT_X>
		{
			//Important Note: If Player Get Loadout 1 Example AK47 and in Loadout 2 You Give AWP LoadOut 1 Will Override LoadOut 2 Because Slot Where Rifle At is Gived
			//Make Sure Vips/Flags Always LOADOUT_1 To Override LOADOUT_2
			
			"GiveThisLoadOutPerRoundOnly": true, //Give LOADOUT_1 This Round ONLY, The Next Spawn Will Not Get LOADOUT_1 Until Start New Round (Default Is False Or If Not Used It Will Set False)
			
			"FLAGS": "@css/root,@css/admin,@css/vip,#css/admin,#css/vip", //Flags Add Many As You Like (Not Using It Or Making It ["FLAGS": ""] Empty Means Give LOADOUT_1 To Everyone)
			
			"CT": "weapon_taser,weapon_decoy", //CT Loadout
			"T": "weapon_taser,weapon_decoy", //T Loadout
			
			"CT_Refill_Nades": "weapon_decoy", //Give Auto Refill (ex: decoy) CT Side
			"CT_Refill_Time_InSec": 30, //Every 30 Secs Give (ex: decoy)
			
			"T_Refill_Nades": "weapon_decoy", //Give Auto Refill (ex: decoy) T Side
			"T_Refill_Time_InSec": 30 //Every 30 Secs Give (ex: decoy)
		},
		"LOADOUT_2"://LoadOut 2
		{
		  "CT": "weapon_hkp2000,weapon_knife,weapon_smokegrenade",
		  "T": "weapon_hkp2000,weapon_knife,weapon_smokegrenade"
		}
		//You Can Add More LOADOUT_X
	}
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
weapon_knife
weapon_knife_t
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
--------------------------
```

## .:[ Change Log ]:.
```
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
