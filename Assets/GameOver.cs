using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [Tooltip("If set, this scene name will be loaded when Retry is pressed.")]
    public string retrySceneName = "";

    [Tooltip("Fallback build index to load when Retry is pressed. Use -1 to ignore.")]
    public int retrySceneBuildIndex = -1;

    [Tooltip("Optional PlayerPrefs key that stores the last played level build index. If set and present, it will be used as a fallback.")]
    public string lastLevelPrefsKey = "LastLevelIndex";

    public void Retry()
    {
        // 1) If scene name provided in inspector, load it
        if (!string.IsNullOrEmpty(retrySceneName))
        {
            SceneManager.LoadScene(retrySceneName);
            return;
        }

        // 2) If a valid build index is provided in inspector, load it
        if (retrySceneBuildIndex >= 0)
        {
            SceneManager.LoadScene(retrySceneBuildIndex);
            return;
        }

        // 3) If a PlayerPrefs key exists with last level index, use that
        if (!string.IsNullOrEmpty(lastLevelPrefsKey) && PlayerPrefs.HasKey(lastLevelPrefsKey))
        {
            int idx = PlayerPrefs.GetInt(lastLevelPrefsKey, -1);
            if (idx >= 0)
            {
                SceneManager.LoadScene(idx);
                return;
            }
        }

        // 4) Fallback to the previous hard-coded behaviour (scene index 1)
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
