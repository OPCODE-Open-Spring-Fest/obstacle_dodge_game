using UnityEngine;
using TMPro;
using System.Collections;

public class GameStartController : MonoBehaviour
{
    // --- Public variables to assign in the Inspector ---
    [Header("UI & Timing")]
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 3f;

    [Header("Game Component References")]
    public GameObject playerObject;

    private Mover playerMover;

    void Start()
    {
        if (playerObject != null)
        {
            playerMover = playerObject.GetComponent<Mover>();
        }
        if (playerMover != null && countdownText != null)
        {
            StartCoroutine(CountdownSequence());
        }
        else
        {
            Debug.LogError("Countdown setup is missing components! Starting game immediately.");
            SetGameActive(true);
        }
    }
    IEnumerator CountdownSequence()
    {
        SetGameActive(false);
        countdownText.gameObject.SetActive(true);
        float timer = countdownDuration;

        while (timer > 0)
        {
            int displayTime = Mathf.CeilToInt(timer);
            countdownText.text = displayTime.ToString();
            yield return new WaitForSeconds(1f);

            timer -= 1f;
        }
        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        countdownText.gameObject.SetActive(false);
        SetGameActive(true);
    }

    void SetGameActive(bool isActive)
    {
        if (playerMover != null)
        {
            playerMover.enabled = isActive;
        }
        EnemyFollower[] allEnemies = FindObjectsOfType<EnemyFollower>();

        foreach (EnemyFollower enemy in allEnemies)
        {
            enemy.enabled = isActive;
        }
    }
}