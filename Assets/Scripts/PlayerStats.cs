using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Base Stats")]
    public float maxHP = 100f;
    public float currentHP;
    public float attackDamage = 10f;
    public float moveSpeed = 5f;

    [Header("Active Buff")]
    public BuffData activeBuff;

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

        currentHP = maxHP;
    }

    public void ApplyBuff(BuffData buff)
    {
        activeBuff = buff;

        attackDamage *= buff.attackMultiplier;
        moveSpeed    *= buff.speedMultiplier;
        maxHP        += buff.bonusHP;
        currentHP     = maxHP;

        Debug.Log($"[Buff Applied] {buff.buffName} | ATK:{attackDamage} SPD:{moveSpeed} HP:{maxHP}");
    }

    // Dipanggil saat player memungut Potion (heal)
    public void Heal(float amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        Debug.Log($"[Heal] +{amount} HP | Current HP: {currentHP}/{maxHP}");
    }
}