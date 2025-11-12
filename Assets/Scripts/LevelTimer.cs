using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public float timeLimit = 60f; 
    public string gameOverSceneName = "GameOver"; 

    private float elapsedTime = 0f;
    private bool isTimerRunning = false;
    private bool timeUpTriggered = false;

    public float TimeRemaining
    {
        get { return timeLimit - elapsedTime; }
    }

    public float ElapsedTime
    {
        get { return elapsedTime; }
    }

    void Update()
    {
        if (!isTimerRunning || timeUpTriggered)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeLimit)
        {
            isTimerRunning = false;
            timeUpTriggered = true;
            Debug.Log("Time's Up! You Lose.");

            DisablePlayer();
            
            LastLevelRecorder.SaveAndLoad(gameOverSceneName);
        }
    }

    public void SetTimerActive(bool isActive)
    {
        isTimerRunning = isActive;
    }

    public float StopTimer()
    {
        isTimerRunning = false;
        Debug.Log($"LevelTimer stopped. Final time: {elapsedTime}");
        return elapsedTime;
    }

    private void DisablePlayer()
    {
        ParkourPlayerController player = FindObjectOfType<ParkourPlayerController>();
        if (player != null)
        {
            player.enabled = false;
        }
    }
}