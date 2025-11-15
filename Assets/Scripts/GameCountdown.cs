using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Playables;

public class GameCountdown : MonoBehaviour
{
    public int countdownStart = 3;
    public float countdownInterval = 1f;
    public float goMessageDuration = 0.5f;

    public TextMeshProUGUI countdownTextTMP;
    public Text countdownText;

    public string countdownFormat = "{0}";
    public string goText = "GO!";

    public EndlessGameManager gameManager;
    public PlayableDirector introTimeline;
    public MonoBehaviour playerController;

    private bool countdownActive = false;
    private bool countdownComplete = false;

    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<EndlessGameManager>();

        if (playerController != null)
            playerController.enabled = false;

        StartCountdown();
    }

    public void StartCountdown()
    {
        if (countdownActive) return;

        countdownActive = true;
        countdownComplete = false;

        Time.timeScale = 0f; 

        if (introTimeline != null)
            introTimeline.Play();

        if (countdownTextTMP != null)
            countdownTextTMP.gameObject.SetActive(true);
        else if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        StartCoroutine(CountdownSequence());
    }

    IEnumerator CountdownSequence()
    {
        for (int i = countdownStart; i > 0; i--)
        {
            UpdateCountdownText(i.ToString());
            yield return new WaitForSecondsRealtime(countdownInterval);
        }

        UpdateCountdownText(goText);
        yield return new WaitForSecondsRealtime(goMessageDuration);
        
        if (countdownTextTMP != null)
            countdownTextTMP.gameObject.SetActive(false);
        else if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        countdownComplete = true;
        Time.timeScale = 1f;

        if (playerController != null)
            playerController.enabled = true;

        if (gameManager != null)
            gameManager.StartGame();

        countdownActive = false;
    }

    void UpdateCountdownText(string text)
    {
        string displayText = string.Format(countdownFormat, text);

        if (countdownTextTMP != null)
            countdownTextTMP.text = displayText;
        else if (countdownText != null)
            countdownText.text = displayText;
    }

    public bool IsCountdownComplete() => countdownComplete;
    public bool IsCountdownActive() => countdownActive;
}