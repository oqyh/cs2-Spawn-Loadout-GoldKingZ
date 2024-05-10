using CounterStrikeSharp.API.Core;

namespace Spawn_Loadout_GoldKingZ;

public class Globals
{
    public static Dictionary<ulong, bool> VipsFlag = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> Gived = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> NadeGived = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VipGived = new Dictionary<ulong, bool>();
    public static Dictionary<ulong, bool> VipNadeGived = new Dictionary<ulong, bool>();
	public static string SMapName => NativeAPI.GetMapName();
}