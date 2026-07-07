using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon1Manager : MonoBehaviour
{
    public enum DungeonPhase
    {
        IntroMonologue,      // Dialog 1: awal Alaric
        BuffSelection,       // Memilih kartu buff
        MidMonologue,        // Dialog 2: Alaric setelah buff terpasang
        Wave1Combat,         // Pertarungan Wave 1 (Goblin & Mushroom atas)
        Wave2Combat,         // Pertarungan Wave 2 (Skeleton & Goblin lorong)
        MinibossEncounter,   // Dialog 3: Percakapan Alaric dengan Miniboss Malakor
        Wave3Miniboss,       // Pertarungan Wave 3 (Evil Wizard Miniboss & Flying Eyes)
        MinibossOutro,       // Dialog 4: Monolog Alaric setelah miniboss kalah
        Wave4Combat,         // Pertarungan Wave 4 (Creeps di lantai bawah setelah tangga terbuka)
        Wave5Combat,         // Pertarungan Wave 5 (Creeps di Boss Room sebelum boss)
        MainBossEncounter,   // Dialog 5: Pertemuan Alaric dengan Boss Utama di bawah
        Wave6Boss,           // Pertarungan Wave 6 (Main Boss & Creeps saling bertarung)
        MainBossOutro,       // Dialog 6: Monolog Akhir Alaric setelah Boss Utama kalah
        DungeonCompleted     // Portal transisi level berikutnya aktif
    }

    [Header("Current Status")]
    [SerializeField] private DungeonPhase m_currentPhase = DungeonPhase.IntroMonologue;

    [Header("Wave 1 Enemies (Upper Area)")]
    [SerializeField] private List<GameObject> m_wave1Enemies = new List<GameObject>();

    [Header("Wave 2 Enemies (Corridor)")]
    [SerializeField] private List<GameObject> m_wave2Enemies = new List<GameObject>();

    [Header("Wave 3 Enemies (Miniboss Room)")]
    [SerializeField] private List<GameObject> m_wave3Enemies = new List<GameObject>();

    [Header("Wave 4 Enemies (Lower Creeps)")]
    [SerializeField] private List<GameObject> m_wave4Enemies = new List<GameObject>();

    [Header("Wave 5 Enemies (Main Boss Room Creeps)")]
    [SerializeField] private List<GameObject> m_wave5Enemies = new List<GameObject>();

    [Header("Wave 6 Enemies (Main Boss Fight Creeps)")]
    [SerializeField] private List<GameObject> m_wave6Enemies = new List<GameObject>();

    [Header("Main Boss Object")]
    [SerializeField] private GameObject m_mainBossObject;

    [Header("Dialogue Triggers")]
    [SerializeField] private DialogueTrigger m_introMonologueTrigger;      // Dialog 1
    [SerializeField] private DialogueTrigger m_midMonologueTrigger;        // Dialog 2
    [SerializeField] private DialogueTrigger m_minibossEncounterTrigger;   // Dialog 3 (Pertemuan Miniboss)
    [SerializeField] private DialogueTrigger m_minibossOutroTrigger;       // Dialog 4 (Miniboss Outro)
    [SerializeField] private DialogueTrigger m_mainBossEncounterTrigger;   // Dialog 5 (Pertemuan Boss)
    [SerializeField] private DialogueTrigger m_mainBossOutroTrigger;       // Dialog 6 (Boss Outro)

    [Header("Buff Selection Manager")]
    [SerializeField] private BuffSelectionManager m_buffSelectionManager;

    [Header("Barriers & Exit Portals")]
    [SerializeField] private GameObject m_exitBarrier1; // Pembatas jalan ke lantai bawah (nonaktif setelah Dialog 4)
    [SerializeField] private GameObject m_exitPortal;   // Portal transisi scene ke Dungeon 2 (aktif setelah Dialog 6)

    private void Awake()
    {
        // 1. Matikan AI untuk Wave 1 di awal agar diam saat monolog
        foreach (var enemy in m_wave1Enemies)
        {
            SetEnemyAIActive(enemy, false);
        }

        // 2. Sembunyikan Wave 2, 3, 4, 5, & 6 musuh sepenuhnya di awal
        SetWaveActive(m_wave2Enemies, false);
        SetWaveActive(m_wave3Enemies, false);
        SetWaveActive(m_wave4Enemies, false);
        SetWaveActive(m_wave5Enemies, false);
        SetWaveActive(m_wave6Enemies, false);
        
        if (m_mainBossObject != null)
        {
            m_mainBossObject.SetActive(false);
            SetEnemyAIActive(m_mainBossObject, false);
        }

        // 3. Pasang pembatas jalan ke lantai bawah di awal
        SetBarrierActive(m_exitBarrier1, true);

        // 4. Matikan portal keluar transisi level di awal
        SetPortalActive(m_exitPortal, false);

        // 5. Daftarkan event callback untuk semua dialogue triggers
        if (m_introMonologueTrigger != null)
            m_introMonologueTrigger.OnDialogueComplete.AddListener(OnIntroMonologueComplete);

        if (m_buffSelectionManager != null)
            m_buffSelectionManager.OnBuffSelectionComplete.AddListener(OnBuffSelectionComplete);

        if (m_midMonologueTrigger != null)
            m_midMonologueTrigger.OnDialogueComplete.AddListener(OnMidMonologueComplete);

        if (m_minibossEncounterTrigger != null)
            m_minibossEncounterTrigger.OnDialogueComplete.AddListener(OnMinibossEncounterComplete);

        if (m_minibossOutroTrigger != null)
            m_minibossOutroTrigger.OnDialogueComplete.AddListener(OnMinibossOutroComplete);

        if (m_mainBossEncounterTrigger != null)
            m_mainBossEncounterTrigger.OnDialogueComplete.AddListener(OnMainBossEncounterComplete);

        if (m_mainBossOutroTrigger != null)
            m_mainBossOutroTrigger.OnDialogueComplete.AddListener(OnMainBossOutroComplete);
    }

    private void Start()
    {
        // Jalankan monolog intro di awal game
        if (m_introMonologueTrigger != null)
        {
            Debug.Log("[Dungeon1Manager] Memulai Intro Monologue (Dialog 1).");
            m_introMonologueTrigger.TriggerDialogue();
        }
        else
        {
            OnIntroMonologueComplete();
        }
    }

    private void Update()
    {
        switch (m_currentPhase)
        {
            case DungeonPhase.Wave1Combat:
                if (IsWaveDefeated(m_wave1Enemies))
                {
                    StartWave2();
                }
                break;

            case DungeonPhase.Wave2Combat:
                if (IsWaveDefeated(m_wave2Enemies))
                {
                    TriggerMinibossEncounter();
                }
                break;

            case DungeonPhase.Wave3Miniboss:
                if (IsWaveDefeated(m_wave3Enemies))
                {
                    m_currentPhase = DungeonPhase.MinibossOutro;
                    TriggerMinibossOutro();
                }
                break;

            case DungeonPhase.Wave4Combat:
                if (IsWaveDefeated(m_wave4Enemies))
                {
                    StartWave5();
                }
                break;

            case DungeonPhase.Wave5Combat:
                if (IsWaveDefeated(m_wave5Enemies))
                {
                    TriggerMainBossEncounter();
                }
                break;

            case DungeonPhase.Wave6Boss:
                if (IsWaveDefeated(m_wave6Enemies))
                {
                    m_currentPhase = DungeonPhase.MainBossOutro;
                    TriggerMainBossOutro();
                }
                break;
        }
    }

    // --- Phase Transitions ---

    private void OnIntroMonologueComplete()
    {
        if (m_currentPhase == DungeonPhase.IntroMonologue)
        {
            m_currentPhase = DungeonPhase.BuffSelection;
            Debug.Log("[Dungeon1Manager] Intro Monologue selesai. Membuka UI Buff Card...");
            if (m_buffSelectionManager != null)
            {
                m_buffSelectionManager.OpenBuffSelection();
            }
            else
            {
                OnBuffSelectionComplete();
            }
        }
    }

    private void OnBuffSelectionComplete()
    {
        if (m_currentPhase == DungeonPhase.BuffSelection)
        {
            m_currentPhase = DungeonPhase.MidMonologue;
            Debug.Log("[Dungeon1Manager] Buff Card dipilih. Memulai Mid Monologue (Dialog 2)...");
            if (m_midMonologueTrigger != null)
            {
                m_midMonologueTrigger.TriggerDialogue();
            }
            else
            {
                OnMidMonologueComplete();
            }
        }
    }

    private void OnMidMonologueComplete()
    {
        if (m_currentPhase == DungeonPhase.MidMonologue)
        {
            m_currentPhase = DungeonPhase.Wave1Combat;
            Debug.Log("[Dungeon1Manager] Mid Monologue selesai. Mulai Wave 1 Combat!");

            foreach (var enemy in m_wave1Enemies)
            {
                SetEnemyAIActive(enemy, true);
            }
        }
    }

    private void StartWave2()
    {
        m_currentPhase = DungeonPhase.Wave2Combat;
        Debug.Log("[Dungeon1Manager] Wave 1 kalah. Memulai Wave 2 Combat!");
        SetWaveActive(m_wave2Enemies, true);
    }

    private void TriggerMinibossEncounter()
    {
        m_currentPhase = DungeonPhase.MinibossEncounter;
        Debug.Log("[Dungeon1Manager] Wave 2 kalah. Memulai Miniboss Encounter (Dialog 3)...");

        // Spawn musuh Wave 3 dalam keadaan pasif terlebih dahulu
        SetWaveActive(m_wave3Enemies, true, false);

        if (m_minibossEncounterTrigger != null)
        {
            m_minibossEncounterTrigger.TriggerDialogue();
        }
        else
        {
            OnMinibossEncounterComplete();
        }
    }

    private void OnMinibossEncounterComplete()
    {
        if (m_currentPhase == DungeonPhase.MinibossEncounter)
        {
            m_currentPhase = DungeonPhase.Wave3Miniboss;
            Debug.Log("[Dungeon1Manager] Dialog Pertemuan Miniboss selesai. Memulai Wave 3 Combat!");

            foreach (var enemy in m_wave3Enemies)
            {
                if (enemy != null) SetEnemyAIActive(enemy, true);
            }
        }
    }

    private void TriggerMinibossOutro()
    {
        Debug.Log("[Dungeon1Manager] Miniboss dikalahkan. Memicu Miniboss Outro (Dialog 4)...");
        if (m_minibossOutroTrigger != null)
        {
            m_minibossOutroTrigger.TriggerDialogue();
        }
        else
        {
            OnMinibossOutroComplete();
        }
    }

    private void OnMinibossOutroComplete()
    {
        if (m_currentPhase == DungeonPhase.MinibossOutro)
        {
            m_currentPhase = DungeonPhase.Wave4Combat;
            Debug.Log("[Dungeon1Manager] Miniboss Outro selesai. Membuka jalan tangga ke bawah & Mengaktifkan Wave 4!");

            // Buka jalan tangga ke bawah (matikan barrier 1)
            SetBarrierActive(m_exitBarrier1, false);

            // Aktifkan musuh-musuh Wave 4 di lantai bawah
            SetWaveActive(m_wave4Enemies, true);
        }
    }

    private void StartWave5()
    {
        m_currentPhase = DungeonPhase.Wave5Combat;
        Debug.Log("[Dungeon1Manager] Wave 4 kalah. Memulai Wave 5 Combat!");
        SetWaveActive(m_wave5Enemies, true);
    }

    private void TriggerMainBossEncounter()
    {
        m_currentPhase = DungeonPhase.MainBossEncounter;
        Debug.Log("[Dungeon1Manager] Wave 5 kalah. Memulai Main Boss Encounter (Dialog 5)...");

        // Hiduplah si boss di scene tapi pasif dulu
        if (m_mainBossObject != null)
        {
            m_mainBossObject.SetActive(true);
            SetEnemyAIActive(m_mainBossObject, false);
        }

        if (m_mainBossEncounterTrigger != null)
        {
            m_mainBossEncounterTrigger.TriggerDialogue();
        }
        else
        {
            OnMainBossEncounterComplete();
        }
    }

    private void OnMainBossEncounterComplete()
    {
        if (m_currentPhase == DungeonPhase.MainBossEncounter)
        {
            m_currentPhase = DungeonPhase.Wave6Boss;
            Debug.Log("[Dungeon1Manager] Dialog Pertemuan Boss selesai. Memulai Wave 6 Combat (Boss & Creeps)!");

            if (m_mainBossObject != null)
            {
                if (!m_wave6Enemies.Contains(m_mainBossObject))
                {
                    m_wave6Enemies.Add(m_mainBossObject);
                }
                SetEnemyAIActive(m_mainBossObject, true);
            }

            SetWaveActive(m_wave6Enemies, true);
        }
    }

    private void TriggerMainBossOutro()
    {
        Debug.Log("[Dungeon1Manager] Boss Utama dikalahkan. Memicu Main Boss Outro (Dialog 6)...");
        if (m_mainBossOutroTrigger != null)
        {
            m_mainBossOutroTrigger.TriggerDialogue();
        }
        else
        {
            OnMainBossOutroComplete();
        }
    }

    private void OnMainBossOutroComplete()
    {
        if (m_currentPhase == DungeonPhase.MainBossOutro)
        {
            m_currentPhase = DungeonPhase.DungeonCompleted;
            Debug.Log("[Dungeon1Manager] Dungeon 1 selesai secara keseluruhan! Portal transisi level berikutnya aktif.");

            // Aktifkan portal perpindahan scene
            SetPortalActive(m_exitPortal, true);
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
                    return false;
                }
            }
        }
        return true;
    }

    private void SetWaveActive(List<GameObject> enemies, bool activeState, bool enableAI = true)
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.SetActive(activeState);
                if (activeState)
                {
                    SetEnemyAIActive(enemy, enableAI);
                }
            }
        }
    }

    private void SetBarrierActive(GameObject barrier, bool active)
    {
        if (barrier == null) return;

        // Jika active = true, berarti pembatas menghalangi player
        // Jika active = false, berarti pembatas tidak aktif (jalan terbuka)
        if (barrier.TryGetComponent<Collider2D>(out var col))
        {
            col.enabled = active;
        }
        else
        {
            barrier.SetActive(active);
        }
    }

    private void SetPortalActive(GameObject portal, bool active)
    {
        if (portal == null) return;

        // Portal transisi scene diaktifkan/dinonaktifkan
        if (portal.TryGetComponent<Collider2D>(out var col))
        {
            col.enabled = active;
        }
        else if (portal.TryGetComponent<SceneTransitionTrigger>(out var trans))
        {
            trans.enabled = active;
        }
        else
        {
            portal.SetActive(active);
        }
    }

    private void SetEnemyAIActive(GameObject enemy, bool active)
    {
        if (enemy == null) return;

        // 1. Skeleton Roaming script
        if (enemy.TryGetComponent<Skeleton_roaming>(out var skeletonRoam))
            skeletonRoam.enabled = active;

        // 2. Flying Eye Roaming script (class: flyingEye_roaming)
        if (enemy.TryGetComponent<flyingEye_roaming>(out var flyingEyeRoam))
            flyingEyeRoam.enabled = active;

        // 3. Goblin Roaming script (class: Goblin_roaming)
        if (enemy.TryGetComponent<Goblin_roaming>(out var goblinRoam))
            goblinRoam.enabled = active;

        // 4. Mushroom Roaming script (class: Mushroom_roaming)
        if (enemy.TryGetComponent<Mushroom_roaming>(out var mushroomRoam))
            mushroomRoam.enabled = active;

        // 5. Miniboss Combat script (class: Miniboss_Combat)
        if (enemy.TryGetComponent<Miniboss_Combat>(out var minibossRoam))
            minibossRoam.enabled = active;

        // 6. Bringer of Death Boss Combat script (class: BringerOfDeath_Combat)
        if (enemy.TryGetComponent<BringerOfDeath_Combat>(out var bossCombat))
            bossCombat.enabled = active;

        // 6. Rigidbody2D body type
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
