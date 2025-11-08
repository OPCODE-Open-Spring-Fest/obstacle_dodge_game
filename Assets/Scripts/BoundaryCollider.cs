using UnityEngine;

/// <summary>
/// Script to attach to boundary colliders around the gameplay area.
/// When the player touches these boundaries, the game immediately ends.
/// Make sure the collider is set as a Trigger.
/// </summary>
public class BoundaryCollider : MonoBehaviour
{
    [Tooltip("Optional: Play collision sound effect when player hits boundary")]
    public bool playCollisionSound = true;
    
    [Tooltip("Optional: Spawn an effect at the collision point")]
    public GameObject collisionEffectPrefab;
    
    [Tooltip("Optional: Camera shake on collision (requires Cinemachine Impulse Source)")]
    public bool triggerCameraShake = false;
    
    private Component impulseSourceComponent;
    private System.Reflection.MethodInfo generateImpulseMethod;

    private void Awake()
    {
        // Setup camera shake (optional Cinemachine integration)
        // Try to get CinemachineImpulseSource without compile-time reference
        var impulseType = System.Type.GetType("Cinemachine.CinemachineImpulseSource, Cinemachine");
        if (impulseType == null)
        {
            // Fallback by component name
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
        // Ensure collider is set as trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"[BoundaryCollider] Collider on {gameObject.name} is not set as Trigger. Setting it now.");
            col.isTrigger = true;
        }
        else if (col == null)
        {
            Debug.LogError($"[BoundaryCollider] No Collider component found on {gameObject.name}. Please add a Collider.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Debug.Log("Player touched boundary! Game Over.");

        // Play collision sound
        if (playCollisionSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollisionSFX();
        }

        // Spawn collision effect
        if (collisionEffectPrefab != null)
        {
            Vector3 spawnPos = other.ClosestPoint(transform.position);
            Instantiate(collisionEffectPrefab, spawnPos, Quaternion.identity);
        }

        // Camera shake
        if (triggerCameraShake && impulseSourceComponent != null && generateImpulseMethod != null)
        {
            generateImpulseMethod.Invoke(impulseSourceComponent, null);
        }

        // Immediately trigger game over
        LastLevelRecorder.SaveAndLoad("GameOver");
    }

    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Red with transparency
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
        }
    }
}

