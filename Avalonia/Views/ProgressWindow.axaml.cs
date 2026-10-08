using Avalonia.Controls;
using SBEnemyRandomizer.Avalonia.ViewModels;

namespace SBEnemyRandomizer.Avalonia.Views;

public partial class ProgressWindow : Window
{
    private CancellationTokenSource? _cts;

    public ProgressWindow()
    {
        InitializeComponent();
        DataContext = new ProgressViewModel();
        Opened += OnOpened;
        Closed += OnClosed;
        CanMaximize = false;
        CanMinimize = false;
        CanResize = false;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        _cts = new CancellationTokenSource();
        if(DataContext is ProgressViewModel vm) await vm.Start(_cts.Token);
        Close(true);
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }
}