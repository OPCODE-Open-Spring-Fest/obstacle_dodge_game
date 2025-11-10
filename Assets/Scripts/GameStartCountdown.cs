using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStartCountdown : MonoBehaviour
{
    [Header("Countdown Settings")]
    [Tooltip("Values to show during the countdown in order")]
    public string[] countdownSteps = { "3", "2", "1", "GO!" };

    [Tooltip("Time (in seconds) to wait before the first countdown value is shown")]
    public float initialDelay = 0.5f;

    [Tooltip("Duration each countdown value remains on screen (in seconds)")]
    public float stepDuration = 1f;

    [Tooltip("Automatically start the countdown on Start()")]
    public bool startAutomatically = true;

    [Tooltip("Clear any spawned obstacles before beginning the countdown")]
    public bool clearObstaclesBeforeCountdown = false;

    [Header("UI References")]
    [Tooltip("Optional TextMeshPro component used to display the countdown")]
    public TextMeshProUGUI countdownTextTMP;

    [Tooltip("Optional legacy Text component used to display the countdown")]
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
        EndlessGameManager manager = EndlessGameManager.Instance;
        if (manager != null)
        {
            manager.PrepareForNewRun(clearExistingObstacles: clearObstaclesBeforeCountdown);
        }
        else
        {
            Time.timeScale = 0f;
        }

        SetCountdownVisibility(true);

        if (initialDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(initialDelay);
        }

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
                yield return new WaitForSecondsRealtime(Mathf.Max(0f, stepDuration));
            }
        }

        if (hideTextOnFinish)
        {
            SetCountdownVisibility(false);
        }

        manager?.StartGame();
        countdownRoutine = null;
    }

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
