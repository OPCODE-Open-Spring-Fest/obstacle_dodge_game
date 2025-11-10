using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameCountdown : MonoBehaviour
{
    [Header("Countdown Settings")]
    [Tooltip("Starting countdown number (e.g., 3 for 3, 2, 1, GO!)")]
    public int countdownStart = 3;
    
    [Tooltip("Time between each countdown number (in seconds)")]
    public float countdownInterval = 1f;
    
    [Tooltip("Time to show 'GO!' message (in seconds)")]
    public float goMessageDuration = 0.5f;
    
    [Header("UI Settings")]
    [Tooltip("TextMeshPro component to display countdown")]
    public TextMeshProUGUI countdownTextTMP;
    
    [Tooltip("Unity Text component to display countdown (alternative to TMP)")]
    public Text countdownText;
    
    [Tooltip("Format string for countdown (e.g., '{0}' or 'Get Ready: {0}')")]
    public string countdownFormat = "{0}";
    
    [Tooltip("Text to show when countdown finishes (e.g., 'GO!')")]
    public string goText = "GO!";
    
    [Header("Game Manager Reference")]
    [Tooltip("Reference to EndlessGameManager (will find automatically if not assigned)")]
    public EndlessGameManager gameManager;
    
    private bool countdownActive = false;
    private bool countdownComplete = false;

    void Start()
    {
        // Find game manager if not assigned
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<EndlessGameManager>();
        }

        // Check if UI components are assigned
        if (countdownTextTMP == null && countdownText == null)
        {
            Debug.LogWarning("GameCountdown: No UI text component assigned! Countdown will not be visible.");
        }

        // Start countdown
        StartCountdown();
    }

    public void StartCountdown()
    {
        if (countdownActive) return;

        countdownActive = true;
        countdownComplete = false;

        // Pause the game
        if (gameManager != null)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 0f;
        }

        // Show countdown UI
        if (countdownTextTMP != null)
        {
            countdownTextTMP.gameObject.SetActive(true);
        }
        else if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        // Start countdown coroutine
        StartCoroutine(CountdownSequence());
    }

    IEnumerator CountdownSequence()
    {
        // Wait for real time (not scaled time)
        for (int i = countdownStart; i > 0; i--)
        {
            UpdateCountdownText(i.ToString());
            yield return new WaitForSecondsRealtime(countdownInterval);
        }

        // Show "GO!" message
        UpdateCountdownText(goText);
        yield return new WaitForSecondsRealtime(goMessageDuration);

        // Hide countdown UI
        if (countdownTextTMP != null)
        {
            countdownTextTMP.gameObject.SetActive(false);
        }
        else if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // Resume game
        countdownComplete = true;
        Time.timeScale = 1f;

        // Start the game
        if (gameManager != null)
        {
            gameManager.StartGame();
        }

        countdownActive = false;
        Debug.Log("Countdown complete! Game started!");
    }

    void UpdateCountdownText(string text)
    {
        string displayText = string.Format(countdownFormat, text);

        if (countdownTextTMP != null)
        {
            countdownTextTMP.text = displayText;
        }
        else if (countdownText != null)
        {
            countdownText.text = displayText;
        }
    }

    public bool IsCountdownComplete()
    {
        return countdownComplete;
    }

    public bool IsCountdownActive()
    {
        return countdownActive;
    }
}

