using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spawn_Loadout_GoldKingZ.Config
{
    public static class Configs
    {
        public static class Shared {
            public static string? CookiesModule { get; set; }
            public static CustomGameData? CustomFunctions { get; set; }
        }
    }
}
