using UnityEngine;

public class GameplayBalancer : MonoBehaviour
{
    [Header("Difficulty Progression")]
    [SerializeField] private float baseSpawnRate = 1.0f;
    [SerializeField] private float maxSpawnRate = 3.0f;
    [SerializeField] private float difficultyIncreaseRate = 0.1f;
    
    [Header("Speed Settings")]
    [SerializeField] private float baseObstacleSpeed = 5.0f;
    [SerializeField] private float maxObstacleSpeed = 15.0f;
    [SerializeField] private float speedIncreaseRate = 0.5f;
    
    [Header("Timing Settings")]
    [SerializeField] private float gameStartTime = 0f;
    [SerializeField] private float difficultyCheckInterval = 5.0f;
    
    private float currentSpawnRate;
    private float currentObstacleSpeed;
    private float lastDifficultyCheck;
    
    public static GameplayBalancer Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeGameplay();
    }
    
    void InitializeGameplay()
    {
        gameStartTime = Time.time;
        currentSpawnRate = baseSpawnRate;
        currentObstacleSpeed = baseObstacleSpeed;
        lastDifficultyCheck = Time.time;
        
        Debug.Log("Gameplay balancer initialized");
    }
    
    void Update()
    {
        UpdateDifficulty();
    }
    
    void UpdateDifficulty()
    {
        if (Time.time - lastDifficultyCheck >= difficultyCheckInterval)
        {
            float gameTime = Time.time - gameStartTime;
            
            // Gradually increase spawn rate
            currentSpawnRate = Mathf.Min(
                baseSpawnRate + (gameTime * difficultyIncreaseRate),
                maxSpawnRate
            );
            
            // Gradually increase obstacle speed
            currentObstacleSpeed = Mathf.Min(
                baseObstacleSpeed + (gameTime * speedIncreaseRate),
                maxObstacleSpeed
            );
            
            lastDifficultyCheck = Time.time;
            
            Debug.Log($"Difficulty updated - Spawn Rate: {currentSpawnRate:F2}, Speed: {currentObstacleSpeed:F2}");
        }
    }
    
    public float GetCurrentSpawnRate()
    {
        return currentSpawnRate;
    }
    
    public float GetCurrentObstacleSpeed()
    {
        return currentObstacleSpeed;
    }
    
    public float GetGameTime()
    {
        return Time.time - gameStartTime;
    }
    
    public void ResetDifficulty()
    {
        gameStartTime = Time.time;
        currentSpawnRate = baseSpawnRate;
        currentObstacleSpeed = baseObstacleSpeed;
        lastDifficultyCheck = Time.time;
    }
}
