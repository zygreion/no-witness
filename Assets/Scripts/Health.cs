using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] protected float m_maxHealth = 100.0f;
    [SerializeField] protected float m_currentHealth = 100.0f;
    protected Animator m_animator;
    public bool IsDead { get; protected set; } = false;

    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public virtual void TakeDamage(float dmgAmount)
    {
        m_currentHealth = Mathf.Clamp(m_currentHealth - dmgAmount, 0, m_maxHealth);
    }

    public virtual void Revive()
    {
        m_currentHealth = m_maxHealth;
        IsDead = false;
    }

    public virtual void Heal(float amount)
{
    m_currentHealth = Mathf.Clamp(m_currentHealth + amount, 0, m_maxHealth);
}

public virtual void IncreaseMaxHealth(float amount)
{
    m_maxHealth += amount;
    m_currentHealth = m_maxHealth;
}
}
