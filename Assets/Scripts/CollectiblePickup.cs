using UnityEngine;

public enum PickupType { Heart, Gem }

public class CollectiblePickup : MonoBehaviour
{
    public PickupType pickupType = PickupType.Heart;

    [Tooltip("How many hearts this pickup gives (only for Heart type)")]
    public int heartAmount = 1;

    [Tooltip("Duration of invincibility in seconds (only for Gem type)")]
    public float invincibilityDuration = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var playerCollectibles = other.GetComponent<PlayerCollectibles>();
        if (playerCollectibles == null)
        {
            Debug.LogWarning("Player does not have PlayerCollectibles component. Attach it to the Player.");
            return;
        }

        switch (pickupType)
        {
            case PickupType.Heart:
                playerCollectibles.AddLives(heartAmount);
                break;
            case PickupType.Gem:
                playerCollectibles.StartInvincibility(invincibilityDuration);
                break;
        }

        Destroy(gameObject);
    }
}
