using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialPhase
    {
        IntroDialogue,   // Dialog awal Elder Kenji
        Wave1Combat,     // Mengalahkan 2 skeleton awal
        MidDialogue,     // Dialog Alaric "Ini terlalu mudah" & Kenji "Jangan lengah..."
        Wave2Combat,     // Mengalahkan 2 skeleton + 4 flying eye di ujung dungeon
        OutroDialogue,   // Dialog akhir kelulusan
        PortalActive     // Portal keluar terbuka/aktif menuju Dungeon 1
    }

    [Header("Current Status")]
    [SerializeField] private TutorialPhase m_currentPhase = TutorialPhase.IntroDialogue;

    [Header("Wave 1 Enemies (Initial)")]
    [SerializeField] private List<GameObject> m_wave1Enemies = new List<GameObject>();

    [Header("Wave 2 Enemies (Spawned)")]
    [SerializeField] private List<GameObject> m_wave2Enemies = new List<GameObject>();

    [Header("Dialogue Triggers")]
    [SerializeField] private DialogueTrigger m_introDialogueTrigger;
    [SerializeField] private DialogueTrigger m_midDialogueTrigger;
    [SerializeField] private DialogueTrigger m_outroDialogueTrigger;

    [Header("Exit settings")]
    [SerializeField] private GameObject m_exitPortal; // GameObject portal atau trigger keluar

    private void Awake()
    {
        // 1. Matikan AI untuk Wave 1 di awal agar diam saat dialog pertama
        foreach (var enemy in m_wave1Enemies)
        {
            SetEnemyAIActive(enemy, false);
        }

        // 2. Sembunyikan Wave 2 musuh sepenuhnya di awal
        foreach (var enemy in m_wave2Enemies)
        {
            if (enemy != null)
            {
                enemy.SetActive(false);
            }
        }

        // 3. Matikan portal keluar di awal
        if (m_exitPortal != null)
        {
            m_exitPortal.SetActive(false);
        }

        // 4. Daftarkan event callback untuk setiap dialog trigger
        if (m_introDialogueTrigger != null)
        {
            m_introDialogueTrigger.OnDialogueComplete.AddListener(OnIntroDialogueComplete);
        }

        if (m_midDialogueTrigger != null)
        {
            m_midDialogueTrigger.OnDialogueComplete.AddListener(OnMidDialogueComplete);
        }

        if (m_outroDialogueTrigger != null)
        {
            m_outroDialogueTrigger.OnDialogueComplete.AddListener(OnOutroDialogueComplete);
        }
    }

    private void Start()
    {
        // Pemicu otomatis untuk dialog intro di awal game
        if (m_introDialogueTrigger != null)
        {
            m_introDialogueTrigger.TriggerDialogue();
        }
    }

    private void Update()
    {
        switch (m_currentPhase)
        {
            case TutorialPhase.Wave1Combat:
                // Cek apakah Wave 1 sudah habis
                if (IsWaveDefeated(m_wave1Enemies))
                {
                    m_currentPhase = TutorialPhase.MidDialogue;
                    TriggerMidDialogue();
                }
                break;

            case TutorialPhase.Wave2Combat:
                // Cek apakah Wave 2 sudah habis
                if (IsWaveDefeated(m_wave2Enemies))
                {
                    m_currentPhase = TutorialPhase.OutroDialogue;
                    TriggerOutroDialogue();
                }
                break;
        }
    }

    // --- Wave 1 Handlers ---
    private void OnIntroDialogueComplete()
    {
        if (m_currentPhase == TutorialPhase.IntroDialogue)
        {
            m_currentPhase = TutorialPhase.Wave1Combat;
            
            // Aktifkan AI untuk Wave 1
            foreach (var enemy in m_wave1Enemies)
            {
                SetEnemyAIActive(enemy, true);
            }
            Debug.Log("[TutorialManager] Intro Dialogue selesai. Mulai Wave 1 Combat!");
        }
    }

    // --- Mid Dialogue Handlers ---
    private void TriggerMidDialogue()
    {
        Debug.Log("[TutorialManager] Wave 1 kalah. Memicu Mid-Dialogue...");
        if (m_midDialogueTrigger != null)
        {
            // Panggil dialog tengah secara langsung
            m_midDialogueTrigger.TriggerDialogue();
        }
        else
        {
            // Fallback jika dialog trigger tidak ada
            OnMidDialogueComplete();
        }
    }

    private void OnMidDialogueComplete()
    {
        if (m_currentPhase == TutorialPhase.MidDialogue)
        {
            m_currentPhase = TutorialPhase.Wave2Combat;

            // Aktifkan musuh-musuh Wave 2
            foreach (var enemy in m_wave2Enemies)
            {
                if (enemy != null)
                {
                    enemy.SetActive(true);
                    SetEnemyAIActive(enemy, true);
                }
            }
            Debug.Log("[TutorialManager] Mid-Dialogue selesai. Mulai Wave 2 Combat!");
        }
    }

    // --- Outro Dialogue Handlers ---
    private void TriggerOutroDialogue()
    {
        Debug.Log("[TutorialManager] Wave 2 kalah. Memicu Outro Dialogue...");
        if (m_outroDialogueTrigger != null)
        {
            m_outroDialogueTrigger.TriggerDialogue();
        }
        else
        {
            // Fallback jika dialog trigger tidak ada
            OnOutroDialogueComplete();
        }
    }

    private void OnOutroDialogueComplete()
    {
        if (m_currentPhase == TutorialPhase.OutroDialogue)
        {
            m_currentPhase = TutorialPhase.PortalActive;

            // Aktifkan portal / pintu keluar
            if (m_exitPortal != null)
            {
                m_exitPortal.SetActive(true);
            }
            Debug.Log("[TutorialManager] Outro Dialogue selesai. Portal keluar diaktifkan!");
        }
    }

    // --- Helper Methods ---
    private bool IsWaveDefeated(List<GameObject> enemies)
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                if (enemy.TryGetComponent<EnemyHealth>(out var health))
                {
                    if (!health.IsDead)
                        return false;
                }
                else
                {
                    // Masih aktif dan tidak memiliki komponen kesehatan (asumsi masih hidup)
                    return false;
                }
            }
        }
        return true;
    }

    private void SetEnemyAIActive(GameObject enemy, bool active)
    {
        if (enemy == null) return;

        // 1. Skeleton Roaming script
        if (enemy.TryGetComponent<Skeleton_roaming>(out var skeletonRoam))
        {
            skeletonRoam.enabled = active;
        }

        // 2. Flying Eye Roaming script (class name: flyingEye_roaming)
        if (enemy.TryGetComponent<flyingEye_roaming>(out var flyingEyeRoam))
        {
            flyingEyeRoam.enabled = active;
        }

        // 3. Rigidbody2D handling untuk mencegah musuh bergeser/didorong saat diam
        if (enemy.TryGetComponent<Rigidbody2D>(out var rb))
        {
            if (!active)
            {
                rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
}
