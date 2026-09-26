namespace SBEnemyRandomizer.src;

using static SBEnemyRandomizer.src.GameData;

public class GlobalSettings
{
    // Settings provided by the user
    // -------------------------------------------------------------------------
    public static string gamePath = "D:/Steam/steamapps/common/StellarBlade";   // Tell user: Select the folder with SB.exe
    public static int seed = 123456;
    public static bool randomizeNPCAppearances = true;
    public static string forceReplacementEnemyName = "";
    public static string forceReplacementForEvent = "";
    public static bool resetAllEnemySpawnsOnLoad = false;
    // Testing settings
    public static int enemyHPForTesting = 0;
    public static string customZoneNameRestriction = "";
    public static EnemyRank? customRankRestriction = null;
    public static bool replaceSaveDataOnRun = false;

    // Which rank of enemy should an enemy of a given rank be changed to? (logic unimplemented)
    public static bool smallToSmall = true;
    public static bool smallToNormal = false;
    public static bool smallToBoss = false;
    public static bool normalToSmall = false;
    public static bool normalToNormal = true;
    public static bool normalToBoss = false;
    public static bool bossToSmall = false;
    public static bool bossToNormal = false;
    public static bool bossToBoss = true;

    // Shuffle bosses around or select them completely randomly
    public static bool onlyShuffleExistingBoss = true;
    // Add non-story bosses to the pool (logic unimplemented)
    public static bool includeMann = true;
    public static bool includeScarlet = true;

    // Incremental IDs for cloned table rows
    public static uint incrementalID = 990000000;
    public static uint incrementalEffectID = 9000000;
    public static uint incrementalEventActorEffectID = 10000;

    // Constants
    // -------------------------------------------------------------------------
    #if DEBUG
    public static readonly string basePath = Directory.GetCurrentDirectory();
    #else
    public static readonly string basePath = AppContext.BaseDirectory;
    #endif
    public static readonly string gamePakPath = $"{gamePath}/SB/Content/Paks";
    public static readonly string tableSubdirectory = "SB/Content/Local/Data";
    public static readonly string aiSubdirectory = "SB/Content/GameDesign/Combat/BehaviorTree/Monster";
    // Paths dependent on the basePath
    public static readonly string tempPath = Path.Combine(basePath, "temp");
    public static readonly string retocPath = Path.Combine(basePath, "tools/retoc/retoc.exe");
    public static readonly string mapPath = Path.Combine(basePath, "tools/StellarBlade_1.1.0.usmap");
    public static readonly string logPath = Path.Combine(basePath, "logs");
    // Temp folders. The subdirectory of the modified assets must match how they were extracted
    public static readonly string unpackTablePath = $"{tempPath}/unpacked/{tableSubdirectory}";
    public static readonly string unpackAIPath = $"{tempPath}/unpacked/{aiSubdirectory}";
    public static readonly string repackTablePath = $"{tempPath}/modified/{tableSubdirectory}";
    public static readonly string repackAIPath = $"{tempPath}/modified/{aiSubdirectory}";
    // Assets
    public static readonly string characterTable = "CharacterTable.uasset";
    public static readonly string eventSpawnTable = "EventSpawnTable.uasset";
    public static readonly string levelTargetFilterTable = "LevelTargetFilter.uasset";
    public static readonly string eventActorEffectTable = "EventActorEffectTable.uasset";
    public static readonly string conditionTable = "ConditionTable.uasset";
    public static readonly string effectTable = "EffectTable.uasset";
    public static readonly string characterMoveTable = "CharacterMoveTable.uasset";
    public static readonly string skillActiveStepTable = "SkillActiveStepTable.uasset";
    public static readonly string zoneEventTable = "ZoneEventTable.uasset";
    public static readonly string tachyAI = "M_Tachy_AI.uasset";
}