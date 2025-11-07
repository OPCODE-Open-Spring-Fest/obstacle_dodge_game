using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script to toggle a settings panel on/off using CanvasGroup.
/// Attach this to a button that opens/closes the settings panel.
/// </summary>
public class SettingsPanelToggle : MonoBehaviour
{
    [Header("Settings Panel")]
    [Tooltip("The settings panel GameObject that contains the AudioSettingsUI")]
    [SerializeField] private GameObject settingsPanel;
    
    [Tooltip("CanvasGroup component on the settings panel (for alpha/raycast control)")]
    [SerializeField] private CanvasGroup settingsCanvasGroup;
    
    [Tooltip("Button that toggles the settings panel")]
    [SerializeField] private Button toggleButton;
    
    [Header("Animation Settings")]
    [Tooltip("Smooth fade in/out duration (0 = instant)")]
    [SerializeField] private float fadeDuration = 0.2f;
    
    private bool isPanelOpen = false;
    private AudioSettingsUI audioSettingsUI;
    private bool isToggling = false; // Prevent rapid toggling

    private void Start()
    {
        // Find AudioSettingsUI component on the panel
        if (settingsPanel != null)
        {
            audioSettingsUI = settingsPanel.GetComponent<AudioSettingsUI>();
            if (audioSettingsUI == null)
            {
                audioSettingsUI = settingsPanel.GetComponentInChildren<AudioSettingsUI>();
            }
        }

        // Get CanvasGroup if not assigned
        if (settingsCanvasGroup == null && settingsPanel != null)
        {
            settingsCanvasGroup = settingsPanel.GetComponent<CanvasGroup>();
        }

        // Setup button click listener - REMOVE ALL FIRST to prevent duplicates
        if (toggleButton == null)
        {
            // Try to get button component from this GameObject
            toggleButton = GetComponent<Button>();
        }
        
        if (toggleButton != null)
        {
            // Remove any existing listeners to prevent duplicates
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(ToggleSettingsPanel);
        }

        // Initially hide the panel (but keep it active if using CanvasGroup)
        if (settingsPanel != null)
        {
            isPanelOpen = false;
            SetPanelVisibility(false);
        }
    }

    /// <summary>
    /// Toggle the settings panel on/off
    /// </summary>
    public void ToggleSettingsPanel()
    {
        // Prevent rapid clicking
        if (isToggling) return;
        
        isToggling = true;
        isPanelOpen = !isPanelOpen;
        SetPanelVisibility(isPanelOpen);
        
        // Reset toggle flag after a short delay
        Invoke(nameof(ResetToggleFlag), 0.1f);
    }

    private void ResetToggleFlag()
    {
        isToggling = false;
    }

    /// <summary>
    /// Open the settings panel
    /// </summary>
    public void OpenSettingsPanel()
    {
        isPanelOpen = true;
        SetPanelVisibility(true);
    }

    /// <summary>
    /// Close the settings panel
    /// </summary>
    public void CloseSettingsPanel()
    {
        isPanelOpen = false;
        SetPanelVisibility(false);
    }

    private void SetPanelVisibility(bool visible)
    {
        if (settingsPanel == null) return;

        // Update state flag
        isPanelOpen = visible;

        // If using CanvasGroup, keep GameObject active and control via CanvasGroup
        // Otherwise, use SetActive
        if (settingsCanvasGroup != null)
        {
            // Keep GameObject active, control visibility via CanvasGroup
            settingsPanel.SetActive(true);
            
            // Stop any running fade coroutines to prevent conflicts
            StopAllCoroutines();
            
            if (fadeDuration > 0f && visible)
            {
                // Smooth fade in
                StartCoroutine(FadeCanvasGroup(settingsCanvasGroup, settingsCanvasGroup.alpha, 1f, fadeDuration));
            }
            else if (fadeDuration > 0f && !visible)
            {
                // Smooth fade out
                StartCoroutine(FadeCanvasGroup(settingsCanvasGroup, settingsCanvasGroup.alpha, 0f, fadeDuration));
            }
            else
            {
                settingsCanvasGroup.alpha = visible ? 1f : 0f;
                // IMMEDIATELY set interactivity when not using fade
                settingsCanvasGroup.interactable = visible;
                settingsCanvasGroup.blocksRaycasts = visible;
            }
        }
        else
        {
            // No CanvasGroup - use SetActive instead
            settingsPanel.SetActive(visible);
        }

        // Force AudioSettingsUI to refresh when panel becomes visible
        if (visible)
        {
            // Find AudioSettingsUI component
            if (audioSettingsUI == null)
            {
                audioSettingsUI = settingsPanel.GetComponent<AudioSettingsUI>();
                if (audioSettingsUI == null)
                {
                    audioSettingsUI = settingsPanel.GetComponentInChildren<AudioSettingsUI>();
                }
            }
            
            if (audioSettingsUI != null)
            {
                // Call refresh method to re-initialize with current AudioManager
                audioSettingsUI.RefreshSettings();
            }
        }
    }

    private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        
        // Set interactivity immediately if fading in
        if (endAlpha > startAlpha)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        group.alpha = endAlpha;
        
        // Set interactivity for fade out
        if (endAlpha < startAlpha)
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }

    private void OnDisable()
    {
        // Clean up when disabled
        CancelInvoke();
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        // Remove listener and cleanup
        CancelInvoke();
        StopAllCoroutines();
        
        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveListener(ToggleSettingsPanel);
        }
    }
}

