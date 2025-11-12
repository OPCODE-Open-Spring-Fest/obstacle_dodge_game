using UnityEngine;

public class PlayerObstacleCollision : MonoBehaviour
{
    [Header("Collision Settings")]
    [Tooltip("Tag name for obstacles")]
    public string obstacleTag = "Obstacle";
    
    [Tooltip("Cooldown time between collisions (prevents multiple triggers)")]
    public float collisionCooldown = 1.5f; // Increased cooldown for respawn
    
    private float lastCollisionTime = -1f;

    // We no longer need isDead or DeathHelper

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(obstacleTag))
        {
            HandleObstacleCollision();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(obstacleTag))
        {
            HandleObstacleCollision();
        }
    }

    void HandleObstacleCollision()
    {
        // Cooldown check
        if (Time.time - lastCollisionTime < collisionCooldown)
        {
            return;
        }

        lastCollisionTime = Time.time;

        // --- NEW LOGIC ---
        // Tell the GameManager we got hit
        if (EndlessGameManager.Instance != null)
        {
            EndlessGameManager.Instance.PlayerHitObstacle();
        }
    }

    // This is called by the Respawn coroutine to allow hits again
    public void ResetCooldown()
    {
        lastCollisionTime = Time.time;
    }

    // We removed IsDead() and ResetDeath() as they are no longer needed
}