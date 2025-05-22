using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using GameLauncher.Models;
using GameLauncher.Views;
using GameLauncher.ViewModels;

namespace GameLauncher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
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

            // Starting the game
            var psi = new ProcessStartInfo
            {
                FileName = @"..\..\csharp_game\bin\Debug\net9.0\csharp_game.exe",
                Arguments = $"{difficulty} {width} {height} {(fullscreen ? "fullscreen" : "windowed")}",
                UseShellExecute = false
            };
            Process.Start(psi);

            Application.Current.Shutdown(); // close the launcher
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new Views.SettingsView
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
        }

        private void KeyBindingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window
            {
                Title = "Key Bindings",
                Content = new KeyBindingsView(),
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            window.ShowDialog();
        }
    }

    public class GameSettings
    {
        public string Difficulty { get; set; } = "Normal";
        public bool IsFullscreen { get; set; } = false;
        public int ScreenWidth { get; set; } = 1280;
        public int ScreenHeight { get; set; } = 720;
        public string InputDevice { get; set; } = "Keyboard";   // "Keyboard" alebo "Gamepad"
    }
}