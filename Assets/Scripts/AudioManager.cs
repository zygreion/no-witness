using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource m_musicSource;
    [SerializeField] private AudioSource m_sfxSource;

    [Header("Default Audio Clips")]
    [SerializeField] private AudioClip m_defaultClickSound;
    [SerializeField] private AudioClip m_menuBGM;     // Music for Main Menu
    [SerializeField] private AudioClip m_dungeon1BGM; // Music for Dungeon 1
    [SerializeField] private AudioClip m_dungeon2BGM; // Music for Dungeon 2

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

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Main-Menu":
                PlayMusic(m_menuBGM);
                break;
            case "Dungeon 1":
                PlayMusic(m_dungeon1BGM);
                break;
            case "Dungeon 2":
                PlayMusic(m_dungeon2BGM);
                break;
            default:
                StopMusic();
                break;
        }
    }

    private void Start()
    {
        // Play correct BGM if game starts directly in one of these scenes
        switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            case "Main-Menu":
                PlayMusic(m_menuBGM);
                break;
            case "Dungeon 1":
                PlayMusic(m_dungeon1BGM);
                break;
            case "Dungeon 2":
                PlayMusic(m_dungeon2BGM);
                break;
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
