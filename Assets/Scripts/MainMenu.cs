using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

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

    [Header("Button Click Sound")]
    public AudioClip clickSound;

    private void Start()
    {
        if (ProgressManager.Instance == null)
        {
            GameObject progressManagerObj = new GameObject("ProgressManager");
            progressManagerObj.AddComponent<ProgressManager>();
        }
        UpdateHighScoreDisplay();
    }

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

        if (highScoreText != null)
        {
            highScoreText.text = displayText;
        }

        if (highScoreTextLegacy != null)
        {
            highScoreTextLegacy.text = displayText;
        }
    }
    IEnumerator DelayedLoad(string sceneName)
    {
        if (clickSound != null)
        {
            AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position, 1f);
        }

        yield return new WaitForSeconds(0.15f);

        SceneManager.LoadScene(sceneName);
    }
    public void PlayGame()
    {
        if (!string.IsNullOrEmpty(levelSelectSceneName))
        {
            StartCoroutine(DelayedLoad(levelSelectSceneName));
        }
        else
        {
            StartCoroutine(DelayedLoad(SceneManager.GetSceneByBuildIndex(levelSelectBuildIndex).name));
        }
    }
    public void QuitGame()
    {
        if (clickSound != null)
            AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);

        StartCoroutine(QuitDelayed());
    }

    IEnumerator QuitDelayed()
    {
        yield return new WaitForSeconds(0.15f);

#if UNITY_EDITOR
    Debug.Log("Quit game (Editor)");
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log("Quit game (Build)");
        Application.Quit();
#endif
    }
    public void GoToShop()
    {
        StartCoroutine(DelayedLoad("Shop"));
        Debug.Log("Loading Shop Scene...");
    }
}
