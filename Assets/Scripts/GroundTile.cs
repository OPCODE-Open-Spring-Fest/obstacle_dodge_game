using UnityEngine;

public class GroundTile : MonoBehaviour
{
    GroundSpawner groundSpawner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        groundSpawner = GameObject.FindObjectOfType<GroundSpawner>();
        SpawnObstacle();
    }
    private void OnTriggerExit(Collider other)
    {
            groundSpawner.GenerateTile();
            Destroy(gameObject,2);
    }
    public GameObject obstacleprefab;

    void SpawnObstacle()
    {
        int obstacleSpawnIndex = Random.Range(2,5);
        Transform spawnPoint = transform.GetChild(obstacleSpawnIndex).transform;
        Instantiate(obstacleprefab, spawnPoint.position, Quaternion.identity);
    }
}
