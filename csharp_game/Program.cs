using Raylib_cs;
using VampireSurvivorsClone.Engine;
using VampireSurvivorsClone.UI;
using VampireSurvivorsClone.Data;

public class Program
{
    public static void Main()
    {
        var screenWidth = 1280;
        var screenHeight = 720;
        Raylib.InitWindow(screenWidth, screenHeight, "Vampire Survivors Clone");
        Raylib.SetTargetFPS(60);

        // Main menu loop
        var menu = new MainMenu();
        while (!Raylib.WindowShouldClose() && !menu.StartGame)
        {
            menu.Update();
            menu.Draw();
        }
        if (Raylib.WindowShouldClose()) return;

        // Get chosen difficulty
        Difficulty diff = menu.ChosenDifficulty;
        var preset = DifficultyPreset.Get(diff);

        // Pass preset to Game
        Game game = new Game(screenWidth, screenHeight, preset);

        while (!Raylib.WindowShouldClose())
        {
            game.Update();
            game.Draw();
        }

        Raylib.CloseWindow();
    }
}
