namespace SBEnemyRandomizer.src;

using static SBEnemyRandomizer.src.GameData;

public class GlobalSettings
{
    // Settings provided by the user
    // -------------------------------------------------------------------------
    public static string gamePath = "D:/Steam/steamapps/common/StellarBlade/SB/Content/Paks";
    public static int seed = 123456;
    public static bool randomizeNPCAppearances = true;
    public static string forceReplacementEnemyName = "SE_M_Crawler_01";
    // Testing settings
    public static bool lowerEnemyHPForTesting = true;
    public static bool resetAllEnemySpawnsOnLoad = true;
    public static string customZoneNameRestriction = "";
    public static EnemyRank? customRankRestriction = EnemyRank.Boss;
    public static bool replaceSaveDataOnRun = true;

    // Which rank of enemy should an enemy of a given rank be changed to? (unimplemented)
    public static bool smallToSmall = true;
    public static bool smallToNormal = false;
    public static bool smallToBoss = false;
    public static bool normalToSmall = false;
    public static bool normalToNormal = true;
    public static bool normalToBoss = false;
    public static bool bossToSmall = false;
    public static bool bossToNormal = false;
    public static bool bossToBoss = true;

    // Prevent duplicate bosses (shuffle) or randomize them fully
    public static bool onlyShuffleExistingBoss = true;
    // Add non-story bosses to the pool (unimplemented)
    public static bool includeMann = true;
    public static bool includeScarlet = true;
    // Replace all non-boss enemies of the same type consistently or replace them all independently? (unimplemented)
    public static bool consistentReplacements = false;

    // Constant paths
    // -------------------------------------------------------------------------
    #if DEBUG
    public static readonly string basePath = Directory.GetCurrentDirectory();
    #else
    public static readonly string basePath = AppContext.BaseDirectory;
    #endif
    public static readonly string tableSubdirectory = "SB/Content/Local/Data";
    public static readonly string aiSubdirectory = "SB/Content/GameDesign/Combat/BehaviorTree/Monster";
    public static readonly string tempPath = Path.Combine(basePath, "temp");
    public static readonly string retocPath = Path.Combine(basePath, "tools/retoc/retoc.exe");
    public static readonly string mapPath = Path.Combine(basePath, "tools/StellarBlade_1.1.0.usmap");
    public static readonly string logPath = Path.Combine(basePath, "logs");
    public static readonly string unpackTablePath = $"{tempPath}/unpacked/{tableSubdirectory}";
    public static readonly string repackTablePath = $"{tempPath}/modified/{tableSubdirectory}";
    public static readonly string unpackAIPath = $"{tempPath}/unpacked/{aiSubdirectory}";
    public static readonly string repackAIPath = $"{tempPath}/modified/{aiSubdirectory}";
    public static readonly string eventSpawnTable = "EventSpawnTable.uasset";
    public static readonly string characterTable = "CharacterTable.uasset";
    public static readonly string skillActiveStepTable = "SkillActiveStepTable.uasset";
    public static readonly string characterMoveTable = "CharacterMoveTable.uasset";
    public static readonly string tachyAI = "M_Tachy_AI.uasset";
}