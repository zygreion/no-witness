using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    [Header("UI")]
    [SerializeField] private UnityEngine.UI.Slider m_healthSlider;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (m_healthSlider != null)
        {
            m_healthSlider.maxValue = 1.0f;
            m_healthSlider.value = m_currentHealth / m_maxHealth;
        }
    }

    public override void TakeDamage(float dmgAmount)
    {
        if (IsDead) return;

        base.TakeDamage(dmgAmount);
        Debug.Log(gameObject.name + " took " + dmgAmount + " damage. Current HP: " + m_currentHealth);

        if (m_healthSlider != null)
        {
            m_healthSlider.value = m_currentHealth / m_maxHealth;
        }

        if (m_currentHealth > 0)
        {
            if (m_animator != null)
            {
                m_animator.SetTrigger("Hurt");
            }
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;

        if (m_animator != null)
        {
            m_animator.SetTrigger("Death");
        }

        // Hide health bar UI on death
        if (m_healthSlider != null)
        {
            m_healthSlider.gameObject.SetActive(false);
        }

        // Disable movement & physics interactions
        if (TryGetComponent<Collider2D>(out var col))
        {
            col.enabled = false;
        }

        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Disable the roaming component
        if (TryGetComponent<Skeleton_roaming>(out var roam))
        {
            roam.enabled = false;
        }
    }

    public override void Revive()
    {
        base.Revive();

        // Enable health bar UI
        if (m_healthSlider != null)
        {
            m_healthSlider.gameObject.SetActive(true);
            m_healthSlider.value = m_currentHealth / m_maxHealth;
        }

        // Enable components again
        if (TryGetComponent<Collider2D>(out var col))
        {
            col.enabled = true;
        }

        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        if (TryGetComponent<Skeleton_roaming>(out var roam))
        {
            roam.enabled = true;
        }
    }
}
