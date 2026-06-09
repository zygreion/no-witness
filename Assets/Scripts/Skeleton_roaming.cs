using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Skeleton_roaming : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 10f;   // how far it walks in each direction
    [SerializeField] private float waitTime = 1f;         // pause duration at each end point
 
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private bool movingRight = true;
    private float waitTimer = 0f;
    private bool isWaiting = false;
 
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 originalScale;
 
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        startPosition = transform.position;
    SetNextTarget();
    }
 
    void FixedUpdate()
    {
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
 
        MoveTowardTarget();
 
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
 
    private void MoveTowardTarget()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        animator.SetBool("isWalking", true);
 
        // Flip sprite to face movement direction
        if (direction.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direction.x) * originalScale.x, originalScale.y, originalScale.z);
    }
 
    // Draw patrol range in the editor for easy tuning
    private void OnDrawGizmosSelected()
    {
        Vector2 origin = Application.isPlaying ? startPosition : (Vector2)transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin + Vector2.left * patrolDistance, origin + Vector2.right * patrolDistance);
        Gizmos.DrawWireSphere(origin + Vector2.left  * patrolDistance, 0.15f);
        Gizmos.DrawWireSphere(origin + Vector2.right * patrolDistance, 0.15f);
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