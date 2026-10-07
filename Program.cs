using static SBEnemyRandomizer.src.GlobalSettings;
using static SBEnemyRandomizer.src.Func;
using static SBEnemyRandomizer.src.Logger;
using SBEnemyRandomizer.Avalonia;
using Avalonia;
using Avalonia.Controls;

namespace SBEnemyRandomizer;

sealed class Program{

    [STAThread]
    async static Task Main(string[] args)
    {
        // CLI execution
        // dotnet run -- -cli
        // SBEnemyRandomizer.exe -cli -seed=123
        if(args.Length > 0) {
            bool cli = false;
            foreach(string a in args)
            {
                if(a == "-cli") cli = true;
                if(a.Contains("-seed="))
                {
                    if(a.Length <= 6) {
                        Log("Error: No seed provided");
                        return;
                    }
                    seed = int.Parse(a.Split("=")[1]);
                }
            }
            if(!cli) return;
            await Randomizer();
            return;
        }

        // UI
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
    
    // Avalonia configuration
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        #if DEBUG
        .WithDeveloperTools()
        #endif
        .WithInterFont()
        .LogToTrace();

}