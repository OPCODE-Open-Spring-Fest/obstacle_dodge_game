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
            Debug.Log("Level Completed! Loading Scene " + sceneIndexToLoad);
            SceneManager.LoadScene(sceneIndexToLoad); // Load scene by index
        }
    }
}
