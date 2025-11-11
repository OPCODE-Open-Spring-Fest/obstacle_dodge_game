using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    [Header("Checkpoint Settings")]
    [Tooltip("How far (in meters) must the player run to set a new checkpoint?")]
    public float checkpointDistanceInterval = 500f;

    [Header("Lives Settings")]
    [Tooltip("The total number of lives the player starts a session with.")]
    public int maxLives = 3;

    public float LastCheckpointDistance { get; private set; } = 0f;
    public int CurrentLives { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CurrentLives = maxLives;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateCheckpoint(float currentDistance)
    {
        if (currentDistance >= LastCheckpointDistance + checkpointDistanceInterval)
        {
            LastCheckpointDistance = Mathf.Floor(currentDistance / checkpointDistanceInterval) * checkpointDistanceInterval;
            Debug.Log($"New session checkpoint set: {LastCheckpointDistance}m");
        }
    }

    public float GetSpawnDistance()
    {
        return LastCheckpointDistance;
    }

    public void SpendLife()
    {
        if (CurrentLives > 0)
        {
            CurrentLives--;
            Debug.Log($"Player spent a life, {CurrentLives} remaining.");
        }
    }

    public void ResetSession()
    {
        LastCheckpointDistance = 0f;
        CurrentLives = maxLives;
        Debug.Log("Session data reset. Lives and distance set to default.");
    }
}