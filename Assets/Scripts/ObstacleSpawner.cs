using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [Tooltip("List of obstacle prefabs to spawn")]
    public GameObject[] obstaclePrefabs;
    
    [Tooltip("Minimum distance between obstacle groups")]
    public float minDistanceBetweenObstacles = 10f;
    
    [Tooltip("Maximum distance between obstacle groups")]
    public float maxDistanceBetweenObstacles = 12f;
    
    [Tooltip("How far ahead of player to spawn obstacles")]
    public float spawnAheadDistance = 50f;
    
    [Tooltip("How far behind player to despawn obstacles")]
    public float despawnBehindDistance = 20f;
    
    [Header("Spawn Area Settings")]
    [Tooltip("Width of the spawn area (should match ground width)")]
    public float spawnWidth = 10f;
    
    [Tooltip("Number of lanes for obstacles (e.g., 3 for left, center, right)")]
    public int numberOfLanes = 3;
    
    [Tooltip("Spawn obstacles on the sides of the path")]
    public bool spawnOnSides = true;
    
    [Header("Spawn Pattern Settings")]
    [Tooltip("Chance to spawn an obstacle group (0-1)")]
    [Range(0f, 1f)]
    public float spawnChance = 0.75f;
    
    [Tooltip("Minimum obstacles per spawn group")]
    public int minObstaclesPerSection = 1;
    
    [Tooltip("Maximum obstacles per spawn group")]
    public int maxObstaclesPerSection = 2;

    [Header("Initial Spawn")]
    [Tooltip("How far from the player's start position should the *first* obstacle spawn?")]
    public float initialSpawnDistance = 10f; 
    
    private Transform player;
    private List<GameObject> spawnedObstacles = new List<GameObject>();
    private float nextSpawnZ = 0f;
    private float lastSpawnZ = -1000f;
    private bool isInitialized = false;
    private GroundSpawner groundSpawner;
    private EndlessGameManager gameManager;

    void Start()
    {
        gameManager = EndlessGameManager.Instance;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            ResetSpawnTimer(player.position.z); 
        }
        else
        {
            Debug.LogError("ObstacleSpawner: Player not found! Make sure player has 'Player' tag.");
            return;
        }

        groundSpawner = FindObjectOfType<GroundSpawner>();
        if (groundSpawner != null)
        {
        }

        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            Debug.LogWarning("ObstacleSpawner: No obstacle prefabs assigned!");
        }

        if (minDistanceBetweenObstacles <= 0 || maxDistanceBetweenObstacles <= 0)
        {
            Debug.LogError("ObstacleSpawner: minDistanceBetweenObstacles and maxDistanceBetweenObstacles must be positive values! Disabling script.");
            enabled = false;
            return;
        }

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized || player == null || (gameManager != null && !gameManager.isGameRunning)) return;

        while (nextSpawnZ < player.position.z + spawnAheadDistance)
        {
            SpawnObstacleSection();
        }

        for (int i = spawnedObstacles.Count - 1; i >= 0; i--)
        {
            if (spawnedObstacles[i] == null)
            {
                spawnedObstacles.RemoveAt(i);
                continue;
            }

            if (spawnedObstacles[i].transform.position.z < player.position.z - despawnBehindDistance)
            {
                Destroy(spawnedObstacles[i]);
                spawnedObstacles.RemoveAt(i);
            }
        }
    }

    void SpawnObstacleSection()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;
        
        if (Random.Range(0f, 1f) > spawnChance) 
        {
            nextSpawnZ += Random.Range(minDistanceBetweenObstacles, maxDistanceBetweenObstacles);
            return; 
        }

        float distanceFromLast = nextSpawnZ - lastSpawnZ;
        if (distanceFromLast < minDistanceBetweenObstacles)
        {
            nextSpawnZ = lastSpawnZ + minDistanceBetweenObstacles;
        }

        int obstaclesToSpawn = Random.Range(minObstaclesPerSection, maxObstaclesPerSection + 1);
        List<int> usedLanes = new List<int>();

        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            int lane = ChooseRandomLane(usedLanes);
            if (lane == -1) break; 

            usedLanes.Add(lane);

            GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            
            float xPosition = CalculateLaneXPosition(lane);
            Vector3 spawnPosition = new Vector3(xPosition, 0f, nextSpawnZ);

            GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
            
            if (!obstacle.CompareTag("Obstacle"))
            {
                obstacle.tag = "Obstacle";
            }

            spawnedObstacles.Add(obstacle);
        }

        lastSpawnZ = nextSpawnZ;
        nextSpawnZ += Random.Range(minDistanceBetweenObstacles, maxDistanceBetweenObstacles);
    }

    int ChooseRandomLane(List<int> usedLanes)
    {
        List<int> availableLanes = new List<int>();
        for (int i = 0; i < numberOfLanes; i++)
        {
            if (!usedLanes.Contains(i))
            {
                availableLanes.Add(i);
            }
        }

        if (availableLanes.Count == 0) return -1;
        return availableLanes[Random.Range(0, availableLanes.Count)];
    }

    float CalculateLaneXPosition(int lane)
    {
        if (numberOfLanes == 1)
        {
            return 0f; 
        }

        float laneWidth = spawnWidth / numberOfLanes;
        float startX = -spawnWidth / 2f + laneWidth / 2f;
        return startX + (lane * laneWidth);
    }

    public void SpawnObstacleAtPosition(Vector3 position, GameObject obstaclePrefab = null)
    {
        if (obstaclePrefab == null)
        {
            if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;
            obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        }

        GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity);
        
        if (!obstacle.CompareTag("Obstacle"))
        {
            obstacle.tag = "Obstacle";
        }

        spawnedObstacles.Add(obstacle);
    }

    public void ClearAllObstacles()
    {
        foreach (GameObject obstacle in spawnedObstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
            }
        }
        spawnedObstacles.Clear();
    }

    public void ResetSpawnTimer(float spawnDistance)
    {
        nextSpawnZ = spawnDistance + initialSpawnDistance;
        lastSpawnZ = spawnDistance - 1000f; 
    }

    public void ResetForRespawn(float spawnDistance)
    {
        ClearAllObstacles();
        ResetSpawnTimer(spawnDistance);
    }
}