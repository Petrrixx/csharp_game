using Raylib_cs;
using VampireSurvivorsClone.Data;
using VampireSurvivorsClone.Engine;

namespace VampireSurvivorsClone.UI;

public class MainMenu
{
    private int selected = 0;
    private Difficulty[] difficulties = new[] { Difficulty.Easy, Difficulty.Normal, Difficulty.Hard };
    private int selectedDifficulty = 1; // Default: Normal

    public bool StartGame { get; private set; } = false;
    public Difficulty ChosenDifficulty => difficulties[selectedDifficulty];

    public void Update()
    {
        Input.UpdateInputDevice();

        // Debug: vypíš či Raylib vidí gamepad
        if (!Raylib.IsGamepadAvailable(0))
            Raylib.DrawText("Gamepad NOT detected!", 10, 10, 20, Color.RED);

        // TODO: Fix Gameplay input in Main Menu, right now only works with keyboard
        if (Input.IsActionPressed("MoveUp")) selected = (selected + 2) % 3;
        if (Input.IsActionPressed("MoveDown")) selected = (selected + 1) % 3;

        if (selected == 1) // Difficulty
        {
            if (Input.IsActionPressed("MoveLeft")) selectedDifficulty = (selectedDifficulty + 2) % 3;
            if (Input.IsActionPressed("MoveRight")) selectedDifficulty = (selectedDifficulty + 1) % 3;
        }

        if (Input.IsActionPressed("Confirm"))
        {
            if (selected == 0) StartGame = true;
            if (selected == 2) Raylib.CloseWindow();
        }
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.DARKGRAY);

        Raylib.DrawText("VAMPIRE SURVIVORS FAN-MADE", 400, 100, 32, Color.LIME);

        string[] options = { "Start Game", $"Difficulty: {difficulties[selectedDifficulty]}", "Quit" };
        for (int i = 0; i < options.Length; i++)
        {
            Color col = (i == selected) ? Color.YELLOW : Color.WHITE;
            Raylib.DrawText(options[i], 540, 250 + i * 60, 28, col);
        }

        // KNOWN BUG: Raylib doesn't detect gamepad in Main Menu - FIXED - Issue was that in WPF project, the project is starting the last built version of the game
        // Before running the WPF, Program.cs needs to be pre-built
        //Raylib.DrawText("Gamepad not working in Main Menu! (W.I.P)", 10, 10, 20, Color.RED);

        Raylib.EndDrawing();
    }
}