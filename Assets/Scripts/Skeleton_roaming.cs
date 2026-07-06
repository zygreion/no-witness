using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Skeleton_roaming : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 10f;   // how far it walks in each direction
    [SerializeField] private float waitTime = 1f;         // pause duration at each end point
 
    [Header("Combat Settings")]
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 1.3f;
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackCooldown = 1.5f;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private bool movingRight = true;
    private float waitTimer = 0f;
    private bool isWaiting = false;
 
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 originalScale;
    
    private Transform playerTransform;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
 
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        startPosition = transform.position;
        SetNextTarget();
    }

    void Start()
    {
        // Automatically find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }
 
    void FixedUpdate()
    {
        // Don't do anything if dead (handled by EnemyHealth)
        if (TryGetComponent<EnemyHealth>(out var eh) && eh.IsDead) return;

        // If player exists and is alive, evaluate distance
        if (playerTransform != null)
        {
            HealthPlayer playerHealth = playerTransform.GetComponent<HealthPlayer>();
            if (playerHealth != null && !playerHealth.IsDead)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

                if (distanceToPlayer <= chaseRange)
                {
                    // Chase and attack behavior
                    isWaiting = false; // Interrupt patrol wait

                    if (distanceToPlayer <= attackRange)
                    {
                        // Stop moving and attack
                        rb.velocity = Vector2.zero;
                        animator.SetBool("isWalking", false);

                        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
                        {
                            StartCoroutine(AttackSequence());
                        }
                    }
                    else if (!isAttacking)
                    {
                        // Move toward player
                        MoveTowardTarget(playerTransform.position);
                    }
                    return; // Skip normal patrol logic
                }
            }
        }

        // --- Normal Patrol Logic ---
        if (isAttacking) return;

        if (isWaiting)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            waitTimer -= Time.fixedDeltaTime;
 
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                movingRight = !movingRight;
                SetNextTarget();
            }
            return;
        }
 
        MoveTowardTarget(targetPosition);
 
        if (Vector2.Distance(transform.position, targetPosition) < 0.3f)
        {
            isWaiting = true;
            waitTimer = waitTime;
        }
    }
 
    private void SetNextTarget()
    {
        float direction = movingRight ? 1f : -1f;
        targetPosition = startPosition + new Vector2(direction * patrolDistance, 0f);
    }
 
    private void MoveTowardTarget(Vector2 destination)
    {
        Vector2 direction = (destination - (Vector2)transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        animator.SetBool("isWalking", true);
 
        // Flip sprite to face movement direction
        if (direction.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direction.x) * originalScale.x, originalScale.y, originalScale.z);
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        animator.SetBool("isWalking", false);

        // Face the player before attacking
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        if (directionToPlayer.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(directionToPlayer.x) * originalScale.x, originalScale.y, originalScale.z);
        }

        // Trigger attack animation
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;

        // Delay to sync damage with sword swing visual (approx 0.3 seconds)
        yield return new WaitForSeconds(0.3f);

        // Check if player is still in range to apply damage
        if (playerTransform != null)
        {
            HealthPlayer playerHealth = playerTransform.GetComponent<HealthPlayer>();
            if (playerHealth != null && !playerHealth.IsDead)
            {
                float finalDistance = Vector2.Distance(transform.position, playerTransform.position);
                if (finalDistance <= attackRange)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
            }
        }

        // Wait a little before enabling movement again (recovery frames)
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }
 
    // Draw patrol range & aggro range in the editor for easy tuning
    private void OnDrawGizmosSelected()
    {
        Vector2 origin = Application.isPlaying ? startPosition : (Vector2)transform.position;
        
        // Patrol range (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin + Vector2.left * patrolDistance, origin + Vector2.right * patrolDistance);
        Gizmos.DrawWireSphere(origin + Vector2.left  * patrolDistance, 0.15f);
        Gizmos.DrawWireSphere(origin + Vector2.right * patrolDistance, 0.15f);

        // Aggro range (orange)
        Gizmos.color = new Color(1f, 0.6f, 0f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Attack range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector2.zero;
            movingRight = !movingRight;
            SetNextTarget();
        }
    }
}