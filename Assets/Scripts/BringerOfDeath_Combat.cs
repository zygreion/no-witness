using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerOfDeath_Combat : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float chaseRange = 8f;

    [Header("Melee Attack")]
    [SerializeField] private float meleeRange = 1.8f;
    [SerializeField] private float meleeDamage = 30f;
    [SerializeField] private float meleeCooldown = 1.5f;

    [Header("Spell Attack")]
    [SerializeField] private float spellDamage = 25f;
    [SerializeField] private float spellCooldown = 4f;
    [SerializeField] private float spellAoeRadius = 2f;
    [SerializeField] private float castDuration = 0.8f;   // how long Cast animation plays before Spell
    [SerializeField] private float spellImpactDelay = 0.15f; // delay after lock before damage lands
    [SerializeField] private float spellOffset = 3f; // how far in front the spell lands
    [SerializeField] private float spellOffsetY = 0f; // vertical offset for spell position
    [SerializeField] private float meleeOffsetX = 0f;
    [SerializeField] private float meleeOffsetY = 0f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 originalScale;

    private Transform playerTransform;
    private float lastMeleeTime = 0f;
    private float lastSpellTime = 0f;
    private bool isAttacking = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (TryGetComponent<EnemyHealth>(out var eh) && eh.IsDead) return;

        if (playerTransform != null)
        {
            HealthPlayer playerHealth = playerTransform.GetComponent<HealthPlayer>();
            if (playerHealth != null && !playerHealth.IsDead)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

                if (distanceToPlayer <= chaseRange)
                {
                    if (isAttacking)
                    {
                        // Mid-attack — stay in place
                        rb.velocity = Vector2.zero;
                        animator.SetBool("isWalking", false);
                        return;
                    }

                    // Decide what to do
                    bool canMelee = distanceToPlayer <= meleeRange && Time.time >= lastMeleeTime + meleeCooldown;
                    bool canSpell = Time.time >= lastSpellTime + spellCooldown;

                    if (canMelee && canSpell)
                    {
                        // Both available — pick randomly
                        if (Random.value < 0.5f)
                            StartCoroutine(MeleeSequence());
                        else
                            StartCoroutine(SpellSequence());
                    }
                    else if (canMelee && distanceToPlayer <= meleeRange)
                    {
                        StartCoroutine(MeleeSequence());
                    }
                    else if (canSpell)
                    {
                        StartCoroutine(SpellSequence());
                    }
                    else
                    {
                        // Nothing available — chase or stand
                        if (distanceToPlayer > meleeRange)
                            MoveTowardPlayer();
                        else
                        {
                            rb.velocity = Vector2.zero;
                            animator.SetBool("isWalking", false);
                        }
                    }

                    return;
                }
            }
        }

        // Player out of range — stand idle
        rb.velocity = Vector2.zero;
        animator.SetBool("isWalking", false);
    }

    private void MoveTowardPlayer()
    {
        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        animator.SetBool("isWalking", true);

        if (direction.x != 0)
            transform.localScale = new Vector3(-Mathf.Sign(direction.x) * originalScale.x, originalScale.y, originalScale.z);
    }

    private IEnumerator MeleeSequence()
{
    isAttacking = true;
    rb.velocity = Vector2.zero;
    animator.SetBool("isWalking", false);

    FacePlayer();
    animator.SetTrigger("Attack");
    lastMeleeTime = Time.time;

    yield return new WaitForSeconds(0.3f);

    // Apply melee damage at offset position
    float facingDirection = Mathf.Sign(transform.localScale.x);
    Vector2 meleeTarget = (Vector2)transform.position + new Vector2(facingDirection * meleeOffsetX, meleeOffsetY);

    Collider2D[] hits = Physics2D.OverlapCircleAll(meleeTarget, meleeRange);
    foreach (Collider2D hit in hits)
    {
        if (hit.TryGetComponent<HealthPlayer>(out var playerHealth))
        {
            if (!playerHealth.IsDead)
                playerHealth.TakeDamage(meleeDamage);
        }
    }

    yield return new WaitForSeconds(0.5f);
    isAttacking = false;
}

    private IEnumerator SpellSequence()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        animator.SetBool("isWalking", false);

        FacePlayer();

        // Play Cast windup
        animator.SetTrigger("Cast");
        lastSpellTime = Time.time;

        yield return new WaitForSeconds(castDuration);

        // Transition to Spell animation
        animator.SetTrigger("Spell");

        // Lock spell position at fixed offset in facing direction
        float facingDirection = Mathf.Sign(transform.localScale.x);
        Vector2 spellTarget = (Vector2)transform.position + new Vector2(facingDirection * spellOffset, spellOffsetY);

        // Position is now locked — wait for impact delay
        yield return new WaitForSeconds(spellImpactDelay);

        // Deal AOE damage at locked position
        Collider2D[] hits = Physics2D.OverlapCircleAll(spellTarget, spellAoeRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<HealthPlayer>(out var playerHealth))
            {
                if (!playerHealth.IsDead)
                    playerHealth.TakeDamage(spellDamage);
            }
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    private void FacePlayer()
    {
        if (playerTransform == null) return;
        Vector2 dir = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        if (dir.x != 0)
            transform.localScale = new Vector3(-Mathf.Sign(dir.x) * originalScale.x, originalScale.y, originalScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        Gizmos.color = new Color(1f, 0.6f, 0f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, spellAoeRadius);

        float facingDirection = Mathf.Sign(transform.localScale.x);
Gizmos.color = Color.cyan;
Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(facingDirection * spellOffset, 0f), spellAoeRadius);
Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(facingDirection * spellOffset, spellOffsetY), spellAoeRadius);

Gizmos.color = Color.red;
Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(facingDirection * meleeOffsetX, meleeOffsetY), meleeRange);
    }
}