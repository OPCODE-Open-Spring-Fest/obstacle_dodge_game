using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This script goes in your "GameOver" scene.
/// It checks the SessionManager to see if it should show the
/// "Restart from Checkpoint" button.
/// </summary>
public class GameOverSceneManager : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("The name of your MAIN GAME scene")]
    public string gameSceneName = "MainGame"; // Make sure this matches your game scene's name
    
    [Tooltip("The name of your MAIN MENU scene")]
    public string mainMenuSceneName = "MainMenu"; // Make sure this matches your menu scene's name

    [Header("UI Buttons")]
    [Tooltip("Assign your 'Restart from Checkpoint' button here")]
    public Button restartFromCheckpointButton;
    
    [Tooltip("Assign your 'Restart from Beginning' button here")]
    public Button restartFromBeginningButton;
    
    [Tooltip("Assign your 'Main Menu' button here")]
    public Button mainMenuButton;

    void Start()
    {
        // Make sure the SessionManager exists
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
            // --- This is the main checkpoint logic ---
            if (SessionManager.Instance.LastCheckpointDistance > 0)
            {
                // Player has checkpoint data, so show the button
                if (restartFromCheckpointButton != null)
                {
                    restartFromCheckpointButton.gameObject.SetActive(true);
                }
            }
            else
            {
                // No checkpoint data, so hide the button
                if (restartFromCheckpointButton != null)
                {
                    restartFromCheckpointButton.gameObject.SetActive(false);
                }
            }
        }

        // --- Add Listeners ---
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
        // We DON'T reset the session. Just load the game scene.
        // The EndlessGameManager will read the checkpoint distance from SessionManager.
        Time.timeScale = 1;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnRestartFromBeginning()
    {
        // We MUST reset the session data before loading.
        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.ResetSession();
        }
        
        Time.timeScale = 1;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnMainMenu()
    {
        // We MUST reset the session data before loading.
        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.ResetSession();
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}