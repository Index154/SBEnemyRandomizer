using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
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
}