using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Tooltip("Name of the scene to open when Play is clicked. If empty, Build Index below will be used.")]
    public string levelSelectSceneName = "Levels";

    [Tooltip("Fallback build index to load if the scene name is empty or not set in Inspector.")]
    public int levelSelectBuildIndex = 1;

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
