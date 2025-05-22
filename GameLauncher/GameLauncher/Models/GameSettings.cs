using System;
using GameLauncher.ViewModels;

namespace GameLauncher.Models
{
    public class GameSettings : BaseViewModel
    {
        private string _difficulty = "Normal";
        private bool _isFullscreen = false;
        private int _screenWidth = 1280;
        private int _screenHeight = 720;
        private string _inputDevice = "Keyboard";

        public string Difficulty
        {
            get => _difficulty;
            set { _difficulty = value; OnPropertyChanged(); }
        }
        public bool IsFullscreen
        {
            get => _isFullscreen;
            set { _isFullscreen = value; OnPropertyChanged(); }
        }
        public int ScreenWidth
        {
            get => _screenWidth;
            set { _screenWidth = value; OnPropertyChanged(); }
        }
        public int ScreenHeight
        {
            get => _screenHeight;
            set { _screenHeight = value; OnPropertyChanged(); }
        }
        public string InputDevice
        {
            get => _inputDevice;
            set { _inputDevice = value; OnPropertyChanged(); }
        }
    }
}