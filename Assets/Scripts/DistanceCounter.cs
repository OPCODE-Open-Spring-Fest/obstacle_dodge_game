using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DistanceCounter : MonoBehaviour
{
    [Header("Distance Settings")]
    [Tooltip("Starting position Z coordinate (usually 0)")]
    public float startZ = 0f;
    
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
    private float bestDistance = 0f;
    private int frameCount = 0;
    private bool isInitialized = false;

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            startZ = player.position.z;
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

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized || player == null) return;

        frameCount++;
        if (frameCount % updateInterval != 0) return;

        // Calculate distance traveled
        float newDistance = player.position.z - startZ;
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

    public void ResetDistance()
    {
        currentDistance = 0f;
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

