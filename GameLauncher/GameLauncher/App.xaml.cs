using System.Windows;

namespace GameLauncher
{
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += (s, e) =>
            {
                MessageBox.Show(e.Exception.ToString(), "Unhandled Exception");
                e.Handled = true;
            };
        }

        /*
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        */
        
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}