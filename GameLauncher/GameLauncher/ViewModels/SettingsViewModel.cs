using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GameLauncher.Models;
using GameLauncher.Helpers;
using GameLauncher.Views;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;

namespace GameLauncher.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private GameSettings _gameSettings = new GameSettings();

        public GameSettings GameSettings
        {
            get => _gameSettings;
            set
            {
                _gameSettings = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> InputDevices { get; } = new() { "Keyboard", "Gamepad" };
        public ObservableCollection<string> Difficulties { get; } = new() { "Easy", "Normal", "Hard" };

        public ICommand OpenKeyBindingsCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand QuitCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ExitCommand { get; }

        public SettingsViewModel()
        {
            OpenKeyBindingsCommand = new RelayCommand(_ => OpenKeyBindings());
            StartGameCommand = new RelayCommand(_ => StartGame());
            QuitCommand = new RelayCommand(_ => QuitGame());
            SaveCommand = new RelayCommand(_ => Save());
            ExitCommand = new RelayCommand(_ => Exit());
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

        private void StartGame()
        {
            Application.Current.Windows
                .OfType<Window>()
                .SingleOrDefault(w => w.IsActive)
                ?.Close();
        }

        private void QuitGame()
        {
            Application.Current.Shutdown();
        }

        private void Save()
        {
            var json = JsonSerializer.Serialize(GameSettings);
            File.WriteAllText("gamesettings.json", json);
            CloseWindow();
        }

        private void Exit()
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)
                ?.Close();
        }
    }
}