using Raylib_cs;
using System.Numerics;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;

namespace VampireSurvivorsClone;

public static class Input
{
    public enum InputDevice { Keyboard, Gamepad }
    public static InputDevice CurrentDevice { get; set; } = InputDevice.Keyboard;

    private static Dictionary<string, KeyboardKey> KeyBindings = new();
    private static Dictionary<string, GamepadButton> GamepadBindings = new();

    static Input()
    {
        LoadKeyBindings();
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

            // Default gamepad mapping
            GamepadBindings["MoveUp"] = GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_UP;
            GamepadBindings["MoveDown"] = GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN;
            GamepadBindings["MoveLeft"] = GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_LEFT;
            GamepadBindings["MoveRight"] = GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_RIGHT;
            GamepadBindings["Confirm"] = GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN; // A
            GamepadBindings["Pause"] = GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT; // Menu
            GamepadBindings["Quit"] = GamepadButton.GAMEPAD_BUTTON_MIDDLE_LEFT; // View
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
            // fallback na D-Pad
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
        if (CurrentDevice == InputDevice.Gamepad && Raylib.IsGamepadAvailable(0))
        {
            if (GamepadBindings.TryGetValue(action, out var btn))
                return Raylib.IsGamepadButtonPressed(0, btn);
        }
        else
        {
            if (KeyBindings.TryGetValue(action, out var key))
                return Raylib.IsKeyPressed(key);
        }
        return false;
    }

    private class KeyBindingData
    {
        public string? Action { get; set; }
        public string? Key { get; set; }
        public string? Gamepad { get; set; }
    }
}
