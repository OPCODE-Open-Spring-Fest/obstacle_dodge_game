using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component for audio settings (volume sliders and mute button).
/// Attach to a settings panel in your UI.
/// </summary>
public class AudioSettingsUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Slider for master volume (0-1)")]
    [SerializeField] private Slider masterVolumeSlider;
    
    [Tooltip("Slider for music volume (0-1)")]
    [SerializeField] private Slider musicVolumeSlider;
    
    [Tooltip("Slider for SFX volume (0-1)")]
    [SerializeField] private Slider sfxVolumeSlider;
    
    [Tooltip("Toggle button for mute")]
    [SerializeField] private Toggle muteToggle;
    
    [Header("Volume Display Text (Optional)")]
    [Tooltip("Text labels to show current volume percentages - Legacy Text")]
    [SerializeField] private Text masterVolumeText;
    [SerializeField] private Text musicVolumeText;
    [SerializeField] private Text sfxVolumeText;
    
    [Tooltip("TMP Text labels to show current volume percentages - TextMeshPro")]
    [SerializeField] private TextMeshProUGUI masterVolumeTextTMP;
    [SerializeField] private TextMeshProUGUI musicVolumeTextTMP;
    [SerializeField] private TextMeshProUGUI sfxVolumeTextTMP;

    private AudioManager audioManager;

    private void Start()
    {
        InitializeAudioManager();
    }

    private void OnEnable()
    {
        // Re-initialize when UI becomes active (in case AudioManager wasn't ready)
        InitializeAudioManager();
    }

    private void InitializeAudioManager()
    {
        if (audioManager == null)
        {
            audioManager = AudioManager.Instance;
        }

        if (audioManager == null)
        {
            Debug.LogWarning("AudioSettingsUI: AudioManager not found! Trying to find it...");
            // Try to find AudioManager in scene (works in all Unity versions)
            AudioManager managerObj = null;
            #if UNITY_2023_1_OR_NEWER
            managerObj = FindFirstObjectByType<AudioManager>();
            #else
            managerObj = FindObjectOfType<AudioManager>();
            #endif
            if (managerObj != null)
            {
                audioManager = managerObj;
            }
            else
            {
                Debug.LogError("AudioSettingsUI: AudioManager not found! Make sure AudioManager GameObject exists in the scene.");
                return;
            }
        }

        // Load current settings
        audioManager.GetVolumes(out float master, out float music, out float sfx, out bool muted);

        // Setup sliders - remove listeners first to avoid duplicate calls
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.RemoveAllListeners();
            masterVolumeSlider.value = master;
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.value = music;
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.value = sfx;
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        if (muteToggle != null)
        {
            muteToggle.onValueChanged.RemoveAllListeners();
            muteToggle.isOn = muted;
            muteToggle.onValueChanged.AddListener(OnMuteToggled);
        }

        UpdateVolumeTexts();
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (audioManager != null)
        {
            audioManager.SetMasterVolume(value);
            UpdateVolumeTexts();
        }
        else
        {
            InitializeAudioManager();
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (audioManager != null)
        {
            audioManager.SetMusicVolume(value);
            UpdateVolumeTexts();
        }
        else
        {
            InitializeAudioManager();
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (audioManager != null)
        {
            audioManager.SetSFXVolume(value);
            UpdateVolumeTexts();
        }
        else
        {
            InitializeAudioManager();
        }
    }

    private void OnMuteToggled(bool isMuted)
    {
        if (audioManager != null)
        {
            audioManager.SetMute(isMuted);
            UpdateVolumeTexts();
        }
        else
        {
            InitializeAudioManager();
        }
    }

    private void UpdateVolumeTexts()
    {
        if (audioManager != null)
        {
            audioManager.GetVolumes(out float master, out float music, out float sfx, out bool muted);
            
            string masterStr = muted ? "Muted" : $"{Mathf.RoundToInt(master * 100)}%";
            string musicStr = muted ? "Muted" : $"{Mathf.RoundToInt(music * 100)}%";
            string sfxStr = muted ? "Muted" : $"{Mathf.RoundToInt(sfx * 100)}%";

            // Legacy Text
            if (masterVolumeText != null)
            {
                masterVolumeText.text = masterStr;
            }
            if (musicVolumeText != null)
            {
                musicVolumeText.text = musicStr;
            }
            if (sfxVolumeText != null)
            {
                sfxVolumeText.text = sfxStr;
            }

            // TextMeshPro
            if (masterVolumeTextTMP != null)
            {
                masterVolumeTextTMP.text = masterStr;
            }
            if (musicVolumeTextTMP != null)
            {
                musicVolumeTextTMP.text = musicStr;
            }
            if (sfxVolumeTextTMP != null)
            {
                sfxVolumeTextTMP.text = sfxStr;
            }
        }
    }

    /// <summary>
    /// Public method to manually refresh settings (useful when panel is opened)
    /// </summary>
    public void RefreshSettings()
    {
        InitializeAudioManager();
    }
}

