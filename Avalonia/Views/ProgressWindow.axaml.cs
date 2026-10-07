using Avalonia.Controls;
using SBEnemyRandomizer.Avalonia.ViewModels;

namespace SBEnemyRandomizer.Avalonia.Views;

public partial class ProgressWindow : Window
{
    public ProgressWindow()
    {
        InitializeComponent();
        DataContext = new ProgressViewModel();
        Opened += OnOpened;
        CanMaximize = false;
        CanMinimize = false;
        CanResize = false;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        if(DataContext is ProgressViewModel vm) await vm.Start();
        Close(true);
    }
}