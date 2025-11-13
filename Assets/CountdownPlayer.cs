using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StandaloneCountdown : MonoBehaviour
{
    [Header("Countdown Settings")]
    [Tooltip("Values to show during the countdown in order")]
    public string[] countdownSteps = { "3", "2", "1", "GO!" };

    [Tooltip("Time (in seconds) to wait before the first value is shown")]
    public float initialDelay = 0.5f;

    [Tooltip("Duration each countdown value remains on screen (in seconds)")]
    public float stepDuration = 1f;

    [Tooltip("Automatically start the countdown on Start()")]
    public bool startAutomatically = true;

    [Header("UI References")]
    [Tooltip("TextMeshPro component to display the countdown")]
    public TextMeshProUGUI countdownTextTMP;

    [Tooltip("Legacy Text component to display the countdown")]
    public Text countdownText;

    [Tooltip("Hide the countdown text after the countdown completes")]
    public bool hideTextOnFinish = true;

    private Coroutine countdownRoutine;

    void Start()
    {
        if (startAutomatically)
        {
            StartCountdown();
        }
    }

    public void StartCountdown()
    {
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
        }

        countdownRoutine = StartCoroutine(CountdownSequence());
    }

    private IEnumerator CountdownSequence()
    {
        // 1. Pause the game
        Time.timeScale = 0f;

        SetCountdownVisibility(true);

        // Wait for the initial delay
        if (initialDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(initialDelay);
        }

        // --- Run the countdown steps ---
        if (countdownSteps == null || countdownSteps.Length == 0)
        {
            SetCountdownText("GO!");
            yield return new WaitForSecondsRealtime(stepDuration);
        }
        else
        {
            foreach (string step in countdownSteps)
            {
                SetCountdownText(step);
                // Wait for the step duration (using Realtime)
                yield return new WaitForSecondsRealtime(Mathf.Max(0f, stepDuration));
            }
        }

        // --- Countdown Finished ---

        if (hideTextOnFinish)
        {
            SetCountdownVisibility(false);
        }

        // 2. Unpause the game
        Time.timeScale = 1f;

        countdownRoutine = null;
    }

    // --- Helper methods to show/hide the text ---

    private void SetCountdownText(string text)
    {
        if (countdownTextTMP != null)
        {
            countdownTextTMP.text = text;
        }

        if (countdownText != null)
        {
            countdownText.text = text;
        }
    }

    private void SetCountdownVisibility(bool visible)
    {
        if (countdownTextTMP != null)
        {
            countdownTextTMP.gameObject.SetActive(visible);
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(visible);
        }
    }
}