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
                // Play heart collect SFX via AudioManager if available
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayHeartCollectSFX();
                }
                break;
            case CollectibleType.Gem:
                p.UseGem(gemDuration);
                // Play gem collect SFX via AudioManager if available
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayGemCollectSFX();
                }
                break;
        }

        Destroy(gameObject);
    }
}
