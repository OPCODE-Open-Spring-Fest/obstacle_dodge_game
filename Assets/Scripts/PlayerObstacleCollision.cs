using UnityEngine;

public class PlayerObstacleCollision : MonoBehaviour
{
    [Header("Collision Settings")]
    [Tooltip("Tag name for obstacles")]
    public string obstacleTag = "Obstacle";
    
    [Tooltip("Cooldown time between collisions (prevents multiple triggers)")]
    public float collisionCooldown = 0.5f;
    
    [Header("Death Settings")]
    [Tooltip("Delay before triggering death after collision")]
    public float deathDelay = 0.1f;
    
    private float lastCollisionTime = -1f;
    private bool isDead = false;

    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;
        
        // Check if collided with obstacle
        if (collision.gameObject.CompareTag(obstacleTag))
        {
            HandleObstacleCollision(collision);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        
        // Check if triggered with obstacle
        if (other.gameObject.CompareTag(obstacleTag))
        {
            HandleObstacleCollision(other);
        }
    }

    void HandleObstacleCollision(Collision collision)
    {
        // Cooldown check
        if (Time.time - lastCollisionTime < collisionCooldown)
        {
            return;
        }

        lastCollisionTime = Time.time;
        TriggerDeath();
    }

    void HandleObstacleCollision(Collider collider)
    {
        // Cooldown check
        if (Time.time - lastCollisionTime < collisionCooldown)
        {
            return;
        }

        lastCollisionTime = Time.time;
        TriggerDeath();
    }

    void TriggerDeath()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Player collided with obstacle! Game Over!");

        // Disable player movement
        PlayerEndlessMovement movement = GetComponent<PlayerEndlessMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        // Stop rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }

        // Use DeathHelper to handle death animation and scene transition
        DeathHelper.TriggerDeath(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void ResetDeath()
    {
        isDead = false;
        lastCollisionTime = -1f;
    }
}

