using CommunityToolkit.Mvvm.ComponentModel;
using static SBEnemyRandomizer.src.Func;

namespace SBEnemyRandomizer.Avalonia.ViewModels;

public partial class ProgressViewModel : ViewModelBase
{
    [ObservableProperty]
    public int _progress = 0;

    public async Task Start(CancellationToken token)
    {
        Progress = 0;
        var p = new Progress<int>( value => {Progress = value;} );
        await Task.Run(() => Randomizer(p, token));
        Console.WriteLine("Stopped randomizing");
    }
}