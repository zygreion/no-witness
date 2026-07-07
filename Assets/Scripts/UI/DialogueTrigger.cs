using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        Interact,      // Membutuhkan player mendekat dan menekan tombol untuk berbicara
        AutoStart,     // Berjalan otomatis segera setelah scene dimuat
        TriggerOnEnter // Berjalan otomatis saat player menyentuh/masuk ke area trigger collider
    }

    [Header("Trigger Mode")]
    [SerializeField] private TriggerType m_triggerType = TriggerType.Interact;

    [Header("Dialogue Content")]
    [SerializeField] private Dialogue m_dialogue;

    [Header("Interact Settings")]
    [SerializeField] private GameObject m_interactPrompt; // Clue visual (misal text: "Tekan F")
    [SerializeField] private KeyCode m_interactKey = KeyCode.F; // Tombol interaksi default

    [Header("Events")]
    public UnityEvent OnDialogueComplete; // Dipicu ketika dialog selesai

    private bool m_isPlayerInRange = false;
    private bool m_hasAutoTriggered = false;

    private void Start()
    {
        // Sembunyikan prompt visual di awal
        if (m_interactPrompt != null)
        {
            m_interactPrompt.SetActive(false);
        }

        // Jika diset AutoStart, langsung jalankan dialog saat scene dimulai
        if (m_triggerType == TriggerType.AutoStart)
        {
            TriggerDialogue();
        }
    }

    private void Update()
    {
        if (m_triggerType == TriggerType.Interact && m_isPlayerInRange)
        {
            // Periksa jika player menekan tombol interaksi dan dialog sedang tidak aktif
            if (Input.GetKeyDown(m_interactKey) && (DialogueManager.Instance == null || !DialogueManager.Instance.IsDialogueActive))
            {
                if (m_interactPrompt != null)
                {
                    m_interactPrompt.SetActive(false);
                }
                TriggerDialogue();
            }
        }
    }

    /// <summary>
    /// Memulai dialog.
    /// </summary>
    public void TriggerDialogue()
    {
        if ((m_triggerType == TriggerType.AutoStart || m_triggerType == TriggerType.TriggerOnEnter) && m_hasAutoTriggered) return;
        m_hasAutoTriggered = true;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(m_dialogue, () =>
            {
                // Panggil UnityEvent setelah percakapan selesai
                OnDialogueComplete?.Invoke();

                // Jika player masih di jangkauan area interaksi, tampilkan petunjuk tombol kembali
                if (m_triggerType == TriggerType.Interact && m_isPlayerInRange && m_interactPrompt != null)
                {
                    m_interactPrompt.SetActive(true);
                }
            });
        }
        else
        {
            Debug.LogWarning("DialogueManager instance tidak ditemukan di Scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (m_triggerType == TriggerType.Interact)
            {
                m_isPlayerInRange = true;
                if (m_interactPrompt != null && (DialogueManager.Instance == null || !DialogueManager.Instance.IsDialogueActive))
                {
                    m_interactPrompt.SetActive(true);
                }
            }
            else if (m_triggerType == TriggerType.TriggerOnEnter)
            {
                TriggerDialogue();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (m_triggerType == TriggerType.Interact && other.CompareTag("Player"))
        {
            m_isPlayerInRange = false;
            if (m_interactPrompt != null)
            {
                m_interactPrompt.SetActive(false);
            }
        }
    }
}

