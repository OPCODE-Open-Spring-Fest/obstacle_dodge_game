using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndlessGameManager : MonoBehaviour
{
    [Header("Game State")]
    [Tooltip("Is the game currently running?")]
    public bool isGameRunning = false;
    
    [Tooltip("Is the game paused?")]
    public bool isGamePaused = false;

    [Header("Start Settings")]
    [Tooltip("Pause the game on scene load until StartGame is called manually (e.g. by a countdown)")]
    public bool pauseOnStart = true;

    [Tooltip("Reset the distance counter when a new run begins")]
    public bool resetDistanceOnStart = true;

    [Tooltip("Clear spawned obstacles when a new run begins")]
    public bool clearObstaclesOnStart = true;

    [Header("Game References")]
    [Tooltip("Reference to the player GameObject")]
    public GameObject player;
    
    [Tooltip("Reference to GroundSpawner")]
    public GroundSpawner groundSpawner;
    
    [Tooltip("Reference to ObstacleSpawner")]
    public ObstacleSpawner obstacleSpawner;
    
    [Tooltip("Reference to DistanceCounter")]
    public DistanceCounter distanceCounter;
    
    [Tooltip("Reference to PlayerObstacleCollision")]
    public PlayerObstacleCollision playerCollision;

    [Header("Game Over Settings")]
    [Tooltip("Scene name to load on game over")]
    public string gameOverSceneName = "GameOver";
    
    [Tooltip("Delay before loading game over scene")]
    public float gameOverDelay = 2f;

    private static EndlessGameManager instance;
    public static EndlessGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EndlessGameManager>();
            }
            return instance;
        }
    }

    private Coroutine gameOverRoutine;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj;
            }
        }

        // Find components if not assigned
        if (groundSpawner == null)
        {
            groundSpawner = FindObjectOfType<GroundSpawner>();
        }

        if (obstacleSpawner == null)
        {
            obstacleSpawner = FindObjectOfType<ObstacleSpawner>();
        }

        if (distanceCounter == null)
        {
            distanceCounter = FindObjectOfType<DistanceCounter>();
        }

        if (player != null && playerCollision == null)
        {
            playerCollision = player.GetComponent<PlayerObstacleCollision>();
        }

        PrepareForNewRun(clearExistingObstacles: false);

        if (!pauseOnStart)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        isGameRunning = true;
        isGamePaused = false;
        Time.timeScale = 1f;

        if (playerCollision != null)
        {
            playerCollision.ResetDeath();
        }

        // Reset distance counter
        if (resetDistanceOnStart && distanceCounter != null)
        {
            distanceCounter.ResetDistance();
        }

        // Clear obstacles if any
        if (clearObstaclesOnStart && obstacleSpawner != null)
        {
            obstacleSpawner.ClearAllObstacles();
        }

        Debug.Log("Game Started!");
    }

    public void PauseGame()
    {
        if (!isGameRunning) return;

        isGamePaused = true;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        if (!isGameRunning) return;

        isGamePaused = false;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }

    public void GameOver()
    {
        if (!isGameRunning) return;

        isGameRunning = false;
        isGamePaused = false;

        if (gameOverRoutine != null)
        {
            StopCoroutine(gameOverRoutine);
        }

        gameOverRoutine = StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        Time.timeScale = 0f;

        // Get final distance
        float finalDistance = 0f;
        if (distanceCounter != null)
        {
            finalDistance = distanceCounter.GetCurrentDistance();
        }

        Debug.Log($"Game Over! Final Distance: {finalDistance}");

        if (gameOverDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(gameOverDelay);
        }

        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(gameOverSceneName))
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
        else
        {
            Debug.LogWarning("GameOver scene name not set!");
        }
        gameOverRoutine = null;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Assuming main menu is at index 0
    }

    public void PrepareForNewRun(bool clearExistingObstacles = true)
    {
        isGameRunning = false;
        isGamePaused = true;
        Time.timeScale = 0f;

        if (playerCollision != null)
        {
            playerCollision.ResetDeath();
        }

        if (clearExistingObstacles && clearObstaclesOnStart && obstacleSpawner != null)
        {
            obstacleSpawner.ClearAllObstacles();
        }
    }

    void Update()
    {
        // Check if player is dead
        if (isGameRunning && playerCollision != null && playerCollision.IsDead())
        {
            GameOver();
        }

        // Pause/Resume with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else if (isGameRunning)
            {
                PauseGame();
            }
        }
    }
}

