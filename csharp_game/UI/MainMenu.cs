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
        if (Input.IsActionPressed("MoveUp") || Input.IsActionPressed("MoveDown")) 
        {
            selectedDifficulty = (selectedDifficulty + 1) % difficulties.Length;
        }
        
        if (Input.IsActionPressed("MoveLeft") || Input.IsActionPressed("MoveRight"))
        {
            selectedDifficulty = (selectedDifficulty + difficulties.Length - 1) % difficulties.Length;
        }
        
        if (Input.IsActionPressed("Confirm"))
        {
            StartGame = true;
        }
        
        // Return to main menu on cancel
        if (Input.IsActionPressed("Quit"))
        {
            currentState = MenuState.MainMenu;
            selected = 0;
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
        if (Input.IsActionPressed("MoveLeft") || Input.IsActionPressed("MoveRight")) 
            selected = (selected + 1) % 2; // Toggle between Load and Delete options
        
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
                        // Implement Delete functionality
                        // saveSystem.DeleteSave(saveFiles[selectedSaveIndex].SaveName);
                        
                        // For now, just refresh the save list
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
            
            if (Input.IsActionPressed("Quit"))
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
            
            // Return to load game menu on cancel
            if (Input.IsActionPressed("Quit"))
            {
                currentState = MenuState.LoadGameMenu;
                selected = selectedSaveIndex;
            }
        }
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.DARKGRAY);

        // Draw the title
        Raylib.DrawText("VAMPIRE SURVIVORS FAN-MADE", 400, 60, 32, Color.LIME);

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
        // Create option list based on available saves
        List<string> options = new();
        
        if (saveFiles.Count > 0)
        {
            options.Add("Continue");
        }
        
        options.Add("New Game");
        options.Add("Load Game");
        options.Add("Quit Game");
        
        // Draw options
        for (int i = 0; i < options.Count; i++)
        {
            Color col = (i == selected) ? Color.YELLOW : Color.WHITE;
            Raylib.DrawText(options[i], 540, 250 + i * 60, 28, col);
        }
        
        // If "Continue" is selected, show info about the latest save
        if (saveFiles.Count > 0 && selected == 0)
        {
            var latestSave = saveFiles.First();
            string saveInfo = $"Last save: {latestSave.SaveDate.ToShortDateString()} {latestSave.SaveDate.ToShortTimeString()} - Wave: {latestSave.WaveNumber} - Level: {latestSave.PlayerLevel}";
            Raylib.DrawText(saveInfo, 400, 550, 16, Color.LIGHTGRAY);
        }
    }

    private void DrawDifficultySelection()
    {
        Raylib.DrawText("SELECT DIFFICULTY", 500, 180, 28, Color.WHITE);
        
        for (int i = 0; i < difficulties.Length; i++)
        {
            Color col = (i == selected) ? Color.YELLOW : Color.WHITE;
            Raylib.DrawText(difficulties[i].ToString(), 540, 250 + i * 60, 28, col);
        }
        
        Raylib.DrawText("Press ESC to go back", 500, 550, 18, Color.GRAY);
    }

    private void DrawLoadGameMenu()
    {
        Raylib.DrawText("LOAD GAME", 540, 180, 28, Color.WHITE);
        
        if (saveFiles.Count == 0)
        {
            Raylib.DrawText("No save files found!", 500, 300, 24, Color.GRAY);
            Raylib.DrawText("Press any key to go back", 500, 350, 18, Color.GRAY);
            return;
        }
        
        int yStart = 250;
        int spacing = 40;
        
        for (int i = 0; i < saveFiles.Count; i++)
        {
            var save = saveFiles[i];
            string saveText = $"{save.SaveDate.ToShortDateString()} {save.SaveDate.ToShortTimeString()} - Wave: {save.WaveNumber} - Level: {save.PlayerLevel}";
            
            Color col = (i == selected) ? Color.YELLOW : Color.WHITE;
            Raylib.DrawText(saveText, 400, yStart + i * spacing, 20, col);
        }
        
        Raylib.DrawText("Press Back to return", 500, 550, 18, Color.GRAY);
    }

    private void DrawSaveDetails()
    {
        if (selectedSaveIndex >= saveFiles.Count) return;
        
        var save = saveFiles[selectedSaveIndex];
        
        Raylib.DrawText("SAVE DETAILS", 540, 180, 28, Color.WHITE);
        
        int yPos = 230;
        Raylib.DrawText($"Date: {save.SaveDate.ToString()}", 400, yPos, 20, Color.WHITE); yPos += 30;
        Raylib.DrawText($"Wave: {save.WaveNumber}", 400, yPos, 20, Color.WHITE); yPos += 30;
        Raylib.DrawText($"Player Level: {save.PlayerLevel}", 400, yPos, 20, Color.WHITE); yPos += 30;
        Raylib.DrawText($"Difficulty: {save.Difficulty}", 400, yPos, 20, Color.WHITE); yPos += 30;
        
        if (save.Player != null)
        {
            Raylib.DrawText($"Health: {save.Player.Health}", 400, yPos, 20, Color.WHITE); yPos += 30;
            Raylib.DrawText($"Strength: {save.Player.Strength}", 400, yPos, 20, Color.WHITE); yPos += 30;
            Raylib.DrawText($"Weapons: {save.Player.Weapons.Count}", 400, yPos, 20, Color.WHITE); yPos += 30;
        }
        
        yPos = 450;
        
        if (deleteSaveMode)
        {
            Raylib.DrawText("Are you sure you want to delete this save?", 400, yPos, 20, Color.RED);
            
            string[] options = { "Yes", "No" };
            for (int i = 0; i < options.Length; i++)
            {
                Color col = (i == selected) ? Color.YELLOW : Color.WHITE;
                Raylib.DrawText(options[i], 500 + i * 100, yPos + 40, 24, col);
            }
        }
        else
        {
            string[] options = { "Load Save", "Delete Save" };
            for (int i = 0; i < options.Length; i++)
            {
                Color col = (i == selected) ? Color.YELLOW : Color.WHITE;
                Raylib.DrawText(options[i], 450 + i * 200, yPos, 24, col);
            }
        }
        
        Raylib.DrawText("Press Back to return", 500, 550, 18, Color.GRAY);
    }
}