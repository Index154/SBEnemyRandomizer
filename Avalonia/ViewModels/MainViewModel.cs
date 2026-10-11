using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SBEnemyRandomizer.src;
using static SBEnemyRandomizer.src.GlobalSettings;

namespace SBEnemyRandomizer.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private SettingsService _settings;

    [ObservableProperty]
    public int _seed = seed;
    [ObservableProperty]
    public bool _bossShuffle = shuffleBosses;
    [ObservableProperty]
    public bool _addMann = includeMann;
    [ObservableProperty]
    public bool _addScarlet = includeScarlet;
    [ObservableProperty]
    public bool _preventGunBossesBeforeXion = preventGunBossesBeforeXion;
    [ObservableProperty]
    public bool _resetSpawnsOnZoneReset = resetAllEnemySpawnsOnLoad;
    [ObservableProperty]
    public bool _shuffleNPCAppearances = shuffleNPCAppearances;
    [ObservableProperty]
    public float _enemyHPMultiplier = enemyHPMultiplier;
    [ObservableProperty]
    public float _enemyAttackMultiplier = enemyAttackMultiplier;
    [ObservableProperty]
    public float _fishingPowerMultiplier = fishingPowerMultiplier;
    [ObservableProperty]
    public float _hiveHPMultiplier = hiveHPMultiplier;

    public MainViewModel()
    {
        PropertyChanged += OnAnyPropertyChanged;
    }

    private void OnAnyPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Necessary in order for the config to be updated upon closing the window without first pressing the randomize button
        switch(e.PropertyName)
        {
            case nameof(Seed):
                seed = Seed;
                break;
            case nameof(BossShuffle):
                shuffleBosses = BossShuffle;
                break;
            case nameof(AddMann):
                includeMann = AddMann;
                break;
            case nameof(AddScarlet):
                includeScarlet = AddScarlet;
                break;
            case nameof(PreventGunBossesBeforeXion):
                preventGunBossesBeforeXion = PreventGunBossesBeforeXion;
                break;
            case nameof(ResetSpawnsOnZoneReset):
                resetAllEnemySpawnsOnLoad = ResetSpawnsOnZoneReset;
                break;
            case nameof(ShuffleNPCAppearances):
                shuffleNPCAppearances = ShuffleNPCAppearances;
                break;
            case nameof(EnemyHPMultiplier):
                enemyHPMultiplier = EnemyHPMultiplier;
                break;
            case nameof(EnemyAttackMultiplier):
                enemyAttackMultiplier = EnemyAttackMultiplier;
                break;
            case nameof(FishingPowerMultiplier):
                fishingPowerMultiplier = FishingPowerMultiplier;
                break;
            case nameof(HiveHPMultiplier):
                hiveHPMultiplier = HiveHPMultiplier;
                break;
        }
    }

    private void SaveConfig()
    {
        _settings = new SettingsService();
        var s = _settings.Load();
        _settings.Save(s);
    }
}