using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLineTrigger : MonoBehaviour
{
    // Set the index of the scene you want to load
    [SerializeField] private int sceneIndexToLoad = 3; // Scene 3 index

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // Get current level index
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            
            // Find and stop the level timer
            LevelTimer timer = FindObjectOfType<LevelTimer>();
            if (timer != null)
            {
                float completionTime = timer.StopTimer();
                
                // Save the completion time if ProgressManager exists
                if (ProgressManager.Instance != null)
                {
                    ProgressManager.Instance.SaveLevelTime(currentLevelIndex, completionTime);
                    
                    // Unlock the next level
                    if (sceneIndexToLoad > 0)
                    {
                        ProgressManager.Instance.UnlockLevel(sceneIndexToLoad);
                    }
                }
            }
            else
            {
                Debug.LogWarning("LevelTimer not found! Completion time will not be saved.");
            }

            Debug.Log("Level Completed! Loading Scene " + sceneIndexToLoad);
            SceneManager.LoadScene(sceneIndexToLoad); // Load scene by index
        }
    }
}
