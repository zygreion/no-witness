using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPlayer : Health
{
    [SerializeField] private RawImage m_healthGfx;
    private const float k_maxWidth = 200.0f;

    private void UpdateHealthGfx()
    {
        if (m_healthGfx == null) return;

        RectTransform rectTransform = m_healthGfx.GetComponent<RectTransform>();
        float newWidth = m_currentHealth / m_maxHealth * k_maxWidth;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        Debug.Log($"{gameObject.name}'s health: {m_currentHealth / m_maxHealth * 100:F0}%");
    }

    public override void TakeDamage(float dmgAmount)
    {
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
