namespace VampireSurvivorsClone.Data
{
    public class GameSettings
    {
        public string Difficulty { get; set; } = "Normal";
        public bool IsFullscreen { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public string InputDevice { get; set; } = "Keyboard"; // "Keyboard" or "Gamepad"
    }
}