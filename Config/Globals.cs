using CounterStrikeSharp.API.Core;

namespace Spawn_Loadout_GoldKingZ;

public class Globals
{
    public static Dictionary<ulong, HashSet<string>> loadoutsGivenPerPlayer = new Dictionary<ulong, HashSet<string>>();
	public static string SMapName => NativeAPI.GetMapName();
}