using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("Sub-Panels")]
    [SerializeField] private GameObject m_mainSettingsMenu;
    [SerializeField] private GameObject m_audioPanel;
    [SerializeField] private GameObject m_inputsPanel;

    [Header("Audio Sliders")]
    [SerializeField] private Slider m_musicSlider;
    [SerializeField] private Slider m_sfxSlider;

    // PlayerPrefs keys
    private const string k_musicVolumeKey = "MusicVolume";
    private const string k_sfxVolumeKey = "SFXVolume";

    private void OnEnable()
    {
        // Default: Show main settings menu, hide sub-panels (do not play click sound on enable)
        ShowMainSettingsMenu(false);

        // Load saved values (default to 1.0 if no save exists)
        if (m_musicSlider != null)
        {
            m_musicSlider.value = PlayerPrefs.GetFloat(k_musicVolumeKey, 1.0f);
            m_musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (m_sfxSlider != null)
        {
            m_sfxSlider.value = PlayerPrefs.GetFloat(k_sfxVolumeKey, 1.0f);
            m_sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent duplicate listeners
        if (m_musicSlider != null)
        {
            m_musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        }

        if (m_sfxSlider != null)
        {
            m_sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }
    }

    // --- Navigation Functions ---

    public void ShowMainSettingsMenu()
    {
        ShowMainSettingsMenu(true);
    }

    public void ShowMainSettingsMenu(bool playSound)
    {
        if (playSound && AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        if (m_mainSettingsMenu != null) m_mainSettingsMenu.SetActive(true);
        if (m_audioPanel != null) m_audioPanel.SetActive(false);
        if (m_inputsPanel != null) m_inputsPanel.SetActive(false);
    }

    public void OpenAudioPanel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        if (m_mainSettingsMenu != null) m_mainSettingsMenu.SetActive(false);
        if (m_audioPanel != null) m_audioPanel.SetActive(true);
        if (m_inputsPanel != null) m_inputsPanel.SetActive(false);
    }

    public void OpenInputsPanel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        if (m_mainSettingsMenu != null) m_mainSettingsMenu.SetActive(false);
        if (m_audioPanel != null) m_audioPanel.SetActive(false);
        if (m_inputsPanel != null) m_inputsPanel.SetActive(true);
    }

    // --- Volume Callbacks ---

    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(k_musicVolumeKey, value);
        PlayerPrefs.Save();
        Debug.Log($"Music Volume: {value:F2}");
    }

    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(k_sfxVolumeKey, value);
        PlayerPrefs.Save();
        Debug.Log($"SFX Volume: {value:F2}");
    }
}
