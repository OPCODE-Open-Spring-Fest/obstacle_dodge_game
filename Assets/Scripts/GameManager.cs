using UnityEngine;
using UnityEngine.SceneManagement;

/// &lt;summary&gt;
/// A persistent Singleton to manage game state across scene loads.
/// This will store the player's checkpoint progress FOR THE CURRENT SESSION ONLY.
/// &lt;/summary&gt;
public class GameManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static GameManager Instance { get; private set; }

    // --- Configurable Fields ---
    [Tooltip("The distance interval for setting a new checkpoint (e.g., every 500 units).")]
    public float checkpointDistanceInterval = 500f;

    // --- Session State ---
    /// &lt;summary&gt;
    /// Stores the fardest checkpoint distance reached in this session.
    /// This is RESET to 0 when the game is first loaded or the player returns to the main menu.
    /// &lt;/summary&gt;
    public float LastCheckpointDistance { get; private set; } = 0f;

    private void Awake()
    {
        // --- Implement Singleton ---
        if (Instance == null)
        {
            Instance = this;
            // This object will persist across scene loads (e.g., when restarting)
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another GameManager already exists, destroy this one.
            Destroy(gameObject);
        }
    }

    /// &lt;summary&gt;
    /// The PlayerController calls this every frame to update its distance.
    /// &lt;/summary&gt;
    /// &lt;param name="currentDistance"&gt;Player's current distance from the start.&lt;/param&gt;
    public void UpdateCheckpoint(float currentDistance)
    {
        // Check if the player has passed a new checkpoint threshold
        // We use FloorToInt to get the number of checkpoints passed.
        int checkpointsPassed = Mathf.FloorToInt(currentDistance / checkpointDistanceInterval);
        
        // Calculate the distance of the checkpoint we just passed
        float newCheckpointDistance = checkpointsPassed * checkpointDistanceInterval;

        // If this new checkpoint is farther than our last saved one, save it.
        if (newCheckpointDistance > LastCheckpointDistance)
        {
            LastCheckpointDistance = newCheckpointDistance;
            Debug.Log($"New checkpoint set at: {LastCheckpointDistance}");
        }
    }

    /// &lt;summary&gt;
    /// Gets the distance at which the player should spawn.
    /// &lt;/summary&gt;
    public float GetSpawnDistance()
    {
        return LastCheckpointDistance;
    }

    /// &lt;summary&gt;
    /// Call this when the player explicitly restarts from the beginning or returns to the main menu.
    /// This resets the session progress.
    /// &lt;/summary&gt;
    public void ResetSession()
    {
        LastCheckpointDistance = 0f;
        Debug.Log("Session progress reset. Starting from 0.");
    }
}
