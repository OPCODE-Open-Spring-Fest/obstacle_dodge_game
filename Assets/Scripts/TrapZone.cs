using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrapZone : MonoBehaviour
{
    [Tooltip("Hearts to remove from the player on enter.")]
    [SerializeField] private int heartsToRemove = 2;
    [Tooltip("If true, the trap only triggers once per player entry and then ignores further entries until scene reload.")]
    [SerializeField] private bool consumeOnce = true;
    [Tooltip("Show a warning when the player is within this many meters of the trap.")]
    [SerializeField] private float warningRange = 2f;
    [Tooltip("UI Text (Legacy) to display the trap warning (optional).")]
    [SerializeField] private Text warningText;
    [Tooltip("TMP Text to display the trap warning (optional).")]
    [SerializeField] private TextMeshProUGUI warningTMP;
    [Tooltip("Custom warning message text.")]
    [SerializeField] private string warningMessage = "Warning: Trap nearby!";
    [Tooltip("If no UI is assigned, try to find a UI object by this name at runtime.")]
    [SerializeField] private string warningObjectName = "TrapWarningText";
    [Tooltip("Enable to print distance debug logs and draw gizmos for range.")]
    [SerializeField] private bool debugLogs = false;

    [Header("Collision FX")]
    [Tooltip("Optional effect to spawn when the player triggers the trap.")]
    [SerializeField] private GameObject collisionEffectPrefab;
    [Tooltip("Auto-destroy time for the spawned effect. Set 0 to persist.")]
    [SerializeField] private float effectLifetime = 3f;
    [Tooltip("Spawn effect at the player's position (true) or at the trap surface closest to the player (false)")]
    [SerializeField] private bool effectAtPlayer = false;

    [Header("Camera Shake (Cinemachine)")]
    [Tooltip("If a Cinemachine Impulse Source is on the Player, trigger it on trap.")]
    [SerializeField] private bool triggerCameraShake = true;
    private Component impulseSourceComponent; // CinemachineImpulseSource (resolved via reflection)
    private System.Reflection.MethodInfo generateImpulseMethod;

    private bool triggered;
    private Transform player;
    private Collider trapCollider;

    private void Awake()
    {
        trapCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (player == null)
        {
            var pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) player = pObj.transform;
            // Try wire Cinemachine impulse from player if enabled
            if (player != null && triggerCameraShake && impulseSourceComponent == null)
            {
                var impulseType = System.Type.GetType("Cinemachine.CinemachineImpulseSource, Cinemachine");
                if (impulseType == null)
                {
                    impulseSourceComponent = player.GetComponent("CinemachineImpulseSource");
                }
                else
                {
                    impulseSourceComponent = player.GetComponent(impulseType);
                }
                if (impulseSourceComponent != null)
                {
                    generateImpulseMethod = impulseSourceComponent.GetType().GetMethod("GenerateImpulse", System.Type.EmptyTypes);
                }
            }
        }
        // Lazy UI lookup if not assigned
        if (warningText == null && warningTMP == null && !string.IsNullOrEmpty(warningObjectName))
        {
            var go = GameObject.Find(warningObjectName);
            if (go != null)
            {
                warningTMP = go.GetComponent<TextMeshProUGUI>();
                if (warningTMP == null) warningText = go.GetComponent<Text>();
            }
        }
        // Gracefully handle no player or no UI assigned
        if (player == null)
        {
            if (warningText != null) warningText.enabled = false;
            if (warningTMP != null) warningTMP.enabled = false;
            return;
        }

        // Measure distance to the collider surface if available, else to transform
        float dist;
        if (trapCollider != null)
        {
            Vector3 closest = trapCollider.ClosestPoint(player.position);
            dist = Vector3.Distance(closest, player.position);
        }
        else
        {
            dist = Vector3.Distance(transform.position, player.position);
        }

        bool show = dist <= warningRange;
        if (debugLogs)
        {
            Debug.Log($"[TrapZone] distance={dist:F2}, show={show}");
        }
        if (warningText != null)
        {
            warningText.enabled = show;
            if (show) warningText.text = warningMessage;
        }
        if (warningTMP != null)
        {
            warningTMP.enabled = show;
            if (show) warningTMP.text = warningMessage;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, warningRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var collectibles = other.GetComponent<PlayerCollectibles>();
        if (collectibles == null) return;
        if (collectibles.IsInvincible) return;
        if (consumeOnce && triggered) return;

        // Check if this will kill the player before removing hearts
        int currentLives = collectibles.GetLives();
        bool willDie = (currentLives - Mathf.Abs(heartsToRemove)) <= 0;
        
        // Remove hearts
        collectibles.AddLife(-Mathf.Abs(heartsToRemove));

        // Spawn optional effect
        if (collisionEffectPrefab != null)
        {
            Vector3 spawnPos;
            if (effectAtPlayer)
            {
                spawnPos = other.transform.position;
            }
            else if (trapCollider != null)
            {
                spawnPos = trapCollider.ClosestPoint(other.transform.position);
            }
            else
            {
                spawnPos = transform.position;
            }
            var fx = Instantiate(collisionEffectPrefab, spawnPos, Quaternion.identity);
            if (effectLifetime > 0f) Destroy(fx, effectLifetime);
        }

        // Camera shake
        if (triggerCameraShake && impulseSourceComponent != null && generateImpulseMethod != null)
        {
            generateImpulseMethod.Invoke(impulseSourceComponent, null);
        }

        // If no lives left, go to Game Over immediately
        if (willDie || collectibles.GetLives() <= 0)
        {
            LastLevelRecorder.SaveAndLoad("GameOver");
            return; // Exit early to prevent further processing
        }

        if (consumeOnce) triggered = true;
    }
}


