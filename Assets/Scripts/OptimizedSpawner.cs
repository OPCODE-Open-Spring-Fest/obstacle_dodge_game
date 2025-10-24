using UnityEngine;
using System.Collections.Generic;

public class OptimizedSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnDistance = 20f;
    [SerializeField] private float despawnDistance = 30f;
    
    [Header("Object Pooling")]
    [SerializeField] private int poolSize = 20;
    [SerializeField] private bool useObjectPooling = true;
    
    private List<GameObject> obstaclePool;
    private List<GameObject> activeObstacles;
    private float lastSpawnTime;
    private Transform playerTransform;
    
    void Start()
    {
        InitializeSpawner();
    }
    
    void InitializeSpawner()
    {
        // find player transform
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        // initialize object pool
        if (useObjectPooling)
        {
            obstaclePool = new List<GameObject>();
            activeObstacles = new List<GameObject>();
            
            // pre-instantiate objects for pooling
            for (int i = 0; i < poolSize; i++)
            {
                if (obstaclePrefabs.Length > 0)
                {
                    GameObject obstacle = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)]);
                    obstacle.SetActive(false);
                    obstaclePool.Add(obstacle);
                }
            }
        }
        
        lastSpawnTime = Time.time;
    }
    
    void Update()
    {
        if (GameplayBalancer.Instance != null)
        {
            SpawnObstacles();
            ManageObstacles();
        }
    }
    
    void SpawnObstacles()
    {
        float spawnRate = GameplayBalancer.Instance.GetCurrentSpawnRate();
        float timeSinceLastSpawn = Time.time - lastSpawnTime;
        
        if (timeSinceLastSpawn >= 1f / spawnRate)
        {
            SpawnObstacle();
            lastSpawnTime = Time.time;
        }
    }
    
    void SpawnObstacle()
    {
        if (spawnPoints.Length == 0 || obstaclePrefabs.Length == 0) return;
        
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        
        GameObject obstacle;
        
        if (useObjectPooling && obstaclePool.Count > 0)
        {
            // get from pool
            obstacle = obstaclePool[0];
            obstaclePool.RemoveAt(0);
            obstacle.SetActive(true);
            obstacle.transform.position = spawnPoint.position;
            obstacle.transform.rotation = spawnPoint.rotation;
        }
        else
        {
            // instantiate new
            obstacle = Instantiate(obstaclePrefab, spawnPoint.position, spawnPoint.rotation);
        }
        
        // apply current speed to obstacle
        if (obstacle != null)
        {
            Rigidbody rb = obstacle.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.back * GameplayBalancer.Instance.GetCurrentObstacleSpeed();
            }
            
            if (useObjectPooling)
            {
                activeObstacles.Add(obstacle);
            }
        }
    }
    
    void ManageObstacles()
    {
        if (!useObjectPooling || playerTransform == null) return;
        
        // check for obstacles that needs to be returned to pool
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            GameObject obstacle = activeObstacles[i];
            
            if (obstacle == null)
            {
                activeObstacles.RemoveAt(i);
                continue;
            }
            
            float distance = Vector3.Distance(obstacle.transform.position, playerTransform.position);
            
            if (distance > despawnDistance)
            {
                // return to pool
                obstacle.SetActive(false);
                obstaclePool.Add(obstacle);
                activeObstacles.RemoveAt(i);
            }
        }
    }
    
    void OnDestroy()
    {
        // clean up pooled objects
        if (obstaclePool != null)
        {
            foreach (GameObject obj in obstaclePool)
            {
                if (obj != null)
                    Destroy(obj);
            }
        }
        
        if (activeObstacles != null)
        {
            foreach (GameObject obj in activeObstacles)
            {
                if (obj != null)
                    Destroy(obj);
            }
        }
    }
}
