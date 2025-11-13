using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLineTrigger : MonoBehaviour
{
    
    [SerializeField] private int sceneIndexToLoad = 3; 

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            
            LevelTimer timer = FindObjectOfType<LevelTimer>();  

            if (timer != null)
            {
                float completionTime = timer.StopTimer();
                
                if (ProgressManager.Instance != null)
                {
                    ProgressManager.Instance.SaveLevelTime(currentLevelIndex, completionTime);
                    
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
            SceneManager.LoadScene(sceneIndexToLoad); 
        }
    }
}