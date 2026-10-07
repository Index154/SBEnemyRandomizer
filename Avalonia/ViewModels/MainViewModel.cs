using CommunityToolkit.Mvvm.ComponentModel;

namespace SBEnemyRandomizer.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public int _seed = 0;
}