using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private string m_targetSceneName = "Dungeon 1";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[SceneTransitionTrigger] Player memasuki trigger. Memuat scene: " + m_targetSceneName);
            
            // Pastikan time scale berjalan normal (tidak freeze) saat berganti scene
            Time.timeScale = 1f;
            SceneManager.LoadScene(m_targetSceneName);
        }
    }
}
