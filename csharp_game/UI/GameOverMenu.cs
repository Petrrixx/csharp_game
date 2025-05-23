using Raylib_cs;
using System;
using VampireSurvivorsClone.Engine;

namespace VampireSurvivorsClone.UI
{
    public class GameOverMenu
    {
        private string[] options = { "Return to Main Menu", "Quit Game" };
        private int selectedOption = 0;
        
        // Action flags
        public bool ReturnToMainMenuSelected { get; private set; } = false;
        public bool QuitGameSelected { get; private set; } = false;
        
        public void Update()
        {
            // Reset action flags
            ReturnToMainMenuSelected = false;
            QuitGameSelected = false;
            
            // Navigate menu
            if (Input.IsActionPressed("MoveUp"))
            {
                selectedOption = (selectedOption + options.Length - 1) % options.Length;
            }
            
            if (Input.IsActionPressed("MoveDown"))
            {
                selectedOption = (selectedOption + 1) % options.Length;
            }
            
            // Select option
            if (Input.IsActionPressed("Confirm"))
            {
                switch (selectedOption)
                {
                    case 0: // Return to Main Menu
                        ReturnToMainMenuSelected = true;
                        break;
                    case 1: // Quit Game
                        QuitGameSelected = true;
                        break;
                }
            }
        }
        
        public void Draw()
        {
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
            
            // Game Over title
            string gameOverText = "GAME OVER";
            int titleFontSize = 60;
            int titleWidth = Raylib.MeasureText(gameOverText, titleFontSize);
            Raylib.DrawText(gameOverText, screenWidth / 2 - titleWidth / 2, screenHeight / 4, titleFontSize, Color.RED);
            
            // Menu options
            for (int i = 0; i < options.Length; i++)
            {
                int fontSize = (i == selectedOption) ? 30 : 24;
                Color color = (i == selectedOption) ? Color.YELLOW : Color.WHITE;
                
                int textWidth = Raylib.MeasureText(options[i], fontSize);
                int posY = screenHeight / 2 + i * 50;
                
                Raylib.DrawText(options[i], screenWidth / 2 - textWidth / 2, posY, fontSize, color);
            }
            
            // Controls hint
            string controlsHint = Input.CurrentDevice == Input.InputDevice.Gamepad ? 
                "▲▼: Navigate  A: Select" : 
                "W/S: Navigate  Enter: Select";
            int hintWidth = Raylib.MeasureText(controlsHint, 20);
            Raylib.DrawText(controlsHint, screenWidth / 2 - hintWidth / 2, screenHeight - 50, 20, Color.GRAY);
            
            Raylib.EndDrawing();
        }
    }
}