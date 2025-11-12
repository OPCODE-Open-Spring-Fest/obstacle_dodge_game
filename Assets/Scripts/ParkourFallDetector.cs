using UnityEngine;

public class ParkourFallDetector : MonoBehaviour
{
    [SerializeField] private float fallThreshold = -10f;
    [SerializeField] private GameObject player;
    [SerializeField] private bool checkEveryFrame = true;
    [SerializeField] private float checkInterval = 0.1f;
    
    [SerializeField] private string gameOverSceneName = "GameOver";

    private float lastCheckTime;
    private bool gameOverTriggered = false;
    
    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj;
            }
            else
            {
                Debug.LogError("[ParkourFallDetector] Player not found! Make sure your player has the 'Player' tag assigned.");
            }
        }
        
        lastCheckTime = Time.time;
    }
    
    private void Update()
    {
        if (player == null || gameOverTriggered)
        {
            return;
        }
        
        if (checkEveryFrame || Time.time - lastCheckTime >= checkInterval)
        {
            CheckPlayerFall();
            lastCheckTime = Time.time;
        }
    }
    
    private void CheckPlayerFall()
    {
        if (player.transform.position.y < fallThreshold)
        {
            TriggerGameOver();
        }
    }
    
    private void TriggerGameOver()
    {
        if (gameOverTriggered)
        {
            return;
        }
        
        gameOverTriggered = true;
        Debug.Log($"[ParkourFallDetector] Player fell below threshold ({fallThreshold}). Game Over!");
        
        PlayerDeathAnimator deathAnimator = player.GetComponent<PlayerDeathAnimator>();
        if (deathAnimator != null)
        {
            deathAnimator.PlayDeathAnimation();
        }
        else
        {
            LastLevelRecorder.SaveAndLoad(gameOverSceneName);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 start = new Vector3(-100f, fallThreshold, 0f);
        Vector3 end = new Vector3(100f, fallThreshold, 0f);
        Gizmos.DrawLine(start, end);
        
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(new Vector3(0f, fallThreshold, 0f), $"Fall Threshold: {fallThreshold}");
        #endif
    }
}