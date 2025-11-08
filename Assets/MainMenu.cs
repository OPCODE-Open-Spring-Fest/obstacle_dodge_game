using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Tooltip("Name of the scene to open when Play is clicked. If empty, Build Index below will be used.")]
    public string levelSelectSceneName = "Levels";

    [Tooltip("Fallback build index to load if the scene name is empty or not set in Inspector.")]
    public int levelSelectBuildIndex = 1;

    [Header("High Score Display")]
    [Tooltip("Optional TextMeshProUGUI component to display the high score. Leave empty if not needed.")]
    public TextMeshProUGUI highScoreText;

    [Tooltip("Optional Text component to display the high score. Leave empty if not needed.")]
    public UnityEngine.UI.Text highScoreTextLegacy;

    [Tooltip("Prefix text for high score display (e.g., 'Best Time: ').")]
    public string highScorePrefix = "Best Time: ";

    private void Start()
    {
        // Ensure ProgressManager exists and loads progress
        if (ProgressManager.Instance == null)
        {
            GameObject progressManagerObj = new GameObject("ProgressManager");
            progressManagerObj.AddComponent<ProgressManager>();
        }

        // Display high score
        UpdateHighScoreDisplay();
    }

    /// <summary>
    /// Updates the high score display with the saved high score
    /// </summary>
    private void UpdateHighScoreDisplay()
    {
        if (ProgressManager.Instance == null)
        {
            return;
        }

        float highScore = ProgressManager.Instance.GetHighScore();
        string displayText = highScorePrefix;

        if (highScore > 0)
        {
            displayText += ProgressManager.FormatTime(highScore);
        }
        else
        {
            displayText += "N/A";
        }

        // Update TextMeshProUGUI if assigned
        if (highScoreText != null)
        {
            highScoreText.text = displayText;
        }

        // Update legacy Text component if assigned
        if (highScoreTextLegacy != null)
        {
            highScoreTextLegacy.text = displayText;
        }
    }

    // Called by the Play button on the main menu. By default it will open the Level Select scene.
    public void PlayGame()
    {
        if (!string.IsNullOrEmpty(levelSelectSceneName))
        {
            SceneManager.LoadScene(levelSelectSceneName);
        }
        else
        {
            SceneManager.LoadScene(levelSelectBuildIndex);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
