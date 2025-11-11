using UnityEngine;
using System.Collections.Generic;

public class GroundSpawner : MonoBehaviour
{
    [Header("Ground Settings")]
    [Tooltip("The ground tile prefab to spawn")]
    public GameObject groundTile;
    
    [Tooltip("How many tiles to spawn ahead of the player")]
    public int tilesAhead = 5;
    
    [Tooltip("Length of each ground tile")]
    public float tileLength = 10f;
    
    [Tooltip("Distance to despawn tiles behind the player")]
    public float despawnDistance = 20f;
    
    [Header("Spawn Settings")]
    [Tooltip("Width of the ground (for spawning obstacles on sides)")]
    public float groundWidth = 10f;
    
    private Transform player;
    private Queue<GameObject> spawnedTiles = new Queue<GameObject>();
    private float nextSpawnZ = 0f;
    private bool isInitialized = false;
    private EndlessGameManager gameManager;

    void Start()
    {
        gameManager = EndlessGameManager.Instance;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("GroundSpawner: Player not found! Make sure player has 'Player' tag.");
            return;
        }

        if (groundTile == null)
        {
            Debug.LogError("GroundSpawner: Ground tile prefab not assigned!");
            return;
        }

        if (tileLength <= 0)
        {
            Debug.LogError("GroundSpawner: tileLength must be a positive value! Disabling script.");
            enabled = false;
            return;
        }

        SpawnInitialTiles(0f);
        
        isInitialized = true;
    }

    void SpawnInitialTiles(float startZ)
    {
        nextSpawnZ = startZ;
        for (int i = 0; i < tilesAhead; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        if (!isInitialized || player == null || (gameManager != null && !gameManager.isGameRunning)) return;

        while (nextSpawnZ < player.position.z + (tilesAhead * tileLength))
        {
            SpawnTile();
        }

        while (spawnedTiles.Count > 0)
        {
            GameObject oldestTile = spawnedTiles.Peek();
            if (oldestTile == null)
            {
                spawnedTiles.Dequeue();
                continue;
            }

            if (oldestTile.transform.position.z < player.position.z - despawnDistance)
            {
                Destroy(oldestTile);
                spawnedTiles.Dequeue();
            }
            else
            {
                break;
            }
        }
    }

    void SpawnTile()
    {
        if (groundTile == null) return;

        Vector3 spawnPosition = new Vector3(0f, 0f, nextSpawnZ);
        GameObject newTile = Instantiate(groundTile, spawnPosition, Quaternion.identity);
        spawnedTiles.Enqueue(newTile);
        nextSpawnZ += tileLength;
    }

    public float GetGroundWidth()
    {
        return groundWidth;
    }

    public void ClearAllTiles()
    {
        while (spawnedTiles.Count > 0)
        {
            GameObject tile = spawnedTiles.Dequeue();
            if (tile != null)
            {
                Destroy(tile);
            }
        }
        spawnedTiles.Clear();
    }

    public void ResetForRespawn(float spawnDistance)
    {
        ClearAllTiles();
        SpawnInitialTiles(spawnDistance);
    }
}