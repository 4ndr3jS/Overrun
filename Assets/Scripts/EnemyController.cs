using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3.5f;

    [Header("Combat")]
    public int touchDamage = 10;
    public float attackCooldown = 2f;
    public float flashDuration = 0.15f;

    Rigidbody2D rb;
    Transform target;
    Vector2 moveDirection;
    Animator animator;

    private float lastAttackTime = -Mathf.Infinity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Debug.Log("does this work");
    }

    void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (target)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            moveDirection = direction;
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (target)
        {
            rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null)
            return;

        bool isWalking = moveDirection.sqrMagnitude > 0.01f;
        animator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            animator.SetFloat("InputX", moveDirection.x);
            animator.SetFloat("InputY", moveDirection.y);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Collision with: " + collision.collider.name + " tag: " + collision.collider.tag);

        if (!collision.collider.CompareTag("Player"))
            return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        Debug.Log("Attacking playerj!");
        lastAttackTime = Time.time;

        PlayerVitals.Instance?.TakeDamage(touchDamage);

        SpriteFlash flash = collision.collider.GetComponent<SpriteFlash>();
        Debug.Log("Flash component found: " + (flash != null));
        if (flash != null)
            flash.Flash(Color.white, flashDuration);
    }
}
