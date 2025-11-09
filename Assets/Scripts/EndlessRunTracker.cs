using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndlessRunTracker : MonoBehaviour
{
    [Header("Speed Increase Settings")]
    [SerializeField] private bool enableSpeedIncrease = true;
    [SerializeField] private float speedIncreaseRate = 0.01f;
    [SerializeField] private float speedIncreaseInterval = 1f;
    [SerializeField] private float maxSpeedMultiplier = 2f;

    [Header("Distance Tracking")]
    [SerializeField] private bool trackDistance = true;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float distanceTraveled = 0f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI distanceTextTMP;
    [SerializeField] private Text distanceTextLegacy;
    [SerializeField] private string distanceFormat = "Distance: {0:F1}m";

    [Header("Player Reference")]
    [SerializeField] private GameObject player;
    [SerializeField] private Mover mover;
    [SerializeField] private EndlessRunnerMover endlessMover;

    private float gameStartTime;
    private float lastSpeedIncreaseTime;
    private float baseSpeed;
    private float currentSpeedMultiplier = 1f;

    public static EndlessRunTracker Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeTracker();
    }

    private void InitializeTracker()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj;
            }
        }

        if (endlessMover == null && player != null)
        {
            endlessMover = player.GetComponent<EndlessRunnerMover>();
        }
        
        if (mover == null && player != null && endlessMover == null)
        {
            mover = player.GetComponent<Mover>();
        }

        if (endlessMover != null)
        {
            float baseSpeedValue = endlessMover.GetBaseSpeed();
            if (baseSpeedValue > 0.1f)
            {
                baseSpeed = baseSpeedValue;
            }
            else
            {
                baseSpeed = endlessMover.GetSpeed();
            }
        }
        else if (mover != null)
        {
            float baseSpeedValue = mover.GetBaseSpeed();
            if (baseSpeedValue > 0.1f)
            {
                baseSpeed = baseSpeedValue;
            }
            else
            {
                baseSpeed = mover.GetSpeed();
            }
        }

        if (player != null)
        {
            startPosition = player.transform.position;
        }

        gameStartTime = Time.time;
        lastSpeedIncreaseTime = Time.time;
        distanceTraveled = 0f;
        currentSpeedMultiplier = 1f;

        UpdateUI();
    }

    private void Update()
    {
        if (player == null) return;

        if (trackDistance)
        {
            UpdateDistance();
        }

        if (enableSpeedIncrease)
        {
            if (endlessMover != null)
            {
                IncreaseSpeedOverTime();
            }
            else if (mover != null)
            {
                IncreaseSpeedOverTime();
            }
        }

        UpdateUI();
    }

    private void UpdateDistance()
    {
        if (player != null)
        {
            float currentDistance = Vector3.Distance(startPosition, player.transform.position);
            distanceTraveled = Mathf.Max(distanceTraveled, currentDistance);
        }
    }

    private void IncreaseSpeedOverTime()
    {
        if (Time.time - lastSpeedIncreaseTime >= speedIncreaseInterval)
        {
            if (currentSpeedMultiplier < maxSpeedMultiplier)
            {
                currentSpeedMultiplier += speedIncreaseRate;
                currentSpeedMultiplier = Mathf.Min(currentSpeedMultiplier, maxSpeedMultiplier);
                
                if (endlessMover != null)
                {
                    endlessMover.SetSpeed(baseSpeed * currentSpeedMultiplier);
                }
                else if (mover != null)
                {
                    mover.SetSpeed(baseSpeed * currentSpeedMultiplier);
                }

                lastSpeedIncreaseTime = Time.time;
            }
        }
    }

    private void UpdateUI()
    {
        string distanceStr = string.Format(distanceFormat, distanceTraveled);

        if (distanceTextTMP != null)
        {
            distanceTextTMP.text = distanceStr;
        }

        if (distanceTextLegacy != null)
        {
            distanceTextLegacy.text = distanceStr;
        }
    }

    public float GetDistanceTraveled()
    {
        return distanceTraveled;
    }

    public float GetCurrentSpeedMultiplier()
    {
        return currentSpeedMultiplier;
    }

    public void ResetTracker()
    {
        InitializeTracker();
    }
}

