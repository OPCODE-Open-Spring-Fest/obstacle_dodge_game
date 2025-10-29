
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scorer : MonoBehaviour
{
    [SerializeField] private int defaultLimit = 15;  // fallback if level not listed
    private int hits = 0;

    private void Start()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        switch (buildIndex)
        {
            case 1: defaultLimit = 15; break;
            case 4: defaultLimit = 8; break;
            case 5: defaultLimit = 4; break;
            default: defaultLimit = 15; break;
        }
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTag("Collectible")) return;

        if (other.gameObject.CompareTag("Hit")) return;

        var collectibles = GetComponent<PlayerCollectibles>();
        if (collectibles != null && collectibles.IsInvincible) return;

        if (collectibles != null)
        {
            collectibles.AddLife(-1);
            Debug.Log($"Player hit an obstacle. Lives now: {collectibles.GetLives()}");

            if (collectibles.GetLives() <= 0)
            {
                Debug.Log("No lives left. Loading Game Over scene...");
                LastLevelRecorder.SaveAndLoad("GameOver");
            }
            return;
        }

        hits++;
        Debug.Log($"You bumped into a thing this many times: {hits}");

        if (hits >= defaultLimit)
        {
            Debug.Log("Max hit limit reached! Loading Game Over scene...");
            LastLevelRecorder.SaveAndLoad("GameOver");
        }
    }
}
