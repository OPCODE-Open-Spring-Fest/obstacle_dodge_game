using UnityEngine;
using UnityEngine.SceneManagement;

public class Scorer : MonoBehaviour
{
    private int hits = 0;
    private int limit_hits;

    private void Start()
    {
        // Set hit limits based on scene/level
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        switch (currentScene)
        {
            case 4:
                limit_hits = 8; // More hits allowed for level 4
                break;
            case 5:
                limit_hits = 4; // Fewer hits allowed for level 5 (more challenging)
                break;
            default:
                limit_hits = 15; // Default hit limit for other levels
                break;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Hit")) return;

        hits++;
        Debug.Log($"You bumped into a thing this many times: {hits}");
        if (hits == limit_hits)
        {
            Debug.Log("You have reached the maximum number of hits!");
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }
    }
}
