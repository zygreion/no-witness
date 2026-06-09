using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] protected float m_maxHealth, m_currentHealth = 100.0f;
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
}
