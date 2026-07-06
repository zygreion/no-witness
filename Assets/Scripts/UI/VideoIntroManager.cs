using UnityEngine;
using UnityEngine.Video;

public class VideoIntroManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer m_videoPlayer;
    [SerializeField] private GameObject m_dialogueCanvasGroup; // DialoguePanel atau Canvas yang disembunyikan saat video main
    [SerializeField] private DialogueTrigger m_dialogueTrigger; // DialogueTrigger yang akan dipicu setelah video selesai
    [SerializeField] private AudioClip m_introBGM; // BGM kustom yang dimainkan setelah video selesai
    [SerializeField] private GameObject m_pauseCanvas; // PauseCanvas yang disembunyikan saat video main, aktif saat dialog mulai

    [Header("Settings")]
    [SerializeField] private bool m_allowSkip = true; // Apakah video boleh di-skip
    [SerializeField] private KeyCode m_skipKey = KeyCode.Escape; // Tombol skip (default Escape)

    private bool m_hasFinished = false;

    private void Start()
    {
        if (m_videoPlayer == null)
        {
            m_videoPlayer = GetComponent<VideoPlayer>();
        }

        // Sembunyikan UI dialog selama video diputar
        if (m_dialogueCanvasGroup != null)
        {
            m_dialogueCanvasGroup.SetActive(false);
        }

        if (m_pauseCanvas != null)
        {
            m_pauseCanvas.SetActive(false);
        }

        if (m_videoPlayer != null)
        {
            // Daftarkan event callback ketika video selesai diputar secara alami
            m_videoPlayer.loopPointReached += OnVideoFinished;
            m_videoPlayer.Play();
        }
        else
        {
            // Jika VideoPlayer tidak ditemukan, langsung buka dialog
            FinishIntro();
        }
    }

    private void Update()
    {
        // Fitur Skip Video
        if (m_allowSkip && !m_hasFinished && m_videoPlayer != null && m_videoPlayer.isPlaying)
        {
            // Deteksi jika menekan tombol Escape atau Space untuk skip
            if (Input.GetKeyDown(m_skipKey) || Input.GetKeyDown(KeyCode.Space))
            {
                m_videoPlayer.Stop();
                FinishIntro();
            }
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        FinishIntro();
    }

    private void FinishIntro()
    {
        if (m_hasFinished) return;
        m_hasFinished = true;

        // Unsubscribe event untuk keamanan memori
        if (m_videoPlayer != null)
        {
            m_videoPlayer.loopPointReached -= OnVideoFinished;
            m_videoPlayer.gameObject.SetActive(false); // Matikan/sembunyikan VideoPlayer
        }

        // Mainkan BGM intro setelah video selesai
        if (AudioManager.Instance != null && m_introBGM != null)
        {
            AudioManager.Instance.PlayMusic(m_introBGM);
        }

        // Munculkan kembali UI dialog
        if (m_dialogueCanvasGroup != null)
        {
            m_dialogueCanvasGroup.SetActive(true);
        }

        if (m_pauseCanvas != null)
        {
            m_pauseCanvas.SetActive(true);
        }

        // Picu jalannya dialog
        if (m_dialogueTrigger != null)
        {
            m_dialogueTrigger.TriggerDialogue();
        }
    }
}
