using CommunityToolkit.Mvvm.ComponentModel;
using static SBEnemyRandomizer.src.GlobalSettings;

namespace SBEnemyRandomizer.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // Causes the UI value to update automatically when the value is changed
    [ObservableProperty]
    public int _seed = seed;

    partial void OnSeedChanged(int oldValue, int newValue)
    {  
        // Update global variable on UI input change. Required so the change is saved to the json on close without first having to press the Randomize button
        seed = _seed;
    }
}