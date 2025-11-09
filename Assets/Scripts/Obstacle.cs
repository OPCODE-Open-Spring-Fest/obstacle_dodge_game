using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Debug.Log("[Obstacle] Player collided with obstacle! Game Over.");

        PlayerCollectibles collectibles = collision.gameObject.GetComponent<PlayerCollectibles>();
        if (collectibles != null && collectibles.IsInvincible) return;

        DeathHelper.TriggerDeath(collision.gameObject);
    }
}
