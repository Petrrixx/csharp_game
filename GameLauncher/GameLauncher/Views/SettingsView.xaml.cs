using System.Windows;
using GameLauncher.ViewModels;

namespace GameLauncher.Views
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }
    }
}