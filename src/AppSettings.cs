using System.Text.Json;
using static SBEnemyRandomizer.src.GlobalSettings;

namespace SBEnemyRandomizer.src;

public class AppSettings
{
    public string GamePath { get; set; } = "";
    public int Seed { get; set; } = 0;
    public bool BossShuffle { get; set; } = true;
    public bool AddMann { get; set; } = true;
    public bool AddScarlet { get; set; } = false;
    public bool BossDupesInOptionalEncounters { get; set; } = true;
    public bool GunBossesBeforeXion { get; set; } = false;
    public bool ProjectileSoftlockReduction { get; set; } = true;
    public bool SmallToSmall { get; set; } = true;
    public bool SmallToNormal { get; set; } = false;
    public bool SmallToBoss { get; set; } = false;
    public bool NormalToSmall { get; set; } = false;
    public bool NormalToNormal { get; set; } = true;
    public bool NormalToBoss { get; set; } = false;
    public bool BossToSmall { get; set; } = false;
    public bool BossToNormal { get; set; } = false;
    public bool BossToBoss { get; set; } = true;
    public bool ResetSpawnsOnZoneReset { get; set; } = false;
    public float EnemyHPMultiplier { get; set; } = 1.0F;
    public float EnemyAttackMultiplier { get; set; } = 1.0F;
    public bool ShuffleNPCAppearances { get; set; } = false;
    public float FishingPowerMultiplier { get; set; } = 1.0F;
    public float HiveHPMultiplier { get; set; } = 1.0F;
    public double WindowWidth { get; set; } = 600;
    public double WindowHeight { get; set; } = 700;
    public int WindowX { get; set; } = -1;
    public int WindowY { get; set; } = -1;
}

public class SettingsService
{
    private static readonly string settingsPath = Path.Combine(basePath, "config.json");

    public AppSettings Load()
    {
        if (!File.Exists(settingsPath)) return new AppSettings();

        var json = File.ReadAllText(settingsPath);
        AppSettings settings;
        try{
            settings = JsonSerializer.Deserialize<AppSettings>(json);
        }catch{
            settings = new AppSettings();
        }
        return settings;
    }

    public void Save(AppSettings settings)
    {
        settings.GamePath = gamePath;
        settings.Seed = seed;

        settings.BossShuffle = shuffleBosses;
        settings.AddMann = includeMann;
        settings.AddScarlet = includeScarlet;
        settings.BossDupesInOptionalEncounters = tryPlaceDuplicatesForAvoidableFights;
        settings.GunBossesBeforeXion = allowGunBossesBeforeXion;
        settings.ProjectileSoftlockReduction = reduceDestroyableProjectileSoftlocks;

        settings.ResetSpawnsOnZoneReset = resetAllEnemySpawnsOnLoad;
        settings.EnemyHPMultiplier = enemyHPMultiplier;
        settings.EnemyAttackMultiplier = enemyAttackMultiplier;
        settings.ShuffleNPCAppearances = shuffleNPCAppearances;
        settings.FishingPowerMultiplier = fishingPowerMultiplier;
        settings.HiveHPMultiplier = hiveHPMultiplier;

        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(settingsPath, json);
    }
}