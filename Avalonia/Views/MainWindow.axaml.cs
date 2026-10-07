using Avalonia.Controls;
using Avalonia.Interactivity;
using static SBEnemyRandomizer.src.GlobalSettings;

namespace SBEnemyRandomizer.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    // Button press function
    public async void RandomizeOnClick(object sender, RoutedEventArgs args)
    {
        if(SeedInput.Value != null) seed = (int)SeedInput.Value;

        // Create progress window and wait for it to close. Display success popup only if it returned true
        var progress = new ProgressWindow(){
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        bool status = await progress.ShowDialog<bool>(this);
        if(!status) RandomizerResult.Text = "Aborted!";
        FinishedNotification.IsOpen = true;
    }
}