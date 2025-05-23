using Raylib_cs;
using System;
using System.Collections.Generic;
using VampireSurvivorsClone.Data;
using VampireSurvivorsClone.Engine;

namespace VampireSurvivorsClone.UI
{
    public class PauseMenu
    {
        public enum MenuState
        {
            MainPauseMenu,
            SaveMenu,
            LoadMenu,
            QuitMenu
        }

        private MenuState currentState = MenuState.MainPauseMenu;
        private List<string> mainMenuOptions = new() { "Continue", "Save Game", "Load Game", "Quit Game" };
        private List<string> quitMenuOptions = new() { "Cancel", "Quit to Main Menu", "Quit to Desktop" };
        private List<SaveGameData> saveFiles = new();
        private int selectedIndex = 0;
        
        private SaveSystem saveSystem;
        private Game gameInstance;
        
        // Actions that can be triggered by menu selections
        public bool ShouldUnpause { get; set; } = false;
        public bool ShouldQuitToMenu { get; private set; } = false;
        public bool ShouldQuitToDesktop { get; private set; } = false;
        
        public PauseMenu(Game game)
        {
            gameInstance = game;
            saveSystem = new SaveSystem();
            RefreshSaveFiles();
        }
        
        public void RefreshSaveFiles()
        {
            saveFiles = saveSystem.GetSaveFiles();
        }
        
        public void Update()
        {
            // Handle different menu states
            switch (currentState)
            {
                case MenuState.MainPauseMenu:
                    UpdateMainMenu();
                    break;
                case MenuState.SaveMenu:
                    UpdateSaveMenu();
                    break;
                case MenuState.LoadMenu:
                    UpdateLoadMenu();
                    break;
                case MenuState.QuitMenu:
                    UpdateQuitMenu();
                    break;
            }
        }
        
        private void UpdateMainMenu()
        {
            if (Input.IsActionPressed("MoveUp"))
            {
                selectedIndex = (selectedIndex + mainMenuOptions.Count - 1) % mainMenuOptions.Count;
            }
            
            if (Input.IsActionPressed("MoveDown"))
            {
                selectedIndex = (selectedIndex + 1) % mainMenuOptions.Count;
            }
            
            if (Input.IsActionPressed("Confirm"))
            {
                switch (selectedIndex)
                {
                    case 0: // Continue
                        ShouldUnpause = true;
                        break;
                    case 1: // Save Game
                        currentState = MenuState.SaveMenu;
                        selectedIndex = 0;
                        break;
                    case 2: // Load Game
                        currentState = MenuState.LoadMenu;
                        RefreshSaveFiles();
                        selectedIndex = 0;
                        break;
                    case 3: // Quit Game
                        currentState = MenuState.QuitMenu;
                        selectedIndex = 0;
                        break;
                }
            }

            // Support for input - Back
            if (Input.IsActionPressed("Back"))
            {
                ShouldUnpause = true;
            }
        }
        
        private void UpdateSaveMenu()
        {
            // Simple save implementation - always save to a new slot
            if (Input.IsActionPressed("Confirm"))
            {
                saveSystem.SaveGame(gameInstance);
                RefreshSaveFiles();
                
                currentState = MenuState.MainPauseMenu;
                selectedIndex = 0; // Výber Continue
                return;
            }
            
            // Support for input - Back
            if (Input.IsActionPressed("Back") || Input.IsActionPressed("Quit"))
            {
                currentState = MenuState.MainPauseMenu;
                selectedIndex = 1; // Save Game
            }
        }
        
        private void UpdateLoadMenu()
        {
            // If no save files exist
            if (saveFiles.Count == 0)
            {
                if (Input.IsActionPressed("Confirm") || Input.IsActionPressed("Back") || Input.IsActionPressed("Quit"))
                {
                    currentState = MenuState.MainPauseMenu;
                    selectedIndex = 2; // Return to Load Game option
                }
                return;
            }
            
            if (Input.IsActionPressed("MoveUp"))
            {
                selectedIndex = (selectedIndex + saveFiles.Count - 1) % saveFiles.Count;
            }
            
            if (Input.IsActionPressed("MoveDown"))
            {
                selectedIndex = (selectedIndex + 1) % saveFiles.Count;
            }
            
            if (Input.IsActionPressed("Confirm") && selectedIndex < saveFiles.Count)
            {
                // Show loading screen
                var loadingScreen = new LoadingScreen();
                loadingScreen.ShowForDuration(1.0f);
                
                // Loading the selected save file
                saveSystem.LoadGame(gameInstance, saveFiles[selectedIndex]);
                
                // Go back to main menu
                currentState = MenuState.MainPauseMenu;
                selectedIndex = 0; // Continue
                ShouldUnpause = true;
                return;
            }
            
            // Support for input - Back
            if (Input.IsActionPressed("Back") || Input.IsActionPressed("Quit"))
            {
                currentState = MenuState.MainPauseMenu;
                selectedIndex = 2; // return to Load Game
            }
        }
        
        private void UpdateQuitMenu()
        {
            if (Input.IsActionPressed("MoveUp"))
            {
                selectedIndex = (selectedIndex + quitMenuOptions.Count - 1) % quitMenuOptions.Count;
            }
            
            if (Input.IsActionPressed("MoveDown"))
            {
                selectedIndex = (selectedIndex + 1) % quitMenuOptions.Count;
            }
            
            if (Input.IsActionPressed("Confirm"))
            {
                switch (selectedIndex)
                {
                    case 0: // Cancel
                        currentState = MenuState.MainPauseMenu;
                        selectedIndex = 3; // Go back to Quit option
                        break;
                    case 1: // Quit to Main Menu
                        ShouldQuitToMenu = true;
                        break;
                    case 2: // Quit to Desktop
                        ShouldQuitToDesktop = true;
                        break;
                }
            }
            
            // Support for input - Back
            if (Input.IsActionPressed("Back"))
            {
                currentState = MenuState.MainPauseMenu;
                selectedIndex = 3; // Quit Game
            }
        }
        
        public void Draw()
        {
            Raylib.BeginDrawing();
            
            // Semi-transparent background
            Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), new Color(0, 0, 0, 200));
            
            switch (currentState)
            {
                case MenuState.MainPauseMenu:
                    DrawMainMenu();
                    break;
                case MenuState.SaveMenu:
                    DrawSaveMenu();
                    break;
                case MenuState.LoadMenu:
                    DrawLoadMenu();
                    break;
                case MenuState.QuitMenu:
                    DrawQuitMenu();
                    break;
            }
            
            Raylib.EndDrawing();
        }
        
        private void DrawMainMenu()
        {
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            
            // Semi-transparent background
            Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, new Color(0, 0, 0, 150));
            
            // Title
            string title = "PAUSED";
            int titleFontSize = 50;
            int titleWidth = Raylib.MeasureText(title, titleFontSize);
            Raylib.DrawText(title, screenWidth / 2 - titleWidth / 2, screenHeight / 6, titleFontSize, Color.WHITE);
            
            // Draw menu options
            for (int i = 0; i < mainMenuOptions.Count; i++)
            {
                int fontSize = (i == selectedIndex) ? 32 : 24;
                Color col = (i == selectedIndex) ? Color.YELLOW : Color.WHITE;
                
                string option = mainMenuOptions[i];
                int textWidth = Raylib.MeasureText(option, fontSize);
                int posY = screenHeight / 3 + 50 + i * 60;
                
                Raylib.DrawText(option, screenWidth / 2 - textWidth / 2, posY, fontSize, col);
            }
            
            // Controls hint - adaptive based by input device
            string controlsHint = Input.CurrentDevice == Input.InputDevice.Gamepad ? 
                "▲▼: Navigate  A: Select  B: Return" : 
                "W/S: Navigate  Enter: Select  Backspace: Return";
            int hintWidth = Raylib.MeasureText(controlsHint, 20);
            Raylib.DrawText(controlsHint, screenWidth / 2 - hintWidth / 2, screenHeight - 50, 20, Color.GRAY);
        }
        
        private void DrawSaveMenu()
        {
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            
            // Semi-transparent background
            Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, new Color(0, 0, 0, 150));
            
            // Title
            string title = "SAVE GAME";
            int titleFontSize = 40;
            int titleWidth = Raylib.MeasureText(title, titleFontSize);
            Raylib.DrawText(title, screenWidth / 2 - titleWidth / 2, screenHeight / 6, titleFontSize, Color.WHITE);
            
            // Save info
            string saveInfo = "Press confirm to save your game";
            int infoWidth = Raylib.MeasureText(saveInfo, 30);
            Raylib.DrawText(saveInfo, screenWidth / 2 - infoWidth / 2, screenHeight / 2 - 15, 30, Color.WHITE);
            
            // Controls hint - adaptive
            string controlsHint = Input.CurrentDevice == Input.InputDevice.Gamepad ? 
                "A: Save  B: Cancel" : 
                "Enter: Save  Backspace: Cancel";
            int hintWidth = Raylib.MeasureText(controlsHint, 20);
            Raylib.DrawText(controlsHint, screenWidth / 2 - hintWidth / 2, screenHeight - 50, 20, Color.GRAY);
        }
        
        private void DrawLoadMenu()
        {
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            
            // Title
            string title = "LOAD GAME";
            int titleFontSize = 40;
            int titleWidth = Raylib.MeasureText(title, titleFontSize);
            Raylib.DrawText(title, screenWidth / 2 - titleWidth / 2, screenHeight / 4, titleFontSize, Color.WHITE);
            
            if (saveFiles.Count == 0)
            {
                string noSaves = "No save files found!";
                int noSavesWidth = Raylib.MeasureText(noSaves, 24);
                Raylib.DrawText(noSaves, screenWidth / 2 - noSavesWidth / 2, screenHeight / 2, 24, Color.GRAY);
                Raylib.DrawText("Press Back to return", 500, 350, 18, Color.GRAY);
                return;
            }
            
            // Save files list
            for (int i = 0; i < saveFiles.Count; i++)
            {
                var save = saveFiles[i];
                string saveText = $"{save.SaveDate} - Wave: {save.WaveNumber} - Level: {save.PlayerLevel}";
                
                int fontSize = i == selectedIndex ? 26 : 20;
                Color color = i == selectedIndex ? Color.YELLOW : Color.WHITE;
                
                int textWidth = Raylib.MeasureText(saveText, fontSize);
                int posY = screenHeight / 3 + 50 + i * 40;
                Raylib.DrawText(saveText, screenWidth / 2 - textWidth / 2, posY, fontSize, color);
            }
            
            // Controls hint - adaptive
            string controlsHint = Input.CurrentDevice == Input.InputDevice.Gamepad ? 
                "▲▼: Navigate  A: Select  B: Cancel" : 
                "W/S: Navigate  Enter: Select  Backspace: Cancel";
            int hintWidth = Raylib.MeasureText(controlsHint, 20);
            Raylib.DrawText(controlsHint, screenWidth / 2 - hintWidth / 2, screenHeight - 50, 20, Color.GRAY);
        }
        
        private void DrawQuitMenu()
        {
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            
            // Title
            string title = "QUIT GAME";
            int titleFontSize = 40;
            int titleWidth = Raylib.MeasureText(title, titleFontSize);
            Raylib.DrawText(title, screenWidth / 2 - titleWidth / 2, screenHeight / 4, titleFontSize, Color.WHITE);
            
            // Quit options
            for (int i = 0; i < quitMenuOptions.Count; i++)
            {
                string option = quitMenuOptions[i];
                int fontSize = i == selectedIndex ? 30 : 24;
                Color color = i == selectedIndex ? Color.YELLOW : Color.WHITE;
                
                int textWidth = Raylib.MeasureText(option, fontSize);
                int posY = screenHeight / 3 + 50 + i * 50;
                Raylib.DrawText(option, screenWidth / 2 - textWidth / 2, posY, fontSize, color);
            }
            
            // Controls hint - adaptive
            string controlsHint = Input.CurrentDevice == Input.InputDevice.Gamepad ? 
                "▲▼: Navigate  A: Select  B: Cancel" : 
                "W/S: Navigate  Enter: Select  Backspace: Cancel";
            int hintWidth = Raylib.MeasureText(controlsHint, 20);
            Raylib.DrawText(controlsHint, screenWidth / 2 - hintWidth / 2, screenHeight - 50, 20, Color.GRAY);
        }
    }
}