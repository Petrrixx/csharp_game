using Raylib_cs;
using VampireSurvivorsClone.Engine;
using VampireSurvivorsClone.UI;
using VampireSurvivorsClone.Data;
using VampireSurvivorsClone;
using System.IO;
using System.Text.Json;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            // Set up the game window and default values
            Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT);
            Raylib.SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT);
            {
                // Default values
                var screenWidth = 1280;
                var screenHeight = 720;
                var difficulty = Difficulty.Normal;
                bool fullscreen = false;

                // Parse args
                if (args.Length >= 1)
                    Enum.TryParse(args[0], true, out difficulty);
                if (args.Length >= 3)
                {
                    int.TryParse(args[1], out screenWidth);
                    int.TryParse(args[2], out screenHeight);
                }
                if (args.Length >= 4)
                    fullscreen = args[3].ToLower() == "fullscreen";

                Raylib.InitWindow(screenWidth, screenHeight, "Vampire Survivors Clone");
                if (fullscreen)
                    Raylib.ToggleFullscreen();
                Raylib.SetTargetFPS(60);

                Input.LoadKeyBindings();
                string inputDevice = "Keyboard";
                if (File.Exists("gamesettings.json"))
                {
                    var settings = JsonSerializer.Deserialize<GameSettings>(File.ReadAllText("gamesettings.json"));
                    if (settings != null && !string.IsNullOrEmpty(settings.InputDevice))
                    {
                        inputDevice = settings.InputDevice;
                    }
                }
                Input.CurrentDevice = inputDevice == "Gamepad" ? Input.InputDevice.Gamepad : Input.InputDevice.Keyboard;

            MainMenuStart: // Label for goto statement
                // Main menu loop
                var menu = new MainMenu();
                while (!Raylib.WindowShouldClose() && !menu.StartGame && !menu.LoadGame)
                {
                    menu.Update();
                    menu.Draw();
                }
                if (Raylib.WindowShouldClose()) return;

                // create loadingScreen
                var loadingScreen = new LoadingScreen();
                Game? game = null;

                // Show loading screen while creating game
                if (menu.LoadGame && menu.SelectedSave != null)
                {
                    // Animation loading screen
                    loadingScreen.ShowForDuration(1.5f);
                    
                    // Load Game
                    Difficulty loadedDifficulty = Difficulty.Normal;
                    if (Enum.TryParse<Difficulty>(menu.SelectedSave.Difficulty, out var diff))
                    {
                        loadedDifficulty = diff;
                    }
                    var preset = DifficultyPreset.Get(loadedDifficulty);
                    game = new Game(screenWidth, screenHeight, preset);
                    
                    var saveSystem = new SaveSystem();
                    saveSystem.LoadGame(game, menu.SelectedSave);
                }
                else
                {
                    // Loading screen
                    loadingScreen.ShowForDuration(1.0f);
                    
                    // Creating new game
                    Difficulty diff = menu.ChosenDifficulty;
                    var preset = DifficultyPreset.Get(diff);
                    game = new Game(screenWidth, screenHeight, preset);
                }

                bool returningToMainMenu = false;
                while (!Raylib.WindowShouldClose() && !returningToMainMenu)
                {
                    game.Update();
                    
                    if (game.PauseMenu != null && game.PauseMenu.ShouldQuitToMenu)
                    {
                        returningToMainMenu = true;
                        loadingScreen.ShowForDuration(1.0f); // Show Loading Screen
                        continue;
                    }
                    
                    game.Draw();
                }

                if (returningToMainMenu)
                {
                    goto MainMenuStart; // Back to Main Menu
                }

                Raylib.CloseWindow();
            }
        }
        catch (Exception ex)
        {
            File.WriteAllText("crashlog.txt", ex.ToString());
            throw;
        }
    }
}
