using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using SBEnemyRandomizer.src;
using static SBEnemyRandomizer.src.GlobalSettings;

namespace SBEnemyRandomizer.Avalonia.Views;

public partial class MainWindow : Window
{
    private readonly SettingsService _settings;

    public MainWindow()
    {
        InitializeComponent();
        _settings = new SettingsService();
        RestoreSettings();
    }

    private void RestoreSettings()
    {
        var s = _settings.Load();

        seed = s.Seed;
        gamePath = s.GamePath;

        shuffleBosses = s.BossShuffle;
        includeMann = s.AddMann;
        includeScarlet = s.AddScarlet;
        tryPlaceDuplicatesForAvoidableFights = s.BossDupesInOptionalEncounters;
        allowGunBossesBeforeXion = s.GunBossesBeforeXion;
        reduceDestroyableProjectileSoftlocks = s.ProjectileSoftlockReduction;

        resetAllEnemySpawnsOnLoad = s.ResetSpawnsOnZoneReset;
        enemyHPMultiplier = s.EnemyHPMultiplier;
        enemyAttackMultiplier = s.EnemyAttackMultiplier;
        randomEnemySizes = s.RandomEnemySize;
        shuffleNPCAppearances = s.ShuffleNPCAppearances;
        fishingPowerMultiplier = s.FishingPowerMultiplier;
        hiveHPMultiplier = s.HiveHPMultiplier;
        
        if (s.WindowWidth > 0 && s.WindowHeight > 0)
        {
            Width = s.WindowWidth;
            Height = s.WindowHeight;
        }
        if (s.WindowX >= 0 && s.WindowY >= 0)
        {
            int totalScreenWidth = 0;
            int totalScreenHeight = 0;
            foreach(Screen sc in Screens.All)
            {
                totalScreenWidth += sc.Bounds.BottomRight.X;
                totalScreenHeight = sc.Bounds.BottomRight.Y;
            }
            if(s.WindowX >= totalScreenWidth || s.WindowY >= totalScreenHeight) return;
            Position = new PixelPoint(s.WindowX, s.WindowY);
        }
    }
    
    public async void RandomizeOnClick(object sender, RoutedEventArgs args)
    {
        // Ask for folder path if it's still unknown
        bool folderFound = gamePath != "";
        if(!folderFound)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions(){
                Title = "Select SB.exe in game installation folder (...\\steamapps\\common\\StellarBlade)",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("SB"){
                    Patterns = ["SB.exe"]
                }]
            });
            if(files.Count != 1)
            {
                RandomizerResult.Text = "Game path missing!";
            }
            else
            {
                string folder = files[0].Path.ToString().Split("/SB.exe")[0].Split("///")[1];
                Console.WriteLine(folder);
                gamePath = folder;
                folderFound = true;
            }
        }

        // Create progress window and wait for it to close. Display success popup only if it returned true
        if(folderFound)
        {
            var progress = new ProgressWindow(){
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            bool status = await progress.ShowDialog<bool>(this);
            if(!status) RandomizerResult.Text = "Aborted!";
        }
        FinishedNotification.IsOpen = true;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        var s = _settings.Load();
        s.WindowWidth = Width;
        s.WindowHeight = Height;
        s.WindowX = Position.X;
        s.WindowY = Position.Y;
        _settings.Save(s);
        base.OnClosing(e);
    }
}