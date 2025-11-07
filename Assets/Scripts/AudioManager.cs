using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Centralized audio manager for background music and sound effects.
/// Persists across scenes using DontDestroyOnLoad.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [Tooltip("AudioSource for background music (loop enabled)")]
    [SerializeField] private AudioSource musicSource;
    
    [Tooltip("AudioSource for sound effects (one-shot)")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Background Music")]
    [Tooltip("Music clip for main menu")]
    [SerializeField] private AudioClip menuMusic;
    
    [Tooltip("Music clip for gameplay scenes")]
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Sound Effects")]
    [Tooltip("Sound effect when player collides with obstacle")]
    [SerializeField] private AudioClip collisionSFX;
    
    [Tooltip("Sound effect when collecting a heart")]
    [SerializeField] private AudioClip heartCollectSFX;
    
    [Tooltip("Sound effect when collecting a gem")]
    [SerializeField] private AudioClip gemCollectSFX;
    
    [Tooltip("Sound effect when scoring points")]
    [SerializeField] private AudioClip scoreSFX;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [Tooltip("Master volume (affects all audio)")]
    [SerializeField] private float masterVolume = 1f;
    
    [Range(0f, 1f)]
    [Tooltip("Music volume")]
    [SerializeField] private float musicVolume = 0.7f;
    
    [Range(0f, 1f)]
    [Tooltip("SFX volume")]
    [SerializeField] private float sfxVolume = 0.8f;

    [Header("Settings")]
    [Tooltip("Persist audio settings in PlayerPrefs")]
    [SerializeField] private bool usePlayerPrefs = true;
    
    private const string PREFS_MASTER_VOLUME = "MasterVolume";
    private const string PREFS_MUSIC_VOLUME = "MusicVolume";
    private const string PREFS_SFX_VOLUME = "SFXVolume";
    private const string PREFS_MUTE = "Mute";

    private bool isMuted = false;

    private void Awake()
    {
        // Singleton pattern - only one AudioManager instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        // Create music source if not assigned
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        // Create SFX source if not assigned
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        ApplyVolumes();
    }

    #region Background Music

    /// <summary>
    /// Play menu background music
    /// </summary>
    public void PlayMenuMusic()
    {
        if (menuMusic != null)
        {
            PlayMusic(menuMusic);
        }
    }

    /// <summary>
    /// Play gameplay background music
    /// </summary>
    public void PlayGameplayMusic()
    {
        if (gameplayMusic != null)
        {
            PlayMusic(gameplayMusic);
        }
    }

    /// <summary>
    /// Play a specific music clip
    /// </summary>
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        
        if (musicSource.clip != clip || !musicSource.isPlaying)
        {
            musicSource.clip = clip;
            // Apply volumes before playing to ensure correct volume
            ApplyVolumes();
            // Only attempt to play if not muted and source is enabled/active
            if (!isMuted && musicSource.enabled && musicSource.gameObject.activeInHierarchy)
            {
                // If previously paused, UnPause; otherwise Play
                if (!musicSource.isPlaying)
                {
                    musicSource.UnPause();
                    if (!musicSource.isPlaying)
                    {
                        musicSource.Play();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Stop background music
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    #endregion

    #region Sound Effects

    /// <summary>
    /// Play collision sound effect
    /// </summary>
    public void PlayCollisionSFX()
    {
        PlaySFX(collisionSFX);
    }

    /// <summary>
    /// Play heart collect sound effect
    /// </summary>
    public void PlayHeartCollectSFX()
    {
        PlaySFX(heartCollectSFX);
    }

    /// <summary>
    /// Play gem collect sound effect
    /// </summary>
    public void PlayGemCollectSFX()
    {
        PlaySFX(gemCollectSFX);
    }

    /// <summary>
    /// Play score sound effect
    /// </summary>
    public void PlayScoreSFX()
    {
        PlaySFX(scoreSFX);
    }

    /// <summary>
    /// Play any sound effect clip
    /// </summary>
    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null) return;
        
        // Don't play if muted, but still respect volume settings
        float finalVolume = isMuted ? 0f : volumeScale;
        sfxSource.PlayOneShot(clip, finalVolume);
    }

    #endregion

    #region Volume Controls

    /// <summary>
    /// Set master volume (0-1)
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        if (usePlayerPrefs) PlayerPrefs.SetFloat(PREFS_MASTER_VOLUME, masterVolume);
    }

    /// <summary>
    /// Set music volume (0-1)
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        // If music volume is zero, pause music; otherwise, ensure it is playing
        if (musicSource != null)
        {
            if (musicVolume <= 0f || isMuted)
            {
                if (musicSource.isPlaying) musicSource.Pause();
            }
            else if (musicSource.enabled && musicSource.gameObject.activeInHierarchy)
            {
                if (musicSource.clip != null && !musicSource.isPlaying)
                {
                    musicSource.UnPause();
                    // If UnPause didn't resume (e.g., never played), Play safely
                    if (!musicSource.isPlaying)
                    {
                        musicSource.Play();
                    }
                }
            }
        }
        if (usePlayerPrefs) PlayerPrefs.SetFloat(PREFS_MUSIC_VOLUME, musicVolume);
    }

    /// <summary>
    /// Set SFX volume (0-1)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        if (usePlayerPrefs) PlayerPrefs.SetFloat(PREFS_SFX_VOLUME, sfxVolume);
    }

    /// <summary>
    /// Toggle mute all audio
    /// </summary>
    public void ToggleMute()
    {
        isMuted = !isMuted;
        ApplyVolumes();
        if (usePlayerPrefs) PlayerPrefs.SetInt(PREFS_MUTE, isMuted ? 1 : 0);
    }

    /// <summary>
    /// Set mute state
    /// </summary>
    public void SetMute(bool muted)
    {
        isMuted = muted;
        ApplyVolumes();
        if (usePlayerPrefs) PlayerPrefs.SetInt(PREFS_MUTE, isMuted ? 1 : 0);
    }

    /// <summary>
    /// Get current volume settings
    /// </summary>
    public void GetVolumes(out float master, out float music, out float sfx, out bool muted)
    {
        master = masterVolume;
        music = musicVolume;
        sfx = sfxVolume;
        muted = isMuted;
    }

    private void ApplyVolumes()
    {
        float actualMusicVolume = isMuted ? 0f : (musicVolume * masterVolume);
        float actualSFXVolume = isMuted ? 0f : (sfxVolume * masterVolume);

        if (musicSource != null)
        {
            musicSource.volume = actualMusicVolume;
            // If muted and music is playing, pause it (optional - can also just set volume to 0)
            if (isMuted && musicSource.isPlaying)
            {
                musicSource.Pause();
            }
            else if (!isMuted && musicSource.clip != null && !musicSource.isPlaying)
            {
                musicSource.UnPause();
            }
        }

        if (sfxSource != null)
        {
            sfxSource.volume = actualSFXVolume;
        }

        // Also apply to Unity's AudioListener for global mute
        AudioListener.volume = isMuted ? 0f : masterVolume;
    }

    private void LoadSettings()
    {
        if (usePlayerPrefs)
        {
            masterVolume = PlayerPrefs.GetFloat(PREFS_MASTER_VOLUME, 1f);
            musicVolume = PlayerPrefs.GetFloat(PREFS_MUSIC_VOLUME, 0.7f);
            sfxVolume = PlayerPrefs.GetFloat(PREFS_SFX_VOLUME, 0.8f);
            isMuted = PlayerPrefs.GetInt(PREFS_MUTE, 0) == 1;
        }
        ApplyVolumes();
    }

    public void SaveSettings()
    {
        if (usePlayerPrefs)
        {
            PlayerPrefs.SetFloat(PREFS_MASTER_VOLUME, masterVolume);
            PlayerPrefs.SetFloat(PREFS_MUSIC_VOLUME, musicVolume);
            PlayerPrefs.SetFloat(PREFS_SFX_VOLUME, sfxVolume);
            PlayerPrefs.SetInt(PREFS_MUTE, isMuted ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    #endregion
}

