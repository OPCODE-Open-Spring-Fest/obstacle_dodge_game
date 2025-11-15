using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;

public class Scorer : MonoBehaviour
{
    [SerializeField] private int defaultLimit = 15;
    private int hits = 0;

    [Header("FX")]
    [Tooltip("Particle system prefab to play on collision")]
    [SerializeField] private GameObject collisionEffectPrefab;

    private Component impulseSourceComponent;
    private MethodInfo generateImpulseMethod;

    private void Awake()
    {
        var impulseType = System.Type.GetType("Cinemachine.CinemachineImpulseSource, Cinemachine");
        if (impulseType == null)
        {
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

        // --- ADDED THIS LINE ---
        // Reset the streak/combo on a valid hit
        if (StreakManager.Instance != null)
        {
            StreakManager.Instance.ResetStreak();
        }
        // --- END OF ADDITION ---

        if ((collectibles != null) || (hits < defaultLimit))
        {
            if (collisionEffectPrefab != null && other.contacts.Length > 0)
            {
                Vector3 impactPoint = other.contacts[0].point;
                Quaternion impactRotation = Quaternion.identity;
                Instantiate(collisionEffectPrefab, impactPoint, impactRotation);
            }
            
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
                DeathHelper.TriggerDeath(gameObject);
            }
            return;
        }

        hits++;
        Debug.Log($"You bumped into a thing this many times: {hits}");

        if (hits >= defaultLimit)
        {
            Debug.Log("Max hit limit reached! Loading Game Over scene...");
            DeathHelper.TriggerDeath(gameObject);
        }
    }
}