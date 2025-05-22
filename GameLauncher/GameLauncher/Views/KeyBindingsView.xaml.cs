using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GameLauncher.ViewModels;

namespace GameLauncher.Views
{
    public partial class KeyBindingsView : UserControl
    {
        public KeyBindingsView()
        {
            InitializeComponent();
            DataContext = new KeyBindingsViewModel();
            this.PreviewKeyDown += KeyBindingsView_PreviewKeyDown;
        }

        private void KeyBindingsView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var vm = DataContext as KeyBindingsViewModel;
            if (vm?.IsWaitingForKey == true && vm.SelectedInputDevice == "Keyboard")
            {
                vm.SetKeyForAction(e.Key.ToString());
                e.Handled = true;
            }
        }
    }
}