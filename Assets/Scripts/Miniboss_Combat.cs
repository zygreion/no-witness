using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Miniboss_Combat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float chaseRange = 6f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackCooldown = 1.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 originalScale;

    private Transform playerTransform;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private int lastAttack = 0; // tracks which attack was last used
    private Slider healthSlider;

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

        healthSlider = GetComponentInChildren<Slider>(true);
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
                    if (distanceToPlayer <= attackRange)
                    {
                        // In attack range — stop and attack
                        rb.velocity = Vector2.zero;
                        animator.SetBool("isRunning", false);

                        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
                            StartCoroutine(AttackSequence());
                    }
                    else if (isAttacking)
                    {
                        // Mid-attack — hover in place
                        rb.velocity = Vector2.zero;
                        animator.SetBool("isRunning", false);
                    }
                    else
                    {
                        // Chase player
                        MoveTowardPlayer();
                    }
                    return;
                }
            }
        }

        // Player out of range — stand idle
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);
    }

    private void MoveTowardPlayer()
    {
        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        rb.velocity = direction * 3f;
        animator.SetBool("isRunning", true);

        if (direction.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction.x) * originalScale.x, originalScale.y, originalScale.z);

            Slider.Direction sliderDir = direction.x > 0 ? Slider.Direction.LeftToRight : Slider.Direction.RightToLeft;
            if (healthSlider != null)
                healthSlider.SetDirection(sliderDir, false);
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);

        // Face the player
        Vector2 directionToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        if (directionToPlayer.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(directionToPlayer.x) * originalScale.x, originalScale.y, originalScale.z);

        // Alternate between Attack1 and Attack2
        lastAttack = lastAttack == 1 ? 2 : 1;
        animator.SetTrigger("Attack" + lastAttack);
        lastAttackTime = Time.time;

        yield return new WaitForSeconds(0.3f);

        // Apply damage if player still in range
        if (playerTransform != null)
        {
            HealthPlayer playerHealth = playerTransform.GetComponent<HealthPlayer>();
            if (playerHealth != null && !playerHealth.IsDead)
            {
                float finalDistance = Vector2.Distance(transform.position, playerTransform.position);
                if (finalDistance <= attackRange)
                    playerHealth.TakeDamage(attackDamage);
            }
        }

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.6f, 0f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}