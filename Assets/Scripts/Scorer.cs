
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scorer : MonoBehaviour
{
    [SerializeField] private int defaultLimit = 15;  // fallback if level not listed
    private int hits = 0;

    private void Start()
    {
        // set hit limit based on level
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        switch (buildIndex)
        {
            case 1: defaultLimit = 15; break; // Level 1
            case 4: defaultLimit = 8; break; // Level 4
            case 5: defaultLimit = 4; break; // Level 5
            default: defaultLimit = 15; break; // other levels
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Hit")) return;

        hits++;
        Debug.Log($"You bumped into a thing this many times: {hits}");

        if (hits >= defaultLimit)
        {
            Debug.Log("Max hit limit reached! Loading Game Over scene...");
            LastLevelRecorder.SaveAndLoad("GameOver"); // This saves current level and loads GameOver
        }
    }
}
