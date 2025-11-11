using UnityEngine;
 
public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    [Tooltip("The distance interval for setting a new checkpoint (e.g., every 500 units).")]
    public float checkpointDistanceInterval = 500f;

    [HideInInspector]
    public float LastCheckpointDistance { get; private set; } = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

 
    public void UpdateCheckpoint(float currentDistance)
    {
        int checkpointsPassed = Mathf.FloorToInt(currentDistance / checkpointDistanceInterval);
        
        float newCheckpointDistance = checkpointsPassed * checkpointDistanceInterval;

        if (newCheckpointDistance > LastCheckpointDistance)
        {
            LastCheckpointDistance = newCheckpointDistance;
            Debug.Log($"New session checkpoint set at: {LastCheckpointDistance}");
        }
    }

 
    public float GetSpawnDistance()
    {
        return LastCheckpointDistance;
    }
 
    public void ResetSession()
    {
        LastCheckpointDistance = 0f;
        Debug.Log("Session progress reset. Starting from 0.");
    }
}