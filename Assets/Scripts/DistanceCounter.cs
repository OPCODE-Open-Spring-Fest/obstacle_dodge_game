using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DistanceCounter : MonoBehaviour
{
    [Header("Distance Settings")]
    public float startZ;
    public string distanceUnit = "m";
    public int updateInterval = 1;
    
    [Header("UI Settings")]
    public TextMeshProUGUI distanceTextTMP;
    public Text distanceText;
    public string distanceFormat = "Distance: {0:F1} {1}";
    
    [Header("High Score Settings")]
    public bool saveBestDistance = true;
    public string bestDistanceKey = "BestDistance";
    
    private Transform player;
    private float currentDistance = 0f;
    private float distanceOffset = 0f;
    private float bestDistance = 0f;
    private int frameCount = 0;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("DistanceCounter: Player not found! Distance will not be tracked.");
        }

        if (saveBestDistance)
        {
            bestDistance = PlayerPrefs.GetFloat(bestDistanceKey, 0f);
        }

        if (distanceTextTMP == null && distanceText == null)
        {
            Debug.LogWarning("DistanceCounter: No UI text component assigned! Distance will be logged to console only.");
        }
    }

    public void SetInitialDistance(float initialDistance)
    {
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

        startZ = player.position.z; 
        currentDistance = initialDistance;
        distanceOffset = initialDistance;

        UpdateDistanceDisplay();
    }


    void Update()
    {
        if (player == null || EndlessGameManager.Instance == null || !EndlessGameManager.Instance.isGameRunning) return;

        frameCount++;
        if (frameCount % updateInterval != 0) return;

        float distanceTraveledThisRun = player.position.z - startZ;
        
        float newDistance = distanceOffset + distanceTraveledThisRun;

        if (newDistance > currentDistance)
        {
            currentDistance = newDistance;

            if (currentDistance > bestDistance)
            {
                bestDistance = currentDistance;
                if (saveBestDistance)
                {
                    PlayerPrefs.SetFloat(bestDistanceKey, bestDistance);
                    PlayerPrefs.Save();
                }
            }

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
        distanceOffset = 0f;
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