using Raylib_cs;
using VampireSurvivorsClone.Engine;
using System.Drawing;

public class Program
{
    public static void Main()
    {
        //TODO: Add a splash screen
        // Get the user's screen resolution
        var screenWidth = Screen.PrimaryScreen.Bounds.Width;  // Get screen width
        var screenHeight = Screen.PrimaryScreen.Bounds.Height; // Get screen height

        // Set the window size to the screen resolution
        Raylib.InitWindow(screenWidth, screenHeight, "Vampire Survivors Clone");

        // Optionally enable fullscreen by default
        Raylib.ToggleFullscreen();  // Enable fullscreen mode

        // Create a new game with the screen size
        Game game = new Game(screenWidth, screenHeight);

        Raylib.SetTargetFPS(60); // Set FPS to 60

        while (!Raylib.WindowShouldClose())
        {
            game.Update();
            game.Draw();
        }

        Raylib.CloseWindow();
    }
}
