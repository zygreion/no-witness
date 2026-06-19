using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject m_pauseMenuPanel;
    [SerializeField] private GameObject m_settingsPanel;
    [SerializeField] private GameObject m_pauseButton; // The HUD pause button

    private bool m_isPaused = false;

    // Public getter to allow DeathPanelManager to hide the pause button dynamically
    public GameObject PauseButton => m_pauseButton;

    private void Start()
    {
        // Ensure panels are in correct states on start
        m_pauseMenuPanel.SetActive(false);
        m_settingsPanel.SetActive(false);
        if (m_pauseButton != null)
        {
            m_pauseButton.SetActive(true);
        }
    }

    private void Update()
    {
        // Allow pressing Escape to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_isPaused)
            {
                if (m_settingsPanel != null && m_settingsPanel.activeSelf)
                {
                    // If in settings, go back to pause menu
                    OnSettingsBackClicked();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        m_isPaused = true;
        m_pauseMenuPanel.SetActive(true);
        if (m_settingsPanel != null)
        {
            m_settingsPanel.SetActive(false);
        }
        if (m_pauseButton != null)
        {
            m_pauseButton.SetActive(false);
        }
        Time.timeScale = 0f; // Freeze the game physics and animations
    }

    public void ResumeGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        m_isPaused = false;
        m_pauseMenuPanel.SetActive(false);
        if (m_settingsPanel != null)
        {
            m_settingsPanel.SetActive(false);
        }
        if (m_pauseButton != null)
        {
            m_pauseButton.SetActive(true);
        }
        Time.timeScale = 1f; // Resume the game physics and animations
    }

    public void OnSettingsClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        m_pauseMenuPanel.SetActive(false);
        if (m_settingsPanel != null)
        {
            m_settingsPanel.SetActive(true);
        }
    }

    public void OnSettingsBackClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        if (m_settingsPanel != null)
        {
            m_settingsPanel.SetActive(false);
        }
        m_pauseMenuPanel.SetActive(true);
    }

    public void OnBackToMenuClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        Time.timeScale = 1f; // IMPORTANT: Always restore time scale before changing scenes!
        SceneManager.LoadScene("Main-Menu");
    }
}
