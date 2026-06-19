using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPanelManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject m_deathPanel;

    [Header("Settings")]
    [SerializeField] private float m_displayDelay = 1.5f; // Delay to allow death animation to play

    [Header("Audio")]
    [SerializeField] private AudioClip m_deathSFX;
    [SerializeField] private AudioClip m_deathBGM;

    private void Start()
    {
        // Ensure the death panel is hidden on start
        if (m_deathPanel != null)
        {
            m_deathPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // Subscribe to player death event
        HealthPlayer.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        HealthPlayer.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        // Play death SFX
        if (AudioManager.Instance != null && m_deathSFX != null)
        {
            AudioManager.Instance.PlaySFX(m_deathSFX);
        }

        // Play death BGM
        if (AudioManager.Instance != null && m_deathBGM != null)
        {
            AudioManager.Instance.PlayMusic(m_deathBGM);
        }

        // Disable standard PauseManager and hide the pause button
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            if (pauseManager.PauseButton != null)
            {
                pauseManager.PauseButton.SetActive(false);
            }
            pauseManager.enabled = false;
        }

        // Start delay to show death panel
        StartCoroutine(ShowDeathPanelCoroutine());
    }

    private IEnumerator ShowDeathPanelCoroutine()
    {
        // Wait for the animation to play
        yield return new WaitForSeconds(m_displayDelay);

        // Show the panel
        if (m_deathPanel != null)
        {
            m_deathPanel.SetActive(true);
        }

        // Freeze time scale
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Restart the current active scene.
    /// </summary>
    public void RestartGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        Time.timeScale = 1f; // IMPORTANT: Restore time scale!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Return to the Main Menu scene.
    /// </summary>
    public void BackToMainMenu()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        Time.timeScale = 1f; // IMPORTANT: Restore time scale!
        SceneManager.LoadScene("Main-Menu");
    }
}
