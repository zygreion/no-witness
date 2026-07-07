using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject m_dialoguePanel;
    [SerializeField] private TextMeshProUGUI m_nameText;
    [SerializeField] private TextMeshProUGUI m_dialogueText;
    [SerializeField] private Image m_portraitImage;
    [SerializeField] private GameObject m_continueIndicator; // Panah berkedip atau indikator penunjuk lanjut
    [SerializeField] private Image m_backgroundImage; // Komponen UI Background kustom untuk transisi adegan

    [Header("Settings")]
    [SerializeField] private float m_typeSpeed = 0.02f; // Kecepatan efek mengetik per huruf
    [SerializeField] private AudioClip m_typingSFX;    // Suara ketikan teks dialog
    [Range(1, 5)] [SerializeField] private int m_playSFXFrequency = 2; // Bunyikan SFX setiap X karakter
    [SerializeField] private AudioClip m_continueSFX;  // Suara klik ketika lanjut ke dialog berikutnya

    private Queue<DialogueSentence> m_sentences;
    private bool m_isDialogueActive = false;
    private bool m_isTyping = false;
    private string m_currentSentenceText = "";
    private Action m_onDialogueCompleteCallback;

    public bool IsDialogueActive => m_isDialogueActive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        m_sentences = new Queue<DialogueSentence>();

        // Pastikan panel UI tersembunyi di awal game di Awake sebelum Start() memicu dialog
        if (m_dialoguePanel != null)
            m_dialoguePanel.SetActive(false);
            
        if (m_continueIndicator != null)
            m_continueIndicator.SetActive(false);

        if (m_backgroundImage != null)
            m_backgroundImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Hanya mendeteksi input keyboard ketika dialog sedang aktif berjalan
        if (m_isDialogueActive)
        {
            // Tekan Space, Enter, atau F untuk lanjut/skip
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.F))
            {
                OnContinueInputPressed();
            }
        }
    }

    /// <summary>
    /// Dipanggil saat tombol continue UI diklik, atau player menekan tombol input keyboard lanjut.
    /// </summary>
    public void OnContinueInputPressed()
    {
        if (!m_isDialogueActive) return;

        // Mainkan SFX klik continue jika diatur
        if (m_continueSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(m_continueSFX);
        }

        if (m_isTyping)
        {
            // Jika sedang mengetik, skip langsung ke akhir kalimat
            StopAllCoroutines();
            m_dialogueText.text = m_currentSentenceText;
            m_isTyping = false;
            if (m_continueIndicator != null)
                m_continueIndicator.SetActive(true);
        }
        else
        {
            // Jika sudah selesai mengetik, lanjut ke kalimat berikutnya
            DisplayNextSentence();
        }
    }

    /// <summary>
    /// Memulai rentetan dialog.
    /// </summary>
    /// <param name="dialogue">Data dialog yang akan ditampilkan.</param>
    /// <param name="onDialogueComplete">Aksi callback opsional ketika dialog selesai.</param>
    public void StartDialogue(Dialogue dialogue, Action onDialogueComplete = null)
    {
        if (m_dialoguePanel == null)
        {
            Debug.LogWarning("Dialogue Panel reference is missing in DialogueManager!");
            return;
        }

        m_isDialogueActive = true;
        m_onDialogueCompleteCallback = onDialogueComplete;

        // Kunci pergerakan player
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.LockPlayerControl(true);
        }

        m_dialoguePanel.SetActive(true);
        m_sentences.Clear();

        foreach (DialogueSentence sentence in dialogue.sentences)
        {
            m_sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    /// <summary>
    /// Menampilkan kalimat berikutnya dalam antrean.
    /// </summary>
    public void DisplayNextSentence()
    {
        if (m_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueSentence currentSentence = m_sentences.Dequeue();

        // Update teks nama pembicara
        if (m_nameText != null)
        {
            m_nameText.text = currentSentence.speakerName;
        }

        // Update gambar portrait pembicara
        if (m_portraitImage != null)
        {
            if (currentSentence.speakerPortrait != null)
            {
                m_portraitImage.gameObject.SetActive(true);
                m_portraitImage.sprite = currentSentence.speakerPortrait;
            }
            else
            {
                m_portraitImage.gameObject.SetActive(false);
            }
        }

        // Update gambar background jika kalimat ini memiliki background kustom
        if (m_backgroundImage != null)
        {
            if (currentSentence.backgroundImage != null)
            {
                m_backgroundImage.gameObject.SetActive(true);
                m_backgroundImage.sprite = currentSentence.backgroundImage;
            }
            else
            {
                m_backgroundImage.gameObject.SetActive(false);
            }
        }

        m_currentSentenceText = currentSentence.sentence;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence.sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        m_isTyping = true;
        m_dialogueText.text = "";
        if (m_continueIndicator != null)
            m_continueIndicator.SetActive(false);

        int charCount = 0;
        foreach (char letter in sentence.ToCharArray())
        {
            m_dialogueText.text += letter;

            charCount++;
            if (charCount % m_playSFXFrequency == 0 && m_typingSFX != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(m_typingSFX);
            }

            yield return new WaitForSeconds(m_typeSpeed);
        }

        m_isTyping = false;
        if (m_continueIndicator != null)
            m_continueIndicator.SetActive(true);
    }

    /// <summary>
    /// Menutup percakapan dialog secara total dan memulihkan kontrol player.
    /// </summary>
    public void EndDialogue()
    {
        m_isDialogueActive = false;
        m_isTyping = false;

        if (m_dialoguePanel != null)
            m_dialoguePanel.SetActive(false);

        if (m_continueIndicator != null)
            m_continueIndicator.SetActive(false);

        // Buka kembali kontrol player
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.LockPlayerControl(false);
        }

        // Jalankan callback transisi level / aksi lain jika ada
        m_onDialogueCompleteCallback?.Invoke();
        m_onDialogueCompleteCallback = null;
    }
}
