using UnityEngine;

// Jembatan antara Buff Selection dan Player, tanpa mengubah script combat/movement
public class BuffApplier : MonoBehaviour
{
    public static BuffApplier Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<BuffApplier>();
                if (m_instance == null)
                {
                    GameObject go = new GameObject("BuffApplierManager");
                    m_instance = go.AddComponent<BuffApplier>();
                    Debug.Log("[BuffApplier] Menghasilkan BuffApplierManager secara otomatis karena tidak ditemukan di Scene.");
                }
            }
            return m_instance;
        }
    }
    private static BuffApplier m_instance;

    [Header("Stats hasil Buff (disimpan di sini)")]
    public float attackMultiplier = 1f;
    public float speedMultiplier = 1f;

    private Health playerHealth;

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private Health GetPlayerHealth()
    {
        if (playerHealth == null)
        {
            // First check if it is attached directly to the Player
            playerHealth = GetComponent<Health>();

            // If not, find the GameObject with the "Player" tag
            if (playerHealth == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerHealth = player.GetComponent<Health>();
                }
            }
        }
        return playerHealth;
    }

    void Start()
    {
        GetPlayerHealth();
    }

    public void ApplyBuff(BuffData buff)
    {
        attackMultiplier *= buff.attackMultiplier;
        speedMultiplier  *= buff.speedMultiplier;

        Health hp = GetPlayerHealth();
        if (buff.bonusHP > 0f && hp != null)
        {
            hp.IncreaseMaxHealth(buff.bonusHP);
        }

        Debug.Log($"[Buff Applied] {buff.buffName} | ATK x{attackMultiplier} SPD x{speedMultiplier}");
    }

    // Dipanggil saat player memungut Potion
    public void Heal(float amount)
    {
        Health hp = GetPlayerHealth();
        if (hp != null)
        {
            hp.Heal(amount);
            Debug.Log($"[Heal] +{amount} HP");
        }
    }
}