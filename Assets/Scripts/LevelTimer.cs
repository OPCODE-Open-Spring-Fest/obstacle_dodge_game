using UnityEngine;

/// <summary>
/// Tracks the time elapsed since the level started.
/// Used to calculate completion time for saving best times.
/// </summary>
public class LevelTimer : MonoBehaviour
{
    private float levelStartTime;
    private bool isTimerRunning = false;
    private int currentLevelIndex;

    private void Start()
    {
        StartTimer();
    }

    /// <summary>
    /// Starts the timer when level begins
    /// </summary>
    public void StartTimer()
    {
        levelStartTime = Time.time;
        isTimerRunning = true;
        currentLevelIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        Debug.Log($"Level {currentLevelIndex} timer started");
    }

    /// <summary>
    /// Stops the timer and returns the elapsed time
    /// </summary>
    /// <returns>Time elapsed in seconds</returns>
    public float StopTimer()
    {
        if (!isTimerRunning)
        {
            Debug.LogWarning("Timer was not running!");
            return 0f;
        }

        float elapsedTime = Time.time - levelStartTime;
        isTimerRunning = false;
        Debug.Log($"Level {currentLevelIndex} completed in {elapsedTime:F2} seconds");
        return elapsedTime;
    }

    /// <summary>
    /// Gets the current elapsed time without stopping the timer
    /// </summary>
    /// <returns>Time elapsed in seconds</returns>
    public float GetCurrentTime()
    {
        if (!isTimerRunning)
        {
            return 0f;
        }
        return Time.time - levelStartTime;
    }

    /// <summary>
    /// Checks if the timer is currently running
    /// </summary>
    public bool IsRunning()
    {
        return isTimerRunning;
    }

    /// <summary>
    /// Resets the timer (useful for restarting a level)
    /// </summary>
    public void ResetTimer()
    {
        levelStartTime = Time.time;
        Debug.Log($"Level {currentLevelIndex} timer reset");
    }
}

