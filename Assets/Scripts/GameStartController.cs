using UnityEngine;
using TMPro;
using System.Collections;

public class GameStartController : MonoBehaviour
{
    [Header("UI & Timing")]
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 3f;

    [Header("Game Component References")]
    public GameObject playerObject;
    public LevelTimer levelTimer;  

    private ParkourPlayerController parkourController; 

    void Start()
    {
        if (playerObject != null)
        {
            parkourController = playerObject.GetComponent<ParkourPlayerController>(); 
        }
        
        if (levelTimer == null)
        {
            levelTimer = FindObjectOfType<LevelTimer>(); // <-- This line is UPDATED
        }

        if (parkourController != null && countdownText != null) 
        {
            StartCoroutine(CountdownSequence());
        }
        else
        {
            Debug.LogError("Countdown setup is missing components! (Player or Text). Starting game immediately.");
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
        if (parkourController != null)
        {
            parkourController.enabled = isActive;
        }
        
        if (levelTimer != null)
        {
            levelTimer.SetTimerActive(isActive);
        }

        EnemyFollower[] allEnemies = FindObjectsOfType<EnemyFollower>();
        foreach (EnemyFollower enemy in allEnemies)
        {
            enemy.enabled = isActive;
        }
    }
}