using UnityEngine;

// Jembatan antara Buff Selection dan Player, tanpa mengubah script combat/movement
public class BuffApplier : MonoBehaviour
{
    public static BuffApplier Instance;

    [Header("Stats hasil Buff (disimpan di sini)")]
    public float attackMultiplier = 1f;
    public float speedMultiplier = 1f;

    private Health playerHealth;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerHealth = GetComponent<Health>();
    }

    public void ApplyBuff(BuffData buff)
    {
        attackMultiplier *= buff.attackMultiplier;
        speedMultiplier  *= buff.speedMultiplier;

        if (buff.bonusHP > 0f && playerHealth != null)
        {
            playerHealth.IncreaseMaxHealth(buff.bonusHP);
        }

        Debug.Log($"[Buff Applied] {buff.buffName} | ATK x{attackMultiplier} SPD x{speedMultiplier}");
    }

    // Dipanggil saat player memungut Potion
    public void Heal(float amount)
    {
        if (playerHealth != null)
        {
            playerHealth.Heal(amount);
            Debug.Log($"[Heal] +{amount} HP");
        }
    }
}