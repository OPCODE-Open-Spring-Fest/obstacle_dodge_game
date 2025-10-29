using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InvincibilityUI : MonoBehaviour
{
    [Tooltip("The player GameObject that has PlayerCollectibles component")]
    public GameObject player;

    [Tooltip("Root UI object to enable/disable while invincible")]
    public GameObject uiRoot;

    [Tooltip("Unity UI Text used to show remaining seconds (or leave empty and use TextMeshPro below)")]
    public Text countdownText;

    [Tooltip("(Optional) TextMeshProUGUI text to show remaining seconds. Use this if your UI uses TextMeshPro.")]
    public TextMeshProUGUI countdownTextTMP;

    [Tooltip("(Optional) Fill image (Image.Type = Filled) to show a radial/smooth countdown.")]
    public Image fillImage;

    private PlayerCollectibles playerCollectibles;

    private Coroutine running;
    private float currentDuration;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("InvincibilityUI: Player is not assigned.");
            enabled = false;
            return;
        }

        playerCollectibles = player.GetComponent<PlayerCollectibles>();
        if (playerCollectibles == null)
        {
            Debug.LogError("InvincibilityUI: Player does not have PlayerCollectibles component.");
            enabled = false;
            return;
        }

        uiRoot.SetActive(false);
        playerCollectibles.OnInvincibilityStarted += OnStarted;
        playerCollectibles.OnInvincibilityEnded += OnEnded;
    }

    private void OnDestroy()
    {
        if (playerCollectibles != null)
        {
            playerCollectibles.OnInvincibilityStarted -= OnStarted;
            playerCollectibles.OnInvincibilityEnded -= OnEnded;
        }
    }

    private void OnStarted(float duration)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(RunCountdown(duration));
    }

    private void OnEnded()
    {
        if (running != null) StopCoroutine(running);
        running = null;
        if (uiRoot != null) uiRoot.SetActive(false);
    }

    private IEnumerator RunCountdown(float duration)
    {
        if (uiRoot != null) uiRoot.SetActive(true);

        float t = duration;
        currentDuration = duration;
        while (t > 0f)
        {
            // update UI.Text
            if (countdownText != null)
                countdownText.text = Mathf.CeilToInt(t).ToString();

            // update TextMeshPro if assigned
            if (countdownTextTMP != null)
                countdownTextTMP.text = Mathf.CeilToInt(t).ToString();

            // update radial fill image smoothly
            if (fillImage != null && currentDuration > 0f)
                fillImage.fillAmount = Mathf.Clamp01(t / currentDuration);

            t -= Time.deltaTime;
            yield return null;
        }

        // clear/hide
        if (countdownText != null) countdownText.text = "";
        if (countdownTextTMP != null) countdownTextTMP.text = string.Empty;
        if (fillImage != null) fillImage.fillAmount = 0f;
        if (uiRoot != null) uiRoot.SetActive(false);
        running = null;
    }
}
