using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using VampireSurvivorsClone.Data;
using VampireSurvivorsClone.Engine;

namespace VampireSurvivorsClone.UI;

public class MainMenu
{
    // Menu states
    public enum MenuState
    {
        MainMenu,
        DifficultySelection,
        LoadGameMenu,
        SaveDetails
    }
    
    private MenuState currentState = MenuState.MainMenu;
    private int selected = 0;
    private Difficulty[] difficulties = new[] { Difficulty.Easy, Difficulty.Normal, Difficulty.Hard };
    private int selectedDifficulty = 1; // Default: Normal
    private List<SaveGameData> saveFiles = new();
    private int selectedSaveIndex = 0;
    private bool deleteSaveMode = false;
    
    // Properties
    public bool StartGame { get; private set; } = false;
    public bool LoadGame { get; private set; } = false;
    public Difficulty ChosenDifficulty => difficulties[selectedDifficulty];
    public SaveGameData? SelectedSave => saveFiles.Count > selectedSaveIndex && selectedSaveIndex >= 0 ? saveFiles[selectedSaveIndex] : null;
    
    // Save system reference
    private SaveSystem saveSystem;
    
    public MainMenu()
    {
        saveSystem = new SaveSystem();
        RefreshSaveFiles();
    }
    
    private void RefreshSaveFiles()
    {
        saveFiles = saveSystem.GetSaveFiles();
    }

    public void Update()
    {
        Input.UpdateInputDevice();

        switch (currentState)
        {
            case MenuState.MainMenu:
                UpdateMainMenu();
                break;
                
            case MenuState.DifficultySelection:
                UpdateDifficultySelection();
                break;
                
            case MenuState.LoadGameMenu:
                UpdateLoadGameMenu();
                break;
                
            case MenuState.SaveDetails:
                UpdateSaveDetails();
                break;
        }
    }

    private void UpdateMainMenu()
    {
        // Get number of menu options based on if we have saves
        int menuOptionCount = saveFiles.Count > 0 ? 4 : 3; // Continue option only shown if save exists
        
        if (Input.IsActionPressed("MoveUp")) 
            selected = (selected + menuOptionCount - 1) % menuOptionCount;
            
        if (Input.IsActionPressed("MoveDown")) 
            selected = (selected + 1) % menuOptionCount;
        
        if (Input.IsActionPressed("Confirm"))
        {
            // With saves: 0=Continue, 1=New Game, 2=Load Game, 3=Quit
            // Without saves: 0=New Game, 1=Load Game, 2=Quit
            int optionIndex = selected;
            bool hasSaves = saveFiles.Count > 0;
            
            if (hasSaves)
            {
                switch (optionIndex)
                {
                    case 0: // Continue - load the latest save
                        if (saveFiles.Count > 0)
                        {
                            selectedSaveIndex = 0; // First save is the newest
                            LoadGame = true;
                        }
                        break;
                        
                    case 1: // New Game
                        currentState = MenuState.DifficultySelection;
                        selected = selectedDifficulty;
                        break;
                        
                    case 2: // Load Game
                        currentState = MenuState.LoadGameMenu;
                        selectedSaveIndex = 0; // Reset selection to first save
                        selected = 0;
                        RefreshSaveFiles();
                        break;
                        
                    case 3: // Quit
                        Raylib.CloseWindow();
                        break;
                }
            }
            else
            {
                switch (optionIndex)
                {
                    case 0: // New Game (no saves)
                        currentState = MenuState.DifficultySelection;
                        selected = selectedDifficulty;
                        break;
                        
                    case 1: // Load Game (no saves)
                        currentState = MenuState.LoadGameMenu;
                        selected = 0;
                        break;
                        
                    case 2: // Quit (no saves)
                        Raylib.CloseWindow();
                        break;
                }
            }
        }
    }

    private void UpdateDifficultySelection()
    {
        if (Input.IsActionPressed("MoveUp")) 
        {
            selectedDifficulty = (selectedDifficulty + difficulties.Length - 1) % difficulties.Length;
            selected = selectedDifficulty; // Synchronized selected with selectedDifficulty
        }
        
        if (Input.IsActionPressed("MoveDown")) 
        {
            selectedDifficulty = (selectedDifficulty + 1) % difficulties.Length;
            selected = selectedDifficulty; 
        }
        
        if (Input.IsActionPressed("MoveLeft") || Input.IsActionPressed("MoveRight"))
        {
            selectedDifficulty = (selectedDifficulty + difficulties.Length - 1) % difficulties.Length;
            selected = selectedDifficulty; 
        }
        
        if (Input.IsActionPressed("Confirm"))
        {
            StartGame = true;
        }
        
        if (Input.IsActionPressed("Back") || Input.IsActionPressed("Quit"))
        {
            currentState = MenuState.MainMenu;
            selected = saveFiles.Count > 0 ? 1 : 0; // Return to New Game option if no saves
        }
    }

    private void UpdateLoadGameMenu()
    {
        if (saveFiles.Count == 0)
        {
            if (Input.IsActionPressed("Confirm") || Input.IsActionPressed("Back"))
            {
                currentState = MenuState.MainMenu;
                selected = saveFiles.Count > 0 ? 2 : 1; // Select "Load Game" option
            }
            return;
        }
        
        if (Input.IsActionPressed("MoveUp")) 
            selected = (selected + saveFiles.Count - 1) % saveFiles.Count;
            
        if (Input.IsActionPressed("MoveDown")) 
            selected = (selected + 1) % saveFiles.Count;
        
        if (Input.IsActionPressed("Confirm"))
        {
            selectedSaveIndex = selected;
            currentState = MenuState.SaveDetails;
            selected = 0; // Default to "Load" option
            deleteSaveMode = false;
        }
        
        // Return to main menu on cancel
        if (Input.IsActionPressed("Back"))
        {
            currentState = MenuState.MainMenu;
            selected = 2; // Return to Load Game option
        }
    }

    private void UpdateSaveDetails()
    {
        // Select
        if (!deleteSaveMode && (Input.IsActionPressed("MoveLeft") || Input.IsActionPressed("MoveRight")))
            selected = (selected + 1) % 2; // Toggle between Load and Delete options only when not in delete mode
        
        if (deleteSaveMode)
        {
            // In delete confirmation mode
            if (Input.IsActionPressed("MoveLeft") || Input.IsActionPressed("MoveRight")) 
                selected = (selected + 1) % 2; // Toggle between Yes and No
            
            if (Input.IsActionPressed("Confirm"))
            {
                if (selected == 0) // Yes - confirm delete
                {
                    // Delete the save file
                    if (selectedSaveIndex < saveFiles.Count)
                    {
                        // Delete functionality
                        saveSystem.DeleteSave(saveFiles[selectedSaveIndex].SaveName);
                        
                        // Refresh the save list
                        RefreshSaveFiles();
                    }
                    
                    deleteSaveMode = false;
                    currentState = MenuState.LoadGameMenu;
                    selected = Math.Min(selectedSaveIndex, saveFiles.Count - 1);
                    if (selected < 0) selected = 0;
                }
                else // No - cancel delete
                {
                    deleteSaveMode = false;
                    selected = 1; // Set back to Delete button
                }
            }
            
            // Support for both Back and Quit
            if (Input.IsActionPressed("Back") || Input.IsActionPressed("Quit"))
            {
                deleteSaveMode = false;
                selected = 1; // Set back to Delete button
            }
        }
        else
        {
            // Normal save details mode
            if (Input.IsActionPressed("Confirm"))
            {
                if (selected == 0) // Load Save
                {
                    LoadGame = true;
                }
                else // Delete Save
                {
                    deleteSaveMode = true;
                    selected = 1; // Default to "No" for safety
                }
            }
            
            // Return to load game menu on cancel - Back
            if (Input.IsActionPressed("Back") || Input.IsActionPressed("Quit"))
            {
                currentState = MenuState.LoadGameMenu;
                selected = selectedSaveIndex;
            }
        }
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(new Color(20, 20, 30, 255));

        // Draw the appropriate menu based on state
        switch (currentState)
        {
            case MenuState.MainMenu:
                DrawMainMenu();
                break;
                
            case MenuState.DifficultySelection:
                DrawDifficultySelection();
                break;
                
            case MenuState.LoadGameMenu:
                DrawLoadGameMenu();
                break;
                
            case MenuState.SaveDetails:
                DrawSaveDetails();
                break;
        }

        Raylib.EndDrawing();
    }

    private void DrawMainMenu()
    {
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();
        
        // Title
        string title = "VAMPIRE SURVIVORS FAN-MADE";
        int titleFontSize = 40;
        int titleWidth = Raylib.MeasureText(title, titleFontSize);
        Raylib.DrawText(title, screenWidth / 2 - titleWidth / 2, screenHeight / 6, titleFontSize, Color.LIME);
        
        // Create option list based on available saves
        List<string> options = new();
        
        if (saveFiles.Count > 0)
        {
            options.Add("Continue");
        }
        
        options.Add("New Game");
        options.Add("Load Game");
        options.Add("Quit Game");
        
        // Draw options with improved styling
        for (int i = 0; i < options.Count; i++)
        {
            int fontSize = (i == selected) ? 32 : 24; // Larger font for selected item
            Color col = (i == selected) ? Color.YELLOW : Color.WHITE;
            
            string option = options[i];
            int textWidth = Raylib.MeasureText(option, fontSize);
            int posY = screenHeight / 3 + 50 + i * 60;
            
            Raylib.DrawText(option, screenWidth / 2 - textWidth / 2, posY, fontSize, col);
        }
        
        // If "Continue" is selected, show info about the latest save
        if (saveFiles.Count > 0 && selected == 0)
        {
            var latestSave = saveFiles.First();
            string saveInfo = $"Last save: {latestSave.SaveDate.ToShortDateString()} {latestSave.SaveDate.ToShortTimeString()} - Wave: {latestSave.WaveNumber} - Level: {latestSave.PlayerLevel}";
            int infoWidth = Raylib.MeasureText(saveInfo, 16);
            Raylib.DrawText(saveInfo, screenWidth / 2 - infoWidth / 2, screenHeight - 100, 16, Color.LIGHTGRAY);
        }
        
        // Controls hint at the bottom
        string controlsHint = Input.CurrentDevice == Input.InputDevice.Gamepad ? 
            "▲▼: Navigate  A: Select  B: Back" : 
            "W/S: Navigate  Enter: Select  Backspace: Back";
        int hintWidth = Raylib.MeasureText(controlsHint, 20);
        Raylib.DrawText(controlsHint, screenWidth / 2 - hintWidth / 2, screenHeight - 50, 20, Color.GRAY);
    }

    private void DrawDifficultySelection()
    {
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();
        
        // Title
        string title = "SELECT DIFFICULTY";
        int titleFontSize = 40;
        int titleWidth = Raylib.MeasureText(title, titleFontSize);
        Raylib.DrawText(title, screenWidth / 2 - titleWidth / 2, screenHeight / 6, titleFontSize, Color.WHITE);
        
        // Draw difficulty options
        for (int i = 0; i < difficulties.Length; i++)
        {
            int fontSize = (i == selectedDifficulty) ? 32 : 24;
            Color col = (i == selectedDifficulty) ? Color.YELLOW : Color.WHITE;
            
            string option = difficulties[i].ToString();
            int textWidth = Raylib.MeasureText(option, fontSize);
            int posY = screenHeight / 3 + 50 + i * 60;
            
            Raylib.DrawText(option, screenWidth / 2 - textWidth / 2, posY, fontSize, col);
        }
        
        // Controls hint
        string hint = Input.CurrentDevice == Input.InputDevice.Gamepad ? 
            "B: Return" : 
            "Backspace: Return";
        int hintWidth = Raylib.MeasureText(hint, 20);
        Raylib.DrawText(hint, screenWidth / 2 - hintWidth / 2, screenHeight - 50, 20, Color.GRAY);
    }

    private void DrawLoadGameMenu()
    {
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();
        
        // Title
        string title = "LOAD GAME";
        int titleFontSize = 40;
        int titleWidth = Raylib.MeasureText(title, titleFontSize);
        Raylib.DrawText(title, screenWidth / 2 - titleWidth / 2, screenHeight / 6, titleFontSize, Color.WHITE);
        
        if (saveFiles.Count == 0)
        {
            string noSaves = "No save files found!";
            int textWidth = Raylib.MeasureText(noSaves, 28);
            Raylib.DrawText(noSaves, screenWidth / 2 - textWidth / 2, screenHeight / 2, 28, Color.GRAY);
            
            // Adaptívny hint
            string hint = Input.CurrentDevice == Input.InputDevice.Gamepad ? 
                "Press any button to go back" : 
                "Press any key to go back";
            int hintWidth2 = Raylib.MeasureText(hint, 20);
            Raylib.DrawText(hint, screenWidth / 2 - hintWidth2 / 2, screenHeight - 50, 20, Color.GRAY);
            return;
        }
        
        // Draw save files list
        int yStart = screenHeight / 3;
        int spacing = 40;
        
        for (int i = 0; i < saveFiles.Count; i++)
        {
            var save = saveFiles[i];
            string saveText = $"{save.SaveDate.ToShortDateString()} {save.SaveDate.ToShortTimeString()} - Wave: {save.WaveNumber} - Level: {save.PlayerLevel}";
            
            int fontSize = (i == selected) ? 24 : 20;
            Color col = (i == selected) ? Color.YELLOW : Color.WHITE;
            
            int textWidth = Raylib.MeasureText(saveText, fontSize);
            Raylib.DrawText(saveText, screenWidth / 2 - textWidth / 2, yStart + i * spacing, fontSize, col);
        }
        
        // Controls hint - adaptívne podľa typu ovládania
        string controlsHint = Input.CurrentDevice == Input.InputDevice.Gamepad ? 
            "B: Return" : 
            "Backspace: Return";
        int hintWidth = Raylib.MeasureText(controlsHint, 20);
        Raylib.DrawText(controlsHint, screenWidth / 2 - hintWidth / 2, screenHeight - 50, 20, Color.GRAY);
    }

    private void DrawSaveDetails()
    {
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();
        
        if (selectedSaveIndex >= saveFiles.Count) return;
        
        var save = saveFiles[selectedSaveIndex];
        
        // Title
        string title = "SAVE DETAILS";
        int titleFontSize = 40;
        int titleWidth = Raylib.MeasureText(title, titleFontSize);
        Raylib.DrawText(title, screenWidth / 2 - titleWidth / 2, screenHeight / 6, titleFontSize, Color.WHITE);
        
        // Save details
        int centerX = screenWidth / 2;
        int yPos = screenHeight / 3;
        int spacing = 30;
        
        Raylib.DrawText($"Date: {save.SaveDate.ToString()}", centerX - 200, yPos, 20, Color.WHITE); yPos += spacing;
        Raylib.DrawText($"Wave: {save.WaveNumber}", centerX - 200, yPos, 20, Color.WHITE); yPos += spacing;
        Raylib.DrawText($"Player Level: {save.PlayerLevel}", centerX - 200, yPos, 20, Color.WHITE); yPos += spacing;
        
        if (save.Player != null)
        {
            Raylib.DrawText($"Health: {save.Player.Health}", centerX - 200, yPos, 20, Color.WHITE); yPos += spacing;
            Raylib.DrawText($"Strength: {save.Player.Strength}", centerX - 200, yPos, 20, Color.WHITE); yPos += spacing;
            Raylib.DrawText($"Weapons: {save.Player.Weapons.Count}", centerX - 200, yPos, 20, Color.WHITE); yPos += spacing;
        }
        
        yPos = screenHeight / 2 + 100;
        
        if (deleteSaveMode)
        {
            string deleteConfirm = "Are you sure you want to delete this save?";
            int confirmWidth = Raylib.MeasureText(deleteConfirm, 24);
            Raylib.DrawText(deleteConfirm, screenWidth / 2 - confirmWidth / 2, yPos, 24, Color.RED);
            
            string[] options = { "Yes", "No" };
            yPos += 50;
            
            for (int i = 0; i < options.Length; i++)
            {
                int fontSize = (i == selected) ? 32 : 24;
                Color col = (i == selected) ? Color.YELLOW : Color.WHITE;
                
                int width = Raylib.MeasureText(options[i], fontSize);
                int x = screenWidth / 2 - 80 + i * 160;
                Raylib.DrawText(options[i], x - width / 2, yPos, fontSize, col);
            }
        }
        else
        {
            string[] options = { "Load Save", "Delete Save" };
            
            for (int i = 0; i < options.Length; i++)
            {
                int fontSize = (i == selected) ? 32 : 24;
                Color col = (i == selected) ? Color.YELLOW : Color.WHITE;
                
                int width = Raylib.MeasureText(options[i], fontSize);
                int x = screenWidth / 2 - 150 + i * 300;
                Raylib.DrawText(options[i], x - width / 2, yPos, fontSize, col);
            }
        }
        
        // Controls hint
        string hint = Input.CurrentDevice == Input.InputDevice.Gamepad ? 
            "B: Return" : 
            "Backspace: Return";
        int hintWidth = Raylib.MeasureText(hint, 20);
        Raylib.DrawText(hint, screenWidth / 2 - hintWidth / 2, screenHeight - 50, 20, Color.GRAY);
    }
}