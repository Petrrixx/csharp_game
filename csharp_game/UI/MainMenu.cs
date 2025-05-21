using Raylib_cs;
using VampireSurvivorsClone.Data;

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
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_W)) selected = (selected + 2) % 3;
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_S)) selected = (selected + 1) % 3;

        if (selected == 1) // Difficulty
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT)) selectedDifficulty = (selectedDifficulty + 2) % 3;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT)) selectedDifficulty = (selectedDifficulty + 1) % 3;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
        {
            if (selected == 0) StartGame = true;
            if (selected == 2) Raylib.CloseWindow();
        }
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.DARKGRAY);

        Raylib.DrawText("VAMPIRE SURVIVORS CLONE", 400, 100, 32, Color.LIME);

        string[] options = { "Start Game", $"Difficulty: {difficulties[selectedDifficulty]}", "Quit" };
        for (int i = 0; i < options.Length; i++)
        {
            Color col = (i == selected) ? Color.YELLOW : Color.WHITE;
            Raylib.DrawText(options[i], 540, 250 + i * 60, 28, col);
        }

        Raylib.EndDrawing();
    }
}