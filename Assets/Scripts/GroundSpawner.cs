using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public GameObject groundTile;
    Vector3 nextSpawnPoint;
    int length = 25;
    public void GenerateTile()
    {
        GameObject newTile = Instantiate(groundTile, nextSpawnPoint, Quaternion.identity);
        nextSpawnPoint = newTile.transform.GetChild(0).position;
    }
    void Start()
    {
        for (int i = 0; i < length; i++)
        {
            GenerateTile();
        }
    }

}
