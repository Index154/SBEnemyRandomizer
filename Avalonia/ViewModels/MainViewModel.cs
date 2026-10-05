using CommunityToolkit.Mvvm.ComponentModel;

namespace SBEnemyRandomizer.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Enemy Randomizer";
}
