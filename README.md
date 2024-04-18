# [CS2] Spawn-Loadout-GoldKingZ (1.0.0)

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
  //Give Weapon Per Round
  "GiveOneTimePerRound": false,
  
  //Flags To Give CT_Vip Or T_Vip
  "Vips": "@css/root,@css/admin,@css/vip,#css/admin,#css/vip",

}
```

## .:[ Weapons Configuration ]:.

> [!CAUTION]
> Weapon Config Located In ..\addons\counterstrikesharp\plugins\Spawn-Loadout-GoldKingZ\Weapons\Weapons.json                                           

> [!WARNING]
> "ANY" Will Override All Maps, if you make it "awp_" it will give to loadout to any map start with `awp_ `                                          
> Maps Override Path Example //"ANY" > "awp_" > "awp_lego_" > "awp_lego_2"

```json
{
  "ANY": { //ANY > awp_ > awp_lego_ > awp_lego_2
    "CT": "weapon_hkp2000,weapon_knife",
    "CT_Vip": "weapon_taser,weapon_smokegrenade", //Assign In config.json "Vips"
    "T": "weapon_glock,weapon_knife",
    "T_Vip": "weapon_taser,weapon_smokegrenade"
  },
  "de_": {
    "CT": "weapon_ssg08,weapon_deagle",
    "T": "weapon_ssg08,weapon_deagle"
  },
  "awp_lego_2": {
    "CT": "weapon_awp,weapon_deagle",
    "CT_Vip": "weapon_taser",
    "T": "weapon_awp,weapon_deagle"
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
(1.0.0)
-Initial Release
```

## .:[ Donation ]:.

If this project help you reduce time to develop, you can give me a cup of coffee :)

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/oQYh)
