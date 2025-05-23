using System.Collections.ObjectModel;
using System.Windows.Input;
using GameLauncher.Helpers;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Windows.Threading;
using System.Windows;
using Raylib_cs;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace GameLauncher.ViewModels
{
    public class KeyBindingsViewModel : BaseViewModel
    {
        public ObservableCollection<KeyBindingVM> KeyBindings { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SetKeyCommand { get; }

        public ObservableCollection<string> InputDevices { get; } = new() { "Keyboard", "Gamepad" };
        private string _selectedInputDevice = "Keyboard";
        public string SelectedInputDevice
        {
            get => _selectedInputDevice;
            set
            {
                if (_selectedInputDevice != value)
                {
                    _selectedInputDevice = value;
                    OnPropertyChanged();
                    // Synchronize for all KeyBindingVM
                    foreach (var kb in KeyBindings)
                    {
                        kb.SelectedInputDevice = value;
                    }
                }
            }
        }

        private string? _waitingForAction = null;
        public bool IsWaitingForKey => _waitingForAction != null;
        public string WaitingForText => IsWaitingForKey ? $"Press a {(SelectedInputDevice == "Keyboard" ? "key" : "gamepad button")} for '{_waitingForAction}'..." : "";

        private DispatcherTimer _gamepadTimer;

        private bool _raylibWindowOpened = false;

        public KeyBindingsViewModel()
        {
            KeyBindings = new ObservableCollection<KeyBindingVM>
            {
                new KeyBindingVM("MoveUp", "W", "GAMEPAD_BUTTON_LEFT_FACE_UP"),
                new KeyBindingVM("MoveDown", "S", "GAMEPAD_BUTTON_LEFT_FACE_DOWN"),
                new KeyBindingVM("MoveLeft", "A", "GAMEPAD_BUTTON_LEFT_FACE_LEFT"),
                new KeyBindingVM("MoveRight", "D", "GAMEPAD_BUTTON_LEFT_FACE_RIGHT"),
                new KeyBindingVM("Confirm", "Enter", "GAMEPAD_BUTTON_RIGHT_FACE_DOWN"),
                new KeyBindingVM("Pause", "P", "GAMEPAD_BUTTON_MIDDLE_RIGHT"),
                new KeyBindingVM("Quit", "Escape", "GAMEPAD_BUTTON_MIDDLE_LEFT"),
                new KeyBindingVM("Back", "Backspace", "GAMEPAD_BUTTON_RIGHT_FACE_RIGHT")
            };

            SaveCommand = new RelayCommand(_ => SaveKeyBindings());
            CancelCommand = new RelayCommand(_ => Cancel());
            SetKeyCommand = new RelayCommand(param => { if (param is string s) StartKeyCapture(s); });

            _gamepadTimer = new DispatcherTimer { Interval = System.TimeSpan.FromMilliseconds(50) };
            _gamepadTimer.Tick += GamepadTimer_Tick;

            if (File.Exists("gamesettings.json"))
            {
                var json = File.ReadAllText("gamesettings.json");
                var settings = JsonSerializer.Deserialize<GameLauncher.Models.GameSettings>(json);
                if (settings != null)
                    SelectedInputDevice = settings.InputDevice;
            }
        }

        private void SaveKeyBindings()
        {
            string configDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "VampireSurvivorsClone"
            );
            
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }
            
            // Path location to keybindings.json AppData/Romaing/VampireSurvivorsClone
            string keybindingsPath = Path.Combine(configDirectory, "keybindings.json");
            
            // Serialize and save
            var json = JsonSerializer.Serialize(KeyBindings.Select(k => k.ToModel()).ToList(), 
                new JsonSerializerOptions { WriteIndented = true });
                
            File.WriteAllText(keybindingsPath, json);
            
            // Backwards compatibility
            File.WriteAllText("keybindings.json", json);
        }

        private void Cancel()
        {
            // Closes the window
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Content is FrameworkElement fe && fe.DataContext == this)
                {
                    window.Close();
                    return;
                }
            }
        }

        private async void StartKeyCapture(string action)
        {
            _waitingForAction = action;
            OnPropertyChanged(nameof(IsWaitingForKey));
            OnPropertyChanged(nameof(WaitingForText));

            if (SelectedInputDevice == "Gamepad")
            {
                if (!_raylibWindowOpened)
                {
                    Raylib.InitWindow(1, 1, "InputCapture");
                    _raylibWindowOpened = true;
                }

                await System.Threading.Tasks.Task.Run(() =>
                {
                    bool captured = false;
                    while (!captured && !Raylib.WindowShouldClose())
                    {
                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Color.BLANK); // Raylib event loop

                        for (int btn = 0; btn <= (int)GamepadButton.GAMEPAD_BUTTON_RIGHT_THUMB; btn++)
                        {
                            if (Raylib.IsGamepadButtonPressed(0, (GamepadButton)btn))
                            {
                                string buttonName = ((GamepadButton)btn).ToString();

                                Application.Current.Dispatcher.Invoke(() => SetKeyForAction(buttonName));
                                captured = true;
                                break;
                            }
                        }

                        Raylib.EndDrawing();
                        System.Threading.Thread.Sleep(10);
                    }
                });

                if (_raylibWindowOpened)
                {
                    Raylib.CloseWindow();
                    _raylibWindowOpened = false;
                }
            }
        }

        private void GamepadTimer_Tick(object? sender, System.EventArgs e)
        {
            if (SelectedInputDevice != "Gamepad" || !IsWaitingForKey)
                return;

            for (int btn = 0; btn <= (int)GamepadButton.GAMEPAD_BUTTON_RIGHT_THUMB; btn++)
            {
                if (Raylib.IsGamepadButtonPressed(0, (GamepadButton)btn))
                {
                    SetKeyForAction(((GamepadButton)btn).ToString());
                    break;
                }
            }
        }

        public void SetKeyForAction(string key)
        {
            var binding = KeyBindings.FirstOrDefault(b => b.Action == _waitingForAction);
            if (binding != null)
            {
                if (SelectedInputDevice == "Keyboard")
                    binding.Key = key;
                else
                    binding.Gamepad = key;
            }
            _waitingForAction = null;
            OnPropertyChanged(nameof(KeyBindings));
            OnPropertyChanged(nameof(IsWaitingForKey));
            OnPropertyChanged(nameof(WaitingForText));
            _gamepadTimer.Stop();
        }
    }

    public class KeyBindingVM : BaseViewModel
    {
        public string Action { get; }
        private string _key;
        private string _gamepad;
        private string _selectedInputDevice = "Keyboard";

        public string Key
        {
            get => _key;
            set { _key = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayKey)); }
        }
        public string Gamepad
        {
            get => _gamepad;
            set { _gamepad = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayKey)); }
        }

        public string SelectedInputDevice
        {
            get => _selectedInputDevice;
            set
            {
                if (_selectedInputDevice != value)
                {
                    _selectedInputDevice = value;
                    OnPropertyChanged(nameof(DisplayKey));
                }
            }
        }

        public string DisplayKey => SelectedInputDevice == "Gamepad" ? Gamepad : Key;

        public KeyBindingVM(string action, string key, string gamepad)
        {
            Action = action;
            _key = key;
            _gamepad = gamepad;
        }

        public GameLauncher.Models.KeyBinding ToModel() => new() { Action = Action, Key = Key, Gamepad = Gamepad };
    }
}