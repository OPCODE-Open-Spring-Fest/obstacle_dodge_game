using UnityEngine;
using TMPro; 

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimerUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The LevelTimer script in your scene.")]
    [SerializeField] private LevelTimer levelTimer; // <-- This line is UPDATED
    
    private TextMeshProUGUI timerText;

    void Awake()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        
        if (levelTimer == null)
        {
            levelTimer = FindObjectOfType<LevelTimer>(); // <-- This line is UPDATED
        }

        if (levelTimer == null)
        {
            Debug.LogError("TimerUI cannot find the LevelTimer script in the scene!");
            enabled = false;
        }
    }

    void Update()
    {
        float timeRemaining = levelTimer.TimeRemaining; // <-- This line is UPDATED

        if (timeRemaining < 0)
        {
            timeRemaining = 0;
        }

        int minutes = (int)(timeRemaining / 60);
        int seconds = (int)(timeRemaining % 60);
        int milliseconds = (int)((timeRemaining * 100) % 100);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}