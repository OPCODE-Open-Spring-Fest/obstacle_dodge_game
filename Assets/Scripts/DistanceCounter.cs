using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DistanceCounter : MonoBehaviour
{
    [Header("Distance Settings")]
    [Tooltip("Starting position Z coordinate (will be set automatically)")]
    public float startZ; // CHANGED: This is now set by SetInitialDistance or ResetDistance
    
    [Tooltip("Distance unit (e.g., 'm' for meters)")]
    public string distanceUnit = "m";
    
    [Tooltip("Update distance every N frames (for performance)")]
    public int updateInterval = 1;
    
    [Header("UI Settings")]
    [Tooltip("Text component to display distance (TextMeshPro or Unity Text)")]
    public TextMeshProUGUI distanceTextTMP;
    
    [Tooltip("Text component to display distance (Unity Text - alternative to TMP)")]
    public Text distanceText;
    
    [Tooltip("Format string for distance display (e.g., 'Distance: {0:F1} {1}')")]
    public string distanceFormat = "Distance: {0:F1} {1}";
    
    [Header("High Score Settings")]
    [Tooltip("Save best distance to PlayerPrefs")]
    public bool saveBestDistance = true;
    
    [Tooltip("PlayerPrefs key for best distance")]
    public string bestDistanceKey = "BestDistance";
    
    private Transform player;
    private float currentDistance = 0f;
    private float distanceOffset = 0f; // NEW: Stores our starting checkpoint distance
    private float bestDistance = 0f;
    private int frameCount = 0;
    // private bool isInitialized = false; // We can remove this

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            // REMOVED: startZ = player.position.z;
            // This is now handled by SetInitialDistance or ResetDistance
        }
        else
        {
            Debug.LogWarning("DistanceCounter: Player not found! Distance will not be tracked.");
        }

        // Load best distance
        if (saveBestDistance)
        {
            bestDistance = PlayerPrefs.GetFloat(bestDistanceKey, 0f);
        }

        // Check if UI components are assigned
        if (distanceTextTMP == null && distanceText == null)
        {
            Debug.LogWarning("DistanceCounter: No UI text component assigned! Distance will be logged to console only.");
        }
        
        // Ensure distance is 0 on a fresh start
        // UpdateDistanceDisplay(); // SetInitialDistance will handle this
    }

    /// <summary>
    // NEW: Called by EndlessGameManager to set the starting distance from a checkpoint.
    /// </summary>
    public void SetInitialDistance(float initialDistance)
    {
        // Ensure player is found
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
            else
            {
                Debug.LogError("DistanceCounter: SetInitialDistance called, but Player not found!");
                return;
            }
        }

        // The player's transform.position was just set by EndlessGameManager
        // This is our new "zero" point for calculating travel *from* this spot.
        startZ = player.position.z; 

        // This is the score (distance) we are starting *with*.
        currentDistance = initialDistance;
        distanceOffset = initialDistance;

        // Update the UI immediately to show the starting distance (e.g., "500m" or "0m")
        UpdateDistanceDisplay();
    }


    void Update()
    {
        // Stop updating if player is lost or game isn't running
        if (player == null || EndlessGameManager.Instance == null || !EndlessGameManager.Instance.isGameRunning) return;

        frameCount++;
        if (frameCount % updateInterval != 0) return;

        // CHANGED: Calculate distance traveled *since the last spawn point*
        float distanceTraveledThisRun = player.position.z - startZ;
        
        // CHANGED: Add it to our starting offset (e.g., 500m + distance traveled)
        float newDistance = distanceOffset + distanceTraveledThisRun;

        if (newDistance > currentDistance)
        {
            currentDistance = newDistance;

            // Update best distance
            if (currentDistance > bestDistance)
            {
                bestDistance = currentDistance;
                if (saveBestDistance)
                {
                    PlayerPrefs.SetFloat(bestDistanceKey, bestDistance);
                    PlayerPrefs.Save();
                }
            }

            // Update UI
            UpdateDistanceDisplay();
        }
    }

    void UpdateDistanceDisplay()
    {
        string distanceString = string.Format(distanceFormat, currentDistance, distanceUnit);

        if (distanceTextTMP != null)
        {
            distanceTextTMP.text = distanceString;
        }
        else if (distanceText != null)
        {
            distanceText.text = distanceString;
        }
    }

    public float GetCurrentDistance()
    {
        return currentDistance;
    }

    public float GetBestDistance()
    {
        return bestDistance;
    }

    /// <summary>
    /// This now fully resets the counter to zero.
    /// </summary>
    public void ResetDistance()
    {
        currentDistance = 0f;
        distanceOffset = 0f; // CHANGED: Must reset the offset too
        if (player != null)
        {
            startZ = player.position.z;
        }
        UpdateDistanceDisplay();
    }

    public string GetFormattedDistance()
    {
        return string.Format(distanceFormat, currentDistance, distanceUnit);
    }

    public string GetFormattedBestDistance()
    {
        return string.Format(distanceFormat, bestDistance, distanceUnit);
    }
}