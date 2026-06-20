using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float m_speed;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

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
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        m_animator.SetBool("Grounded", true);
        m_healthPlayer = GetComponent<HealthPlayer>();

        if (m_staminaGfx == null) return;
        m_staminaGfxTransform = m_staminaGfx.GetComponent<RectTransform>();
        k_maxWidth = ((RectTransform)m_staminaGfx.transform.parent).rect.width;
    }

    private void Update()
    {
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
            m_body2d.velocity = m_speed * dir;

        if (!m_healthPlayer.IsDead)
        {
            //Death
            if (Input.GetKeyDown(KeyCode.E) && !m_isRolling)
            {
                m_animator.SetBool("noBlood", m_noBlood);
                m_animator.SetTrigger("Death");
            }

            //Hurt
            else if (Input.GetKeyDown(KeyCode.Q) && !m_isRolling)
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            m_healthPlayer.Revive();
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
}
