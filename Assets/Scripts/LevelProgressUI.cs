using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays level progress information (best times, unlocked status) on the Levels scene.
/// Attach this to a GameObject in the Levels scene and assign level buttons and text components.
/// </summary>
public class LevelProgressUI : MonoBehaviour
{
    [System.Serializable]
    public class LevelButtonData
    {
        [Tooltip("The level button GameObject")]
        public GameObject levelButton;
        
        [Tooltip("The level build index (scene index)")]
        public int levelIndex;
        
        [Tooltip("Optional: Text component to display best time. If null, will search for Text or TextMeshProUGUI in children.")]
        public Text timeText;
        
        [Tooltip("Optional: TextMeshProUGUI component to display best time. If null, will search for Text or TextMeshProUGUI in children.")]
        public TextMeshProUGUI timeTextTMP;
        
        [Tooltip("Optional: Text component to display 'Locked' status. If null, will search for Text or TextMeshProUGUI in children.")]
        public Text lockedText;
        
        [Tooltip("Optional: TextMeshProUGUI component to display 'Locked' status. If null, will search for Text or TextMeshProUGUI in children.")]
        public TextMeshProUGUI lockedTextTMP;
    }

    [Header("Level Buttons Configuration")]
    [Tooltip("Array of level button data. Configure each level's button and display components.")]
    public LevelButtonData[] levelButtons;

    [Header("UI Settings")]
    [Tooltip("Text to show when level is locked")]
    public string lockedText = "LOCKED";
    
    [Tooltip("Text to show when level has no best time yet")]
    public string noTimeText = "Not Completed";

    private void Start()
    {
        // Ensure ProgressManager exists
        if (ProgressManager.Instance == null)
        {
            GameObject progressManagerObj = new GameObject("ProgressManager");
            progressManagerObj.AddComponent<ProgressManager>();
        }

        UpdateLevelDisplays();
    }

    /// <summary>
    /// Updates all level button displays with best times and unlock status
    /// </summary>
    public void UpdateLevelDisplays()
    {
        if (levelButtons == null || levelButtons.Length == 0)
        {
            Debug.LogWarning("LevelProgressUI: No level buttons configured!");
            return;
        }

        foreach (var levelData in levelButtons)
        {
            if (levelData.levelButton == null)
            {
                continue;
            }

            // All levels are always unlocked - keep buttons clickable
            Button button = levelData.levelButton.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = true; // Always enable buttons
            }

            // Update time display (show for all levels)
            UpdateTimeDisplay(levelData, true);

            // Hide locked status display (all levels unlocked)
            UpdateLockedDisplay(levelData, true);
        }
    }

    private void UpdateTimeDisplay(LevelButtonData levelData, bool isUnlocked)
    {
        // Always show time display (all levels are unlocked)
        // Get best time for this level
        float bestTime = -1f;
        if (ProgressManager.Instance != null)
        {
            bestTime = ProgressManager.Instance.GetLevelBestTime(levelData.levelIndex);
        }

        string timeString = bestTime > 0 ? ProgressManager.FormatTime(bestTime) : noTimeText;

        // Try to update Text component
        if (levelData.timeText != null)
        {
            levelData.timeText.text = timeString;
        }
        else if (levelData.timeTextTMP != null)
        {
            levelData.timeTextTMP.text = timeString;
        }
        else
        {
            // Try to find Text or TextMeshProUGUI in children
            Text foundText = levelData.levelButton.GetComponentInChildren<Text>();
            if (foundText != null)
            {
                foundText.text = timeString;
            }
            else
            {
                TextMeshProUGUI foundTMP = levelData.levelButton.GetComponentInChildren<TextMeshProUGUI>();
                if (foundTMP != null)
                {
                    foundTMP.text = timeString;
                }
            }
        }
    }

    private void UpdateLockedDisplay(LevelButtonData levelData, bool isUnlocked)
    {
        if (isUnlocked)
        {
            // Hide locked text if level is unlocked
            if (levelData.lockedText != null)
            {
                levelData.lockedText.gameObject.SetActive(false);
            }
            if (levelData.lockedTextTMP != null)
            {
                levelData.lockedTextTMP.gameObject.SetActive(false);
            }
        }
        else
        {
            // Show locked text if level is locked
            if (levelData.lockedText != null)
            {
                levelData.lockedText.text = lockedText;
                levelData.lockedText.gameObject.SetActive(true);
            }
            else if (levelData.lockedTextTMP != null)
            {
                levelData.lockedTextTMP.text = lockedText;
                levelData.lockedTextTMP.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Call this method to refresh displays (useful when returning to Levels scene)
    /// </summary>
    public void RefreshDisplays()
    {
        UpdateLevelDisplays();
    }
}

