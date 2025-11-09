using UnityEngine;

public class pauseButton : MonoBehaviour
{
    private bool isPaused = false;

    public void TogglePause()
    {
        if (isPaused)
        {
            Time.timeScale = 1f; 
            isPaused = false;
        }
        else
        {
            Time.timeScale = 0f; 
            isPaused = true;
        }
    }
}
