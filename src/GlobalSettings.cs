namespace SBEnemyRandomizer.src;

using static SBEnemyRandomizer.src.GameData;

public class GlobalSettings
{
    // User settings
    // -------------------------------------------------------------------------
    #if DEBUG
    public static string gamePath = "D:/Steam/steamapps/common/StellarBlade";   // game folder path for cli testing
    #else
    public static string gamePath = "";
    #endif
    public static int seed = 112233;
    public static bool shuffleNPCAppearances = false;
    public static bool randomEnemySizes = false;
    public static bool resetAllEnemySpawnsOnLoad = false;
    public static string forceReplacementBossName = "";
    public static string forceReplacementEnemyName = "";
    public static float enemyHPMultiplier = 1.0F;
    public static float enemyAttackMultiplier = 1.0F;
    // Testing settings
    public static string forceReplacementForEvent = "";
    public static bool blockAIForTesting = false;
    public static string customZoneNameRestriction = "";
    public static EnemyRank? customRankRestriction = null;
    public static bool replaceSaveDataOnRun = false;
    public static bool keepVanilla = false;
    // Unrelated QoL settings
    public static float fishingPowerMultiplier = 6.0F;
    public static float hiveHPMultiplier = 0.4F;


    // Which rank of enemy should an enemy of a given rank be changed to? (disabled ones are currently unimplemented)
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
    public static bool shuffleBosses = true;
    public static bool allowGunBossesBeforeXion = false;
    public static bool reduceDestroyableProjectileSoftlocks = true;
    public static bool tryPlaceDuplicatesForAvoidableFights = true;
    // Add non-story bosses to the pool
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
    public static readonly string mapPath = Path.Combine(basePath, "tools/StellarBlade_1.4.1.usmap");
    public static readonly string logPath = Path.Combine(basePath, "logs");
    // Temp folders. The subdirectory of the modified assets must match how they were extracted
    public static readonly string unpackTablePath = $"{tempPath}/unpacked/{tableSubdirectory}";
    public static readonly string unpackAIPath = $"{tempPath}/unpacked/{aiSubdirectory}";
    public static readonly string repackTablePath = $"{tempPath}/modified/{tableSubdirectory}";
    public static readonly string repackAIPath = $"{tempPath}/modified/{aiSubdirectory}";
    // Assets
    public static readonly List<string> assetNames = [
        "CharacterTable",
        "EventSpawnTable",
        "EventNewGamePlusSpawnTable",
        "LevelTargetFilter",
        "EventActorEffectTable",
        "ConditionTable",
        "EffectTable",
        "CharacterMoveTable",
        "SkillActiveStepTable",
        "ZoneEventTable",
        "ZoneTriggerTable",
        "EventTheaterTable",
        "M_Tachy_AI"
    ];
}