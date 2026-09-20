namespace SBEnemyRandomizer.src;

public class GlobalSettings
{
    // Settings provided by the user through GUI (unimplemented)
    public static string gamePath = "D:/Steam/steamapps/common/StellarBlade/SB/Content/Paks";
    public static int seed = 12414321;
    // Which rank of enemy should an enemy of a given rank be replaceable by?
    public static bool smallToSmall = true;
    public static bool smallToNormal = false;
    public static bool smallToBoss = false;
    public static bool normalToSmall = false;
    public static bool normalToNormal = true;
    public static bool normalToBoss = false;
    public static bool bossToSmall = false;
    public static bool bossToNormal = false;
    public static bool bossToBoss = true;
    // Keep the number of enemies per type the same across the game (true shuffle) or make them fully random? Prevents duplicate bosses aside from the ones already in the game
    public static bool onlyShuffleExistingRegular = false;
    public static bool onlyShuffleExistingBoss = true;
    // Add non-story bosses to the pool
    public static bool includeMann = true;
    public static bool includeScarlet = true;
    // Replace all instances of an enemy with the same target enemy or replace them all independently? (Does not work with onlyShuffleExisting)
    public static bool consistentReplacements = false;

    public static readonly string assetSubdirectory = "SB/Content/Local/Data";
    public static readonly string tempPath = "./temp";
    public static readonly string retocPath = "./retoc/retoc.exe";
    public static readonly string mapPath = "./maps/StellarBlade_1.1.0.usmap";
    public static readonly string eventSpawnTable = "EventSpawnTable.uasset";
    public static readonly string characterTable = "CharacterTable.uasset";
}
