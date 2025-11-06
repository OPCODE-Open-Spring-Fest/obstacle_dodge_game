
using UnityEngine;
using UnityEngine.SceneManagement;
// (no direct Cinemachine using to avoid hard dependency)

public class Scorer : MonoBehaviour
{
    [SerializeField] private int defaultLimit = 15;  // fallback if level not listed
    private int hits = 0;

    [Header("FX")]  // Helps group in Inspector
    [Tooltip("Particle system prefab to play on collision")]  
    [SerializeField] private GameObject collisionEffectPrefab;  // Inspector-visible

    // Optional Cinemachine impulse (resolved at runtime to avoid compile-time dependency)
    private Component impulseSourceComponent; // holds CinemachineImpulseSource if available
    private System.Reflection.MethodInfo generateImpulseMethod;

    private void Awake()
    {
        // Try to get CinemachineImpulseSource without compile-time reference
        // Method 1: by full type name (if assembly name is available)
        var impulseType = System.Type.GetType("Cinemachine.CinemachineImpulseSource, Cinemachine");
        if (impulseType == null)
        {
            // Method 2: fallback by component name
            impulseSourceComponent = GetComponent("CinemachineImpulseSource");
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
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        switch (buildIndex)
        {
            case 1: defaultLimit = 15; break;
            case 4: defaultLimit = 8; break;
            case 5: defaultLimit = 4; break;
            default: defaultLimit = 15; break;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Collectible")) return;

        if (other.gameObject.CompareTag("Hit")) return;

        var collectibles = GetComponent<PlayerCollectibles>();
        if (collectibles != null && collectibles.IsInvincible) return;

        // Trigger VFX & camera shake on valid hit
        if ((collectibles != null) || (hits < defaultLimit))
        {
            // VFX - Spawn at the impact point every collision; do not parent so it stays there
            if (collisionEffectPrefab != null && other.contacts.Length > 0)
            {
                Vector3 impactPoint = other.contacts[0].point;
                Quaternion impactRotation = Quaternion.identity;
                Instantiate(collisionEffectPrefab, impactPoint, impactRotation);
            }
            // Play collision sound effect
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCollisionSFX();
            }
            // Camera shake via Cinemachine impulse (if component exists)
            if (impulseSourceComponent != null && generateImpulseMethod != null)
            {
                generateImpulseMethod.Invoke(impulseSourceComponent, null);
            }
        }

        if (collectibles != null)
        {
            collectibles.AddLife(-1);
            Debug.Log($"Player hit an obstacle. Lives now: {collectibles.GetLives()}");

            if (collectibles.GetLives() <= 0)
            {
                Debug.Log("No lives left. Loading Game Over scene...");
                LastLevelRecorder.SaveAndLoad("GameOver");
            }
            return;
        }

        hits++;
        Debug.Log($"You bumped into a thing this many times: {hits}");

        if (hits >= defaultLimit)
        {
            Debug.Log("Max hit limit reached! Loading Game Over scene...");
            LastLevelRecorder.SaveAndLoad("GameOver");
        }
    }
}
