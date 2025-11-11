using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class EndlessGameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool isGameRunning = false;
    public bool isGamePaused = false;

    [Header("Start Settings")]
    public bool pauseOnStart = true;
    public bool clearObstaclesOnStart = true;

    [Header("Game References")]
    public GameObject player;
    public PlayerEndlessMovement playerMovement;
    public GroundSpawner groundSpawner;
    public ObstacleSpawner obstacleSpawner;
    public DistanceCounter distanceCounter;
    public PlayerObstacleCollision playerCollision;
    
    [Header("Game Over Settings")]
    public string gameOverSceneName = "GameOver";
    public float gameOverDelay = 1f; 

    [Header("UI")]
    [Tooltip("Drag your heart UI controller object here.")]
    public LivesHeartUI livesHeartUI; 

    [Header("Respawn Settings")]
    [Tooltip("How long (in seconds) is the player invincible after respawning?")]
    public float respawnInvincibility = 1.5f;

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
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (SessionManager.Instance == null)
        {
            Debug.LogError("SessionManager.Instance not found! Did you add the SessionManager to your Main Menu scene?");
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && playerMovement == null)
        {
            playerMovement = player.GetComponent<PlayerEndlessMovement>();
        }

        if (groundSpawner == null) groundSpawner = FindObjectOfType<GroundSpawner>();
        if (obstacleSpawner == null) obstacleSpawner = FindObjectOfType<ObstacleSpawner>();
        if (distanceCounter == null) distanceCounter = FindObjectOfType<DistanceCounter>();
        if (player != null && playerCollision == null)
        {
            playerCollision = player.GetComponent<PlayerObstacleCollision>();
        }

        if (livesHeartUI == null)
        {
            livesHeartUI = FindObjectOfType<LivesHeartUI>();
        }

        float spawnDistance = 0f;
        if (SessionManager.Instance != null)
        {
            spawnDistance = SessionManager.Instance.GetSpawnDistance();
            UpdateLivesUI(); 
        }

        if (playerMovement != null)
        {
            playerMovement.SetInitialPosition(spawnDistance);
        }
        
        if (distanceCounter != null)
        {
            distanceCounter.SetInitialDistance(spawnDistance); 
        }

        if (pauseOnStart)
        {
            PrepareForNewRun(false); 
        }
        else
        {
            StartGame(); 
        }
    }
    void UpdateLivesUI()
    {
        if (livesHeartUI != null && SessionManager.Instance != null)
        {
            livesHeartUI.UpdateHearts(SessionManager.Instance.CurrentLives);
        }
    }

    public void StartGame()
    {
        isGameRunning = true;
        isGamePaused = false;
        Time.timeScale = 1f;
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
        if (!isGamePaused) return;
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

    public void PrepareForNewRun(bool clearExistingObstacles = true)
    {
        isGameRunning = false;
        isGamePaused = true;
        Time.timeScale = 0f;

        if (clearExistingObstacles && clearObstaclesOnStart && obstacleSpawner != null)
        {
            obstacleSpawner.ClearAllObstacles();
        }
    }

    public void PlayerHitObstacle()
    {
        if (!isGameRunning) return; 

        if (SessionManager.Instance == null)
        {
            Debug.LogError("SessionManager not found!");
            return;
        }

        SessionManager.Instance.SpendLife();
        UpdateLivesUI();

        if (SessionManager.Instance.CurrentLives <= 0)
        {
            if (playerMovement != null) playerMovement.enabled = false;
            GameOver();
        }
        else
        {
            StartCoroutine(RespawnPlayer());
        }
    }

    private IEnumerator RespawnPlayer()
    {
        isGameRunning = false;
        if (playerMovement != null) playerMovement.enabled = false;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true; 
        }
        
        yield return new WaitForSeconds(0.1f);

        float respawnDistance = SessionManager.Instance.GetSpawnDistance();

        if (groundSpawner != null)
        {
            groundSpawner.ResetForRespawn(respawnDistance);
        }
        if (obstacleSpawner != null)
        {
            obstacleSpawner.ResetForRespawn(respawnDistance);
        }
        
        if (playerMovement != null)
        {
            playerMovement.SetInitialPosition(respawnDistance);
        }
        if (distanceCounter != null)
        {
            distanceCounter.SetInitialDistance(respawnDistance);
        }

        if (rb != null) rb.isKinematic = false;
        if (playerMovement != null) playerMovement.enabled = true;

        if(playerCollision != null)
        {
            playerCollision.ResetCooldown();
        }
        
        Debug.Log($"Respawning... Invincible for {respawnInvincibility} seconds.");
        yield return new WaitForSeconds(respawnInvincibility);

        Debug.Log("Respawn complete. Game running.");
        isGameRunning = true;
    }


    void Update()
    {
        if (isGameRunning && SessionManager.Instance != null && distanceCounter != null)
        {
            SessionManager.Instance.UpdateCheckpoint(distanceCounter.GetCurrentDistance());
        }

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