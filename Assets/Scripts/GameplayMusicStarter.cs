using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple component to start gameplay music when a gameplay scene loads.
/// Attach this to any GameObject in your gameplay scenes.
/// </summary>
public class GameplayMusicStarter : MonoBehaviour
{
    [Tooltip("Check to start gameplay music on scene load")]
    [SerializeField] private bool playOnStart = true;

    private void Start()
    {
        if (playOnStart && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }
    }

    private void OnEnable()
    {
        // Also check when scene loads via SceneManager
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (playOnStart && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }
    }
}

