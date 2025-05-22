using System.Windows.Input;

namespace GameLauncher.Models
{
    public class KeyBinding
    {
        public string Action { get; set; } = "";
        public string Key { get; set; } = "";
        public string Gamepad { get; set; } = "";
    }
}