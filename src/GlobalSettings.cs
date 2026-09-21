namespace SBEnemyRandomizer.src;

public class GlobalSettings
{
    // Settings provided by the user
    // -------------------------------------------------------------------------
    public static string gamePath = "D:/Steam/steamapps/common/StellarBlade/SB/Content/Paks";
    public static int seed = 123456;
    public static bool randomizeNPCAppearances = true;
    public static string forceReplacementEnemyName = "";

    // Which rank of enemy should an enemy of a given rank be changed to?
    public static bool smallToSmall = true;
    public static bool smallToNormal = false;
    public static bool smallToBoss = false;
    public static bool normalToSmall = false;
    public static bool normalToNormal = true;
    public static bool normalToBoss = false;
    public static bool bossToSmall = false;
    public static bool bossToNormal = false;
    public static bool bossToBoss = true;

    // Keep the number of enemies per type the same across the game (true shuffle) or make them fully random? Prevents extra duplicate bosses
    public static bool onlyShuffleExistingRegular = false;
    public static bool onlyShuffleExistingBoss = true;
    // Add non-story bosses to the pool (unimplemented)
    public static bool includeMann = true;
    public static bool includeScarlet = true;
    // Replace all instances of an enemy with the same target enemy or replace them all independently? (Does not work with onlyShuffleExisting)
    public static bool consistentReplacements = false;

    // Constant paths
    // -------------------------------------------------------------------------
    #if DEBUG
    public static readonly string basePath = Directory.GetCurrentDirectory();
    #else
    public static readonly string basePath = AppContext.BaseDirectory;
    #endif
    public static readonly string assetSubdirectory = "SB/Content/Local/Data";
    public static readonly string tempPath = Path.Combine(basePath, "temp");
    public static readonly string retocPath = Path.Combine(basePath, "tools/retoc/retoc.exe");
    public static readonly string mapPath = Path.Combine(basePath, "tools/maps/StellarBlade_1.1.0.usmap");
    public static readonly string logPath = Path.Combine(basePath, "logs");
    public static readonly string unpackPath = $"{tempPath}/unpacked/{assetSubdirectory}";
    public static readonly string repackPath = $"{tempPath}/modified/{assetSubdirectory}";
    public static readonly string eventSpawnTable = "EventSpawnTable.uasset";
    public static readonly string characterTable = "CharacterTable.uasset";
}
