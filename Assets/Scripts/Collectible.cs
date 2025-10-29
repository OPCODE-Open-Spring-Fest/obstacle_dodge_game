using UnityEngine;

public enum CollectibleType { Heart, Gem }

public class Collectible : MonoBehaviour
{
    public CollectibleType type = CollectibleType.Heart;
    public int heartAmount = 1;
    public float gemDuration = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var p = other.GetComponent<PlayerCollectibles>();
        if (p == null) return;

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
