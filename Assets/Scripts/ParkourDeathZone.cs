using UnityEngine;

public class ParkourDeathZone : MonoBehaviour
{
    [SerializeField] private bool useTrigger = true;
    
    [SerializeField] private string gameOverSceneName = "";
    
    private Component impulseSourceComponent;
    private System.Reflection.MethodInfo generateImpulseMethod;
    private bool gameOverTriggered = false;
    
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
        if (col == null)
        {
            Debug.LogError($"[ParkourDeathZone] No Collider component found on {gameObject.name}. Please add a Collider.");
        }
        else
        {
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
    
    private void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        TriggerGameOver(other.gameObject);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (useTrigger) return;
        
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        
        TriggerGameOver(collision.gameObject);
    }
    
    private void TriggerGameOver(GameObject playerObject)
    {
        if (gameOverTriggered)
        {
            return;
        }
        
        gameOverTriggered = true;
        Debug.Log($"[ParkourDeathZone] Player touched death zone on {gameObject.name}. Game Over!");
        
        if (impulseSourceComponent != null && generateImpulseMethod != null)
        {
            generateImpulseMethod.Invoke(impulseSourceComponent, null);
        }

        // --- NEW CODE ---
        // Stop the timer so it can't load its own game over scene
        LevelTimer timer = FindObjectOfType<LevelTimer>();
        if (timer != null)
        {
            timer.StopTimer();
        }
        // --- END OF NEW CODE ---
        
        PlayerDeathAnimator deathAnimator = playerObject.GetComponent<PlayerDeathAnimator>();
        if (deathAnimator != null)
        {
            deathAnimator.PlayDeathAnimation();
        }
        else
        {
            LastLevelRecorder.SaveAndLoad(gameOverSceneName);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
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