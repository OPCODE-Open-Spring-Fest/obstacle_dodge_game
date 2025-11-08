using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton manager for saving and loading player progress.
/// Handles high scores (best times per level) and unlocked levels.
/// Uses PlayerPrefs for data persistence.
/// </summary>
public class ProgressManager : MonoBehaviour
{
    private const string HIGH_SCORE_KEY = "HighScore";
    private const string LEVEL_TIME_PREFIX = "LevelTime_";
    private const string LEVEL_UNLOCKED_PREFIX = "LevelUnlocked_";
    private const string DATA_VERSION_KEY = "ProgressDataVersion";
    private const int CURRENT_DATA_VERSION = 1;

    public static ProgressManager Instance { get; private set; }

    private Dictionary<int, float> levelBestTimes;
    private Dictionary<int, bool> levelUnlocked;
    private float overallHighScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Loads all saved progress from PlayerPrefs
    /// </summary>
    public void LoadProgress()
    {
        levelBestTimes = new Dictionary<int, float>();
        levelUnlocked = new Dictionary<int, bool>();

        // Check data version for future migration support
        int savedVersion = PlayerPrefs.GetInt(DATA_VERSION_KEY, 0);
        if (savedVersion != CURRENT_DATA_VERSION)
        {
            Debug.Log($"Progress data version mismatch. Current: {CURRENT_DATA_VERSION}, Saved: {savedVersion}");
        }

        // Load overall high score (best time across all levels)
        overallHighScore = PlayerPrefs.GetFloat(HIGH_SCORE_KEY, 0f);

        // Load level-specific data
        // We'll check for levels 1-10 (adjust range as needed)
        for (int levelIndex = 1; levelIndex <= 10; levelIndex++)
        {
            string timeKey = LEVEL_TIME_PREFIX + levelIndex;
            string unlockKey = LEVEL_UNLOCKED_PREFIX + levelIndex;

            if (PlayerPrefs.HasKey(timeKey))
            {
                float bestTime = PlayerPrefs.GetFloat(timeKey, float.MaxValue);
                if (bestTime < float.MaxValue)
                {
                    levelBestTimes[levelIndex] = bestTime;
                }
            }

            // Level 1 is always unlocked by default
            if (levelIndex == 1)
            {
                levelUnlocked[levelIndex] = true;
            }
            else
            {
                levelUnlocked[levelIndex] = PlayerPrefs.GetInt(unlockKey, 0) == 1;
            }
        }

        Debug.Log("Progress loaded successfully");
    }

    /// <summary>
    /// Saves completion time for a level. Only saves if it's a new best time.
    /// </summary>
    /// <param name="levelIndex">Build index of the level scene</param>
    /// <param name="completionTime">Time taken to complete the level in seconds</param>
    public void SaveLevelTime(int levelIndex, float completionTime)
    {
        if (completionTime <= 0)
        {
            Debug.LogWarning($"Invalid completion time: {completionTime} for level {levelIndex}");
            return;
        }

        string timeKey = LEVEL_TIME_PREFIX + levelIndex;
        bool isNewBest = false;

        // Check if this is a new best time
        if (levelBestTimes.ContainsKey(levelIndex))
        {
            if (completionTime < levelBestTimes[levelIndex])
            {
                isNewBest = true;
                levelBestTimes[levelIndex] = completionTime;
            }
        }
        else
        {
            // First completion of this level
            isNewBest = true;
            levelBestTimes[levelIndex] = completionTime;
        }

        if (isNewBest)
        {
            PlayerPrefs.SetFloat(timeKey, completionTime);
            PlayerPrefs.Save();

            // Update overall high score if this is the best time across all levels
            if (overallHighScore == 0f || completionTime < overallHighScore)
            {
                overallHighScore = completionTime;
                PlayerPrefs.SetFloat(HIGH_SCORE_KEY, overallHighScore);
                PlayerPrefs.Save();
            }

            Debug.Log($"New best time for Level {levelIndex}: {completionTime:F2}s");
        }
    }

    /// <summary>
    /// Gets the best time for a specific level
    /// </summary>
    /// <param name="levelIndex">Build index of the level scene</param>
    /// <returns>Best time in seconds, or -1 if level hasn't been completed</returns>
    public float GetLevelBestTime(int levelIndex)
    {
        if (levelBestTimes.ContainsKey(levelIndex))
        {
            return levelBestTimes[levelIndex];
        }
        return -1f; // Level not completed yet
    }

    /// <summary>
    /// Gets the overall high score (best time across all levels)
    /// </summary>
    /// <returns>Best time in seconds, or 0 if no levels completed</returns>
    public float GetHighScore()
    {
        return overallHighScore;
    }

    /// <summary>
    /// Unlocks a level
    /// </summary>
    /// <param name="levelIndex">Build index of the level scene</param>
    public void UnlockLevel(int levelIndex)
    {
        if (!levelUnlocked.ContainsKey(levelIndex) || !levelUnlocked[levelIndex])
        {
            levelUnlocked[levelIndex] = true;
            string unlockKey = LEVEL_UNLOCKED_PREFIX + levelIndex;
            PlayerPrefs.SetInt(unlockKey, 1);
            PlayerPrefs.Save();
            Debug.Log($"Level {levelIndex} unlocked!");
        }
    }

    /// <summary>
    /// Checks if a level is unlocked
    /// All levels are always unlocked (no restrictions)
    /// </summary>
    /// <param name="levelIndex">Build index of the level scene</param>
    /// <returns>Always returns true (all levels unlocked)</returns>
    public bool IsLevelUnlocked(int levelIndex)
    {
        // All levels are always unlocked - no restrictions
        return true;
    }

    /// <summary>
    /// Formats time in seconds to a readable string (MM:SS.ms)
    /// </summary>
    public static string FormatTime(float timeInSeconds)
    {
        if (timeInSeconds < 0)
        {
            return "N/A";
        }

        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);

        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    /// <summary>
    /// Resets all progress (for testing/debugging)
    /// </summary>
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        levelBestTimes.Clear();
        levelUnlocked.Clear();
        overallHighScore = 0f;
        Debug.Log("All progress has been reset");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}

