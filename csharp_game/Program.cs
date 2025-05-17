using Raylib_cs;
using VampireSurvivorsClone.Engine;

public class Program
{
    public static void Main()
    {
        // Get the user's screen resolution
        var screenWidth = 1280;  // Get screen width
        var screenHeight = 720; // Get screen height

        // Set the window size to the screen resolution
        Raylib.InitWindow(screenWidth, screenHeight, "Vampire Survivors Clone");

        // Optionally enable fullscreen by default
        //Raylib.ToggleFullscreen();  // Enable fullscreen mode

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
