using UnityEngine;

public class GroundTile : MonoBehaviour
{
    GroundSpawner groundSpawner;
    private bool hasSpawnedNext = false;
    
    void Start()
    {
        groundSpawner = GameObject.FindObjectOfType<GroundSpawner>();
        if (groundSpawner == null)
        {
            Debug.LogError("[GroundTile] GroundSpawner not found!");
        }
        SpawnObstacle();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (!hasSpawnedNext && groundSpawner != null)
        {
            hasSpawnedNext = true;
            groundSpawner.GenerateTile();
        }
        
        Destroy(gameObject, 2);
    }
    
    public GameObject obstacleprefab;

    void SpawnObstacle()
    {
        if (obstacleprefab == null) return;
        if (transform.childCount < 5) return;
        
        int obstacleSpawnIndex = Random.Range(2, 5);
        Transform spawnPoint = transform.GetChild(obstacleSpawnIndex).transform;
        Instantiate(obstacleprefab, spawnPoint.position, Quaternion.identity);
    }
}
