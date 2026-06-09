using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider m_musicSlider;
    [SerializeField] private Slider m_sfxSlider;

    // PlayerPrefs keys
    private const string k_musicVolumeKey = "MusicVolume";
    private const string k_sfxVolumeKey = "SFXVolume";

    private void OnEnable()
    {
        // Load saved values (default to 1.0 if no save exists)
        m_musicSlider.value = PlayerPrefs.GetFloat(k_musicVolumeKey, 1.0f);
        m_sfxSlider.value = PlayerPrefs.GetFloat(k_sfxVolumeKey, 1.0f);

        // Subscribe to slider events
        m_musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        m_sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent duplicate listeners
        m_musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        m_sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }

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
