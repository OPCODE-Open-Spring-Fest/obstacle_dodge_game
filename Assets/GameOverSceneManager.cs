using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

public class GameOverSceneManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "MainGame";
    public string mainMenuSceneName = "MainMenu";

    [Header("UI Buttons")]
    public Button restartFromCheckpointButton;
    public Button restartFromBeginningButton;
    public Button mainMenuButton;

    public TextMeshProUGUI checkpointButtonText;

    void Start()
    {
        if (SessionManager.Instance == null)
        {
            Debug.LogError("SessionManager not found! Cannot check for checkpoints.");
            if (restartFromCheckpointButton != null)
            {
                restartFromCheckpointButton.gameObject.SetActive(false);
            }
        }
        else
        {
            if (SessionManager.Instance.CurrentLives > 0 && SessionManager.Instance.LastCheckpointDistance > 0)
            {
                if (restartFromCheckpointButton != null)
                {
                    restartFromCheckpointButton.gameObject.SetActive(true);
                }

                if (checkpointButtonText != null)
                {
                    checkpointButtonText.text = $"Restart from Checkpoint (Lives: {SessionManager.Instance.CurrentLives})";
                }
            }
            else
            {
                if (restartFromCheckpointButton != null)
                {
                    restartFromCheckpointButton.gameObject.SetActive(false);
                }
            }
        }

        if (restartFromCheckpointButton != null)
        {
            restartFromCheckpointButton.onClick.AddListener(OnRestartFromCheckpoint);
        }
        
        if (restartFromBeginningButton != null)
        {
            restartFromBeginningButton.onClick.AddListener(OnRestartFromBeginning);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenu);
        }
    }

    public void OnRestartFromCheckpoint()
    {
        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.SpendLife();
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnRestartFromBeginning()
    {
        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.ResetSession();
        }
        
        Time.timeScale = 1;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnMainMenu()
    {
        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.ResetSession();
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}