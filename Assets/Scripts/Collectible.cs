using UnityEngine;

public enum CollectibleType { Heart, Gem }

public class Collectible : MonoBehaviour
{
    public CollectibleType type = CollectibleType.Heart;
    [Tooltip("How many hearts this pickup gives (only for Heart type)")]
    public int heartAmount = 1;
    [Tooltip("Duration of invincibility in seconds (only for Gem type)")]
    public float gemDuration = 5f;

    private bool isCollected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        if (!other.CompareTag("Player")) return;

        var p = other.GetComponent<PlayerCollectibles>();
        if (p == null) return;

        isCollected = true;

        switch (type)
        {
            case CollectibleType.Heart:
                p.AddLife(heartAmount);
                break;
            case CollectibleType.Gem:
                p.UseGem(gemDuration);
                break;
        }

        Destroy(gameObject);
    }
}
