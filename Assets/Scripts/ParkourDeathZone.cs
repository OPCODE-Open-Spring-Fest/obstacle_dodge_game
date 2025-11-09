using UnityEngine;

/// <summary>
/// Script for parkour levels that triggers game over when player touches a death zone.
/// Works with both trigger and non-trigger colliders.
/// Attach this to any collider that should kill the player on contact.
/// </summary>
public class ParkourDeathZone : MonoBehaviour
{
    [Header("Death Zone Settings")]
    [Tooltip("If true, the collider must be a trigger. If false, uses collision detection.")]
    [SerializeField] private bool useTrigger = true;
    
    [Tooltip("Optional: Play sound effect when player touches death zone")]
    [SerializeField] private bool playDeathSound = true;
    
    [Tooltip("Optional: Spawn an effect at the collision point")]
    [SerializeField] private GameObject deathEffectPrefab;
    
    [Tooltip("Optional: Delay before loading game over scene")]
    [SerializeField] private float gameOverDelay = 0.5f;
    
    [Tooltip("Optional: Camera shake on death (requires Cinemachine Impulse Source)")]
    [SerializeField] private bool triggerCameraShake = false;
    
    private Component impulseSourceComponent;
    private System.Reflection.MethodInfo generateImpulseMethod;
    private bool gameOverTriggered = false;
    
    private void Awake()
    {
        // Setup camera shake (optional Cinemachine integration)
        var impulseType = System.Type.GetType("Cinemachine.CinemachineImpulseSource, Cinemachine");
        if (impulseType == null)
        {
            impulseSourceComponent = GetComponent("CinemachineImpulseSource") as Component;
        }
        else
        {
            impulseSourceComponent = GetComponent(impulseType);
        }

        if (impulseSourceComponent != null)
        {
            generateImpulseMethod = impulseSourceComponent.GetType().GetMethod("GenerateImpulse", System.Type.EmptyTypes);
        }
    }
    
    private void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"[ParkourDeathZone] No Collider component found on {gameObject.name}. Please add a Collider.");
        }
        else
        {
            // Set collider to trigger if useTrigger is true
            if (useTrigger && !col.isTrigger)
            {
                col.isTrigger = true;
                Debug.Log($"[ParkourDeathZone] Set collider on {gameObject.name} to Trigger mode.");
            }
            else if (!useTrigger && col.isTrigger)
            {
                col.isTrigger = false;
                Debug.Log($"[ParkourDeathZone] Set collider on {gameObject.name} to Collision mode.");
            }
        }
    }
    
    // For trigger colliders
    private void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return; // Only process if using trigger mode
        
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        TriggerGameOver(other);
    }
    
    // For non-trigger colliders (collision detection)
    private void OnCollisionEnter(Collision collision)
    {
        if (useTrigger) return; // Only process if using collision mode
        
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        
        // Get the collider from the collision
        Collider other = collision.collider;
        TriggerGameOver(other);
    }
    
    private void TriggerGameOver(Collider playerCollider)
    {
        if (gameOverTriggered)
        {
            return; // Prevent multiple triggers
        }
        
        gameOverTriggered = true;
        Debug.Log($"[ParkourDeathZone] Player touched death zone on {gameObject.name}. Game Over!");
        
        // Play death sound
        if (playDeathSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollisionSFX();
        }
        
        // Spawn death effect
        if (deathEffectPrefab != null)
        {
            Vector3 spawnPos = playerCollider.ClosestPoint(transform.position);
            Instantiate(deathEffectPrefab, spawnPos, Quaternion.identity);
        }
        
        // Camera shake
        if (triggerCameraShake && impulseSourceComponent != null && generateImpulseMethod != null)
        {
            generateImpulseMethod.Invoke(impulseSourceComponent, null);
        }
        
        // Trigger game over after delay
        Invoke(nameof(LoadGameOver), gameOverDelay);
    }
    
    private void LoadGameOver()
    {
        LastLevelRecorder.SaveAndLoad("GameOver");
    }
    
    /// <summary>
    /// Draws gizmos in the Scene view to visualize the death zone
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // Red with transparency
            if (col is BoxCollider)
            {
                BoxCollider box = col as BoxCollider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphere = col as SphereCollider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawSphere(sphere.center, sphere.radius);
            }
            else if (col is CapsuleCollider)
            {
                CapsuleCollider capsule = col as CapsuleCollider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireSphere(capsule.center, capsule.radius);
            }
        }
    }
}

