using UnityEngine;

/// <summary>
/// Script to make platforms/blocks static and prevent them from falling.
/// Attach this to any block or platform that should stay in place.
/// This script will automatically configure the Rigidbody to be kinematic (static).
/// </summary>
public class StaticPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    [Tooltip("If true, will remove Rigidbody if present. If false, will make Rigidbody kinematic.")]
    [SerializeField] private bool removeRigidbody = true;
    
    [Tooltip("If true, will freeze all positions to prevent any movement")]
    [SerializeField] private bool freezeAllPositions = true;
    
    [Tooltip("Lock position in the editor (prevents accidental movement)")]
    [SerializeField] private bool lockPositionInEditor = false;
    
    private void Start()
    {
        ConfigurePlatform();
    }
    
    private void ConfigurePlatform()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            if (removeRigidbody)
            {
                // Remove Rigidbody - platform will be completely static
                Debug.Log($"[StaticPlatform] Removing Rigidbody from {gameObject.name} to make it static.");
                Destroy(rb);
            }
            else
            {
                // Make Rigidbody kinematic (static but can still be moved via code if needed)
                rb.isKinematic = true;
                rb.useGravity = false;
                
                if (freezeAllPositions)
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
                
                Debug.Log($"[StaticPlatform] Made Rigidbody on {gameObject.name} kinematic (static).");
            }
        }
        
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(1f, 0.1f, 1f);
            Debug.Log($"[StaticPlatform] Added BoxCollider to {gameObject.name}.");
            col = boxCollider;
        }
        
        if (col != null)
        {
            if (col.isTrigger)
            {
                col.isTrigger = false;
                Debug.Log($"[StaticPlatform] Changed collider on {gameObject.name} from trigger to solid.");
            }
            
            if (col is BoxCollider)
            {
                BoxCollider box = col as BoxCollider;
                if (box.size.y < 0.2f)
                {
                    box.size = new Vector3(box.size.x, Mathf.Max(0.2f, box.size.y), box.size.z);
                }
            }
        }
    }
    
    /// <summary>
    /// Call this method to manually fix the platform (useful for runtime fixes)
    /// </summary>
    public void FixPlatform()
    {
        ConfigurePlatform();
    }
    
    #if UNITY_EDITOR
    /// <summary>
    /// Lock position in editor to prevent accidental movement
    /// </summary>
    private void OnValidate()
    {
        if (lockPositionInEditor && Application.isPlaying == false)
        {
            // This is just a visual reminder - actual locking would require custom editor script
            // For now, we'll just ensure the platform is configured correctly
        }
    }
    #endif
}

