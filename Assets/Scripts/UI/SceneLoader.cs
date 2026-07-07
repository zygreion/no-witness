using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Memuat scene berdasarkan nama scene yang diberikan.
    /// Sangat berguna untuk dipanggil lewat UnityEvent (seperti OnDialogueComplete).
    /// </summary>
    /// <param name="sceneName">Nama scene yang ingin dimuat.</param>
    public void LoadScene(string sceneName)
    {
        // Pastikan time scale berjalan normal (tidak freeze) saat berganti scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
