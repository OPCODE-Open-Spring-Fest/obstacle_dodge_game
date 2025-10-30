using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple UI helper to display player's hearts using a heart Image prefab.
/// Put a horizontal layout group as the parent and assign `heartPrefab` and the Player GameObject.
/// </summary>
public class HeartsUI : MonoBehaviour
{
    [Tooltip("UI Image prefab representing one heart. Should be a UnityEngine.UI.Image.")]
    public Image heartPrefab;

    [Tooltip("Player GameObject that has the PlayerCollectibles component")]
    public GameObject player;

    private PlayerCollectibles playerCollectibles;
    private readonly List<Image> hearts = new List<Image>();

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("HeartsUI: Player GameObject not assigned in inspector.");
            return;
        }

        playerCollectibles = player.GetComponent<PlayerCollectibles>();
        if (playerCollectibles == null)
        {
            Debug.LogError("HeartsUI: Player does not have PlayerCollectibles component.");
            return;
        }

        // If there are already Image children under this UI (a design-time sample), include them
        // so we don't accidentally show an extra heart. Prefer assigning a prefab asset to heartPrefab
        // (from the Project window) rather than a scene instance.
        hearts.Clear();
        foreach (Transform child in transform)
        {
            var img = child.GetComponent<Image>();
            if (img != null)
                hearts.Add(img);
        }

        playerCollectibles.OnLivesChanged += UpdateHearts;
        // Initial update - guard against missing prefab
        if (heartPrefab == null)
        {
            Debug.LogError("HeartsUI: heartPrefab is not assigned. Assign a UI Image prefab representing one heart (from the Project window).");
        }
        UpdateHearts(playerCollectibles.GetLives());
    }

    private void OnDestroy()
    {
        if (playerCollectibles != null)
            playerCollectibles.OnLivesChanged -= UpdateHearts;
    }

    private void UpdateHearts(int currentLives)
    {
        // Destroy or create heart images to match currentLives
        while (hearts.Count > currentLives)
        {
            var last = hearts[hearts.Count - 1];
            hearts.RemoveAt(hearts.Count - 1);
            if (last != null) Destroy(last.gameObject);
        }

        if (heartPrefab == null)
        {
            // If prefab missing, nothing we can instantiate. Log and exit.
            Debug.LogError("HeartsUI: Cannot create heart images because heartPrefab is null.");
            return;
        }

        while (hearts.Count < currentLives)
        {
            // Instantiate the heart prefab and add to list. Use try/catch to avoid errors if the asset was destroyed.
            Image img = null;
            try
            {
                img = Instantiate(heartPrefab, transform);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"HeartsUI: Failed to instantiate heartPrefab - {e.Message}");
                return;
            }
            hearts.Add(img);
        }
    }
}
