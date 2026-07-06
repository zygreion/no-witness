using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject m_mainMenuPanel;
    [SerializeField] private GameObject m_settingsPanel;

    [Header("Scene Settings")]
    [SerializeField] private string m_gameplaySceneName = "IntroDialogue";

    private void Start()
    {
        // Ensure correct panel state on start
        m_mainMenuPanel.SetActive(true);
        m_settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Loads the gameplay scene. Called by the New Game button.
    /// </summary>
    public void OnNewGameClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        SceneManager.LoadScene(m_gameplaySceneName);
    }

    /// <summary>
    /// Placeholder for future save/load system. Called by the Continue button.
    /// </summary>
    public void OnContinueClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        Debug.Log("Continue: Save/Load system not yet implemented.");
    }

    /// <summary>
    /// Shows the Settings panel and hides the main menu. Called by the Settings button.
    /// </summary>
    public void OnSettingsClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        m_mainMenuPanel.SetActive(false);
        m_settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Returns from Settings to the main menu. Called by the Back button.
    /// </summary>
    public void OnBackToMenuClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
        m_settingsPanel.SetActive(false);
        m_mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Quits the application. Called by the Exit button.
    /// </summary>
    public void OnExitClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
