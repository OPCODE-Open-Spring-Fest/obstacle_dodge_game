using UnityEngine;

/// <summary>
/// Detects when the player falls below a certain Y position threshold.
/// When the player falls, it immediately triggers game over.
/// This script is specifically designed for parkour levels.
/// </summary>
public class ParkourFallDetector : MonoBehaviour
{
    [Header("Fall Detection Settings")]
    [Tooltip("The Y position threshold. If player falls below this, game over is triggered.")]
    [SerializeField] private float fallThreshold = -10f;
    
    [Tooltip("The player GameObject. If not assigned, will search for GameObject with 'Player' tag.")]
    [SerializeField] private GameObject player;
    
    [Tooltip("Check player position every frame. If false, checks at fixed intervals.")]
    [SerializeField] private bool checkEveryFrame = true;
    
    [Tooltip("If checkEveryFrame is false, how often to check (in seconds).")]
    [SerializeField] private float checkInterval = 0.1f;
    
    [Header("Visual Feedback (Optional)")]
    [Tooltip("Optional: Play sound effect when player falls")]
    [SerializeField] private bool playFallSound = true;
    
    [Tooltip("Optional: Spawn an effect at the player position when they fall")]
    [SerializeField] private GameObject fallEffectPrefab;
    
    [Tooltip("Optional: Delay before loading game over scene (gives time for effects/sounds)")]
    [SerializeField] private float gameOverDelay = 0.5f;
    
    private float lastCheckTime;
    private bool gameOverTriggered = false;
    
    private void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj;
            }
            else
            {
                Debug.LogError("[ParkourFallDetector] Player not found! Make sure your player has the 'Player' tag assigned.");
            }
        }
        
        lastCheckTime = Time.time;
    }
    
    private void Update()
    {
        if (player == null || gameOverTriggered)
        {
            return;
        }
        
        // Check if it's time to check player position
        if (checkEveryFrame || Time.time - lastCheckTime >= checkInterval)
        {
            CheckPlayerFall();
            lastCheckTime = Time.time;
        }
    }
    
    private void CheckPlayerFall()
    {
        if (player.transform.position.y < fallThreshold)
        {
            TriggerGameOver();
        }
    }
    
    private void TriggerGameOver()
    {
        if (gameOverTriggered)
        {
            return; // Prevent multiple triggers
        }
        
        gameOverTriggered = true;
        Debug.Log($"[ParkourFallDetector] Player fell below threshold ({fallThreshold}). Game Over!");
        
        // Play fall sound
        if (playFallSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollisionSFX();
        }
        
        // Spawn fall effect
        if (fallEffectPrefab != null)
        {
            Instantiate(fallEffectPrefab, player.transform.position, Quaternion.identity);
        }
        
        // Trigger game over after delay
        Invoke(nameof(LoadGameOver), gameOverDelay);
    }
    
    private void LoadGameOver()
    {
        LastLevelRecorder.SaveAndLoad("GameOver");
    }
    
    /// <summary>
    /// Draws a visual line in the Scene view to show the fall threshold
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 start = new Vector3(-100f, fallThreshold, 0f);
        Vector3 end = new Vector3(100f, fallThreshold, 0f);
        Gizmos.DrawLine(start, end);
        
        // Draw a label
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(new Vector3(0f, fallThreshold, 0f), $"Fall Threshold: {fallThreshold}");
        #endif
    }
}

