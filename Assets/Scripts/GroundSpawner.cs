using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public GameObject groundTile;
    Vector3 nextSpawnPoint;
    int length = 25;
    [SerializeField] private float spawnAheadDistance = 50f;
    [SerializeField] private int minTilesAhead = 5;
    
    private GameObject player;
    private float lastCheckTime;
    private float checkInterval = 0.5f;
    
    void Start()
    {
        if (groundTile == null)
        {
            Debug.LogError("[GroundSpawner] Ground Tile prefab not assigned!");
            return;
        }
        
        if (nextSpawnPoint == Vector3.zero)
        {
            nextSpawnPoint = transform.position;
        }
        
        for (int i = 0; i < length; i++)
        {
            GenerateTile();
        }
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj;
        }
        
        lastCheckTime = Time.time;
        
        Debug.Log($"[GroundSpawner] Initialized. Next spawn point: {nextSpawnPoint}");
    }
    
    void Update()
    {
        if (player == null || groundTile == null) return;
        
        if (Time.time - lastCheckTime >= checkInterval)
        {
            CheckAndSpawnAhead();
            lastCheckTime = Time.time;
        }
    }
    
    private void CheckAndSpawnAhead()
    {
        if (player == null || nextSpawnPoint == Vector3.zero) return;
        
        float distanceToNextSpawn = Vector3.Distance(player.transform.position, nextSpawnPoint);
        
        if (distanceToNextSpawn < spawnAheadDistance)
        {
            for (int i = 0; i < minTilesAhead; i++)
            {
                GenerateTile();
            }
            Debug.Log($"[GroundSpawner] Spawned {minTilesAhead} tiles ahead. Distance to next: {distanceToNextSpawn:F1}m");
        }
    }
    
    public void GenerateTile()
    {
        if (groundTile == null) return;
        
        GameObject newTile = Instantiate(groundTile, nextSpawnPoint, Quaternion.identity);
        if (newTile.transform.childCount > 0)
        {
            nextSpawnPoint = newTile.transform.GetChild(0).position;
        }
        else
        {
            nextSpawnPoint += Vector3.forward * 5f;
        }
    }
}
