using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class flyingEye_roaming : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 10f;
    [SerializeField] private float waitTime = 1f;

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
    private Slider healthSlider;

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
                    isWaiting = false;

                    if (distanceToPlayer <= attackRange)
                    {
                        // In attack range — stop and attack
                        rb.velocity = Vector2.zero;

                        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
                            StartCoroutine(AttackSequence());
                    }
                    else if (isAttacking)
                    {
                        // Mid-attack, out of attack range — hover in place
                        rb.velocity = Vector2.zero;
                    }
                    else
                    {
                        // Not attacking — chase player
                        MoveTowardTarget(playerTransform.position);
                    }

                    return;
                }
            }
        }

        // --- Normal Patrol Logic ---
        if (isAttacking) return;

        if (isWaiting)
        {
            rb.velocity = Vector2.zero;
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

        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        if (directionToPlayer.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(directionToPlayer.x) * originalScale.x, originalScale.y, originalScale.z);

        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;

        yield return new WaitForSeconds(0.3f);

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
        Vector2 origin = Application.isPlaying ? startPosition : (Vector2)transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin + Vector2.left * patrolDistance, origin + Vector2.right * patrolDistance);
        Gizmos.DrawWireSphere(origin + Vector2.left * patrolDistance, 0.15f);
        Gizmos.DrawWireSphere(origin + Vector2.right * patrolDistance, 0.15f);

        Gizmos.color = new Color(1f, 0.6f, 0f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);

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