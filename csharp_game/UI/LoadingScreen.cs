using Raylib_cs;
using System;

namespace VampireSurvivorsClone.UI
{
    public class LoadingScreen
    {
        private float animationTimer = 0f;
        private int dotCount = 0;
        private const float DOT_INTERVAL = 0.3f; // Interval for dot animation
        
        public void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
            
            string loadingText = "Loading" + new string('.', dotCount);
            
            // Font size and text width
            int fontSize = 40;
            int textWidth = Raylib.MeasureText(loadingText, fontSize);
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            
            // Centre the text on the screen
            Raylib.DrawText(
                loadingText, 
                screenWidth / 2 - textWidth / 2, 
                screenHeight / 2 - fontSize / 2, 
                fontSize, 
                Color.WHITE
            );
            
            Raylib.EndDrawing();
        }
        
        public void Update()
        {
            float deltaTime = Raylib.GetFrameTime();
            animationTimer += deltaTime;
            
            if (animationTimer >= DOT_INTERVAL)
            {
                dotCount = (dotCount + 1) % 4; // 0-3 dots
                animationTimer = 0f;
            }
        }
        
        // Method for showing the loading screen for a specific duration
        public void ShowForDuration(float durationSeconds)
        {
            float timer = 0f;
            
            while (timer < durationSeconds)
            {
                float deltaTime = Raylib.GetFrameTime();
                timer += deltaTime;
                
                Update();
                Draw();
            }
        }
    }
}