using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damageAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Health targetHealth = other.GetComponent<Health>();

        if (targetHealth != null && targetHealth.IsDead == false)
            targetHealth.TakeDamage(damageAmount);
    }
}
