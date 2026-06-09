using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource m_musicSource;
    [SerializeField] private AudioSource m_sfxSource;

    [Header("Default Audio Clips")]
    [SerializeField] private AudioClip m_defaultClickSound;
    [SerializeField] private AudioClip m_menuBGM; // Default music for Main Menu / Pause Menu

    private const string k_musicVolumeKey = "MusicVolume";
    private const string k_sfxVolumeKey = "SFXVolume";

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudio()
    {
        // Load saved volumes
        float musicVol = PlayerPrefs.GetFloat(k_musicVolumeKey, 1.0f);
        float sfxVol = PlayerPrefs.GetFloat(k_sfxVolumeKey, 1.0f);

        if (m_musicSource != null)
        {
            m_musicSource.volume = musicVol;
            m_musicSource.loop = true;
        }

        if (m_sfxSource != null)
        {
            m_sfxSource.volume = sfxVol;
        }
    }

    private void Start()
    {
        // Automatically start playing menu BGM if assigned
        if (m_menuBGM != null)
        {
            PlayMusic(m_menuBGM);
        }
    }

    // --- Public Audio Controls ---

    public void PlayMusic(AudioClip clip)
    {
        if (m_musicSource == null || clip == null) return;

        // Don't restart the clip if it's already playing
        if (m_musicSource.clip == clip && m_musicSource.isPlaying) return;

        m_musicSource.clip = clip;
        m_musicSource.Play();
    }

    public void StopMusic()
    {
        if (m_musicSource != null)
        {
            m_musicSource.Stop();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (m_sfxSource == null || clip == null) return;
        m_sfxSource.PlayOneShot(clip);
    }

    public void PlayClickSound()
    {
        if (m_defaultClickSound != null)
        {
            PlaySFX(m_defaultClickSound);
        }
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(k_musicVolumeKey, volume);
        PlayerPrefs.Save();

        if (m_musicSource != null)
        {
            m_musicSource.volume = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat(k_sfxVolumeKey, volume);
        PlayerPrefs.Save();

        if (m_sfxSource != null)
        {
            m_sfxSource.volume = volume;
        }
    }
}
