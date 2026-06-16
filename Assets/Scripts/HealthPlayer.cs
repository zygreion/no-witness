using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPlayer : Health
{
    [Header("Player")]
    [SerializeField] private RawImage m_healthGfx;
    [SerializeField] [Range(0f, 1f)] private float m_blockDamageMultiplier = 0.2f; // Takes 20% damage when blocking
    
    private RectTransform m_healthGfxTransform;
    private float k_maxWidth;
    private PlayerController m_playerController;

    private void Start()
    {
        m_playerController = GetComponent<PlayerController>();

        if (m_healthGfx == null) return;

        m_healthGfxTransform = m_healthGfx.GetComponent<RectTransform>();
        k_maxWidth = ((RectTransform)m_healthGfx.transform.parent).rect.width;
    }

    private void UpdateHealthGfx()
    {
        if (m_healthGfx == null) return;

        float newWidth = m_currentHealth / m_maxHealth * k_maxWidth;
        m_healthGfxTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }

    public override void TakeDamage(float dmgAmount)
    {
        if (IsDead) return;

        // Roll I-Frames (Invulnerability)
        if (m_playerController != null && m_playerController.IsRolling)
        {
            return; // Ignore damage during roll
        }

        // Block Damage Mitigation
        if (m_playerController != null && m_playerController.IsBlocking)
        {
            dmgAmount *= m_blockDamageMultiplier;
        }

        base.TakeDamage(dmgAmount);

        if (m_currentHealth > 0)
        {
            m_animator.SetTrigger("Hurt");
        }
        else
        {
            IsDead = true;
            m_animator.SetTrigger("Death");
        }

        UpdateHealthGfx();
    }

    public override void Revive()
    {
        base.Revive();
        m_animator.SetInteger("AnimState", 3);
        UpdateHealthGfx();
    }
}
