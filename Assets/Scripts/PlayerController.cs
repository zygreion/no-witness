using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Testing Mode")]
    [SerializeField] bool _isTestingMode = false;

    [Header("Movement")]
    [SerializeField] float m_speed;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    [Header("Combat Settings")]
    [SerializeField] private Transform m_attackPoint;
    [SerializeField] private float m_attackRange = 1.2f;
    [SerializeField] private float m_attackDamage = 25.0f;
    [SerializeField] private LayerMask m_enemyLayers;

    // Public properties for external access
    public bool IsRolling => m_isRolling;
    public bool IsBlocking => m_isBlocking;
    public static PlayerController Instance { get; private set; }
    private bool m_isDialogueLocked = false;
    public bool IsDialogueLocked => m_isDialogueLocked;

    public void LockPlayerControl(bool isLocked)
    {
        m_isDialogueLocked = isLocked;
        if (isLocked)
        {
            if (m_body2d != null) m_body2d.velocity = Vector2.zero;
            if (m_animator != null) m_animator.SetInteger("AnimState", 0);
        }
    }

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private bool m_isRolling = false;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    // Health related
    private HealthPlayer m_healthPlayer;

    // Stamina related
    [Header("Stamina")]
    [SerializeField] private float m_maxStamina = 50.0f;
    [SerializeField] private float m_currentStamina = 50.0f;
    [SerializeField] private RawImage m_staminaGfx;
    private RectTransform m_staminaGfxTransform;
    private float k_maxWidth;
    private float m_staminaRegenDuration = 1.0f; // Regen every 1.0f second
    private float m_staminaRegenCurrentTime = 0.0f;
    private bool m_isStaminaRegen = false;
    private const float k_staminaRegenAmount = 0.5f;

    private Dictionary<string, float> actionStamina = new()
    {
        {"Attack", 1.0f},
        {"Block", 2.0f},
        {"BlockStill", 1.0f / 256.0f},
        {"Roll", 4.0f}
    };

    // Block
    private bool m_isBlocking = false;

    private void Awake()
    {
        Instance = this;
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        m_animator.SetBool("Grounded", true);
        m_healthPlayer = GetComponent<HealthPlayer>();

        // Set player sorting order to 5 so they render in front of stairs/props on the same layer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 5;
        }

        // Sync sorting layer with physics layer on start to prevent startup visual glitches on stairs
        string currentLayerName = LayerMask.LayerToName(gameObject.layer);
        if (currentLayerName == "Layer 1" || currentLayerName == "Layer 2")
        {
            if (sr != null)
            {
                sr.sortingLayerName = currentLayerName;
            }
            SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer childSr in srs)
            {
                childSr.sortingLayerName = currentLayerName;
            }
        }

        if (m_staminaGfx == null) return;
        m_staminaGfxTransform = m_staminaGfx.GetComponent<RectTransform>();
        k_maxWidth = ((RectTransform)m_staminaGfx.transform.parent).rect.width;
    }

    private void Update()
    {
        if (m_isDialogueLocked)
        {
            if (m_body2d != null) m_body2d.velocity = Vector2.zero;
            if (m_animator != null) m_animator.SetInteger("AnimState", 0);
            return;
        }

        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if (m_isRolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
        {
            m_isRolling = false;
            m_rollCurrentTime = 0.0f;
        }

        RegenStamina();


        // -- Handle input and movement --
        Vector2 dir = Vector2.zero;
        if (!m_healthPlayer.IsDead && !m_isRolling)
        {
            if (Input.GetKey(KeyCode.A))
            {
                dir.x = -1;
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                dir.x = 1;
                GetComponent<SpriteRenderer>().flipX = false;
            }

            if (Input.GetKey(KeyCode.W))
            {
                dir.y = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                dir.y = -1;
            }
        }

        // Move
        dir.Normalize();
        if (!m_isRolling)
        {
            float currentSpeed = m_speed;
            if (BuffApplier.Instance != null)
            {
                currentSpeed *= BuffApplier.Instance.speedMultiplier;
            }
            m_body2d.velocity = currentSpeed * dir;
        }

        if (!m_healthPlayer.IsDead)
        {
            //Death
            if (_isTestingMode && Input.GetKeyDown(KeyCode.E) && !m_isRolling)
            {
                m_animator.SetBool("noBlood", m_noBlood);
                m_animator.SetTrigger("Death");
            }

            //Hurt
            else if (_isTestingMode && Input.GetKeyDown(KeyCode.Q) && !m_isRolling)
                m_healthPlayer.TakeDamage(10);

            //Attack
            else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_isRolling)
            {
                if (!TryUseStamina("Attack")) return;

                m_currentAttack++;

                // Loop back to one after third attack
                if (m_currentAttack > 3)
                    m_currentAttack = 1;

                // Reset Attack combo if time since last attack is too large
                if (m_timeSinceAttack > 1.0f)
                    m_currentAttack = 1;

                // Call one of three attack animations "Attack1", "Attack2", "Attack3"
                m_animator.SetTrigger("Attack" + m_currentAttack);

                // Perform combat hit detection
                PerformAttackDetection();

                // Reset timer
                m_timeSinceAttack = 0.0f;
            }

            // Block
            else if (Input.GetMouseButtonDown(1) && !m_isRolling)
            {
                if (!TryUseStamina("Block")) return;

                m_isBlocking = true;
                m_animator.SetTrigger("Block");
                m_animator.SetBool("IdleBlock", true);
            }

            else if (Input.GetMouseButton(1) && m_isBlocking)
            {
                if (!TryUseStamina("BlockStill"))
                {
                    m_isBlocking = false;
                    m_animator.SetBool("IdleBlock", false);
                }
            }

            else if (Input.GetMouseButtonUp(1))
            {
                m_isBlocking = false;
                m_animator.SetBool("IdleBlock", false);
            }

            // Roll
            else if (Input.GetKeyDown(KeyCode.Space) && !m_isRolling)
            {
                if (!TryUseStamina("Roll")) return;

                m_isRolling = true;
                m_animator.SetTrigger("Roll");
                m_body2d.velocity = new Vector2(dir.x * m_rollForce, m_body2d.velocity.y);
            }


            //Run
            else if (
                Mathf.Abs(dir.x) > Mathf.Epsilon ||     // Horizontal
                Mathf.Abs(dir.y) > Mathf.Epsilon)       // Vertical
            {
                // Reset timer
                m_delayToIdle = 0.05f;
                m_animator.SetInteger("AnimState", 1);
            }

            //Idle
            else
            {
                // Prevents flickering transitions to idle
                m_delayToIdle -= Time.deltaTime;
                if (m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
            }
        }

        if (_isTestingMode && Input.GetKeyDown(KeyCode.R))
        {
            m_healthPlayer.Revive();
            m_currentStamina = m_maxStamina;
            UpdateStaminaGfx();
        }
    }

    // ----- Stamina -----
    private void TakeStamina(float amount)
    {
        m_currentStamina = Mathf.Clamp(m_currentStamina - amount, 0, m_maxStamina);
        m_isStaminaRegen = true;
        UpdateStaminaGfx();
    }

    private bool TryUseStamina(string action)
    {
        if (actionStamina.TryGetValue(action, out float cost) && m_currentStamina >= cost)
        {
            TakeStamina(cost);
            return true;
        }

        return false;
    }

    // Every 1 second regenerate stamina by a certain amount
    private void RegenStamina()
    {
        // Cannot regen while block still
        if (!m_isStaminaRegen || m_isBlocking) return;

        // Add timer
        m_staminaRegenCurrentTime += Time.deltaTime;

        // Reset timer when surpass certain duration
        if (m_staminaRegenCurrentTime > m_staminaRegenDuration)
        {
            m_currentStamina = Mathf.Clamp(m_currentStamina + k_staminaRegenAmount, 0, m_maxStamina);
            m_staminaRegenCurrentTime = 0.0f;

            // Disable stamina regen
            if (m_currentStamina >= m_maxStamina)
                m_isStaminaRegen = false;

            UpdateStaminaGfx();
        }
    }

    private void UpdateStaminaGfx()
    {
        if (m_staminaGfx == null) return;

        float newWidth = m_currentStamina / m_maxStamina * k_maxWidth;
        m_staminaGfxTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }

    private void PerformAttackDetection()
    {
        if (m_attackPoint == null) return;

        // Calculate attack position based on player facing direction (flipX)
        Vector3 attackPos = m_attackPoint.position;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.flipX)
        {
            // Mirror the local X offset if facing left
            Vector3 localOffset = m_attackPoint.localPosition;
            attackPos = transform.TransformPoint(new Vector3(-localOffset.x, localOffset.y, localOffset.z));
        }

        // Detect all enemies in range of attack point
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, m_attackRange, m_enemyLayers);
        Debug.Log("Attacking! Detected " + hitEnemies.Length + " colliders in range.");

        // Keep track of already damaged targets to prevent double damage
        HashSet<Health> damagedTargets = new HashSet<Health>();

        // Calculate buff damage multiplier if applicable
        float finalDamage = m_attackDamage;
        if (BuffApplier.Instance != null)
        {
            finalDamage *= BuffApplier.Instance.attackMultiplier;
        }

        // Damage each enemy
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent<Health>(out var enemyHealth))
            {
                if (!damagedTargets.Contains(enemyHealth))
                {
                    enemyHealth.TakeDamage(finalDamage);
                    damagedTargets.Add(enemyHealth);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (m_attackPoint == null) return;

        Vector3 attackPos = m_attackPoint.position;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.flipX)
        {
            Vector3 localOffset = m_attackPoint.localPosition;
            attackPos = transform.TransformPoint(new Vector3(-localOffset.x, localOffset.y, localOffset.z));
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, m_attackRange);
    }
}
