using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using GameLauncher.Helpers;
using GameLauncher.Views;

namespace GameLauncher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _selectedDifficulty = "Normal";
        private bool _isFullscreen = false;
        private int _screenWidth = 1280;
        private int _screenHeight = 720;

        public string SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                _selectedDifficulty = value;
                OnPropertyChanged();
            }
        }

        public bool IsFullscreen
        {
            get => _isFullscreen;
            set
            {
                _isFullscreen = value;
                OnPropertyChanged();
            }
        }

        public int ScreenWidth
        {
            get => _screenWidth;
            set
            {
                _screenWidth = value;
                OnPropertyChanged();
            }
        }

        public int ScreenHeight
        {
            get => _screenHeight;
            set
            {
                _screenHeight = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartGameCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenKeyBindingsCommand { get; }
        public ICommand QuitCommand { get; }

        public MainViewModel()
        {
            StartGameCommand = new RelayCommand(_ => StartGame());
            OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
            OpenKeyBindingsCommand = new RelayCommand(_ => OpenKeyBindings());
            QuitCommand = new RelayCommand(_ => QuitGame());
            // Initialize default settings
            SelectedDifficulty = "Normal";
            IsFullscreen = false;
            ScreenWidth = 1280;
            ScreenHeight = 720;
        }

        private void StartGame()
        {
            GameSettings? settings = null;
            if (File.Exists("gamesettings.json"))
                settings = JsonSerializer.Deserialize<GameSettings>(File.ReadAllText("gamesettings.json"));
            if (settings == null)
                settings = new GameSettings(); // default

            string difficulty = settings.Difficulty ?? "Normal";
            int width = settings.ScreenWidth;
            int height = settings.ScreenHeight;
            bool fullscreen = settings.IsFullscreen;

            var psi = new ProcessStartInfo
            {
                FileName = @"..\..\csharp_game\bin\Debug\net9.0\csharp_game.exe",
                Arguments = $"{difficulty} {width} {height} {(fullscreen ? "fullscreen" : "windowed")}",
                UseShellExecute = false
            };
            Process.Start(psi);

            Application.Current.Shutdown(); // close the launcher
        }

        private void OpenSettings()
        {
            var window = new SettingsView();
            window.ShowDialog();
        }

        private void OpenKeyBindings()
        {
            var window = new Window
            {
                Title = "Key Bindings",
                Content = new KeyBindingsView(),
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
        }

        private void QuitGame()
        {
            Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}