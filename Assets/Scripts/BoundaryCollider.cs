using UnityEngine;

public class BoundaryCollider : MonoBehaviour
{
    public bool playCollisionSound = true;
    public GameObject collisionEffectPrefab;
    public bool triggerCameraShake = false;
    
    private Component impulseSourceComponent;
    private System.Reflection.MethodInfo generateImpulseMethod;

    private void Awake()
    {
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

        if (playCollisionSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollisionSFX();
        }

        if (collisionEffectPrefab != null)
        {
            Vector3 spawnPos = other.ClosestPoint(transform.position);
            Instantiate(collisionEffectPrefab, spawnPos, Quaternion.identity);
        }

        if (triggerCameraShake && impulseSourceComponent != null && generateImpulseMethod != null)
        {
            generateImpulseMethod.Invoke(impulseSourceComponent, null);
        }

        DeathHelper.TriggerDeath(other.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
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

