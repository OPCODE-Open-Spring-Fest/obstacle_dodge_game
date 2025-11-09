using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class LevelTimer : MonoBehaviour
{
    public Text countdownText; // Assign in inspector
    public float countdownFrom = 3f;

    private float levelStartTime;
    private bool isTimerRunning = false;
    private int currentLevelIndex;

    private void Start()
    {
        StartCoroutine(CountdownAndStart());
    }

    private IEnumerator CountdownAndStart()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }
        
        Time.timeScale = 0f; // Pause the game

        float countdown = countdownFrom;
        while (countdown > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = Mathf.Ceil(countdown).ToString();
            }
            yield return new WaitForSecondsRealtime(1f);
            countdown--;
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        
        Time.timeScale = 1f; // Resume the game
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
