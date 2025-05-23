using Raylib_cs;
using System.Numerics;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using VampireSurvivorsClone.Data;

namespace VampireSurvivorsClone.Engine;
public static class Input
{
    public enum InputDevice { Keyboard, Gamepad }
    private static InputDevice _currentDevice = InputDevice.Keyboard;
    public static InputDevice CurrentDevice 
    { 
        get => _currentDevice; 
        set 
        { 
            if (_currentDevice != value)
            {
                _currentDevice = value;
                // Aktualizovať nastavenie v súbore pri zmene zariadenia
                UpdateGameSettingsFile();
            }
        }
    }

    private static Dictionary<string, KeyboardKey> KeyBindings = new();
    private static Dictionary<string, GamepadButton> GamepadBindings = new();

    static Input()
    {
        LoadKeyBindings();
    }

    // Metóda na aktualizáciu gamesettings.json
    private static void UpdateGameSettingsFile()
    {
        try
        {
            // Správna relatívna cesta k súboru od koreňa projektu
            string filePath = "..\\..\\..\\..\\GameLauncher\\GameLauncher\\gamesettings.json";
            
            // Načítaj existujúce nastavenia ak existujú, inak vytvor nové
            GameSettings settings = File.Exists(filePath) 
                ? JsonSerializer.Deserialize<GameSettings>(File.ReadAllText(filePath)) ?? new GameSettings() 
                : new GameSettings();

            // Aktualizuj InputDevice
            settings.InputDevice = CurrentDevice == InputDevice.Gamepad ? "Gamepad" : "Keyboard";
            
            // Ulož nastavenia jednoducho ako v Save() metóde
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chyba pri aktualizácii gamesettings.json: {ex.Message}");
        }
    }

    public static void LoadKeyBindings(string path = "keybindings.json")
    {
        if (!File.Exists(path))
        {
            // Default keybindings
            KeyBindings["MoveUp"] = KeyboardKey.KEY_W;
            KeyBindings["MoveDown"] = KeyboardKey.KEY_S;
            KeyBindings["MoveLeft"] = KeyboardKey.KEY_A;
            KeyBindings["MoveRight"] = KeyboardKey.KEY_D;
            KeyBindings["Confirm"] = KeyboardKey.KEY_ENTER;
            KeyBindings["Pause"] = KeyboardKey.KEY_P;
            KeyBindings["Quit"] = KeyboardKey.KEY_ESCAPE;
            KeyBindings["Back"] = KeyboardKey.KEY_BACKSPACE;  

            // Default gamepad mapping
            GamepadBindings["MoveUp"] = GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_UP;
            GamepadBindings["MoveDown"] = GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN;
            GamepadBindings["MoveLeft"] = GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_LEFT;
            GamepadBindings["MoveRight"] = GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_RIGHT;
            GamepadBindings["Confirm"] = GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN; // A
            GamepadBindings["Pause"] = GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT; // Menu
            GamepadBindings["Quit"] = GamepadButton.GAMEPAD_BUTTON_MIDDLE_LEFT; // View
            GamepadBindings["Back"] = GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_RIGHT; // B
            return;
        }

        var json = File.ReadAllText(path);
        var bindings = JsonSerializer.Deserialize<List<KeyBindingData>>(json);
        if (bindings != null)
        {
            foreach (var b in bindings)
            {
                if (!string.IsNullOrEmpty(b.Action) && !string.IsNullOrEmpty(b.Key) && Enum.TryParse<KeyboardKey>("KEY_" + b.Key.ToUpper(), out var key))
                    KeyBindings[b.Action] = key;
                if (!string.IsNullOrEmpty(b.Action) && !string.IsNullOrEmpty(b.Gamepad) && Enum.TryParse<GamepadButton>(b.Gamepad, out var btn))
                    GamepadBindings[b.Action] = btn;
            }
        }
    }

    // Method to get player movement direction
    public static Vector2 GetMovementDirection()
    {
        if (CurrentDevice == InputDevice.Gamepad && Raylib.IsGamepadAvailable(0))
        {
            float x = Raylib.GetGamepadAxisMovement(0, GamepadAxis.GAMEPAD_AXIS_LEFT_X);
            float y = Raylib.GetGamepadAxisMovement(0, GamepadAxis.GAMEPAD_AXIS_LEFT_Y);
            Vector2 dir = new(x, y);
            if (dir.Length() > 0.2f) return Vector2.Normalize(dir);
            // fallback for D-Pad
            Vector2 dpad = Vector2.Zero;
            if (Raylib.IsGamepadButtonDown(0, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_UP)) dpad.Y -= 1;
            if (Raylib.IsGamepadButtonDown(0, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN)) dpad.Y += 1;
            if (Raylib.IsGamepadButtonDown(0, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_LEFT)) dpad.X -= 1;
            if (Raylib.IsGamepadButtonDown(0, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_RIGHT)) dpad.X += 1;
            return dpad;
        }
        else
        {
            Vector2 direction = Vector2.Zero;
            if (IsActionDown("MoveUp")) direction.Y -= 1;
            if (IsActionDown("MoveDown")) direction.Y += 1;
            if (IsActionDown("MoveLeft")) direction.X -= 1;
            if (IsActionDown("MoveRight")) direction.X += 1;
            return direction;
        }
    }

    public static bool IsActionDown(string action)
    {
        if (CurrentDevice == InputDevice.Gamepad && Raylib.IsGamepadAvailable(0))
        {
            if (GamepadBindings.TryGetValue(action, out var btn))
                return Raylib.IsGamepadButtonDown(0, btn);
        }
        else
        {
            if (KeyBindings.TryGetValue(action, out var key))
                return Raylib.IsKeyDown(key);
        }
        return false;
    }

    public static bool IsActionPressed(string action)
    {
        bool keyboardPressed = false;
        bool gamepadPressed = false;

        if (KeyBindings.TryGetValue(action, out var key))
            keyboardPressed = Raylib.IsKeyPressed(key);

        if (Raylib.IsGamepadAvailable(0) && GamepadBindings.TryGetValue(action, out var btn))
            gamepadPressed = Raylib.IsGamepadButtonPressed(0, btn);

        return keyboardPressed || gamepadPressed;
    }

    public static void UpdateInputDevice()
    {
        // If gamepad is available and any button is pressed, switch to gamepad
        if (Raylib.IsGamepadAvailable(0))
        {
            for (int btn = 0; btn <= (int)GamepadButton.GAMEPAD_BUTTON_RIGHT_THUMB; btn++)
            {
                if (Raylib.IsGamepadButtonPressed(0, (GamepadButton)btn))
                {
                    CurrentDevice = InputDevice.Gamepad;
                    UpdateGameSettingsFile();
                    return;
                }
            }
        }
        // If no gamepad is available or any keyboard key is pressed, switch to keyboard
        for (int key = 32; key < 350; key++)
        {
            if (Raylib.IsKeyPressed((KeyboardKey)key))
            {
                CurrentDevice = InputDevice.Keyboard;
                UpdateGameSettingsFile();
                return;
            }
        }
    }

    private class KeyBindingData
    {
        public string? Action { get; set; }
        public string? Key { get; set; }
        public string? Gamepad { get; set; }
    }
}
