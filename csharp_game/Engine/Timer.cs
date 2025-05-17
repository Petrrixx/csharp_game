namespace VampireSurvivorsClone.Engine;
public class Timer
{
    public float TimeRemaining { get; private set; } = 30f;  // Start with 30 seconds
    public bool IsWaveReady => TimeRemaining <= 0f;

    public void Update(float deltaTime)
    {
        if (TimeRemaining > 0)
        {
            TimeRemaining -= deltaTime;
        }
    }

    public void Reset(float newTime)
    {
        TimeRemaining = newTime;
    }
}
// This class handles the countdown timer for the game waves. It tracks the time remaining and can reset the timer when needed.
// The IsWaveReady property indicates whether the timer has reached zero, allowing the game to spawn new enemies or waves.
// The Update method decreases the time remaining based on the delta time, and the Reset method allows for setting a new countdown duration.